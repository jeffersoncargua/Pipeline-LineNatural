using AutoMapper;
using LineNatural.DTOs.Category;
using LineNatural.DTOs.LocalUser;
using LineNatural.DTOs.Product;
using LineNatural.Entities;

namespace LineNatural.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();


            //Product
            CreateMap<Producto,ProductoDto>();
            //CreateMap<ProductoDto, Producto>().ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<ProductoDto, Producto>();

            CreateMap<Producto, ProductoCreateDto>().ReverseMap();
            CreateMap<Producto, ProductoUpdateDto>().ReverseMap();

            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}
