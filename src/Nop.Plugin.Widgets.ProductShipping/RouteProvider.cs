using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace Nop.Plugin.Widgets.ProductShipping
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            //get estimate shipping (AJAX link)
            routes.MapRoute("Plugin.Widgets.ProductShipping.GetEstimateShipping",
                            "Plugins/ProductShipping/GetEstimateShipping",
                            new { controller = "ProductShipping", action = "GetEstimateShipping" },
                            new[] { "Nop.Plugin.Widgets.ProductShipping.Controllers" });
        }
    }
}
