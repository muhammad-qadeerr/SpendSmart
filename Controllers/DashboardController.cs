using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Syncfusion.EJ2.Schedule;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            // Considering Last 7 days transactions

            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transations
                    .Include(x => x.Category)
                    .Where(y => y.TransactionDate >= StartDate && y.TransactionDate <= EndDate)
                    .ToListAsync();

            // Total Income
            int totalIncome = SelectedTransactions
                .Where(i => i.Category?.CategoryType == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = totalIncome.ToString("C0");

            // Total Expense
            int totalExpense = SelectedTransactions
                .Where(i => i.Category?.CategoryType == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = totalExpense.ToString("C0");

            // Balance App
            int balance = totalIncome - totalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", balance);

            // Donut Chart - Expense By Category
            ViewBag.DonutChartData = SelectedTransactions
                .Where(x => x.Category?.CategoryType == "Expense")
                .GroupBy(y => y.Category?.CategoryId)
                .Select(z => new
                {
                    categoryTitleWithIcon = z.First().Category?.Icon + " " + z.First().Category?.Title,
                    amount = z.Sum(y => y.Amount),
                    formattedAmount = z.Sum(y => y.Amount).ToString("C0"),
                })
                .OrderByDescending(a => a.amount)
                .ToList();

            // SplineChart - Income Vs. Expense

            // Income
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category?.CategoryType == "Income")
                .GroupBy(j => j.TransactionDate)
                .Select(k => new SplineChartData
                {
                    day = k.First().TransactionDate.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount),
                }).ToList();

            // Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category?.CategoryType == "Expense")
                .GroupBy(j => j.TransactionDate)
                .Select(k => new SplineChartData
                {
                    day = k.First().TransactionDate.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount),
                }).ToList();


            // Combine Income & Expense
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            // Recent Transactions

            ViewBag.RecentTransactions = await _context.Transations
                .Include(i => i.Category)
                .OrderByDescending(j => j.TransactionDate)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }

}
