using Application.Extensions;
using Repository.Extensions;
using Repository.Seeders;
using Shared.Extensions;
using System.Reflection;
using System.Text.Json;
using Serilog;
using System.Text.Json.Serialization;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
//builder.Services.AddSerilog();
//builder.Services.AddCustomLogConfiguration(builder.Configuration);
builder.Services.AddApiVersioningExtension();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = $"Clean Architecture API - {builder.Environment.EnvironmentName}",
        Version = "v1",
    });
});
builder.Services.AddApplicationServices();
builder.Services.AddRepositoryServices(builder.Configuration);
builder.Services.AddSharedServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseErrorHandlerMiddleware();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.ApplyMigrations();
await SeedData.InitializeDataAsync(app.Services);
app.Run();
