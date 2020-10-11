using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendenceSystem.ViewModel
{
    class RegisterViewModel : NotifyPropertyChanged, IDataErrorInfo
    {
        private string firstName;
        private string lastName;
        private string dob;
        private string address;

        public string FirstName
        {
            get { return this.firstName; }
            set
            {
                if (this.firstName != value)
                {
                    this.firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public string LastName
        {
            get { return this.lastName; }
            set
            {
                if (this.lastName != value)
                {
                    this.lastName = value;
                    OnPropertyChanged("LastName");
                }
            }
        }

        public string Dob
        {
            get { return this.dob; }
            set
            {
                if (this.dob != value)
                {
                    this.dob = value;
                    OnPropertyChanged("Dob");
                }
            }
        }

        public string Address
        {
            get { return this.address; }
            set
            {
                if (this.address != value)
                {
                    this.address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "FirstName")
                {
                    return string.IsNullOrEmpty(this.firstName) ? "Required value" : null;
                }
                if (columnName == "LastName")
                {
                    return string.IsNullOrEmpty(this.lastName) ? "Required value" : null;
                }
                if (columnName == "Dob")
                {
                    return string.IsNullOrEmpty(this.dob) ? "Required value" : null;
                }
                if (columnName == "Address")
                {
                    return string.IsNullOrEmpty(this.address) ? "Required value" : null;
                }
                return null;
            }
        }
    }
}

