using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.Models
{
    public class Attendence
    {
        private readonly int sn;
        private readonly string name;
        private readonly DateTime attime;

        public Attendence(int sn, string name, DateTime attime)
        {
            this.sn = sn;
            this.name = name;
            this.attime = attime;
        }
        public int Sn { get { return sn; } }
        public string Name { get { return name; } }
        public DateTime AtTime { get { return attime; } }
    }
}
