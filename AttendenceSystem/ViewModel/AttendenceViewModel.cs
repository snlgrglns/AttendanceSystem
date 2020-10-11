using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AttendenceSystem.Models;

namespace AttendenceSystem.ViewModel
{
    public class AttendenceViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Fields

        private ObservableCollection<Attendence> attendences;

        private int start = 0;

        private int itemCount = 20;

        private string sortColumn = "Sn";

        private bool ascending = true;

        private int totalItems = 0;

        private ICommand firstCommand;

        private ICommand previousCommand;

        private ICommand nextCommand;

        private ICommand lastCommand;

        #endregion

        /// <summary>
        /// Constructor. Initializes the list of attendences.
        /// </summary>
        public AttendenceViewModel()
        {
            RefreshAttendences();
        }

        /// <summary>
        /// The list of attendences in the current page.
        /// </summary>
        public ObservableCollection<Attendence> Attendences
        {
            get
            {
                return attendences;
            }
            private set
            {
                if (object.ReferenceEquals(attendences, value) != true)
                {
                    attendences = value;
                    NotifyPropertyChanged("Attendences");
                }
            }
        }

        /// <summary>
        /// Gets the index of the first item in the attendences list.
        /// </summary>
        public int Start { get { return start + 1; } }

        /// <summary>
        /// Gets the index of the last item in the attendences list.
        /// </summary>
        public int End { get { return start + itemCount < totalItems ? start + itemCount : totalItems; } }

        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        public int TotalItems { get { return totalItems; } }

        /// <summary>
        /// Gets the command for moving to the first page of attendences.
        /// </summary>
        public ICommand FirstCommand
        {
            get
            {
                if (firstCommand == null)
                {
                    firstCommand = new RelayCommand
                    (
                        param =>
                        {
                            start = 0;
                            RefreshAttendences();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }

                return firstCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the previous page of attendences.
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new RelayCommand
                    (
                        param =>
                        {
                            start -= itemCount;
                            RefreshAttendences();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }

                return previousCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the next page of attendences.
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new RelayCommand
                    (
                        param =>
                        {
                            start += itemCount;
                            RefreshAttendences();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }

                return nextCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the last page of attendences.
        /// </summary>
        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                {
                    lastCommand = new RelayCommand
                    (
                        param =>
                        {
                            start = (totalItems / itemCount - 1) * itemCount;
                            start += totalItems % itemCount == 0 ? 0 : itemCount;
                            RefreshAttendences();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }

                return lastCommand;
            }
        }

        /// <summary>
        /// Sorts the list of attendences.
        /// </summary>
        /// <param name="sortColumn">The column or member that is the basis for sorting.</param>
        /// <param name="ascending">Set to true if the sort</param>
        public void Sort(string sortColumn, bool ascending)
        {
            this.sortColumn = sortColumn;
            this.ascending = ascending;

            RefreshAttendences();
        }

        /// <summary>
        /// Refreshes the list of attendences. Called by navigation commands.
        /// </summary>
        private void RefreshAttendences()
        {
            Attendences = AttendenceAccess.GetAttendences(start, itemCount, sortColumn, ascending, out totalItems);

            NotifyPropertyChanged("Start");
            NotifyPropertyChanged("End");
            NotifyPropertyChanged("TotalItems");
        }

        /// <summary>
        /// Notifies subscribers of changed properties.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
