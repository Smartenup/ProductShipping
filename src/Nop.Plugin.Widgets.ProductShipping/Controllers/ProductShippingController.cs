using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Plugin.Widgets.ProductShipping.Models;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using SmartenUP.Core.Services.Shippping;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.ProductShipping.Controllers
{
    public class ProductShippingController : BasePluginController
    {

        private readonly ISUPShippingService _supShippingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;        
        private readonly IPriceFormatter _priceFormatter;

        public ProductShippingController(ISUPShippingService supShippingService, IWorkContext workContext,
            IStoreContext storeContext, ITaxService taxService, 
            ICurrencyService currencyService, IPriceFormatter priceFormatter)
        {
            _supShippingService = supShippingService;
            _workContext = workContext;
            _storeContext = storeContext;
            _taxService = taxService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;

        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View();
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            return Configure();
        }


        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {

            var model = new EstimateShippingModel();

            if ((_workContext.CurrentCustomer.Addresses != null) && ( _workContext.CurrentCustomer.Addresses.Count() > 0))
                model.ZipPostalCode = _workContext.CurrentCustomer.Addresses.FirstOrDefault().ZipPostalCode;

            if (additionalData != null)
                model.ProductId = (int)additionalData;

            return View("~/Plugins/Widgets.ProductShipping/Views/WidgetsProductShipping/EstimateShipping.cshtml", model);
        }


        [PublicStoreAllowNavigation(true)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetEstimateShipping(string zipPostalCode, int productId = 0, FormCollection formProductDetails = null)
        {

            var model = new EstimateShippingResultModel();

            var address = new Address
            {
                CountryId = null,
                Country = null,
                StateProvinceId = null,
                StateProvince = null,
                ZipPostalCode = zipPostalCode,
            };


            GetShippingOptionResponse getShippingOptionResponse = _supShippingService
                    .GetShippingOptions(productId, address, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id, formProductDetails);

            if (getShippingOptionResponse.Success)
            {
                if (getShippingOptionResponse.ShippingOptions.Any())
                {
                    foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                    {
                        var soModel = new EstimateShippingResultModel.ShippingOptionModel
                        {
                            Name = shippingOption.Name,
                            Description = shippingOption.Description,

                        };


                        decimal rateBase = _taxService.GetShippingPrice(shippingOption.Rate, _workContext.CurrentCustomer);
                        decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                        soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                        model.ShippingOptions.Add(soModel);
                    }
                }
            }
            else
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);


            //return Json(model, JsonRequestBehavior.AllowGet);

            return PartialView("~/Plugins/Widgets.ProductShipping/Views/WidgetsProductShipping/_EstimateShippingResult.cshtml", model);
            
        }

    }
}
