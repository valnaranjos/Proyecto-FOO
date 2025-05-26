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

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotNullOrWhitespace(ErrorMessage = "El nombre es obligatorio y no puede contener solo espacios.")] 
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z·ÈÌÛ˙¡…Õ”⁄Ò—\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'Ò' y espacios.")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z·ÈÌÛ˙¡…Õ”⁄Ò—\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la 'Ò' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrÛnico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no v·lido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "varchar(20)")]
        [OptionalPhone(ErrorMessage = "N˙mero de mÛvil no v·lido.")]
        [StringLength(20, ErrorMessage = "El n˙mero de mÛvil debe ser positivo.")]
        public string? Phone { get; set; }


        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z¡…Õ”⁄·ÈÌÛ˙—Ò\s]+$", ErrorMessage = "El tÌtulo puede contener letras y espacios.")]
        public string? Title { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        //CALCULADOS POR EL SISTEMA
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccesDate { get; set; }
        public bool IsVerified { get; set; }


        // Constructor sin par·metros para Entity Framework
        public Usuario() 
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
            IsVerified = false;
        }

        // MÈtodo para verificar la contraseÒa comparando el hash
        public bool VerifyPassword(string contrasena)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, PasswordHash);
        }

        //MÈtodo para actualizar el ultimo acceso.
        public void ActualizeLastAcces()
        {
            LastAccesDate = DateTime.Now;
        }

        // MÈtodo para generar el hash de la contraseÒa
        public static string HashPassword(string contrasena)
        {
            // Por simplicidad, usaremos una combinaciÛn b·sica
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }
    }
}