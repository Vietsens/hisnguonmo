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

namespace MPS.Processor.Mps000092.PDO
{
    public class Mps000092PDO : RDOBase
    {
        public List<V_HIS_EXP_MEST> _SaleExpMests = null;
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;
        public List<V_HIS_CASHIER_ROOM> _HisCashierRoom = null;
        public V_HIS_TREATMENT _Treatment = null;
        public List<Mps000092ADO> listAdo = new List<Mps000092ADO>();
        public V_HIS_TRANSACTION Transaction = new V_HIS_TRANSACTION();
        public List<V_HIS_TRANSACTION> _Transactions = new List<V_HIS_TRANSACTION>();
        public List<HIS_BILL_GOODS> _BillGoods = null;
        public List<V_HIS_IMP_MEST> _HisImpMest = null;

        public long expMesttSttId__Draft = 1;// trạng thái nháp
        public long expMesttSttId__Request = 2;// trạng thái yêu cầu
        public long expMesttSttId__Reject = 3;// không duyệt
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất      

        public Mps000092PDO() { }

        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, V_HIS_TRANSACTION transaction)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
        }

        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export, V_HIS_TRANSACTION transaction)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
        }

        public Mps000092PDO(List<HIS_BILL_GOODS> listBillGoods, V_HIS_TRANSACTION transaction)
        {
            this._BillGoods = listBillGoods;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
        }

        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, V_HIS_TRANSACTION transaction, List<V_HIS_CASHIER_ROOM> hisCashierRoom, V_HIS_TREATMENT treatment)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
            this._HisCashierRoom = hisCashierRoom;
            this._Treatment = treatment;
        }


        //
        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, V_HIS_TRANSACTION transaction, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
            this._HisImpMest = hisImpMest;
        }

        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export, V_HIS_TRANSACTION transaction, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.expMesttSttId__Draft = _expMesttSttId__Draft;
            this.expMesttSttId__Request = _expMesttSttId__Request;
            this.expMesttSttId__Reject = _expMesttSttId__Reject;
            this.expMesttSttId__Approval = _expMesttSttId__Approval;
            this.expMesttSttId__Export = _expMesttSttId__Export;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
            this._HisImpMest = hisImpMest;
        }

        public Mps000092PDO(List<HIS_BILL_GOODS> listBillGoods, V_HIS_TRANSACTION transaction, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this._BillGoods = listBillGoods;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
            this._HisImpMest = hisImpMest;
        }

        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, V_HIS_TRANSACTION transaction, List<V_HIS_CASHIER_ROOM> hisCashierRoom, V_HIS_TREATMENT treatment, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            this.Transaction = transaction;
            if (transaction != null && transaction.ID != 0)
            {
                this._Transactions.Add(transaction);
            }
            this._HisCashierRoom = hisCashierRoom;
            this._Treatment = treatment;
            this._HisImpMest = hisImpMest;
        }

        public Mps000092PDO(List<V_HIS_EXP_MEST> saleExpMests, List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_EXP_MEST_MATERIAL> listMaterial, List<V_HIS_TRANSACTION> listTransaction, List<V_HIS_CASHIER_ROOM> hisCashierRoom, V_HIS_TREATMENT treatment, List<V_HIS_IMP_MEST> hisImpMest)
        {
            this._SaleExpMests = saleExpMests;
            this._Materials = listMaterial;
            this._Medicines = listMedicine;
            if (listTransaction != null && listTransaction.Count == 1)
            {
                this.Transaction = listTransaction.First();
            }
            this._Transactions = listTransaction;
            this._HisCashierRoom = hisCashierRoom;
            this._Treatment = treatment;
            this._HisImpMest = hisImpMest;
        }

    }

    public class Mps000092ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }

        public decimal? DISCOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? PRICE_VAT { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal SUM_TOTAL_PRICE { get; set; }
        public decimal SUM_PRICE_NOT_ROUNT { get; set; }
        public string CONCENTRA { get; set; }

        public decimal AMOUNT { get; set; }

        public long? NUM_ORDER { get; set; }

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }


        public Mps000092ADO()
        {
        }

        public Mps000092ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.PRICE = listMedicine.First().PRICE;
                    this.TUTORIAL = listMedicine.First().TUTORIAL;
                    this.PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    if (listMedicine.First().VAT_RATIO.HasValue)
                    {
                        this.VAT_RATIO_100 = listMedicine.First().VAT_RATIO.Value * 100;
                    }
                    this.DISCOUNT = listMedicine.Sum(o => o.DISCOUNT ?? 0);
                    this.AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.TOTAL_AMOUNT = listMedicine.Sum(s => s.AMOUNT - (s.TH_AMOUNT ?? 0));
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.PRICE_VAT = this.PRICE.Value * (1 + vat);
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat) - (this.DISCOUNT ?? 0);
                        this.SUM_PRICE_NOT_ROUNT = Math.Round(this.SUM_TOTAL_PRICE, 0, MidpointRounding.AwayFromZero);
                    }

                    this.CONCENTRA = listMedicine.First().CONCENTRA;
                    this.ACTIVE_INGR_BHYT_CODE = listMedicine.First().ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = listMedicine.First().ACTIVE_INGR_BHYT_NAME;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000092ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
            try
            {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.PRICE = listMaterial.First().PRICE;
                    this.PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    if (listMaterial.First().VAT_RATIO.HasValue)
                    {
                        this.VAT_RATIO_100 = listMaterial.First().VAT_RATIO.Value * 100;
                    }
                    this.DISCOUNT = listMaterial.Sum(o => o.DISCOUNT ?? 0);
                    this.AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.TOTAL_AMOUNT = listMaterial.Sum(s => s.AMOUNT - (s.TH_AMOUNT ?? 0));
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.PRICE_VAT = this.PRICE.Value * (1 + vat);
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat) - (this.DISCOUNT ?? 0);
                        this.SUM_PRICE_NOT_ROUNT = Math.Round(this.SUM_TOTAL_PRICE, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000092ADO(HIS_BILL_GOODS billGoods)
        {
            try
            {
                if (billGoods != null)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_NAME = billGoods.GOODS_NAME;
                    this.SERVICE_UNIT_NAME = billGoods.GOODS_UNIT_NAME;
                    this.PRICE = billGoods.PRICE;
                    this.DISCOUNT = billGoods.DISCOUNT;
                    this.TOTAL_AMOUNT = billGoods.AMOUNT;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.PRICE_VAT = this.PRICE.Value * (1 + vat);
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat) - (this.DISCOUNT ?? 0);
                        this.SUM_PRICE_NOT_ROUNT = Math.Round(this.SUM_TOTAL_PRICE, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
