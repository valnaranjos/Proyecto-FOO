using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    public class VerifyRegistrationDto
    {
        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código de verificación es obligatorio.")]
        public string VerificationCode { get; set; } = string.Empty;
    }
}
