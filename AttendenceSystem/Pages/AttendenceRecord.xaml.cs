using AttendenceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for AttendenceRecord.xaml
    /// </summary>
    public partial class AttendenceRecord : UserControl
    {
        public AttendenceRecord()
        {
//            SelectFrmDb slct = new SelectFrmDb();
//            datatable.DataContext = null;
//            datatable.DataContext = slct.AttendenceRecords();
            InitializeComponent();

            Loaded += LoadDt;
//            LoadDt();
        }

        public void LoadDt(object sender, RoutedEventArgs e)
        {
            SelectFrmDb slct = new SelectFrmDb();
            datatable.DataContext = null;
            datatable.DataContext = slct.AttendenceRecords();
        }
    }
}
