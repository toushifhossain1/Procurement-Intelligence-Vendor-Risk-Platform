using System.Net.Http.Headers;
using IntegrationServices.Clients;
using IntegrationServices.Models;
using IntegrationServices.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add API explorer / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind Business Central configuration
builder.Services.Configure<BusinessCentralOptions>(
    builder.Configuration.GetSection("BusinessCentral"));

// SQL Server / Entity Framework Core
builder.Services.AddDbContext<ProcurementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// Register Business Central HTTP client
builder.Services.AddHttpClient<IBusinessCentralClient, BusinessCentralClient>(
    (serviceProvider, client) =>
    {
        var configuration = serviceProvider
            .GetRequiredService<IConfiguration>();

        var baseUrl = configuration["BusinessCentral:BaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "BusinessCentral:BaseUrl is not configured.");
        }

        client.BaseAddress = new Uri(baseUrl);

        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    });

var app = builder.Build();

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();