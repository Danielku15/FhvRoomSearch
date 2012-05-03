using System;

namespace FhvRoomSearch.Model
{
    public class SearchResult : IComparable<SearchResult>
    {
        public Room FreeRoom
        {
            get;
            set;
        }

        public RoomState RoomState
        {
            get;
            set;
        }

        public Course CurrentCourse
        {
            get;
            set;
        }
        //public Course NextCourse { get; set; }
        public DateTime? NextCourseStart
        {
            get;
            set;
        }

        public SearchResult()
        {
            RoomState = RoomState.Unoccupied;
        }

        public SearchResult(Room freeRoom, RoomState roomState, Course currentCourse, DateTime? nextCourseStart)
        {
            FreeRoom = freeRoom;
            RoomState = roomState;
            CurrentCourse = currentCourse;
            NextCourseStart = nextCourseStart;
        }


        public int CompareTo(SearchResult other)
        {
            if (ReferenceEquals(null, other))
                return 1;
            if (ReferenceEquals(null, other.FreeRoom))
                return 1;
            if (ReferenceEquals(null, FreeRoom))
                return -1;
            return FreeRoom.CompareTo(other.FreeRoom);

        }
    }

    public enum RoomState
    {
        Occupied,
        Unoccupied
    }
}
