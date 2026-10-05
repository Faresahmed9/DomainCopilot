using System;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Domain
{
    public class AdjudicationDecision     // نتيجة فحص الطلب
    {
        public Guid AdjudicationDecisionId { get; private set; }

    public Guid ClaimId { get; private set; }

        public DecisionStatus Decision { get; private set; }   //القرار النهائي للمطالبة.

        public decimal ApprovedAmount { get; private set; }  // لمبلغ اللي النظام حسب إنه مستحق.

        public string Reason { get; private set; } = string.Empty;         // سبب القرار.

        public DateTime CreatedAt { get; private set; }   // وقت إنشاء القرار.

        // Constructor لإنشاء قرار جديد من النظام
        public AdjudicationDecision(
            Guid claimId,
            DecisionStatus decision,
            decimal approvedAmount,
            string reason)
        {
            AdjudicationDecisionId = Guid.NewGuid();
            ClaimId = claimId;
            Decision = decision;
            ApprovedAmount = approvedAmount;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }
    }


}
