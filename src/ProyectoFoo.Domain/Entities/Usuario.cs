using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ProyectoFoo.Domain.Entities
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; private set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccesDate { get; set; }


        // Constructor sin parámetros para Entity Framework
        public Usuario() { }
        public Usuario(int id, string nombre, string correo, string contrasena)
        {
            Id = id;
            Name = nombre;
            Email = correo;
            PasswordHash = HashPassword(contrasena); // Guardamos el hash de la contraseña
            CreatedDate = DateTime.Now;
            LastAccesDate = DateTime.Now;
        }

        // Método para verificar la contraseña comparando el hash
        public bool VerificarContrasena(string contrasena)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, PasswordHash);
        }

        //Método para actualizar el ultimo acceso.
        public void ActualizarUltimoAcceso()
        {
            LastAccesDate = DateTime.Now;
        }

        // Método para generar el hash de la contraseña
        public string HashPassword(string contrasena)
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