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

        public string ReviewerId { get; private set; } = string.Empty;  //  ده هنربطه بالموظف اثنا عمليه الدخول مين الشخص اللي راجع القرار

        public string ReviewerComment { get; private set; } = string.Empty;  //  الموظف يقدر يكتب تعليق على قراره.

        public DateTime CreatedAt { get; private set; }    // إمتى اتعمل طلب الموافقة

        public DateTime? ReviewedAt { get; private set; }  //  الطلب في البداية لسه محدش راجعه

        // اعتماد القرار من خلال الموظف المسؤول عن المراجعة

        public ApprovalRequest(Guid adjudicationDecisionId)
        {
            ApprovalRequestId = Guid.NewGuid();
            AdjudicationDecisionId = adjudicationDecisionId;
            Status = ApprovalStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
        public void Approve(
            string reviewerId,
            string reviewerComment)
        {
            Status = ApprovalStatus.Approved;
            ReviewerId = reviewerId;
            ReviewerComment = reviewerComment;
            ReviewedAt = DateTime.UtcNow;
        }

        // رفض القرار من خلال الموظف المسؤول عن المراجعة
        public void Reject(
            string reviewerId,
            string reviewerComment)
        {
            Status = ApprovalStatus.Rejected;
            ReviewerId = reviewerId;
            ReviewerComment = reviewerComment;
            ReviewedAt = DateTime.UtcNow;
        }
    }


}
