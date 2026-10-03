using DomainCopilot.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Domain
{
    public class ApprovalRequest
    {
        public Guid ApprovalRequestId { get; private set; }

        public Guid AdjudicationDecisionId { get; private set; }   // طلب الموافقة ده خاص بأنهي قرار

        public ApprovalStatus Status { get; private set; }  // Pending  Approved  Rejected

        public string ReviewerId { get; private set; }  //  ده هنربطه بالموظف اثنا عمليه الدخول مين الشخص اللي راجع القرار

        public string ReviewerComment { get; private set; }  //  الموظف يقدر يكتب تعليق على قراره.

        public DateTime CreatedAt { get; private set; }    // إمتى اتعمل طلب الموافقة

        public DateTime? ReviewedAt { get; private set; }  //  الطلب في البداية لسه محدش راجعه
    }
}
