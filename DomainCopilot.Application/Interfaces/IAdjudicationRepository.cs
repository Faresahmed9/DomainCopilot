using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{               // ده مسؤول عن تخزين واسترجاع الـ Decision، مش عن اتخاذ القرار نفسه
    public interface IAdjudicationRepository
    {
        Task<AdjudicationDecision?> GetByIdAsync(
       Guid adjudicationDecisionId);

        Task<AdjudicationDecision?> GetByClaimIdAsync(
            Guid claimId);

        Task AddAsync(
            AdjudicationDecision decision);
    }
}
