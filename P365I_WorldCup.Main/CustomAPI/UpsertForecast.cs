using Microsoft.Xrm.Sdk;
using P365I_WorldCup.Core.Handlers;
using System;

namespace P365I_WorldCup.Main.CustomAPIs
{
    public class UpsertForecast : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService service1 = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext service2 = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService organizationService = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(new Guid?(((IExecutionContext)service2).UserId));
            service1.Trace("Start Custom API CreateForecast", Array.Empty<object>());
            string inputParameter = (string)((DataCollection<string, object>)((IExecutionContext)service2).InputParameters)["p365i_Forecast"];
            service1.Trace("input: " + inputParameter, Array.Empty<object>());
            new ForecastHandler(service1, organizationService).UpsertForecasts(inputParameter);
            ((DataCollection<string, object>)((IExecutionContext)service2).OutputParameters)["UpsertForecastResult"] = (object)"Ok";
            service1.Trace("End Custom API CreateForecast", Array.Empty<object>());
        }
    }
}
