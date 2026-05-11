using Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

var consulClient = new ConsulClient(c => c.Address = new Uri("http://consul:8500"));
var registration = new AgentServiceRegistration()
{
    ID = "order-service-" + Environment.MachineName,
    Name = "order-service",
    Address = Environment.GetEnvironmentVariable("SERVICE_HOST") ?? "orderwebapi",
    Port = 80,
    Check = new AgentServiceCheck()
    {
        HTTP = "http://orderwebapi/health",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5),
        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
    }
};

await consulClient.Agent.ServiceRegister(registration);

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(async () =>
{
    await consulClient.Agent.ServiceDeregister(registration.ID);
});

app.Run();
