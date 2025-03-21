using System;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class SaleItemTests
    {
        private readonly Sale _sale;
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _branchId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public SaleItemTests()
        {
            _sale = new Sale("John Doe", "Main Branch", _customerId, _branchId, _userId);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productName = "Product 1";
            var quantity = 2;
            var unitPrice = 100m;
            var discountPercentage = 10m;

            // Act
            var item = new SaleItem(_sale, productId, productName, quantity, unitPrice, discountPercentage);

            // Assert
            item.ProductId.Should().Be(productId);
            item.ProductName.Should().Be(productName);
            item.Quantity.Should().Be(quantity);
            item.UnitPrice.Should().Be(unitPrice);
            item.DiscountPercentage.Should().Be(discountPercentage);
            item.IsCancelled.Should().BeFalse();
            item.GetTotalAmount().Should().Be(180); // (2 * 100) - 10% = 180
        }

        [Fact]
        public void Update_WhenNotCancelled_ShouldUpdateProperties()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var item = new SaleItem(_sale, productId, "Product 1", 2, 100m, 0m);
            var newQuantity = 3;
            var newUnitPrice = 150m;
            var newDiscountPercentage = 5m;

            // Act
            item.Update(newQuantity, newUnitPrice, newDiscountPercentage);

            // Assert
            item.Quantity.Should().Be(newQuantity);
            item.UnitPrice.Should().Be(newUnitPrice);
            item.DiscountPercentage.Should().Be(newDiscountPercentage);
            item.GetTotalAmount().Should().Be(427.5m); // (3 * 150) - 5% = 427.50
        }

        [Fact]
        public void Update_WhenCancelled_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var item = new SaleItem(_sale, productId, "Product 1", 2, 100m, 0m);
            item.Cancel();

            // Act & Assert
            var act = () => item.Update(3, 150m, 5m);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot update a cancelled item");
        }

        [Fact]
        public void Cancel_ShouldSetIsCancelledToTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var item = new SaleItem(_sale, productId, "Product 1", 2, 100m, 0m);

            // Act
            item.Cancel();

            // Assert
            item.IsCancelled.Should().BeTrue();
            item.GetTotalAmount().Should().Be(0); // Cancelled items return 0
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var item = new SaleItem(_sale, productId, "Product 1", 2, 100m, 0m);
            item.Cancel();

            // Act & Assert
            var act = () => item.Cancel();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Item is already cancelled");
        }

        [Fact]
        public void GetTotalAmount_WithDiscount_ShouldCalculateCorrectly()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var item = new SaleItem(_sale, productId, "Product 1", 5, 100m, 10m);

            // Assert
            item.GetTotalAmount().Should().Be(450m); // (5 * 100) - 10% = 450
        }

        [Fact]
        public void GetTotalAmount_WhenCancelled_ShouldReturnZero()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var item = new SaleItem(_sale, productId, "Product 1", 5, 100m, 10m);
            item.Cancel();

            // Assert
            item.GetTotalAmount().Should().Be(0);
        }
    }
}