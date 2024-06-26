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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.Save
{
    class SaveManuBehavior : SaveAbstract, ISaveInit
    {
        string Deliverer { get; set; }
        decimal DocumentPrice { get; set; }
        decimal DiscountPrice { get; set; }
        decimal DiscountRatio { get; set; }
        string DocumentNumber { get; set; }
        long DocumentDate { get; set; }
        bool IsShowMessDocument = false;
        bool IsAllowDuplicateDocument = false;
        HIS_IMP_MEST _ImpMestUp { get; set; }

        internal SaveManuBehavior(CommonParam param,
            List<VHisServiceADO> serviceADOs,
            UCImpMestCreate ucImpMestCreate,
            Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicbidmedicine,
            Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicbidmaterial,
            long roomId,
            ResultImpMestADO resultADO)
            : base(param,
                serviceADOs,
                ucImpMestCreate,
                dicbidmedicine,
                dicbidmaterial,
                roomId,
                resultADO)
        {
            this.Deliverer = ucImpMestCreate.txtDeliverer.Text;
            this.DocumentPrice = ucImpMestCreate.spinDocumentPrice.Value;
            this.DocumentNumber = ucImpMestCreate.txtDocumentNumber.Text;

            if (ucImpMestCreate.dtDocumentDate.EditValue != null && ucImpMestCreate.dtDocumentDate.DateTime != DateTime.MinValue)
            {
                this.DocumentDate = Convert.ToInt64(ucImpMestCreate.dtDocumentDate.DateTime.ToString("yyyyMMddHHmmss"));
            }
            this.IsShowMessDocument = ucImpMestCreate.IsShowMessDocument;
            this.IsAllowDuplicateDocument = ucImpMestCreate.IsAllowDuplicateDocument;
            this._ImpMestUp = ucImpMestCreate._currentImpMestUp;
        }

        object ISaveInit.Run()
        {
            ResultImpMestADO result = null;
            if (this.CheckValid())
            {
                CheckValidateDocumentNumberAndDocumentDate();
                if (CheckDocumentNumber())
                    return result;
                WaitingManager.Show();
                this.InitBase();

                HisImpMestManuSDO inputImpMestSDO = new HisImpMestManuSDO();
                inputImpMestSDO.ImpMest = this._ImpMestUp != null ? this._ImpMestUp : this.ImpMest;
                inputImpMestSDO.ManuMaterials = this.MaterialWithPatySDOs;
                inputImpMestSDO.ManuMedicines = this.MedicineWithPatySDOs;

                inputImpMestSDO.ImpMest.DELIVERER = this.Deliverer;
                inputImpMestSDO.ImpMest.DESCRIPTION = this.Description;
                inputImpMestSDO.ImpMest.RECEIVER_LOGINNAME = this.LogginName;
                inputImpMestSDO.ImpMest.RECEIVER_USERNAME = this.UserName;
                Inventec.Common.Logging.LogSystem.Debug(" this.LogginName " + this.LogginName);
                inputImpMestSDO.ImpMest.DOCUMENT_PRICE = this.DocumentPrice;
                var totalPrice = this.ServiceADOs.Sum(o => (o.IMP_AMOUNT * o.IMP_PRICE));
                inputImpMestSDO.ImpMest.DISCOUNT = this.DiscountPrice;
                if (totalPrice > 0)
                {
                    inputImpMestSDO.ImpMest.DISCOUNT_RATIO = this.DiscountRatio / 100;
                }
                inputImpMestSDO.ImpMest.DOCUMENT_NUMBER = this.DocumentNumber;
                inputImpMestSDO.ImpMest.INVOICE_SYMBOL = this.InvoiceSymbol;
                if (this.DocumentDate > 0)
                {
                    inputImpMestSDO.ImpMest.DOCUMENT_DATE = this.DocumentDate;
                }
                if (this.ResultADO != null)
                {
                    inputImpMestSDO.ImpMest = this.ResultADO.HisManuSDO.ImpMest;
                }
                inputImpMestSDO.ImpMest.SUPPLIER_ID = this.SupplierId;


                HisImpMestManuSDO rs = null;
                //if (this.ResultADO == null)
                if (this._ImpMestUp == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CALL API: api/HisImpMest/ManuCreate" + Inventec.Common.Logging.LogUtil.TraceData("", inputImpMestSDO));
                    rs = new Inventec.Common.Adapter.BackendAdapter(Param).Post<HisImpMestManuSDO>("api/HisImpMest/ManuCreate", ApiConsumers.MosConsumer, inputImpMestSDO, Param);
                }
                else
                {                   
                    inputImpMestSDO.ImpMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    Inventec.Common.Logging.LogSystem.Debug("CALL API: api/HisImpMest/ManuCreate" + Inventec.Common.Logging.LogUtil.TraceData("", inputImpMestSDO));
                    rs = new Inventec.Common.Adapter.BackendAdapter(Param).Post<HisImpMestManuSDO>("api/HisImpMest/ManuUpdate", ApiConsumers.MosConsumer, inputImpMestSDO, Param);
                }

                if (rs != null)
                    result = this.ResultADO = new ResultImpMestADO(rs);
                //else
                //{
                //    MessageManager.Show(Param, false);
                //}
            }

            return result;
        }

        private bool CheckDocumentNumber()
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(this.DocumentNumber) && (this.IsShowMessDocument || !this.IsAllowDuplicateDocument))
                {
                    MOS.Filter.HisImpMestFilter manuImpMestFilter = new HisImpMestFilter();
                    manuImpMestFilter.DOCUMENT_NUMBER__EXACT = this.DocumentNumber;
                    if (!this.IsShowMessDocument)
                        manuImpMestFilter.SUPPLIER_ID = this.SupplierId;
                    var manuImpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, manuImpMestFilter, new CommonParam());
                    if (manuImpMests != null && manuImpMests.Count > 0)
                    {
                        WaitingManager.Hide();

                        long expMestIdEdit = this._ImpMestUp != null ? this._ImpMestUp.ID : 0;

                        //#21142
                        List<HIS_IMP_MEST> dataChecks = null;
                        if (!String.IsNullOrWhiteSpace(this.InvoiceSymbol))
                        {
                            dataChecks = manuImpMests.Where(p => p.ID != expMestIdEdit && p.SUPPLIER_ID == this.SupplierId && p.DOCUMENT_NUMBER.Equals(this.DocumentNumber.Trim()) && p.INVOICE_SYMBOL == this.InvoiceSymbol).ToList();
                        }
                        else
                        {
                            dataChecks = manuImpMests.Where(p => p.ID != expMestIdEdit && p.SUPPLIER_ID == this.SupplierId && p.DOCUMENT_NUMBER.Equals(this.DocumentNumber.Trim()) && String.IsNullOrWhiteSpace(p.INVOICE_SYMBOL)).ToList();
                        }
                        if (this.SupplierId > 0 && dataChecks != null && dataChecks.Count > 0)
                        {
                            if (IsAllowDuplicateDocument)
                                return result;
                            string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}', Không thể nhập nhà cung cấp với số chứng từ này", string.Join(",", dataChecks.Select(p => p.IMP_MEST_CODE).ToList()), this.DocumentNumber);
                            DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                            result = true;
                        }
                        else if (this.IsShowMessDocument)
                        {
                            if (this._ImpMestUp != null && this._ImpMestUp.ID > 0)
                            {
                                var checkCurrent = this._ImpMestUp != null ? manuImpMests.Where(o => o.ID != this._ImpMestUp.ID).ToList() : null;
                                if (checkCurrent != null && checkCurrent.Count > 0)
                                {
                                    string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}',", string.Join(",", checkCurrent.Select(p => p.IMP_MEST_CODE).ToList()), this.DocumentNumber);
                                    DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                                    result = false;
                                }
                            }
                            else
                            {
                                string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}',", string.Join(",", manuImpMests.Select(p => p.IMP_MEST_CODE).ToList()), this.DocumentNumber);
                                DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CheckValidateDocumentNumberAndDocumentDate()
        {
            try
            {
                List<string> _mess = new List<string>();
                if (string.IsNullOrEmpty(this.DocumentNumber))
                {
                    _mess.Add("Số chứng từ");
                }
                if (this.DocumentDate <= 0)
                {
                    _mess.Add("Ngày chứng từ");
                }
                if (_mess != null && _mess.Count > 0)
                {
                    WaitingManager.Hide();
                    string mess = string.Format("Thông tin '{0}' rỗng", string.Join(",", _mess), this.DocumentNumber);
                    DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaCanhBao);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
