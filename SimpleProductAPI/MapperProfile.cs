using AutoMapper;
using SimpleProductAPI.Models;

namespace SimpleProductAPI
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
