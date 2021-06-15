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
using BifrostApi.Models.DTO;

namespace BifrostApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {

        private readonly bifrostContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(bifrostContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserController/Create
        [HttpPost]
        [RequiredPermission("UserCreateUpdateEqual")]
        [RequiredPermission("UserCreateUpdate")]
        [RequireHierarchy("userGroupUid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public async Task<ActionResult> Create(Guid userGroupUid, UserCreateDTO user)
        {
            PasswordValidator<User> validator = new Microsoft.AspNetCore.Identity.PasswordValidator<User>();

            IdentityResult result = await validator.ValidateAsync(_userManager, null, user.unencryptedPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Encrypt password for storage
            string salt = Cryptography.GenerateSalt();
            string encryptedPassword = Cryptography.HashPassword(user.unencryptedPassword, salt);


            List<UserGroup> foundgroup = _context.UserGroups.Where(x => x.Uid == userGroupUid).ToList();

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
                Name = user.name,
                Email = user.email,
                UserName = user.username,
                PasswordHash = encryptedPassword,
                PasswordSalt = salt,
                UserGroup = foundgroup.FirstOrDefault()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser.Id);
        }

        [HttpGet]
        [RequireHierarchy("userUid", false, RequireHierarchyAttribute.HierarchySearchType.User)]
        [QueryRouteSelector("username", false)]
        [QueryRouteSelector("userUid", true)]
        public async Task<ActionResult> Get(Guid userUid)
        {
            var foundUsers = _context.Users.AsNoTracking().Where(x => x.Id == userUid).ToList();

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


        // TODO: make string searches for username in hierarchychecks
        [HttpGet]
        [QueryRouteSelector("username", true)]
        [QueryRouteSelector("userUid", false)]
        [ApiExplorerSettings(IgnoreApi = true)] // Hide search from swagger to avoid ambiguous  
        public async Task<ActionResult> Search(string username)
        {

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

        [HttpDelete]
        [RequireHierarchy("userUid", false, RequireHierarchyAttribute.HierarchySearchType.User)]
        [RequiredPermission("UserHardDelete")]
        [Route("Hard")]
        public async Task<ActionResult> HardDelete(Guid userUid)
        {
            var foundUsers = _context.Users.Where(x => x.Id == userUid).ToList();

            if (foundUsers.Count == 0)
                return BadRequest("No users found");

            if (foundUsers.Count > 1)
                return BadRequest("Multiple users found");

            var founduser = foundUsers.FirstOrDefault();

            _context.Users.Remove(founduser);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [RequireHierarchy("userUid", false, RequireHierarchyAttribute.HierarchySearchType.User)]
        [RequiredPermission("UserSoftDelete")]
        [Route("Soft")]
        public async Task<ActionResult> SoftDelete(Guid userUid)
        {
            var foundUsers = _context.Users.Where(x => x.Id == userUid).ToList();

            if (foundUsers.Count == 0)
                return BadRequest("No users found");

            if (foundUsers.Count > 1)
                return BadRequest("Multiple users found");

            var founduser = foundUsers.FirstOrDefault();
            founduser.Deleted = true;
            
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
