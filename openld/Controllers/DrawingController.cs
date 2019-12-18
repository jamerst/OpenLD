using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using openld.Models;

namespace openld.Controllers {
    [ApiController]
    [Route("api/drawing/[action]")]
    public class DrawingController : ControllerBase {
        [HttpPost]
        public async Task AddDrawing([FromBody] Drawing drawing) {
            var user = (HttpContext.User.Identity as ClaimsIdentity);
            drawing.Owner = new User();
        }
        [HttpPost]
        [Authorize]
        public void Test() {
            var user = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}