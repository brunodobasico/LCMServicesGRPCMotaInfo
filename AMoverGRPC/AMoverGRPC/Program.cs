using AMoverGRPC.Data;
using AMoverGRPC.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar suporte a autentica��o com JWT (Keycloak)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://172.20.0.201:8080/realms/Amover"; // URL do realm no Keycloak
        options.Audience = "lcmsrv"; // client_id que est�s a usar
        options.RequireHttpsMetadata = false; // como est�s a usar HTTP
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

builder.Services.AddAuthorization();

// 2. Registar servi�os gRPC com interceptor (opcional, mas bom para logs/autentica��o manual)
builder.Services.AddGrpc(options =>
{
    // podes adicionar interceptores aqui, ex:
    // options.Interceptors.Add<AuthInterceptor>();
});

// 3. Configurar liga��o � base de dados
builder.Services.AddDbContext<MotaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// 4. Ativar autentica��o e autoriza��o no pipeline
app.UseAuthentication();
app.UseAuthorization();

// 5. Mapear servi�os gRPC
app.MapGrpcService<GreeterService>();
app.MapGrpcService<MotaService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
