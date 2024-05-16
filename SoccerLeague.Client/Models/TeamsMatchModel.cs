using SoccerLeague.Core.Entities;

namespace SoccerLeague.Client.Models
{
    public class TeamsMatchModel : TeamsMatch
        {
            private int scoreTeam1;
            public new int ScoreTeam1
            {
                get => scoreTeam1;
                set
                {
                    if (value == scoreTeam1) return;
                    scoreTeam1 = Math.Max(value, 0);
                }
            }
            private int scoreTeam2;
            public new int ScoreTeam2
            {
                get => scoreTeam2;
                set
                {
                    if (value == scoreTeam2) return;
                    scoreTeam2 = Math.Max(value, 0);
                }
            }

            public TeamsMatch GetTeamsMatch() => new TeamsMatch
            {
                MatchDate = this.MatchDate,
                IdTeam1 = this.IdTeam1,
                IdTeam2 = this.IdTeam2,
                ScoreTeam1 = this.ScoreTeam1,
                ScoreTeam2 = this.ScoreTeam2
            };
       
    }
}
