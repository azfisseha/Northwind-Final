using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
  public DataContext(DbContextOptions<DataContext> options) : base(options) { }

  public DbSet<Product> Products { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Discount> Discounts { get; set; }
  public DbSet<Customer> Customers { get; set; }
  public DbSet<CartItem> CartItems { get; set; }
  public DbSet<Employee> Employees { get; set; }

  public void AddCustomer(Customer customer)
  {
    Customers.Add(customer);
    SaveChanges();
  }
  public void EditCustomer(Customer customer)
  {
    var customerToUpdate = Customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
    customerToUpdate.Address = customer.Address;
    customerToUpdate.City = customer.City;
    customerToUpdate.Region = customer.Region;
    customerToUpdate.PostalCode = customer.PostalCode;
    customerToUpdate.Country = customer.Country;
    customerToUpdate.Phone = customer.Phone;
    customerToUpdate.Fax = customer.Fax;
    SaveChanges();
  }
  public CartItem AddToCart(CartItemJSON cartItemJSON)
  {
    int CustomerId = Customers.FirstOrDefault(c => c.Email == cartItemJSON.email).CustomerId;
    int ProductId = cartItemJSON.id;
    // check for duplicate cart item
    CartItem cartItem = CartItems.FirstOrDefault(ci => ci.ProductId == ProductId && ci.CustomerId == CustomerId);
    if (cartItem == null)
    {
      // this is a new cart item
      cartItem = new CartItem()
      {
        CustomerId = CustomerId,
        ProductId = cartItemJSON.id,
        Quantity = cartItemJSON.qty
      };
      CartItems.Add(cartItem);
    }
    else
    {
      // for duplicate cart item, simply update the quantity
      cartItem.Quantity += cartItemJSON.qty;
    }
    SaveChanges();
    cartItem.Product = Products.Find(cartItem.ProductId);
    return cartItem;
  }

  public void DeleteDiscount(Discount discount)
  {
    this.Remove(discount);
    this.SaveChanges();
  }

  public void AddDiscount(Discount discount)
  {
    this.Add(discount);
    this.SaveChanges();
  }

  public void EditDiscount(Discount discount)
  {
    var discountToEdit = Discounts.FirstOrDefault(d => d.DiscountId == discount.DiscountId);
    discountToEdit.Title = discount.Title;
    discountToEdit.Description = discount.Description;
    discountToEdit.DiscountPercent = discount.DiscountPercent;
    discountToEdit.StartTime = discount.StartTime;
    discountToEdit.EndTime = discount.EndTime;
    this.SaveChanges();
  }
}
