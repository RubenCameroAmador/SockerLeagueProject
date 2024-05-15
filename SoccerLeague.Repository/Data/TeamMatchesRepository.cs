using Microsoft.Extensions.Configuration;
using SoccerLeague.Core.Contracts.Repositories;
using SoccerLeague.Core.Entities;
using SoccerLeague.Repository.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Repository.Data
{
    public class TeamMatchesRepository : DbFactoryBase, ITeamMatchesRepository
    {
        public TeamMatchesRepository(IConfiguration config) : base(config.GetConnectionString("DefaultConnection"))
        {
        }

        public async Task<bool> insertTeamMatch(TeamsMatch teamMatch)
        {
            const string query = @"INSERT INTO public.teams_matches(
	                               id_team1, score_team_1, id_team_2, score_team_2, match_date, date_created, last_updated)
	                               VALUES ( @IdTeam1, @ScoreTeam1, @IdTeam2, @ScoreTeam2, @MatchDate, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            return await DbExecuteAsync<bool>(query, teamMatch);
        }
    }
}
