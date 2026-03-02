using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order
{
    public record OrderReadDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ClientId { get; set; } = Guid.NewGuid();

        public ClientReadDto Client { get; set; } = default!;

        public string OrderNumber { get; set; } = default!;

        public OrderStatus Status { get; set; } = OrderStatus.Created;

        public string Priority { get; set; } = string.Empty;

        public Guid PickupAddressId { get; set; } = Guid.NewGuid();

        public AddressReadDto PickupAddress { get; set; } = default!;

        public Guid DeliveryAddressId { get; set; } = Guid.NewGuid();

        public AddressReadDto DeliveryAddress { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}