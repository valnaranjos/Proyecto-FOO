using System;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoFoo.Shared.Models
{
    public class Usuario
    {
        public int Id { get;  set; }
        public string Name { get;  set; }
        public string Email { get;  set; }
        public string PasswordHash { get; private set; } 
        public DateTime CreatedDate { get;  set; }
        public DateTime LastAccesDate { get;  set; }


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
            return PasswordHash == HashPassword(contrasena);
        }

        //Método para actualizar el ultimo acceso.
        public void ActualizarUltimoAcceso()
        {
            LastAccesDate = DateTime.Now;
        }

        // Método para generar el hash de la contraseña
        private string HashPassword(string contrasena)
        {
            using var sha256 = SHA256.Create();
            var data = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
            return Convert.ToBase64String(data);
        }
    }
}
