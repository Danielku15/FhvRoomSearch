using System;

namespace FhvRoomSearch.Model
{
    [Flags]
    public enum RoomExtras
    {
        None = 0,
        Blackboard = 1,
        Projector = 2, 
        Speaker = 4,
        Microphone = 8,
        SilverScreen = 16, 
        Overhead = 32,
        Climated = 64,
        PartlyClimated = 128,
        AccessControl = 256
    }
}
