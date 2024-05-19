using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;
using SoccerLeague.Core.Services;
using SoccerLeague.Core.Models;

namespace SoccerLeague.API.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;
        private readonly SocketClientService _socketClientService;
        public TeamController(ITeamRepository teamRepository, SocketClientService socketClientService)
        {
            _teamRepository = teamRepository;
            _socketClientService = socketClientService;
        }

        [HttpGet("api/teams")]
        public async Task<IActionResult> GetAllTeams()
        {
            try
            {
                List<Team> teams = await _teamRepository.getAllTeamsAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("api/addteam")]
        public async Task<IActionResult> AddTeam([FromBody] Team teamInput)
        {
            try
            {
                if (teamInput == null)
                {
                    return BadRequest("Invalid request body"); 
                }
                bool isSucess = await _teamRepository.addTeamAsync(teamInput);
                if ( isSucess )
                {
                    await _socketClientService.SendMessage(new SocketClientMessage
                    {
                        Message = "New team",
                        Payload = (await _teamRepository.getAllTeamsAsync())?.LastOrDefault(),
                        PayloadType = SocketClientMessage.MainType.Team,
                        Type = SocketClientMessage.MessageType.OnMessage
                    });
                    return Ok("Team added succesfully");
                }
                else
                {
                    return BadRequest("Team cannot be added");
                }

            }
            catch (Exception ex) 
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }
    }
}
