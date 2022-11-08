using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P365I_WorldCup.Core.Models
{
    public class RootForecast
    {
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        public int? Country1Goals { get; set; }

        public int? Country2Goals { get; set; }

        public string UserEmail { get; set; }

        public string UserFullName { get; set; }

        public int? p365i_group { get; set; }

        public string p365i_id { get; set; }

        public string p365i_matchid { get; set; }

        public int p365i_stage { get; set; }

        public string Country1Id { get; set; }

        public string Country2Id { get; set; }
    }
}
