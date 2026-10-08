using Application.Interfaces;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repository.Contexts;
using Repository.Options;

namespace Repository.Services
{
    /// <summary>
    /// SQL-backed idempotency store. The primary key on <see cref="IdempotencyRecord.Key"/> makes the
    /// reservation atomic across API instances. Each operation uses its own DbContext scope so it never
    /// shares the change tracker with the request handler.
    /// </summary>
    internal sealed class IdempotencyStore(
        IServiceScopeFactory scopeFactory,
        IDateTimeService dateTimeService,
        IOptions<IdempotencyOptions> options) : IIdempotencyStore
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IDateTimeService _dateTimeService = dateTimeService;
        private readonly IdempotencyOptions _options = options.Value;

        public async Task<IdempotencyRecord?> TryBeginAsync(string key, string requestHash, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var records = context.Set<IdempotencyRecord>();
            var now = _dateTimeService.Now;

            var existing = await records.FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
            if (existing is not null)
            {
                if (!IsReusable(existing, now))
                    return existing;

                records.Remove(existing);
                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Another request already removed it; compete for the insert below.
                }
                context.ChangeTracker.Clear();
            }

            records.Add(new IdempotencyRecord
            {
                Key = key,
                RequestHash = requestHash,
                CreatedOn = now,
                ExpiresOn = now.Add(_options.Expiration),
            });
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return null;
            }
            catch (DbUpdateException)
            {
                // Lost the race against a concurrent request with the same key.
                context.ChangeTracker.Clear();
                return await records.AsNoTracking().FirstOrDefaultAsync(r => r.Key == key, cancellationToken)
                    ?? new IdempotencyRecord { Key = key, RequestHash = requestHash, CreatedOn = now, ExpiresOn = now };
            }
        }

        public async Task CompleteAsync(string key, int statusCode, string responseBody, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var record = await context.Set<IdempotencyRecord>().FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
            if (record is null)
                return;

            var now = _dateTimeService.Now;
            record.StatusCode = statusCode;
            record.ResponseBody = responseBody;
            record.CompletedOn = now;
            record.ExpiresOn = now.Add(_options.Expiration);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task ReleaseAsync(string key, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var record = await context.Set<IdempotencyRecord>().FirstOrDefaultAsync(r => r.Key == key, cancellationToken);
            if (record is null || record.IsCompleted)
                return;

            context.Remove(record);
            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Already removed by another request.
            }
        }

        private bool IsReusable(IdempotencyRecord record, DateTime now)
            => record.ExpiresOn <= now
               || (!record.IsCompleted && record.CreatedOn.Add(_options.InProgressTimeout) <= now);
    }
}
