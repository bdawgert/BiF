﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BiF.DAL.Models;
using BiF.Web.Identity;
using BiF.Web.ViewModels.Administration;

namespace BiF.Web.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdministrationController : BaseController
    {
        // GET: Administration
        public ActionResult Index() {

            var users = DAL.Context.Users.Select(x => new {User = x, Profile = x.Profile, Claims = x.Claims, Roles = x.Roles.Select(s => s.Name)}).ToList();

            List<UserInformation> list = users.Select(x => new UserInformation {
                Id = x.User.Id,
                Email = x.User.Email,
                Username = x.User.UserName ?? x.Profile?.RedditUsername ?? x.User.Email,
                HasProfile = x.Profile != null,
                Roles = x.Roles.ToArray(),
                UserStatus = (int?)x.User.UserStatus ?? 0,
                AllowedExclusions = x.Claims.FirstOrDefault(c => c.Type == "http://21brews.com/identity/claims/allowed-exclusions")?.Value
            }).Where(x => x.UserStatus > -10)
            .OrderByDescending(x => x.Roles.Any(r => r == "ADMIN"))
            .ThenBy(x => x.UserStatus < (int)IdentityUser.UserStatuses.None )
            .ThenBy(x => x.UserStatus == (int)IdentityUser.UserStatuses.Approved)
            .ToList();

            IndexVM vm = new IndexVM {
                Users = list
            };

            return View(vm);
        }


        public JsonResult UpdateRoles(string id, string role, string action) {
            role = role?.ToUpper();
            if (action == "Add" && !DAL.Context.Roles.Any(x => x.UserId == id && x.Name == role))
                DAL.Context.Roles.Add(new IdentityRole {UserId = id, Name = role});
            else if (action == "Remove") {
                IQueryable<IdentityRole> roles = DAL.Context.Roles.Where(x => x.UserId == id && x.Name == role);
                if (roles.Any())
                    DAL.Context.Roles.RemoveRange(roles);
            }
            DAL.Context.SaveChanges();

            return Json(new {Success = true});
        }


        public JsonResult UpdateApproval(string id, int status) {
            IdentityUser user = DAL.Context.Users.Find(id);
            if (user == null)
                return Json(new { Success = false });

            user.UserStatus = (IdentityUser.UserStatuses) status;

            DAL.Context.SaveChanges();

            //createMatchRecord(id, status);

            return Json(new { Success = true });
        }

        public JsonResult DeleteUser(string id)
        {
            IdentityUser user = DAL.Context.Users.Find(id);
            if (user == null)
                return Json(new { Success = false });

            user.UserStatus = IdentityUser.UserStatuses.Deleted;
            user.Email = $"!{user.Email}";

            DAL.Context.SaveChanges();

            return Json(new { Success = true });
        }

        public JsonResult UpdateAllowedExclusionCount(string id, int count) {
            string claimType = "http://21brews.com/identity/claims/allowed-exclusions";

            IdentityClaim claim = DAL.Context.Claims.FirstOrDefault(x => x.UserId == id && x.Type == claimType);
            if (claim == null) {
                claim = new IdentityClaim {UserId = id, Type = claimType, Value = "2"};
                DAL.Context.Claims.Add(claim);
            }

            claim.Value = count.ToString();
            DAL.Context.SaveChanges();

             return Json(new { Success = true });

        }


    }


}