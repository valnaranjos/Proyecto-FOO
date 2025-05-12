using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFoo.Domain.Entities
{
    public class Usuario
    {
        //OBLIGATORIOS
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "El n�mero de identificaci�n es obligatorio.")]
        [Range(10, int.MaxValue, ErrorMessage = "El n�mero de identificaci�n debe ser positivo.")]
        public int Identification { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El nombre solo permite letras, acentos, la '�' y espacios.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El Apellido solo permite letras, acentos, la '�' y espacios.")]
        public string Surname { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Correo no v�lido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Email { get; set; }

        [Phone(ErrorMessage ="N�mero de tel�fono no v�lido.")]
        [Range(15, int.MaxValue, ErrorMessage = "El n�mero de identificaci�n debe ser positivo.")]
        public long? Phone { get; set; }

       
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z������������\s]+$", ErrorMessage = "El t�tulo puede contener letras y espacios.")]
        public string? Title { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; private set; }

        //CALCULADOS POR EL SISTEMA
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccesDate { get; set; }
        public bool IsVerified { get; set; }


        // Constructor sin par�metros para Entity Framework
        public Usuario() 
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        //Constructor sin telefono ni especialidad (titulo), ya que son opcionales.
        public Usuario(int id, int identification, string nombre, string surname,  string correo, string contrasena)
        {
            Id = id;
            Identification = identification;
            Name = nombre;
            Surname = surname;
            Email = correo;
            PasswordHash = HashPassword(contrasena); // Guardamos el hash de la contrase�a
            CreatedDate = DateTime.Now;
            LastAccesDate = DateTime.Now;
        }

        //Constructor con telefono y especialidad (titulo), ya que son opcionales.
        public Usuario(int id, int identification, string nombre, string surname, string correo, string contrasena, long phone, string title)
        {
            Id = id;
            Identification = identification;
            Name = nombre;
            Surname = surname;
            Email = correo;
            Phone = phone;
            Title = title;
            PasswordHash = HashPassword(contrasena); // Guardamos el hash de la contrase�a
            CreatedDate = DateTime.Now;
            LastAccesDate = DateTime.Now;
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