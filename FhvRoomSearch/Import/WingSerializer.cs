using System;
using System.Collections.Generic;
using System.Xml;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.Import
{
    class WingSerializer
    {
        private readonly XmlDocument _document;
        private readonly IDataService _dataService;

        public WingSerializer(XmlDocument document, IDataService dataService)
        {
            _document = document;
            _dataService = dataService;
        }

        public IList<Wing> Wings
        {
            get;
            private set;
        }

        public void Deserialize()
        {
            Wings = new List<Wing>();


            XmlElement root = _document.DocumentElement;
            ParseWings(root.GetElementsByTagName("Wing"));

        }

        private void ParseWings(XmlNodeList wingNodes)
        {
            foreach (var node in wingNodes)
            {
                XmlElement wingNode = (XmlElement)node;

                Wing wing = new Wing();

                wing.Name = wingNode.GetAttribute("Name");

                // TODO: ParseLevels(wing, wingNode.GetElementsByTagName("Level"));

                Wings.Add(wing);
            }
        }

        private void ParseLevels(Wing wing, XmlNodeList levelNodes)
        {
            foreach (var node in levelNodes)
            {
                XmlElement levelNode = (XmlElement)node;

                Level lvl = new Level();

                wing.Name = levelNode.GetAttribute("Name");

                ParseRooms(lvl, levelNode.GetElementsByTagName("Room"));

                wing.Level.Add(lvl);
            }
        }

        private void ParseRooms(Level lvl, XmlNodeList roomNodes)
        {
            foreach (var node in roomNodes)
            {
                XmlElement roomNode = (XmlElement)node;

                Room room = new Room();

                room.RoomId = roomNode.GetAttribute("RoomId");
                room.RoomName = roomNode.GetAttribute("RoomName");
                room.Tables = int.Parse(roomNode.GetAttribute("Tables"));
                room.Chairs = int.Parse(roomNode.GetAttribute("Chairs"));
                room.Computers = int.Parse(roomNode.GetAttribute("Computers"));
                room.Extras = ((RoomExtras) Enum.Parse(typeof (RoomExtras), roomNode.GetAttribute("Extras")));
                room.ChairType =
                    ((RoomChairTypes) Enum.Parse(typeof (RoomChairTypes), roomNode.GetAttribute("ChairType")));

                lvl.Room.Add(room);
            }
        }
    }
}