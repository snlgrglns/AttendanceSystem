using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.Models
{
    public class Employee
    {
        private readonly int id, sn;
        private readonly string name, address, status;
        private readonly DateTime dob;

        public Employee(int id, int sn, string name, DateTime dob, string address, string status)
        {
            this.id = id;
            this.sn = sn;
            this.name = name;
            this.dob = dob;
            this.address = address;
            this.status = status;
        }
        public int Id { get { return id; } }
        public int Sn { get { return sn; } }
        public string Name { get { return name; } }
        public DateTime Dob { get { return dob; } }
        public string Address { get { return address; } }
        public string Status { get { return status; } }
    }
}
