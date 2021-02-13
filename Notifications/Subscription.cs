using System;
using Microsoft.Azure.Documents;

namespace vaxalert.Stores
{
    public class Subscription : Resource
    {
        public string EventKey { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}