using Microsoft.Crm.Sdk.Messages;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using P365I_WorldCup.Core.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace P365I_WorldCup.Core.Helpers
{
    public class Common
    {
        public static string RemoveCRLFFromString(string pString)
        {
            if (string.IsNullOrEmpty(pString))
                return pString;
            char ch = '\u2028';
            string oldValue1 = ch.ToString();
            ch = '\u2029';
            string oldValue2 = ch.ToString();
            return pString.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(oldValue1, string.Empty).Replace(oldValue2, string.Empty);
        }

        public static void setStatus(
          IOrganizationService service,
          EntityReference Target,
          int? statecode,
          int? statuscode)
        {
            Entity entity = new Entity(Target.LogicalName, Target.Id);
            if (statecode.HasValue)
                ((DataCollection<string, object>)entity.Attributes).Add(nameof(statecode), (object)new OptionSetValue(statecode.Value));
            if (statuscode.HasValue)
                ((DataCollection<string, object>)entity.Attributes).Add(nameof(statuscode), (object)new OptionSetValue(statuscode.Value));
            if (((DataCollection<string, object>)entity.Attributes).Count <= 0)
                return;
            service.Update(entity);
        }

        public static T ReturnValueFromAlias<T>(Entity entity, string field)
        {
            if (!entity.Contains(field))
                return default(T);
            AliasedValue attributeValue = entity.GetAttributeValue<AliasedValue>(field);
            return attributeValue == null ? default(T) : (T)attributeValue.Value;
        }

        public static Entity createEntityfromMapping(
          IOrganizationService service,
          EntityReference sourceEntityRef,
          string targetEntityName,
          TargetFieldType targetFieldType)
        {
            return ((InitializeFromResponse)service.Execute((OrganizationRequest)new InitializeFromRequest()
            {
                EntityMoniker = sourceEntityRef,
                TargetEntityName = targetEntityName,
                TargetFieldType = targetFieldType
            })).Entity;
        }

        public static EntityCollection getDatabyFetchXML(
          IOrganizationService service,
          string fetchXml)
        {
            EntityCollection databyFetchXml = new EntityCollection();
            FetchXmlToQueryExpressionRequest expressionRequest = new FetchXmlToQueryExpressionRequest()
            {
                FetchXml = fetchXml
            };
            QueryExpression query = ((FetchXmlToQueryExpressionResponse)service.Execute((OrganizationRequest)expressionRequest)).Query;
            int num1 = 1;
            int num2 = 250;
            string str = (string)null;
            while (true)
            {
                query.PageInfo = new PagingInfo()
                {
                    Count = num2,
                    PageNumber = num1,
                    PagingCookie = str
                };
                EntityCollection entityCollection = service.RetrieveMultiple((QueryBase)query);
                if (((Collection<Entity>)entityCollection.Entities).Count > 0)
                    databyFetchXml.Entities.AddRange((IEnumerable<Entity>)entityCollection.Entities);
                if (entityCollection.MoreRecords)
                {
                    ++num1;
                    str = entityCollection.PagingCookie;
                }
                else
                    break;
            }
            return databyFetchXml;
        }

        public static EntityCollection getDatabyQueryExpression(
          IOrganizationService service,
          QueryExpression query)
        {
            EntityCollection databyQueryExpression = new EntityCollection();
            int num1 = 1;
            int num2 = 250;
            string str = (string)null;
            while (true)
            {
                query.PageInfo = new PagingInfo()
                {
                    Count = num2,
                    PageNumber = num1,
                    PagingCookie = str
                };
                EntityCollection entityCollection = service.RetrieveMultiple((QueryBase)query);
                if (((Collection<Entity>)entityCollection.Entities).Count > 0)
                    databyQueryExpression.Entities.AddRange((IEnumerable<Entity>)entityCollection.Entities);
                if (entityCollection.MoreRecords)
                {
                    ++num1;
                    str = entityCollection.PagingCookie;
                }
                else
                    break;
            }
            return databyQueryExpression;
        }

        public static void traceEntityData(Entity target, ITracingService tracingService)
        {
            tracingService.Trace(string.Format("Entity {0} contains {1} Attributes", (object)target.LogicalName, (object)((DataCollection<string, object>)target.Attributes).Count), Array.Empty<object>());
            tracingService.Trace(string.Format("Entity {0} contains {1} Formatted Values", (object)target.LogicalName, (object)((DataCollection<string, string>)target.FormattedValues).Count), Array.Empty<object>());
            int num1 = 0;
            foreach (KeyValuePair<string, object> attribute in (DataCollection<string, object>)target.Attributes)
            {
                ++num1;
                tracingService.Trace(string.Format("{0} | Attribute Key : {1} and Attribute Value : {2}", (object)num1, (object)attribute.Key, attribute.Value), Array.Empty<object>());
            }
            int num2 = 0;
            foreach (KeyValuePair<string, string> formattedValue in (DataCollection<string, string>)target.FormattedValues)
            {
                ++num2;
                tracingService.Trace(string.Format("{0} | Formatted Key : {1} and Formatted Value : {2}", (object)num2, (object)formattedValue.Key, (object)formattedValue.Value), Array.Empty<object>());
            }
        }

        public static string getEnvironmentVariable(
          IOrganizationService service,
          ITracingService tracingService,
          string EnvDefName)
        {
            string fetchXml = "<fetch>\r\n                      <entity name='environmentvariabledefinition'>\r\n                        <attribute name='defaultvalue' />\r\n                        <filter type='and'>\r\n                          <condition attribute='schemaname' operator='eq' value='" + EnvDefName + "'/>\r\n                        </filter>\r\n                        <link-entity name='environmentvariablevalue' from='environmentvariabledefinitionid' to='environmentvariabledefinitionid' link-type='outer' alias='envvarvalue'>\r\n                          <attribute name='value' />\r\n                        </link-entity>\r\n                      </entity>\r\n                    </fetch>";
            Entity entity = ((IEnumerable<Entity>)Common.getDatabyFetchXML(service, fetchXml).Entities).FirstOrDefault<Entity>();
            string attributeValue = entity.GetAttributeValue<string>("defaultvalue");
            string str = Common.ReturnValueFromAlias<string>(entity, "envvarvalue.value");
            return string.IsNullOrEmpty(str) ? attributeValue : str;
        }

        public static EntityCollection querybyAttribute(
          IOrganizationService service,
          ITracingService tracingServive,
          string entityName,
          ColumnSet columns,
          string attributeName,
          string attributeValue)
        {
            QueryByAttribute queryByAttribute = new QueryByAttribute(entityName);
            queryByAttribute.ColumnSet = columns;
            queryByAttribute.Attributes.AddRange(new string[1]
            {
        attributeName
            });
            queryByAttribute.Values.AddRange(new object[1]
            {
        (object) attributeValue
            });
            return service.RetrieveMultiple((QueryBase)queryByAttribute);
        }

        public static DateTime convertToUserDateTime(
          IOrganizationService service,
          DateTime inputDate,
          Guid userGuid)
        {
            TimeZoneInfo userTimeZone = Common.getUserTimeZone(service, userGuid);
            return TimeZoneInfo.ConvertTimeFromUtc(inputDate, userTimeZone);
        }

        public static TimeZoneInfo getUserTimeZone(
          IOrganizationService service,
          Guid userGuid)
        {
            Entity entity = service.Retrieve("usersettings", userGuid, new ColumnSet(new string[1]
            {
        "timezonecode"
            }));
            int num = 85;
            if (entity != null && entity["timezonecode"] != null)
                num = (int)entity["timezonecode"];
            QueryExpression queryExpression = new QueryExpression("timezonedefinition");
            queryExpression.ColumnSet = new ColumnSet(new string[1]
            {
        "standardname"
            });
            queryExpression.Criteria.AddCondition("timezonecode", (ConditionOperator)0, new object[1]
            {
        (object) num
            });
            EntityCollection entityCollection = service.RetrieveMultiple((QueryBase)queryExpression);
            TimeZoneInfo userTimeZone = (TimeZoneInfo)null;
            if (((Collection<Entity>)entityCollection.Entities).Count == 1)
                userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(((Collection<Entity>)entityCollection.Entities)[0]["standardname"].ToString());
            return userTimeZone;
        }

        public static string cleanId(string Id) => string.IsNullOrEmpty(Id) ? string.Empty : Id.Replace("{", "").Replace("}", "");

        public static void ExecuteBatchRequest(
          IOrganizationService service,
          ITracingService tracingService,
          OrganizationRequestCollection requestCollection,
          int split = 50)
        {
            string empty = string.Empty;
            List<List<OrganizationRequest>> organizationRequestListList = ((IEnumerable<OrganizationRequest>)requestCollection).ToList<OrganizationRequest>().ChunkBy<OrganizationRequest>(split);
            tracingService.Trace(string.Format("Splitted {0} into {1} List with split setting of {2}", (object)((Collection<OrganizationRequest>)requestCollection).Count, (object)organizationRequestListList.Count, (object)split), Array.Empty<object>());
            int num = 1;
            foreach (List<OrganizationRequest> organizationRequestList in organizationRequestListList)
            {
                OrganizationRequestCollection requestCollection1 = new OrganizationRequestCollection();
                ((DataCollection<OrganizationRequest>)requestCollection1).AddRange((IEnumerable<OrganizationRequest>)organizationRequestList);
                ExecuteMultipleRequest executeMultipleRequest = new ExecuteMultipleRequest()
                {
                    Settings = new ExecuteMultipleSettings()
                    {
                        ReturnResponses = true,
                        ContinueOnError = false
                    },
                    Requests = requestCollection1
                };
                try
                {
                    tracingService.Trace(string.Format("Execute Multiple Request {0} of {1}", (object)num, (object)organizationRequestListList.Count), Array.Empty<object>());
                    ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)service.Execute((OrganizationRequest)executeMultipleRequest);
                    tracingService.Trace(string.Format("Multiple Request Executed. Is faulted : {0}", (object)multipleResponse.IsFaulted), Array.Empty<object>());
                    ++num;
                }
                catch (Exception ex)
                {
                    tracingService.Trace(string.Format("Error {0}", (object)ex), Array.Empty<object>());
                    empty += ex.Message;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(empty))
                        tracingService.Trace("Exception: " + empty, Array.Empty<object>());
                }
            }
        }

        public static void CheckForStringEmtpy(string input, string description)
        {
            if (string.IsNullOrEmpty(input))
                throw new InvalidPluginExecutionException(description + " not filled");
        }

        public static void CheckForAttributeEmtpy(dynamic input, string description)
        {
            if (input == null)
            {
                throw new InvalidPluginExecutionException($"{description} not filled");
            }
        }

        public static void CallFlow(string endpoint, object body)
        {
            RestClient restClient = new RestClient(endpoint);
            restClient.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);
            restClient.Execute((IRestRequest)request);
        }

        public static string JsonSerialize(object input) => JsonConvert.SerializeObject(input, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd"
        });

        public static string JsonIndentFour(object input) => JsonConvert.SerializeObject(input, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.Ignore
        });

        public static T JsonDeSerialize<T>(string JSON) => JsonConvert.DeserializeObject<T>(JSON, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.Ignore
        });

        public static T SelectTargetOrImage<T>(Entity target, Entity image, string attr) => image != null && !target.Contains(attr) ? image.GetAttributeValue<T>(attr) : target.GetAttributeValue<T>(attr);

        public static EntityCollection GetEnvironmentVariables(
          IOrganizationService _service,
          string[] variablesNameToFind)
        {
            string str1 = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                  <entity name='environmentvariablevalue'>\r\n                                    <attribute name='environmentvariablevalueid' />\r\n                                    <attribute name='value' />\r\n                                    <attribute name='createdon' />\r\n                                    <order attribute='createdon' descending='false' />\r\n                                    <link-entity name='environmentvariabledefinition' from='environmentvariabledefinitionid' to='environmentvariabledefinitionid' link-type='inner' alias='ae'>\r\n                                      <attribute name='schemaname' />\r\n                                      <filter type='and'>\r\n                                        <filter type='or'>";
            foreach (string str2 in variablesNameToFind)
                str1 = str1 + "<condition attribute='schemaname' operator='eq' value='" + str2 + "' />";
            string fetchXml = str1 + "</filter>\r\n                                        </filter>\r\n                                    </link-entity>\r\n                                </entity>\r\n                            </fetch>";
            return Common.getDatabyFetchXML(_service, fetchXml);
        }

        public static string GetStringValueFromRelatedEntity(
          List<Entity> environmentCollList,
          string relatedAttribute,
          string attributeName,
          string attributeToRetrieve)
        {
            try
            {
                return ((IEnumerable<Entity>)environmentCollList).FirstOrDefault<Entity>((Func<Entity, bool>)(item => item.GetAttributeValue<AliasedValue>(relatedAttribute).Value.ToString() == attributeName)).GetAttributeValue<string>(attributeToRetrieve);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetStringValueFromEntity(
          List<Entity> environmentCollList,
          string attributeName,
          string compareString,
          string attributeToRetrieve)
        {
            try
            {
                return ((IEnumerable<Entity>)environmentCollList).FirstOrDefault<Entity>((Func<Entity, bool>)(item => item.GetAttributeValue<string>(attributeName) == compareString)).GetAttributeValue<string>(attributeToRetrieve);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public class TracingService : ITracingService
        {
            public void Trace(string format, params object[] args) => Console.WriteLine(string.Format(format, args));
        }

        public class PluginContext : IPluginExecutionContext, IExecutionContext
        {
            public Guid BusinessUnitId => throw new NotImplementedException();

            public Guid CorrelationId => throw new NotImplementedException();

            public int Depth { get; set; }

            public Guid InitiatingUserId { get; set; }

            public ParameterCollection InputParameters { get; set; }

            public bool IsExecutingOffline => throw new NotImplementedException();

            public bool IsInTransaction => throw new NotImplementedException();

            public bool IsOfflinePlayback => throw new NotImplementedException();

            public int IsolationMode => throw new NotImplementedException();

            public string MessageName { get; set; }

            public int Mode => throw new NotImplementedException();

            public DateTime OperationCreatedOn => throw new NotImplementedException();

            public Guid OperationId => throw new NotImplementedException();

            public Guid OrganizationId => throw new NotImplementedException();

            public string OrganizationName => throw new NotImplementedException();

            public ParameterCollection OutputParameters => throw new NotImplementedException();

            public EntityReference OwningExtension => throw new NotImplementedException();

            public IPluginExecutionContext ParentContext => (IPluginExecutionContext)null;

            public EntityImageCollection PostEntityImages { get; set; }

            public EntityImageCollection PreEntityImages { get; set; }

            public Guid PrimaryEntityId { get; set; }

            public string PrimaryEntityName { get; set; }

            public Guid? RequestId => throw new NotImplementedException();

            public string SecondaryEntityName => throw new NotImplementedException();

            public ParameterCollection SharedVariables => throw new NotImplementedException();

            public int Stage { get; set; }

            public Guid UserId => throw new NotImplementedException();
        }
    }
}
