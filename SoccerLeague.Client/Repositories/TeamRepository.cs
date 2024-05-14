using System.Net.Http.Json;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;

namespace SoccerLeague.Client.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly HttpClient httpClient;

    public TeamRepository(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<bool> addTeamAsync(Team team)
    {
        HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync("api/addteam", team);
        return responseMessage.IsSuccessStatusCode;
    }

    public async Task<List<Team>> getAllTeamsAsync()
    {
        List<Team>? teams = await httpClient.GetFromJsonAsync<List<Team>>("api/teams");
        return teams ?? new();
    }
}
