using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Interfaces.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Sale>> GetSalesAsync(
            int page = 1,
            int size = 10,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string customerName = null,
            Guid? customerId = null,
            Guid? branchId = null,
            bool? isCancelled = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            string orderBy = null,
            CancellationToken cancellationToken = default);
        Task AddAsync(Sale sale, CancellationToken cancellationToken = default);
        Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}