using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SoccerLeague.Client;
using SoccerLeague.Client.Repositories;
using SoccerLeague.Client.Services;
using SoccerLeague.Core.Contracts.Repositories;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string? urlApi = builder.Configuration.GetValue<string>("Api");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(urlApi ?? builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<SocketClientService>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamMatchesRepository, TeamsMatchesRepository>();
builder.Services.AddScoped<ApplicationService>();

await builder.Build().RunAsync();
