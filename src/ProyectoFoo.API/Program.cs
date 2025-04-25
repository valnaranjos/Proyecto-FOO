using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Agregar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Add Swagger to the container

//Activar este línea en caso de querer desactivar Swagger en producción
//var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { Title = "Gestión de Pacientes API", Version = "v1" });

    //Include XML comments for better documentation
    var xmlFile = $"{System.AppDomain.CurrentDomain.FriendlyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});


var app = builder.Build();

// Swagger for production only
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestión de Pacientes API");
        c.RoutePrefix = string.Empty;
    });
}*/

// Habilitar Swagger en todos los entornos
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestión de pacientes API");
    c.RoutePrefix = string.Empty; // Para mostrar Swagger en la raíz
});


// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

