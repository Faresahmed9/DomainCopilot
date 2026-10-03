using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Domain
{
    public class Anomaly      // المشكله لو جايه من طلب التامين
    {
        public Guid AnomalyId { get; private set; }   

        public Guid ClaimId { get; private set; }   // بيربط الـ Anomaly بالـ Claim اللي فيها المشكلة

        public string AnomalyType { get; private set; }    

        public string Description { get; private set; }

        public AnomalySeverity Severity { get; private set; }       // درجة خطورة المشكلة.

        public DateTime DetectedAt { get; private set; }   // وقت اكتشاف الـ anomaly.

        public Anomaly(
        Guid claimId,
        string anomalyType,
        string description,
        AnomalySeverity severity)
    {
        AnomalyId = Guid.NewGuid();
        ClaimId = claimId;
        AnomalyType = anomalyType;
        Description = description;
        Severity = severity;
        DetectedAt = DateTime.UtcNow;
    }


    }

}
