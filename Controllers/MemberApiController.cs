using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practise.Helper;
using Practise.Models;
using System.Collections.Generic;

namespace Practise.Controllers
{
    [Route("~/api/[controller]")]
    [ApiController]
    public class MemberApiController : ControllerBase
    {
        private readonly DataBaseHelper data;
        public MemberApiController(DataBaseHelper data)
        {
            this.data = data;
        }
        [HttpGet("List")]
        public List<Member> List()
        {
            return data.GetMembers();
        }
        [HttpGet("Get")]
        public Member Get(int Id)
        {
            return data.GetMember(Id);
        }
        [HttpPost("Create")]
        public string Create([FromBody]Member member)
        {            
            return data.DataCreate(member);
        }
        [HttpPost("Update")]
        public string Update([FromBody] Member member)
        {
            return data.Update(member);
        }

    }
}
