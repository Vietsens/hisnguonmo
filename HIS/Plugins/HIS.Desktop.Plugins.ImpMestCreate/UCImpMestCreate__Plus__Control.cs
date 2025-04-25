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
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.Plugins.ImpMestCreate.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        private bool CheckAllowAdd(ref string messageError)
        {
            try
            {
                if (cboImpMestType.EditValue == null)
                {
                    messageError = Base.ResourceMessageManager.NguoiDungChuaChonLoaiNhap;
                    return false;
                }

                if (cboMediStock.EditValue == null)
                {
                    messageError = Base.ResourceMessageManager.NguoiDungChuaChonKhoNhap;
                    return false;
                }

                if (Convert.ToInt64(cboImpMestType.EditValue) != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    return true;
                if (this.currentSupplierForEdit == null || txtNhaCC.EditValue == null)
                {
                    messageError = Base.ResourceMessageManager.LoaiNhapLaNhapTuNhaCungCapNguoiDungPhaiChonNhaCungCap;
                    return false;
                }

                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                {
                    if (this.currentBid == null && medicineProcessor.GetBid(this.ucMedicineTypeTree) == null && !checkOutBid.Checked && !checkInOutBid.Checked)
                    {
                        messageError = Base.ResourceMessageManager.LoaiNhapLaNhapTuNhaCungCaoNguoiDungPhaiChonGoiThauHoacTichVaoNgoaiThau;
                        return false;
                    }
                }

                if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                {
                    if (this.currentBid == null && materialProcessor.GetBid(this.ucMaterialTypeTree) == null && !checkOutBid.Checked && !checkInOutBid.Checked)
                    {
                        messageError = Base.ResourceMessageManager.LoaiNhapLaNhapTuNhaCungCaoNguoiDungPhaiChonGoiThauHoacTichVaoNgoaiThau;
                        return false;
                    }
                }

                if (currrentServiceAdo != null && currrentServiceAdo.IsRequireHsd)
                {
                    if (dtExpiredDate.EditValue == null || String.IsNullOrEmpty(txtExpiredDate.Text))
                    {
                        messageError = Base.ResourceMessageManager.ChuaNhapHanSuDung;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool CheckBhytMedicineInfo()
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currrentServiceAdo), currrentServiceAdo));
                if (this.currrentServiceAdo.IsMedicine && this.currrentServiceAdo.isBusiness != 1)
                {
                    List<string> messError = new List<string>();
                    if (!IsBID)
                    {
                        if (this.currrentServiceAdo.MEDICINE_LINE_ID == 2 || this.currrentServiceAdo.MEDICINE_LINE_ID == 3)
                        {
                            if (String.IsNullOrWhiteSpace(this.txtHeinServiceBidMateType.Text))
                            {
                                messError.Add(lciHeinServiceBidMateType.Text.Trim().Trim(':'));
                            }
                            if (String.IsNullOrWhiteSpace(this.txtActiveIngrBhytName.Text))
                            {
                                messError.Add(lciActiveIngrBhytName.Text.Trim().Trim(':'));
                            }
                            if (String.IsNullOrWhiteSpace(this.txtPackingJoinBid.Text))
                            {
                                messError.Add(lciPackingJoinBid.Text.Trim().Trim(':'));
                            }
                            if (String.IsNullOrWhiteSpace(this.cboDosageForm.Text))
                            {
                                messError.Add(lciDosageForm.Text.Trim().Trim(':'));
                            }
                        }
                        else if (this.currrentServiceAdo.MEDICINE_LINE_ID != 2 || this.currrentServiceAdo.MEDICINE_LINE_ID != 3)
                        {
                            if (String.IsNullOrWhiteSpace(this.txtNognDoHL.Text))
                            {
                                messError.Add(lciConcentra.Text.Trim().Trim(':'));
                            }
                            if (String.IsNullOrWhiteSpace(this.txtSoDangKy.Text))
                            {
                                messError.Add(lciSoDangKy.Text.Trim().Trim(':'));
                            }
                            if (String.IsNullOrWhiteSpace(this.txtHeinServiceBidMateType.Text))
                            {
                                messError.Add(lciHeinServiceBidMateType.Text.Trim().Trim(':'));
                            }
                            if (String.IsNullOrWhiteSpace(this.txtActiveIngrBhytName.Text))
                            {

                                messError.Add(lciActiveIngrBhytName.Text.Trim().Trim(':'));

                            }
                        }

                    }



                    if (this.cboMedicineUseForm.EditValue == null)
                    {
                        messError.Add(lciMedicineUseForm.Text.Trim().Trim(':'));
                    }

                    messError = messError.Distinct().ToList();

                    if (messError.Count > 0 && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Bạn chưa nhập trường {0}. Bạn có chắc muốn bổ sung không?", string.Join(",", messError)), Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        result = false;

                        if (String.IsNullOrWhiteSpace(this.txtNognDoHL.Text))
                        {
                            this.txtNognDoHL.Focus();
                        }
                        else if (String.IsNullOrWhiteSpace(this.txtSoDangKy.Text))
                        {
                            this.txtSoDangKy.Focus();
                        }
                        //else if (String.IsNullOrWhiteSpace(this.txtPackingJoinBid.Text))
                        //{
                        //    this.txtPackingJoinBid.Focus();
                        //}
                        else if (String.IsNullOrWhiteSpace(this.txtHeinServiceBidMateType.Text))
                        {
                            this.txtHeinServiceBidMateType.Focus();
                        }
                        else if (this.cboMedicineUseForm.EditValue == null)
                        {
                            this.cboMedicineUseForm.Focus();
                        }
                        else if (String.IsNullOrWhiteSpace(this.txtActiveIngrBhytName.Text))
                        {
                            this.txtActiveIngrBhytName.Focus();
                        }
                        //else if (String.IsNullOrWhiteSpace(this.txtDosageForm.Text))
                        //{
                        //    this.txtDosageForm.Focus();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ShowMessValidPrice()
        {
            //23422
            bool _result = false;
            try
            {
                //neu khogn bat cau hinh - > khong canh bao - > lay du lieu nhap
                if (this.configWarningDiff != null && this.configWarningDiff.VALUE != "1") _result = true;
                if (this.currrentServiceAdo != null && spinEditGiaNhapLanTruoc.EditValue != null && spinImpPriceVAT.EditValue != null)
                {
                    decimal _giaLanTruoc = Math.Round(spinEditGiaNhapLanTruoc.Value, ConfigApplications.NumberSeperator, MidpointRounding.AwayFromZero);
                    decimal _giaSauVAT = Math.Round(spinImpPriceVAT.Value, ConfigApplications.NumberSeperator, MidpointRounding.AwayFromZero);

                    string error_warning = "";
                    Inventec.Common.Logging.LogSystem.Debug("Danh sach lo thuoc: " + LogUtil.TraceData("currentMedicine", this.currentMedicine));
                    Inventec.Common.Logging.LogSystem.Debug("Danh sach lo vat tu: " + LogUtil.TraceData("currentMaterial", this.currentMaterial));
                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        if (this.configWarningDiff != null && this.currentMedicine != null && this.configWarningDiff.VALUE == "1")
                        {
                            if (_giaLanTruoc > 0 && _giaLanTruoc != _giaSauVAT)
                            {
                                error_warning += string.Format(" giá sau VAT = {0},", _giaLanTruoc);
                                //string giaLanTruoc = Inventec.Common.Number.Convert.NumberToString(_giaLanTruoc, ConfigApplications.NumberSeperator);
                                //string messShow = string.Format("Thuốc {0} lần gần nhất nhập có giá sau VAT = {1} khác với giá nhập hiện tại. Bạn có muốn tiếp tục thêm?", this.currrentServiceAdo.MEDI_MATE_NAME, giaLanTruoc);
                                //if (DevExpress.XtraEditors.XtraMessageBox.Show(messShow, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                //{
                                //    _result = true;
                                //}
                            }
                            if (txtSoDangKy.Text != (this.currentMedicine.MEDICINE_REGISTER_NUMBER??""))
                            {
                                error_warning += string.Format(" số đăng ký là {0},", this.currentMedicine.MEDICINE_REGISTER_NUMBER);
                            }
                            if (txtPackingJoinBid.Text != (this.currentMedicine.PACKING_TYPE_NAME??""))
                            {
                                error_warning += string.Format(" quy cách đóng gói là {0},", this.currentMedicine.PACKING_TYPE_NAME);
                            }
                            if (txtHeinServiceBidMateType.Text != (this.currentMedicine.HEIN_SERVICE_BHYT_NAME??""))
                            {
                                error_warning += string.Format(" tên BHYT là {0},", this.currentMedicine.HEIN_SERVICE_BHYT_NAME);
                            }
                            if (!string.IsNullOrEmpty(error_warning))
                            {
                                if (error_warning.EndsWith(",")) error_warning = error_warning.Substring(0, error_warning.Length - 1);
                                string messShow = string.Format("Thuốc {0} lần gần nhất nhập có{1} khác với hiện tại. Bạn có muốn tiếp tục thêm?", this.currrentServiceAdo.MEDI_MATE_NAME, error_warning);
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(messShow, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    _result = true;

                                }
                                else
                                {
                                    _result = false;
                                    txtSoDangKy.Text = this.currentMedicine.MEDICINE_REGISTER_NUMBER;
                                    txtPackingJoinBid.Text = this.currentMedicine.PACKING_TYPE_NAME;
                                    txtHeinServiceBidMateType.Text = this.currentMedicine.HEIN_SERVICE_BHYT_NAME;
                                }
                                error_warning = "";
                            }
                            else _result = true;
                        }
                        else _result = true;

                        
                    }
                    else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        if (this.configWarningDiff != null  && this.currentMaterial != null && this.configWarningDiff.VALUE == "1")
                        {
                            if (_giaLanTruoc > 0 && _giaLanTruoc != _giaSauVAT)
                            {
                                error_warning += string.Format(" giá sau VAT = {0},", _giaLanTruoc);
                            }
                            if (txtSoDangKy.Text != (this.currentMaterial.MATERIAL_REGISTER_NUMBER??""))
                            {
                                error_warning += string.Format(" số đăng ký là {0},", this.currentMaterial.MATERIAL_REGISTER_NUMBER);
                            }

                            if (!string.IsNullOrEmpty(error_warning))
                            {
                                if (error_warning.EndsWith(",")) error_warning = error_warning.Substring(0, error_warning.Length - 1);
                                string messShow = string.Format("Vật tư {0} lần gần nhất nhập có{1} khác với hiện tại. Bạn có muốn tiếp tục thêm?", this.currrentServiceAdo.MEDI_MATE_NAME, error_warning);
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(messShow, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    _result = true;
                                   
                                }
                                else
                                {
                                    _result = false;
                                    txtSoDangKy.Text = this.currentMaterial.MATERIAL_REGISTER_NUMBER;
                                }
                                error_warning = "";
                            }
                            else _result = true;
                        }
                        else _result = true;


                    }

                }
            }
            catch (Exception ex)
            {
                _result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return _result;
        }

        private bool CheckDocumentNumber(string documentNumber, string kyHieuHoaDon)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrWhiteSpace(documentNumber))
                {
                    long? supplierId = null;
                    if (this.currentSupplierForEdit != null)
                    {
                        supplierId = this.currentSupplierForEdit.ID;
                    }

                    MOS.Filter.HisImpMestFilter manuImpMestFilter = new HisImpMestFilter();
                    manuImpMestFilter.DOCUMENT_NUMBER__EXACT = documentNumber;
                    manuImpMestFilter.SUPPLIER_ID = supplierId;
                    var manuImpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, manuImpMestFilter, new CommonParam());
                    if (manuImpMests != null && manuImpMests.Count > 0 && !string.IsNullOrEmpty(documentNumber))
                    {
                        long expMestIdEdit = this._currentImpMestUp != null ? this._currentImpMestUp.ID : 0;

                        if (!String.IsNullOrWhiteSpace(kyHieuHoaDon))
                        {
                            result = manuImpMests.Any(a => a.DOCUMENT_NUMBER.Equals(documentNumber.Trim()) && resultADO == null && a.SUPPLIER_ID == supplierId && a.INVOICE_SYMBOL == kyHieuHoaDon && a.ID != expMestIdEdit);
                        }
                        else
                        {
                            result = manuImpMests.Any(a => a.DOCUMENT_NUMBER.Equals(documentNumber.Trim()) && resultADO == null && a.SUPPLIER_ID == supplierId && String.IsNullOrWhiteSpace(a.INVOICE_SYMBOL) && a.ID != expMestIdEdit);
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

        private bool CheckExpiredDate()
        {
            bool result = true;
            try
            {
                if (cboImpMestType.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMestType.EditValue.ToString()) != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    return result;
                }

                if (WarningExpiredDateCFG.WarningExpiredDate > 0 && (dtExpiredDate.EditValue != null || !String.IsNullOrEmpty(txtExpiredDate.Text)))
                {
                    DateTime expired = DateTime.Now;
                    if (dtExpiredDate.EditValue != null)
                    {
                        expired = dtExpiredDate.DateTime;
                    }
                    else if (String.IsNullOrEmpty(txtExpiredDate.Text))
                    {
                        expired = DateTimeHelper.ConvertDateStringToSystemDate(txtExpiredDate.Text) ?? DateTime.Now;
                    }

                    var chk = expired - DateTime.Now;
                    if (chk.TotalDays < WarningExpiredDateCFG.WarningExpiredDate)
                    {
                        string _tittle = "";
                        if (this.currrentServiceAdo.IsMedicine)
                            _tittle = Base.ResourceMessageManager.TieuDeThuoc;
                        else
                            _tittle = Base.ResourceMessageManager.TieuDeVatTu;

                        string messageError = string.Format(Base.ResourceMessageManager.Plugins_ImpMestCreate__HanSuDungNhoHonCauHinh, _tittle, this.currrentServiceAdo.MEDI_MATE_NAME, WarningExpiredDateCFG.WarningExpiredDate);
                        if (MessageBox.Show(messageError, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return result;
        }

        private bool ShowMessValidDocumentAndDate()
        {
            bool _result = false;
            try
            {
                if (cboImpMestType.EditValue != null
                    && Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMestType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    string keyCheck = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("His.Desktop.Plugins.ImpMestCreate.SupplierImpMest.MustEnterDocumentNumberAndDocumentDate");

                    //#21673
                    List<string> _mess = new List<string>();
                    bool focusD = false;
                    if (String.IsNullOrEmpty(txtDocumentDate.Text))
                    {
                        _mess.Add("ngày hóa đơn");
                    }
                    if (string.IsNullOrEmpty(txtDocumentNumber.Text))
                    {
                        _mess.Add("số hóa đơn");
                        focusD = true;
                    }
                    if (_mess != null && _mess.Count > 0)
                    {
                        if (keyCheck == "0")
                        {
                            string messShow = string.Format("Bạn chưa nhập {0} bạn có muốn để trống không?", string.Join(",", _mess));
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(messShow, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                if (focusD)
                                {
                                    txtDocumentNumber.Focus();
                                }
                                else
                                {
                                    txtDocumentDate.Focus();
                                }
                                _result = true;
                            }
                        }
                        else if (keyCheck == "1")
                        {
                            string messShow = string.Format("Bạn phải quay lại nhập {0} mới được nhập tên thuốc, vật tư", string.Join(",", _mess));
                            DevExpress.XtraEditors.XtraMessageBox.Show(messShow, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                            if (focusD)
                            {
                                txtDocumentNumber.Focus();
                            }
                            else
                            {
                                txtDocumentDate.Focus();
                            }
                            _result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return _result;
        }

        private bool CheckDocumentNumberV2(string _document, string kyHieuHoaDon)
        {
            bool result = true;
            try
            {
                kyHieuHoaDon = String.IsNullOrWhiteSpace(kyHieuHoaDon) ? "" : kyHieuHoaDon.Trim();
                if (!string.IsNullOrEmpty(_document))
                {
                    long? _SupplierId = null;
                    if (this.currentSupplierForEdit != null)
                    {
                        _SupplierId = this.currentSupplierForEdit.ID;
                    }
                    MOS.Filter.HisImpMestFilter manuImpMestFilter = new HisImpMestFilter();
                    manuImpMestFilter.DOCUMENT_NUMBER__EXACT = _document;
                    if (!this.IsShowMessDocument)
                        manuImpMestFilter.SUPPLIER_ID = _SupplierId;
                    var manuImpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, manuImpMestFilter, new CommonParam());
                    if (manuImpMests != null && manuImpMests.Count > 0)
                    {
                        //WaitingManager.Hide();
                        long expMestIdEdit = this._currentImpMestUp != null ? this._currentImpMestUp.ID : 0;

                        //#21142
                        List<HIS_IMP_MEST> dataChecks = null;
                        if (!String.IsNullOrWhiteSpace(kyHieuHoaDon))
                        {
                            dataChecks = manuImpMests.Where(p => p.ID != expMestIdEdit && p.SUPPLIER_ID == _SupplierId && p.DOCUMENT_NUMBER.Equals(_document.Trim()) && p.INVOICE_SYMBOL == kyHieuHoaDon).ToList();
                        }
                        else
                        {
                            dataChecks = manuImpMests.Where(p => p.ID != expMestIdEdit && p.SUPPLIER_ID == _SupplierId && p.DOCUMENT_NUMBER.Equals(_document.Trim()) && String.IsNullOrWhiteSpace(p.INVOICE_SYMBOL)).ToList();
                        }
                        if (_SupplierId > 0 && dataChecks != null && dataChecks.Count > 0)
                        {
                            List<string> expMests = new List<string>();
                            foreach (var item in dataChecks)
                            {
                                V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                                string name = stock != null ? stock.MEDI_STOCK_NAME : "";
                                expMests.Add(String.Format("{0}({1})", item.IMP_MEST_CODE, name));
                            }
                            if (this.IsAllowDuplicateDocument)
                            {

                                string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}', Bạn có muốn tiếp tục nhập nhà cung cấp với số chứng từ này?", string.Join(",", expMests), _document);
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                {
                                    txtDocumentNumber.Focus();
                                    result = false;
                                }
                            }
                            else
                            {
                                string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}', Không thể nhập nhà cung cấp với số chứng từ này", string.Join(",", expMests), _document);
                                DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                                result = false;
                            }
                        }
                        else if (this.IsShowMessDocument)
                        {
                            List<string> expMests = new List<string>();
                            foreach (var item in manuImpMests)
                            {
                                V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == item.MEDI_STOCK_ID);
                                string name = stock != null ? stock.MEDI_STOCK_NAME : "";
                                expMests.Add(String.Format("{0}({1})", item.IMP_MEST_CODE, name));
                            }
                            string mess = string.Format("Đã tồn tại mã phiếu nhập '{0}' có số chứng từ '{1}',", string.Join(",", expMests), _document);
                            DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                            result = false;
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

        private bool HiglightSubString(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, string findText)
        {
            int index = FindSubStringStartPosition(e.DisplayText, findText);
            if (index == -1)
            {
                return false;
            }

            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, GetStringWithoutQuotes(findText),
                e.Appearance, Color.Indigo, Color.Gold, true, index);
            return true;
        }

        private string AddStringByConfig(int num)
        {
            string str = "";
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }

        private string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private CriteriaOperator ConvertFindPanelTextToCriteriaOperator(string findPanelText, GridView view, bool applyPrefixes)
        {
            if (!string.IsNullOrEmpty(findPanelText))
            {
                FindSearchParserResults parseResult = new FindSearchParser().Parse(findPanelText, GetFindToColumnsCollection(view));
                if (applyPrefixes)
                    parseResult.AppendColumnFieldPrefixes();

                return DxFtsContainsHelperAlt.Create(parseResult, FilterCondition.Contains, false);
            }
            return null;
        }

        private ICollection GetFindToColumnsCollection(GridView view)
        {
            System.Reflection.MethodInfo mi = typeof(ColumnView).GetMethod("GetFindToColumnsCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.Invoke(view, null) as ICollection;
        }

        private string GetStringWithoutQuotes(string findText)
        {
            string stringWithoutQuotes = findText.ToLower().Replace("\"", string.Empty);
            return stringWithoutQuotes;
        }

        private int FindSubStringStartPosition(string dispalyText, string findText)
        {
            string stringWithoutQuotes = GetStringWithoutQuotes(findText);
            int index = dispalyText.ToLower().IndexOf(stringWithoutQuotes);
            return index;
        }

        private bool CheckLifespan(VHisServiceADO vHisServiceADO)
        {
            bool result = false;
            try
            {
                if (vHisServiceADO.EXPIRED_DATE.HasValue && vHisServiceADO.monthLifespan.HasValue)
                {
                    List<HIS_EXPIRED_DATE_CFG> listExpiredDateCfg = new List<HIS_EXPIRED_DATE_CFG>();
                    string _tittle = "";
                    if (vHisServiceADO.IsMedicine)
                    {
                        _tittle = Base.ResourceMessageManager.TieuDeThuoc;
                        listExpiredDateCfg = BackendDataWorker.Get<HIS_EXPIRED_DATE_CFG>().Where(o => o.IS_MATERIAL != 1).ToList();
                        listExpiredDateCfg = listExpiredDateCfg.Where(o => (!o.LIFESPAN_MONTH_FROM.HasValue || o.LIFESPAN_MONTH_FROM <= vHisServiceADO.monthLifespan.Value) && (!o.LIFESPAN_MONTH_TO.HasValue || o.LIFESPAN_MONTH_TO > vHisServiceADO.monthLifespan.Value)).ToList();
                    }
                    else
                    {
                        _tittle = Base.ResourceMessageManager.TieuDeVatTu;
                        listExpiredDateCfg = BackendDataWorker.Get<HIS_EXPIRED_DATE_CFG>().Where(o => o.IS_MATERIAL == 1).ToList();
                        listExpiredDateCfg = listExpiredDateCfg.Where(o => (!o.LIFESPAN_MONTH_FROM.HasValue || o.LIFESPAN_MONTH_FROM <= vHisServiceADO.monthLifespan.Value) && (!o.LIFESPAN_MONTH_TO.HasValue || o.LIFESPAN_MONTH_TO > vHisServiceADO.monthLifespan.Value)).ToList();
                    }

                    if (listExpiredDateCfg.Count > 0)
                    {
                        var expiredDateCfg = listExpiredDateCfg.OrderBy(o => o.ID).FirstOrDefault();
                        if (expiredDateCfg != null)
                        {
                            long expiredDay = expiredDateCfg.EXPIRED_DAY ?? (long)((expiredDateCfg.EXPIRED_DAY_RATIO ?? 0) * vHisServiceADO.monthLifespan.Value * 30);
                            DateTime dayNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            DateTime expiredDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)(System.Math.Truncate((decimal)(vHisServiceADO.EXPIRED_DATE.Value / 1000000)) * 1000000)) ?? DateTime.Now;

                            var dif = expiredDate - dayNow;
                            long totalDays = dif.Days;

                            if (expiredDay > totalDays)
                            {
                                string lowerTittle = _tittle.ToLower();
                                string mess = string.Format("Hạn sử dụng của {0} trúng thầu có tuổi thọ {1} tháng là {2} ngày. {3} {4} có hạn sử dụng chỉ còn {5} ngày, không cho phép nhập", lowerTittle, vHisServiceADO.monthLifespan.Value, expiredDay, _tittle, vHisServiceADO.MEDI_MATE_NAME, totalDays);
                                DevExpress.XtraEditors.XtraMessageBox.Show(mess, Base.ResourceMessageManager.TieuDeCuaSoThongBaoLaThongBao);
                                result = true;
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
    }
}
