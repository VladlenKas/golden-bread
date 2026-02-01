using Microsoft.AspNetCore.Mvc;
using GoldenBread.Contracts.Requests;

namespace GoldenBread.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController() : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> GetAll()
    {
        return new string[] { "value1", "value2" };
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
