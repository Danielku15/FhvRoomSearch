using System;

namespace FhvRoomSearch.Model
{
    partial class Room : IEquatable<Room>, IComparable<Room>
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

        public int CompareTo(Room other)
        {
            if (ReferenceEquals(null, other)) return 1;
            if (ReferenceEquals(this, other)) return 0;
            return RoomId.CompareTo(other.RoomId);
        }

        public override string ToString()
        {
            return RoomId;
        }

        public bool Equals(Room other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._Id == _Id && _Id != 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Room)) return false;
            return Equals((Room) obj);
        }

        public override int GetHashCode()
        {
            return _Id;
        }

        public static bool operator ==(Room left, Room right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Room left, Room right)
        {
            return !Equals(left, right);
        }


    }
}
