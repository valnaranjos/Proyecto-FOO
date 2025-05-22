using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Shared.ValidationAttributes;

namespace ProyectoFoo.Domain.Entities
{
    public class Usuario
    {
        //OBLIGATORIOS
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        [StringLength(20, ErrorMessage = "La identificación no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "La identificación solo puede contener letras, números y guiones.")]
        public string Identification { get; set; } = string.Empty;


        [NotNullOrWhitespace(ErrorMessage = "El nombre es obligatorio y no puede contener solo espacios.")] 
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'ñ' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Número de teléfono no válido.")]
        [StringLength(20, ErrorMessage = "El número de móvil debe ser positivo.")]
        public string? Phone { get; set; }


        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El título puede contener letras y espacios.")]
        public string? Title { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        //CALCULADOS POR EL SISTEMA
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccesDate { get; set; }
        public bool IsVerified { get; set; }


        // Constructor sin parámetros para Entity Framework
        public Usuario() 
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
            IsVerified = false;
        }

        // Método para verificar la contraseña comparando el hash
        public bool VerifyPassword(string contrasena)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, PasswordHash);
        }

        //Método para actualizar el ultimo acceso.
        public void ActualizeLastAcces()
        {
            LastAccesDate = DateTime.Now;
        }

        // Método para generar el hash de la contraseña
        public static string HashPassword(string contrasena)
        {
            // Por simplicidad, usaremos una combinación básica
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }
    }
}