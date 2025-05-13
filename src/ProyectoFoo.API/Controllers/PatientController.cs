using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ProyectoFoo.Application.Contracts.Persistence;
using ProyectoFoo.Application.Features.Patients.Search;
using ProyectoFoo.Application.Features.Patients.Filters;
using ProyectoFoo.Application.Features.Patients.CRUD;
using ProyectoFoo.Shared.Models;
using ProyectoFoo.Application.Features.PatientMaterials;
using ProyectoFoo.Application.Features.Patients;


namespace ProyectoFOO.API.Controllers
{
    /// <summary>
    /// Controlador API para manejar operaciones sobre pacientes.
    /// </summary>
    [Authorize] // Requiere autenticación para todas las acciones en este controlador
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IPatientService _patientService;

        public PatientController(IMediator mediator, IPatientService patientService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
        }



        /// <summary>
        /// Obtiene la lista de pacientes de la base de datos.
        /// </summary>
        /// /// <returns>Respuesta HTTP con una lista de todos los pacientes.</returns>
        /// <remarks>
        /// Este endpoint devuelve una colección de todos los pacientes actualmente registrados en el sistema.
        ///
        /// Ejemplo de respuesta (200 OK):
        ///
        ///     [
        ///         {
        ///             "id": 1,
        ///             "name": "Ana",
        ///             "surname": "Nanana",
        ///             "birthdate": "2005-04-30T00:00:00",
        ///             "identification": "214748357",
        ///             "sex": "Masculino",
        ///             "modality": "Presencial",
        ///             "email": "userrr@example.com",
        ///             "phone": "234566", 
        ///             "age": 20,
        ///             "admissionDate: "2025-05-01T00:00:00",
        ///             "ageRange": "Adulto",
        ///             "nationality": "Colombiano",
        ///         },
        ///     ]
        ///
        /// Ejemplo de respuesta (500 Internal Server Error - Error al obtener pacientes):
        ///
        ///     {
        ///         "message": "Error al obtener todos los pacientes."
        ///     }
        /// </remarks>
        [HttpGet("pacientes")]
        public async Task<ActionResult<GetAllPatientsResponse>> GetPacientes()
        {
            try
            {
                var query = new GetAllPatientsQuery();
                var response = await _mediator.Send(query);

                if (response.Success)
                {
                    return Ok(response.Patients);
                }
                else
                {
                    return StatusCode(500, response.Message ?? "Error al obtener todos los pacientes.");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devuelve un BadRequest
                return BadRequest($"Error al obtener los pacientes: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo paciente.
        /// </summary>
        /// <param name="request">Objeto JSON que representa la información del nuevo paciente.</param>
        /// <returns>Respuesta HTTP indicando el éxito de la creación. En caso de éxito, devuelve la información del paciente creado.</returns>
        /// <remarks>
        /// Este endpoint recibe la información para crear un nuevo paciente en el sistema.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     POST /api/patients
        ///     {
        ///         "nuevoPaciente": {
        ///             "name": "Juan",
        ///             "surname": "Pérez",
        ///             "birthdate": "2000-05-15T00:00:00",
        ///             "nationality" : "Colombiano",
        ///             "typeOfIdentification": "Cédula de Ciudadanía",
        ///             "identification": "123456789",
        ///             "sex": "Masculino",
        ///             "email": "juan.perez@example.com",
        ///             "phone": "3101234567"
        ///         }
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<CreatePatientResponse>> CreatePaciente([FromBody] CreatePatientCommand command)
        {
            var response = await _mediator.Send(command);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (response.Success)
                {
                    return CreatedAtAction(nameof(GetPatientById), new { id = response.PatientId }, response);
                }
                else
                {
                    return BadRequest(response); // O podrías usar StatusCode según el tipo de error
                }
            }
            catch (DbUpdateException ex) // Captura excepciones relacionadas con la base de datos
            {
                if (ex.InnerException is MySqlException mySqlException && mySqlException.Number == 1062) // 1062 es el código de error para violación de unicidad en MySQL
                {
                    return Conflict($"Ya existe un paciente con la identificación '{command.Identification}'."); // Usamos command.Identification ahora
                }

                return BadRequest($"Error al crear el paciente: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado al crear el paciente: {ex.Message}"); // Usamos StatusCode 500 para errores inesperados
            }
        }


        /// <summary>
        /// Obtiene un paciente especifico por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a buscar.</param>
        /// <returns>Respuesta HTTP con la información del paciente si se encuentra.</returns>
        /// <remarks>
        /// Este endpoint permite obtener los detalles de un paciente específico utilizando su identificador único.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPatientByIdResponse>> GetPatientById(int id)
        {
            try
            {
                var query = new GetPatientByIdQuery(id);
                var response = await _mediator.Send(query);

                if (response.Success && response.Patient != null)
                {
                    return Ok(response.Patient);
                }
                else
                {
                    return NotFound(response.Message ?? $"Paciente con ID {id} no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado al obtener el paciente con ID {id}: {ex.Message}");
            }
        }


        /// <summary>
        /// Actualiza parcialmente los datos de un paciente existente.
        /// </summary>
        /// <param name="id">ID del paciente a actualizar.</param>
        /// <param name="command">Objeto JSON con los campos a actualizar.</param>
        /// <returns>Respuesta HTTP indicando el éxito o error de la actualización. En caso de éxito, devuelve el paciente actualizado.</returns>
        /// <remarks>
        /// Este endpoint permite actualizar solo los campos proporcionados en el cuerpo de la solicitud.
        /// El ID del paciente se debe especificar en la URL.
        /// </remarks>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            command.Id = id;

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return Ok(new { message = $"Paciente con Id #{id} fue actualizado con éxito." });
                }
                else
                {
                    return NotFound(response.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el paciente: {ex.Message}");
            }
        }


        /// <summary>
        /// Elimina un paciente existente por su ID.
        /// </summary>
        /// <param name="id">ID del paciente a eliminar.</param>
        /// <returns>Respuesta HTTP indicando el éxito o error de la eliminación.</returns>
        /// <remarks>
        /// Este endpoint permite eliminar un paciente del sistema utilizando su identificador único.
        ///
        /// Ejemplo de solicitud:
        ///
        ///     DELETE /api/patients/5
        ///
        /// Ejemplo de respuesta (204 No Content - Éxito):
        ///
        ///     // Sin cuerpo de respuesta
        ///
        /// Ejemplo de respuesta (404 Not Found - Paciente no encontrado):
        ///
        ///     {
        ///         "message": "Paciente con ID 5 no encontrado."
        ///     }
        ///
        /// Ejemplo de respuesta (500 Internal Server Error - Error al eliminar):
        ///
        ///     {
        ///         "message": "Error inesperado al eliminar el paciente con ID 5: Error de base de datos."
        ///     }
        /// </remarks>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var command = new DeletePatientCommand { PatientId = id };

            try
            {
                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return NoContent(); // Código 204 para indicar una eliminación exitosa
                }
                else
                {
                    return NotFound(response.Message); // Código 404 si el paciente no se encuentra
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error inesperado al eliminar el paciente: {ex.Message}");
            }
        }


        //RELACIONADO A ARCHIVAR/DESARCHIVAR Y OBTENER ARCHIVADOS:


        /// <summary\>
        /// Archiva un paciente existente por su ID\.
        /// </summary\>
        /// <param name\="id"\>ID del paciente a archivar\.</param\>
        /// <returns\>Respuesta HTTP indicando el éxito o error del archivado\.</returns\>
        [HttpPut("pacientes/{id}/archive")]
        public async Task<ActionResult<ArchivePatientResponse>> ArchivePaciente(int id)
        {
            var command = new ArchivePatientCommand(id);
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(new { message = response.Message });
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        /// <summary\>
        /// Desarchiva un paciente existente por su ID\.
        /// </summary\>
        /// <param name\="id"\>ID del paciente a archivar\.</param\>
        /// <returns\>Respuesta HTTP indicando el éxito o error del archivado\.</returns\>
        [HttpPut("pacientes/{id}/unarchive")]
        public async Task<ActionResult<ArchivePatientResponse>> UnarchivePaciente(int id)
        {
            var command = new UnarchivePatientCommand(id);
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(new { message = response.Message });
            }
            else
            {
                return NotFound(response.Message);
            }

        }


        /// <summary>
        /// Obtiene todos los pacientes que están deshabilitados (IsEnabled = false) con toda su información.
        /// </summary>
        /// <returns>Una lista de todos los pacientes deshabilitados.</returns>
        /// <response code="200">Retorna la lista de pacientes deshabilitados.</response>
        [HttpGet("disabled")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PatientDTO>>> GetAllDisabledPatients()
        {
            var query = new GetAllArchivedPatientsCommand();
            var disabledPatients = await _mediator.Send(query);
            return Ok(disabledPatients);
        }



        //RELACIONADO A MATERIAL DE PACIENTE:


        /// <summary>
        /// Crea un nuevo material para un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="createPacienteMaterialDto">Datos para la creación del material.</param>
        /// <returns>Un ActionResult que indica el resultado de la creación.</returns>
        /// <response code="201">Retorna el material recién creado.</response>
        /// <response code="400">Si la petición no es válida.</response>
        /// <response code="404">Si el paciente especificado no existe.</response>
        [HttpPost("{patientId}/materials")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreatePatientMaterialResponse>> CreatePatientMaterial(
            [FromRoute] int pacienteId,
            [FromBody] CreatePatientMaterialDto createPacienteMaterialDto)
        {
            if (createPacienteMaterialDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CreatePatientMaterialCommand
            {
                PatientId = pacienteId,
                Material = createPacienteMaterialDto
            };

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return CreatedAtAction(nameof(GetPatientMaterialById),
                    new { pacienteId = pacienteId, materialId = response.PatientMaterial.Id },
                    response);
            }

            if (response.Message.Contains("No se encontró el paciente"))
            {
                return NotFound(response.Message);
            }

            return BadRequest(response.Message);
        }


        /// <summary>
        /// Obtiene todo el material asociado a un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <returns>Una lista de materiales del paciente.</returns>
        /// <response code="200">Retorna la lista de materiales del paciente.</response>
        /// <response code="404">Si el paciente especificado no existe.</response>
        [HttpGet("{patientId}/materials")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PatientMaterialDto>>> GetAllPatientMaterials([FromRoute] int patientId)
        {
            // Crear el query para obtener el material del paciente
            var query = new GetAllPatientMaterialsCommand { PatientId = patientId };

            // Enviar el query al Mediator y obtener la respuesta
            var materials = await _mediator.Send(query);

            if (materials != null && materials.Count > 0)
            {
                return Ok(materials);
            }

            var patientExists = await _mediator.Send(new GetPatientByIdQuery(patientId));
            if (patientExists == null)
            {
                return NotFound($"No se encontró el paciente con ID: {patientId}");
            }

            return Ok(new List<PatientMaterialDto>()); // Paciente encontrado, pero no tiene material
        }


        /// <summary>
        /// Obtiene un material específico por su ID para un paciente específico.
        /// </summary>
        /// <param name="patientId">Identificador único del paciente.</param>
        /// <param name="materialId">Identificador único del material.</param>
        /// <returns>El material solicitado.</returns>
        /// <response code="200">Retorna el material solicitado.</response>
        /// <response code="404">Si el paciente o el material no existen.</response>
        [HttpGet("{patientId}/materials/{materialId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientMaterialDto>> GetPatientMaterialById([FromRoute] int patientId, [FromRoute] int materialId)
        {
            var query = new GetPatientMaterialByIdQuery { PatientId = patientId, MaterialId = materialId };
            var materialDto = await _mediator.Send(query);

            if (materialDto != null)
            {
                return Ok(materialDto);
            }

            return NotFound($"No se encontró el material con ID: {materialId} para el paciente con ID: {patientId}");
        }


        //ENDPOINTS DE BUSQUEDA 

        /// <summary>
        /// Busca un paciente por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del paciente.</param>
        /// <returns>Paciente si se encuentra, 404 si no se encuentra.</returns>
        [HttpGet("search-by-email")]
        public async Task<IActionResult> GetPatientByEmail(string email)
        {
            var paciente = await _patientService.GetPatientByEmailAsync(email);
            if (paciente == null)
            {
                return NotFound($"Paciente con email {email} no encontrado.");
            }
            return Ok(paciente);
        }


        /// <summary>
        /// Busca un paciente por su número de identificación.
        /// </summary>
        /// <param name="identification">Número de identificación del paciente a buscar (como parámetro de consulta).</param>
        /// <returns>Respuesta HTTP con la información del paciente encontrado o NotFound si no existe.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con el número de identificación proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-identification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPatientByIdResponse>> GetPatientByIdentification([FromQuery] string identification)
        {
            var query = new GetPatientByIdentificationCommand(identification);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patient != null)
            {
                return Ok(response.Patient);
            }
            else if (response.Success)
            {
                return NotFound(" No se encontró ningún paciente con esa identificación"); // No se encontró ningún paciente con esa identificación
            }
            else
            {
                return BadRequest(response.Message); // O un StatusCode más apropiado para el error
            }
        }

        /// <summary>
        /// Busca un paciente por su nacionalidad.
        /// </summary>
        /// <param name="nationality"> String nacionalidad del paciente(como parámetro de consulta).</param>
        /// <returns>Respuesta HTTP con la información del paciente encontrado o NotFound si no existe.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con la nacionalidad proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-nationality")]
        public async Task<ActionResult<GetPatientsByNationalityResponse>> GetPatientsByNationality([FromQuery] string nationality)
        {
            var query = new GetPatientsByNationalityCommand(nationality);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null)
            {
                return Ok(response.Patients);
            }
            else if (response.Success)
            {
                return NotFound();
            }
            else
            {
                return BadRequest(response.Message); // O un StatusCode??
            }
        }

        /// <summary>
        /// Busca un paciente por su nombre.
        /// </summary>
        /// <param name="fullname"> String nombre del paciente(como parámetro de consulta).</param>
        /// <returns>Respuesta HTTP con la información del paciente encontrado o NotFound si no existe.</returns>
        /// <response code="200">Paciente encontrado exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontró ningún paciente con la nacionalidad proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("search-by-fullname")]
        public async Task<ActionResult<GetPatientsByNationalityResponse>> GetPatientsByFullName([FromQuery] string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname))
            {
                return BadRequest("El nombre completo no puede estar vacío.");
            }

            var query = new GetPatientsByFullNameCommand(fullname.Trim());
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null && response.Patients.Any())
            {
                return Ok(response.Patients);
            }
            else
            {
                return NotFound($"No se encontraron pacientes con el nombre completo: {fullname.Trim()}");
            }
        }

        //FILTROS 

        /// <summary>
        /// Filtra los pacientes por tipo de sexo.
        /// </summary>
        /// <param name="sexType">Tipo de sexo del paciente.</param>
        /// <returns>Lista de pacientes filtrados por tipo de sexo.</returns>
        /// <returns>Respuesta HTTP con la lista de pacientes que coinciden con la el tipo de sexo proporcionada o NotFound si no existe ninguno.</returns>
        /// <response code="200">Lista de pacientes filtrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontraron pacientes con el tipo de sexo proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter-by-sexType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]       
        public async Task<ActionResult<GetPatientsBySexTypeResponse>> GetPatientsBySex([FromQuery] string sex)
        {
            var query = new GetPatientsBySexTypeCommand(sex);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null)
            {
                return Ok(response.Patients);
            }
            else if (response.Success)
            {
                return NotFound($"No se encontraron pacientes con tipo de sexo {sex}"); 
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// Filtra los pacientes por modalidad.
        /// </summary>
        /// <param name="modality">Modalidad del paciente.</param>
        /// <returns>Lista de pacientes filtrados por modalidad.</returns>
        ///   /// <returns>Respuesta HTTP con la lista de pacientes que coinciden con la modalidad proporcionada o NotFound si no existe ninguno.</returns>
        /// <response code="200">Lista de pacientes filtrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontraron pacientes con la modalidad proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter-by-modality")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]     
        public async Task<IActionResult> GetPatientByModality(string modality)
        {
            var patients = await _patientService.GetPacientesByModalityAsync(modality);
            if (patients == null || patients.Count == 0)
            {
                return NotFound("No se encontraron pacientes con esa modalidad.");
            }
            return Ok(patients);
        }


        /// <summary>
        /// Filtra pacientes por rango etario.
        /// </summary>
        /// <param name="ageRange">Rango etario de los pacientes a filtrar (como parámetro de consulta).</param>
        /// <returns>Respuesta HTTP con la lista de pacientes que coinciden con el rango etario proporcionado o NotFound si no existe ninguno.</returns>
        /// <response code="200">Lista de pacientes filtrados exitosamente.</response>
        /// <response code="400">Error en la solicitud.</response>
        /// <response code="404">No se encontraron pacientes en el rango etario proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter-by-age-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetPatientsByAgeRangeResponse>> GetPatientsByAgeRange([FromQuery] string ageRange)
        {
            var query = new GetPatientsByAgeRangeCommand(ageRange);
            var response = await _mediator.Send(query);

            if (response.Success && response.Patients != null)
            {
                return Ok(response.Patients);
            }
            else if (response.Success)
            {
                return NotFound($"No se encontraron pacientes {ageRange} de rango etario."); 
            }
            else
            {
                return BadRequest(response.Message); // O un StatusCode?
            }
        }

    }
}