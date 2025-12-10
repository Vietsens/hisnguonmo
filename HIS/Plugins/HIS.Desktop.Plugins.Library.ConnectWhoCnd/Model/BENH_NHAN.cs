using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd.Model
{
    internal class BENH_NHAN
    {
        public string HO_TEN { get; set; }
        /// <summary>
        /// Là mã giới tính của người bệnh (01: Nam; 02: Nữ).
        /// </summary>
        public string GIOI_TINH { get; set; }
        public string NGAY_SINH { get; set; }
        public string MA_THE_BHYT { get; set; }
        /// <summary>
        /// Ghi số căn cước công dân hoặc số chứng minh thư nhân dân hoặc số hộ chiếu của người bệnh.
        /// Trường hợp không có số căn cước công dân hoặc số chứng minh thư nhân dân hoặc số hộ chiếu thì sử dụng mã tài khoản định danh điện tử.
        /// </summary>
        public string SO_CCCD { get; set; }
        public string DIEN_THOAI { get; set; }
        public string MAXA_CU_TRU { get; set; }
        public string DIA_CHI { get; set; }
        public string MA_NGHE_NGHIEP { get; set; }
        public string MA_CSKCB { get; set; }
        public string MA_CSKCB_BHXH { get; set; }
        public string MA_CSKCB_BHYT { get; set; }
        public string MA_LK { get; set; }
        public BENH_NHAN(HIS_TREATMENT data)
        {
            this.DIA_CHI = data.TDL_PATIENT_ADDRESS;
            this.DIEN_THOAI = data.TDL_PATIENT_MOBILE ?? data.TDL_PATIENT_PHONE;
            this.GIOI_TINH = data.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "01" : "02";
            this.HO_TEN = data.TDL_PATIENT_NAME.Trim();
            this.MA_LK = data.TREATMENT_CODE;
            this.MA_NGHE_NGHIEP = data.TDL_PATIENT_CAREER_NAME;
            this.MA_THE_BHYT = data.TDL_HEIN_CARD_NUMBER;
            this.MA_CSKCB_BHYT = data.TDL_HEIN_MEDI_ORG_CODE;            
            this.MAXA_CU_TRU = data.TDL_PATIENT_COMMUNE_CODE;
            this.NGAY_SINH = DateTime.ParseExact(data.TDL_PATIENT_DOB + "", "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            this.SO_CCCD = data.TDL_PATIENT_CCCD_NUMBER;
            HIS_BRANCH branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == data.BRANCH_ID);
            if (branch != null)
            {
                this.MA_CSKCB = branch.HEIN_MEDI_ORG_CODE;
                this.MA_CSKCB_BHXH = branch.HEIN_MEDI_ORG_CODE;
            }
        }
    }
}
