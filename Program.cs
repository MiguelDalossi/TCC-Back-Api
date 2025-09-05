using System.Security.Claims;
using System.Text;
using ConsultorioMedico.Api.Data;
using ConsultorioMedico.Api.Models;
using ConsultorioMedico.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwt["Issuer"]),
            ValidateAudience = !string.IsNullOrWhiteSpace(jwt["Audience"]),


            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// CORS (ajuste a origin do seu front quando for usar)
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();

// Swagger + Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConsultorioMedico.Api", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT como: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// 🔎 logger simples para ver auth/roles chegando
app.Use(async (ctx, next) =>
{
    var auth = ctx.User?.Identity?.IsAuthenticated ?? false;
    var roles = string.Join(",",
    (ctx.User?.Claims ?? Enumerable.Empty<Claim>())
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
);
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {ctx.Request.Method} {ctx.Request.Path} | Auth={auth} | Roles={roles}");
    await next();
});

// rota de diagnóstico para listar endpoints
app.MapGet("/_routes", (Microsoft.AspNetCore.Routing.EndpointDataSource eds) =>
{
    var routes = eds.Endpoints
        .OfType<Microsoft.AspNetCore.Routing.RouteEndpoint>()
        .Select(e => new
        {
            Route = e.RoutePattern.RawText,
            Methods = string.Join(",", e.Metadata
                .OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
                .FirstOrDefault()?.HttpMethods ?? Array.Empty<string>())
        })
        .OrderBy(r => r.Route);
    return Results.Ok(routes);
});

app.MapControllers();

app.Run();
