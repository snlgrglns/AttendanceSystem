using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.Models
{
    public static class EmployeeAccess
    {
        /// <summary>
        /// A list of employees. This should be replaced by a database.
        /// </summary>
        private static ObservableCollection<Employee> employees = EmployeeData();

        public static ObservableCollection<Employee> EmployeeData()
        {
            SelectFrmDb sfd = new SelectFrmDb();
            DataTable dt = sfd.EmpRecords();
            employees = new ObservableCollection<Employee>();
            int sn = 1;
            foreach (DataRow row in dt.Rows)
            {
                string full_name = row["first_name"].ToString().Trim() + " " + row["last_name"].ToString().Trim();
                string status = "";
                if ((int)row["card_status"] == 0) status = "no card";
                else if ((int)row["card_status"] == 1) status = "has card";
                else if ((int)row["card_status"] == 2) status = "card blocked";
                else if ((int)row["card_status"] == 3) status = "card expired";
                else status = "";
                employees.Add(new Employee((int)row["id"], sn, full_name, (DateTime)row["date_of_birth"], (string)row["address"], status));
                sn++;
            }
            return employees;
        }

        /// <summary>
        /// Gets the employees.
        /// </summary>
        /// <param name="start">Zero-based index that determines the start of the employees to be returned.</param>
        /// <param name="itemCount">Number of employees that is requested to be returned.</param>
        /// <param name="sortColumn">Name of column or member that is the basis for sorting.</param>
        /// <param name="ascending">Indicates the sort direction to be used.</param>
        /// <param name="totalItems">Total number of employees.</param>
        /// <returns>List of employees.</returns>
        public static ObservableCollection<Employee> GetEmployees(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = employees.Count;

            ObservableCollection<Employee> sortedEmployees = new ObservableCollection<Employee>();

            // Sort the employees. In reality, the items should be stored in a database and
            // use SQL statements for sorting and querying items.
            switch (sortColumn)
            {
                case ("Sn"):
                    sortedEmployees = new ObservableCollection<Employee>
                    (
                        from p in employees
                        orderby p.Sn
                        select p
                    );
                    break;
                case ("Name"):
                    sortedEmployees = new ObservableCollection<Employee>
                    (
                        from p in employees
                        orderby p.Name
                        select p
                    );
                    break;
                case ("Dob"):
                    sortedEmployees = new ObservableCollection<Employee>
                    (
                        from p in employees
                        orderby p.Dob
                        select p
                    );
                    break;
                case ("Address"):
                    sortedEmployees = new ObservableCollection<Employee>
                    (
                        from p in employees
                        orderby p.Address
                        select p
                    );
                    break;
                case ("Status"):
                    sortedEmployees = new ObservableCollection<Employee>
                    (
                        from p in employees
                        orderby p.Status
                        select p
                    );
                    break;
            }

            sortedEmployees = ascending ? sortedEmployees : new ObservableCollection<Employee>(sortedEmployees.Reverse());

            ObservableCollection<Employee> filteredEmployees = new ObservableCollection<Employee>();

            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredEmployees.Add(sortedEmployees[i]);
            }

            return filteredEmployees;
        }
    }
}
