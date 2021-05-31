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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GetMachine")]
        public ActionResult GetMachine(){
            return Ok();
        }

        [HttpPost]
        [Route("InsertMachine")]
        public ActionResult InsertMachine(){
            return Ok();
        }

        [HttpPost]
        [Route("UpdateMachine")]
        public ActionResult UpdateMachine(){
            return Ok();
        }

        [HttpPost]
        [Route("SoftDeleteMachine")]
        public ActionResult SoftDeleteMachine(){
            return Ok();
        }

        [HttpPost]
        [Route("HardDeleteMachine")]
        public ActionResult HardDeleteMachine(string machineId){
            
            if(){

            } else {

            }
            return Ok();
        }

        [HttpPost]
        [Route("SetLastOnline")]
        public ActionResult SetLastOnline(){
            return Ok();
        }

        [HttpGet]
        [Route("ConnectToMachine")]
        public ActionResult ConnectToMachine(){
            return Ok();
        }
    }
}
