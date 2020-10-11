using AttendenceSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.ViewModel
{
    public class CreateCardViewModel : INotifyPropertyChanged
    {
        private EmpListForCreateCard _SelectedEmp;
        public EmpListForCreateCard SelectedEmp
        {
            get {
                return _SelectedEmp; }
            set
            {
                _SelectedEmp = value;
                EmpSetWhenChange.Emp_Id = _SelectedEmp.ID;
                EmpSetWhenChange.Emp_Name = _SelectedEmp.Full_Name;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedEmp"));
            }
        }
        private IEnumerable<EmpListForCreateCard> _AvailableEmps = GetAvailableEmps();

        private static IEnumerable<EmpListForCreateCard> GetAvailableEmps()
        {
            SelectFrmDb sfd = new SelectFrmDb();
            DataTable dt = sfd.FindEmpWithNoCard();
            List<EmpListForCreateCard> emp_list = new List<EmpListForCreateCard>();
            foreach (DataRow row in dt.Rows)
            {
                string full_name = row["first_name"].ToString().Trim() + " " + row["last_name"].ToString().Trim();
                emp_list.Add(new EmpListForCreateCard((int)row["id"], full_name));
            }
            return emp_list;
        }

        public IEnumerable<EmpListForCreateCard> AvailableEmps
        {
            get { return _AvailableEmps; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
