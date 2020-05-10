using Microsoft.AspNetCore.Mvc;

namespace DrDocx.API.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}