using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FhvRoomSearch.Model
{
    partial class Level : IEquatable<Level>
    {
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Level other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._Id == _Id && _Id != 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Level)) return false;
            return Equals((Level) obj);
        }

        public override int GetHashCode()
        {
            return _Id;
        }

        public static bool operator ==(Level left, Level right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Level left, Level right)
        {
            return !Equals(left, right);
        }
    }
}
