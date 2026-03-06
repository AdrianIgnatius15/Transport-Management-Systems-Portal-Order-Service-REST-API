using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Pagination;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Utilities;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepo _repo;

        public OrderController(IOrderRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("emailPagination")]
        // [Authorize(Roles = "shipper")]
        public async Task<ActionResult<PaginatedResult<OrderReadDto>>> GetAllOrdersByClientEmail([FromQuery] PaginationOrderSearchParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new PaginationOrderSearchParameters();
            }

            Console.WriteLine("Email", parameters.Email);
            var pagedOrders = await _repo.GetAllOrdersByClientEmailWithPagination(parameters);

            var orderReadDtos = pagedOrders.Items.Select(o =>
            {
                var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(o);
                if (o.Client != null)
                    orderReadDto.Client = MapperUtility.Map<Client, ClientReadDto>(o.Client);
                if (o.PickupAddress != null)
                    orderReadDto.PickupAddress = MapperUtility.Map<Address, AddressReadDto>(o.PickupAddress);
                if (o.DeliveryAddress != null)
                    orderReadDto.DeliveryAddress = MapperUtility.Map<Address, AddressReadDto>(o.DeliveryAddress);
                return orderReadDto;
            }).ToList();

            var result = new PaginatedResult<OrderReadDto>(orderReadDtos, pagedOrders.TotalCount, pagedOrders.PageNumber, pagedOrders.PageSize);

            return Ok(result);
        }

        [HttpPost("all")]
        [Authorize(Roles = "shipper,receiver")]
        public async Task<ActionResult<PaginatedResult<OrderReadDto>>> GetAllOrders([FromBody] PaginationParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new PaginationParameters();
            }

            var pagedAllOrders = await _repo.GetAllOrdersWithPagination(parameters);

            var orderReadDtos = pagedAllOrders.Items.Select(order =>
            {
                var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(order);
                return orderReadDto;
            }).ToList();

            return Ok(new PaginatedResult<OrderReadDto>(orderReadDtos, pagedAllOrders.TotalCount, pagedAllOrders.PageNumber, pagedAllOrders.PageSize));
        }

        [HttpPost("create")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult<string>> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            if (orderCreateDto == null)
            {
                return BadRequest("Order details are empty");
            }

            if (!ModelState.IsValid)
            {
                var validationResults = new List<ValidationResult>();

                return BadRequest(new { errors = validationResults.Select(v => v.MemberNames).ToList() });
            }
            else
            {
                var order = MapperUtility.Map<OrderCreateDto, Order>(orderCreateDto);
                order.OrderNumber = Guid.NewGuid().ToString();

                await _repo.CreateOrder(order);
                await _repo.SaveChangesAsync();

                return Ok("Order Created!");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrders()
        {
            var orders = await _repo.GetAllOrders();

            return Ok(orders);
        }
    }
}