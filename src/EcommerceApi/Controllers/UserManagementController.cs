using EcommerceApplication.Features.Auth.Commands.UserManagement.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<UserManagementController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserManagementController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserManagementController>
        [HttpPost]
        public async Task<IActionResult> PostAsync(string email)
        {
            var result = await _mediator.Send(new ResetPasswordCommand(email));
            return Ok(result);
        }

        // PUT api/<UserManagementController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserManagementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
