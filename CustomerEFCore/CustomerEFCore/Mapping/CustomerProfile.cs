using AutoMapper;
using CustomerEFCore.Domain;
using CustomerEFCore.Model;

namespace CustomerEFCore.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerModel>().ReverseMap();
        }
    }
}
