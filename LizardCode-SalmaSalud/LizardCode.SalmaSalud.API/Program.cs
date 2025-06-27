using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;
using LizardCode.SalmaSalud.Application;
using LizardCode.SalmaSalud.Infrastructure;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.Application.Helpers;
using LizardCode.SalmaSalud.API.Infrastructure.ApiKey;
using Microsoft.Net.Http.Headers;
using LizardCode.SalmaSalud.API.Infrastructure.JWT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Authority"],
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
        };
    })
    .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>("ApiKeyScheme", options => { });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    };

    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        []
                    }
                };

    o.AddSecurityRequirement(securityRequirement);

    o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };

    var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
    o.AddSecurityRequirement(requirement);
});

builder.Services.AddSingleton<TokenProvider>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddHttpContextAccessor();

var _httpContextAccessor = new HttpContextAccessor();

builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton(_httpContextAccessor);

HttpContextHelper.Configure(_httpContextAccessor);
Extensions.Configure(builder.Configuration, _httpContextAccessor, null);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure();

//var _httpContextAccessor = new HttpContextAccessor();

var app = builder.Build();

Extensions.Configure(app.Configuration, null, null);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

AppDomain.CurrentDomain.SetData("ContentRootPath", app.Environment.ContentRootPath);
AppDomain.CurrentDomain.SetData("WebRootPath", app.Environment.WebRootPath);

app.Run();
