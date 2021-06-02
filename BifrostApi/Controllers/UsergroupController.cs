using BifrostApi.Models;
using BifrostApi.Models.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Controllers
{
    [Route("[controller]")]
    public class UsergroupController : Controller
    {

        private readonly bifrostContext _context;

        public UsergroupController(bifrostContext context)
        {
            _context = context;
        }

        [HttpGet]
        [RequiredPermission("READ")]
        [RequiredPermission("")]
        public ActionResult Get(Guid uid)
        {
            List<UserGroup> groups = _context.UserGroups.Where(x => x.Uid == uid).ToList();

            return Ok(groups);
        }

        // GET: UsergroupController/Create
        [HttpPost]
        public ActionResult Create(string name, Guid parent)
        {
            // TODO: Create Group permission
            UserGroup group = new UserGroup
            {
                Name = name,
                Parent = parent
            };

            _context.UserGroups.Add(group);
            _context.SaveChanges();

            return Ok(group.Uid);
        }

        // POST: UsergroupController/Edit/5
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid uid, UserGroup patchedGroup)
        {
            List<UserGroup> groups = _context.UserGroups.Where(x => x.Uid == uid).ToList();
            if (groups.Count() == 0)
            {
                return BadRequest("No usergroup found");
            }

            if (groups.Count > 1)
            {
                return BadRequest("Multiple entries found");
            }
            UserGroup group = groups.FirstOrDefault();
            patchedGroup.Uid = uid;

            _context.UserGroups.Remove(group);
            _context.UserGroups.Add(patchedGroup);
            _context.SaveChanges();

            return Ok(patchedGroup);
            
        }

        // GET: UsergroupController/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid uid)
        {
            List<UserGroup> groups = _context.UserGroups.Where(x => x.Uid == uid).ToList();

            if (groups.Count() == 0)
            {
                return BadRequest("No usergroup found");
            }

            if (groups.Count > 1)
            {
                return BadRequest("Multiple entries found");
            }

            UserGroup group = groups.FirstOrDefault();

            _context.UserGroups.Remove(group);
            _context.SaveChanges();

            return Ok();
        }
    }
}
