using System.ComponentModel.DataAnnotations;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Shipment;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order
{
    public record OrderCreateDto
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string OrderNumber { get; set; } = default!;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Created;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public Guid PickupAddressId { get; set; }

        [Required]
        public Guid DeliveryAddressId { get; set; }

        public List<ShipmentCreateDto> Shipments { get; set; } = new List<ShipmentCreateDto>();
    }
}
