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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private BackgroundWorker _worker;
        CardAccess ca = new CardAccess();

        public SettingsPage()
        {
            InitializeComponent();
            this._worker = new BackgroundWorker();
            this._worker.WorkerSupportsCancellation = true;
            this._worker.DoWork += ca.KeepWaiting;
            this._worker.RunWorkerAsync();
        }

        private void Link_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}
