using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoccerLeague.Client.Models
{
    public class TeamTable
    {
        public int Position { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TotalPlayed { get; set; }
        public int TotalWon { get; set; }
        public int TotalDraw { get; set; }
        public int TotalLost { get; set; }
        public int TotalGF { get; set; }
        public int TotalGA { get; set; }
        public int TotalGD { get; set; }
        public int TotalPoints { get; set; }
    }
}