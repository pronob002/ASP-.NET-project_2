using FinalApp.Models;
using FinalApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using FinalApp.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(ApplicationDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = model.Name,
                Email = model.Email,
                Age = model.Age,
                Address = model.Address,
                MobileNumber = model.MobileNumber,
                Role = model.Role,
                Password = model.Password
            };

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var fileName = Path.GetFileName(model.ProfileImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImagePath = $"/uploads/{fileName}";
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        return View(model);
    }


 
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
   
            if (model.Email == "admin@gmail.com" && model.Password == "123")
            {
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("AdminDashboard");
            }
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            if (user != null)
            {

                HttpContext.Session.SetString("UserId", user.UserId.ToString());

                if (user.Role == "Seller")
                {
                    return RedirectToAction("SellerHome");
                }
                else
                {
                    return RedirectToAction("CustomerHome", new { userId = user.UserId });
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
        }

        return View(model);
    }

    public IActionResult SellerHome()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login");
        }

        var userGuid = Guid.Parse(userId);
        var user = _context.Users.FirstOrDefault(u => u.UserId == userGuid);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var items = _context.Items
            .Where(i => i.SellerId == userGuid.ToString())
            .ToList();

        ViewBag.UserId = user.UserId;
        ViewBag.UserName = user.Name;
        ViewBag.ProfileImagePath = user.ProfileImagePath; 
        ViewBag.Items = items;

        return View();
    }


    [HttpGet]
    public IActionResult AddItem()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(AddItemViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var userGuid = Guid.Parse(userId);

            var item = new Item
            {
                ItemId = Guid.NewGuid(),
                ItemName = model.ItemName,
                Price = model.Price,
                Description = model.Description,
                SellerId = userGuid.ToString()
            };

            if (model.ItemImage != null && model.ItemImage.Length > 0)
            {
                var fileName = Path.GetFileName(model.ItemImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ItemImage.CopyToAsync(stream);
                }

                item.ItemImagePath = $"/uploads/{fileName}";
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("SellerHome");
        }

        return View(model);
    }



    [HttpGet]
    public async Task<IActionResult> EditItem(Guid id, bool? flag)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return RedirectToAction("SellerHome");
        }

        var model = new EditItemViewModel
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            Price = item.Price,
            Description = item.Description,
            ExistingItemImagePath = item.ItemImagePath
        };

        // Store flag in ViewData to use in POST method
        ViewData["Flag"] = flag;

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(EditItemViewModel model)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine("Model state is not valid.");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(model);
        }

        var item = await _context.Items.FindAsync(model.ItemId);
        if (item == null)
        {
            return RedirectToAction("SellerHome");
        }

        item.ItemName = model.ItemName;
        item.Price = model.Price;
        item.Description = model.Description;

        if (model.ItemImage != null && model.ItemImage.Length > 0)
        {
            var fileName = Path.GetFileName(model.ItemImage.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            Console.WriteLine($"File path: {filePath}");


            if (!string.IsNullOrEmpty(item.ItemImagePath))
            {
                var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetFileName(item.ItemImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                    Console.WriteLine($"Deleted old file: {existingFilePath}");
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ItemImage.CopyToAsync(stream);
                Console.WriteLine($"Uploaded new file: {filePath}");
            }

            item.ItemImagePath = $"/uploads/{fileName}";
        }

        try
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            Console.WriteLine("Changes saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
            ModelState.AddModelError("", "Error saving changes. Please try again.");
            return View(model);
        }

        return RedirectToAction("SellerHome");
    }









    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(Guid id, bool flag)
    {
        _logger.LogInformation("DeleteItem action invoked for ItemId: {ItemId}", id);

        var item = _context.Items.FirstOrDefault(i => i.ItemId == id);

        if (item != null)
        {
            _logger.LogInformation("Item found. Preparing to delete ItemId: {ItemId}", id);

            if (!string.IsNullOrEmpty(item.ItemImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ItemImagePath);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    _logger.LogInformation("Image file deleted: {ImagePath}", imagePath);
                }
                else
                {
                    _logger.LogWarning("Image file not found: {ImagePath}", imagePath);
                }
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Item deleted from database. ItemId: {ItemId}", id);
        }
        else
        {
            _logger.LogWarning("Item not found in database. ItemId: {ItemId}", id);
        }


        if (flag)
        {
       
            _logger.LogInformation("Redirecting to AdminDashboard based on flag.");
            return RedirectToAction("AdminDashboard", "User");
        }
        else
        {
            // Otherwise, redirect to SellerHome
            _logger.LogInformation("Redirecting to SellerHome based on flag.");
            return RedirectToAction("SellerHome", "User");
        }
    }
















    [HttpGet]
    public IActionResult Logout()
    {

        HttpContext.Session.Clear();


        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult CustomerHome(Guid userId, string searchQuery = "", bool flag = false)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        ViewBag.UserName = user.Name;
        ViewBag.SearchQuery = searchQuery;
        ViewBag.ProfileImagePath = user.ProfileImagePath;
        ViewBag.Flag = flag;

        var items = _context.Items
            .Where(i => i.ItemName.Contains(searchQuery))
            .ToList();


        var selectedItemCount = HttpContext.Session.GetInt32("SelectedItemCount") ?? 0;
        ViewBag.SelectedItemCount = selectedItemCount;

        ViewBag.Items = items;
        ViewBag.UserId = userId;

        return View();
    }

    [HttpGet]
    public IActionResult EditProfile(Guid userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new EditProfileViewModel
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
            Address = user.Address,
            MobileNumber = user.MobileNumber,
            ProfileImagePath = user.ProfileImagePath 
        };


        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            Console.WriteLine("Model state is not valid.");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(model);
        }

        var user = await _context.Users.FindAsync(model.UserId);
        if (user == null)
        {
            Console.WriteLine($"User with ID {model.UserId} not found.");
            return RedirectToAction("Login");
        }

        user.Name = model.Name;
        user.Email = model.Email;
        user.Age = model.Age;
        user.Address = model.Address;
        user.MobileNumber = model.MobileNumber;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var fileName = Path.GetFileName(model.ProfileImage.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            Console.WriteLine($"File path: {filePath}");

            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetFileName(user.ProfileImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                    Console.WriteLine($"Deleted old file: {existingFilePath}");
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(stream);
                Console.WriteLine($"Uploaded new file: {filePath}");
            }

            user.ProfileImagePath = $"/uploads/{fileName}";
        }

        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            Console.WriteLine("Changes saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
            ModelState.AddModelError("", "Error saving changes. Please try again.");
            return View(model);
        }

        // Redirect based on user role
        if (user.Role == "Seller") // Adjust this condition based on your actual role naming
        {
            return RedirectToAction("SellerHome", new { userId = user.UserId });
        }
        else
        {
            return RedirectToAction("CustomerHome", new { userId = user.UserId });
        }
    }







    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectedItems(List<Guid> selectedItems, Guid userId)
    {
        if (selectedItems == null || !selectedItems.Any())
        {
            return RedirectToAction("CustomerHome");
        }

        var items = await _context.Items
            .Where(i => selectedItems.Contains(i.ItemId))
            .ToListAsync();

        var viewModel = new SelectedItemsViewModel
        {
            UserId = userId,
            Items = items
        };

        var existingCart = await _context.Carts.SingleOrDefaultAsync(c => c.UserId == userId);
        if (existingCart == null)
        {
            var newCart = new Cart { UserId = userId };
            _context.Carts.Add(newCart);
            await _context.SaveChangesAsync();
            existingCart = newCart;
        }

        return View(viewModel);
    }



    [HttpPost]
    public IActionResult BuyItems(Guid userId, List<Guid> itemIds, Dictionary<Guid, int> quantities, Dictionary<Guid, Guid> sellerIds)
    {
        foreach (var itemId in itemIds)
        {
            var quantity = quantities[itemId];
            var sellerId = sellerIds[itemId];

            var item = _context.Items.FirstOrDefault(i => i.ItemId == itemId);
            if (item != null)
            {
                var purchase = new Purchase
                {
                    SellerId = sellerId,
                    ItemId = itemId,
                    CustomerId = userId,
                    Price = item.Price,
                    Quantity = quantity,
                    TotalPrice = item.Price * quantity
                };

                _context.Purchases.Add(purchase);
            }
        }

        _context.SaveChanges();

        var userCartItems = _context.Carts.Where(c => c.UserId == userId).ToList();
        if (userCartItems.Any())
        {
            _context.Carts.RemoveRange(userCartItems);
            _context.SaveChanges();
        }

        TempData["SuccessMessage"] = "Items purchased successfully!";
        return RedirectToAction("CustomerHome", "User", new { userId, flag = true });

    }

















    public IActionResult SeeCustomers()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login");
        }

        var sellerGuid = Guid.Parse(userId);


        var purchases = _context.Purchases
            .Where(p => p.SellerId == sellerGuid)
            .Select(p => new
            {
                CustomerId = p.CustomerId,
                ItemId = p.ItemId,
                Quantity = p.Quantity,
                TotalPrice = p.TotalPrice,

                PerUnitPrice = p.Quantity != 0 ? p.TotalPrice / p.Quantity : 0
            })
            .ToList();


        var customers = purchases
            .GroupBy(p => p.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                CustomerName = _context.Users.FirstOrDefault(u => u.UserId == g.Key)?.Name,
                TotalSpent = g.Sum(p => p.TotalPrice),
                ItemsPurchased = g.Select(p => new
                {
                    ItemId = p.ItemId,
                    ItemName = _context.Items.FirstOrDefault(i => i.ItemId == p.ItemId)?.ItemName,
                    Quantity = p.Quantity,
                    PerUnitPrice = p.PerUnitPrice,
                    TotalPrice = p.TotalPrice
                }).ToList()
            })
            .ToList();


        return View(customers);
    }




    public IActionResult AdminDashboard()
    {
        if (HttpContext.Session.GetString("Role") == "Admin")
        {

            var customers = _context.Users.Where(u => u.Role == "Customer").ToList();
            var sellers = _context.Users.Where(u => u.Role == "Seller").ToList();
            var items = _context.Items.ToList();


            var itemPurchaseQuantities = _context.Purchases
                .GroupBy(p => p.ItemId)
                .Select(g => new
                {
                    ItemId = g.Key,
                    TotalQuantity = g.Sum(p => p.Quantity)
                })
                .ToList();


            var itemPurchaseDetails = items.ToDictionary(
                item => item.ItemId,
                item => new ItemPurchaseDetail
                {
                    ItemName = item.ItemName,
                    PurchaseCount = itemPurchaseQuantities.FirstOrDefault(ip => ip.ItemId == item.ItemId)?.TotalQuantity ?? 0
                });

            var viewModel = new AdminDashboardViewModel
            {
                Customers = customers,
                Sellers = sellers,
                Items = items,
                ItemPurchaseDetails = itemPurchaseDetails
            };

            return View(viewModel);
        }
        else
        {
            return RedirectToAction("Login");
        }
    }


    public IActionResult UpdateCartCount()
    {
        var cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
        return Json(new { count = cartCount });
    }


    [HttpPost]
    public IActionResult AddToCart(Guid itemId)
    {
        var cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
        cartCount++;
        HttpContext.Session.SetInt32("CartCount", cartCount);

        return RedirectToAction("CustomerHome");
    }





    [HttpGet]
    public async Task<IActionResult> CheckUserIdExists(Guid userId)
    {
        var exists = await _context.Carts.AnyAsync(cart => cart.UserId == userId);
        return Json(new { exists });
    }














    public IActionResult ShowAllItems()
    {
        var items = _context.Items.ToList();  // Retrieve all items from the database
        var viewModel = new AdminDashboardViewModel
        {
            Items = items,
            // Populate other properties (Customers, Sellers, etc.)
        };

        return View(viewModel);
    }








}

