using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    /// <summary>
    /// Implementation of ISaleRepository using Entity Framework Core
    /// </summary>
    public class SaleRepository : ISaleRepository
    {
        private readonly DbContext _context;

        /// <summary>
        /// Initializes a new instance of SaleRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public SaleRepository(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a sale by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale if found, null otherwise</returns>
        public async Task<Sale> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Sale>()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves a paginated list of sales with optional filtering and ordering
        /// </summary>
        public async Task<IEnumerable<Sale>> GetSalesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Sale>()
                .Include(s => s.Items)
                .AsQueryable();

            // Apply filters
            if (startDate.HasValue)
                query = query.Where(s => s.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.CreatedAt <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(customerName))
                query = query.Where(s => s.CustomerName.Contains(customerName));

            if (customerId.HasValue)
                query = query.Where(s => s.CustomerId == customerId.Value);

            if (branchId.HasValue)
                query = query.Where(s => s.BranchId == branchId.Value);

            if (isCancelled.HasValue)
                query = query.Where(s => s.IsCancelled == isCancelled.Value);

            if (minAmount.HasValue)
                query = query.Where(s => s.Items.Sum(i => i.TotalAmount) >= minAmount.Value);

            if (maxAmount.HasValue)
                query = query.Where(s => s.Items.Sum(i => i.TotalAmount) <= maxAmount.Value);

            // Apply ordering
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "date":
                        query = query.OrderByDescending(s => s.CreatedAt);
                        break;
                    case "customer":
                        query = query.OrderBy(s => s.CustomerName);
                        break;
                    case "branch":
                        query = query.OrderBy(s => s.BranchName);
                        break;
                    case "amount":
                        query = query.OrderByDescending(s => s.Items.Sum(i => i.TotalAmount));
                        break;
                    default:
                        query = query.OrderByDescending(s => s.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(s => s.CreatedAt);
            }

            // Apply pagination
            query = query.Skip((page - 1) * size).Take(size);

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adds a new sale to the database
        /// </summary>
        /// <param name="sale">The sale to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await _context.Set<Sale>().AddAsync(sale, cancellationToken);
        }

        /// <summary>
        /// Updates an existing sale in the database
        /// </summary>
        /// <param name="sale">The sale to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            var existingSale = await _context.Set<Sale>()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == sale.Id, cancellationToken);

            if (existingSale == null)
                throw new InvalidOperationException($"Sale with ID {sale.Id} not found");

            // Update sale properties
            _context.Entry(existingSale).CurrentValues.SetValues(sale);

            // Handle items
            var existingItems = existingSale.Items.ToList();
            var newItems = sale.Items.ToList();

            // Create lists to track items to add, update, and remove
            var itemsToAdd = new List<SaleItem>();
            var itemsToUpdate = new List<SaleItem>();
            var itemsToRemove = new List<SaleItem>();

            // Identify items to add and update
            foreach (var item in newItems)
            {
                var existingItem = existingItems.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem == null)
                {
                    // New item
                    item.SetUpdatedAt(sale.UpdatedAt); // Ensure timestamps match
                    itemsToAdd.Add(item);
                }
                else
                {
                    // Updated item
                    existingItem.SetUpdatedAt(sale.UpdatedAt); // Ensure timestamps match
                    itemsToUpdate.Add(item);
                }
            }

            // Identify items to remove
            foreach (var existingItem in existingItems)
            {
                if (!newItems.Any(i => i.Id == existingItem.Id))
                {
                    itemsToRemove.Add(existingItem);
                }
            }

            // Apply changes
            foreach (var item in itemsToAdd)
            {
                _context.Set<SaleItem>().Add(item);
            }

            foreach (var item in itemsToUpdate)
            {
                var existingItem = existingItems.First(i => i.Id == item.Id);
                _context.Entry(existingItem).CurrentValues.SetValues(item);
            }

            foreach (var item in itemsToRemove)
            {
                _context.Set<SaleItem>().Remove(item);
            }

            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // If we get a concurrency exception, reload the entity and retry
                await _context.Entry(existingSale).ReloadAsync(cancellationToken);
                var itemsToReload = existingSale.Items.ToList();
                foreach (var item in itemsToReload)
                {
                    await _context.Entry(item).ReloadAsync(cancellationToken);
                }

                // Update sale properties again
                _context.Entry(existingSale).CurrentValues.SetValues(sale);

                // Handle items again
                existingItems = existingSale.Items.ToList();
                newItems = sale.Items.ToList();

                // Reset lists
                itemsToAdd.Clear();
                itemsToUpdate.Clear();
                itemsToRemove.Clear();

                // Identify items to add and update
                foreach (var item in newItems)
                {
                    var existingItem = existingItems.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem == null)
                    {
                        // New item
                        item.SetUpdatedAt(sale.UpdatedAt); // Ensure timestamps match
                        itemsToAdd.Add(item);
                    }
                    else
                    {
                        // Updated item
                        existingItem.SetUpdatedAt(sale.UpdatedAt); // Ensure timestamps match
                        itemsToUpdate.Add(item);
                    }
                }

                // Identify items to remove
                foreach (var existingItem in existingItems)
                {
                    if (!newItems.Any(i => i.Id == existingItem.Id))
                    {
                        itemsToRemove.Add(existingItem);
                    }
                }

                // Apply changes
                foreach (var item in itemsToAdd)
                {
                    _context.Set<SaleItem>().Add(item);
                }

                foreach (var item in itemsToUpdate)
                {
                    var existingItem = existingItems.First(i => i.Id == item.Id);
                    _context.Entry(existingItem).CurrentValues.SetValues(item);
                }

                foreach (var item in itemsToRemove)
                {
                    _context.Set<SaleItem>().Remove(item);
                }

                await SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}