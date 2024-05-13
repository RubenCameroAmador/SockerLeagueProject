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
    public class TeamRepository : DbFactoryBase, ITeamRepository
    {
        public TeamRepository(IConfiguration config) : base(config.GetConnectionString("DefaultConnection"))
        {
        }

        public bool addTeam(Team team)
        {
            const string query = @"INSERT INTO public.team(
	                                 name, date_created, last_update)
	                                VALUES ( @Name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            return DbExecute<bool>(query, team);
        }

        public List<Team> getAllTeams()
        {
            const string query = @"SELECT name AS Name
	                               FROM team;";
            return DbQuery<Team>(query).ToList();
        }
    }
}
