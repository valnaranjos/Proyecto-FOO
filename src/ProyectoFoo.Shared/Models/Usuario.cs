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


        // Constructor sin par�metros para Entity Framework
        public Usuario() { }
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
            return PasswordHash == HashPassword(contrasena);
        }

        //M�todo para actualizar el ultimo acceso.
        public void ActualizarUltimoAcceso()
        {
            LastAccesDate = DateTime.Now;
        }

        // M�todo para generar el hash de la contrase�a
        private string HashPassword(string contrasena)
        {
            using var sha256 = SHA256.Create();
            var data = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
            return Convert.ToBase64String(data);
        }
    }
}
