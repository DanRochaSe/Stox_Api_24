using Microsoft.EntityFrameworkCore;
using webapi.Helpers;
using webapi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using webapi.Data;
using Microsoft.Extensions.Configuration;
using Serilog;
using Newtonsoft;
using webapi.Repository;
using AutoMapper;
using webapi.Services;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.OpenApi.Writers;
using System.IdentityModel.Tokens.Jwt;
using webapi.Entities;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.EntityFrameworkCore.Diagnostics;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/StoxRollilng.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DbConnectionString");




// Add services to the container.

builder.Services.AddControllers(endpoint =>
{
    endpoint.ReturnHttpNotAcceptable = true;

}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();


builder.Services.AddDbContext<StoxContext>(options => options.UseSqlServer(connString));
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connString));


builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStoxRepository, StoxRepository>();
builder.Services.AddScoped<ICompareRepository, CompareRepository>();

#if DEBUG
builder.Services.AddTransient<IMailService, CloudMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

////For Auth0
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{

//    options.Authority = "https://danielrochamz.eu.auth0.com/";
//    options.Audience = "https://stox_live";
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        NameClaimType = ClaimTypes.NameIdentifier
//    };
//});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "token";
    })
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidIssuers = new List<string> {
            builder.Configuration["Authentication:Issuer"],
            "https://localhost:44323/"
        },
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["token"];
            if (string.IsNullOrEmpty(accessToken) == false)
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});




builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy(
    //    "read:stox",
    //    policy => policy.Requirements.Add(
    //        new HasScopeRequirement("read:stox", "danielrochamz.eu.auth0.com")
    //    )
    //);
    //options.AddPolicy(
    //    "update:stox",
    //    policy => policy.Requirements.Add(
    //        new HasScopeRequirement("update:stox", "danielrochamz.eu.auth0.com")
    //    )
    //);
    //options.AddPolicy(
    //    "create:stox",
    //    policy => policy.Requirements.Add(
    //        new HasScopeRequirement("create:stox", "danielrochamz.eu.auth0.com")
    //    )
    //);
    //options.AddPolicy(
    //    "delete:stox",
    //    policy => policy.Requirements.Add(
    //        new HasScopeRequirement("delete:stox", "danielrochamz.eu.auth0.com")
    //    )
    //);
    options.AddPolicy(
        "RequireAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, StoxEnum.RoleType.Admin.GetDisplayName()));;

});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

//builder.Services.AddCors(opt =>
//{
//    opt.AddPolicy("AllowAll", builder =>
//    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//});


//Add api versioning
builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    //app.UseCors(MyAllowSpecificOrigins);
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self'; font-src 'self'; img-src 'self'; frame-src 'self'; connect-src *;");
//    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
//    await next();
//});

//app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
