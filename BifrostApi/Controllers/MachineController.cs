using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using BifrostApi.Session;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMachine(Guid machineUid){
            return Ok();
        }

        [HttpPost]
        [Route("InsertMachine")]
        public ActionResult InsertMachine([FromBody]Machine machineOut){
            return Ok();
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
