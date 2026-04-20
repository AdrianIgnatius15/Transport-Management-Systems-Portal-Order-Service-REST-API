using System.ComponentModel.DataAnnotations;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order
{
    public record OrderCreateDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ClientId is required")]
        public Guid ClientId { get; set; }

        public string Priority { get; set; } = string.Empty;

        public AddressCreateDto PickupAddress { get; set; } = new AddressCreateDto();

        public AddressCreateDto DeliveryAddress { get; set; } = new AddressCreateDto();
    }
}
