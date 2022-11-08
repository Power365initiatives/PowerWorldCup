using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P365I_WorldCup.Core.Models
{
    public class PlayersRoot
    {
        public List<Player> players { get; set; }

        public List<NationalPlayer> nationalPlayers { get; set; }

        public ForeignPlayers foreignPlayers { get; set; }
    }

    public class ForeignPlayers
    {
    }

    public class Player
    {
        public ProposedMarketValueRaw proposedMarketValueRaw { get; set; }

        public int id { get; set; }

        public int? dateOfBirthTimestamp { get; set; }

        public int? height { get; set; }

        public string slug { get; set; }

        public string firstName { get; set; }

        public string jerseyNumber { get; set; }

        public string name { get; set; }

        public int userCount { get; set; }

        public bool? retired { get; set; }

        public int? shirtNumber { get; set; }

        public string preferredFoot { get; set; }

        public int? proposedMarketValue { get; set; }

        public string shortName { get; set; }

        public Country country { get; set; }

        public string position { get; set; }

        public string lastName { get; set; }

        public Team team { get; set; }

        public int? contractUntilTimestamp { get; set; }
    }

    public class NationalPlayer
    {
        public Player player { get; set; }
    }

    public class ProposedMarketValueRaw
    {
        public int value { get; set; }

        public string currency { get; set; }
    }

    public class Country
    {
        public string alpha2 { get; set; }

        public string name { get; set; }
    }

    public class Team
    {
        public int userCount { get; set; }

        public string gender { get; set; }

        public int id { get; set; }

        public string nameCode { get; set; }

        public string name { get; set; }

        public PrimaryUniqueTournament primaryUniqueTournament { get; set; }

        public Tournament tournament { get; set; }

        public TeamColors teamColors { get; set; }

        public string slug { get; set; }

        public Sport sport { get; set; }

        public string shortName { get; set; }

        public Country country { get; set; }

        public bool national { get; set; }

        public bool? disabled { get; set; }

        public int type { get; set; }

        public int? ranking { get; set; }
    }

    public class PrimaryUniqueTournament
    {
        public bool displayInverseHomeAwayTeams { get; set; }

        public int id { get; set; }

        public int userCount { get; set; }

        public string name { get; set; }

        public Category category { get; set; }

        public string slug { get; set; }
    }
}
