using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Domain
{
    public class Coverage     //التغطيه التمينيه بتغطى اى والى اى حد
    {
        public Guid CoverageId { get; private set; }

        public Guid PolicyId { get; private set; }

        public string CoverageName { get; private set; }

        public string Description { get; private set; }

        public decimal Limit { get; private set; }

        public decimal Deductible { get; private set; }    // الجزء ال العميل بيتحمله قبل ما التمين يدفع

    }
}
