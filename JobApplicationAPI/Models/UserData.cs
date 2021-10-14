using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobApplicationAPI.Models
{
    public class UserData
    {
        public List<Experiences> Experiences { get; set; }

        public Expectations Expectations { get; set; }

        public PersonalInfo PersonalInfo { get; set; }

        public int UserId { get; set; }
    }
}
