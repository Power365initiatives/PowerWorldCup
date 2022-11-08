using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P365I_WorldCup.Core.Models
{
    public class GetSchedulesRoot
    {
        public List<Event> events { get; set; }
    }
    public class Event
    {
        public HomeScore homeScore { get; set; }

        public bool hasGlobalHighlights { get; set; }

        public bool finalResultOnly { get; set; }

        public string customId { get; set; }

        public Status status { get; set; }

        public bool hasEventPlayerStatistics { get; set; }

        public string slug { get; set; }

        public int? homeRedCards { get; set; }

        public object aggregatedWinnerCode { get; set; }

        public bool hasEventPlayerHeatMap { get; set; }

        public Changes changes { get; set; }

        public int id { get; set; }

        public Tournament tournament { get; set; }

        public Time time { get; set; }

        public object statusTime { get; set; }

        public int startTimestamp { get; set; }

        public RoundInfo roundInfo { get; set; }

        public object coverage { get; set; }

        public AwayScore awayScore { get; set; }

        public int? awayRedCards { get; set; }

        public AwayTeam awayTeam { get; set; }

        public object lastPeriod { get; set; }

        public HomeTeam homeTeam { get; set; }

        public int winnerCode { get; set; }
    }

    public class HomeScore
    {
        public object extra2 { get; set; }

        public int? normaltime { get; set; }

        public object extra1 { get; set; }

        public object penalties { get; set; }

        public int? current { get; set; }

        public int? period2 { get; set; }

        public int? period1 { get; set; }

        public int? display { get; set; }

        public object overtime { get; set; }
    }

    public class Status
    {
        public string type { get; set; }

        public int code { get; set; }

        public string description { get; set; }
    }

    public class Changes
    {
        public int changeTimestamp { get; set; }

        public object changes { get; set; }
    }

    public class Tournament
    {
        public int priority { get; set; }

        public int id { get; set; }

        public UniqueTournament uniqueTournament { get; set; }

        public string name { get; set; }

        public Category category { get; set; }

        public string slug { get; set; }
    }

    public class Time
    {
        public int? currentPeriodStartTimestamp { get; set; }

        public object max { get; set; }

        public int? injuryTime1 { get; set; }

        public object extra { get; set; }

        public object initial { get; set; }

        public int? injuryTime2 { get; set; }
    }

    public class RoundInfo
    {
        public int round { get; set; }

        public object name { get; set; }
    }

    public class AwayScore
    {
        public object extra2 { get; set; }

        public int? normaltime { get; set; }

        public object extra1 { get; set; }

        public object penalties { get; set; }

        public int? current { get; set; }

        public int? period2 { get; set; }

        public int? period1 { get; set; }

        public int? display { get; set; }

        public object overtime { get; set; }
    }

    public class AwayTeam
    {
        public int id { get; set; }

        public TeamColors teamColors { get; set; }

        public string name { get; set; }

        public SubTeams subTeams { get; set; }

        public string shortName { get; set; }

        public bool? disabled { get; set; }

        public string slug { get; set; }

        public int userCount { get; set; }

        public int type { get; set; }
    }

    public class HomeTeam
    {
        public int id { get; set; }

        public TeamColors teamColors { get; set; }

        public string name { get; set; }

        public SubTeams subTeams { get; set; }

        public string shortName { get; set; }

        public bool? disabled { get; set; }

        public string slug { get; set; }

        public int userCount { get; set; }

        public int type { get; set; }
    }

    public class UniqueTournament
    {
        public bool displayInverseHomeAwayTeams { get; set; }

        public int id { get; set; }

        public int userCount { get; set; }

        public string name { get; set; }

        public Category category { get; set; }

        public string slug { get; set; }
    }

    public class Category
    {
        public string alpha2 { get; set; }

        public Sport sport { get; set; }

        public int id { get; set; }

        public string flag { get; set; }

        public string name { get; set; }

        public string slug { get; set; }
    }

    public class TeamColors
    {
        public string secondary { get; set; }

        public string primary { get; set; }

        public string text { get; set; }
    }

    public class SubTeams
    {
    }

    public class Sport
    {
        public string name { get; set; }

        public string slug { get; set; }

        public int id { get; set; }
    }
}
