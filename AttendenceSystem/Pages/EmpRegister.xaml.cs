using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AttendenceSystem.Pages
{
    /// <summary>
    /// Interaction logic for EmpRegister.xaml
    /// </summary>
    public partial class EmpRegister : UserControl
    {
        public EmpRegister()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstName.Text) || string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(DateBirth.Text) || string.IsNullOrWhiteSpace(Address.Text))
            {
                MessageBox.Show("Fill up All Fields");
            }
            else
            {
                SelectFrmDb sdb = new SelectFrmDb();
                int add_emp = sdb.AddEmp(FirstName.Text, LastName.Text, DateBirth.Text, Address.Text);
                if(add_emp == 1)
                {
                    FirstName.Text = "";
                    LastName.Text = "";
                    DateBirth.Text = "";
                    Address.Text = "";
                    MessageBox.Show("Add Success!!!");
                }
            }
        }
    }
}