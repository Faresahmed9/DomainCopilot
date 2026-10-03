using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Domain
{
    public class Claim          // مطالبه التامين
    {
        public Guid ClaimId { get; private set; }

        public Guid TenantId { get; private set; }

        public Guid PolicyId { get; private set; }

        public string ClaimNumber { get; private set; }

        public string PolicyNumber { get; private set; }
        public DateTime IncidentDate { get; private set; }

        public DateTime SubmittedAt { get; private set; }

        public decimal ClaimedAmount { get; private set; }

        public string Description { get; private set; }

        public ClaimStatus Status { get; private set; }
    }
}
