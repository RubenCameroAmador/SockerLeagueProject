using Microsoft.Extensions.Configuration;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Repository.Data;

var builder = WebApplication.CreateBuilder(args);

//CORS policy
string corsPolicyName = string.Empty;
string? clientOrigin = Environment.GetEnvironmentVariable("CLIENT_ORIGIN");
if (!string.IsNullOrEmpty(clientOrigin))
{
    string[] origins = clientOrigin.Split(';');
    if (origins.Length > 0)
    {
        corsPolicyName = "AllowClientOrigin";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: corsPolicyName,
                              policy =>
                              {
                                  policy.WithOrigins(origins);
                                  policy.AllowAnyHeader();
                                  policy.AllowAnyMethod();
                              });
        });
    }
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Custom services
builder.Services.AddTransient<ITeamRepository, TeamRepository>();
builder.Services.AddTransient<ITeamMatchesRepository, TeamMatchesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

if (!string.IsNullOrEmpty(corsPolicyName))
    app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
