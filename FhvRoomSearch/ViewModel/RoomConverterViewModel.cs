using System;
using System.Text.RegularExpressions;
using FhvRoomSearch.Model;
using GalaSoft.MvvmLight;

namespace FhvRoomSearch.ViewModel
{
    class RoomConverterViewModel : ViewModelBase
    {
        private static readonly Regex NumberMatcher = new Regex(@"^[0-9]+(\+1)?$", RegexOptions.Compiled);
        private string _inputString;
        public string InputString
        {
            get
            {
                return _inputString;
            }
            set
            {
                if (_inputString == value)
                    return;
                _inputString = value.Trim();
                UpdateOutput();
                RaisePropertyChanged("InputString");
            }
        }

        private string _outputString;
        public string OutputString
        {
            get
            {
                return _outputString;
            }
            set
            {
                if (_outputString == value)
                    return;
                _outputString = value;
                RaisePropertyChanged("OutputString");
            }
        }


        private void UpdateOutput()
        {
            try
            {
                // parse input string
                string startData;
                string metaData;
                int metaStart = InputString.IndexOf("(");

                if (metaStart > 0)
                {
                    startData = InputString.Substring(0, metaStart);
                    metaData = InputString.Substring(metaStart);
                }
                else
                {
                    startData = InputString;
                    metaData = "";
                }

                string[] lines = startData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                Room room = new Room();
                room.RoomId = ParseRoomId(lines[0]).Trim();
                room.RoomName = "";
                for (int i = 1; i < lines.Length; i++)
                {
                    room.RoomName += lines[i];
                }

                ParseRoomData(room, metaData);

                // generate output
                //       <Room RoomId="A001" RoomName="Veranstaltungssaal (Aula)" Tables="0" Chairs="178" Computers="0" ExtrasValue="" ChairTypeValue="" />

                OutputString =
                    string.Format(
                        "<Room RoomId=\"{0}\" RoomName=\"{1}\" Tables=\"{2}\" Chairs=\"{3}\" Computers=\"{4}\" Extras=\"{5}\" ChairType=\"{6}\" />",
                        room.RoomId, room.RoomName, room.Tables, room.Chairs, room.Computers, room.Extras,
                        room.ChairType);
            }
            catch (Exception)
            {
                OutputString = "Invalid Format";
            }

        }

        private string ParseRoomId(string p)
        {
            return p.Replace(" ", "");
        }

        private void ParseRoomData(Room room, string p)
        {
            string s = p.ToLower().Replace("kb ", "kb").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ").Replace("(", "").Replace(")", "").Replace("*", "");

            string[] parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].EndsWith("st+1"))
                {
                    room.Chairs = GetNumberWithPlusCheck(parts[i].Substring(0, parts[i].Length - 4)) + 1;
                }
                else if (parts[i].EndsWith("st"))
                {
                    room.Chairs = GetNumberWithPlusCheck(parts[i].Substring(0, parts[i].Length - 2));
                }
                else if (parts[i].EndsWith("t+1"))
                {
                    room.Tables = GetNumberWithPlusCheck(parts[i].Substring(0, parts[i].Length - 3)) + 1;
                }
                else if (parts[i].Length > 1 && parts[i].EndsWith("t"))
                {
                    room.Tables = GetNumberWithPlusCheck(parts[i].Substring(0, parts[i].Length - 1));
                }
                else if (parts[i].EndsWith("pc+1"))
                {
                    room.Computers = GetNumberWithPlusCheck(parts[i].Substring(0, parts[i].Length - 4)) + 1;
                }
                else if (parts[i].EndsWith("pc"))
                {
                    room.Computers = GetNumberWithPlusCheck(parts[i].Substring(0, parts[i].Length - 2));
                }
                else if (i == 0 && char.IsDigit(parts[i][0]))
                {
                    room.Chairs = GetNumberWithPlusCheck(parts[i]);
                }
                else if (parts[i] == "t")
                {
                    room.Extras |= RoomExtras.Blackboard;
                }
                else if (parts[i] == "b")
                {
                    room.Extras |= RoomExtras.Projector;
                }
                else if (parts[i] == "l")
                {
                    room.Extras |= RoomExtras.Speaker;
                }
                else if (parts[i] == "m")
                {
                    room.Extras |= RoomExtras.Microphone;
                }
                else if (parts[i] == "lw")
                {
                    room.Extras |= RoomExtras.SilverScreen;
                }
                else if (parts[i] == "o")
                {
                    room.Extras |= RoomExtras.Overhead;
                }
                else if (parts[i].StartsWith("kb"))
                {
                    if (parts[i].Length > 2)
                    {
                        int chairs = GetNumberWithPlusCheck(parts[i].Substring(2));
                        room.Chairs = chairs;
                    }

                    // check if next token is a number only
                    if (i < parts.Length - 1 && NumberMatcher.IsMatch(parts[i + 1]))
                    {
                        i++;
                        room.Chairs = GetNumberWithPlusCheck(parts[i]);
                    }
                    room.ChairType = RoomChairTypes.NormalClass;
                }
                else if (parts[i].StartsWith("u"))
                {
                    if (parts[i].Length > 2)
                    {
                        int chairs = GetNumberWithPlusCheck(parts[i].Substring(1));
                        room.Chairs = chairs;
                    }
                    room.ChairType = RoomChairTypes.UShape;
                }
                else if (parts[i].StartsWith("r"))
                {
                    if (parts[i].Length > 2)
                    {
                        int chairs = GetNumberWithPlusCheck(parts[i].Substring(1));
                        room.Chairs = chairs;
                    }
                    room.ChairType = RoomChairTypes.RectangleShape;
                }
                else if (parts[i].StartsWith("k"))
                {
                    if (parts[i].Length > 1)
                    {
                        int chairs = GetNumberWithPlusCheck(parts[i].Substring(1));
                        room.Chairs = chairs;
                    }
                    room.ChairType = RoomChairTypes.Cinema;
                }
                else if (parts[i].StartsWith("stk"))
                {
                    if (parts[i].Length > 2)
                    {
                        int chairs = GetNumberWithPlusCheck(parts[i].Substring(3));
                        room.Chairs = chairs;
                    }
                    room.ChairType = RoomChairTypes.Circle;
                }
            }
        }

        private int GetNumberWithPlusCheck(string value)
        {
            if (value.Contains("+"))
            {
                string[] chairs = value.Split('+');
                return int.Parse(chairs[0]) + int.Parse(chairs[1]);
            }
            return int.Parse(value);
        }
    }
}
