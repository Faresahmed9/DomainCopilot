using DomainCopilot.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.Interfaces
{
    public interface IClaimRepository
    {
        Task<Claim?> GetByIdAsync(Guid claimId);         // هاتلي الـ Claim اللي الـ ClaimId بتاعه كذا.

        Task<Claim?> GetByClaimNumberAsync(             // عشان نضمن إننا بنجيب الـ Claim الخاص بالـ Tenant الصحيح.     وده جزء مهم جدًا في الـ Security بتاع المشروع.
            string claimNumber,
            Guid tenantId);
    }
}
