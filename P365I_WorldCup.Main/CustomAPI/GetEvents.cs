using Microsoft.Xrm.Sdk;
using P365I_WorldCup.Core.Handlers;
using System;

namespace P365I_WorldCup.Main.CustomAPIs
{
    public class GetEvents : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService service1 = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext service2 = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService organizationService = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(new Guid?(((IExecutionContext)service2).UserId));
            service1.Trace("Start Custom API GetEvents", Array.Empty<object>());
            string inputParameter = (string)((DataCollection<string, object>)((IExecutionContext)service2).InputParameters)["p365i_GetEventsDate"];
            service1.Trace("eventsDate: " + inputParameter, Array.Empty<object>());
            new APIHandler(service1, organizationService).CallGetIncidents(inputParameter);
            ((DataCollection<string, object>)((IExecutionContext)service2).OutputParameters)["GetEventsResult"] = (object)"Ok";
            service1.Trace("End Custom API GetEvents", Array.Empty<object>());
        }
    }
}
