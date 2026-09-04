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
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000217.PDO
{
    public partial class Mps000217PDO : RDOBase
    {
        public SingKey217 _SingKey217 { get; set; }
        public List<Mrs00067RDO> _Mrs00067RDOs { get; set; }
        public List<Mrs00067RDO> _ListMedicines { get; set; }
        public List<Mrs00067RDO> _MedicineByPakages { get; set; }

        public Mps000217PDO() { }

        public Mps000217PDO(
            List<Mrs00067RDO> _mrs00067RDOs,
            List<Mrs00067RDO> _listMedicines,
            SingKey217 _singKey217
            )
        {
            this._Mrs00067RDOs = _mrs00067RDOs;
            this._ListMedicines = _listMedicines;
            this._SingKey217 = _singKey217;
        }

        public Mps000217PDO(
            List<Mrs00067RDO> _mrs00067RDOs,
            List<Mrs00067RDO> _listMedicines,
            List<Mrs00067RDO> lstMedicineByPackage,
            SingKey217 _singKey217
            )
        {
            this._Mrs00067RDOs = _mrs00067RDOs;
            this._ListMedicines = _listMedicines;
            this._MedicineByPakages = lstMedicineByPackage;
            this._SingKey217 = _singKey217;
        }
    }

    public class SingKey217
    {
        public string TIME_TO_STR { get; set; }
        public string TIME_FROM_STR { get; set; }
        public string BEGIN_AMOUNT { get; set; }
        public string END_AMOUNT { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string MEDI_BEGIN_AMOUNT { get; set; }
        public string MEDI_END_AMOUNT { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }

        public string CONCENTRA { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public Dictionary<string, object> DIC_OTHER_KEY { get; set; }
    }

    public class Mrs00067RDO
    {
        public long? EXECUTE_TIME { get; set; }
        public string EXECUTE_DATE_STR { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string DESCRIPTION_DETAIL { get; set; }
        public string IMP_MEST_TYPE_NAME { get; set; }
        public string EXP_MEST_TYPE_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public long MEDICINE_ID { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal BEGIN_AMOUNT { get; set; }
        public decimal? IMP_AMOUNT { get; set; }
        public decimal? EXP_AMOUNT { get; set; }
        public decimal END_AMOUNT { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string REQ_ROOM_NAME { get; set; }
        public string REQ_DEPARTMENT_NAME { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string EXP_MEDI_STOCK_CODE { get; set; }
        public string EXP_MEDI_STOCK_NAME { get; set; }
        public string IMP_MEDI_STOCK_CODE { get; set; }
        public string IMP_MEDI_STOCK_NAME { get; set; }
        public string CLIENT_NAME { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string SECOND_MEST_CODE { get; set; }
        public string VIR_PATIENT_ADDRESS { get; set; }

        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }

        public decimal? IMP_AMOUNT_KHONG_GOM_HOAN { get; set; }
        public decimal? IMP_AMOUNT_HOAN { get; set; }
        public decimal? CHMS_TYPE_ID { get; set; }
        public string DOCUMENT_NUMBER { get; set; }

        #region Noi dung dien giai chi tiet (kho nguon - kho dich, benh nhan)
        /// <summary>
        /// Ten kho/tu truc xuat hang, da kem tien to "kho"/"tu truc".
        /// Chi co gia tri voi phieu chuyen kho.
        /// </summary>
        public string FROM_MEDI_STOCK_NAME { get; set; }

        /// <summary>
        /// Ten kho/tu truc nhan hang, da kem tien to "kho"/"tu truc".
        /// Chi co gia tri voi phieu chuyen kho.
        /// </summary>
        public string TO_MEDI_STOCK_NAME { get; set; }

        /// <summary>Ma dieu tri gan voi phieu - lay cho MOI loai phieu nhap/xuat</summary>
        public string TDL_TREATMENT_CODE { get; set; }

        /// <summary>Ma benh nhan gan voi phieu - lay cho MOI loai phieu nhap/xuat</summary>
        public string TDL_PATIENT_CODE { get; set; }

        /// <summary>Ten benh nhan gan voi phieu - lay cho MOI loai phieu nhap/xuat</summary>
        public string TDL_PATIENT_NAME { get; set; }

        /// <summary>
        /// ID dieu tri lay tu dong chi tiet xuat kho - dung de tra cuu ma dieu tri, ma benh nhan
        /// khi phieu xuat khong luu san cac truong TDL_ (vi du phieu tong hop phong kham).
        /// </summary>
        public long? TDL_TREATMENT_ID { get; set; }

        /// <summary>ID benh nhan lay tu dong chi tiet xuat kho</summary>
        public long? TDL_PATIENT_ID { get; set; }

        /// <summary>
        /// Noi dung dien giai day du dung cho cot "Dien giai" cua the kho.
        /// - Chuyen kho: ghi ro xuat tu kho nao sang tu truc nao (nhap tu kho nao vao tu truc nao).
        /// - Xuat/nhap cho benh nhan: ghi ro ma benh nhan, ma dieu tri, ten benh nhan.
        /// </summary>
        private string content;
        public string CONTENT
        {
            get { return !string.IsNullOrEmpty(content) ? content : BuildContent(); }
            set { content = value; }
        }
        #endregion

        public Mrs00067RDO(
            V_HIS_IMP_MEST_MEDICINE imp,
            List<V_HIS_IMP_MEST> Listimp,
            List<V_HIS_EXP_MEST> sourceMest,
            List<HIS_IMP_MEST_TYPE> impMestTypes,
            List<HIS_DEPARTMENT> _Departments,
            List<V_HIS_ROOM> _Rooms)
        {
            try
            {
                EXECUTE_TIME = imp.IMP_TIME;
                PACKAGE_NUMBER = imp.PACKAGE_NUMBER;
                IMP_MEST_CODE = imp.IMP_MEST_CODE;
                EXPIRED_DATE = imp.EXPIRED_DATE;
                IMP_AMOUNT =    imp.AMOUNT;
                CHMS_TYPE_ID = imp.CHMS_TYPE_ID;
                DOCUMENT_NUMBER = imp.DOCUMENT_NUMBER;
               

                if (imp.CHMS_TYPE_ID == 2)
                {
                    IMP_AMOUNT_HOAN = imp.AMOUNT;
                }
                else
                {
                    IMP_AMOUNT_KHONG_GOM_HOAN = imp.AMOUNT;
                }
                if (imp.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    SUPPLIER_NAME = imp.SUPPLIER_NAME;
                    SUPPLIER_ID = imp.SUPPLIER_ID;
                    SUPPLIER_CODE = imp.SUPPLIER_CODE;
                //thong tin phieu nhap
                V_HIS_IMP_MEST ImpMest = Listimp.FirstOrDefault(o => o.ID == imp.IMP_MEST_ID);
                if (ImpMest != null)
                {
                    DESCRIPTION = ImpMest.DESCRIPTION;
                }

                V_HIS_IMP_MEST chmsImpMest = Listimp != null && imp.IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK ? Listimp.Where(o => o.ID == imp.IMP_MEST_ID).FirstOrDefault() : null;
                if (chmsImpMest != null)
                {
                    V_HIS_EXP_MEST chmsExpMest = sourceMest != null ? sourceMest.Where(o => o.ID == chmsImpMest.CHMS_EXP_MEST_ID).FirstOrDefault() : null;
                    if (chmsExpMest != null)
                    {
                        this.EXP_MEDI_STOCK_CODE = chmsExpMest.MEDI_STOCK_CODE;
                        this.EXP_MEDI_STOCK_NAME = chmsExpMest.MEDI_STOCK_NAME;
                        this.SECOND_MEST_CODE = chmsExpMest.EXP_MEST_CODE;
                    }
                }

                var data = impMestTypes != null ? impMestTypes.Where(o => o.ID == imp.IMP_MEST_TYPE_ID).FirstOrDefault() : null;
                if (data != null)
                {
                    IMP_MEST_TYPE_NAME = data.IMP_MEST_TYPE_NAME;
                }

                if (imp.AGGR_IMP_MEST_ID.HasValue)
                {
                    var firstimp = Listimp.FirstOrDefault(o => o.ID == imp.AGGR_IMP_MEST_ID);
                    if (firstimp != null)
                    {
                        data = impMestTypes != null ? impMestTypes.Where(o => o.ID == firstimp.IMP_MEST_TYPE_ID).FirstOrDefault() : null;
                        IMP_MEST_CODE = firstimp.IMP_MEST_CODE;
                        VIR_PATIENT_NAME = "";
                        TREATMENT_CODE = "";
                        VIR_PATIENT_ADDRESS = "";

                        if (data != null)
                        {
                            IMP_MEST_TYPE_NAME = data.IMP_MEST_TYPE_NAME;

                            if (firstimp.REQ_DEPARTMENT_ID.HasValue && _Departments != null)
                            {
                                var department = _Departments.FirstOrDefault(o => o.ID == firstimp.REQ_DEPARTMENT_ID.Value);
                                if (department != null)
                                {
                                    REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                                }
                            }
                        }
                    }
                }

                if (imp.REQ_ROOM_ID.HasValue && _Rooms != null)
                {
                    var room = _Rooms.FirstOrDefault(o => o.ID == imp.REQ_ROOM_ID.Value);
                    if (room != null)
                    {
                        REQ_ROOM_NAME = MEDI_STOCK_NAME = room.ROOM_NAME;
                    }
                }

                if (imp.REQ_DEPARTMENT_ID.HasValue && _Departments != null)
                {
                    var department = _Departments.FirstOrDefault(o => o.ID == imp.REQ_DEPARTMENT_ID.Value);
                    if (department != null)
                    {
                        REQ_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }
                }

                SetExtendField(this);

                this.IMP_PRICE = imp.IMP_PRICE;
                this.IMP_VAT_RATIO = imp.IMP_VAT_RATIO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Overload bo sung danh sach kho de xac dinh ro kho/tu truc nguon - dich
        /// va thong tin benh nhan cua phieu nhap.
        /// </summary>
        public Mrs00067RDO(
            V_HIS_IMP_MEST_MEDICINE imp,
            List<V_HIS_IMP_MEST> Listimp,
            List<V_HIS_EXP_MEST> sourceMest,
            List<HIS_IMP_MEST_TYPE> impMestTypes,
            List<HIS_DEPARTMENT> _Departments,
            List<V_HIS_ROOM> _Rooms,
            List<HIS_MEDI_STOCK> _MediStocks)
            : this(imp, Listimp, sourceMest, impMestTypes, _Departments, _Rooms)
        {
            try
            {
                V_HIS_IMP_MEST impMest = Listimp != null ? Listimp.FirstOrDefault(o => o.ID == imp.IMP_MEST_ID) : null;
                if (impMest == null) return;

                //Chuyen kho: kho xuat lay tu phieu xuat nguon, kho nhap la kho dang in the kho
                if (impMest.CHMS_EXP_MEST_ID.HasValue && sourceMest != null)
                {
                    V_HIS_EXP_MEST sourceExpMest = sourceMest.FirstOrDefault(o => o.ID == impMest.CHMS_EXP_MEST_ID.Value);
                    if (sourceExpMest != null)
                    {
                        FROM_MEDI_STOCK_NAME = MakeMediStockLabel(sourceExpMest.MEDI_STOCK_NAME, GetIsCabinet(_MediStocks, sourceExpMest.MEDI_STOCK_ID));
                        TO_MEDI_STOCK_NAME = MakeMediStockLabel(impMest.MEDI_STOCK_NAME, GetIsCabinet(_MediStocks, impMest.MEDI_STOCK_ID));
                    }
                }

                //Lay theo phieu con de the kho van hien thi duoc ma dieu tri, ma benh nhan
                TDL_TREATMENT_CODE = impMest.TDL_TREATMENT_CODE;
                TDL_PATIENT_CODE = impMest.TDL_PATIENT_CODE;
                TDL_PATIENT_NAME = impMest.TDL_PATIENT_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mrs00067RDO(
            V_HIS_EXP_MEST_MEDICINE exp,
            List<V_HIS_EXP_MEST> Listexp,
            List<V_HIS_IMP_MEST> destMest,
            List<HIS_EXP_MEST_TYPE> expMestTypes,
            List<HIS_MEDI_STOCK> _MediStocks,
            List<HIS_DEPARTMENT> _Departments,
            List<V_HIS_ROOM> _Rooms)
        {
            try
            {
                EXECUTE_TIME = exp.EXP_TIME;
                PACKAGE_NUMBER = exp.PACKAGE_NUMBER;
                EXPIRED_DATE = exp.EXPIRED_DATE;
                EXP_AMOUNT = exp.AMOUNT;
                EXP_MEST_CODE = exp.EXP_MEST_CODE;
                DESCRIPTION_DETAIL = exp.DESCRIPTION;
                TDL_TREATMENT_ID = exp.TDL_TREATMENT_ID;
                TDL_PATIENT_ID = exp.TDL_PATIENT_ID;
                SUPPLIER_CODE = exp.SUPPLIER_CODE;
                SUPPLIER_ID = exp.SUPPLIER_ID;
                SUPPLIER_NAME = exp.SUPPLIER_NAME;
                //thong tin phieu xuất
                V_HIS_EXP_MEST ExpMest = Listexp.FirstOrDefault(o => o.ID == exp.EXP_MEST_ID);
                if (ExpMest != null)
                {
                    DESCRIPTION = ExpMest.DESCRIPTION;

                    //Thong tin benh nhan lay cho MOI loai phieu xuat (khong chi don thuoc/ban le)
                    TDL_TREATMENT_CODE = ExpMest.TDL_TREATMENT_CODE;
                    TDL_PATIENT_CODE = ExpMest.TDL_PATIENT_CODE;
                    TDL_PATIENT_NAME = ExpMest.TDL_PATIENT_NAME;
                }


                V_HIS_EXP_MEST chmsExpMest = Listexp != null && exp.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK ? Listexp.FirstOrDefault(o => o.ID == exp.EXP_MEST_ID) : null;

                if (chmsExpMest != null)
                {
                    V_HIS_IMP_MEST chmsImpMest = destMest != null ? destMest.Where(o => o.CHMS_EXP_MEST_ID == chmsExpMest.ID).FirstOrDefault() : null;
                    HIS_MEDI_STOCK impMediStock = _MediStocks != null && chmsExpMest.IMP_MEDI_STOCK_ID.HasValue
                        ? _MediStocks.FirstOrDefault(o => o.ID == chmsExpMest.IMP_MEDI_STOCK_ID.Value) : null;
                    this.IMP_MEDI_STOCK_CODE = impMediStock != null ? impMediStock.MEDI_STOCK_CODE : "";
                    this.IMP_MEDI_STOCK_NAME = impMediStock != null ? impMediStock.MEDI_STOCK_NAME : "";
                    this.SECOND_MEST_CODE = chmsImpMest != null ? chmsImpMest.IMP_MEST_CODE : "";
                    CHMS_TYPE_ID = chmsImpMest != null ? chmsImpMest.CHMS_TYPE_ID:null;

                    //Ghi ro xuat tu kho nao sang tu truc nao
                    this.FROM_MEDI_STOCK_NAME = MakeMediStockLabel(chmsExpMest.MEDI_STOCK_NAME, GetIsCabinet(_MediStocks, chmsExpMest.MEDI_STOCK_ID));
                    this.TO_MEDI_STOCK_NAME = impMediStock != null ? MakeMediStockLabel(impMediStock.MEDI_STOCK_NAME, impMediStock.IS_CABINET) : "";
                }



                V_HIS_EXP_MEST saleExpMest = Listexp != null && exp.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN ? Listexp.FirstOrDefault(o => o.ID == exp.EXP_MEST_ID) : null;
                if (saleExpMest != null)
                {
                    this.CLIENT_NAME = saleExpMest.TDL_PATIENT_NAME;
                    this.TREATMENT_CODE = saleExpMest.TDL_TREATMENT_CODE;
                    this.VIR_PATIENT_NAME = saleExpMest.TDL_PATIENT_NAME;
                    this.VIR_PATIENT_ADDRESS = saleExpMest.TDL_PATIENT_ADDRESS;
                }

                V_HIS_EXP_MEST prescription = Listexp != null && (exp.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                    || exp.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                    || exp.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                    ? Listexp.FirstOrDefault(o => o.ID == exp.EXP_MEST_ID) : null;
                if (prescription != null)
                {
                    this.TREATMENT_CODE = prescription.TDL_TREATMENT_CODE;
                    this.VIR_PATIENT_NAME = prescription.TDL_PATIENT_NAME;
                    this.VIR_PATIENT_ADDRESS = prescription.TDL_PATIENT_ADDRESS;
                }

                var data = expMestTypes != null ? expMestTypes.FirstOrDefault(o => o.ID == exp.EXP_MEST_TYPE_ID) : null;
                if (data != null)
                {
                    EXP_MEST_TYPE_NAME = data.EXP_MEST_TYPE_NAME;
                }

                if (exp.AGGR_EXP_MEST_ID.HasValue)
                {
                    var firstexp = Listexp.FirstOrDefault(o => o.ID == exp.AGGR_EXP_MEST_ID);
                    if (firstexp != null)
                    {
                        data = expMestTypes != null ? expMestTypes.FirstOrDefault(o => o.ID == firstexp.EXP_MEST_TYPE_ID) : null;
                        EXP_MEST_CODE = firstexp.EXP_MEST_CODE;
                        this.VIR_PATIENT_NAME = "";
                        this.TREATMENT_CODE = "";
                        this.VIR_PATIENT_ADDRESS = "";

                        //TDL_TREATMENT_CODE/TDL_PATIENT_CODE giu nguyen theo phieu con
                        //de the kho van hien thi duoc ma dieu tri, ma benh nhan cua phieu tong hop

                        if (data != null)
                        {
                            EXP_MEST_TYPE_NAME = data.EXP_MEST_TYPE_NAME;

                            if (firstexp.REQ_DEPARTMENT_ID > 0 && _Departments != null)
                            {
                                var department = _Departments.FirstOrDefault(o => o.ID == firstexp.REQ_DEPARTMENT_ID);
                                if (department != null)
                                {
                                    REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                                }
                            }
                        }
                    }
                }

                if (exp.REQ_ROOM_ID > 0 && _Rooms != null)
                {
                    var room = _Rooms.FirstOrDefault(o => o.ID == exp.REQ_ROOM_ID);
                    if (room != null)
                    {
                        REQ_ROOM_NAME = MEDI_STOCK_NAME = room.ROOM_NAME;
                    }
                }

                if (exp.REQ_DEPARTMENT_ID > 0 && _Departments != null)
                {
                    var department = _Departments.FirstOrDefault(o => o.ID == exp.REQ_DEPARTMENT_ID);
                    if (department != null)
                    {
                        REQ_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }
                    if (exp.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP) REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                }

                SetExtendField(this);

                this.IMP_PRICE = exp.IMP_PRICE;
                this.IMP_VAT_RATIO = exp.IMP_VAT_RATIO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ghep cac gia tri khac rong, khong trung lap - dung khi gom nhieu phieu con
        /// vao 1 dong the kho (phieu tong hop gom nhieu benh nhan).
        /// </summary>
        public static string JoinDistinct(IEnumerable<string> values)
        {
            string result = "";
            try
            {
                if (values == null) return result;
                result = string.Join(", ", values.Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Trim()).Distinct());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Tra ve gia tri duy nhat neu ca nhom chi co 1 gia tri, nguoc lai tra ve rong.
        /// Dung cho ten benh nhan - phieu tong hop nhieu benh nhan thi khong hien thi ten.
        /// </summary>
        public static string SingleOrEmpty(IEnumerable<string> values)
        {
            string result = "";
            try
            {
                if (values == null) return result;
                List<string> distinctValues = values.Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Trim()).Distinct().ToList();
                if (distinctValues.Count == 1) result = distinctValues[0];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Gan tien to "kho"/"tu truc" vao truoc ten kho de noi dung dien giai ro nghia.
        /// Neu ten kho da bat dau bang "kho"/"tu" thi giu nguyen, tranh lap tu.
        /// </summary>
        private static string MakeMediStockLabel(string mediStockName, short? isCabinet)
        {
            string result = "";
            try
            {
                if (string.IsNullOrWhiteSpace(mediStockName)) return result;

                result = mediStockName.Trim();
                //So sanh theo tu dau tien de khong nham "Khoa ..." voi "Kho ..."
                string firstWord = result.Split(' ')[0].ToLower();
                if (firstWord == "kho" || firstWord == "tủ" || firstWord == "tu")
                    return result;

                result = (isCabinet == 1 ? "tủ trực " : "kho ") + result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private static short? GetIsCabinet(List<HIS_MEDI_STOCK> mediStocks, long? mediStockId)
        {
            short? result = null;
            try
            {
                if (mediStocks == null || !mediStockId.HasValue) return result;

                HIS_MEDI_STOCK mediStock = mediStocks.FirstOrDefault(o => o.ID == mediStockId.Value);
                if (mediStock != null) result = mediStock.IS_CABINET;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Ghep noi dung dien giai: loai phieu - kho nguon/kho dich - NCC - khoa - benh nhan - phieu doi ung.
        /// </summary>
        private string BuildContent()
        {
            string result = "";
            try
            {
                bool isImport = !string.IsNullOrWhiteSpace(IMP_MEST_CODE);
                List<string> parts = new List<string>();

                string mestTypeName = isImport ? IMP_MEST_TYPE_NAME : EXP_MEST_TYPE_NAME;
                if (!string.IsNullOrWhiteSpace(mestTypeName)) parts.Add(mestTypeName.Trim());

                string stockContent = BuildStockContent(isImport);
                if (!string.IsNullOrWhiteSpace(stockContent)) parts.Add(stockContent);

                //NCC chi co y nghia voi phieu nhap - dong xuat khong hien thi de tranh nham lan
                if (isImport && !string.IsNullOrWhiteSpace(SUPPLIER_NAME)) parts.Add("NCC: " + SUPPLIER_NAME.Trim());

                if (!string.IsNullOrWhiteSpace(REQUEST_DEPARTMENT_NAME)) parts.Add("Khoa: " + REQUEST_DEPARTMENT_NAME.Trim());

                string patientContent = BuildPatientContent();
                if (!string.IsNullOrWhiteSpace(patientContent)) parts.Add(patientContent);

                if (!string.IsNullOrWhiteSpace(SECOND_MEST_CODE)) parts.Add("Phiếu: " + SECOND_MEST_CODE.Trim());

                result = string.Join(" - ", parts);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Noi dung kho nguon - kho dich: "Xuat tu kho A sang tu truc B" / "Nhap tu kho A vao tu truc B".
        /// </summary>
        private string BuildStockContent(bool isImport)
        {
            string result = "";
            try
            {
                bool hasFromStock = !string.IsNullOrWhiteSpace(FROM_MEDI_STOCK_NAME);
                bool hasToStock = !string.IsNullOrWhiteSpace(TO_MEDI_STOCK_NAME);
                if (!hasFromStock && !hasToStock) return result;

                string action = isImport ? "Nhập" : "Xuất";
                string toWord = isImport ? "vào" : "sang";

                if (hasFromStock && hasToStock)
                    result = string.Format("{0} từ {1} {2} {3}", action, FROM_MEDI_STOCK_NAME, toWord, TO_MEDI_STOCK_NAME);
                else if (hasFromStock)
                    result = string.Format("{0} từ {1}", action, FROM_MEDI_STOCK_NAME);
                else
                    result = string.Format("{0} {1} {2}", action, toWord, TO_MEDI_STOCK_NAME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Noi dung benh nhan: ma benh nhan, ma dieu tri, ten benh nhan.
        /// </summary>
        private string BuildPatientContent()
        {
            string result = "";
            try
            {
                string patientCode = TDL_PATIENT_CODE;
                string treatmentCode = !string.IsNullOrWhiteSpace(TDL_TREATMENT_CODE) ? TDL_TREATMENT_CODE : TREATMENT_CODE;
                string patientName = !string.IsNullOrWhiteSpace(TDL_PATIENT_NAME) ? TDL_PATIENT_NAME : VIR_PATIENT_NAME;
                if (string.IsNullOrWhiteSpace(patientName)) patientName = CLIENT_NAME;

                List<string> parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(patientCode)) parts.Add("Mã BN: " + patientCode.Trim());
                if (!string.IsNullOrWhiteSpace(treatmentCode)) parts.Add("Mã ĐT: " + treatmentCode.Trim());
                if (!string.IsNullOrWhiteSpace(patientName)) parts.Add(patientName.Trim());

                result = string.Join(" - ", parts);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetExtendField(Mrs00067RDO data)
        {
            EXECUTE_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXECUTE_TIME ?? 0);
            EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
        }

        public void CalculateAmount(decimal previousEndAmount)
        {
            try
            {
                BEGIN_AMOUNT = previousEndAmount;
                END_AMOUNT = BEGIN_AMOUNT + (IMP_AMOUNT.HasValue ? IMP_AMOUNT.Value : 0) - (EXP_AMOUNT.HasValue ? EXP_AMOUNT.Value : 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mrs00067RDO() { }
    }
}
