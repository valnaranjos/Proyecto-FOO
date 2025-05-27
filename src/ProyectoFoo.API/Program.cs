using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProyectoFoo.Infrastructure.Context;
using ProyectoFoo.Application.ServiceExtension;
using ProyectoFoo.Infrastructure.ServiceExtensions;
using ProyectoFoo.API.Services;
using System.Text;
using ProyectoFoo.Domain.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


// Agregar servicios
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();



var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(secretKey))
{
    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    var logger = loggerFactory.CreateLogger("Program");
    logger.LogError("La variable de entorno JWT_SECRET_KEY no está configurada. La aplicación no puede iniciar correctamente.");
    throw new InvalidOperationException("JWT_SECRET_KEY no configurada.");
}
else
{
    builder.Services.AddSingleton<ITokenService, TokenService>(sp => new TokenService(secretKey, sp.GetRequiredService<IConfiguration>()));
}

// Añade Swagger al contendor
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { Title = "Gestión de Pacientes API", Version = "v1" });

    //Incluye comentarios XML para documentacion
    var xmlFile = $"{System.AppDomain.CurrentDomain.FriendlyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configuración de seguridad para JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduce el token JWT Bearer (ej. 'Bearer eyJ...').",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Añade contexto de base de datos.
builder.Services.AddDbContext<ApplicationContextSqlServer>(options =>
options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
                      b =>
                      {
                          b.MigrationsAssembly("ProyectoFoo.Infrastructure");
                          b.EnableStringComparisonTranslations();
                      })
);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOrigins", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://insight-twya.onrender.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Agregar autenticación JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };    });

var app = builder.Build();


// Habilitar Swagger en todos los entornos
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insight API");
    c.RoutePrefix = string.Empty; // Para mostrar Swagger en la raíz
});



//Se asegura que la base de datos esté creada
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContextSqlServer>();
    db.Database.Migrate(); // Aplica migraciones pendietes.
    Console.WriteLine("✅ Conexión a la base de datos exitosa.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error al conectar con la base de datos: {ex.Message}");
}



// Middleware
app.UseCors("FrontendOrigins");
app.UseHttpsRedirection();
app.UseAuthentication(); // Agrega autenticación
app.UseAuthorization(); // Agrega JWT Bearer
app.MapControllers(); 

app.Run();


app.MapFallbackToFile("index.html");