using LMSEarlyBird.Interfaces;
using LMSEarlyBird.Models;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using LMSEarlyBird.Repository;
using Microsoft.Extensions.FileSystemGlobbing;

namespace LMSEarlyBird.Controllers
{
    public class PaymentController : Controller
    {
        /// <summary>
        /// Context for accessing the balance database
        /// </summary>
        private readonly IBalanceRepository _balanceRepository;
        /// <summary>
        /// Context accessor for reading session data
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;
        /// <summary>
        /// Context for accessing the assignments database
        /// </summary>
        private readonly IAssignmentsRepository _assignmentRepository;

        public PaymentController(IHttpContextAccessor contextAccessor, 
            IBalanceRepository balanceRepository,
            IAssignmentsRepository assignmentsRepository)
        {
            _contextAccessor = contextAccessor;
            _balanceRepository = balanceRepository;
            _assignmentRepository = assignmentsRepository;
        }

        // gather the current balance for the user and pass it on to the payment page
        [HttpGet]
        public async Task<IActionResult> PaymentPage()
        {
            // gather the user id
            var userId = _contextAccessor.HttpContext.User.GetUserId();

            // gather the current balance for the user
            decimal currentBalance = await _balanceRepository.GetCurrentBalance(userId);

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            // insert the current balance into the view
            PaymentViewModel paymentVM = new PaymentViewModel
            {
                CurrentBalance = currentBalance,
                StudentAssignment = assignments
        };
            // gather the current ballance history and place it into the view
            List<BalanceHistory> balances = await _balanceRepository.GetBalanceHistory(userId);
            paymentVM.Payments = balances;


            return View(paymentVM);
        }

        // gather the payment intent (aka the payment reciept) and pass it to the paymentsuccess for processing the payment, then pass to the success view
        [HttpGet]
        public async Task<IActionResult> Success(string recieptNumber)
        {
            // gather the user id
            var userId = _contextAccessor.HttpContext.User.GetUserId();

            // gather the payment intent (aka the payment reciept)
            var service = new PaymentIntentService();
            var reciept = service.Get(recieptNumber);

            // check to make sure this reciept has not been used yet
            List<string> recipetNumbers = _balanceRepository.GetAllReciepts().Result.Select(x => x.Reciept).ToList();

            foreach (string reicpet in recipetNumbers)
            {
                if (reicpet == recieptNumber)
                {
                    return RedirectToAction("Error");
                }
            }

            // create a new view model for the passing of the payment amount
            PaymentViewModel paymentVM = new PaymentViewModel();

            // pass on the payment amount into the view model
            paymentVM.PaymentAmount = reciept.Amount / 100.00m;

            // create a new balance update for the payment
            await _balanceRepository.UpdateBalancePayment(userId, paymentVM.PaymentAmount, reciept.Id);

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            paymentVM.StudentAssignment = assignments;

            // pass on the payment view model for the payment amount
            return View(paymentVM);
        }

        // pass on the payment view model for the payment amount to the error view
        [HttpGet]
        public async Task<IActionResult> Error()
        {
            return View();
        }

        //pass on the payment view model for the payment amount to the checkout view
        [HttpGet]
        public async Task<IActionResult> Checkout(decimal paymentAmount)
        {
            // gather the user id
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            //gather the payment amount and place into a new view model so that we don't include the balance as well
            PaymentViewModel paymentVM2 = new PaymentViewModel
            {
                PaymentAmount = paymentAmount,
                StudentAssignment = assignments
            };

            return View(paymentVM2);
        }

        // pass on the payment view model for the payment amount to the cancel view
        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            // gather the user id
            var userId = _contextAccessor.HttpContext.User.GetUserId();
            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            PaymentViewModel paymentVM = new PaymentViewModel
            {
                StudentAssignment = assignments
            };

            return View(paymentVM);
        }

        // pass on the payment view model for the payment amount to the payment page
        [HttpPost]
        public async Task<IActionResult> PaymentPage(decimal paymentAmount)
        {
            var userId = _contextAccessor.HttpContext.User.GetUserId();

            // provide a list of assignments for the user for the _Layout to display for the notifications
            List<StudentAssignment> assignments = await _assignmentRepository.GetStudentAssignments(userId);

            // gather the current balance for the user
            decimal currentBalance = await _balanceRepository.GetCurrentBalance(userId);
            // insert the current balance and payment amount into the view
            PaymentViewModel paymentVM = new PaymentViewModel
            {
                PaymentAmount = paymentAmount,
                CurrentBalance = currentBalance,
                StudentAssignment = assignments
            };

            // check to verify the user has entered an amount to pay
            if (paymentAmount == 0)
            {
                // gather a list of the balances and insert it into the view
                List<BalanceHistory> balances = await _balanceRepository.GetBalanceHistory(userId);
                paymentVM.Payments = balances;

                return View(paymentVM);
            }

            // send all information to the payment view
            return View("Checkout", paymentVM);
        }


        // run the payment process
        [HttpPost]
        public async Task<IActionResult> CreateCheckout(PaymentViewModel paymentVM)
        {
            // create the amount for the payment for the intent of converting to int64
            decimal paymentAmount = paymentVM.PaymentAmount * 100;

            // create the payment intent (aka the payment reciept)
            var optionsIntent = new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt64(paymentAmount),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var serviceIntent = new PaymentIntentService();
            var receipt = serviceIntent.Create(optionsIntent);

            // create the checkout session
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = Convert.ToInt64(paymentAmount),
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Payment on Tuition",
                                },
                            },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7243/Payment/Success/?recieptNumber=" + receipt.Id, // sends over payment intent id to success page
                CancelUrl = "https://localhost:7243/Payment/Cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            // update the payment intent (aka the payment reciept)
            var options2 = new PaymentIntentUpdateOptions
            {
                Metadata = new Dictionary<string, string> { { "order_id", "6735" } },
            };
            var service2 = new PaymentIntentService();
            service2.Update(receipt.Id, options2);

            // pass on all of the information for checking out to the stripe website
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }



        // testing cases, push a payment, add a class and drop a class, verify payment passes
        public async Task pushPaymentToDb(PaymentViewModel paymentVM, string userId, string recieptNumber)
        {
            // push the view model into a new viewmodel
            PaymentViewModel paymentVM2 = new PaymentViewModel
            {
                PaymentAmount = paymentVM.PaymentAmount
            };

            // create a new balance update for the payment
            await _balanceRepository.UpdateBalancePayment(userId, paymentVM2.PaymentAmount, recieptNumber);
        }
        public async Task pushClassAddedToDb(string userId, int courseHours, string className)
        {
            // create a new balance update for the payment
            await _balanceRepository.UpdateBalanceAddCourse(userId, courseHours, className);
        }
        public async Task pushClassDroppedToDb(string userId, int courseHours, string className)
        {
            // create a new balance update for the payment
            await _balanceRepository.UpdateBalanceDropCourse(userId, courseHours, className);
        }
    }
}