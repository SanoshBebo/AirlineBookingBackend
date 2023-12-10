namespace SanoshAirlines.Models.RequestBodyModels
{
    public class PartnerBookingModel
    {
        public string? Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid BookingId { get; set; }
        public string? BookingType { get; set; }
        public int ScheduleId { get; set; }
        public int TicketNo { get; set; }
        public List<PassengerInfo>? PassengerInfos { get; set; }
        public string? FlightName { get; set; }
        public string? SourceAirportId { get; set; }
        public string? DestinationAirportId { get; set; }
        public DateTime DateTime { get; set; }
        public string? AirlineName { get; set; }

    }
}
