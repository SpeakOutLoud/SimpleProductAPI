using SimpleProductAPI.Context;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Repositories
{
    public interface IProductRepository : IGenericServices<Product>
    {

    }

    public class ProductRepository : GenericServices<Product>, IProductRepository
    {
        public ProductRepository(IDataContext dbContext) : base(dbContext)
        {

        }
    }
}
