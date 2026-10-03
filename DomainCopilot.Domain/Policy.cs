using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Domain
{
    public class Policy    // وثيقه التامين
    {
        public Guid PolicyId { get; private set; }

        public Guid TenantId { get; private set; }

        public string PolicyNumber { get; private set; }

        public string PolicyName { get; private set; }

        public int Version { get; private set; }     //D2: Wrong policy versio  

        public DateTime EffectiveFrom { get; private set; }

        public DateTime EffectiveTo { get; private set; }
    }
}
