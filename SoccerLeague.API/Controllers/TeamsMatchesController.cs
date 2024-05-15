using Microsoft.AspNetCore.Mvc;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;

namespace SoccerLeague.API.Controllers
{
    public class TeamsMatchesController : ControllerBase
    {
        private readonly ITeamMatchesRepository _repository;
        public TeamsMatchesController(ITeamMatchesRepository repository)
        {
            _repository = repository;
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
