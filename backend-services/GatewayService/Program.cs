using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", builder =>
    {
        builder.WithOrigins(["http://localhost:3000"]) // origins
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
    });
});
// Add Ocelot Configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


// Register Ocelot services
builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors("GatewayCorsPolicy");
// Configure Ocelot middleware
app.UseOcelot().Wait();

app.Run();
