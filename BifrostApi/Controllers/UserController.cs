using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Models;
using BifrostApi.BusinessLogic;
using Microsoft.AspNetCore.Identity;

namespace BifrostApi.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {

        private readonly bifrostContext _context;

        public UserController(bifrostContext context)
        {
            _context = context;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Create
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> Create(string name, string email, string username, string unencryptedPassword, Guid usergroupUID)
        {
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
                Username = username,
                Password = encryptedPassword,
                Passwordsalt = salt,
                UserGroup = foundgroup.FirstOrDefault()
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(newUser.Uid);
        }

        // GET: UserController/Edit/5'
        [HttpPatch]
        public ActionResult Edit()
        {

            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
