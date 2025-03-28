﻿using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.DTO
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        public DateTime CreatedAt { get; set; }

        public string BookingNumber { get; set; } = null!;
        public int BookingOrderId { get; set; }

        [EnumDataType(typeof(BookingStatus))]
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
        public string? SeatNumber { get; set; }

        public BookingFlightInformationDTO FlightInformation { get; set; } = null!;
        public BookingFlightFareDTO FlightFare { get; set; } = null!;
    }

    public class BookingRequestDTO
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; } = Gender.PreferNotToSay;

        [Required]
        [Range(1, int.MaxValue)]
        public int OutboundFareId { get; set; } //We are keeping Fare per booking to allow flexibility e.g Adult in Economy and Child in Business class

        public int? ReturnFareId { get; set; }
    }
}