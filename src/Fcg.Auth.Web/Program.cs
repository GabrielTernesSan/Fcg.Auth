using Fcg.Auth.Domain.Services;
using Fcg.Auth.Infra;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Services Configuration

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

#region Database
builder.Services.AddDbContext<FcgAuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region MediatR
builder.Services.AddMediatR(fcg =>
    fcg.RegisterServicesFromAssemblyContaining<RegisterUserRequest>());
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

#region Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion

#region Queries
builder.Services.AddScoped<IUserQuery, UserQuery>();
#endregion

#region Domain Services
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserRequest>();
#endregion

#endregion

var app = builder.Build();


#region Minimal APIs

#region User Endpoints
app.MapPost("/api/users", async (RegisterUserRequest request, IValidator<RegisterUserRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/users/{response.UserId}", response)
        : Results.BadRequest(response!.Message);
}).AllowAnonymous().WithTags("Users");

app.MapGet("/api/users/{id}", async (Guid id, IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetByIdUserAsync(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
}).RequireAuthorization().WithTags("Users");

app.MapPut("/api/users/{id}/role", async (Guid id, UpdateRoleRequest request, IValidator<UpdateRoleRequest> validator, IMediator _mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/users/{response.UserId}", response)
        : Results.BadRequest(response!.Message);
}).RequireAuthorization("AdminPolicy").WithTags("Users");

app.MapGet("/api/users", async (IUserQuery _userQuery) =>
{
    var users = await _userQuery.GetAllUsersAsync();

    return users is not null ? Results.Ok(users) : Results.NotFound();
}).RequireAuthorization("AdminPolicy").WithTags("Users");
#endregion

#region Auth Endpoints
app.MapPost("/api/login", async (LoginRequest request, IValidator<LoginRequest> validator, IMediator mediator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var response = await mediator.Send(request);

    if (!response.Success)
        return Results.Json(new { response.Message }, statusCode: StatusCodes.Status401Unauthorized);

    return Results.Ok(new
    {
        response.Token,
        response.UserId,
        response.Email,
        response.Message
    });
}).AllowAnonymous().WithTags("Auth");
#endregion

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseLogMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
#endregion