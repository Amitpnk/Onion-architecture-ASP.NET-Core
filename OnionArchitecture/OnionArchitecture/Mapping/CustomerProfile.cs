using AutoMapper;
using OnionArchitecture.Domain.Entities;
using OnionArchitecture.Model;

namespace OnionArchitecture.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerModel>().ReverseMap();
        }
    }
}
