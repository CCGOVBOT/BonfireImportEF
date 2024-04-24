using System;
using System.Collections.Generic;

#nullable disable

namespace BonfireImportEF.Models
{
    public partial class BidListContractsTest
    {
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public short Rebid { get; set; }
        public string Cfor { get; set; }
        public string Cfrom { get; set; }
        public string LinkTo { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime BidOpening { get; set; }
        public DateTime BidExpiration { get; set; }
        public string VendorAwarded { get; set; }
        public double? AmountAwarded { get; set; }
        public DateTime? DateAwarded { get; set; }
        public int? CommodityGroupId { get; set; }
        public string Detail { get; set; }
        public int? VendorId { get; set; }
        public string OperatorId { get; set; }
        public DateTime DateStamp { get; set; }
        public int? Addendum { get; set; }
        public int Notification { get; set; }
        public int? Attachment { get; set; }
        public byte[] Description { get; set; }
        public string Type { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Subdivision { get; set; }
        public string Agency { get; set; }
        public string StatusValue { get; set; }
        public string StatusLastUser { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string FileType { get; set; }
        public string File0 { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string File3 { get; set; }
    }
}
