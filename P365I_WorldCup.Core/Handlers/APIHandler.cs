using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using P365I_WorldCup.Core.Helpers;
using P365I_WorldCup.Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace P365I_WorldCup.Core.Handlers
{
    public class APIHandler
    {
        private ITracingService _tracingService;
        private IOrganizationService _service;
        private OrganizationRequestCollection requestCollection;

        public APIHandler(ITracingService tracingService, IOrganizationService service)
        {
            this._tracingService = tracingService;
            this._service = service;
        }

        public void CallGetSchedules(string date)
        {
            this._tracingService.Trace("Start CallGetSchedules", Array.Empty<object>());
            List<Entity> list = ((IEnumerable<Entity>)Common.GetEnvironmentVariables(this._service, new string[3]
            {
        "p365i_apibaseurl",
        "p365i_apikey",
        "p365i_footapiurl"
            }).Entities).ToList<Entity>();
            string fromRelatedEntity1 = Common.GetStringValueFromRelatedEntity(list, "ae.schemaname", "p365i_apibaseurl", "value");
            string fromRelatedEntity2 = Common.GetStringValueFromRelatedEntity(list, "ae.schemaname", "p365i_apikey", "value");
            string fromRelatedEntity3 = Common.GetStringValueFromRelatedEntity(list, "ae.schemaname", "p365i_footapiurl", "value");
            RestClient restClient = new RestClient(fromRelatedEntity1 + "matches/" + date);
            restClient.Timeout = -1;
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-RapidAPI-Key", fromRelatedEntity2);
            request.AddHeader("X-RapidAPI-Host", fromRelatedEntity3);
            this._tracingService.Trace("Make Call", Array.Empty<object>());
            IRestResponse restResponse = restClient.Execute((IRestRequest)request);
            if (restResponse.ResponseStatus != ResponseStatus.Completed)
            {
                this._tracingService.Trace("API Endpoint did not respond!", Array.Empty<object>());
                throw new InvalidPluginExecutionException("API Endpoint did not respond!");
            }
            GetSchedulesRoot getSchedulesRoot = Common.JsonDeSerialize<GetSchedulesRoot>(restResponse.Content);
            this.requestCollection = new OrganizationRequestCollection();
            this.UpsertTeamsParticipantTeamsResults(getSchedulesRoot.events);
            Common.ExecuteBatchRequest(this._service, this._tracingService, this.requestCollection);
            this._tracingService.Trace("End CallGetSchedules", Array.Empty<object>());
        }

        public void CallGetTeamPlayers(string date)
        {
            this._tracingService.Trace("Start CallGetTeamPlayers", Array.Empty<object>());
            List<Entity> list = ((IEnumerable<Entity>)Common.GetEnvironmentVariables(this._service, new string[3]
            {
        "p365i_apibaseurl",
        "p365i_apikey",
        "p365i_footapiurl"
            }).Entities).ToList<Entity>();
            string fromRelatedEntity1 = Common.GetStringValueFromRelatedEntity(list, "ae.schemaname", "p365i_apibaseurl", "value");
            string fromRelatedEntity2 = Common.GetStringValueFromRelatedEntity(list, "ae.schemaname", "p365i_apikey", "value");
            string fromRelatedEntity3 = Common.GetStringValueFromRelatedEntity(list, "ae.schemaname", "p365i_footapiurl", "value");
            this.requestCollection = new OrganizationRequestCollection();
            string date1 = this.ConvertDateTimeFormat(date, "dd/MM/yyyy", "yyyy/MM/dd", (IFormatProvider)null);
            this._tracingService.Trace("DataverseDate: " + date1, Array.Empty<object>());
            foreach (Entity entity in (Collection<Entity>)this.GetCountriesFromDataverse(date1).Entities)
            {
                string attributeValue = entity.GetAttributeValue<string>("p365i_id");
                string baseUrl = fromRelatedEntity1 + "team/" + attributeValue + "/players";
                this._tracingService.Trace("teamId: " + attributeValue, Array.Empty<object>());
                RestClient restClient = new RestClient(baseUrl);
                restClient.Timeout = -1;
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-RapidAPI-Key", fromRelatedEntity2);
                request.AddHeader("X-RapidAPI-Host", fromRelatedEntity3);
                this._tracingService.Trace("Make Call", Array.Empty<object>());
                IRestResponse restResponse = restClient.Execute((IRestRequest)request);
                if (restResponse.ResponseStatus != ResponseStatus.Completed)
                {
                    this._tracingService.Trace("API Endpoint did not respond!", Array.Empty<object>());
                    throw new InvalidPluginExecutionException("API Endpoint did not respond!");
                }
                this.UpsertPlayers(Common.JsonDeSerialize<PlayersRoot>(restResponse.Content).nationalPlayers, attributeValue);
                Thread.Sleep(1000);
            }
            Common.ExecuteBatchRequest(this._service, this._tracingService, this.requestCollection);
            this._tracingService.Trace("End CallGetTeamPlayers", Array.Empty<object>());
        }

        public void CallGetIncidents(string date)
        {
            this._tracingService.Trace("Start CallGetIncidents", Array.Empty<object>());
            List<Entity> list1 = ((IEnumerable<Entity>)Common.GetEnvironmentVariables(this._service, new string[3]
            {
        "p365i_apibaseurl",
        "p365i_apikey",
        "p365i_footapiurl"
            }).Entities).ToList<Entity>();
            string fromRelatedEntity1 = Common.GetStringValueFromRelatedEntity(list1, "ae.schemaname", "p365i_apibaseurl", "value");
            string fromRelatedEntity2 = Common.GetStringValueFromRelatedEntity(list1, "ae.schemaname", "p365i_apikey", "value");
            string fromRelatedEntity3 = Common.GetStringValueFromRelatedEntity(list1, "ae.schemaname", "p365i_footapiurl", "value");
            this.requestCollection = new OrganizationRequestCollection();
            foreach (Entity entity in (Collection<Entity>)this.GetMatchesFromDataverse(date).Entities)
            {
                string attributeValue = entity.GetAttributeValue<string>("p365i_id");
                RestClient restClient = new RestClient(fromRelatedEntity1 + "match/" + attributeValue + "/incidents");
                restClient.Timeout = -1;
                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-RapidAPI-Key", fromRelatedEntity2);
                request.AddHeader("X-RapidAPI-Host", fromRelatedEntity3);
                this._tracingService.Trace("matchId: " + attributeValue, Array.Empty<object>());
                this._tracingService.Trace("Make Call", Array.Empty<object>());
                IRestResponse restResponse = restClient.Execute((IRestRequest)request);
                if (restResponse.IsSuccessful && restResponse.StatusCode.ToString() == "OK")
                {
                    if (restResponse.ResponseStatus != ResponseStatus.Completed)
                    {
                        this._tracingService.Trace("API Endpoint did not respond!", Array.Empty<object>());
                        throw new InvalidPluginExecutionException("API Endpoint did not respond!");
                    }
                    IncidentRoot incidentRoot = Common.JsonDeSerialize<IncidentRoot>(restResponse.Content);
                    List<Incident> list2 = incidentRoot.incidents.Where<Incident>((Func<Incident, bool>)(incident => incident.incidentType == "goal")).ToList<Incident>();
                    List<Incident> list3 = incidentRoot.incidents.Where<Incident>((Func<Incident, bool>)(incident => incident.incidentType == "card")).ToList<Incident>();
                    this.UpsertGoals(list2, attributeValue);
                    this.UpsertCards(list3, attributeValue);
                }
                Thread.Sleep(2000);
            }
            Common.ExecuteBatchRequest(this._service, this._tracingService, this.requestCollection);
            this._tracingService.Trace("End CallGetIncidents", Array.Empty<object>());
        }

        private void UpsertTeamsParticipantTeamsResults(List<Event> events)
        {
            this._tracingService.Trace("Start UpsertTeamsParticipantTeamsResults!", Array.Empty<object>());
            this._tracingService.Trace(string.Format("events.Count: {0}", (object)events.Count), Array.Empty<object>());
            foreach (Event @event in events)
            {
                if (@event.tournament.name.Contains("World Cup"))
                {
                    KeyAttributeCollection attributeCollection1 = new KeyAttributeCollection();
                    int id;
                    if (@event.tournament.uniqueTournament != null)
                    {
                        KeyAttributeCollection attributeCollection2 = attributeCollection1;
                        id = @event.tournament.uniqueTournament.id;
                        string str1 = id.ToString();
                        ((DataCollection<string, object>)attributeCollection2).Add("p365i_id", (object)str1);
                        Entity entity = new Entity("p365i_worldcup", attributeCollection1);
                        ((DataCollection<string, object>)entity.Attributes).Add("p365i_name", (object)@event.tournament.uniqueTournament.name);
                        AttributeCollection attributes = entity.Attributes;
                        id = @event.tournament.uniqueTournament.id;
                        string str2 = id.ToString();
                        ((DataCollection<string, object>)attributes).Add("p365i_id", (object)str2);
                        ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                        {
                            Target = entity
                        });
                    }
                    else
                    {
                        KeyAttributeCollection attributeCollection3 = attributeCollection1;
                        id = @event.tournament.id;
                        string str3 = id.ToString();
                        ((DataCollection<string, object>)attributeCollection3).Add("p365i_id", (object)str3);
                        Entity entity = new Entity("p365i_worldcup", attributeCollection1);
                        ((DataCollection<string, object>)entity.Attributes).Add("p365i_name", (object)@event.tournament.name);
                        AttributeCollection attributes = entity.Attributes;
                        id = @event.tournament.id;
                        string str4 = id.ToString();
                        ((DataCollection<string, object>)attributes).Add("p365i_id", (object)str4);
                        ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                        {
                            Target = entity
                        });
                    }
                    KeyAttributeCollection attributeCollection4 = new KeyAttributeCollection();
                    KeyAttributeCollection attributeCollection5 = attributeCollection4;
                    id = @event.homeTeam.id;
                    string str5 = id.ToString();
                    ((DataCollection<string, object>)attributeCollection5).Add("p365i_id", (object)str5);
                    Entity entity1 = new Entity("p365i_country", attributeCollection4);
                    ((DataCollection<string, object>)entity1.Attributes).Add("p365i_name", (object)@event.homeTeam.name);
                    AttributeCollection attributes1 = entity1.Attributes;
                    id = @event.homeTeam.id;
                    string str6 = id.ToString();
                    ((DataCollection<string, object>)attributes1).Add("p365i_id", (object)str6);
                    ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                    {
                        Target = entity1
                    });
                    KeyAttributeCollection attributeCollection6 = new KeyAttributeCollection();
                    KeyAttributeCollection attributeCollection7 = attributeCollection6;
                    id = @event.awayTeam.id;
                    string str7 = id.ToString();
                    ((DataCollection<string, object>)attributeCollection7).Add("p365i_id", (object)str7);
                    Entity entity2 = new Entity("p365i_country", attributeCollection6);
                    ((DataCollection<string, object>)entity2.Attributes).Add("p365i_name", (object)@event.awayTeam.name);
                    AttributeCollection attributes2 = entity2.Attributes;
                    id = @event.awayTeam.id;
                    string str8 = id.ToString();
                    ((DataCollection<string, object>)attributes2).Add("p365i_id", (object)str8);
                    ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                    {
                        Target = entity2
                    });
                    KeyAttributeCollection attributeCollection8 = new KeyAttributeCollection();
                    KeyAttributeCollection attributeCollection9 = attributeCollection8;
                    id = @event.homeTeam.id;
                    string str9 = id.ToString();
                    ((DataCollection<string, object>)attributeCollection9).Add("p365i_id", (object)str9);
                    Entity entity3 = new Entity("p365i_participantcountry", attributeCollection8);
                    ((DataCollection<string, object>)entity3.Attributes).Add("p365i_name", (object)@event.homeTeam.name);
                    AttributeCollection attributes3 = entity3.Attributes;
                    id = @event.homeTeam.id;
                    string str10 = id.ToString();
                    ((DataCollection<string, object>)attributes3).Add("p365i_id", (object)str10);
                    ((DataCollection<string, object>)entity3.Attributes).Add("p365i_worldcup", (object)new EntityReference("p365i_worldcup", attributeCollection1));
                    ((DataCollection<string, object>)entity3.Attributes).Add("p365i_country", (object)new EntityReference("p365i_country", attributeCollection4));
                    ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                    {
                        Target = entity3
                    });
                    KeyAttributeCollection attributeCollection10 = new KeyAttributeCollection();
                    KeyAttributeCollection attributeCollection11 = attributeCollection10;
                    id = @event.awayTeam.id;
                    string str11 = id.ToString();
                    ((DataCollection<string, object>)attributeCollection11).Add("p365i_id", (object)str11);
                    Entity entity4 = new Entity("p365i_participantcountry", attributeCollection10);
                    ((DataCollection<string, object>)entity4.Attributes).Add("p365i_name", (object)@event.awayTeam.name);
                    AttributeCollection attributes4 = entity4.Attributes;
                    id = @event.awayTeam.id;
                    string str12 = id.ToString();
                    ((DataCollection<string, object>)attributes4).Add("p365i_id", (object)str12);
                    ((DataCollection<string, object>)entity4.Attributes).Add("p365i_worldcup", (object)new EntityReference("p365i_worldcup", attributeCollection1));
                    ((DataCollection<string, object>)entity4.Attributes).Add("p365i_country", (object)new EntityReference("p365i_country", attributeCollection6));
                    ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                    {
                        Target = entity4
                    });
                    KeyAttributeCollection attributeCollection12 = new KeyAttributeCollection();
                    KeyAttributeCollection attributeCollection13 = attributeCollection12;
                    id = @event.id;
                    string str13 = id.ToString();
                    ((DataCollection<string, object>)attributeCollection13).Add("p365i_id", (object)str13);
                    Entity entity5 = new Entity("p365i_match", attributeCollection12);
                    ((DataCollection<string, object>)entity5.Attributes).Add("p365i_name", (object)@event.slug);
                    AttributeCollection attributes5 = entity5.Attributes;
                    id = @event.id;
                    string str14 = id.ToString();
                    ((DataCollection<string, object>)attributes5).Add("p365i_id", (object)str14);
                    ((DataCollection<string, object>)entity5.Attributes).Add("p365i_country1", (object)new EntityReference("p365i_participantcountry", attributeCollection8));
                    ((DataCollection<string, object>)entity5.Attributes).Add("p365i_country2", (object)new EntityReference("p365i_participantcountry", attributeCollection10));
                    DateTime dateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dateTime1 = dateTime1.AddSeconds((double)@event.startTimestamp).ToLocalTime();
                    DateTime dateTime2 = Convert.ToDateTime(dateTime1.ToString("yyyy-MM-dd HH:mm:ss"));
                    ((DataCollection<string, object>)entity5.Attributes).Add("p365i_matchdate", (object)dateTime2);
                    this._tracingService.Trace("eventRecord.tournament.slug: " + @event.tournament.slug, Array.Empty<object>());
                    if (@event.tournament.slug != null)
                    {
                        if (!@event.tournament.slug.Contains("knockout-stage"))
                        {
                            ((DataCollection<string, object>)entity5.Attributes).Add("p365i_stage", (object)new OptionSetValue(446310000));
                            if (@event.tournament.slug.Contains("group-a"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310000));
                            else if (@event.tournament.slug.Contains("group-b"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310001));
                            else if (@event.tournament.slug.Contains("group-c"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310002));
                            else if (@event.tournament.slug.Contains("group-d"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310003));
                            else if (@event.tournament.slug.Contains("group-e"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310004));
                            else if (@event.tournament.slug.Contains("group-f"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310005));
                            else if (@event.tournament.slug.Contains("group-g"))
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310006));
                            else
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_group", (object)new OptionSetValue(446310007));
                        }
                        else
                        {
                            DateTime dateTime3 = new DateTime(2022, 12, 3, 0, 0, 0);
                            DateTime dateTime4 = new DateTime(2022, 12, 6, 23, 0, 0);
                            DateTime dateTime5 = new DateTime(2022, 12, 9, 0, 0, 0);
                            DateTime dateTime6 = new DateTime(2022, 12, 10, 23, 0, 0);
                            DateTime dateTime7 = new DateTime(2022, 12, 13, 0, 0, 0);
                            DateTime dateTime8 = new DateTime(2022, 12, 14, 23, 0, 0);
                            DateTime dateTime9 = new DateTime(2022, 12, 17, 0, 0, 0);
                            DateTime dateTime10 = new DateTime(2022, 12, 17, 23, 0, 0);
                            DateTime dateTime11 = new DateTime(2022, 12, 18, 0, 0, 0);
                            DateTime dateTime12 = new DateTime(2022, 12, 18, 23, 0, 0);
                            if (dateTime2 >= dateTime3 && dateTime2 <= dateTime4)
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_stage", (object)new OptionSetValue(446310001));
                            else if (dateTime2 >= dateTime5 && dateTime2 <= dateTime6)
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_stage", (object)new OptionSetValue(446310002));
                            else if (dateTime2 >= dateTime7 && dateTime2 <= dateTime8)
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_stage", (object)new OptionSetValue(446310003));
                            else if (dateTime2 >= dateTime9 && dateTime2 <= dateTime10)
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_stage", (object)new OptionSetValue(446310004));
                            else if (dateTime2 >= dateTime11 && dateTime2 <= dateTime12)
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_stage", (object)new OptionSetValue(446310005));
                        }
                        int? display1 = @event.homeScore.display;
                        int? display2 = @event.awayScore.display;
                        this._tracingService.Trace(string.Format("homeScore: {0}", (object)display1), Array.Empty<object>());
                        this._tracingService.Trace(string.Format("awayScore: {0}", (object)display2), Array.Empty<object>());
                        if (display1.HasValue && display2.HasValue)
                        {
                            if (display1.Value > display2.Value)
                            {
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_pointscountry1", (object)new OptionSetValue(446310002));
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_pointscountry2", (object)new OptionSetValue(446310000));
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_winner", (object)new EntityReference("p365i_participantcountry", attributeCollection8));
                            }
                            if (display1.Value == display2.Value)
                            {
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_pointscountry1", (object)new OptionSetValue(446310001));
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_pointscountry2", (object)new OptionSetValue(446310001));
                            }
                            if (display1.Value < display2.Value)
                            {
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_pointscountry1", (object)new OptionSetValue(446310000));
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_pointscountry2", (object)new OptionSetValue(446310002));
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_winner", (object)new EntityReference("p365i_participantcountry", attributeCollection10));
                            }
                        }
                    }
                  ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                  {
                      Target = entity5
                  });
                }
            }
            this._tracingService.Trace("End UpsertTeamsParticipantTeamsResults", Array.Empty<object>());
        }

        private void UpsertPlayers(List<NationalPlayer> players, string teamId)
        {
            this._tracingService.Trace("Start CreateUpdatePlayers", Array.Empty<object>());
            foreach (NationalPlayer player in players)
            {
                KeyAttributeCollection attributeCollection1 = new KeyAttributeCollection();
                KeyAttributeCollection attributeCollection2 = attributeCollection1;
                int id = player.player.id;
                string str1 = id.ToString();
                ((DataCollection<string, object>)attributeCollection2).Add("p365i_id", (object)str1);
                Entity entity = new Entity("p365i_player", attributeCollection1);
                ((DataCollection<string, object>)entity.Attributes).Add("p365i_fullname", (object)player.player.name);
                AttributeCollection attributes = entity.Attributes;
                id = player.player.id;
                string str2 = id.ToString();
                ((DataCollection<string, object>)attributes).Add("p365i_id", (object)str2);
                KeyAttributeCollection attributeCollection3 = new KeyAttributeCollection();
                ((DataCollection<string, object>)attributeCollection3).Add("p365i_id", (object)teamId);
                ((DataCollection<string, object>)entity.Attributes).Add("p365i_country", (object)new EntityReference("p365i_country", attributeCollection3));
                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                {
                    Target = entity
                });
            }
            this._tracingService.Trace("End CreateUpdatePlayers", Array.Empty<object>());
        }

        private void UpsertGoals(List<Incident> incidents, string matchId)
        {
            this._tracingService.Trace("Start CreateUpdateGoals", Array.Empty<object>());
            foreach (Incident incident in incidents)
            {
                KeyAttributeCollection attributeCollection1 = new KeyAttributeCollection();
                ((DataCollection<string, object>)attributeCollection1).Add("p365i_id", (object)incident.id.ToString());
                Entity entity1 = new Entity("p365i_goal", attributeCollection1);
                ((DataCollection<string, object>)entity1.Attributes).Add("p365i_id", (object)incident.id.ToString());
                AttributeCollection attributes1 = entity1.Attributes;
                int num = incident.time;
                string str1 = num.ToString();
                ((DataCollection<string, object>)attributes1).Add("p365i_time", (object)str1);
                KeyAttributeCollection attributeCollection2 = new KeyAttributeCollection();
                ((DataCollection<string, object>)attributeCollection2).Add("p365i_id", (object)matchId);
                ((DataCollection<string, object>)entity1.Attributes).Add("p365i_match", (object)new EntityReference("p365i_match", attributeCollection2));
                if (incident.player != null)
                {
                    num = incident.player.id;
                    EntityCollection countryFromPlayer = this.GetParticipantCountryFromPlayer(num.ToString());
                    if (((Collection<Entity>)countryFromPlayer.Entities).Count > 0)
                    {
                        KeyAttributeCollection attributeCollection3 = new KeyAttributeCollection();
                        KeyAttributeCollection attributeCollection4 = attributeCollection3;
                        num = incident.player.id;
                        string str2 = num.ToString();
                        ((DataCollection<string, object>)attributeCollection4).Add("p365i_id", (object)str2);
                        ((DataCollection<string, object>)entity1.Attributes).Add("p365i_player", (object)new EntityReference("p365i_player", attributeCollection3));
                        Entity entity2 = ((IEnumerable<Entity>)countryFromPlayer.Entities).FirstOrDefault<Entity>();
                        ((DataCollection<string, object>)entity1.Attributes).Add("p365i_forwhichcountry", (object)entity2.ToEntityReference());
                        AttributeCollection attributes2 = entity1.Attributes;
                        string[] strArray = new string[5];
                        num = incident.time;
                        strArray[0] = num.ToString();
                        strArray[1] = " - ";
                        strArray[2] = incident.player.name;
                        strArray[3] = " - ";
                        strArray[4] = entity2.GetAttributeValue<string>("p365i_name");
                        string str3 = string.Concat(strArray);
                        ((DataCollection<string, object>)attributes2).Add("p365i_name", (object)str3);
                    }
                    else
                    {
                        AttributeCollection attributes3 = entity1.Attributes;
                        num = incident.time;
                        string str4 = num.ToString() + " - " + incident.player.name;
                        ((DataCollection<string, object>)attributes3).Add("p365i_name", (object)str4);
                    }
                }
                else
                {
                    AttributeCollection attributes4 = entity1.Attributes;
                    num = incident.time;
                    string str5 = num.ToString() + " - " + incident.playerName;
                    ((DataCollection<string, object>)attributes4).Add("p365i_name", (object)str5);
                }
              ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
              {
                  Target = entity1
              });
            }
            this._tracingService.Trace("End CreateUpdateGoals", Array.Empty<object>());
        }

        private void UpsertCards(List<Incident> incidents, string matchId)
        {
            this._tracingService.Trace("Start CreateUpdateCards", Array.Empty<object>());
            foreach (Incident incident in incidents)
            {
                KeyAttributeCollection attributeCollection1 = new KeyAttributeCollection();
                ((DataCollection<string, object>)attributeCollection1).Add("p365i_id", (object)incident.id.ToString());
                Entity entity1 = new Entity("p365i_card", attributeCollection1);
                ((DataCollection<string, object>)entity1.Attributes).Add("p365i_id", (object)incident.id.ToString());
                ((DataCollection<string, object>)entity1.Attributes).Add("p365i_time", (object)incident.time.ToString());
                if (incident.incidentClass == "yellow")
                    ((DataCollection<string, object>)entity1.Attributes).Add("p365i_cardtype", (object)new OptionSetValue(446310000));
                else
                    ((DataCollection<string, object>)entity1.Attributes).Add("p365i_cardtype", (object)new OptionSetValue(446310001));
                KeyAttributeCollection attributeCollection2 = new KeyAttributeCollection();
                ((DataCollection<string, object>)attributeCollection2).Add("p365i_id", (object)matchId);
                ((DataCollection<string, object>)entity1.Attributes).Add("p365i_match", (object)new EntityReference("p365i_match", attributeCollection2));
                if (incident.player != null)
                {
                    EntityCollection player = this.GetPlayer(incident.player.id.ToString());
                    if (((Collection<Entity>)player.Entities).Count > 0)
                    {
                        Entity entity2 = ((IEnumerable<Entity>)player.Entities).FirstOrDefault<Entity>();
                        ((DataCollection<string, object>)entity1.Attributes).Add("p365i_player", (object)entity2.ToEntityReference());
                    }
                  ((DataCollection<string, object>)entity1.Attributes).Add("p365i_name", (object)(incident.time.ToString() + " - " + incident.player.name));
                }
                else
                    ((DataCollection<string, object>)entity1.Attributes).Add("p365i_name", (object)(incident.incidentType + " - " + incident.playerName));
                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                {
                    Target = entity1
                });
            }
            this._tracingService.Trace("End CreateUpdateCards", Array.Empty<object>());
        }

        private EntityCollection GetCountriesFromDataverse(string date) => Common.getDatabyFetchXML(this._service, "<fetch>\r\n                                  <entity name='p365i_country'>\r\n                                    <attribute name='p365i_id' />\r\n                                    <filter type='and'>\r\n                                      <filter type='and'>\r\n                                        <condition attribute='p365i_name' operator='ne' value='A1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='A2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='B1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='B2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='C1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='C2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='D1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='D2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='E1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='E2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='F1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='F2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='G1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='G2' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='H1' />\r\n                                        <condition attribute='p365i_name' operator='ne' value='H2' />\r\n                                      </filter>\r\n                                    </filter>\r\n                                    <link-entity name='p365i_participantcountry' from='p365i_country' to='p365i_countryid'>\r\n                                      <link-entity name='p365i_match' from='p365i_country1' to='p365i_participantcountryid' link-type='outer'>\r\n                                        <attribute name='p365i_matchdate' />\r\n                                        <filter>\r\n                                          <condition attribute='p365i_matchdate' operator='on' value='" + date + "' />\r\n                                        </filter>\r\n                                      </link-entity>\r\n                                    </link-entity>\r\n                                  </entity>\r\n                                </fetch>");

        private EntityCollection GetMatchesFromDataverse(string date) => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='p365i_match'>\r\n                                    <attribute name='p365i_matchid' />\r\n                                    <attribute name='p365i_name' />\r\n                                    <attribute name='createdon' />\r\n                                    <attribute name='p365i_id' />\r\n                                    <attribute name='p365i_matchdate' />\r\n                                    <order attribute='p365i_name' descending='false' />\r\n                                    <filter type='and'>\r\n                                      <condition attribute='p365i_matchdate' operator='on' value='" + date + "' />\r\n                                    </filter>\r\n                                  </entity>\r\n                                </fetch>");

        private EntityCollection GetParticipantCountryFromPlayer(string playerId) => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>\r\n                                  <entity name='p365i_participantcountry'>\r\n                                    <attribute name='p365i_participantcountryid' />\r\n                                    <attribute name='p365i_name' />\r\n                                    <order attribute='p365i_name' descending='false' />\r\n                                    <link-entity name='p365i_country' from='p365i_countryid' to='p365i_country' link-type='inner' alias='ac'>\r\n                                      <link-entity name='p365i_player' from='p365i_country' to='p365i_countryid' link-type='inner' alias='ad'>\r\n                                        <filter type='and'>\r\n                                          <condition attribute='p365i_id' operator='eq' value='" + playerId + "' />\r\n                                        </filter>\r\n                                      </link-entity>\r\n                                    </link-entity>\r\n                                  </entity>\r\n                                </fetch>");

        private EntityCollection GetPlayer(string playerId) => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='p365i_player'>\r\n                                    <attribute name='p365i_playerid' />\r\n                                    <attribute name='p365i_fullname' />\r\n                                    <order attribute='p365i_fullname' descending='false' />\r\n                                    <filter type='and'>\r\n                                      <condition attribute='p365i_id' operator='eq' value='" + playerId + "' />\r\n                                    </filter>\r\n                                  </entity>\r\n                                </fetch>");

        private string ConvertDateTimeFormat(
          string input,
          string inputFormat,
          string outputFormat,
          IFormatProvider culture)
        {
            return DateTime.ParseExact(input, inputFormat, culture).ToString(outputFormat, culture);
        }
    }
}
