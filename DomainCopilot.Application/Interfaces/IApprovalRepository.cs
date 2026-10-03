using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{    // مسؤول عن طلبات موافقة الـ Adjuster.
    public interface IApprovalRepository
    {
        Task<ApprovalRequest?> GetByIdAsync(
        Guid approvalRequestId);

        Task<ApprovalRequest?> GetByDecisionIdAsync(
            Guid adjudicationDecisionId);  

        Task AddAsync(
            ApprovalRequest approvalRequest);

        Task UpdateAsync(
            ApprovalRequest approvalRequest);
    }
}
