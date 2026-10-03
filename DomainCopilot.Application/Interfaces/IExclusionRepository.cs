using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{
    public interface IExclusionRepository
    {
        Task<Exclusion?> GetByIdAsync(
       Guid exclusionId);

        Task<IReadOnlyList<Exclusion>> GetByPolicyIdAsync(
            Guid policyId);
    }
}
