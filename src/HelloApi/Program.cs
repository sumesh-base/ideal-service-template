var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello", () => Results.Ok(new { message = "Hello, World!" }))
    .WithName("Hello")
    .WithOpenApi();

app.MapHealthChecks("/health");

app.Run();

// Exposed for WebApplicationFactory-based integration tests.
public partial class Program { }
