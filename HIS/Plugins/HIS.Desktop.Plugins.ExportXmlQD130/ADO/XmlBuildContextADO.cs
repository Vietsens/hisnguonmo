/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using His.Bhyt.ExportXml.XML130;
using MOS.EFMODEL.DataModels;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    /// <summary>
    /// Du lieu dung chung cho CA MOT LO ho so khi dung tep XML.
    ///
    /// Cac tu dien trong day duoc nap MOT LAN truoc vong lap, roi tra cuu theo ma dot dieu tri.
    /// Tach ra thanh lop rieng de ca luong gui Cong tiep nhan bao hiem xa hoi lan luong soat loi
    /// MDInsight dung CHUNG mot ham dung ado - tranh dung lai logic o hai noi roi lech nhau.
    ///
    /// ⚠️ Vi sao bat buoc phai dung chung: danh sach thuoc da dung (ListExpMedimateUsed) phai duoc
    /// loc THEO TUNG HO SO. Truyen ca lo vao thi viec do khop se nhan cheo giua cac ho so, lam so
    /// luong thuoc bi cong don va thoi phong T_TONGCHI_BV / T_TONGCHI_BH tren tep XML.
    /// </summary>
    internal class XmlBuildContextADO
    {
        public Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> DicPatientTypeAlter { get; set; }

        public Dictionary<long, List<V_HIS_SERE_SERV_2>> DicSereServ { get; set; }

        public Dictionary<long, List<HIS_DHST>> DicDhstList { get; set; }

        public Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> DicSereServTein { get; set; }

        public Dictionary<long, List<V_HIS_SERE_SERV_SUIN>> DicSereServSuin { get; set; }

        public Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> DicSereServPttt { get; set; }

        public Dictionary<long, List<V_HIS_BED_LOG>> DicBedLog { get; set; }

        public Dictionary<long, List<HIS_TRACKING>> DicTracking { get; set; }

        public Dictionary<long, List<HIS_EKIP_USER>> DicEkipUser { get; set; }

        public Dictionary<long, List<HIS_DEBATE>> DicDebate { get; set; }

        public Dictionary<long, List<V_HIS_BABY>> DicBaby { get; set; }

        public Dictionary<long, List<V_HIS_MEDICAL_ASSESSMENT>> DicMedicalAssessment { get; set; }

        public Dictionary<long, HIS_HIV_TREATMENT> DicHivTreatment { get; set; }

        public Dictionary<long, HIS_TUBERCULOSIS_TREAT> DicTuberculosisTreat { get; set; }

        /// <summary>Thuoc da dung, tra cuu theo ma dong xuat thuoc</summary>
        public Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> DicExpUsedByExpMestMedicineId { get; set; }

        /// <summary>Vat tu da dung, tra cuu theo ma dong xuat vat tu</summary>
        public Dictionary<long, List<HIS_EXP_MEDIMATE_USED>> DicExpUsedByExpMestMaterialId { get; set; }

        /// <summary>Danh sach loai tep XML se sinh, ngan boi dau phay</summary>
        public string TypeXml { get; set; }

        /// <summary>
        /// Thong tin may chu Cong tiep nhan bao hiem xa hoi.
        /// Luong soat loi MDInsight KHONG gui cong nay nhung van phai truyen vi thu vien
        /// dung tep doc mot so tham so tu day.
        /// </summary>
        public ServerInfo ServerInfo { get; set; }
    }
}
