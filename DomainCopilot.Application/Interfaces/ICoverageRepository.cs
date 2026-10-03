using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{
    public interface ICoverageRepository
    {
        Task<Coverage?> GetByIdAsync(
     Guid coverageId);

        Task<IReadOnlyList<Coverage>> GetByPolicyIdAsync(
            Guid policyId);
    }
}
