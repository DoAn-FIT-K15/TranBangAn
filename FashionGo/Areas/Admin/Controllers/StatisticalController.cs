using FashionGo.Controllers;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace FashionGo.Areas.Admin.Controllers
{
    public class StatisticalController : BaseController
    {
        public ActionResult BestSeller(int? sl, string selectedMonthYear)
        {
            if (string.IsNullOrEmpty(selectedMonthYear))
            {
                selectedMonthYear = DateTime.Now.ToString("MM-yyyy");
            }
            var query = db.OrderDetails.Include(od => od.Order).AsQueryable();

            if (!string.IsNullOrEmpty(selectedMonthYear))
            {
                string[] monthYear = selectedMonthYear.Split('-');
                if (monthYear.Length == 2)
                {
                    int month = int.Parse(monthYear[0]);
                    int year = int.Parse(monthYear[1]);

                    query = query.Where(od => od.Order.OrderDate.Value.Month == month && od.Order.OrderDate.Value.Year == year);
                }
            }

            var mostPurchasedProducts = query
                .GroupBy(od => od.ProductId)
                .OrderByDescending(g => g.Sum(od => od.Amount))
                .Select(group => new
                {
                    Amount = group.Sum(od => od.Amount),
                    Product = group.FirstOrDefault().Product
                });

            if (sl.HasValue && sl > 0)
            {
                mostPurchasedProducts = mostPurchasedProducts.Take(sl.Value);
            }
            else
            {
                mostPurchasedProducts = mostPurchasedProducts.Take(4);
            }
            var bestSellers = mostPurchasedProducts.ToArray();
            var quantities = bestSellers.Select(group => group.Amount).ToArray();
            var productNames = bestSellers.Select(group => group.Product.Name).ToArray();

            ViewBag.Sort = selectedMonthYear;
            ViewBag.SoLuong = sl;
            ViewBag.DataA = quantities;
            ViewBag.DataB = productNames;
            return View();
        }


        public ActionResult Revenue()
        {

            var currentYear = DateTime.Now.Year;
            var monthlyRevenue = db.Orders
           .Where(o => o.OrderDate.Value.Year == currentYear)
           .GroupBy(o => new { Year = o.OrderDate.Value.Year, Month = o.OrderDate.Value.Month })
           .Select(g => new
           {
               Year = g.Key.Year,
               Month = g.Key.Month,
               TotalRevenue = g.Sum(o => o.TotalOrder)
           })
           .OrderBy(g => g.Year)
           .ThenBy(g => g.Month)
           .ToList();

            // Tạo danh sách chứa 12 tháng từ 1 đến 12
            var allMonths = Enumerable.Range(1, 12);

            // Kết hợp danh sách tháng với danh sách doanh thu theo tháng
            var resultWithZeroRevenue = allMonths
                .GroupJoin(
                    monthlyRevenue,
                    m => m,
                    r => r.Month,
                    (month, revenue) => new
                    {
                        Year = 0, // Đặt Year = 0 vì ta không quan tâm đến năm trong trường hợp này
                        Month = month,
                        TotalRevenue = revenue.Sum(r => r.TotalRevenue)
                    })
                .OrderBy(r => r.Month)
                .ToArray();

            var result = resultWithZeroRevenue.Select(group => group.TotalRevenue).ToArray();

            ViewBag.Data = result;
            ViewBag.Year = currentYear;
            return View(ViewBag);
        }

        [HttpPost]
        public ActionResult Revenue(int? year)
        {
            year = year == null ? DateTime.Now.Year : year;
            var monthlyRevenue = db.Orders
           .Where(o => o.OrderDate.Value.Year == year)
           .GroupBy(o => new { Year = o.OrderDate.Value.Year, Month = o.OrderDate.Value.Month })
           .Select(g => new
           {
               Year = g.Key.Year,
               Month = g.Key.Month,
               TotalRevenue = g.Sum(o => o.TotalOrder)
           })
           .OrderBy(g => g.Year)
           .ThenBy(g => g.Month)
           .ToList();

            // Tạo danh sách chứa 12 tháng từ 1 đến 12
            var allMonths = Enumerable.Range(1, 12);

            // Kết hợp danh sách tháng với danh sách doanh thu theo tháng
            var resultWithZeroRevenue = allMonths
                .GroupJoin(
                    monthlyRevenue,
                    m => m,
                    r => r.Month,
                    (month, revenue) => new
                    {
                        Year = 0, // Đặt Year = 0 vì ta không quan tâm đến năm trong trường hợp này
                        Month = month,
                        TotalRevenue = revenue.Sum(r => r.TotalRevenue)
                    })
                .OrderBy(r => r.Month)
                .ToArray();

            var result = resultWithZeroRevenue.Select(group => group.TotalRevenue).ToArray();

            ViewBag.Data = result;
            ViewBag.Year = year;

            return View(ViewBag);
        }

        [HttpPost]
        public ActionResult BestSeller(int sl)
        {
            var mostPurchasedProducts = db.OrderDetails
                .GroupBy(od => od.ProductId)
                .OrderByDescending(g => g.Sum(od => od.Amount))
                .Take(sl)
                .Select(group => new
                {
                    Amount = group.Sum(od => od.Amount),
                    Product = group.FirstOrDefault().Product
                })
                .ToArray();

            var quantities = mostPurchasedProducts.Select(group => group.Amount).ToArray();
            var productNames = mostPurchasedProducts.Select(group => group.Product.Name).ToArray();

            ViewBag.DataA = quantities;
            ViewBag.DataB = productNames;
            return View(ViewBag);
        }
    }
}