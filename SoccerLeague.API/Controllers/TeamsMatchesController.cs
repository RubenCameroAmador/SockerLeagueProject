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
    }
}
