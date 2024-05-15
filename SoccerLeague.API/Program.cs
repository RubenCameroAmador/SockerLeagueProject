using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Repository.Data;

string basePath = (args.Any() && !string.IsNullOrWhiteSpace(args[0]) && Directory.Exists(args[0])) ? args[0] : System.AppDomain.CurrentDomain.BaseDirectory;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var settings = env == null ? "appsettings.json" : $"appsettings.{env}.json";
Directory.SetCurrentDirectory(basePath);
var configuration = new ConfigurationBuilder()
              .SetBasePath(basePath)
              .AddJsonFile(settings, optional: false, reloadOnChange: false)
              .Build();

var builder = WebApplication.CreateBuilder(args);

//CORS
string? clientOrigin = Environment.GetEnvironmentVariable("CLIENT_ORIGIN");
string corsPolicyName = "AllowClientOrigin";
if (!string.IsNullOrEmpty(clientOrigin))
{
    string[] origins = clientOrigin.Split(';');
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


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddTransient<ITeamRepository, TeamRepository>();
builder.Services.AddTransient<ITeamMatchesRepository, TeamMatchesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
