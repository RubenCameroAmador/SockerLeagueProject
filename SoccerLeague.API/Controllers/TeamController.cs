using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;

namespace SoccerLeague.API.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;
        public TeamController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [HttpGet("api/teams")]
        public IActionResult GetAllTeams()
        {
            try
            {
                List<Team> teams = _teamRepository.getAllTeams();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("api/addteam")]
        public IActionResult AddTeam([FromBody] Team teamInput)
        {
            try
            {
                if (teamInput == null)
                {
                    return BadRequest("Invalid request body"); 
                }
                bool isSucess = _teamRepository.addTeam(teamInput);
                if ( isSucess )
                {
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
