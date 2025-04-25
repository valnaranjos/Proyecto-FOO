using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.Models
{
    /// <summary>
    /// Modelo para iniciar sesión.
    /// </summary>
    public class LoginModel
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 3)]
        public required string Password { get; set; }
    }
}
