using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Domain
{
    public class Exclusion       //مراجعه طلب التامين ويشوف هل فى استثناء ولا لا 
    {
        public Guid ExclusionId { get; private set; }

        public Guid PolicyId { get; private set; }

        public string ExclusionName { get; private set; }

        public string Description { get; private set; }
    }
}
