using System;
using System.Data.Objects;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using FhvRoomSearch.Commands;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private RoomCourseModelContainer _database;

        public MainWindowViewModel()
        {
            _database = new RoomCourseModelContainer();
            SerializeCommand = new RelayCommand(p => DoSerialize());
        }

        private string _serializedModel;

        public string SerializedModel
        {
            get
            {
                return _serializedModel;
            }
            set
            {
                if (_serializedModel == value)
                    return;
                _serializedModel = value;
                OnPropertyChanged("SerializedModel");
            }
        }

        public ICommand SerializeCommand
        {
            get;
            private set;
        }


        private void DoSerialize()
        {
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(Wing));
                using (StringWriter writer = new StringWriter())
                {
                    foreach (var wing in _database.WingSet)
                    {
                        serializer.Serialize(writer, wing);
                    }
                    SerializedModel = writer.ToString();
                }

            }
            catch (Exception e)
            {
                SerializedModel = e.ToString();
            }
        }

    }
}
