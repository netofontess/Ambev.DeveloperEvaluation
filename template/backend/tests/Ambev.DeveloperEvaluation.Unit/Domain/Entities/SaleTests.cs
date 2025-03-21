using System;
using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class SaleTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var customerName = "John Doe";
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var branchName = "Main Branch";
            var userId = Guid.NewGuid();

            // Act
            var sale = new Sale(customerName, branchName, customerId, branchId, userId);

            // Assert
            sale.CustomerName.Should().Be(customerName);
            sale.CustomerId.Should().Be(customerId);
            sale.BranchId.Should().Be(branchId);
            sale.BranchName.Should().Be(branchName);
            sale.UserId.Should().Be(userId);
            sale.IsCancelled.Should().BeFalse();
            sale.Items.Should().BeEmpty();
            sale.GetTotalAmount().Should().Be(0);
            sale.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Update_WhenNotCancelled_ShouldUpdateProperties()
        {
            // Arrange
            var oldCustomerId = Guid.NewGuid();
            var oldBranchId = Guid.NewGuid();
            var oldUserId = Guid.NewGuid();
            var sale = new Sale("Old Name", "Old Branch", oldCustomerId, oldBranchId, oldUserId);

            var newCustomerName = "New Name";
            var newCustomerId = Guid.NewGuid();
            var newBranchId = Guid.NewGuid();
            var newBranchName = "New Branch";
            var newUserId = Guid.NewGuid();

            // Act
            sale.Update(newCustomerName, newBranchName, newCustomerId, newBranchId, newUserId);

            // Assert
            sale.CustomerName.Should().Be(newCustomerName);
            sale.CustomerId.Should().Be(newCustomerId);
            sale.BranchId.Should().Be(newBranchId);
            sale.BranchName.Should().Be(newBranchName);
            sale.UserId.Should().Be(newUserId);
        }

        [Fact]
        public void Update_WhenCancelled_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.Cancel();

            // Act & Assert
            var act = () => sale.Update("New Name", "New Branch", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot update a cancelled sale");
        }

        [Fact]
        public void AddItem_WhenNotCancelled_ShouldAddItemAndRecalculateTotal()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            var productId = Guid.NewGuid();
            var productName = "Product 1";
            var quantity = 2;
            var unitPrice = 100m;
            var discountPercentage = 0m;

            // Act
            sale.AddItem(productId, productName, quantity, unitPrice, discountPercentage);

            // Assert
            sale.Items.Should().ContainSingle();
            var item = sale.Items.First();
            item.ProductId.Should().Be(productId);
            item.ProductName.Should().Be(productName);
            item.Quantity.Should().Be(quantity);
            item.UnitPrice.Should().Be(unitPrice);
            sale.GetTotalAmount().Should().Be(200); // 2 * 100 = 200 (no discount)
        }

        [Fact]
        public void AddItem_WhenCancelled_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.Cancel();

            // Act & Assert
            var act = () => sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m, 0m);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot add items to a cancelled sale");
        }

        [Fact]
        public void CancelItem_WhenNotCancelled_ShouldCancelItemAndRecalculateTotal()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m, 0m);
            sale.AddItem(Guid.NewGuid(), "Product 2", 3, 50m, 0m);
            var itemToCancel = sale.Items.First();

            // Act
            sale.CancelItem(itemToCancel.Id);

            // Assert
            sale.Items.Should().HaveCount(1); // Only one item should remain
            sale.Items.Should().NotContain(itemToCancel); // Cancelled item should be removed
            sale.GetTotalAmount().Should().Be(150); // Only the second item's amount (3 * 50 = 150)
        }

        [Fact]
        public void CancelItem_WhenSaleIsCancelled_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m, 0m);
            var itemToCancel = sale.Items.First();
            sale.Cancel();

            // Act & Assert
            var act = () => sale.CancelItem(itemToCancel.Id);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot cancel items in a cancelled sale");
        }

        [Fact]
        public void Cancel_ShouldSetIsCancelledToTrue()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);

            // Act
            sale.Cancel();

            // Assert
            sale.IsCancelled.Should().BeTrue();
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.Cancel();

            // Act & Assert
            var act = () => sale.Cancel();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Sale is already cancelled");
        }

        [Fact]
        public void GetTotalAmount_ShouldReflectOnlyCancelledItems()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.AddItem(Guid.NewGuid(), "Product 1", 5, 100m, 10m); // Should have 10% discount
            sale.AddItem(Guid.NewGuid(), "Product 2", 3, 50m, 0m);  // No discount
            var itemToCancel = sale.Items.First();

            // Act
            sale.CancelItem(itemToCancel.Id);

            // Assert
            sale.GetTotalAmount().Should().Be(150); // Only the second item's amount (3 * 50 = 150)
        }
    }
}