using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Models;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// add local aliases so Configuration and services names exist
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//============Swagger===========================================
//Step 0:

//Nuget - Swashbuckle.AspNetCore
//launchSettings.json - Add following lines in the "profiles" section of your project
//  "launchBrowser": true,
//  "launchUrl": "Swagger",


//Step 1: Register the Swagger generator
builder.Services.AddSwaggerGen();

//========================================================

var appSettingsSection = configuration.GetSection("AppSettings");
services.Configure<AppSettings>(appSettingsSection);

var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.key);

services.AddAuthentication(au =>
{
    au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.RequireHttpsMetadata = false;
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

services.AddScoped<IAuthenticateService, AuthenticateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    //===========Swagger==============================================================================

    //Step 2: To Add Swagger - Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger(); //

    //Step 3: To Add Swagger -  Enable middleware to serve Swagger UI
    app.UseSwaggerUI(options =>
    {
        // Adjust the endpoint path if you are using newer .NET OpenAPI templates
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); //
    });
    //==============================================================
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
