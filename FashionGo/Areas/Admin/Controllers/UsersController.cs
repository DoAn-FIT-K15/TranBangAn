using FashionGo.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FashionGo.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Users
        public class UserModel
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string DistrictName { get; set; }
            public string Role { get; set; }

        }
        public ActionResult Index()
        {
            var identityUsers = db.Users.Include(a => a.District).Include(a => a.Roles).ToList();
            var listRole = db.Roles.ToList();
            var listUserRole = db.IdentityUserRoles.ToList();
            List<UserModel> users = new List<UserModel>();
            foreach (var item in identityUsers)
            {
                var roleId = listUserRole.Where(x => x.UserId == item.Id).FirstOrDefault() != null ?
                    listUserRole.Where(x => x.UserId == item.Id).FirstOrDefault().RoleId : "";
                users.Add(new UserModel
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    UserName = item.UserName,

                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    Address = item.Address,
                    DistrictName = item.District != null ? item.District.Name : "",
                    Role = roleId != "" ? listRole.Where(x => x.Id == roleId).FirstOrDefault().Name
                    : ""
                });
            }
            ViewBag.listRole = listRole;
            return View(users);
        }

        // GET: Admin/Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            ViewBag.DistrictId = new SelectList(db.Districts, "DistrictId", "Name");
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApplicationUser applicationUser, string RoleId)
        {
            applicationUser.Id = Guid.NewGuid().ToString();
            db.Users.Add(applicationUser);
            db.SaveChanges();
            IdentityUserRole identityUserRole = new IdentityUserRole();
            identityUserRole.RoleId = RoleId;
            identityUserRole.UserId = applicationUser.Id;
            db.IdentityUserRoles.Add(identityUserRole);
            db.SaveChanges();
            return RedirectToAction("Index");


#pragma warning disable CS0162 // Unreachable code detected
            ViewBag.DistrictId = new SelectList(db.Districts, "DistrictId", "Name", applicationUser.DistrictId);
#pragma warning restore CS0162 // Unreachable code detected
            return View(applicationUser);
        }

        // GET: Admin/Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var listUserRole = db.IdentityUserRoles.ToList();
            var roleId = listUserRole.Where(x => x.UserId == id).FirstOrDefault() != null ?
                 listUserRole.Where(x => x.UserId == id).FirstOrDefault().RoleId : "";
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", roleId);
            ViewBag.DistrictId = new SelectList(db.Districts, "DistrictId", "Name", applicationUser.DistrictId);
            return View(applicationUser);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FullName,Address,DistrictId")] ApplicationUser applicationUser)
        public ActionResult Edit([Bind(Include = "Id,Email,PhoneNumber,UserName,FullName,Address,DistrictId")] ApplicationUser applicationUser, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(x => x.Id == applicationUser.Id).FirstOrDefault();
                user.Email = applicationUser.Email;
                user.PhoneNumber = applicationUser.PhoneNumber;
                user.UserName = applicationUser.UserName;
                user.FullName = applicationUser.FullName;
                user.Address = applicationUser.Address;
                user.DistrictId = applicationUser.DistrictId;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                IdentityUserRole identityUserRole = db.IdentityUserRoles.Where(x => x.UserId == applicationUser.Id).FirstOrDefault();
                if (identityUserRole == null)
                {
                    db.IdentityUserRoles.Add(new IdentityUserRole
                    {
                        RoleId = RoleId,
                        UserId = applicationUser.Id,

                    });
                    db.SaveChanges();
                }
                else
                {
                    db.IdentityUserRoles.Remove(identityUserRole);
                    IdentityUserRole UserRole = new IdentityUserRole();
                    UserRole.RoleId = RoleId;
                    UserRole.UserId = applicationUser.Id;
                    db.IdentityUserRoles.Add(UserRole);
                    db.SaveChanges();

                }

                return RedirectToAction("Index");
            }
            ViewBag.DistrictId = new SelectList(db.Districts, "DistrictId", "Name", applicationUser.DistrictId);
            return View(applicationUser);
        }

        // GET: Admin/Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            List<IdentityUserRole> identityUserRole = db.IdentityUserRoles.Where(x => x.UserId == id).ToList();
            db.IdentityUserRoles.RemoveRange(identityUserRole);
            db.SaveChanges();

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
