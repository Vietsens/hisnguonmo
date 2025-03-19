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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.ExportXML.QD917.Common
{
    public class GlobalStore
    {
        //Hein Service Type 
        public static long HeinServiceType_Id__BedIn;
        public static long HeinServiceType_Id__BenOut;
        public static long HeinServiceType_Id__Blood;
        public static long HeinServiceType_Id__Diim;
        public static long HeinServiceType_Id__Exam;
        public static long HeinServiceType_Id__Fuex;
        public static long HeinServiceType_Id__HighTech;
        public static long HeinServiceType_Id__MaterialIn;
        public static long HeinServiceType_Id__MaterialOut;
        public static long HeinServiceType_Id__MaterialRatio;
        public static long HeinServiceType_Id__MaterialVttt;
        public static long HeinServiceType_Id__MedicineCancer;
        public static long HeinServiceType_Id__MedicineIn;
        public static long HeinServiceType_Id__MedicineOut;
        public static long HeinServiceType_Id__MedicineRatio;
        public static long HeinServiceType_Id__SurgMisu;
        public static long HeinServiceType_Id__Test;
        public static long HeinServiceType_Id__Tran;

        //Gender
        public static long Gender_Id__Male;
        public static long Gender_Id__Female;

        //Accident Hurt Type
        public static long AccidentHurtType_Id__Traffic;//Tai nạn giạo thông
        public static long AccidentHurtType_Id__Life;//Sinh hoạt
        public static long AccidentHurtType_Id__Fight;//đánh nhau
        public static long AccidentHurtType_Id__Labor;//tai nạ lao động
        public static long AccidentHurtType_Id__Fall;//ngã
        public static long AccidentHurtType_Id__Animal;//Động vật, súc vật
        public static long AccidentHurtType_Id__Underwater;//Đuối nước
        public static long AccidentHurtType_Id__Burn;//bỏng
        public static long AccidentHurtType_Id__Poisoning;//Ngộ độc
        public static long AccidentHurtType_Id__Suicidal;//Tự tử
        public static long AccidentHurtType_Id__Violence;//Bạo lực
        public static long AccidentHurtType_Id__Other;//Khác

        //Treatment Result Type
        public static long TreatmentResult_Id__Constant;//Không đổi
        public static long TreatmentResult_Id__Cured;//Khỏi
        public static long TreatmentResult_Id__Death;//tử vong
        public static long TreatmentResult_Id__Heavier;//Nặng hơn
        public static long TreatmentResult_Id__Reduce;//Đỡ

        //Treatment End Type
        public static long TreatmentEndType_Id__CapToa;//cấp toa cho về
        public static long TreatmentEndType_Id__ChuyenVien;//Chuyển viện
        public static long TreatmentEndType_Id__HenKham;//Hẹn khám
        public static long TreatmentEndType_Id__RaVien;//Ra viện
        public static long TreatmentEndType_Id__TronVien;//Trốn viện
        public static long TreatmentEndType_Id__TuVong;//Tử vong
        public static long TreatmentEndType_Id__XinRaVien;//xin ra viện
        public static long TreatmentEndType_Id__Other;//Khác

        //Treatment Type
        public static long TreatmentType_Id__Exam;//Khám
        public static long TreatmentType_Id__TreatIn;//Ngoại trú
        public static long TreatmentType_Id__TreatOut;//Nội trú

        //Danh sách Icd10
        public static List<HIS_ICD> ListIcd = new List<HIS_ICD>();
        private static Dictionary<long, HIS_ICD> dicIcd = new Dictionary<long, HIS_ICD>();
        public static Dictionary<long, HIS_ICD> DicIcd
        {
            get
            {
                if (dicIcd == null || dicIcd.Count == 0)
                {
                    dicIcd = new Dictionary<long, HIS_ICD>();
                    foreach (var item in ListIcd)
                    {
                        dicIcd[item.ID] = item;
                    }
                }
                return dicIcd;
            }
        }

        //Danh sách loại thuốc và loại vật tư
        public static List<V_HIS_MEDICINE_TYPE> ListMedicineType = new List<V_HIS_MEDICINE_TYPE>();
        public static List<V_HIS_MATERIAL_TYPE> ListMaterialType = new List<V_HIS_MATERIAL_TYPE>();
        private static Dictionary<long, V_HIS_MEDICINE_TYPE> dicMedicineType = new Dictionary<long, V_HIS_MEDICINE_TYPE>();
        public static Dictionary<long, V_HIS_MEDICINE_TYPE> DicMedicineType
        {
            get
            {
                if (dicMedicineType == null || dicMedicineType.Count == 0)
                {
                    dicMedicineType = new Dictionary<long, V_HIS_MEDICINE_TYPE>();
                    foreach (var item in ListMedicineType)
                    {
                        dicMedicineType[item.SERVICE_ID] = item;
                    }
                }
                return dicMedicineType;
            }
        }
        //danh mã ICD ngoại định suất và ngoài đinh suất đối với trẻ em
        public static List<string> ListIcdCode_Nds = new List<string>();
        public static List<string> ListIcdCode_Nds_Te = new List<string>();

        //Đường dẫn thư mục lưu file xml
        public static string PathSaveXml;

        //Thông tin đơn vị
        public static string Signature;
        public static HIS_BRANCH Branch;

        //Danh sách khoa
        public static List<HIS_DEPARTMENT> ListDepartment = new List<HIS_DEPARTMENT>();
        private static Dictionary<long, HIS_DEPARTMENT> dicDepartment = new Dictionary<long, HIS_DEPARTMENT>();
        public static Dictionary<long, HIS_DEPARTMENT> DicDepartment
        {
            get
            {
                if (dicDepartment == null || dicDepartment.Count == 0)
                {
                    dicDepartment = new Dictionary<long, HIS_DEPARTMENT>();
                    foreach (var item in ListDepartment)
                    {
                        dicDepartment[item.ID] = item;
                    }
                }
                return dicDepartment;
            }
        }
    }
}
