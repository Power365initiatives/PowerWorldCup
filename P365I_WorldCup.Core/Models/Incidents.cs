using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P365I_WorldCup.Core.Models
{
    public class IncidentRoot
    {
        public List<Incident> incidents { get; set; }
    }

    public class Incident
    {
        public int? addedTime { get; set; }

        public int? awayScore { get; set; }

        public object confirmed { get; set; }

        public int? homeScore { get; set; }

        public int? id { get; set; }

        public string incidentClass { get; set; }

        public string incidentType { get; set; }

        public bool? isHome { get; set; }

        public bool? isLive { get; set; }

        public int? length { get; set; }

        public IncidentPlayer player { get; set; }

        public PlayerIn playerIn { get; set; }

        public string playerName { get; set; }

        public PlayerOut playerOut { get; set; }

        public string reason { get; set; }

        public string text { get; set; }

        public int time { get; set; }
    }

    public class IncidentPlayer
    {
        public string firstName { get; set; }

        public int id { get; set; }

        public string lastName { get; set; }

        public string name { get; set; }

        public string position { get; set; }

        public string shortName { get; set; }

        public string slug { get; set; }

        public int userCount { get; set; }
    }

    public class PlayerIn
    {
        public string firstName { get; set; }

        public int id { get; set; }

        public string lastName { get; set; }

        public string name { get; set; }

        public string position { get; set; }

        public string shortName { get; set; }

        public string slug { get; set; }

        public int userCount { get; set; }
    }

    public class PlayerOut
    {
        public string firstName { get; set; }

        public int id { get; set; }

        public string lastName { get; set; }

        public string name { get; set; }

        public string position { get; set; }

        public string shortName { get; set; }

        public string slug { get; set; }

        public int userCount { get; set; }
    }
}
