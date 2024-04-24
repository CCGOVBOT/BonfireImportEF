using System;
using System.Collections.Generic;

#nullable disable

namespace BonfireImportEF.Models
{
    public partial class BidListCommodityGroup
    {
        public BidListCommodityGroup()
        {
            BidListContracts = new HashSet<BidListContract>();
        }

        public int CommodityGroupId { get; set; }
        public string Name { get; set; }
        public DateTime? DateStamp { get; set; }
        public string OperatorId { get; set; }
        public string Code { get; set; }
        public short? Status { get; set; }

        public virtual ICollection<BidListContract> BidListContracts { get; set; }
    }
}
