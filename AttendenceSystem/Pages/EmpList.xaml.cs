using AttendenceSystem.Models;
using AttendenceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for EmpList.xaml
    /// </summary>
    public partial class EmpList : UserControl
    {
        private DataGridColumn currentSortColumn;

        private ListSortDirection currentSortDirection;
        public EmpList()
        {
            InitializeComponent();
            Loaded += Load;
        }
        private void Load(object sender, RoutedEventArgs e)
        {
            DataContext = null;
            DataContext = new EmployeeViewModel();
        }
        private void EmployeesDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;

            // The current sorted column must be specified in XAML.
            currentSortColumn = dataGrid.Columns.Where(c => c.SortDirection.HasValue).Single();
            currentSortDirection = currentSortColumn.SortDirection.Value;
        }

        void BlockCard(object sender, RoutedEventArgs e) {
            Employee obj = ((FrameworkElement)sender).DataContext as Employee;
            if (obj.Status != "card blocked")
            {
                SelectFrmDb sdb = new SelectFrmDb();
                CardAccess ca = new CardAccess();
                if (sdb.BlockCard(obj.Id))
                {
                    MessageBox.Show("Card Blocked");
                }
            }
            else MessageBox.Show("Card Already Blocked");
        }

        void AddCard(object sender, RoutedEventArgs e)
        {
            Employee obj = ((FrameworkElement)sender).DataContext as Employee;
            if (obj.Status == "no card")
            {
                CardAccess ca = new CardAccess();
                if (ca.RegisterCard(obj.Id.ToString()))
                {
                    MessageBox.Show("Card Created");
                }
            }
            else MessageBox.Show("Card Already Exists");
        }

        /// <summary>
        /// Sets the sort direction for the current sorted column since the sort direction
        /// is lost when the DataGrid's ItemsSource property is updated.
        /// </summary>
        /// <param name="sender">The parts data grid.</param>
        /// <param name="e">Ignored.</param>
        private void EmployeesDataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (currentSortColumn != null)
            {
                currentSortColumn.SortDirection = currentSortDirection;
            }
        }

        /// <summary>
        /// Custom sort the datagrid since the actual records are stored in the
        /// server, not in the items collection of the datagrid.
        /// </summary>
        /// <param name="sender">The parts data grid.</param>
        /// <param name="e">Contains the column to be sorted.</param>
        private void EmployeesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;

            EmployeeViewModel empViewModel = (EmployeeViewModel)DataContext;

            string sortField = String.Empty;

            // Use a switch statement to check the SortMemberPath
            // and set the sort column to the actual column name. In this case,
            // the SortMemberPath and column names match.
            switch (e.Column.SortMemberPath)
            {
                case ("Sn"):
                    sortField = "Sn";
                    break;
                case ("Name"):
                    sortField = "Name";
                    break;
                case ("Dob"):
                    sortField = "Dob";
                    break;
                case ("Address"):
                    sortField = "Address";
                    break;
                case ("Status"):
                    sortField = "Status";
                    break;
            }

            ListSortDirection direction = (e.Column.SortDirection != ListSortDirection.Ascending) ?
                ListSortDirection.Ascending : ListSortDirection.Descending;

            bool sortAscending = direction == ListSortDirection.Ascending;

            empViewModel.Sort(sortField, sortAscending);

            currentSortColumn.SortDirection = null;

            e.Column.SortDirection = direction;

            currentSortColumn = e.Column;
            currentSortDirection = direction;
        }
    }
}