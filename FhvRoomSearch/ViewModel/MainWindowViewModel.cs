using System;
using System.Data.Objects;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using FhvRoomSearch.Commands;
using FhvRoomSearch.Import;
using FhvRoomSearch.Model;
using Microsoft.Win32;

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
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "iCal Files (*.ical,*.ics)|*.ical;*.ics";

                if(dlg.ShowDialog().GetValueOrDefault())
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(dlg.FileName, Encoding.UTF8))
                        {
                            FhvICalParser parser = new FhvICalParser(reader);
                            parser.Parse();

                            var data = parser.ParsedData;

                            StringBuilder builder = new StringBuilder();

                            foreach (var wing in parser.ParsedData)
                            {
                                builder.AppendLine(wing.Name);
                                foreach (var level in wing.Level)
                                {
                                    builder.AppendLine("    " + level.Name);

                                    foreach (var room in level.Room)
                                    {
                                        builder.AppendLine("        " + room.RoomId);

                                        foreach (var course in room.Course)
                                        {
                                            builder.AppendLine(string.Format("            {0}({1} - {2})", course.Module, course.StartTime, course.EndTime));
                                        }
                                    }
                                }
                                
                            }

                            SerializedModel = builder.ToString();
                        }
                    }
                    catch (ImportException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        throw new ImportException(string.Format("Error loading the calendar file: {0}", e.Message), e);
                    }
                    
                }

            }
            catch (Exception e)
            {
                SerializedModel = e.ToString();
            }
        }

    }
}
