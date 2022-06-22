using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Practise.Models;
using Practise.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Practise.Extension;
using Practise.Helper;

namespace Practise.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly List<Member> Members = new List<Member>() {
            new Member
            {
                Id = 001,
                Name = "Max",
                Sex ="男",
                Email = "Max@gmail.com",
                CellPhone = "0929123654",
                Phone = "07345678",
                Status = "儲存",
                Skills =new List<string> {"C#","AJAX","MS SQL" },
                ReportTo="Mark",
                Area = new List<string>{"台北" },
                CreateTime = "2022/06/16 08:00",
                Editor=null,
                EditTime=null,
            },
            new Member
            {
                Id = 002,
                Name = "Jay",
                Sex ="男",
                Email = "Jay@gmail.com",
                CellPhone = "0929123654",
                Phone = "07345678",
                Status = "儲存",
                Skills =new List<string> {"C#" ,"AJAX"},
                ReportTo="Mark",
                Area = new List<string>{"新竹","台中" },
                CreateTime = "2022/06/16 08:00",
                Editor="Mark",
                EditTime="2022/06/16 09:12",
            },
            new Member
            {
                Id = 003,
                Name = "Mary",
                Sex ="女",
                Email = "Mary@gmail.com",
                CellPhone = "0929123654",
                Phone = "07345678",
                Status = "追蹤",
                Skills =new  List<string> {"Word" , "Excel" ,"Power Point"},
                Area = new List<string>{ "台南","新竹"},
                ReportTo="Mark",
                CreateTime = "2022/06/16 08:00",
                Editor="Mark",
                EditTime="2022/06/16 10:12",
            }
        };
        private readonly List<string> Skill = new List<string> { "C#", "Java","MS SQL", "AJAX", "Word", "Excel" };
        private readonly List<string> Area = new List<string> { "台南","新竹","台中","台北" };
        private readonly List<string> Sexs = new List<string> { "男", "女" }; 
        private readonly List<string> Status = new List<string> { "儲存", "追蹤" };
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(SearchModel request)
        {
            _logger.LogTrace("Loggin Level = 0 (Trace)");
            _logger.LogDebug("Loggin Level = 1 (Debug)");
            _logger.LogInformation("Loggin Level = 2 (Information)");
            _logger.LogWarning("Loggin Level = 3 (Warning)");
            _logger.LogError("Loggin Level = 4 (Error)");
            _logger.LogCritical("Loggin Level = 5 (Critical)");
  
            List<MemberListViewModel> memberList = this.Members.Select(a=>new MemberListViewModel { 
              Id = a.Id,
              Name = a.Name,
              Email = a.Email,
              Skills = SkillString(a.Skills),
              ReportTo = a.ReportTo,
              CreateTime = a.CreateTime,
              Editor = a.Editor,
              EditTime = a.EditTime
            }).ToList();
            return View(memberList);
        }
        public IActionResult Create()
        {
            ViewBag.Data = new DataList
            {
                Area = this.Area,
                Sexs = this.Sexs,
                Skills = this.Skill,
                Status = this.Status
            };
            return View();
        }
        public IActionResult Edit(string Name)
        {
            Member member = Members.First(a => a.Name == Name);
            ViewBag.Data = new DataList
            {
                Area =this.Area,
                Sexs =this.Sexs,
                Skills=this.Skill,
                Status = this.Status
            };

            return View(member);
        }
        [HttpPost]
        public IActionResult EditAction(Member m)
        {

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Tuple<bool,string> DeleteAction(long Id)
        {
            bool flag = true;
            string msg = "刪除成功";
            //Member member = members.First(a => a.Id == m.Id);
            return new Tuple<bool, string> (flag, msg); 
        }
        public IActionResult Privacy()
        {
            return View();
        }

        private string SkillString(List<string> skills)
        {
            string result = " ";
            foreach(var skill in skills)
            {
                result+=skill+",";
            }
            return result.Remove(result.Length-1,1);
        }


    }
}
