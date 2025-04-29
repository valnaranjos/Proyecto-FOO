using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using ProyectoFoo.Infrastructure.Context;
using ServiceStack.Text.Json;
using MySqlConnector;
using ProyectoFoo.Application.Features.PatientFeature.MapperProfiles;
using Microsoft.AspNetCore.Authorization;
using ProyectoFoo.Domain.Entities;


namespace ProyectoFOO.API.Controllers
{
    /// <summary>
    /// Controlador API para manejar operaciones sobre pacientes.
    /// </summary>
    [Authorize] // Requiere autenticación para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationContextSqlServer _context;

        public PatientController(ApplicationContextSqlServer context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista de pacientes de la base de datos.
        /// </summary>
        [HttpGet("pacientes")]
        public async Task<IActionResult> GetPacientes()
        {
            try
            {
                // Obtiene la lista de pacientes de la base de datos
                var pacientes = await _context.Pacientes.ToListAsync();

                // Devuelve los pacientes en formato JSON
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                // En caso de error, devuelve un BadRequest
                return BadRequest($"Error al obtener los pacientes: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo paciente en la base de datos.
        /// </summary>
        /// <param name="nuevoPaciente">Datos del paciente a crear.</param>
        /// <returns>El paciente creado.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePaciente([FromBody] Paciente nuevoPaciente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve un error 400 con los detalles de la validación
            }

            try
            {
                _context.Pacientes.Add(nuevoPaciente);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPacienteById), new { id = nuevoPaciente.Id }, nuevoPaciente);
            }
            catch (DbUpdateException ex) // Captura excepciones relacionadas con la base de datos
            {
                if (ex.InnerException is MySqlException mySqlException && mySqlException.Number == 1062) // 1062 es el código de error para violación de unicidad en MySQL
                {
                    return Conflict($"Ya existe un paciente con la identificación '{nuevoPaciente.Identification}'.");
                }

                // Otras excepciones de la base de datos
                return BadRequest($"Error al crear el paciente: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error inesperado al crear el paciente: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un paciente especifico por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a buscar.</param>
        /// <returns>Paciente encontrado o un mensaje de error.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPacienteById(int id)
        {
            try
            {
                // Buscar paciente en la base de datos por ID
                var paciente = await _context.Pacientes.FindAsync(id);

                // Si no se encuentra el paciente
                if (paciente == null)
                {
                    return NotFound("Paciente no encontrado.");
                }

                // Si se encuentra el paciente, se retorna con el estado 200 OK
                return Ok(paciente);
            }
            catch (Exception ex)
            {
                // En caso de error, se maneja la excepción
                return BadRequest($"Error al obtener el paciente: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza parcialmente los datos de un paciente existente.
        /// </summary>
        /// <param name="id">ID del paciente a actualizar.</param>
        /// <param name="updatePatientDto">Objeto JSON con los campos a actualizar.</param>
        /// <returns>Respuesta HTTP indicando el éxito o error de la actualización. En caso de éxito, devuelve el paciente actualizado.</returns>
        /// <remarks>
        /// Este endpoint permite actualizar solo los campos proporcionados en el cuerpo de la solicitud.
        /// El ID del paciente se debe especificar en la URL.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     PUT /api/1
        ///     {
        ///         "name": "Nuevo Nombre",
        ///         "phone": "987654321"
        ///     }
        ///
        /// Ejemplo de respuesta (200 OK):
        ///
        ///     {
        ///         "id": 1,
        ///         "name": "Nuevo Nombre",
        ///         "surname": "Pérez",
        ///         "birthdate": "2015-04-10",
        ///         "identification": 12345,
        ///         "diagnosis": "TDAH",
        ///         "institution": "Escuela 23",
        ///         "sex": "Masculino",
        ///         "email": "juan.perez@mail.com",
        ///         "phone": "987654321",
        ///         "modality": "Presencial",
        ///         "admissionDate": "2023-03-01T10:00:00Z"
        ///     }
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaciente(int id, [FromBody] UpdatePatientDto updatePatient)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pacienteExistente = await _context.Pacientes.FindAsync(id);

            if (pacienteExistente == null)
            {
                return NotFound("Paciente no encontrado.");
            }


            // Actualizar solo las propiedades que tienen un valor en el DTO
            if (updatePatient.Name != null) pacienteExistente.Name = updatePatient.Name;
            if (updatePatient.Surname != null) pacienteExistente.Surname = updatePatient.Surname;
            if (updatePatient.Birthdate.HasValue) pacienteExistente.Birthdate = updatePatient.Birthdate.Value;
            if (updatePatient.Identification.HasValue) pacienteExistente.Identification = updatePatient.Identification.Value;
            if (updatePatient.Sex != null) pacienteExistente.Sex = updatePatient.Sex;
            if (updatePatient.Email != null) pacienteExistente.Email = updatePatient.Email;
            if (updatePatient.Phone != null) pacienteExistente.Phone = updatePatient.Phone;
            if (updatePatient.Modality != null) pacienteExistente.Modality = updatePatient.Modality;
            if (updatePatient.Diagnosis != null) pacienteExistente.Diagnosis = updatePatient.Diagnosis;
            if (updatePatient.Institution != null) pacienteExistente.Institution = updatePatient.Institution;

            try
            {
                await _context.SaveChangesAsync();
                var pacienteActualizado = await _context.Pacientes.FindAsync(id); // Volver a obtener para incluir posibles cambios de la base de datos
                return Ok(pacienteActualizado); // Devolver el paciente actualizado con un código 200.
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Los datos del paciente han sido modificados por otro usuario.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el paciente: {ex.Message}");
            }
        }


        /// <summary>
        /// Elimina un paciente de la base de datos.
        /// </summary>
        /// <param name="id">ID del paciente a eliminar.</param>
        /// <returns>Respuesta HTTP indicando el éxito o error de la eliminación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);
                if (paciente == null)
                {
                    return NotFound("Paciente no encontrado.");
                }

                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el paciente: {ex.Message}");
            }
        }
    }
}