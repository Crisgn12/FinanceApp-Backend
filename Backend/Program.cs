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
              .AllowAnyMethod()
              .AllowAnyHeader();
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
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding
                                                        .UTF8
                                                        .GetBytes(builder.Configuration["JWT:Key"]))

        };
    });
#endregion

// Inyeccion de dependencias
#region DI
builder.Services.AddDbContext<FinanceAppContext>();
builder.Services.AddScoped<ITokenService, TokenService>();

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
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
