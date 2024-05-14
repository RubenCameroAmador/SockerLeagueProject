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

        public async Task<bool> addTeamAsync(Team team)
        {
            const string query = @"INSERT INTO public.team(name, date_created, last_update)
	                                VALUES ( @Name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            return await DbExecuteAsync<bool>(query, team);
        }

        public async Task<List<Team>> getAllTeamsAsync()
        {
            const string query = @"SELECT name AS Name 
                                    FROM team;";
            return (await DbQueryAsync<Team>(query)).ToList();
        }
    }
}
