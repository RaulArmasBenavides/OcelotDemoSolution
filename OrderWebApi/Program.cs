var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwaggerGen();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
