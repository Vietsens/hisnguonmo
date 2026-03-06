using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggrExpMestDetail.ADO
{
    public class UpdatePressInApproveMedicineSDO
    {
        /// <summary>ID của HIS_EXP_MEST_MEDICINE hoặc HIS_EXP_MEST_MATERIAL</summary>
        public long Id { get; set; }

        /// <summary>Số lượng mới sau khi sửa</summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Chênh lệch so với số lượng gốc = Amount_mới - Amount_cũ
        /// Ví dụ: sửa 2->5 thì AmountAdd=3; sửa 2->1 thì AmountAdd=-1
        /// </summary>
        public decimal AmountAdd { get; set; }

        /// <summary>TDL_SERVICE_REQ_ID để BE biết thuốc thuộc y lệnh nào</summary>
        public long TdlServiceReqId { get; set; }
    }

    public class UpdatePressInApproveSDO
    {
        public long ExpMestId { get; set; }
        public long WorkingRoomId { get; set; }

        /// <summary>Danh sách thuốc có sửa SL</summary>
        public List<UpdatePressInApproveMedicineSDO> ModifiedMedicines { get; set; }
            = new List<UpdatePressInApproveMedicineSDO>();

        /// <summary>Danh sách vật tư có sửa SL</summary>
        public List<UpdatePressInApproveMedicineSDO> ModifiedMaterials { get; set; }
            = new List<UpdatePressInApproveMedicineSDO>();

        /// <summary>Danh sách thuốc bị xóa</summary>
        public List<UpdatePressInApproveMedicineSDO> DeletedMedicines { get; set; }
            = new List<UpdatePressInApproveMedicineSDO>();

        /// <summary>Danh sách vật tư bị xóa</summary>
        public List<UpdatePressInApproveMedicineSDO> DeletedMaterials { get; set; }
            = new List<UpdatePressInApproveMedicineSDO>();

        /// <summary>
        /// TDL_SERVICE_REQ_ID của các y lệnh bị xóa hết thuốc/VT
        /// </summary>
        public List<long> DeletedServiceReqIds { get; set; }
            = new List<long>();
    }
}
