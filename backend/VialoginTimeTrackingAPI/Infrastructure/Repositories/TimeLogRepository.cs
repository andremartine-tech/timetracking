using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using System.Linq.Expressions;
using Core.Specifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repositório específico para operações relacionadas à entidade TimeLog.
    /// </summary>
    public class TimeLogRepository : RepositoryBase<TimeLog>, ITimeLogRepository
    {
        public TimeLogRepository(Persistence.ApplicationDbContext context) : base(context) { }

        /// <inheritdoc>
        public async Task<IEnumerable<TimeLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.TimeLogs
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.TimestampIn)
                .ToListAsync();
        }

        /// <inheritdoc>
        public async Task<bool> HasOverlapAsync(Guid id, DateTime start, DateTime end, Guid userId)
        {
            return await _context.TimeLogs.AnyAsync(TimeLogSpecifications.HasOverlapWith(id, start, end, userId));
        }

        /// <inheritdoc>
        public async Task<IEnumerable<TimeLog>> GetByUserAndMonthYearAsync(Guid userId, DateTime monthYear)
        {
            return await _context.TimeLogs.Where(TimeLogSpecifications.ByUserAndMonthYear(userId, monthYear)).ToListAsync();
        }

        /// <inheritdoc>
        public async Task<IEnumerable<TimeLog>> QueryAsync(Expression<Func<TimeLog, bool>> predicate, Expression<Func<TimeLog, object>> orderBy = null, bool ascending = true)
        {
            IQueryable<TimeLog> query = _context.TimeLogs.Where(predicate);

            if (orderBy != null)
            {
                query = ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            return await query.ToListAsync();
        }

        /// <inheritdoc>
        public async Task DeleteFromUserAsync(Guid userId)
        {
            IQueryable<TimeLog> query = _context.TimeLogs.Where(t => t.UserId == userId);
            IEnumerable<TimeLog> timeLogList = await query.ToListAsync();

            foreach (var log in timeLogList)
            {
                if (log != null)
                {
                    _context.Remove(log);
                    await _context.SaveChangesAsync();
                }
            }
        }

        /// <inheritdoc>
        public async Task UpdateDetachedAsync(TimeLog timeLog)
        {
            var existingEntity = _context.ChangeTracker.Entries<TimeLog>().FirstOrDefault(e => e.Entity.Id == timeLog.Id);

            if (existingEntity != null)
            {
                _context.Entry(existingEntity.Entity).State = EntityState.Detached;
            }

            _context.Entry(timeLog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
