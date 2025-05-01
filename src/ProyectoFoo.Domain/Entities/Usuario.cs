using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Domain.Entities
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El n�mero de identificaci�n es obligatorio.")]
        [Range(10, int.MaxValue, ErrorMessage = "El n�mero de identificaci�n debe ser positivo.")]
        public int Identification { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        public string Apellido { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Correo no v�lido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; private set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccesDate { get; set; }


        // Constructor sin par�metros para Entity Framework
        public Usuario() 
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
        }
        public Usuario(int id, string nombre, string correo, string contrasena)
        {
            Id = id;
            Name = nombre;
            Email = correo;
            PasswordHash = HashPassword(contrasena); // Guardamos el hash de la contrase�a
            CreatedDate = DateTime.Now;
            LastAccesDate = DateTime.Now;
        }

        // M�todo para verificar la contrase�a comparando el hash
        public bool VerificarContrasena(string contrasena)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, PasswordHash);
        }

        //M�todo para actualizar el ultimo acceso.
        public void ActualizarUltimoAcceso()
        {
            LastAccesDate = DateTime.Now;
        }

        // M�todo para generar el hash de la contrase�a
        public string HashPassword(string contrasena)
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