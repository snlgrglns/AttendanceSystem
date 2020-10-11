using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.Models
{
    public class EmpListForCreateCard
    {
        private int id;
        private string full_name;

        public EmpListForCreateCard(int id, string name)
        {
            this.id = id;
            this.full_name = name;
        }

        public EmpListForCreateCard()
        {
        }

        public int ID { get { return id;  }set { id = value;  } }
        public string Full_Name { get { return full_name; } set { full_name = value; } }
    }

    public static class EmpSetWhenChange
    {
        public static int Emp_Id { get; set; }
        public static string Emp_Name { get; set; }
    }
}
