using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsWithASPNetEF.EndPoints;
using MinimalAPIsWithASPNetEF.Entities;
using MinimalAPIsWithASPNetEF.Repositories;
using MinimalAPIsWithASPNetEF.Services;

var builder = WebApplication.CreateBuilder(args);

// register connection
var config = builder.Configuration;
// The following DbContext object is for SQL Server
builder.Services.AddDbContext<AppDbCtx>(options => options.UseSqlServer(config.GetConnectionString("SqlServerDb")));

builder.Services.AddOutputCache();

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

// add mapper
builder.Services.AddAutoMapper(typeof(Program));

// add validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// add problem details
builder.Services.AddProblemDetails();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
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
        var err = new Error();
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

app.MapControllers();

app.UseOutputCache();

// Endpoints
//app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => {
    throw new InvalidOperationException("sample error!");
});
app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("movies/{movieId:int}/comments").MapComments();

// Middlewares zone ended
app.Run();
