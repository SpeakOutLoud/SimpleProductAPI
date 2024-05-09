using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.UnitTests.Tests
{
    public class TestDbSet<T> : DbSet<Product>
    {
        public override IEntityType EntityType => throw new NotImplementedException();
    }
}