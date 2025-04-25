using Microsoft.AspNetCore.Mvc;

namespace ProyectoFoo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Hola desde ProyectoFoo.API 👋");
    }
}