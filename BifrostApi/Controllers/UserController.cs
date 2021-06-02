using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Models;
using BifrostApi.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using BifrostApi.Models.Attributes;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace BifrostApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {

        private readonly bifrostContext _context;

        public UserController(bifrostContext context)
        {
            _context = context;
        }

        // GET: UserController/Create
        [HttpPost]
        [RequiredPermission("UserCreateUpdateEqual")]
        [RequiredPermission("UserCreateUpdate")]
        public async Task<ActionResult> Create(string name, string email, string username, string unencryptedPassword, Guid usergroupUID)
        {
            // TODO: Enforce minimum password requirements

            // Encrypt password for storage
            string salt = Cryptography.GenerateSalt();
            string encryptedPassword = Cryptography.HashPassword(unencryptedPassword, salt);


            List<UserGroup> foundgroup = _context.UserGroups.Where(x => x.Uid == usergroupUID).ToList();

            if (foundgroup.Count > 1)
            {
                return BadRequest("Multiple usergroups found");
            }

            if (foundgroup.Count == 0)
            {
                return BadRequest("Non existent usergroup supplied");
            }

            // TODO: Should we use a default group and manually assign groups
            // or should we give the user the ability to choose their group disregarding all security best practices
            User newUser = new User
            {
                Name = name,
                Email = email,
                UserName = username,
                PasswordHash = encryptedPassword,
                PasswordSalt = salt,
                UserGroup = foundgroup.FirstOrDefault()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser.Uid);
        }

        [HttpGet]
        public async Task<ActionResult> Get(Guid userUid)
        {
            // TODO: Check if hierarchy is needed (it most likely is!)

            var foundUsers = _context.Users.AsNoTracking().Where(x => x.Uid == userUid).ToList();

            // Blank restricted data for transmitting.
            foundUsers.ForEach(e =>
            {
                e.PasswordHash = null;
                e.PasswordSalt = null;
            });



            // Hide PasswordHash names from model when transmitting, this is to obscure the data schema.
            string securedData = JsonConvert.SerializeObject(foundUsers, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            return Ok(securedData);
        }

        [HttpGet]
        public async Task<ActionResult> search(string username)
        {
            // TODO: Check if hierarchy is needed (it most likely is!)

            // SQL "LIKE" EQUIVALENT
            var foundUsers = _context.Users.AsNoTracking().Where(x => x.UserName.Contains(username)).ToList();

            if (foundUsers.Count == 0)
                return BadRequest("No users found");

            // Blank restricted data for transmitting.
            foundUsers.ForEach(e =>
            {
                e.PasswordHash = null;
                e.PasswordSalt = null;
            });

            // Hide PasswordHash names from model when transmitting, this is to obscure the data schema.
            string securedData = JsonConvert.SerializeObject(foundUsers, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return Ok(securedData);
        }

        public async Task<ActionResult> Delete(Guid userUid)
        {
            var foundUsers = _context.Users.Where(x => x.Uid == userUid).ToList();

            if (foundUsers.Count == 0)
                return BadRequest("No users found");

            if (foundUsers.Count > 1)
                return BadRequest("Multiple users found");

            var founduser = foundUsers.FirstOrDefault();

            _context.Users.Remove(founduser);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
