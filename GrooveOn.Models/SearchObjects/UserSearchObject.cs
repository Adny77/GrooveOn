
namespace GrooveOn.Model.SearchObjects
{
    public class UserSearchObject : BaseSearchObject
    {
        public string? fullNameSearch {get; set;}
        public bool? IsUser { get; set; } 
        public bool? IsAdmin { get; set; } 
        public bool? IsTrener { get; set; } 
        public bool? IsRadnik { get; set; }
    }
}
