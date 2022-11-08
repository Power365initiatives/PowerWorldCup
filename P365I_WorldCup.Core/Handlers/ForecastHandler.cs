using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using P365I_WorldCup.Core.Helpers;
using P365I_WorldCup.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace P365I_WorldCup.Core.Handlers
{
    public class ForecastHandler
    {
        private ITracingService _tracingService;
        private IOrganizationService _service;
        private OrganizationRequestCollection requestCollection;

        public ForecastHandler(ITracingService tracingService, IOrganizationService service)
        {
            this._tracingService = tracingService;
            this._service = service;
        }

        public void UpdateForecasts(string date)
        {
            this._tracingService.Trace("Start UpdateForecasts", Array.Empty<object>());
            this.GetPointsConfiguration();
            List<Entity> list1 = ((IEnumerable<Entity>)this.GetGoalsFromDataverse(date).Entities).ToList<Entity>();
            EntityCollection forecastsFromDataverse = this.GetForecastsFromDataverse(date);
            this.requestCollection = new OrganizationRequestCollection();
            foreach (Entity entity1 in (Collection<Entity>)forecastsFromDataverse.Entities)
            {
                EntityReference entityReference = Common.ReturnValueFromAlias<EntityReference>(entity1, "match.p365i_winner");
                EntityReference country1Match = Common.ReturnValueFromAlias<EntityReference>(entity1, "match.p365i_country1");
                EntityReference country2Match = Common.ReturnValueFromAlias<EntityReference>(entity1, "match.p365i_country2");
                Common.ReturnValueFromAlias<OptionSetValue>(entity1, "match.p365i_pointscountry1");
                Common.ReturnValueFromAlias<OptionSetValue>(entity1, "match.p365i_pointscountry2");
                OptionSetValue optionSetValue = Common.ReturnValueFromAlias<OptionSetValue>(entity1, "match.p365i_stage");
                entity1.GetAttributeValue<EntityReference>("p365i_match");
                EntityReference attributeValue1 = entity1.GetAttributeValue<EntityReference>("p365i_country1");
                EntityReference attributeValue2 = entity1.GetAttributeValue<EntityReference>("p365i_country2");
                int attributeValue3 = entity1.GetAttributeValue<int>("p365i_country1goals");
                int attributeValue4 = entity1.GetAttributeValue<int>("p365i_country2goals");
                List<Entity> list2 = ((IEnumerable<Entity>)list1).Where<Entity>((Func<Entity, bool>)(goal => goal.GetAttributeValue<EntityReference>("p365i_forwhichcountry").Id == country1Match.Id)).ToList<Entity>();
                List<Entity> list3 = ((IEnumerable<Entity>)list1).Where<Entity>((Func<Entity, bool>)(goal => goal.GetAttributeValue<EntityReference>("p365i_forwhichcountry").Id == country2Match.Id)).ToList<Entity>();
                int num1 = ((IEnumerable<Entity>)list2).Count<Entity>();
                int num2 = ((IEnumerable<Entity>)list3).Count<Entity>();
                if (optionSetValue.Value == 446310000)
                {
                    if (attributeValue3 == num1 && attributeValue4 == num2)
                    {
                        Entity entity2 = new Entity("p365i_user_match_forecast", entity1.Id);
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310000));
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_earned_points", (object)5);
                        ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                        {
                            Target = entity2
                        });
                    }
                    else if (attributeValue3 != num1 || attributeValue4 != num2)
                    {
                        if (num1 > num2)
                        {
                            if (attributeValue3 > attributeValue4)
                            {
                                Entity entity3 = new Entity("p365i_user_match_forecast", entity1.Id);
                                ((DataCollection<string, object>)entity3.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310001));
                                ((DataCollection<string, object>)entity3.Attributes).Add("p365i_earned_points", (object)3);
                                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                                {
                                    Target = entity3
                                });
                            }
                            else
                            {
                                Entity entity4 = new Entity("p365i_user_match_forecast", entity1.Id);
                                ((DataCollection<string, object>)entity4.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310003));
                                ((DataCollection<string, object>)entity4.Attributes).Add("p365i_earned_points", (object)0);
                                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                                {
                                    Target = entity4
                                });
                            }
                        }
                        else if (num1 < num2)
                        {
                            if (attributeValue3 < attributeValue4)
                            {
                                Entity entity5 = new Entity("p365i_user_match_forecast", entity1.Id);
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310001));
                                ((DataCollection<string, object>)entity5.Attributes).Add("p365i_earned_points", (object)3);
                                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                                {
                                    Target = entity5
                                });
                            }
                            else
                            {
                                Entity entity6 = new Entity("p365i_user_match_forecast", entity1.Id);
                                ((DataCollection<string, object>)entity6.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310003));
                                ((DataCollection<string, object>)entity6.Attributes).Add("p365i_earned_points", (object)0);
                                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                                {
                                    Target = entity6
                                });
                            }
                        }
                        else if (num1 == num2)
                        {
                            if (attributeValue3 == attributeValue4)
                            {
                                Entity entity7 = new Entity("p365i_user_match_forecast", entity1.Id);
                                ((DataCollection<string, object>)entity7.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310001));
                                ((DataCollection<string, object>)entity7.Attributes).Add("p365i_earned_points", (object)3);
                                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                                {
                                    Target = entity7
                                });
                            }
                            else
                            {
                                Entity entity8 = new Entity("p365i_user_match_forecast", entity1.Id);
                                ((DataCollection<string, object>)entity8.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310003));
                                ((DataCollection<string, object>)entity8.Attributes).Add("p365i_earned_points", (object)0);
                                ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                                {
                                    Target = entity8
                                });
                            }
                        }
                    }
                }
                else if (attributeValue3 == num1 && attributeValue4 == num2)
                {
                    if (num1 > num2)
                    {
                        if (entityReference.Id == attributeValue1.Id)
                        {
                            Entity entity9 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity9.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310000));
                            ((DataCollection<string, object>)entity9.Attributes).Add("p365i_earned_points", (object)7);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity9
                            });
                        }
                        else
                        {
                            Entity entity10 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity10.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310002));
                            ((DataCollection<string, object>)entity10.Attributes).Add("p365i_earned_points", (object)5);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity10
                            });
                        }
                    }
                    else if (num1 < num2)
                    {
                        if (entityReference.Id == attributeValue2.Id)
                        {
                            Entity entity11 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity11.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310000));
                            ((DataCollection<string, object>)entity11.Attributes).Add("p365i_earned_points", (object)7);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity11
                            });
                        }
                        else
                        {
                            Entity entity12 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity12.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310002));
                            ((DataCollection<string, object>)entity12.Attributes).Add("p365i_earned_points", (object)5);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity12
                            });
                        }
                    }
                }
                else if (attributeValue3 != num1 || attributeValue4 != num2)
                {
                    if (num1 > num2)
                    {
                        if (attributeValue3 > attributeValue4)
                        {
                            Entity entity13 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity13.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310001));
                            ((DataCollection<string, object>)entity13.Attributes).Add("p365i_earned_points", (object)3);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity13
                            });
                        }
                        else
                        {
                            Entity entity14 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity14.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310003));
                            ((DataCollection<string, object>)entity14.Attributes).Add("p365i_earned_points", (object)0);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity14
                            });
                        }
                    }
                    else if (num1 < num2)
                    {
                        if (attributeValue3 < attributeValue4)
                        {
                            Entity entity15 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity15.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310001));
                            ((DataCollection<string, object>)entity15.Attributes).Add("p365i_earned_points", (object)3);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity15
                            });
                        }
                        else
                        {
                            Entity entity16 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity16.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310003));
                            ((DataCollection<string, object>)entity16.Attributes).Add("p365i_earned_points", (object)0);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity16
                            });
                        }
                    }
                    else if (num1 == num2)
                    {
                        if (attributeValue3 == attributeValue4)
                        {
                            Entity entity17 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity17.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310001));
                            ((DataCollection<string, object>)entity17.Attributes).Add("p365i_earned_points", (object)3);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity17
                            });
                        }
                        else
                        {
                            Entity entity18 = new Entity("p365i_user_match_forecast", entity1.Id);
                            ((DataCollection<string, object>)entity18.Attributes).Add("p365i_forecast_result", (object)new OptionSetValue(446310003));
                            ((DataCollection<string, object>)entity18.Attributes).Add("p365i_earned_points", (object)0);
                            ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpdateRequest()
                            {
                                Target = entity18
                            });
                        }
                    }
                }
            }
            Common.ExecuteBatchRequest(this._service, this._tracingService, this.requestCollection);
            this._tracingService.Trace("End UpdateForecasts", Array.Empty<object>());
        }

        private EntityCollection GetForecastsFromDataverse(string date) => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='p365i_user_match_forecast'>\r\n                                    <attribute name='p365i_user_match_forecastid' />\r\n                                    <attribute name='p365i_name' />\r\n                                    <attribute name='p365i_match' />\r\n                                    <attribute name='p365i_country2goals' />\r\n                                    <attribute name='p365i_country2' />\r\n                                    <attribute name='p365i_country1goals' />\r\n                                    <attribute name='p365i_country1' />\r\n                                    <attribute name='p365i_stage' />\r\n                                    <order attribute='p365i_name' descending='false' />\r\n                                    <link-entity name='p365i_match' from='p365i_matchid' to='p365i_match' link-type='inner' alias='match'>\r\n                                      <attribute name='p365i_winner' />\r\n                                      <attribute name='p365i_pointscountry2' />\r\n                                      <attribute name='p365i_pointscountry1' />\r\n                                      <attribute name='p365i_id' />\r\n                                      <attribute name='p365i_country2' />\r\n                                      <attribute name='p365i_country1' />\r\n                                      <attribute name='p365i_stage' />\r\n                                      <filter type='and'>\r\n                                        <condition attribute='p365i_matchdate' operator='on' value='" + date + "' />\r\n                                      </filter>\r\n                                    </link-entity>\r\n                                  </entity>\r\n                                </fetch>");

        private EntityCollection GetGoalsFromDataverse(string date) => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='p365i_goal'>\r\n                                    <attribute name='p365i_goalid' />\r\n                                    <attribute name='p365i_name' />\r\n                                    <attribute name='p365i_match' />\r\n                                    <attribute name='p365i_id' />\r\n                                    <attribute name='p365i_forwhichcountry' />\r\n                                    <order attribute='p365i_name' descending='false' />\r\n                                    <link-entity name='p365i_match' from='p365i_matchid' to='p365i_match' link-type='inner' alias='ac'>\r\n                                      <filter type='and'>\r\n                                        <condition attribute='p365i_matchdate' operator='on' value='" + date + "' />\r\n                                      </filter>\r\n                                    </link-entity>\r\n                                  </entity>\r\n                                </fetch>");

        private EntityCollection GetPointsConfiguration() => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='p365i_points_configuration'>\r\n                                    <attribute name='p365i_points_configurationid' />\r\n                                    <attribute name='p365i_name' />\r\n                                    <attribute name='p365i_stage' />\r\n                                    <attribute name='p365i_points' />\r\n                                    <attribute name='p365i_criteria' />\r\n                                    <order attribute='p365i_name' descending='false' />\r\n                                  </entity>\r\n                                </fetch>");

        private EntityCollection GetMatchesFromDataverse() => Common.getDatabyFetchXML(this._service, "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='p365i_match'>\r\n                                    <attribute name='p365i_matchid' />\r\n                                    <attribute name='p365i_name' />\r\n                                    <attribute name='createdon' />\r\n                                    <attribute name='p365i_id' />\r\n                                    <attribute name='p365i_matchdate' />\r\n                                    <attribute name='p365i_country1' />\r\n                                    <attribute name='p365i_country2' />\r\n                                  </entity>\r\n                                </fetch>");

        public void UpsertForecasts(string input)
        {
            this._tracingService.Trace("Start UpsertForecasts", Array.Empty<object>());
            this.requestCollection = new OrganizationRequestCollection();
            List<Entity> list = ((IEnumerable<Entity>)this.GetMatchesFromDataverse().Entities).ToList<Entity>();
            foreach (Match match1 in Common.JsonDeSerialize<RootForecast>(input).Matches)
            {
                Match match = match1;
                int? nullable = match.Country1Goals;
                if (nullable.HasValue)
                {
                    nullable = match.Country2Goals;
                    if (nullable.HasValue)
                    {
                        this._tracingService.Trace("userEmail: " + match.UserEmail, Array.Empty<object>());
                        Entity entity1 = ((IEnumerable<Entity>)((IEnumerable<Entity>)list).Where<Entity>((Func<Entity, bool>)(m => m.GetAttributeValue<string>("p365i_id") == match.p365i_id)).ToList<Entity>()).FirstOrDefault<Entity>();
                        KeyAttributeCollection attributeCollection1 = new KeyAttributeCollection();
                        ((DataCollection<string, object>)attributeCollection1).Add("p365i_id", (object)(match.UserEmail + match.p365i_id));
                        Entity entity2 = new Entity("p365i_user_match_forecast", attributeCollection1);
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_name", (object)entity1.GetAttributeValue<string>("p365i_name"));
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_match", (object)entity1.ToEntityReference());
                        if (match.Country1Id == string.Empty || match.Country2Id == string.Empty)
                        {
                            ((DataCollection<string, object>)entity2.Attributes).Add("p365i_country1", (object)entity1.GetAttributeValue<EntityReference>("p365i_country1"));
                            ((DataCollection<string, object>)entity2.Attributes).Add("p365i_country2", (object)entity1.GetAttributeValue<EntityReference>("p365i_country2"));
                        }
                        else
                        {
                            KeyAttributeCollection attributeCollection2 = new KeyAttributeCollection();
                            ((DataCollection<string, object>)attributeCollection2).Add("p365i_id", (object)match.Country1Id);
                            KeyAttributeCollection attributeCollection3 = new KeyAttributeCollection();
                            ((DataCollection<string, object>)attributeCollection3).Add("p365i_id", (object)match.Country2Id);
                            ((DataCollection<string, object>)entity2.Attributes).Add("p365i_country1", (object)new EntityReference("p365i_participantcountry", attributeCollection2));
                            ((DataCollection<string, object>)entity2.Attributes).Add("p365i_country2", (object)new EntityReference("p365i_participantcountry", attributeCollection3));
                        }
                      ((DataCollection<string, object>)entity2.Attributes).Add("p365i_stage", (object)new OptionSetValue(match.p365i_stage));
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_country1goals", (object)match.Country1Goals);
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_country2goals", (object)match.Country2Goals);
                        ((DataCollection<string, object>)entity2.Attributes).Add("p365i_userfullname", (object)match.UserFullName);
                        ((Collection<OrganizationRequest>)this.requestCollection).Add((OrganizationRequest)new UpsertRequest()
                        {
                            Target = entity2
                        });
                    }
                }
            }
            Common.ExecuteBatchRequest(this._service, this._tracingService, this.requestCollection);
            this._tracingService.Trace("End UpsertForecasts", Array.Empty<object>());
        }
    }
}
