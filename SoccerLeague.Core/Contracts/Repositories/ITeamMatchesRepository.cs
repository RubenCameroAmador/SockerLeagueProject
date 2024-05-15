using SoccerLeague.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Core.Contracts.Repositories
{
    public interface ITeamMatchesRepository
    {
        Task<bool> insertTeamMatch(TeamsMatch teamMatch);
    }
}
