using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.DTOs
{
    public class ClaimContextDto
    {
        public Claim Claim { get; set; }

        public Policy Policy { get; set; }

        public IReadOnlyList<Coverage> Coverages { get; set; }

        public IReadOnlyList<Exclusion> Exclusions { get; set; }
    }
}
