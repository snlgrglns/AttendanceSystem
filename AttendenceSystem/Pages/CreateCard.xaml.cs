using AttendenceSystem.Models;
using AttendenceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CreateCard.xaml
    /// </summary>
    public partial class CreateCard : UserControl
    {
        public CreateCard()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //http://stackoverflow.com/questions/1483652/wpf-combobox-binding-behaviour?rq=1 for binding
            CardAccess ca = new CardAccess();
            if (comboBox.Text != "")
            {
                if (ca.RegisterCard(EmpSetWhenChange.Emp_Id.ToString()))
                {
                    MessageBox.Show("Card Created for \"" + EmpSetWhenChange.Emp_Name + "\"");
                }
            }
        }
    }
}
