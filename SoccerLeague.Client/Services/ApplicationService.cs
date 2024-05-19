using Microsoft.AspNetCore.Components;
using SoccerLeague.Client.Models;
using SoccerLeague.Client.Repositories;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;
using SoccerLeague.Core.Services;
using SoccerLeague.Core.Models;

namespace SoccerLeague.Client.Services;

public class ApplicationService
{
    private readonly ITeamRepository teamRepository;
    private readonly ITeamMatchesRepository teamsMatchesRepository;
    private readonly LogService log;

    public List<Team> Teams { get; set; } = new();
    public List<TeamsMatch> TeamsMatches { get; set; } = new();
    public delegate void StateHasChangedHandler();
    public event StateHasChangedHandler? StateHasChanged = null!;

    private bool _IsLoading;
    public bool IsLoading
    {
        get => _IsLoading;
        set
        {
            if (value == _IsLoading) return;
            _IsLoading = value;
            StateHasChanged?.Invoke();
        }
    }

    public ApplicationService(ITeamRepository teamRepository, ITeamMatchesRepository teamsMatchesRepository, SocketClientService socketClientService, LogService log)
    {
        this.teamRepository = teamRepository;
        this.teamsMatchesRepository = teamsMatchesRepository;
        this.log = log;

        socketClientService.TeamNotification += OnTeamNotification!;
        socketClientService.TeamsMatchNotification += OnTeamsMatchNotification!;
    }

    void OnTeamNotification(Team? team)
    {
        if (team == null) return;

        AddNewTeam(team);

        StateHasChanged?.Invoke();
    }

    void OnTeamsMatchNotification(TeamsMatch? teamsMatch)
    {
        if (teamsMatch == null) return;

        TeamsMatches.Add(teamsMatch);
        StateHasChanged?.Invoke();
    }

    public void LoadTeams(Action<ServiceActionResult>? callback = null)
    {
        IsLoading = true;

        Task.Run(async () =>
        {
            try
            {
                Teams = await teamRepository.getAllTeamsAsync();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
            }
            finally
            {
                IsLoading = false;
                callback?.Invoke(ServiceActionResult.Success("Teams loaded successfully"));
                StateHasChanged?.Invoke();
            }
        });
    }

    public void AddTeam(Team team, Action<ServiceActionResult> callback)
    {
        IsLoading = true;
        Task.Run(async () =>
        {
            string resultMessage = string.Empty;
            bool isOK = false;
            try
            {
                isOK = await teamRepository.addTeamAsync(team);

                if (isOK)
                {
                    AddNewTeam(team);

                    resultMessage = "Team added successfully";
                }
                else
                {
                    resultMessage = "Ups!! Something went wrong...";
                }

            }
            catch (Exception ex)
            {
                resultMessage = "Ups!! An error occurred...";
                log.Exception(ex);
            }
            finally
            {
                IsLoading = false;
                callback(isOK ? ServiceActionResult.Success(resultMessage) : ServiceActionResult.Error(resultMessage));
            }
        });

    }

    private void AddNewTeam(Team team)
    {
        if (!(Teams?.Exists(x => x.Name?.ToLower() == team?.Name?.ToLower()) ?? true))// --Any
        {
            Teams.Add(team);
        }
    }

    public void LoadTeamsMatches(Action<ServiceActionResult>? callback = null)
    {
        IsLoading = true;

        Task.Run(async () =>
        {
            try
            {
                TeamsMatches = await teamsMatchesRepository.getTeamMatchByDate(DateTime.MaxValue.ToString("yyyyMMdd"));
            }
            catch (Exception ex)
            {
                log.Exception(ex);
            }
            finally
            {
                IsLoading = false;
                callback?.Invoke(ServiceActionResult.Success("Teams matches loaded successfully"));
                StateHasChanged?.Invoke();
            }
        });
    }

    public void AddTeamsMatch(TeamsMatch teamsMatch, Action<ServiceActionResult> callback)
    {
        IsLoading = true;
        Task.Run(async () =>
        {
            string resultMessage = string.Empty;
            bool isOK = false;
            try
            {
                isOK = await teamsMatchesRepository.insertTeamMatch(teamsMatch);

                if (isOK)
                {
                    TeamsMatches.Add(teamsMatch);

                    resultMessage = "Team match added successfully";
                }
                else
                {
                    resultMessage = "Ups!! Something went wrong...";
                }

            }
            catch (Exception ex)
            {
                resultMessage = "Ups!! An error occurred...";
                log.Exception(ex);
            }
            finally
            {
                IsLoading = false;
                callback(isOK ? ServiceActionResult.Success(resultMessage) : ServiceActionResult.Error(resultMessage));
            }
        });

    }
    
}
