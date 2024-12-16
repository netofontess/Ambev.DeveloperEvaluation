using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services
{
    /// <summary>
    /// Interface for managing sale-related business rules.
    /// </summary>
    public interface ISaleService
    {
        void ApplyBusinessRules(Sale sale);
    }
}
