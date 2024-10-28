using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.Middleware;
using ToDoAPI.Services;
using ToDoAPI.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa; Password= {dbPassword};TrustServerCertificate=True";
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ToDoItemValidator>());

builder.Services.AddScoped<IToDoService, ToDoService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

//automatic migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();


app.Run();
