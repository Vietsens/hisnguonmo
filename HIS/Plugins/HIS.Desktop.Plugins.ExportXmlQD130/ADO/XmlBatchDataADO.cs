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
using MOS.EFMODEL.DataModels;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    /// <summary>
    /// Noi chua du lieu nap cho MOT LO ho so truoc khi dung tep XML (cac ham ThreadGet* ghi vao day).
    ///
    /// Cac luong cu (Xuat Xml, Gui tu dong, Kiem tra ho so, MDInsight...) van dung chung bo truong
    /// cua man hinh: ban giu cua ho tro thang toi cac danh sach do nen hanh vi khong doi.
    /// Luong tu dong xuat XML ho so du 48 gio chay nen 10 phut mot lan nen dung ban giu RIENG,
    /// khong ghi de len du lieu luong khac dang dung do (viec 61380).
    /// </summary>
    internal class XmlBatchDataADO
    {
        public List<V_HIS_TREATMENT_12> Treatments { get; set; }
        public List<V_HIS_PATIENT_TYPE_ALTER> PatientTypeAlters { get; set; }
        public List<V_HIS_SERE_SERV_2> SereServs { get; set; }
        public List<HIS_EKIP_USER> EkipUsers { get; set; }
        public List<V_HIS_BED_LOG> BedLogs { get; set; }
        public List<HIS_DHST> Dhsts { get; set; }
        public List<HIS_TRACKING> Trackings { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> SereServTeins { get; set; }
        public List<V_HIS_SERE_SERV_SUIN> SereServSuins { get; set; }
        public List<V_HIS_SERE_SERV_PTTT> SereServPttts { get; set; }
        public List<HIS_DEBATE> Debates { get; set; }
        public List<V_HIS_BABY> Babies { get; set; }
        public List<V_HIS_MEDICAL_ASSESSMENT> MedicalAssessments { get; set; }
        public List<HIS_HIV_TREATMENT> HivTreatments { get; set; }
        public List<HIS_TUBERCULOSIS_TREAT> TuberculosisTreats { get; set; }
        public List<HIS_EXP_MEDIMATE_USED> ExpMedimateUseds { get; set; }

        /// <summary>Doi tuong thanh toan dung de loc dong dich vu. Trong = lay tat ca.</summary>
        public List<long> PatientTypeTtIds { get; set; }

        /// <summary>Tao ban giu moi, cac danh sach rong, khong loc doi tuong thanh toan</summary>
        public static XmlBatchDataADO CreateEmpty()
        {
            return new XmlBatchDataADO
            {
                Treatments = new List<V_HIS_TREATMENT_12>(),
                PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>(),
                SereServs = new List<V_HIS_SERE_SERV_2>(),
                EkipUsers = new List<HIS_EKIP_USER>(),
                BedLogs = new List<V_HIS_BED_LOG>(),
                Dhsts = new List<HIS_DHST>(),
                Trackings = new List<HIS_TRACKING>(),
                SereServTeins = new List<V_HIS_SERE_SERV_TEIN>(),
                SereServSuins = new List<V_HIS_SERE_SERV_SUIN>(),
                SereServPttts = new List<V_HIS_SERE_SERV_PTTT>(),
                Debates = new List<HIS_DEBATE>(),
                Babies = new List<V_HIS_BABY>(),
                MedicalAssessments = new List<V_HIS_MEDICAL_ASSESSMENT>(),
                HivTreatments = new List<HIS_HIV_TREATMENT>(),
                TuberculosisTreats = new List<HIS_TUBERCULOSIS_TREAT>(),
                ExpMedimateUseds = new List<HIS_EXP_MEDIMATE_USED>()
            };
        }
    }

    /// <summary>Tham so truyen vao tung ham ThreadGet*: lo ho so can nap va noi chua ket qua</summary>
    internal class XmlBatchThreadArgADO
    {
        public List<V_HIS_TREATMENT_1> Treatments { get; set; }
        public XmlBatchDataADO Holder { get; set; }
    }
}
