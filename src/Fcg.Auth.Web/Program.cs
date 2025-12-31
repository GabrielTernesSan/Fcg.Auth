using Fcg.Auth.Application;
using Fcg.Auth.Application.Requests;
using Fcg.Auth.Domain.Queries;
using Fcg.Auth.Infra;
using Fcg.Auth.Proxy.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayer();
builder.Services.AddInfraLayer(builder.Configuration);
builder.Services.AddInfraProxyUser(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT com prefixo 'Bearer '"
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
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});
#endregion

var app = builder.Build();

#region Minimal APIs
app.MapPost("auth/create-account", async (RegisterUserRequest request, IMediator mediator) =>
{
    var result = await mediator.Send(request);

    return result.HasErrors
        ? Results.BadRequest(result)
        : Results.Ok(result);
}).AllowAnonymous()
.WithTags("Auth");

app.MapGet("auth/users/{id}/email", async (Guid id, IAuthQuery _authQuery) =>
{
    var response = await _authQuery.GetEmailByUserIdAsync(id);

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Auth");

app.MapDelete("auth/users/{id}", async (Guid id, IMediator mediator) =>
{
    var response = await mediator.Send(new DeleteUserRequest { Id = id });

    return Results.Ok(response);
}).AllowAnonymous().WithTags("Auth");

app.MapPut("auth/role", async ([FromBody] ChangeUserRoleRequest request, IMediator mediator) =>
{
    var response = await mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous()
.WithTags("Auth");

app.MapPut("auth/users/{id}/email", async (Guid id, [FromBody] ChangeUserEmailRequest request, IMediator mediator) =>
{
    request.UserId = id;

    var response = await mediator.Send(request);

    return Results.Ok(response);
}).AllowAnonymous()
.WithTags("Auth");

app.MapPost("auth/login", async (LoginRequest request, IMediator mediator) =>
{
    var response = await mediator.Send(request);

    return Results.Ok(response);
})
.AllowAnonymous();
#endregion

#region Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion
