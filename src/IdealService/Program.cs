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

app.MapGet("/", () => 
{
    var html = """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>ideal-service - System Info</title>
        <style>
            :root { --bg: #0f172a; --card: #1e293b; --text: #f8fafc; --accent: #3b82f6; --accent-hover: #60a5fa; }
            body { font-family: 'Inter', system-ui, sans-serif; background-color: var(--bg); color: var(--text); display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; }
            .container { background: var(--card); border-radius: 16px; padding: 40px; box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5); max-width: 600px; width: 100%; text-align: center; border: 1px solid rgba(255, 255, 255, 0.1); }
            h1 { font-size: 2.5rem; margin-bottom: 10px; background: linear-gradient(135deg, #60a5fa, #a78bfa); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
            p { color: #94a3b8; font-size: 1.1rem; margin-bottom: 30px; }
            .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }
            .info-card { background: rgba(0, 0, 0, 0.2); padding: 20px; border-radius: 12px; border: 1px solid rgba(255, 255, 255, 0.05); transition: transform 0.2s; }
            .info-card:hover { transform: translateY(-5px); }
            .info-card h3 { margin: 0 0 10px 0; font-size: 0.9rem; text-transform: uppercase; letter-spacing: 1px; color: var(--accent); }
            .info-card span { font-size: 1.2rem; font-weight: 600; }
            .btn { display: inline-block; padding: 12px 24px; background: var(--accent); color: white; text-decoration: none; border-radius: 8px; font-weight: 600; transition: background 0.2s; }
            .btn:hover { background: var(--accent-hover); }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>ideal-service</h1>
            <p>Version 1.0.0 &bull; Running Smoothly</p>
            <div class="info-grid">
                <div class="info-card">
                    <h3>OS Architecture</h3>
                    <span>""" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString() + """</span>
                </div>
                <div class="info-card">
                    <h3>Framework</h3>
                    <span>""" + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription + """</span>
                </div>
                <div class="info-card">
                    <h3>Machine Name</h3>
                    <span>""" + Environment.MachineName + """</span>
                </div>
                <div class="info-card">
                    <h3>Processors</h3>
                    <span>""" + Environment.ProcessorCount + """ Core(s)</span>
                </div>
            </div>
            <a href="/api/v1/info" class="btn">View Raw API Info</a>
        </div>
    </body>
    </html>
    """;
    return Results.Content(html, "text/html");
});

app.MapGet("/api/v1/info", () => Results.Ok(new 
{ 
    service = "ideal-service",
    version = "1.0.0",
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    machine = Environment.MachineName,
    os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
    framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription
}))
.WithName("ApiInfo")
.WithOpenApi();

app.MapHealthChecks("/health");

app.Run();

// Exposed for WebApplicationFactory-based integration tests.
public partial class Program { }
