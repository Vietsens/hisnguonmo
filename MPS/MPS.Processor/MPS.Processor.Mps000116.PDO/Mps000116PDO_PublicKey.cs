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
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000116.PDO
{
    public partial class Mps000116PDO : RDOBase
    {
        public V_HIS_PATIENT currentPatient { get; set; }
        public V_HIS_TREATMENT _Treatment { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public IsKeyCheck _KeyCheck { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials { get; set; }
        public List<HIS_SERVICE_REQ_MATY> _SerMatys { get; set; }
        public List<HIS_SERVICE_REQ_METY> _SerMetys { get; set; }
        public SingleKeys _SingleKeys { get; set; }
    }

    public class Mps000116ADO
    {
        public long MEDI_MATY_TYPE_ID { get; set; }
        public string MEDI_MATY_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal? VIR_PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public string DESCRIPTION { get; set; }
        public long TypeId { get; set; }
        public string TYPE_NAME { get; set; }
        public string VIR_PRICE_STR { get; set; }
        public string TOTAL_PRICE_STR { get; set; }
        public string MEDICAL_INSTRUCTION { get; set; }
        public decimal? SPEED { get; set; }
        public string MORNING { get; set; }
        public string AFTERNOON { get; set; }
        public string EVENING { get; set; }
        public string NOON { get; set; }
        public string CONCENTRA { get; set; }
        public decimal? VAT_RATIO { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public decimal? MEDICINE_GROUP_NUM_ORDER { get; set; }
        public decimal? MEDICINE_USE_FORM_NUM_ORDER { get; set; }
        public decimal? NUM_ORDER { get; set; }
        public short? MORNING_IS_USED { get; set; }
        public short? NOON_IS_USED { get; set; }
        public short? AFTERNOON_IS_USED { get; set; }
        public short? EVENING_IS_USED { get; set; }
        /// <summary>Cach dung (duong dung) cua y lenh — lay tu HTU_TEXT.</summary>
        public string HTU_TEXT { get; set; }

        public Mps000116ADO() { }

        public Mps000116ADO(List<V_HIS_EXP_MEST_MEDICINE> datas, string _MedicalInstruction)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    this.MEDI_MATY_TYPE_NAME = datas[0].MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = datas[0].SERVICE_UNIT_NAME;
                    this.MEDI_MATY_TYPE_ID = datas[0].MEDICINE_TYPE_ID;
                    this.TUTORIAL = datas[0].TUTORIAL;
                    this.AMOUNT = datas.Sum(p => (p.AMOUNT - (p.TH_AMOUNT ?? 0)));
                    this.VIR_PRICE = datas[0].PRICE;
                    this.TOTAL_PRICE = this.AMOUNT * this.VIR_PRICE;
                    this.DESCRIPTION = datas[0].DESCRIPTION;
                    this.SPEED = datas[0].SPEED;
                    this.TypeId = 1;
                    this.TYPE_NAME = "Lĩnh ở kho";
                    this.TOTAL_PRICE_STR = this.ConvertNumberToString(this.TOTAL_PRICE ?? 0);
                    this.VIR_PRICE_STR = this.ConvertNumberToString(this.VIR_PRICE ?? 0);
                    this.MEDICAL_INSTRUCTION = _MedicalInstruction;
                    this.MORNING = datas[0].MORNING;
                    this.NOON = datas[0].NOON;
                    this.AFTERNOON = datas[0].AFTERNOON;
                    this.EVENING = datas[0].EVENING;
                    this.CONCENTRA = datas[0].CONCENTRA;
                    this.MORNING_IS_USED = datas[0].MORNING_IS_USED;
                    this.NOON_IS_USED = datas[0].NOON_IS_USED;
                    this.AFTERNOON_IS_USED = datas[0].AFTERNOON_IS_USED;
                    this.EVENING_IS_USED = datas[0].EVENING_IS_USED;
                    this.VAT_RATIO = datas[0].VAT_RATIO;
                    this.MEDICINE_GROUP_NUM_ORDER = datas[0].MEDICINE_GROUP_NUM_ORDER;
                    this.MEDICINE_USE_FORM_NUM_ORDER = datas[0].MEDICINE_USE_FORM_NUM_ORDER;
                    this.NUM_ORDER = datas[0].NUM_ORDER;
                    this.HTU_TEXT = !String.IsNullOrEmpty(datas[0].HTU_TEXT) ? datas[0].HTU_TEXT : datas[0].HTU_NAME;
                    this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000116ADO(List<V_HIS_EXP_MEST_MATERIAL> datas)
        {
            try
            {
                if (datas != null && datas.Count > 0)
                {
                    this.MEDI_MATY_TYPE_NAME = datas[0].MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = datas[0].SERVICE_UNIT_NAME;
                    this.MEDI_MATY_TYPE_ID = datas[0].MATERIAL_TYPE_ID;
                    this.AMOUNT = datas.Sum(p => (p.AMOUNT - (p.TH_AMOUNT ?? 0)));
                    this.VIR_PRICE = datas[0].PRICE;
                    this.TOTAL_PRICE = this.AMOUNT * this.VIR_PRICE;
                    this.DESCRIPTION = datas[0].DESCRIPTION;
                    this.SPEED = null;
                    this.TypeId = 2;
                    this.TYPE_NAME = "Lĩnh ở kho";
                    this.TOTAL_PRICE_STR = this.ConvertNumberToString(this.TOTAL_PRICE ?? 0);
                    this.VIR_PRICE_STR = this.ConvertNumberToString(this.VIR_PRICE ?? 0);
                    this.VAT_RATIO = datas[0].VAT_RATIO;
                    this.NUM_ORDER = datas[0].NUM_ORDER;
                    this.TUTORIAL = datas[0].TUTORIAL;
                    this.HTU_TEXT = datas[0].HTU_TEXT;
                    this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                }
                else
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG V_HIS_EXP_MEST_MATERIAL");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000116ADO(List<HIS_SERVICE_REQ_MATY> datas, List<V_HIS_MATERIAL_TYPE> _materialTypes)
        {
            try
            {
                if (datas != null && datas.Count > 0 && _materialTypes != null && _materialTypes.Count > 0)
                {
                    var mateType = _materialTypes.FirstOrDefault(p => p.ID == datas[0].MATERIAL_TYPE_ID);
                    if (mateType != null)
                    {
                        this.MEDI_MATY_TYPE_NAME = mateType.MATERIAL_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = mateType.SERVICE_UNIT_NAME;
                        this.CONCENTRA = mateType.CONCENTRA;
                    }
                    this.MEDI_MATY_TYPE_ID = datas[0].MATERIAL_TYPE_ID ?? 0;
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
                    this.TypeId = 2;
                    this.TYPE_NAME = "Tự mua";
                    this.VIR_PRICE = datas.Sum(o => o.PRICE ?? 0);
                    this.TOTAL_PRICE = datas.Sum(o => o.AMOUNT * (o.PRICE ?? 0));
                    this.TOTAL_PRICE_STR = this.ConvertNumberToString(this.TOTAL_PRICE ?? 0);
                    this.VIR_PRICE_STR = this.ConvertNumberToString(this.VIR_PRICE ?? 0);
                    this.NUM_ORDER = datas[0].NUM_ORDER;
                    this.TUTORIAL = datas[0].TUTORIAL;
                    this.HTU_TEXT = datas[0].HTU_TEXT;
                    this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                }
                else
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG HIS_SERVICE_REQ_MATY, V_HIS_MATERIAL_TYPE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000116ADO(List<HIS_SERVICE_REQ_METY> datas, List<V_HIS_MEDICINE_TYPE> _medicineTypes)
        {
            try
            {
                //Ktra k co MEDICINE_TYPE_ID thỳ thuốc tự túc, thuốc khác
                if (datas != null && datas.Count > 0 && _medicineTypes != null && _medicineTypes.Count > 0)
                {
                    if (datas[0].MEDICINE_TYPE_ID != null)
                    {
                        var mediType = _medicineTypes.FirstOrDefault(p => p.ID == datas[0].MEDICINE_TYPE_ID);
                        if (mediType != null)
                        {
                            this.SERVICE_UNIT_NAME = mediType.SERVICE_UNIT_NAME;
                            this.CONCENTRA = mediType.CONCENTRA;
                            this.MEDICINE_GROUP_NUM_ORDER = mediType.MEDICINE_GROUP_NUM_ORDER;
                            this.MEDICINE_USE_FORM_NUM_ORDER = mediType.MEDICINE_USE_FORM_NUM_ORDER;
                        }
                        this.MEDI_MATY_TYPE_ID = datas[0].MEDICINE_TYPE_ID ?? 0;
                    }
                    this.MEDI_MATY_TYPE_NAME = datas[0].MEDICINE_TYPE_NAME;
                    this.AMOUNT = datas.Sum(p => p.AMOUNT);
                    this.TUTORIAL = datas[0].TUTORIAL;
                    this.SPEED = datas[0].SPEED;
                    this.TypeId = 1;
                    this.TYPE_NAME = "Tự mua";
                    this.VIR_PRICE = datas.Sum(o => o.PRICE ?? 0);
                    this.TOTAL_PRICE= datas.Sum(o => o.AMOUNT * (o.PRICE ?? 0));
                    this.TOTAL_PRICE_STR = this.ConvertNumberToString(this.TOTAL_PRICE ?? 0);
                    this.VIR_PRICE_STR = this.ConvertNumberToString(this.VIR_PRICE ?? 0);
                    this.MORNING = datas[0].MORNING;
                    this.NOON = datas[0].NOON;
                    this.AFTERNOON = datas[0].AFTERNOON;
                    this.EVENING = datas[0].EVENING;
                    this.NUM_ORDER = datas[0].NUM_ORDER;
                    this.HTU_TEXT = datas[0].HTU_TEXT;
                    this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                }
                else
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG HIS_SERVICE_REQ_METY, V_HIS_MEDICINE_TYPE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }

    /// <summary>
    /// Chi tiet TUNG y lenh thuoc/vat tu trong ngay — KHONG gop theo loai thuoc nhu Mps000116ADO.
    /// 1 thuoc ke nhieu lan trong ngay se sinh nhieu ban ghi, moi ban ghi giu dung
    /// so luong, lieu dung, cach dung, toc do truyen cua chinh y lenh do.
    /// </summary>
    public class Mps000116DetailADO
    {
        /// <summary>Ma loai thuoc (TypeId = 1) hoac ma loai vat tu (TypeId = 2)</summary>
        public long MEDI_MATY_TYPE_ID { get; set; }
        /// <summary>Ten thuoc / ten vat tu</summary>
        public string MEDI_MATY_TYPE_NAME { get; set; }
        /// <summary>Don vi tinh</summary>
        public string SERVICE_UNIT_NAME { get; set; }
        /// <summary>Nong do, ham luong</summary>
        public string CONCENTRA { get; set; }
        /// <summary>Toc do truyen dich, truyen mau. Vat tu luon null</summary>
        public decimal? SPEED { get; set; }
        /// <summary>So luong cua rieng y lenh nay (khong cong don)</summary>
        public decimal AMOUNT { get; set; }
        /// <summary>Lieu dung — huong dan su dung</summary>
        public string TUTORIAL { get; set; }
        /// <summary>Cach dung — duong dung cua rieng y lenh nay</summary>
        public string HTU_TEXT { get; set; }
        /// <summary>Lieu sang</summary>
        public string MORNING { get; set; }
        /// <summary>Lieu trua</summary>
        public string NOON { get; set; }
        /// <summary>Lieu chieu</summary>
        public string AFTERNOON { get; set; }
        /// <summary>Lieu toi</summary>
        public string EVENING { get; set; }
        /// <summary>Thoi gian y lenh dang so yyyyMMddHHmmss — dung de sap xep</summary>
        public long? INTRUCTION_TIME { get; set; }
        /// <summary>Thoi gian y lenh dang chuoi hien thi</summary>
        public string INTRUCTION_TIME_STR { get; set; }
        /// <summary>1: thuoc, 2: vat tu — noi quan he voi band Type tren mau in</summary>
        public long TypeId { get; set; }
        /// <summary>Nguon cap: Linh o kho / Tu mua</summary>
        public string TYPE_NAME { get; set; }
        /// <summary>Ghi chu cua y lenh. Thuoc/vat tu tu mua khong luu truong nay nen luon null.</summary>
        public string DESCRIPTION { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public decimal? NUM_ORDER { get; set; }
        public decimal? MEDICINE_GROUP_NUM_ORDER { get; set; }
        public decimal? MEDICINE_USE_FORM_NUM_ORDER { get; set; }

        public Mps000116DetailADO() { }

        /// <summary>Y lenh thuoc linh o kho</summary>
        public Mps000116DetailADO(V_HIS_EXP_MEST_MEDICINE data)
        {
            try
            {
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG V_HIS_EXP_MEST_MEDICINE (Mps000116DetailADO)");
                    return;
                }
                this.MEDI_MATY_TYPE_ID = data.MEDICINE_TYPE_ID;
                this.MEDI_MATY_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.CONCENTRA = data.CONCENTRA;
                this.SPEED = data.SPEED;
                this.AMOUNT = data.AMOUNT - (data.TH_AMOUNT ?? 0);
                this.TUTORIAL = data.TUTORIAL;
                this.HTU_TEXT = !String.IsNullOrEmpty(data.HTU_TEXT) ? data.HTU_TEXT : data.HTU_NAME;
                this.MORNING = data.MORNING;
                this.NOON = data.NOON;
                this.AFTERNOON = data.AFTERNOON;
                this.EVENING = data.EVENING;
                this.NUM_ORDER = data.NUM_ORDER;
                this.MEDICINE_GROUP_NUM_ORDER = data.MEDICINE_GROUP_NUM_ORDER;
                this.MEDICINE_USE_FORM_NUM_ORDER = data.MEDICINE_USE_FORM_NUM_ORDER;
                this.DESCRIPTION = data.DESCRIPTION;
                this.TypeId = 1;
                this.TYPE_NAME = "Lĩnh ở kho";
                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                this.SetIntructionTime(data.TDL_INTRUCTION_TIME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Y lenh vat tu linh o kho.
        /// Nong do/ham luong lay tu danh muc vat tu vi view xuat vat tu khong tra ve truong nay.
        /// </summary>
        public Mps000116DetailADO(V_HIS_EXP_MEST_MATERIAL data, V_HIS_MATERIAL_TYPE materialType)
        {
            try
            {
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG V_HIS_EXP_MEST_MATERIAL (Mps000116DetailADO)");
                    return;
                }
                this.MEDI_MATY_TYPE_ID = data.MATERIAL_TYPE_ID;
                this.MEDI_MATY_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                this.CONCENTRA = materialType != null ? materialType.CONCENTRA : null;
                this.SPEED = null;
                this.AMOUNT = data.AMOUNT - (data.TH_AMOUNT ?? 0);
                this.TUTORIAL = data.TUTORIAL;
                this.HTU_TEXT = data.HTU_TEXT;
                this.NUM_ORDER = data.NUM_ORDER;
                this.DESCRIPTION = data.DESCRIPTION;
                this.TypeId = 2;
                this.TYPE_NAME = "Lĩnh ở kho";
                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                this.SetIntructionTime(data.TDL_INTRUCTION_TIME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Y lenh thuoc tu mua (ngoai kho).
        /// Don vi tinh + nong do/ham luong lay tu danh muc thuoc; thuoc ngoai danh muc
        /// (khong co MEDICINE_TYPE_ID) dung UNIT_NAME nguoi ke da nhap.
        /// Thoi gian y lenh lay tu yeu cau dich vu vi ban ghi chi tiet khong luu.
        /// </summary>
        public Mps000116DetailADO(HIS_SERVICE_REQ_METY data, V_HIS_MEDICINE_TYPE medicineType, long? intructionTime)
        {
            try
            {
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG HIS_SERVICE_REQ_METY (Mps000116DetailADO)");
                    return;
                }
                this.MEDI_MATY_TYPE_ID = data.MEDICINE_TYPE_ID ?? 0;
                this.MEDI_MATY_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                this.SERVICE_UNIT_NAME = (medicineType != null && !String.IsNullOrEmpty(medicineType.SERVICE_UNIT_NAME))
                    ? medicineType.SERVICE_UNIT_NAME
                    : data.UNIT_NAME;
                this.CONCENTRA = medicineType != null ? medicineType.CONCENTRA : null;
                this.SPEED = data.SPEED;
                this.AMOUNT = data.AMOUNT;
                this.TUTORIAL = data.TUTORIAL;
                this.HTU_TEXT = data.HTU_TEXT;
                this.MORNING = data.MORNING;
                this.NOON = data.NOON;
                this.AFTERNOON = data.AFTERNOON;
                this.EVENING = data.EVENING;
                this.NUM_ORDER = data.NUM_ORDER;
                this.MEDICINE_GROUP_NUM_ORDER = medicineType != null ? medicineType.MEDICINE_GROUP_NUM_ORDER : null;
                this.MEDICINE_USE_FORM_NUM_ORDER = medicineType != null ? medicineType.MEDICINE_USE_FORM_NUM_ORDER : null;
                this.TypeId = 1;
                this.TYPE_NAME = "Tự mua";
                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                this.SetIntructionTime(intructionTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Y lenh vat tu tu mua (ngoai kho).
        /// Don vi tinh + nong do/ham luong lay tu danh muc vat tu, fallback UNIT_NAME nguoi ke da nhap.
        /// Thoi gian y lenh lay tu yeu cau dich vu vi ban ghi chi tiet khong luu.
        /// </summary>
        public Mps000116DetailADO(HIS_SERVICE_REQ_MATY data, V_HIS_MATERIAL_TYPE materialType, long? intructionTime)
        {
            try
            {
                if (data == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("DU LIEU RONG HIS_SERVICE_REQ_MATY (Mps000116DetailADO)");
                    return;
                }
                this.MEDI_MATY_TYPE_ID = data.MATERIAL_TYPE_ID ?? 0;
                this.MEDI_MATY_TYPE_NAME = (materialType != null && !String.IsNullOrEmpty(materialType.MATERIAL_TYPE_NAME))
                    ? materialType.MATERIAL_TYPE_NAME
                    : data.MATERIAL_TYPE_NAME;
                this.SERVICE_UNIT_NAME = (materialType != null && !String.IsNullOrEmpty(materialType.SERVICE_UNIT_NAME))
                    ? materialType.SERVICE_UNIT_NAME
                    : data.UNIT_NAME;
                this.CONCENTRA = materialType != null ? materialType.CONCENTRA : null;
                this.SPEED = null;
                this.AMOUNT = data.AMOUNT;
                this.TUTORIAL = data.TUTORIAL;
                this.HTU_TEXT = data.HTU_TEXT;
                this.NUM_ORDER = data.NUM_ORDER;
                this.TypeId = 2;
                this.TYPE_NAME = "Tự mua";
                this.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                this.SetIntructionTime(intructionTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIntructionTime(long? intructionTime)
        {
            try
            {
                this.INTRUCTION_TIME = intructionTime;
                this.INTRUCTION_TIME_STR = (intructionTime ?? 0) > 0
                    ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime ?? 0)
                    : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.INTRUCTION_TIME_STR = "";
            }
        }
    }

    public class SingleKeys
    {
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string LOGIN_NAME { get; set; }
        public int IsOderMedicine { get; set; }
    }

    public enum IsKeyCheck
    {
        TongHop,
        Thuoc,
        VatTu
    }
}
