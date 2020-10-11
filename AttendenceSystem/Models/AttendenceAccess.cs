using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.Models
{
    public static class AttendenceAccess
    {
        /// <summary>
        /// A list of attendences. This should be replaced by a database.
        /// </summary>
        private static ObservableCollection<Attendence> attendences = AttendenceData();

        public static ObservableCollection<Attendence> AttendenceData()
        {
            SelectFrmDb sfd = new SelectFrmDb();
            DataTable dt = sfd.AttendenceRecords();
            attendences = new ObservableCollection<Attendence>();
            int sn = 1;
            foreach (DataRow row in dt.Rows)
            {
                string full_name = row["first_name"].ToString().Trim() + " " + row["last_name"].ToString().Trim();
                attendences.Add(new Attendence(sn, full_name, (DateTime)row["attnd_date"]));
                sn++;
            }
            return attendences;
        }

        /// <summary>
        /// Gets the attendences.
        /// </summary>
        /// <param name="start">Zero-based index that determines the start of the attendences to be returned.</param>
        /// <param name="itemCount">Number of attendences that is requested to be returned.</param>
        /// <param name="sortColumn">Name of column or member that is the basis for sorting.</param>
        /// <param name="ascending">Indicates the sort direction to be used.</param>
        /// <param name="totalItems">Total number of attendences.</param>
        /// <returns>List of attendences.</returns>
        public static ObservableCollection<Attendence> GetAttendences(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = attendences.Count;

            ObservableCollection<Attendence> sortedAttendences = new ObservableCollection<Attendence>();

            // Sort the attendences. In reality, the items should be stored in a database and
            // use SQL statements for sorting and querying items.
            switch (sortColumn)
            {
                case ("Sn"):
                    sortedAttendences = new ObservableCollection<Attendence>
                    (
                        from p in attendences
                        orderby p.Sn
                        select p
                    );
                    break;
                case ("Name"):
                    sortedAttendences = new ObservableCollection<Attendence>
                    (
                        from p in attendences
                        orderby p.Name
                        select p
                    );
                    break;
                case ("AtTime"):
                    sortedAttendences = new ObservableCollection<Attendence>
                    (
                        from p in attendences
                        orderby p.AtTime
                        select p
                    );
                    break;
            }

            sortedAttendences = ascending ? sortedAttendences : new ObservableCollection<Attendence>(sortedAttendences.Reverse());

            ObservableCollection<Attendence> filteredAttendences = new ObservableCollection<Attendence>();

            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredAttendences.Add(sortedAttendences[i]);
            }

            return filteredAttendences;
        }
    }
}