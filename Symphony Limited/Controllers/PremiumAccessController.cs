using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony_Limited.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Symphony_Limited.Controllers
{
    //[Authorize(Roles = "Admin")] // Use Authorize attribute to restrict access
    public class PremiumAccessController : Controller
    {
        private readonly Symphony_LtdContext _context;

        public PremiumAccessController(Symphony_LtdContext context)
        {
            _context = context;
        }

        // GET: PremiumAccess
        public async Task<IActionResult> Index()
        {
            return View(await _context.PremiumAccesses.Include(p => p.User).ToListAsync());
        }
        public IActionResult PaymentRequired()
        {
            return View();
        }


        public IActionResult ProcessEasypaisaPayment(decimal amount, string customerMobileNumber)
        {
            // Simulate Easypaisa payment processing (replace with actual Easypaisa API calls)
            // In a real scenario, you'd interact with the Easypaisa API here.

            // Example: Generate a dummy transaction ID and status.
            string transactionId = Guid.NewGuid().ToString();
            bool paymentSuccessful = SimulatePaymentSuccess(); // Replace with actual logic

            if (paymentSuccessful)
            {
                // Payment successful. Store transaction details, update database, etc.
                ViewBag.TransactionId = transactionId;
                ViewBag.PaymentStatus = "Successful";
                ViewBag.Amount = amount;
                ViewBag.MobileNumber = customerMobileNumber;

                return View("PaymentSuccess"); // Create a PaymentSuccess.cshtml view
            }
            else
            {
                // Payment failed. Handle the error.
                ViewBag.PaymentStatus = "Failed";
                return View("PaymentFailure"); // Create a PaymentFailure.cshtml view
            }
        }

        public IActionResult ProcessJazzCashPayment(decimal amount, string customerMobileNumber)
        {
            // Simulate JazzCash payment processing (replace with actual JazzCash API calls)
            // In a real scenario, you'd interact with the JazzCash API here.

            // Example: Generate a dummy transaction ID and status.
            string transactionId = Guid.NewGuid().ToString();
            bool paymentSuccessful = SimulatePaymentSuccess(); // Replace with actual logic

            if (paymentSuccessful)
            {
                // Payment successful. Store transaction details, update database, etc.
                ViewBag.TransactionId = transactionId;
                ViewBag.PaymentStatus = "Successful";
                ViewBag.Amount = amount;
                ViewBag.MobileNumber = customerMobileNumber;

                return View("PaymentSuccess"); // Create a PaymentSuccess.cshtml view
            }
            else
            {
                // Payment failed. Handle the error.
                ViewBag.PaymentStatus = "Failed";
                return View("PaymentFailure"); // Create a PaymentFailure.cshtml view
            }
        }

        private bool SimulatePaymentSuccess()
        {
            // Simulate payment success or failure (replace with actual logic).
            // For demonstration, randomly return true or false.
            Random random = new Random();
            return random.Next(2) == 0; // 50% chance of success
        }
        // POST: PremiumAccess/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var premiumAccess = await _context.PremiumAccesses.FindAsync(id);
            if (premiumAccess != null)
            {
                premiumAccess.Status = "Approved";
                _context.Update(premiumAccess);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: PremiumAccess/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var premiumAccess = await _context.PremiumAccesses.FindAsync(id);
            if (premiumAccess != null)
            {
                premiumAccess.Status = "Rejected";
                _context.Update(premiumAccess);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}