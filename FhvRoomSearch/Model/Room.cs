namespace FhvRoomSearch.Model
{
    partial class Room
    {
        public RoomChairTypes ChairType
        {
            get
            {
                return (RoomChairTypes)ChairTypeValue;
            }
            set
            {
                ChairTypeValue = (int)value;
            }
        }

        public RoomExtras Extras
        {
            get { return (RoomExtras)ExtrasValue; }
            set { ExtrasValue = (int)value; }
        }
    }
}
