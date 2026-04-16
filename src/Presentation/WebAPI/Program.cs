using Application.Extensions;
using Repository.Extensions;
using Repository.Seeders;
using Shared.Extensions;
using Scalar.AspNetCore;
using System.Text.Json;
using Serilog;
using System.Text.Json.Serialization;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
//builder.Services.AddSerilog();
//builder.Services.AddCustomLogConfiguration(builder.Configuration);
builder.Services.AddApiVersioningExtension();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info.Title = $"Clean Architecture API - {builder.Environment.EnvironmentName}";
        doc.Info.Version = "v1";
        return Task.CompletedTask;
    });
});
builder.Services.AddApplicationServices();
builder.Services.AddRepositoryServices(builder.Configuration);
builder.Services.AddSharedServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseErrorHandlerMiddleware();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
if (!app.Environment.IsEnvironment("Testing"))
{
    app.ApplyMigrations();
    await SeedData.InitializeDataAsync(app.Services);
}
app.Run();

public partial class Program { }
