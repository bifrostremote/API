using BifrostApi.BusinessLogic;
using BifrostApi.Models;
using BifrostApi.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Models.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BifrostApi.Controllers
{
    

    [ApiController]
    [Route("[controller]")]
    public class TokenController : Controller
    {

        private readonly bifrostContext _context;
        private readonly IConfiguration _configuration;
        public TokenController(bifrostContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

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
        [Route("String")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateToken([FromBody]TokenPairDTO tokenPair)
        {
            string[] token = new string[tokenPair.SecurityLevel];
            for (int i = 0; i < tokenPair.SecurityLevel; i++)
            {
                token[i] = GetTokenBlock();
            }

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            MachineToken Token = new MachineToken
            {
                Token = string.Join('-', token),
                CreateDate = secondsSinceEpoch,
                MachineUid = tokenPair.MachineUid
            };

            _context.MachineTokens.Add(Token);
            await _context.SaveChangesAsync();

            return Ok(Token.Token);
        }

        // POST: Token/Word
        [HttpPost]
        [Route("Word")]
        public async Task<ActionResult> GenerateWordToken([FromBody]TokenPairDTO tokenPair)
        {
            string[] tokenArr = new string[tokenPair.SecurityLevel];
            for (int i = 0; i < tokenPair.SecurityLevel; i++)
            {
                tokenArr[i] = GetTokenWord();
            }

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            MachineToken Token = new MachineToken
            {
                Token = string.Join('-', tokenArr),
                CreateDate = secondsSinceEpoch,
                MachineUid = tokenPair.MachineUid
            };

            _context.MachineTokens.Add(Token);
            await _context.SaveChangesAsync();

            return Ok(Token.Token);
        }

        // POST: Token/RenewToken
        [HttpPost]
        [Route("RenewToken")]
        [ValidateAntiForgeryToken]
        public ActionResult RenewToken(){
            return Ok();
        }

        [HttpGet]
        public ActionResult GetMachineIPFromToken(string token)
        {
            int tokenTimeout = Convert.ToInt32(_configuration.GetSection("TokenSettings").GetSection("TimeoutInMinutes").Value);

            // get tokens that have not expired where the correct 
            var tokens = _context.MachineTokens.Include(g => g.Machine).Where(x => x.Token == token).ToList();

            var validTokens = tokens.Where(x => DateTime.Compare(DateTimeOffset.FromUnixTimeSeconds(x.CreateDate).DateTime, DateTime.Now.AddMinutes(tokenTimeout)) < 0).ToList();

            if (validTokens.Count == 0)
                return NotFound("No Token Found");

            // Check for duplicate MachineUids in found tokens
            int numberOfDuplicateTokens = validTokens.GroupBy(x => x.MachineUid)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key).Count();

            if (numberOfDuplicateTokens > 0)
                return BadRequest("Multiple tokens found on multiple machines");

            Machine machine = validTokens.FirstOrDefault().Machine;

            return Ok(machine.Ip);
        }

        // Delete: Token
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteToken(Guid tokenUid){
            return Ok();
        }
    }
}
