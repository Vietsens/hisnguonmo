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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000483.PDO
{
    public partial class Mps000483PDO : RDOBase
    {
        public SingKey483 _SingKey483 { get; set; }
        public List<Mps000483RDO> _Mps000483RDOs { get; set; }
        public List<Mps000483RDO> _ListBloods { get; set; }
        public List<Mps000483RDO> _BloodByPakages { get; set; }

        public Mps000483PDO() { }

        public Mps000483PDO(
            List<Mps000483RDO> _mps000483RDOs,
            List<Mps000483RDO> _listBloods,
            SingKey483 _singKey483
            )
        {
            this._Mps000483RDOs = _mps000483RDOs;
            this._ListBloods = _listBloods;
            this._SingKey483 = _singKey483;
        }

        public Mps000483PDO(
            List<Mps000483RDO> _mps000483RDOs,
            List<Mps000483RDO> _listBloods,
            List<Mps000483RDO> lstBloodByPackage,
            SingKey483 _singKey483
            )
        {
            this._Mps000483RDOs = _mps000483RDOs;
            this._ListBloods = _listBloods;
            this._BloodByPakages = lstBloodByPackage;
            this._SingKey483 = _singKey483;
        }
    }

    public class SingKey483
    {
        public string TIME_TO_STR { get; set; }
        public string TIME_FROM_STR { get; set; }
        public string BEGIN_AMOUNT { get; set; }
        public string END_AMOUNT { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
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

    public class Mps000483RDO
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
        public long BLOOD_ID { get; set; }
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
        public string SECOND_MEST_CODE { get; set; }
        public string VIR_PATIENT_ADDRESS { get; set; }

        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }

        public decimal? IMP_AMOUNT_KHONG_GOM_HOAN { get; set; }
        public decimal? IMP_AMOUNT_HOAN { get; set; }
        public decimal? CHMS_TYPE_ID { get; set; }
        public Mps000483RDO(
            List<V_HIS_IMP_MEST_BLOOD> imp,
            List<V_HIS_IMP_MEST> Listimp,
            List<V_HIS_EXP_MEST> sourceMest,
            List<HIS_IMP_MEST_TYPE> impMestTypes,
            List<HIS_DEPARTMENT> _Departments,
            List<V_HIS_ROOM> _Rooms)
        {
            try
            {
                EXECUTE_TIME = imp.First().IMP_TIME;
                PACKAGE_NUMBER = imp.First().PACKAGE_NUMBER;
                IMP_MEST_CODE = imp.First().IMP_MEST_CODE;
                EXPIRED_DATE = imp.First().EXPIRED_DATE;
                IMP_AMOUNT = imp.Count();

                if (imp.First().IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    SUPPLIER_NAME = imp.First().SUPPLIER_NAME;
                //thong tin phieu nhap
                V_HIS_IMP_MEST ImpMest = Listimp.FirstOrDefault(o => o.ID == imp.First().IMP_MEST_ID);
                if (ImpMest != null)
                {
                    DESCRIPTION = ImpMest.DESCRIPTION;
                }

                V_HIS_IMP_MEST chmsImpMest = Listimp != null && imp.First().IMP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__CK ? Listimp.Where(o => o.ID == imp.First().IMP_MEST_ID).FirstOrDefault() : null;
                if (chmsImpMest != null)
                {
                    V_HIS_EXP_MEST chmsExpMest = sourceMest != null ? sourceMest.Where(o => o.ID == chmsImpMest.CHMS_EXP_MEST_ID).FirstOrDefault() : null;
                    if (chmsExpMest != null)
                    {
                        this.EXP_MEDI_STOCK_CODE = chmsExpMest.MEDI_STOCK_CODE;
                        this.EXP_MEDI_STOCK_NAME = chmsExpMest.MEDI_STOCK_NAME;
                        this.SECOND_MEST_CODE = chmsExpMest.EXP_MEST_CODE;
                        CHMS_TYPE_ID = chmsImpMest != null ? chmsImpMest.CHMS_TYPE_ID : null;
                    }
                }

                var data = impMestTypes != null ? impMestTypes.Where(o => o.ID == imp.First().IMP_MEST_TYPE_ID).FirstOrDefault() : null;
                if (data != null)
                {
                    IMP_MEST_TYPE_NAME = data.IMP_MEST_TYPE_NAME;
                }

                if (imp.First().AGGR_IMP_MEST_ID.HasValue)
                {
                    var firstimp = Listimp.FirstOrDefault(o => o.ID == imp.First().AGGR_IMP_MEST_ID);
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

                if (imp.First().REQ_ROOM_ID.HasValue && _Rooms != null)
                {
                    var room = _Rooms.FirstOrDefault(o => o.ID == imp.First().REQ_ROOM_ID.Value);
                    if (room != null)
                    {
                        REQ_ROOM_NAME = MEDI_STOCK_NAME = room.ROOM_NAME;
                    }
                }

                if (imp.First().REQ_DEPARTMENT_ID.HasValue && _Departments != null)
                {
                    var department = _Departments.FirstOrDefault(o => o.ID == imp.First().REQ_DEPARTMENT_ID.Value);
                    if (department != null)
                    {
                        REQ_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }
                }

                SetExtendField(this);

                this.IMP_PRICE = imp.First().IMP_PRICE;
                this.IMP_VAT_RATIO = imp.First().IMP_VAT_RATIO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000483RDO(
            List<V_HIS_EXP_MEST_BLOOD> exp,
            List<V_HIS_EXP_MEST> Listexp,
            List<V_HIS_IMP_MEST> destMest,
            List<HIS_EXP_MEST_TYPE> expMestTypes,
            List<HIS_MEDI_STOCK> _MediStocks,
            List<HIS_DEPARTMENT> _Departments,
            List<V_HIS_ROOM> _Rooms)
        {
            try
            {
                EXECUTE_TIME = exp.First().EXP_TIME;
                PACKAGE_NUMBER = exp.First().PACKAGE_NUMBER;
                EXPIRED_DATE = exp.First().EXPIRED_DATE;
                EXP_AMOUNT = exp.Count();
                EXP_MEST_CODE = exp.First().EXP_MEST_CODE;
                DESCRIPTION_DETAIL = exp.First().DESCRIPTION;
                //thong tin phieu xuất
                V_HIS_EXP_MEST ExpMest = Listexp.FirstOrDefault(o => o.ID == exp.First().EXP_MEST_ID);
                if (ExpMest != null)
                {
                    DESCRIPTION = ExpMest.DESCRIPTION;
                }


                V_HIS_EXP_MEST chmsExpMest = Listexp != null && exp.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__CK ? Listexp.FirstOrDefault(o => o.ID == exp.First().EXP_MEST_ID) : null;

                if (chmsExpMest != null)
                {
                    V_HIS_IMP_MEST chmsImpMest = destMest != null ? destMest.Where(o => o.CHMS_EXP_MEST_ID == chmsExpMest.ID).FirstOrDefault() : null;
                    this.IMP_MEDI_STOCK_CODE = _MediStocks.FirstOrDefault(o => o.ID == chmsExpMest.IMP_MEDI_STOCK_ID).MEDI_STOCK_CODE ?? "";
                    this.IMP_MEDI_STOCK_NAME = _MediStocks.FirstOrDefault(o => o.ID == chmsExpMest.IMP_MEDI_STOCK_ID).MEDI_STOCK_NAME ?? "";
                    this.SECOND_MEST_CODE = chmsImpMest != null ? chmsImpMest.IMP_MEST_CODE : "";
                    CHMS_TYPE_ID = chmsImpMest != null ? chmsImpMest.CHMS_TYPE_ID : null;
                }



                V_HIS_EXP_MEST saleExpMest = Listexp != null && exp.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN ? Listexp.FirstOrDefault(o => o.ID == exp.First().EXP_MEST_ID) : null;
                if (saleExpMest != null)
                {
                    this.CLIENT_NAME = saleExpMest.TDL_PATIENT_NAME;
                    this.TREATMENT_CODE = saleExpMest.TDL_TREATMENT_CODE;
                    this.VIR_PATIENT_NAME = saleExpMest.TDL_PATIENT_NAME;
                    this.VIR_PATIENT_ADDRESS = saleExpMest.TDL_PATIENT_ADDRESS;
                }

                V_HIS_EXP_MEST prescription = Listexp != null && (exp.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK
                    || exp.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT
                    || exp.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT)
                    ? Listexp.FirstOrDefault(o => o.ID == exp.First().EXP_MEST_ID) : null;
                if (prescription != null)
                {
                    this.TREATMENT_CODE = prescription.TDL_TREATMENT_CODE;
                    this.VIR_PATIENT_NAME = prescription.TDL_PATIENT_NAME;
                    this.VIR_PATIENT_ADDRESS = prescription.TDL_PATIENT_ADDRESS;
                }

                var data = expMestTypes != null ? expMestTypes.FirstOrDefault(o => o.ID == exp.First().EXP_MEST_TYPE_ID) : null;
                if (data != null)
                {
                    EXP_MEST_TYPE_NAME = data.EXP_MEST_TYPE_NAME;
                }

                if (exp.First().AGGR_EXP_MEST_ID.HasValue)
                {
                    var firstexp = Listexp.FirstOrDefault(o => o.ID == exp.First().AGGR_EXP_MEST_ID);
                    if (firstexp != null)
                    {
                        data = expMestTypes != null ? expMestTypes.FirstOrDefault(o => o.ID == firstexp.EXP_MEST_TYPE_ID) : null;
                        EXP_MEST_CODE = firstexp.EXP_MEST_CODE;
                        this.VIR_PATIENT_NAME = "";
                        this.TREATMENT_CODE = "";
                        this.VIR_PATIENT_ADDRESS = "";

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

                if (exp.First().REQ_ROOM_ID > 0 && _Rooms != null)
                {
                    var room = _Rooms.FirstOrDefault(o => o.ID == exp.First().REQ_ROOM_ID);
                    if (room != null)
                    {
                        REQ_ROOM_NAME = MEDI_STOCK_NAME = room.ROOM_NAME;
                    }
                }

                if (exp.First().REQ_DEPARTMENT_ID > 0 && _Departments != null)
                {
                    var department = _Departments.FirstOrDefault(o => o.ID == exp.First().REQ_DEPARTMENT_ID);
                    if (department != null)
                    {
                        REQ_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }
                    if (exp.First().EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__HPKP) REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                }

                SetExtendField(this);

                this.IMP_PRICE = exp.First().IMP_PRICE;
                this.IMP_VAT_RATIO = exp.First().IMP_VAT_RATIO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetExtendField(Mps000483RDO data)
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

        public Mps000483RDO() { }
    }
}

