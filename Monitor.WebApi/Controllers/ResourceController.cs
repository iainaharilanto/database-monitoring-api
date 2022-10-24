namespace Monitor.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        // GET: api/<ResourceController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ResourceController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ResourceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ResourceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ResourceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
