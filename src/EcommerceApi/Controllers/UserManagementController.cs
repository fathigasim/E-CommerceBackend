using EcommerceApplication.Features.Auth.Commands.UserManagement.DeleteUser;
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
        [HttpPost("ForegotPassword")]
        public async Task<IActionResult> PostForegotPasswordAsync(ForegotPasswordCommand command)
        {
            var result = await _mediator.Send( command);
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> PostResetPasswordAsync([FromBody] ResetPasswordCommand command)
        {
            //if (command == null)
            //    return BadRequest("Command is null");

            var result = await _mediator.Send(command);

            //if (!result.IsSuccess)
            //    return BadRequest(result);

            return Ok(result);
        }
        // PUT api/<UserManagementController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserManagementController>/5
        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUserAsync(string UserId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(UserId));
            return Ok(result);
        }
    }
}
