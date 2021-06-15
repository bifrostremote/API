using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using BifrostApi.Models.Attributes;
using BifrostApi.Models.DTO;
using BifrostApi.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BifrostApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MachineController : Controller
    {
        private readonly bifrostContext _context;
          public MachineController(bifrostContext context)
        {
            _context = context;
        }

        [HttpGet]
        [QueryRouteSelector("userUid", false)]
        [QueryRouteSelector("machineUid", true)]
        public ActionResult GetMachine(Guid machineUid){
            return Ok();
        }

        [HttpPost]
        public ActionResult InsertMachine([FromBody]MachineCreateDTO machine)
        {

            if (machine.Name == "")
                return BadRequest("name cannot be empty");

            if (machine.UserUid == Guid.Empty)
                return BadRequest("user cannot be empty");

            int foundUsers = _context.Users.Where(x => x.Id == machine.UserUid).Count();

            if (foundUsers > 1)
                return BadRequest("multiple users found");

            if (foundUsers == 0)
                return BadRequest("User does not exist");

            IPAddress address = null;
            IPAddress.TryParse(machine.IPAddress, out address);

            if (address == null)
                return BadRequest("IP Address is not valid");


            Machine inserted = new Machine
            {
                Name = machine.Name,
                UserUid = machine.UserUid,
                Deleted = false,
                Ip = address.ToString(),
                LastOnline = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
                
            };

            _context.Machines.Add(inserted);
            _context.SaveChangesAsync();

            return Ok(inserted.Uid);

        }

        [HttpGet]
        [Route("HierarchyLookup")]
        public async Task<ActionResult> GetAllMachinesInHierarchy()
        {
            var session = SessionHelper.GetCurrentSession(HttpContext.Session);

            var machines = _context.GetHierarchyForUser(session.CurrentUser.UserGroupUid);

            return Ok(machines.ToList());
        }

        [HttpGet]
        [RequireHierarchy("userUid", false, RequireHierarchyAttribute.HierarchySearchType.User)]
        [QueryRouteSelector("userUid", true)]
        [QueryRouteSelector("machineUid", false)]
        public async Task<ActionResult> GetMachinesForUser(Guid userUid)
        {
            List<Machine> machines = _context.Machines.Where(x => x.UserUid == userUid).ToList();

            return Ok(machines);
        }


        [HttpPatch]
        public ActionResult UpdateMachine([FromBody]Machine machineOut){
            Guid machineUid = machineOut.Uid;
            Machine machine = _context.Machines.Where(x => x.Uid == machineUid).FirstOrDefault();

            if(machine.Uid == null){
                return BadRequest("Error");
            }

            if(machine.Deleted == false){
                return BadRequest("Error");
            }

            machine = machineOut;
            _context.SaveChanges();

            machine = _context.Machines.Where(x => x.Uid == machineUid).FirstOrDefault();
            if(machine != machineOut){
                return BadRequest("Error");
            }
            return Ok("Success");
        }

        [HttpDelete]
        [Route("Soft")]
        public ActionResult SoftDeleteMachine(Guid machineUid){
            Machine machine = _context.Machines.Where(x => x.Uid == machineUid).FirstOrDefault();

            if(machine.Uid == null){
                return Ok("Error");
            }

            if(machine.Deleted == false){
                return Ok("Error");
            }

            machine.Deleted = false;
            _context.SaveChanges();

            machine = _context.Machines.Where(x => x.Uid == machineUid).FirstOrDefault();
            if(machine.Deleted){
                return Ok("Error");
            }
            return Ok("Success");
        }

        [HttpDelete]
        [Route("Hard")]
        public ActionResult HardDeleteMachine(Guid machineUid){
            Machine machine = _context.Machines.Where(x => x.Uid == machineUid).FirstOrDefault();

            if(machine == null){
                return Ok("Error");
            }

            if(machine.Deleted == false){
                return Ok("Error");
            }

            _context.MachineTokens.RemoveRange(_context.MachineTokens.Where(x => x.MachineUid == machineUid));
            _context.Machines.Remove(machine);
            _context.SaveChanges();

            machine = _context.Machines.Where(x => x.Uid == machineUid).FirstOrDefault();
            if(machine.Uid != null){
                return Ok("Error");
            }
            return Ok("Success");
        }

        [HttpPost]
        [Route("SetLastOnline")]
        public ActionResult SetLastOnline(){
            return Ok();
        }

        [HttpGet]
        [Route("ConnectToMachine")]
        public ActionResult ConnectToMachine(Guid machineUid){
            return Ok();
        }
    }
}
