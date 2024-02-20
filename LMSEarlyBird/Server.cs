using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;

public class StripeOptions
{
    public string option { get; set; }
}

namespace LMSEarlyBird
{

    [Route("create-checkout-session")]
    [ApiController]
    public class CheckoutApiController : Controller
    {

        [HttpPost]
        public ActionResult Create()
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
              Currency = "usd",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "T-shirt",
              },
            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:7243/Payment/Success",
                CancelUrl = "http://localhost:7243/Payment/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
        {
          new SessionLineItemOptions
          {
            PriceData = new SessionLineItemPriceDataOptions
            {
              UnitAmount = 2000,
              Currency = "usd",
              ProductData = new SessionLineItemPriceDataProductDataOptions
              {
                Name = "T-shirt",
              },
            },
            Quantity = 1,
          },
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:7243/Payment/success",
                CancelUrl = "http://localhost:7243/Payment/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}