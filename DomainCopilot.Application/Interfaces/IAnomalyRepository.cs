using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{
    public interface IAnomalyRepository
    {
        Task<Anomaly?> GetByIdAsync(
        Guid anomalyId);

        Task<IReadOnlyList<Anomaly>> GetByClaimIdAsync(
            Guid claimId);

        Task AddAsync(
            Anomaly anomaly);
    }
}
