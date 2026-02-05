using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Payment_Processing_System.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Требует JWT токен
    public class UserController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst("sub")?.Value;
            var userName = User.Identity?.Name;
            var email = User.FindFirst("email")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userId,
                userName,
                email,
                role,
                message = "Защищённые данные профиля"
            });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")] // Только для Admin
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "Данные для администратора" });
        }
    }
}
