using AutoMapper;
using EcommerceApplication.Features.Auth.Dtos;
using Stripe.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceInfrastructure
{
    public class IdentityMapping : Profile
    {
        public IdentityMapping()
        {
            CreateMap<Identity.ApplicationUser, UserDto>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                //.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                //.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                //.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                //.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ReverseMap();
        }
    }
}
