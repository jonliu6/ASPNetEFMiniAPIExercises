using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIsWithASPNetEF.EndPoints;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.GraphQL;
using MinimalAPIsWithASPNetEF.Repositories;
using MinimalAPIsWithASPNetEF.Services;
using MinimalAPIsWithASPNetEF.Utilities;
using System.Security.Cryptography.Xml;

var builder = WebApplication.CreateBuilder(args);

// register connection
var config = builder.Configuration;
// The following DbContext object is for SQL Server
builder.Services.AddDbContext<AppDbCtx>(options => options.UseSqlServer(config.GetConnectionString("SqlServerDb")));

// GraphQL
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddAuthorization()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// register Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<AppDbCtx>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>(); // allowing users to sign/log in

builder.Services.AddOutputCache();

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUsersService, UsersService>();

// add mapper
builder.Services.AddAutoMapper(typeof(Program));

// add validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// add problem details
builder.Services.AddProblemDetails();

// add authentication and authorization
builder.Services.AddAuthentication().AddJwtBearer(options => 
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = KeysHandler.GetAllKeys(builder.Configuration)
        // IssuerSigningKey = KeysHandler.GetKey(builder.Configuration).First() // this is for only one key from particular issuer
    };
}); // this authentication schema requires JWTBearer package
builder.Services.AddAuthorization(options =>
{
    // add a policy consists claim name = isadmin
    options.AddPolicy("isadmin", policy => policy.RequireClaim("isadmin"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Minimal APIs",
        Description = "This is a sample solution for Minimal APIs",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "tester@demo.freecode.org",
            Name = "Tester 123",
            Url = new Uri("https://www.google.ca")
        }
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme{ 
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    }); // When authorize in Swagger, put "Bearer" in front of a valid token

});
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Middlewares zone started
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => {
        // get the error details
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error!;
        var err = new CustomError();
        err.ErrorDate = DateTime.Now;
        err.ErrorMessage = exception.Message;
        err.StackTrace = exception.StackTrace;

        // save the error
        var repo = context.RequestServices.GetRequiredService<IErrorsRepository>();
        await repo.create(err);

        await Results.BadRequest(new { 
            type = "error",
            message = "An unexpected error occurred!",
            status = 500
        }).ExecuteAsync(context);
    })); // handle errors
    app.UseStatusCodePages(); // HTTP status code error pages

    app.UseCors();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// for GraphSQL
app.MapGraphQL();

app.MapControllers();

app.UseOutputCache();

// Endpoints
//app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => {
    throw new InvalidOperationException("sample error!");
});

// model binding - http://<server>:<port>/modelBinding?userName=John, FromQuery is default, and also can have FromBody, FromForm, FromHeader, FromRoute, FromServices etc 
app.MapPost("/modelBinding", ([FromQuery(Name ="userName")] string? name) => { 
    if (name is null)
    {
        name = "Empty";
    }
    return TypedResults.Ok(name);
});

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movies/{movieId:int}/comments").MapComments();
app.MapGroup("/users").MapUsers();

// Middlewares zone ended
app.Run();
