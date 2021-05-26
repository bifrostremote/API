using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using BifrostApi.Session;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // TODO: Remove INSECURE
        [HttpGet]
        [Route("Encrypt")]
        public IActionResult Encrypt(string plaintext, string password)
        {
            return Ok(Cryptography.AESEncrypt(plaintext, password));
        }

        // TODO: Remove INSECURE
        [HttpGet]
        [Route("Decrypt")]
        public IActionResult Decrypt(string ciphertext, string password)
        {
            return Ok(Cryptography.AESDecrypt(ciphertext, password));
        }

        [HttpGet]
        [Route("Test")]
        public IActionResult Test()
        {
            var currentSession = SessionHelper.GetCurrentSession(HttpContext.Session);

            return Ok(currentSession.isAuthenticated);
        }

        [HttpGet]
        [Route("Test2")]
        public IActionResult Test2()
        {
            var currentSession = SessionHelper.GetCurrentSession(HttpContext.Session);
            currentSession.isAuthenticated = true;
            SessionHelper.SaveSession(currentSession, HttpContext.Session);

            return Ok();
        }

        [HttpPost]
        [Route("Authenticate")]
        public IActionResult Authenticate(string username, string password)
        {
            var users = _context.Users.Where(x => x.Username == username).ToList();

            //if (users.FirstOrDefault() == null)
            //    return BadRequest("No user found");

            //    if (users.Count > 1)
            //    return BadRequest("Multiple users found when authenticating");

            var user = users.FirstOrDefault();

            bool isAuthenticated = Cryptography.IsAuthenticated(password, user.Password, user.Passwordsalt);
           
            if (isAuthenticated)
            {
                var currentSession = SessionHelper.GetCurrentSession(HttpContext.Session);
                currentSession.isAuthenticated = true;
                currentSession.CurrentUser = user;
                
                // Cache permissions in session to reduce 
                var permissiongrouping = _context.GroupPermissions.Where(x => x.Group == user.UserGroup.Uid).FirstOrDefault();

                if (permissiongrouping != null)
                {
                    //permissiongrouping.
                }

                //currentSession.permissions = permissions;

                SessionHelper.SaveSession(currentSession, HttpContext.Session);


                return Ok();
            }

            return Forbid("user not authenticated");
        }
    }
}
