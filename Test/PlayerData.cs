using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class PlayerData:NotificationObject
    {
        ObservableCollection<Hero> _roles;
        public ObservableCollection<Hero> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
                RaisePropertyChanged("Roles");
            }
        }

        ObservableCollection<Building> _buildings;
        public ObservableCollection<Building> Buildings
        {
            get
            {
                return _buildings;
            }

            set
            {
                _buildings = value;
                RaisePropertyChanged("Buildings");
            }
        }

        string _nick;
        public string Nick
        {
            get
            {
                return _nick;
            }

            set
            {
                _nick = value;
                RaisePropertyChanged("Nick");
            }
        }       

        public PlayerData()
        {
            Roles = new ObservableCollection<Hero>();
            Buildings = new ObservableCollection<Building>();
        }
    }
}
