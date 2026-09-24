using System.Collections.Generic;

namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// Bản sao của MOS.SDO.HisKskServiceEditSDO (API api/HisKskContract/ServiceEdit).
    /// Khai báo tại plugin để không phụ thuộc bản MOS.SDO.dll mới; tên property PHẢI trùng backend.
    /// </summary>
    public class HisKskServiceEditSDO
    {
        public long KskContractId { get; set; }
        public long RequestRoomId { get; set; }
        public string Loginname { get; set; }
        public string Username { get; set; }
        public long? IntructionTime { get; set; }
        public List<long> TreatmentIds { get; set; }
        public List<KskServiceAddSDO> AddServices { get; set; }
        public List<long> DeleteServiceIds { get; set; }
        public List<KskServiceChangeRoomSDO> ChangeRooms { get; set; }

        public List<KskServiceEditResultSDO> Results { get; set; }
        public KskServiceEditSummarySDO Summary { get; set; }
    }

    public class KskServiceAddSDO
    {
        public long KskId { get; set; }
        public long ServiceId { get; set; }
        public long? RoomId { get; set; }
    }

    public class KskServiceChangeRoomSDO
    {
        public long ServiceId { get; set; }
        public long NewRoomId { get; set; }
    }

    public class KskServiceEditResultSDO
    {
        public long TreatmentId { get; set; }
        public string TreatmentCode { get; set; }
        public string PatientName { get; set; }
        public string Action { get; set; }
        public long ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public short ResultType { get; set; }
        public List<string> Descriptions { get; set; }
    }

    public class KskServiceEditSummarySDO
    {
        public int TotalTreatment { get; set; }
        public int SuccessTreatment { get; set; }
        public int ErrorTreatment { get; set; }
        public int AddedCount { get; set; }
        public int SkippedDuplicateCount { get; set; }
        public int DeletedCount { get; set; }
        public int NotDeletedCount { get; set; }
        public int ChangedRoomCount { get; set; }
        public int NotChangedRoomCount { get; set; }
    }
}
