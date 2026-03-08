// Rentify.Model/Responses/LoginResponse.cs
using System;
using System.Collections.Generic;

namespace GrooveOn.Model.ResponseObject
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; } = string.Empty;
        public List<String>? Roles { get; set; }
    }
}
