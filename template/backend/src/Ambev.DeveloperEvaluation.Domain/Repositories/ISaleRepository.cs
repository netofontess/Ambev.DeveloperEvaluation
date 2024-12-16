using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    /// <summary>
    /// Interface for managing sales in the repository.
    /// </summary>
    public interface ISaleRepository
    {
        Task AddAsync(Sale sale);
        Task<Sale?> GetByIdAsync(Guid id);
        Task<Sale?> GetBySaleNumberAsync(string saleNumber);
        Task UpdateAsync(Sale sale);
    }
}
