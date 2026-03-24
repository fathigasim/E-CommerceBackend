using AutoMapper;
using EcommerceApplication.DTOs;
using EcommerceApplication.Features.Orders.DTOs;
using EcommerceApplication.Features.Payment.DTOs;
using EcommerceDomain.Entities;
using EcommerceApplication.Features.Products.DTOs;
using EcommerceApplication.Features.Github.DTOs;
using EcommerceApplication.Features.Github.Queries.GithubUser;

namespace EcommerceApplication.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {

            // Product
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();

            // Order
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name));

            CreateMap<Payment, PaymentDto>();
            CreateMap<GithubUserDto, GithubUserVm>()
         .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Login))
         .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
         .ForMember(dest => dest.RepoCount, opt => opt.MapFrom(src => src.Public_Repos));
        }
       
    }
}

