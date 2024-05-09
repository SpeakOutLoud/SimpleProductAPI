using Microsoft.EntityFrameworkCore;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.UnitTests.Tests
{
    public static class FakeProducts
    {
        public static List<Product> GetProducts(int count = 10)
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            return products.Take(count).ToList();
        }

        public static List<Product> GetEmptyProducts()
        {
            return new List<Product>();
        }
    }
}
