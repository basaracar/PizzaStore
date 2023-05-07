using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using PizzaStore.Data;
using PizzaStore.Models;
using PizzaStore.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Web.Helpers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using PizzaStore.Helper;

namespace PizzaStore.Controllers
{
    public class PizzasController : Controller
    {
        private readonly AppDbContext _context;

        public PizzasController(AppDbContext context)
        {
            _context = context;
        }
        private int? CartCount()
        {
          //  var total = HttpContext.Session.Get<List<Cart>>("cartItems").Sum(x => x.adet);
            return HttpContext.Session.Get<List<Cart>>("cartItems") != null? HttpContext.Session.Get<List<Cart>>("cartItems").Count():0;
        }
        public IActionResult SepeteEkle(Cart cart)
        {

            //var cart = new Cart();
          
            //cart.adet = adet;
            //cart.ItemId = itemId;
            if (HttpContext.Session.Get<List<Cart>>("cartItems") == null)
            {
                List<Cart> liste = new List<Cart>();
                liste.Add(cart);
                HttpContext.Session.Set<List<Cart>>("cartItems", liste);


            }
            else
            {
                var json = HttpContext.Session.GetString("cartItems");
                var valueP = HttpContext.Session.Get<List<Cart>>("cartItems");
                valueP.Add(cart);
                HttpContext.Session.Set<List<Cart>>("cartItems", valueP);
                //  var model = JsonConvert.DeserializeObject<List<Cart>>(valueP);
            }




            return RedirectToAction("Index");
            //model.Add(new Cart())

        }
        public async Task<IActionResult> Sepet()
        {
            ViewBag.Sepet = CartCount();
         
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            var sepet = HttpContext.Session.Get<List<Cart>>("cartItems");
            List<CartList> liste = new List<CartList>();
            
            if (sepet != null)
            {
                foreach (var item in sepet)
                {
                    var product=_context.Pizzas.Include(i=>i.Category).FirstOrDefault(x=>x.Id==item.ItemId);
                    liste.Add(new CartList { Item = product, Adet = item.adet });
                }

            }
            return View(liste);
        }
        public IActionResult Login()
        {
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            var hash=Crypto.HashPassword(user.Password);

            if (ModelState.IsValid) { 
                User? account=_context.Users.FirstOrDefault(u=>u.Email.Equals(user.Email)&&u.Password.Equals(user.Password));
                if (account != null)
                {
            Claim[] claims =
            {
               
                new Claim("Name",account.Name),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role,account.role.ToString()),
            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();
                    //Sürekli açık kalsın diye beni hatırla
                    authProperties.IsPersistent = true;
                    //HttpContext.SignInAsync
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index");
                   
                }
                else
                {
                    ViewBag.Hata = "Kullanıcı Adı Veya Şifre Hatalı";
                }
                
            }
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }

        // GET: Pizzas
     
        public async void ViewbagDoldur()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
        }
        public async Task<IActionResult> Index(int? id)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();

            if ( id!=null)
            return _context.Pizzas != null ?
                          View(await _context.Pizzas.Include(i => i.Category).Where(k => k.CategoryId == id).ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Pizzas'  is null.");
            else return _context.Pizzas != null ?
                          View(await _context.Pizzas.Include(i => i.Category).ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Pizzas'  is null.");
        }

        // GET: Pizzas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            if (id == null || _context.Pizzas == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(pizza);
        }
        [Authorize]
        // GET: Pizzas/Create
        public async Task<IActionResult> Create()
        {
            //ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
             ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Pizzas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,CategoryId")] Pizza pizza,IFormFile file)
        {
            //ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            if (file != null)
            {
                var yeniPhoto=CopyPhoto(file);
                if (yeniPhoto != null)
                {
                    pizza.Photo = yeniPhoto;
                    if (ModelState.IsValid)
                    {
                        _context.Add(pizza);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else ModelState.AddModelError("Photo", "Fotoğraf Yüklenemedi");
                
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View(pizza);
        }

        // GET: Pizzas/Edit/5
        [Authorize]

        public async Task<IActionResult> Edit(int? id)
        {
          //  ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            if (id == null || _context.Pizzas == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View(pizza);
        }

        // POST: Pizzas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        private bool RemovePhoto(string photo)
        {
            var silPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", photo);

            if (System.IO.File.Exists(silPath))
            {
                try
                {
                    System.IO.File.Delete(silPath);
                }catch(Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
        private string? CopyPhoto(IFormFile file)
        {
            var extent = Path.GetExtension(file.FileName);
            var randomName = ($"{Guid.NewGuid()}{extent}");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", randomName);

            if (file.ContentType == "image/png" || file.ContentType == "image/jpeg")
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    try { 
                    file.CopyTo(stream);
                    }catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
            return randomName;
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Photo,Price,CategoryId")] Pizza pizza, IFormFile? file)
        {
           // ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            if (id != pizza.Id)
            {
                return NotFound();
            }
            
            if (file != null)
            {
                var yeniPhoto=CopyPhoto(file);
                if(yeniPhoto != null) {
                    if (RemovePhoto(pizza.Photo)) pizza.Photo = yeniPhoto;
                    else { ViewBag.Hata = "Resim Silinemedi"; RemovePhoto(yeniPhoto); }
                }
                else ViewBag.Hata = "Resim Yüklenemedi";

            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pizza);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PizzaExists(pizza.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View(pizza);
        }

        // GET: Pizzas/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            //ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            if (id == null || _context.Pizzas == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(pizza);
        }

        // POST: Pizzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           // ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Sepet = CartCount();
            var identity = User.Identity as ClaimsIdentity;
            ViewBag.Name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).FirstOrDefault();
            ViewBag.Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
            if (_context.Pizzas == null)
            {
                return Problem("Entity set 'AppDbContext.Pizzas'  is null.");
            }
            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza != null)
            {
                RemovePhoto(pizza.Photo);
                _context.Pizzas.Remove(pizza);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PizzaExists(int id)
        {
          return (_context.Pizzas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
