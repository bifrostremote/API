using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using BifrostApi.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Controllers
{
    public class Tokenpar
    {
        public int SecurityLevel { get; set; }
        public Guid MachineUid { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class TokenController : Controller
    {
        private string GetTokenBlock()
        {
            var random = new Random();
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static readonly string[] words = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private string GetTokenWord()
        {
            var random = new Random();
            return words[random.Next(words.Length)];
        }

        // POST: Token/GenerateToken
        [HttpPost]
        [Route("GenerateToken")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateToken([FromBody]Tokenpar tokenpar)
        {
            string[] token = new string[tokenpar.SecurityLevel];
            for (int i = 0; i < tokenpar.SecurityLevel; i++)
            {
                token[i] = GetTokenBlock();
            }
            return Ok(string.Join('-', token));
        }

        // POST: Token/GenerateWordToken
        [HttpPost]
        [Route("GenerateWordToken")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateWordToken([FromBody]Tokenpar tokenpar){
            string[] tokenArr = new string[tokenpar.SecurityLevel];
            for (int i = 0; i < tokenpar.SecurityLevel; i++)
            {
                tokenArr[i] = GetTokenWord();
            }
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            MachineToken Token = new MachineToken
            {
                Token = string.Join('-', tokenArr),
                CreateDate = secondsSinceEpoch,
                MachineUid = tokenpar.MachineUid
            };

            return Ok();
        }

        // POST: Token/RenewToken
        [HttpPost]
        [Route("RenewToken")]
        [ValidateAntiForgeryToken]
        public ActionResult RenewToken(){
            return Ok();
        }

        // Delete: Token
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteToken(Guid tokenUid){
            return Ok();
        }
    }
}
