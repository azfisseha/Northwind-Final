using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class DiscountController : Controller
{
  // this controller depends on the NorthwindRepository
  private DataContext _dataContext;
  public DiscountController(DataContext db) => _dataContext = db;
  public IActionResult Index() => View(_dataContext.Discounts.Include("Product").Where(d => d.StartTime <= DateTime.Now && d.EndTime > DateTime.Now));
}