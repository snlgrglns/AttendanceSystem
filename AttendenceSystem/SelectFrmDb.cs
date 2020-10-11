using AttendenceSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AttendenceSystem
{
    public class SelectFrmDb
    {
        string connectionString = ConfigurationManager.ConnectionStrings["attend"].ConnectionString;

        public int AddEmp(string fname, string lname, string dob, string addr)
        {
            int status;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                //cmd.CommandText = "INSERT into employee_record (first_name,last_name,date_of_birth,address,created_date,card_status) VALUES (@first_name,@last_name,@date_of_birth,@address,@created_date,@card_status)";
                //cmd.CommandType = CommandType.Text;
                //cmd.Parameters.AddWithValue("@first_name", FirstName.Text);
                //cmd.Parameters.AddWithValue("@last_name", LastName.Text);
                //cmd.Parameters.AddWithValue("@date_of_birth", DateTime.Parse(DateBirth.Text));
                //cmd.Parameters.AddWithValue("@address", Address.Text);
                //cmd.Parameters.AddWithValue("@created_date", DateTime.Now);
                //cmd.Parameters.AddWithValue("@card_status", 0); // 0 new record or card not created
                cmd.CommandText = "spAddNewEmp";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@lname", lname);
                cmd.Parameters.AddWithValue("@dob", DateTime.Parse(dob));
                cmd.Parameters.AddWithValue("@addr", addr);
                SqlParameter returnParameter = cmd.Parameters.Add("@result", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteReader();
                status = (int)returnParameter.Value;
                connection.Close();
            }
            return status;
        }

        public DataTable EmpRecords()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                //cmd.CommandText = "SELECT * FROM employee_record";
                //cmd.CommandType = CommandType.Text;
                cmd.CommandText = "spGetAllEmps";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                dt.Load(cmd.ExecuteReader());
                connection.Close();
            }
            return dt;
        }

        public DataTable AttendenceRecords()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = connection;
                //                cmd.CommandText = "SELECT t1.emp_id, t1.attnd_date, t2.id, t2.first_name, t2.last_name FROM attendence_record t1 JOIN employee_record t2 ON t1.emp_id=t2.id;";
                //                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "spGetAllAttRec";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
                connection.Close();
            }
            return dt;
        }

        public DataTable FindEmp(int id, string carduid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = "spEmpWithIdCuid";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emp_id", id);
                cmd.Parameters.AddWithValue("@cuid", carduid);
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
                connection.Close();
            }
            return dt;
        }

        public DataTable FindEmpWithNoCard()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = "spEmpWithNoCard";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
                connection.Close();
            }
            return dt;
        }

        public bool AfterCardCreate(int id, string cuid)
        {
            bool status = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "spEmpUpdateCardStat";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emp_id", id);
                cmd.Parameters.AddWithValue("@cuid", cuid);
                cmd.ExecuteNonQuery();
                connection.Close();
                status = true;
            }
            return status;
        }
        public int CheckAttend(int uid)
        {
            int status = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "spCheckTodayAttnd";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emp_id", uid);
                SqlParameter returnParameter = cmd.Parameters.Add("@result", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteReader();
                status = (int)returnParameter.Value;
                connection.Close();
            }
            return status;
        }
        public void RegisterAttendence(int uid, string carduid)
        {
            try
            {
                DataTable dt = FindEmp(uid, carduid);
                DataRow dr = dt.Rows[0];
                if (dr != null)
                {
                    if ((int)dr["card_status"] == 2)
                        MessageBox.Show("Card Is Blocked");
                    else if ((int)dr["card_status"] == 3)
                        MessageBox.Show("Card Is Expired");
                    else if(CheckAttend(uid)>0)
                        MessageBox.Show("Already Attended");
                    else{
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = connection;
                            cmd.CommandText = "spAttRegister";
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@emp_id", uid);

                            cmd.ExecuteNonQuery();
                            string fname = String.Format("{0}", dr["first_name"]);
                            string lname = String.Format("{0}", dr["last_name"]);
                            MessageBox.Show("Attendence Registered: " + (fname).Trim() + " " + (lname).Trim());
                            connection.Close();
                        }
                    }
                }
                else
                    MessageBox.Show("User Not Found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool BlockCard(int id)
        {
            bool status = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "spEmpBlockCard";
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emp_id", id);
                cmd.ExecuteNonQuery();
                connection.Close();
                status = true;
            }
            return status;
        }
    }
}