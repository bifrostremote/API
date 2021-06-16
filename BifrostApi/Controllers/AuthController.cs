using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using BifrostApi.Models.Attributes;
using BifrostApi.Session;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BifrostApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly bifrostContext _context;

        public AuthController(bifrostContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Authenticate")]
        [IgnoreAntiforgeryToken]
        public IActionResult Authenticate(string username, string password)
        {
            var users = _context.Users.AsNoTracking()
                .Include(e => e.UserGroup)
                //.Include(e => e.PasswordKey)
                .Where(x => x.UserName == username).ToList();

            if (users.FirstOrDefault() == null)
                return BadRequest("No user found");

            if (users.Count > 1)
                return BadRequest("Multiple users found when authenticating");

            var user = users.FirstOrDefault();

            if (user.Deleted)
                return Unauthorized();

            bool isAuthenticated = Cryptography.IsAuthenticated(password, user.PasswordHash, user.PasswordSalt);
           
            if (isAuthenticated)
            {
                var currentSession = SessionHelper.GetCurrentSession(HttpContext.Session);
                currentSession.isAuthenticated = true;
                currentSession.CurrentUser = user;
                currentSession.permissions = new List<PermissionProperty>();    
                
                // Cache permissions in session to reduce 
                var permissiongrouping = _context.GroupPermissions.Include(e => e.PermissionPropertyNavigation).Where(x => x.GroupUid == user.UserGroup.Uid).ToList();


                if (permissiongrouping != null && permissiongrouping.Count() != 0)
                {
                    foreach (var permission in permissiongrouping)
                        currentSession.permissions.Add(permission.PermissionPropertyNavigation);
                }

                SessionHelper.SaveSession(currentSession, HttpContext.Session);

                return Ok(user.Id);
            }

            return Forbid("user not authenticated");
        }
    }
}
