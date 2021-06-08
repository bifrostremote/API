using BifrostApi.Models;
using BifrostApi.Models.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Session;
using Microsoft.EntityFrameworkCore;

namespace BifrostApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsergroupController : Controller
    {

        private readonly bifrostContext _context;

        public UsergroupController(bifrostContext context)
        {
            _context = context;
        }

        [HttpGet]
        [RequireHierarchy("uid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public ActionResult Get(Guid uid, bool includeUsers = false)
        {

            List<UserGroup> groups;

            if (includeUsers)
                groups = _context.UserGroups.Include(e => e.Users).Where(x => x.Uid == uid).ToList();
            else
                groups = _context.UserGroups.Where(x => x.Uid == uid).ToList();

            return Ok(groups);
        }

        //public ActionResult GetFromHierarchy(Guid uid)
        //{
        //    Session.Session session = SessionHelper.GetCurrentSession(_httpContext.Session);
        //    UserGroup currentUserGroup = session.CurrentUser.UserGroup;
        //    List<UserGroup> groups = _context.UserGroups.
        //}

        // GET: UsergroupController/Create
        [HttpPost]
        [RequireHierarchy("parentUid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
        public ActionResult Create(string name, Guid parentUid)
        {
            // TODO: Create Group permission
            UserGroup group = new UserGroup
            {
                Name = name,
                ParentUid = parentUid
            };

            _context.UserGroups.Add(group);
            _context.SaveChanges();

            return Ok(group.Uid);
        }

        // POST: UsergroupController/Edit/5
        [HttpPatch]
        [ValidateAntiForgeryToken]
        [RequireHierarchy("uid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
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
        [Route("Hard")]
        [RequireHierarchy("uid", false, RequireHierarchyAttribute.HierarchySearchType.Usergroup)]
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
