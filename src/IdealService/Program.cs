using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("ideal-service"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Database Initialization
void InitDb()
{
    var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connString)) return;

    try
    {
        using var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS access_logs (
                id SERIAL PRIMARY KEY,
                accessed_at TIMESTAMP NOT NULL,
                pod_name VARCHAR(100)
            )", conn);
        cmd.ExecuteNonQuery();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB Init Error: {ex.Message}");
    }
}

InitDb();

app.MapGet("/", () => 
{
    var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    int visitCount = 0;
    string dbStatus = "Disconnected";

    if (!string.IsNullOrEmpty(connString))
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            
            using var insertCmd = new NpgsqlCommand("INSERT INTO access_logs (accessed_at, pod_name) VALUES (@time, @pod)", conn);
            insertCmd.Parameters.AddWithValue("time", DateTime.UtcNow);
            insertCmd.Parameters.AddWithValue("pod", Environment.MachineName);
            insertCmd.ExecuteNonQuery();

            using var countCmd = new NpgsqlCommand("SELECT COUNT(*) FROM access_logs", conn);
            visitCount = Convert.ToInt32(countCmd.ExecuteScalar());
            dbStatus = "Connected (PostgreSQL Latest)";
        }
        catch (Exception ex)
        {
            dbStatus = $"Error: {ex.Message}";
        }
    }

    var html = $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>ideal-service - System & DB Info</title>
        <style>
            :root { --bg: #f1f5f9; --card: #ffffff; --text: #1e293b; --accent: #2563eb; --accent-hover: #1d4ed8; --success: #10b981; }
            body { font-family: 'Inter', system-ui, sans-serif; background-color: var(--bg); color: var(--text); padding: 40px; margin: 0; }
            .container { background: var(--card); border-radius: 16px; padding: 40px; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1); max-width: 900px; margin: 0 auto; border: 1px solid rgba(0, 0, 0, 0.05); }
            h1 { font-size: 2.5rem; margin-bottom: 10px; background: linear-gradient(135deg, #2563eb, #7c3aed); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
            p.subtitle { color: #64748b; font-size: 1.1rem; margin-bottom: 30px; }
            .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }
            .info-card { background: #f8fafc; padding: 20px; border-radius: 12px; border: 1px solid rgba(0, 0, 0, 0.05); }
            .info-card h3 { margin: 0 0 10px 0; font-size: 0.85rem; text-transform: uppercase; color: var(--accent); }
            .info-card span { font-size: 1.1rem; font-weight: 600; color: #334155; word-break: break-all; }
            .db-section { background: #eff6ff; padding: 20px; border-radius: 12px; border: 1px solid #bfdbfe; margin-bottom: 30px; }
            .db-section h2 { color: #1e40af; margin-top: 0; }
            .db-stat { font-size: 1.5rem; font-weight: bold; color: var(--accent); }
            .architecture { background: #f8fafc; padding: 20px; border-radius: 12px; border: 1px solid #e2e8f0; margin-bottom: 30px; }
            .architecture h3 { margin-top: 0; }
            ul { line-height: 1.6; }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>ideal-service Enterprise (HOTFIX)</h1>
            <p class="subtitle">Version {{Environment.GetEnvironmentVariable("HELM_CHART_VERSION") ?? "1.0.0"}} &bull; Cloud-Native .NET</p>
            
            <div class="db-section">
                <h2>🗄️ PostgreSQL Database Integration</h2>
                <p>Status: <strong>{{dbStatus}}</strong></p>
                <p>Total API Requests Logged to Database: <span class="db-stat">{{visitCount}}</span></p>
            </div>

            <div class="architecture">
                <h3>🏗️ Architecture & Production Features</h3>
                <ul>
                    <li><strong>GitOps CD:</strong> Fully managed by ArgoCD targeting specific namespaces (dev, staging, prod).</li>
                    <li><strong>Database:</strong> Bitnami PostgreSQL 15 deployed as a Helm dependency.</li>
                    <li><strong>Secret Management:</strong> Bitnami SealedSecrets integrated into Helm for encrypted credentials.</li>
                    <li><strong>Security:</strong> Trivy scans Docker image & codebase; CodeQL statically analyzes C# for bugs.</li>
                    <li><strong>Observability:</strong> OpenTelemetry is actively exporting metrics and distributed traces via OTLP.</li>
                    <li><strong>Multi-Arch:</strong> Docker image built for both linux/amd64 and linux/arm64.</li>
                </ul>
            </div>

            <div class="info-grid">
                <div class="info-card"><h3>Helm Release</h3><span>{{Environment.GetEnvironmentVariable("HELM_RELEASE_NAME") ?? "Local"}}</span></div>
                <div class="info-card"><h3>Namespace</h3><span>{{Environment.GetEnvironmentVariable("POD_NAMESPACE") ?? "default"}}</span></div>
                <div class="info-card"><h3>Pod Name</h3><span>{{Environment.MachineName}}</span></div>
                <div class="info-card"><h3>Framework</h3><span>{{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}}</span></div>
            </div>
        </div>
    </body>
    </html>
    """;
    return Results.Content(html, "text/html");
});

app.MapGet("/api/v1/info", () => Results.Ok(new 
{ 
    service = "ideal-service",
    status = "healthy",
    timestamp = DateTime.UtcNow
}))
.WithName("ApiInfo")
.WithOpenApi();

app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
