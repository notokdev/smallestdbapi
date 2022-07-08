using Microsoft.EntityFrameworkCore;
using SmallestDbApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.Configure<Configuration.AppSettings>(Configuration.GetSection(nameof(api.Configuration.AppSettings)));

var connString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DbContext>(options => options
    .UseMySql(connString, MySqlServerVersion.AutoDetect(connString)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
