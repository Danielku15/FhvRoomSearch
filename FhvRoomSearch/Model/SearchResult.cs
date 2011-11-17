using System;

namespace FhvRoomSearch.Model
{
    public class SearchResult
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


        public SearchResult(Room freeRoom, RoomState roomState, Course currentCourse, DateTime? nextCourseStart)
        {
            FreeRoom = freeRoom;
            RoomState = roomState;
            CurrentCourse = currentCourse;
            NextCourseStart = nextCourseStart;
        }

    }

    public enum RoomState
    {
        Occupied,
        Unoccupied
    }
}
