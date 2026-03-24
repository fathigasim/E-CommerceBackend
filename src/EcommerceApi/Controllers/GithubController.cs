using EcommerceApplication.Features.Github;
using EcommerceApplication.Features.Github.Queries;
using EcommerceApplication.Features.Github.Queries.GethubUser;
using EcommerceApplication.Features.Github.Queries.GithubProfile;
using EcommerceApplication.Features.Github.Queries.GithubUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GithubController(IMediator mediator) {
          _mediator = mediator;
        }
        // GET: api/<GithubController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
           var result= await _mediator.Send(new GetGithubUserQuery());
            return Ok(result);
        }
        [HttpGet("GetByUsername/{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var result = await _mediator.Send(new GetGithubUserProfileQuery(username));
            return Ok(result);
        }
        // GET api/<GithubController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<GithubController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GithubController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GithubController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
