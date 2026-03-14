using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Keycloak;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepo _repo;

        public ClientController(IClientRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("user-registration-sync")]
        public async Task<IActionResult> UserRegistrationSync([FromBody] KeycloakEvent @event)
        {
            Console.WriteLine("Registration user sync invoked");
            //1. Security Check (Match the secret from Docker)
            var secret = Request.Headers["X-Event-Secret"];
            if (secret != "tms-event-user-2478") return Unauthorized("Not allowed to synchronize user data");

            //2. Only process registration events, not others
            if (@event.Type != "REGISTER") return Ok("Not needed to synchronize");

            //3. Extract the information from the body and add the data.
            Console.WriteLine(JsonSerializer.Serialize(@event));
            var clientData = new Client
            {
              Id = new Guid(@event.UserId),
              ContactEmail = @event.Details.GetValueOrDefault("email") ?? "",
              ContactPhone = @event.Details.GetValueOrDefault("phoneNumber") ?? "",
              Name = @event.Details.GetValueOrDefault("first_name") + " " + @event.Details.GetValueOrDefault("last_name")
            };

            await _repo.CreateClient(clientData);
            
            if(await _repo.SaveChangesAsync())
            {
                return Accepted();
            }

            return BadRequest("Error encountered when synchronising user registered data");
        }
    }
}