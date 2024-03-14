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
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.SeatCapacity - src.SeatReserved))
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.FlightInformation.FlightNumber));

            CreateMap<ReservedSeat, ReservedSeatDTO>();

            CreateMap<FlightInformation, FlightInformationDTO>()
                .ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.SeatCapacity - src.SeatReserved));

            CreateMap<FlightInformation, BookingFlightInformationDTO>();

            CreateMap<FlightFare, BookingFlightFareDTO>()
               .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.FlightInformation.FlightNumber));

            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.ReservedSeat != null ? src.ReservedSeat.SeatNumber : null));
        }
    }
}