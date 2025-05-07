using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Application.Features.Users
{
    public class ConfirmEmailChangeDto
    {
        [Required(ErrorMessage = "El código de verificación es requerido.")]
        public required string VerificationCode { get; set; }
    }
}
