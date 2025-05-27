using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.Models.User
{
    public class UserInfoDto
    {
        /// <summary>ID del usuario.</summary>
        public int Id { get; set; }
   
        /// <summary>Nombre del usuario.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Apellido del usuario.</summary>
        public string Surname { get; set; } = string.Empty;

        /// <summary>Correo electrónico del usuario.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Número de teléfono del usuario (si tiene).</summary>
        public string? Phone { get; set; }

        /// <summary>Título profesional o especialidad del usuario (si tiene).</summary>
        public string? Title { get; set; }
    }
}
