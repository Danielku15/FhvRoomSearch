namespace FhvRoomSearch.Model
{
    partial class Course
    {
        public string Title
        {
            get
            {
                return string.IsNullOrWhiteSpace(Module) ? Notes : Module;
            }
        }
    }
}
