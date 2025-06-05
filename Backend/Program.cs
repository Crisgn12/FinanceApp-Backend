using Backend.Services.Implementaciones;
using Backend.Services.Interfaces;
using DAL.Implementaciones;
using DAL.Interfaces;
using Entidades.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// String de conexion a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No se encontr� la cadena de conexi�n 'DefaultConnection'.");
builder.Services.AddDbContext<FinanceAppContext>(options =>
    options.UseSqlServer(connectionString));

// Inyeccion de dependencias
builder.Services.AddDbContext<FinanceAppContext>();
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioDAL, UsuarioDAL>();
builder.Services.AddScoped<IAhorroService, AhorroService>();
builder.Services.AddScoped<IAhorroDAL, AhorroDALImpl>();
builder.Services.AddScoped<IAporteMetaAhorroService, AporteMetaAhorroService>();
builder.Services.AddScoped<IAporteMetaAhorroDAL, AporteMetaAhorroDALImpl>();
builder.Services.AddScoped<ICategoriaDAL, CategoriaDAL>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITransaccionDAL, TransaccionDAL>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
