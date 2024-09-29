using lobby_service.Repositories;
using lobby_service.Services;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using RedLockNet;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);



var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;

//Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));


// Register Redlock
builder.Services.AddSingleton<IDistributedLockFactory>(sp =>
{
    var redisConnection = sp.GetRequiredService<IConnectionMultiplexer>();

    var redLockMultiplexers = new List<RedLockMultiplexer>
    {
        new RedLockMultiplexer(redisConnection)  // Wrap the connection in RedLockMultiplexer
    };

    return RedLockFactory.Create(redLockMultiplexers);
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
      {
          h.Username("guest");
          h.Password("guest");
      });
    });
});
// Add services to the container
builder.Services.AddScoped<ILobbyRepository, LobbyRepository>();
builder.Services.AddScoped<ILobbyService, LobbyService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
