using Backend.Services.Implementaciones;
using Backend.Services.Implementations;
using Backend.Services.Interfaces;
using DAL.Implementaciones;
using DAL.Interfaces;
using Entidades.Entities;
using Entities.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DB

builder.Services.AddDbContext<FinanceAppContext>(
                                options =>
                                options.UseSqlServer(
                                    builder
                                    .Configuration
                                    .GetConnectionString("DefaultConnection")
                                        ));

builder.Services.AddDbContext<AuthDBContext>(
                                options =>
                                options.UseSqlServer(
                                    builder
                                    .Configuration
                                    .GetConnectionString("DefaultConnection")
                                        ));

#endregion

#region Identity
builder.Services.AddIdentityCore<IdentityUser>()
                        .AddRoles<IdentityRole>()
                        .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("UNA")
                        .AddEntityFrameworkStores<AuthDBContext>()
                        .AddDefaultTokenProviders();



builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

});
#endregion

#region JWT

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});
#endregion

// Inyeccion de dependencias
#region DI
builder.Services.AddDbContext<FinanceAppContext>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();

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
builder.Services.AddScoped<IPagoProgramadoDAL, PagoProgramadoDAL>();
builder.Services.AddScoped<IPagoProgramadoService, PagoProgramadoService>();
builder.Services.AddScoped<IEventosFinancieroDAL, EventosFinancieroDALImpl>();
builder.Services.AddScoped<IEventosFinancieroService, EventosFinancieroService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
