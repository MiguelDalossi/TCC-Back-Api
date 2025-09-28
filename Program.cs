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

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    // Configurações opcionais do Identity
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwt["Issuer"]),
        ValidateAudience = !string.IsNullOrWhiteSpace(jwt["Audience"]),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Token JWT inválido ou ausente\"}");
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"JWT Token validated for: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Medico", policy => policy.RequireRole("Admin", "MedicoOnly"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins("http://localhost:4200", "http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PdfService>();

builder.Services.AddControllers();

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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (ctx, next) =>
{
    var isAuth = ctx.User?.Identity?.IsAuthenticated ?? false;
    var userName = ctx.User?.Identity?.Name ?? "Anonymous";
    var roles = string.Join(",", ctx.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Array.Empty<string>());
    var userId = ctx.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {ctx.Request.Method} {ctx.Request.Path}");
    Console.WriteLine($"  Auth: {isAuth} | User: {userName} | ID: {userId} | Roles: [{roles}]");

    // Log do header Authorization
    var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine($"  Authorization Header: {authHeader[..Math.Min(50, authHeader.Length)]}...");
    }
    else
    {
        Console.WriteLine("  No Authorization Header");
    }

    await next();
});

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

app.MapGet("/_auth-info", (HttpContext ctx) =>
{
    return Results.Ok(new
    {
        IsAuthenticated = ctx.User?.Identity?.IsAuthenticated ?? false,
        UserName = ctx.User?.Identity?.Name,
        UserId = ctx.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        Roles = ctx.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToArray() ?? Array.Empty<string>(),
        AllClaims = ctx.User?.Claims?.Select(c => new { c.Type, c.Value }).ToArray() ?? Array.Empty<object>(),
        HasAdminRole = ctx.User?.IsInRole("Admin") ?? false,
        HasMedicoRole = ctx.User?.IsInRole("Medico") ?? false,
        HasRecepcaoRole = ctx.User?.IsInRole("Recepcao") ?? false
    });
});

app.MapControllers();

app.Run();