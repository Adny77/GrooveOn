using System;

namespace GrooveOn.Model.RequestObjects
{
    public class RoleUpsertRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
