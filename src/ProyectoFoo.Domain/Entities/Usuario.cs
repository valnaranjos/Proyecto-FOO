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
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la '�' y espacios.")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [NotNullOrWhitespace(ErrorMessage = "El apellido es obligatorio y no puede contener solo espacios.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la '�' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electr�nico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no v�lido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "varchar(20)")]
        [OptionalPhone(ErrorMessage = "N�mero de m�vil no v�lido.")]
        [StringLength(20, ErrorMessage = "El n�mero de m�vil debe ser positivo.")]
        public string? Phone { get; set; }


        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El t�tulo puede contener letras y espacios.")]
        public string? Title { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        //CALCULADOS POR EL SISTEMA
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccesDate { get; set; }
        public bool IsVerified { get; set; }


        // Constructor sin par�metros para Entity Framework
        public Usuario() 
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
            IsVerified = false;
        }

        // M�todo para verificar la contrase�a comparando el hash
        public bool VerifyPassword(string contrasena)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, PasswordHash);
        }

        //M�todo para actualizar el ultimo acceso.
        public void ActualizeLastAcces()
        {
            LastAccesDate = DateTime.Now;
        }

        // M�todo para generar el hash de la contrase�a
        public static string HashPassword(string contrasena)
        {
            // Por simplicidad, usaremos una combinaci�n b�sica
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }
    }
}