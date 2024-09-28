using lobby_service.Repositories;
using lobby_service.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);



var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;

//Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Add services to the container
builder.Services.AddScoped<ILobbyRepository, LobbyRepository>();
builder.Services.AddScoped<ILobbyService, LobbyService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
