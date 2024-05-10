using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

public class DiscountController : Controller
{
  // this controller depends on the NorthwindRepository
  private DataContext _dataContext;
  private Random _numGenerator = new Random();
  public DiscountController(DataContext db) => _dataContext = db;
  public IActionResult Index() => View(_dataContext.Discounts.Include("Product").Where(d => d.StartTime <= DateTime.Now && d.EndTime > DateTime.Now));
  
  [Authorize(Roles = "northwind-employee")]
  public IActionResult Expired() => View(_dataContext.Discounts.Include("Product").Where(d => d.StartTime > DateTime.Now || d.EndTime <= DateTime.Now));

  [Authorize(Roles = "northwind-employee")]
  public IActionResult AddDiscount() => View(new Discount());

  [Authorize(Roles = "northwind-employee")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult AddDiscount(Discount model)
  {
    Product product = _dataContext.Products.FirstOrDefault(p => p.ProductId == model.ProductId);
    var discountCodes = _dataContext.Discounts.Select(d => d.Code);

    if (ModelState.IsValid)
    {
      if (product == null)
      {
        ModelState.AddModelError("", "Product ID not found.");
      }
      else if (model.DiscountPercent < 0 || model.DiscountPercent >= 1)
      {
        ModelState.AddModelError("", "Percent must be between 0.00 and 0.99.");
      }
      else
      {
        int code;
        do {
          code = _numGenerator.Next(1000, 10000);
        } while(discountCodes.Any(d => d == code));
        model.Code = code;

        _dataContext.AddDiscount(model);
        return RedirectToAction("Index");
      }
    }
    return View();
  }
  
  [Authorize(Roles = "northwind-employee")]
  public IActionResult EditDiscount(int id) => View(_dataContext.Discounts.Include("Product").FirstOrDefault(d => d.DiscountId == id));
  [Authorize(Roles = "northwind-employee")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditDiscount(Discount discount)
  {
      // Edit discount info
    _dataContext.EditDiscount(discount);
    return RedirectToAction("Index");
  }

  [Authorize(Roles = "northwind-employee")]
  public IActionResult DeleteDiscount(int id)
  {
    _dataContext.DeleteDiscount(_dataContext.Discounts.FirstOrDefault(d => d.DiscountId == id));
    return RedirectToAction("Index");
  }
}