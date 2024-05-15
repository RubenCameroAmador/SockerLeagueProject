using System.Net.Http.Json;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;

namespace SoccerLeague.Client.Repositories;

public class TeamsMatchesRepository : ITeamMatchesRepository
{
    private readonly HttpClient httpClient;

    public TeamsMatchesRepository(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<TeamsMatch>> getTeamMatchByDate(string dateFilter)
    {
        List<TeamsMatch>? teamsMatchList = await httpClient.GetFromJsonAsync<List<TeamsMatch>>($"api/GetAllTeamsMatchesByDate?dateFilter={dateFilter}");
        return teamsMatchList ?? new(); 
    }

    public async Task<bool> insertTeamMatch(TeamsMatch teamMatch)
    {
        HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync("api/addTeamMatch", teamMatch);
        return responseMessage.IsSuccessStatusCode;
    }
}
