using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FhvRoomSearch.Model
{
    partial class Wing : IEquatable<Wing>
    {
        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Wing other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._Id == _Id && _Id != 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Wing)) return false;
            return Equals((Wing) obj);
        }

        public override int GetHashCode()
        {
            return _Id;
        }

        public static bool operator ==(Wing left, Wing right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Wing left, Wing right)
        {
            return !Equals(left, right);
        }
    }
}
