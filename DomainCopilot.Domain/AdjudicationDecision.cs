using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Domain
{
    public class AdjudicationDecision     // نتيجة فحص الطلب   
    {
        public Guid AdjudicationDecisionId { get; private set; }

        public Guid ClaimId { get; private set; }

        public DecisionStatus Decision { get; private set; }   //القرار النهائي للمطالبة.

        public decimal ApprovedAmount { get; private set; }  // لمبلغ اللي النظام حسب إنه مستحق.

        public string Reason { get; private set; }         // سبب القرار.

        public DateTime CreatedAt { get; private set; }   // وقت إنشاء القرار.
    }
}
