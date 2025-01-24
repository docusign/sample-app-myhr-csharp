using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "OK";
        }
    }
}
