using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Infrastructure.Persistence;
using Core.Entities;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementação genérica do repositório para acesso ao banco de dados.
    /// </summary>
    /// <typeparam name="T">Entidade do domínio.</typeparam>
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly Persistence.ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private bool _disposed = false;

        public RepositoryBase(Persistence.ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = _dbSet.Local.FirstOrDefault(e => EF.Property<Guid>(e, "Id") == id)
                 ?? await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
