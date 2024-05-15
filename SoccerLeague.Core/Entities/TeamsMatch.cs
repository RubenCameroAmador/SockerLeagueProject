using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Core.Entities
{
    public class TeamsMatch
    {
        public int Id { get; set; }
        public int IdTeam1 { get; set; }
        public int ScoreTeam1 { get; set; }
        public int IdTeam2 { get; set; }
        public int ScoreTeam2 { get; set;}
        public DateTime MatchDate { get; set; }
    }
}
