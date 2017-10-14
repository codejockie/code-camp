using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyCodeCamp.Data.Entities;

namespace MyCodeCamp.Data
{
  public class CampIdentityInitializer
  {
    private RoleManager<IdentityRole> _roleMgr;
    private UserManager<CampUser> _userMgr;

    public CampIdentityInitializer(UserManager<CampUser> userMgr, RoleManager<IdentityRole> roleMgr)
    {
      _userMgr = userMgr;
      _roleMgr = roleMgr;
    }

    public async Task Seed()
    {
      var user = await _userMgr.FindByNameAsync("codejockie");

      // Add User
      if (user == null)
      {
        if (!(await _roleMgr.RoleExistsAsync("Admin")))
        {
          var role = new IdentityRole("Admin");
          //role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsAdmin", ClaimValue = "True" });
          // await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, "projects.view"));
          await _roleMgr.CreateAsync(role);
          await _roleMgr.AddClaimAsync(role, new Claim(ClaimTypes.Role, "Admin"));
        }

        user = new CampUser()
        {
          UserName = "codejockie",
          FirstName = "Kennedy",
          LastName = "John",
          Email = "devjckennedy@gmail.com"
        };

        var userResult = await _userMgr.CreateAsync(user, "P@ssw0rd!");
        var roleResult = await _userMgr.AddToRoleAsync(user, "Admin");
        var claimResult = await _userMgr.AddClaimAsync(user, new Claim("SuperUser", "True"));

        if (!userResult.Succeeded || !roleResult.Succeeded || !claimResult.Succeeded)
        {
          throw new InvalidOperationException("Failed to build user and roles");
        }

      }
    }
  }
}
