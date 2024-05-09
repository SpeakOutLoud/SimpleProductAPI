using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleProductAPI.Controllers;
using SimpleProductAPI.Models;
using SimpleProductAPI.Repositories;
using SimpleProductAPI.UnitTests.Tests;
using Xunit;

namespace SimpleProductAPI.UnitTests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        public ProductsControllerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetProducts_ReturnsEmptyList_ForNoProductsExist()
        {
            // Arrange

            _productRepositoryMock.Setup(x => x.GetPagedReponseAsync(1, 1)).ReturnsAsync(FakeProducts.GetEmptyProducts());

            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var products = okObjectResult.Value as List<Product>;
            products.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetProducts_ReturnsPaginatedList_ForProductsExist()
        {
            // Arrange
            _productRepositoryMock.Setup(x => x.GetPagedReponseAsync(1, 1)).ReturnsAsync(FakeProducts.GetProducts(1));

            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.GetProducts(page: 1, pageSize: 1);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var products = okObjectResult.Value as List<Product>;
            products.Should().HaveCount(1);
            products[0].Id.Should().Be(1);
        }

        [Fact]
        public async Task GetProducts_ReturnsBadRequest_ForInvalidPageOrPageSize()
        {
            // Arrange
            _productRepositoryMock.Setup(x => x.GetPagedReponseAsync(1, 1)).ReturnsAsync(FakeProducts.GetProducts());

            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            // Act & Assert
            var result = await controller.GetProducts(page: 0, pageSize: 10);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid page or pageSize. Both must be positive integers.", badRequestResult.Value);

            // Act & Assert
            result = await controller.GetProducts(page: 1, pageSize: 0);
            badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid page or pageSize. Both must be positive integers.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostProduct_ReturnsCreated_ForValidProduct()
        {
            // Arrange
            var newProductDto = new ProductDto { Name = "New Product", Price = 10 };
            var newProduct = new Product { Name = "New Product", Price = 10 };

            _productRepositoryMock.Setup(x => x.AddAsync(newProduct)).ReturnsAsync(new Product() { Id = 1, Name = newProduct.Name, Price = newProduct.Price });
            _mapperMock.Setup(x => x.Map<Product>(newProductDto)).Returns(newProduct);

            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            // Act
            var result = await controller.PostProduct(newProductDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            createdAtActionResult.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public async Task PostProduct_ReturnsBadRequest_ForInvalidProduct()
        {
            // Arrange
            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);
            var newProduct = new ProductDto(); // Name is missing

            controller.ModelState.AddModelError("Name", "Name field is required");
            controller.ModelState.AddModelError("Description", "Description field is required");
            controller.ModelState.AddModelError("Price", "Price field is required");
            controller.ModelState.AddModelError("Quantity", "Quantity field is required");

            // Act
            var result = await controller.PostProduct(newProduct);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(400);
        }


        [Fact]
        public async Task PutProduct_ReturnBadRequest_ForInvalidProduct()
        {
            // Arrange
            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            controller.ModelState.AddModelError("Name", "Name is required");
            var product = new ProductDto();

            // Act
            var result = await controller.PutProduct(1, product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PutProduct_ReturnNotFound_ForProductNotFound()
        {
            // Arrange
            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);
            var product = new ProductDto { Name = "Updated Name" };

            // Act
            var result = await controller.PutProduct(1, product);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PutProduct_UpdateAndReturnNoContent_ForValidRequest()
        {
            // Arrange
            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            var existingProduct = new Product { Id = 1, Name = "Product 1" };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
            var product = new ProductDto { Name = "Updated Product" };

            // Act
            var result = await controller.PutProduct(1, product);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            createdAtActionResult.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public async Task DeleteProduct_ReturnNotFound_ForProductNotFound()
        {
            // Arrange
            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await controller.DeleteProduct(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteProduct_DeleteAndReturnNoContent_ForProductFound()
        {
            // Arrange
            var controller = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);

            var product = new Product { Id = 1 };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await controller.DeleteProduct(1);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            noContentResult.StatusCode.Should().Be(204);
        }
    }
}
