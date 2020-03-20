using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BookShop2020.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShop2020.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: /RoleAdmin/
        public ActionResult Index()
        {
            var roles = context.Roles.ToList();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var users = context.Users.ToList();
            var result = new List<RoleViewModel>();
            foreach (var role in roles)
            {
                var roleView = new RoleViewModel
                {
                    Name = role.Name,
                    Id = role.Id,
                    UsersInRole = new List<ApplicationUser>()
                };
                foreach (var user in users)
                {
                    if (userManager.IsInRole(user.Id, role.Name))
                    {
                        roleView.UsersInRole.Add(user);
                    }
                }
                result.Add(roleView);
            }

            return View(result);
        }
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole([Bind(Include = "Id,Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var roleManager =
                    new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var identityResult = roleManager.Create(role);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            
            return View(role);
        }

        public async Task<ActionResult> EditUsersList(string id)
        {
            var roleManager =
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var role = await roleManager.FindByIdAsync(id);
            var users = context.Users.Include(x => x.Roles).ToList();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            IEnumerable<ApplicationUser> members, nonMembers;
            if (role == null)
            {
                members = new List<ApplicationUser>();
                nonMembers = users;
            }
            else
            {
                members = (from x in users where userManager.IsInRole(x.Id, role.Name) select x).ToList();
                nonMembers = users.Except(members);
            }
            return View(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<ActionResult> EditUsersList(RoleModificationModel model)
        {
            IdentityResult result;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await userManager.AddToRoleAsync(userId, model.RoleName);

                    if (!result.Succeeded)
                    {
                        return View("Error");
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await userManager.RemoveFromRoleAsync(userId,
                        model.RoleName);

                    if (!result.Succeeded)
                    {
                        return View("Error");
                    }
                }
                return RedirectToAction("Index");

            }
            return View("Error");
        }
    }
}