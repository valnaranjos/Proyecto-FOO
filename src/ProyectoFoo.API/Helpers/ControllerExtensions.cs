using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProyectoFoo.API.Helpers
{
    public static class ControllerExtensions
    {
        public static int? GetCurrentUserId(this ControllerBase controller)
        {
            var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
