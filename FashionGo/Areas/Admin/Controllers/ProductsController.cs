using FashionGo.Models;
using FashionGo.Models.Entities;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FashionGo.Areas.Admin.Controllers
{
    public class ProductsController : AdminController
    {
        // GET: Admin/Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.User).Include(p => p.Manufact).Include(p => p.ProductCategory).Include(p => p.ProductType).OrderByDescending(p => p.CreateDate);
            return View(products.ToList());
        }
        public ActionResult ListProductsSize(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                var products = db.Sizes.Include(p => p.Product).ToList();
                return View(products);
            }
            else
            {
                var products = db.Sizes.Include(p => p.Product).Where(x => x.Product.Name.Contains(searchString)).ToList();
                return View(products);
            }
        }
        public ActionResult DeleteProductsSize(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sizes product = db.Sizes.Find(id);
            db.Sizes.Remove(product);
            db.SaveChanges();
            if (product == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("ListProductsSize");
        }
        [HttpPost, ActionName("DeleteProductsSize")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductsSizeConfirmed(int id)
        {
            Sizes product = db.Sizes.Find(id);
            db.Sizes.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ListProductsColor(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                var products = db.Colors.Include(p => p.Product).ToList();
                return View(products);
            }
            else
            {
                var products = db.Colors.Include(p => p.Product).Where(x => x.Product.Name.Contains(searchString)).ToList();
                return View(products);
            }
        }
        public ActionResult DeleteProductsColor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colors product = db.Colors.Find(id);
            db.Colors.Remove(product);
            db.SaveChanges();
            return RedirectToAction("ListProductsColor");
        }

        public ActionResult CreateProperties()
        {
            ViewBag.Products = new SelectList(db.Products, "id", "name");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateProperties(ColorSizeViewModel data)
        {
            if (ModelState.IsValid)
            {
                string ProductId = Request.Form["Id"];
                int selectedProductId;

                int.TryParse(ProductId, out selectedProductId);

                string[] colors = data.Color.ColorName.Split(',');

                foreach (string color in colors)
                {
                    // Lưu từng tùy chọn màu vào cơ sở dữ liệu
                    string colorName = color.Trim().ToUpper();
                    data.Color.ProductID = selectedProductId;
                    db.Colors.Add(new Colors { ColorName = colorName, ProductID = data.Color.ProductID });
                }

                string[] sizes = data.Size.SizeName.Split(',');

                foreach (string size in sizes)
                {
                    // Lưu từng tùy chọn màu vào cơ sở dữ liệu
                    string sizeName = size.Trim().ToUpper();
                    data.Size.ProductID = selectedProductId;
                    db.Sizes.Add(new Sizes { SizeName = sizeName, ProductID = data.Size.ProductID });
                }

                db.SaveChanges();
                return RedirectToAction("ListProductsColor");
            }


            return View(data);
        }





        public ActionResult CreatePropertiesColor()
        {
            ViewBag.Products = new SelectList(db.Products, "id", "name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreatePropertiesColor(ColorSizeViewModel data)
        {
            if (ModelState.IsValid)
            {
                string ProductId = Request.Form["Id"];
                int selectedProductId;

                int.TryParse(ProductId, out selectedProductId);

                string[] colors = data.Color.ColorName.Split(',');

                foreach (string color in colors)
                {
                    // Lưu từng tùy chọn màu vào cơ sở dữ liệu
                    string colorName = color.Trim().ToUpper();
                    data.Color.ProductID = selectedProductId;
                    db.Colors.Add(new Colors { ColorName = colorName, ProductID = data.Color.ProductID });
                }




                db.SaveChanges();
                return RedirectToAction("CreatePropertiesColor");
            }


            return View(data);
        }

        public ActionResult UpdatePropertiesColor(int? id)
        {
            Colors color = db.Colors.Find(id);

            ViewBag.Products = new SelectList(db.Products, "id", "name", color.ProductID);
            ViewBag.ProductName = db.Products.Where(x => x.Id == color.ProductID).FirstOrDefault();
            return View(new ColorSizeViewModel
            {
                Color = color,
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdatePropertiesColor(ColorSizeViewModel data)
        {
            if (ModelState.IsValid)
            {
                string ProductId = Request.Form["Id"];
                int selectedProductId;

                int.TryParse(ProductId, out selectedProductId);
                Colors product = db.Colors.Where(X => X.ColorID == data.Color.ColorID).FirstOrDefault();
                product.ColorName = data.Color.ColorName;

                db.SaveChanges();
                return RedirectToAction("ListProductsColor");
            }


            return View(data);
        }

        public ActionResult UpdatePropertiesSize(int? id)
        {
            Sizes product = db.Sizes.Find(id);

            ViewBag.Products = new SelectList(db.Products, "id", "name", product.ProductID);
            ViewBag.ProductName = db.Products.Where(x => x.Id == product.ProductID).FirstOrDefault();

            return View(new ColorSizeViewModel
            {
                Size = product,
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdatePropertiesSize(ColorSizeViewModel data)
        {
            if (ModelState.IsValid)
            {
                string ProductId = Request.Form["Id"];
                int selectedProductId;

                int.TryParse(ProductId, out selectedProductId);
                Sizes product = db.Sizes.Where(X => X.SizeID == data.Size.SizeID).FirstOrDefault();
                product.SizeName = data.Size.SizeName;

                db.SaveChanges();
                return RedirectToAction("ListProductsSize");
            }


            return View(data);
        }
        public ActionResult CreatePropertiesSize()
        {
            ViewBag.Products = new SelectList(db.Products, "id", "name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreatePropertiesSize(ColorSizeViewModel data)
        {
            if (ModelState.IsValid)
            {
                string ProductId = Request.Form["Id"];
                int selectedProductId;

                int.TryParse(ProductId, out selectedProductId);




                string[] sizes = data.Size.SizeName.Split(',');

                foreach (string size in sizes)
                {
                    // Lưu từng tùy chọn màu vào cơ sở dữ liệu
                    string sizeName = size.Trim().ToUpper();
                    data.Size.ProductID = selectedProductId;
                    db.Sizes.Add(new Sizes { SizeName = sizeName, ProductID = data.Size.ProductID });
                }

                db.SaveChanges();
                return RedirectToAction("ListProductsSize");
            }


            return View(data);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            //ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.ManufactId = new SelectList(db.Manufacts, "id", "name");
            ViewBag.CatId = new SelectList(db.ProductCategories.Where(x => x.ParentId != null), "CatId", "Name");
            ViewBag.TypeId = new SelectList(db.ProductTypes, "id", "name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToAscii();
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", product.UserId);
            ViewBag.ManufactId = new SelectList(db.Manufacts, "id", "name", product.ManufactId);
            ViewBag.CatId = new SelectList(db.ProductCategories.Where(x => x.ParentId != null), "CatId", "Name", product.CatId);
            ViewBag.TypeId = new SelectList(db.ProductTypes, "id", "name", product.TypeId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", product.UserId);
            ViewBag.ManufactId = new SelectList(db.Manufacts, "id", "name", product.ManufactId);
            ViewBag.CatId = new SelectList(db.ProductCategories.Where(x => x.ParentId != null), "CatId", "Name", product.CatId);
            ViewBag.TypeId = new SelectList(db.ProductTypes, "id", "name", product.TypeId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                product.Slug = product.Name.ToAscii();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", product.UserId);
            ViewBag.ManufactId = new SelectList(db.Manufacts, "id", "name", product.ManufactId);
            ViewBag.CatId = new SelectList(db.ProductCategories.Where(x => x.ParentId != null), "CatId", "Name", product.CatId);
            ViewBag.TypeId = new SelectList(db.ProductTypes, "id", "name", product.TypeId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                Product product = db.Products.Include(p => p.Color)
                    .Include(o => o.OrderDetails)
                    .Include(s => s.Size)
                    .FirstOrDefault(p => p.Id == id);

                if (product != null)
                {
                    var colors = db.Colors.Where(c => c.ProductID == product.Id).ToList();
                    var sizes = db.Sizes.Where(c => c.ProductID == product.Id).ToList();
                    var orders = db.OrderDetails.Where(c => c.ProductId == product.Id).ToList();
                    foreach (var color in colors)
                    {
                        db.Colors.Remove(color);
                    }
                    foreach (var size in sizes)
                    {
                        db.Sizes.Remove(size);
                    }
                    foreach (var order in orders)
                    {
                        db.OrderDetails.Remove(order);
                    }
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
