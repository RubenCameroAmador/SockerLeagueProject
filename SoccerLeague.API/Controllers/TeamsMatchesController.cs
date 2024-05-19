using Microsoft.AspNetCore.Mvc;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;
using SoccerLeague.Core.Services;
using SoccerLeague.Core.Models;


namespace SoccerLeague.API.Controllers
{
    public class TeamsMatchesController : ControllerBase
    {
        private readonly ITeamMatchesRepository _repository;
        private readonly SocketClientService _socketClientService;

        public TeamsMatchesController(ITeamMatchesRepository repository, SocketClientService socketClientService)
        {
            _repository = repository;
            _socketClientService = socketClientService;
        }

        [HttpPost("api/addTeamMatch")]
        public async Task<IActionResult> AddTeam([FromBody] TeamsMatch teamInput)
        {
            try
            {
                if (teamInput == null)
                {
                    return BadRequest("Invalid request body");
                }
                bool isSucess = await _repository.insertTeamMatch(teamInput);
                if (isSucess)
                {                    
                    await _socketClientService.SendMessage(new SocketClientMessage
                    {
                        Message = "New teams match",
                        Payload = (await _repository.getTeamMatchByDate(DateTime.MaxValue.ToString("yyyyMMdd")))?.LastOrDefault(),
                        PayloadType = SocketClientMessage.MainType.TeamMatch,
                        Type = SocketClientMessage.MessageType.OnMessage
                    });

                    return Ok("TeamMatch added succesfully");
                }
                else
                {
                    return BadRequest("TeamMatch cannot be added");
                }

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }

        /// <summary>
        /// string parameter format: yyyyMMdd
        /// example: 20240514
        /// </summary>
        /// <param name="dateFilter"></param>
        /// <returns></returns>
        [HttpGet("api/GetAllTeamsMatchesByDate")]
        public async Task<IActionResult> GetAllTeamsMatchesByDate(string dateFilter)
        {
            try
            {
                List<TeamsMatch> teams = await _repository.getTeamMatchByDate(dateFilter);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
