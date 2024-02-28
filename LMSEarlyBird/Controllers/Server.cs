using System.Collections.Generic;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;


using LMSEarlyBird.Models;
using LMSEarlyBird.Repository;

//public class StripeOptions
//{
//    public string option { get; set; }
//}

namespace LMSEarlyBird.Controllers
{

    //[Route("create-checkout-session")]
    //[ApiController]
    public class CheckoutApiController : Controller
    {
        ///// <summary>
        ///// Context for accessing the user identity database
        ///// </summary>
        //private readonly IUserIdentityService _userIdentityService;
        ///// <summary>
        ///// Context for accessing the balance database
        ///// </summary>
        //private readonly IBalanceRepository _balanceRepository;

        //public CheckoutApiController(IUserIdentityService userIdentityService, IBalanceRepository balanceRepository)
        //{
        //    _userIdentityService = userIdentityService;
        //    _balanceRepository = balanceRepository;
        //}
        //PaymentViewModel paymentVM = new PaymentViewModel
        //{
        //    PaymentAmount = 0
        //};

        //[HttpPost]
        ////public void Grabber(PaymentViewModel paymentVM)
        ////{

        ////}
        //public ActionResult Create()
        ////public async Task<IActionResult> Create(PaymentViewModel paymentVM)
        //{
        //    var options = new SessionCreateOptions
        //    {
        //        LineItems = new List<SessionLineItemOptions>
        //{
        //  new SessionLineItemOptions
        //  {
        //    PriceData = new SessionLineItemPriceDataOptions
        //    {
        //      UnitAmount = 2 * 100,
        //      Currency = "usd",
        //      ProductData = new SessionLineItemPriceDataProductDataOptions
        //      {
        //        Name = "Payment on Tuition",
        //      },
        //    },
        //    Quantity = 1,
        //  },
        //},
        //        Mode = "payment",
        //        SuccessUrl = "http://localhost:7243/Payment/Success",
        //        CancelUrl = "http://localhost:7243/Payment/Cancel",
        //    };

        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}

        //[HttpPost("create-checkout-session")]
        //public ActionResult CreateCheckoutSession()
        //{
        //    var options = new SessionCreateOptions
        //    {
        //        LineItems = new List<SessionLineItemOptions>
        //{
        //  new SessionLineItemOptions
        //  {
        //    PriceData = new SessionLineItemPriceDataOptions
        //    {
        //      UnitAmount = 2000,
        //      Currency = "usd",
        //      ProductData = new SessionLineItemPriceDataProductDataOptions
        //      {
        //        Name = "T-shirt",
        //      },
        //    },
        //    Quantity = 1,
        //  },
        //},
        //        Mode = "payment",
        //        SuccessUrl = "http://localhost:7243/Payment/success",
        //        CancelUrl = "http://localhost:7243/Payment/cancel",
        //    };

        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}
    }
}