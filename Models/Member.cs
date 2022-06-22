using System;
using System.Collections.Generic;

namespace Practise.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
        public string CellPhone { get; set; }
        public string Phone { get; set; }
        public List<string> Area { get; set; }
        public List<string> Skills { get; set; }
        public string Status { get; set; }
        public string ReportTo { get; set; }
        public string CreateTime { get; set; }
    
        public string Editor { get; set; }
        public string EditTime { get; set; }
    }
}
