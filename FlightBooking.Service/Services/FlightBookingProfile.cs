using AutoMapper;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;

namespace FlightBooking.Service.Services
{
    public class FlightBookingProfile : Profile
    {
        public FlightBookingProfile()
        {
            CreateMap<FlightFare, FlightFareDTO>()
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.Capacity - src.Reserved))
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.FlightInformation.FlightNumber));
        }
    }
}
