using Aspire.Hosting;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

var builder = DistributedApplication.CreateBuilder(args);

var seq = builder
    .AddSeq("seq")
        .WithHttpEndpoint(port: 5341, targetPort: 80, name: "seqlog");
var postgres = builder
    .AddPostgres("postgres")
    .AddDatabase("gaming");

if (builder.Environment.IsProduction())
{
    builder.AddContainer("healthchecks", "xabarilcoding/healthchecksui")
        .WithEnvironment("HEALTHCHECKSUI_HEALTHCHECKSUIPATH", "/healthchecks")
        .WithEnvironment("HEALTHCHECKSUI_HEALTHCHECKSUIPORT", "80")
        .WithEnvironment("HealthChecksUI__HealthChecks__0__Uri", "http://host.docker.internal:8080/health/ready")
        .WithEnvironment("HealthChecksUI__HealthChecks__0__Name", "Readiness checks")
        .WithHttpEndpoint(port: 8081, targetPort: 80);
}

builder
    .AddProject<Projects.RetroGamingWebAPI>("retrogamingwebapi")
        .WithEnvironment("HEALTH_INITIAL_STATE", "Degraded")
        .WithReplicas(1)
    //.WithHttpEndpoint(port: 5000, name: "retrogamingwebapi")
    //.WithHttpsEndpoint(port: 5001, name: "retrogamingwebapi-secure")
    //.WithHttpEndpoint(port: 5002, name: "management")
    .WithReference(seq)
    .WithReference(postgres);

builder.Build().Run();