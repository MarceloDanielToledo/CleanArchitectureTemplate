using Application.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using Repository.Contexts;

namespace Repository.Services
{
    internal class RepositoryAsync<T>(ApplicationDbContext dbContext) : RepositoryBase<T>(dbContext), IRepositoryAsync<T> where T : class
    {
        private readonly ApplicationDbContext dbContext = dbContext;
    }
}
