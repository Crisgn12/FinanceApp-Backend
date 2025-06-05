using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PagoController : ControllerBase
    {
        // GET: api/<PagoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PagoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PagoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PagoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PagoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
