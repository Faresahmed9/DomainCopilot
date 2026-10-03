using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{
    public interface IPolicyRepository
    {
        Task<Policy?> GetByIdAsync(Guid policyId);       // هاتلي Policy معينة باستخدام الـ ID بتاعها.

        Task<Policy?> GetActiveVersionAsync(          // هاتلي نسخة الـ Policy اللي كانت سارية وقت حدوث الحادث.
            string policyNumber,
            DateTime incidentDate);
    }
}
