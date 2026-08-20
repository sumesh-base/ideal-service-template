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
    var html = $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>ideal-service - System Info</title>
        <style>
            :root { --bg: #f1f5f9; --card: #ffffff; --text: #1e293b; --accent: #2563eb; --accent-hover: #1d4ed8; }
            body { font-family: 'Inter', system-ui, sans-serif; background-color: var(--bg); color: var(--text); display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; padding: 20px; }
            .container { background: var(--card); border-radius: 16px; padding: 40px; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1); max-width: 700px; width: 100%; text-align: center; border: 1px solid rgba(0, 0, 0, 0.05); }
            h1 { font-size: 2.5rem; margin-bottom: 10px; background: linear-gradient(135deg, #2563eb, #7c3aed); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
            p { color: #64748b; font-size: 1.1rem; margin-bottom: 30px; }
            .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }
            .info-card { background: #f8fafc; padding: 20px; border-radius: 12px; border: 1px solid rgba(0, 0, 0, 0.05); transition: transform 0.2s; text-align: left; }
            .info-card:hover { transform: translateY(-5px); box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1); }
            .info-card h3 { margin: 0 0 10px 0; font-size: 0.85rem; text-transform: uppercase; letter-spacing: 1px; color: var(--accent); }
            .info-card span { font-size: 1.1rem; font-weight: 600; color: #334155; word-break: break-all; }
            .btn { display: inline-block; padding: 12px 24px; background: var(--accent); color: white; text-decoration: none; border-radius: 8px; font-weight: 600; transition: background 0.2s; box-shadow: 0 4px 6px -1px rgba(37, 99, 235, 0.2); }
            .btn:hover { background: var(--accent-hover); box-shadow: 0 6px 8px -1px rgba(37, 99, 235, 0.3); }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>ideal-service</h1>
            <p>Version {{Environment.GetEnvironmentVariable("HELM_CHART_VERSION") ?? "1.0.0"}} &bull; Deployment Info</p>
            <div class="info-grid">
                <div class="info-card">
                    <h3>Helm Release</h3>
                    <span>{{Environment.GetEnvironmentVariable("HELM_RELEASE_NAME") ?? "Local/Tilt"}}</span>
                </div>
                <div class="info-card">
                    <h3>Namespace</h3>
                    <span>{{Environment.GetEnvironmentVariable("POD_NAMESPACE") ?? "default"}}</span>
                </div>
                <div class="info-card">
                    <h3>Framework</h3>
                    <span>{{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}}</span>
                </div>
                <div class="info-card">
                    <h3>OS Arch</h3>
                    <span>{{System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString()}}</span>
                </div>
                <div class="info-card">
                    <h3>Host / Pod Name</h3>
                    <span>{{Environment.MachineName}}</span>
                </div>
                <div class="info-card">
                    <h3>CPU Cores</h3>
                    <span>{{Environment.ProcessorCount}} Core(s)</span>
                </div>
            </div>
            <a href="/api/v1/info" class="btn">View Raw JSON Info</a>
        </div>
    </body>
    </html>
    """;
    return Results.Content(html, "text/html");
});

app.MapGet("/api/v1/info", () => Results.Ok(new 
{ 
    service = "ideal-service",
    chart_version = Environment.GetEnvironmentVariable("HELM_CHART_VERSION") ?? "1.0.0",
    helm_release = Environment.GetEnvironmentVariable("HELM_RELEASE_NAME") ?? "Local/Tilt",
    kubernetes_namespace = Environment.GetEnvironmentVariable("POD_NAMESPACE") ?? "default",
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
public partial class Program { }
