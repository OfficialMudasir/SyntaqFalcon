using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.AccessControlList.Dtos
{
    public class ACLInput
    {
        public int? ACLId { get; set; }

        public string Type { get; set; }

        public string Username { get; set; }

        public long? UserId { get; set; } //for v2

        public long? OrgId { get; set; } //for v2

        public string Role { get; set; }

        public Guid EntityId { get; set; }
    }
}
