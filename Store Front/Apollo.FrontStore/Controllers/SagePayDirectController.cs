using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Infrastructure;
using Apollo.Web.Framework.Security;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class SagePayDirectController : BasePublicController
    {
        private readonly IPaymentService _paymentService;
        private readonly ApolloSessionState _session;

        public SagePayDirectController(IPaymentService paymentService, ApolloSessionState session)
        {
            _paymentService = paymentService;
            _session = session;
        }

        [HttpPost]
        public ActionResult Terminal(FormCollection form)
        {
            string md = form["MD"];
            string paRes = form["PaRes"];

            var output = _paymentService.ProcessPaymentAfter3DCallback(md, paRes, true);

            if (output.Status)
            {
                return RedirectToRoute("Checkout Completed", new { orderid = output.OrderId, emailinvoiceid = output.EmailInvoiceId, hasnhs = output.HasNHSPrescription ? 1 : 0 });
            }
            else
            {
                _session["PaymentErrorMessage"] = output.Message;
                return RedirectToAction("Payment", "Checkout");
            }
        }
    }
}