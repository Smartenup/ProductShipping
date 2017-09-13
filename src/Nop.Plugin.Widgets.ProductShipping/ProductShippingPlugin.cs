using Nop.Core.Plugins;
using Nop.Services.Cms;
using System.Collections.Generic;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.ProductShipping
{
    public class ProductShippingPlugin : BasePlugin, IWidgetPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ProductShipping";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.ProductShipping.Controllers" }, { "area", null } };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ProductShipping";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.ProductShipping.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { "productdetails_overview_bottom" };
        }
    }
}
