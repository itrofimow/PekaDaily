using Microsoft.AspNetCore.Mvc;

namespace Jobs.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        [HttpGet]
        public string Status()
        {
            return "Ok";
        }
    }
}