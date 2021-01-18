using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGebra
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _MyTitle = "";

        public string MyTitle
        {
            get { return _MyTitle; }
            set
            {
                _MyTitle = value;
                this.OnPropertyChanged("");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
