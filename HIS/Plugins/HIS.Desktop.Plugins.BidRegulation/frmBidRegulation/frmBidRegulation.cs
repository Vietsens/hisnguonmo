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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using System.Globalization;
using HIS.Desktop.LocalStorage.LocalData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Common;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.BidRegulation.frmBidRegulation
{
    public partial class frmBidRegulation : DevExpress.XtraEditors.XtraForm
    {
        #region init data
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        DateTime minValue;
        DateTime maxValue;
        int positionHandle = -1;
        bool hasValidTime = false;
        MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE dataMaty = null;
        MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE dataMety = null;
        V_HIS_BID_MATY_ADJUST CurrentdataMaty = new V_HIS_BID_MATY_ADJUST();
        V_HIS_BID_METY_ADJUST CurrentdataMety = new V_HIS_BID_METY_ADJUST();
        List<MOS.EFMODEL.DataModels.HIS_BID> listBid = null;
        DelegateSelectData delegateData = null;
        decimal totalAmount = 0;
#endregion
        public frmBidRegulation(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE dataMaterial, MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE dataMedicine,DelegateSelectData delegateData)
        {
            InitializeComponent();
            this.Text = moduleData.text;
            this.dataMaty = dataMaterial;
            this.dataMety = dataMedicine;
            this.delegateData = delegateData;
            LogSystem.Debug("Recived__dataMety DTO: " + LogUtil.TraceData("dataMetyDTO:", dataMedicine));
            LogSystem.Debug("Recived__dataMaty: " + LogUtil.TraceData("dataMatyDTO:", dataMaterial));
            
        }
        #region form action
        private void frmBidRegulation_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
                SetDefaultValue();
                LoadListBid();
                Validate();
                loadSupplier();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void frmBidRegulation_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.delegateData!= null) this.delegateData(totalAmount);
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmBidRegulation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.N)
                {
                    btnSave.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.S)
                {
                    btnEdit.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.R)
                {
                    btnReset.PerformClick();
                }
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
#endregion
        #region load language resouce

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.BidRegulation.Resources.Lang", typeof(frmBidRegulation).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmBidRegulation.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbboSupplier.Properties.NullText = Inventec.Common.Resource.Get.Value("frmBidRegulation.cbboSupplier.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txt1.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.txt1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmBidRegulation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region Validate
        private void Validate()
        {
            try
            {
                ValidationMaxLength(txtNote, 500,false);
                ValidateTime();
                //ValidateAmount();

            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }

        
        private void ValidateTime()
        {
            try
            {
                //if (txtValidtime.EditValue == null) return;
                HIS_BID bid = new HIS_BID();
                if (this.dataMety != null)
                {
                    bid = listBid.FirstOrDefault(s => s.ID == this.dataMety.BID_ID);
                }
                else bid = listBid.FirstOrDefault(s => s.ID == this.dataMaty.BID_ID);

                if (bid!= null && (bid.VALID_FROM_TIME != null && bid.VALID_TO_TIME != null))
                {
                    this.hasValidTime = true;
                    this.minValue = DateTime.ParseExact(Inventec.Common.DateTime.Convert.TimeNumberToDateString(bid.VALID_FROM_TIME ?? 0).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    this.maxValue = DateTime.ParseExact(Inventec.Common.DateTime.Convert.TimeNumberToDateString(bid.VALID_TO_TIME ?? 0).ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //txtValidtime.Properties.MinValue = this.minValue;
                    //txtValidtime.Properties.MaxValue = this.maxValue;
                    
                }

            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void ValidationMaxLength(BaseEdit control, int maxlength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule _rule = new ControlMaxLengthValidationRule();
                _rule.editor = control;
                _rule.maxLength = maxlength;
                _rule.IsRequired = IsRequired;
                _rule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CheckMaxLenth(string Str, int Length)
        {
            try
            {
                return (Str != null && Encoding.UTF8.GetByteCount(Str.Trim()) <= Length);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }
        private void txtNote_Validated(object sender, EventArgs e)
        {
            try
            {
                if (!CheckMaxLenth(txtNote.Text, 500))
                {
                    dxErrorProvider1.SetError(txtNote, "Độ dài vượt quá 500", ErrorType.Warning);
                }
                else
                {
                    dxErrorProvider1.SetError(txtNote, "");
                }
            }
            catch (Exception ex)
            {

                   Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool enableEditValueChanging = true;

        // Hàm để tắt hoặc bật sự kiện EditValueChanging
        private void ToggleEditValueChanging(bool enable)
        {
            enableEditValueChanging = enable;
        }

        private void txtValidtime_Validated(object sender, EventArgs e)
        {
            try
            {
                DateEdit editor = sender as DateEdit;
    
                if (editor != null && editor.EditValue != null)
                {
                    DateTime newDate = Convert.ToDateTime(editor.EditValue);
        
                    if (this.hasValidTime && (newDate < minValue || newDate > maxValue))
                    {
                        // Hiển thị lỗi nếu ngày nằm ngoài khoảng giới hạn
                        dxErrorProvider1.SetError(editor, "Thời gian nằm ngoài khoảng hiệu lực của thầu", ErrorType.Warning);
                    }
                    else
                    {
                        // Xoá lỗi nếu ngày nằm trong khoảng giới hạn
                        dxErrorProvider1.SetError(editor, "");
                    }
                }
                editor.Text = editor.Text.Split(' ')[0];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                throw;
            }
        }
        private void txtAmount_Validated(object sender, EventArgs e)
        {
            try
            {
                if (txtAmount.Text.Trim() == "0")
                {
                    dxErrorProvider1.SetError(txtAmount, "Số lượng nhập phải khác 0", ErrorType.Warning);
                }
                else
                {
                    dxErrorProvider1.SetError(txtAmount, "");
                }
                if (txtAmount != null)
                {
                    decimal amount = decimal.Parse(txtAmount.Text);
                    if (amount < 0)
                    {
                        var vir_amount = Math.Abs(amount);
                        if (this.dataMety != null)
                        {
                            if ((this.dataMety.AMOUNT + this.dataMety.ADJUST_AMOUNT ?? 0) < vir_amount)
                            {
                                DialogResult rs = MessageBox.Show("Số lượng điều tiết bạn nhập đang vượt quá số lượng thầu");
                                dxErrorProvider1.SetError(txtAmount, "Số lượng điều tiết bạn nhập đang vượt quá số lượng thầu", ErrorType.Warning);
                            }
                        }
                        else if (this.dataMaty != null)
                        {
                            if ((this.dataMaty.AMOUNT + this.dataMaty.ADJUST_AMOUNT ?? 0) < vir_amount)
                            {
                                DialogResult rs = MessageBox.Show("Số lượng điều tiết bạn nhập đang vượt quá số lượng thầu");
                                dxErrorProvider1.SetError(txtAmount, "Số lượng điều tiết bạn nhập đang vượt quá số lượng thầu", ErrorType.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {

                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        #endregion
        
        #region load data
        private void LoadListBid()
        {
            try
            {
                listBid = new List<MOS.EFMODEL.DataModels.HIS_BID>();
                MOS.Filter.HisBidFilter filter = new HisBidFilter();
                listBid = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_BID>>("/api/HisBid/Get", ApiConsumers.MosConsumer, filter, null);
               

            }
            catch (Exception ex)
            {

                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, pageSize, this.grcListBid);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Load data to list");
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                grcListBid.BeginUpdate();
                this.totalAmount = 0;
                if (dataMety != null)
                {
                    Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_BID_METY_ADJUST>> apiResult = null;
                    MOS.Filter.HisBidMetyAdjustViewFilter filter = new HisBidMetyAdjustViewFilter();
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.BID_MEDICINE_TYPE_ID = dataMety.ID;
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_BID_METY_ADJUST>>("/api/HisBidMetyAdjust/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (apiResult != null)
                    {
                        var data = apiResult.Data;
                        if (data != null)
                        {
                            grvListBid.GridControl.DataSource = data;
                            rowCount = (data == null ? 0 : data.Count);
                            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                            
                            foreach (var item in data)
                            {
                                this.totalAmount += item.AMOUNT??0;
                            }
                        }
                        else grcListBid.DataSource = null;
                    }
                }
                else if(dataMaty != null)
                {
                    Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_BID_MATY_ADJUST>> apiResult = null;
                    MOS.Filter.HisBidMatyAdjustViewFilter filter = new HisBidMatyAdjustViewFilter();
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.BID_MATERIAL_TYPE_ID = dataMaty.ID;
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_BID_MATY_ADJUST>>("/api/HisBidMatyAdjust/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (apiResult != null)
                    {
                        var data = apiResult.Data;
                        if (data != null)
                        {

                            grvListBid.GridControl.DataSource = data;
                            rowCount = (data == null ? 0 : data.Count);
                            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                            
                            foreach (var item in data)
                            {
                                this.totalAmount += item.AMOUNT??0;
                            }
                        }
                        else grcListBid.DataSource = null;
                    }
                }
                
                grcListBid.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        
        private void SetDefaultValue()
        {
            try
            {
                cbboSupplier.EditValue = null;
                txtAmount.Text = "";
                txtNote.Text = "";
                btnSave.Enabled = true;
                btnEdit.Enabled = false;
                dxErrorProvider1.ClearErrors();

                ToggleEditValueChanging(false);
                txtValidtime.EditValue = null;
                //txtValidtime.Text = "";
                ToggleEditValueChanging(true);
                //btnReset.PerformClick();
                
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void FillDataToEditControl()
        {
            try
            {
                SetDefaultValue();
                btnSave.Enabled = false;
                btnEdit.Enabled = true;
                var rowData = grvListBid.GetFocusedRow();
                if (rowData is V_HIS_BID_METY_ADJUST)
                {
                    this.CurrentdataMety = (V_HIS_BID_METY_ADJUST)rowData;
                    cbboSupplier.EditValue = this.CurrentdataMety.SUPPLIER_ID;
                    txtAmount.Text = this.CurrentdataMety.AMOUNT.ToString();
                    txtNote.Text = this.CurrentdataMety.DESCRIPTION;
                    string ADJUST_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CurrentdataMety.ADJUST_TIME ?? 0);
                    if (ADJUST_TIME != null)
                    {

                        txtValidtime.DateTime = DateTime.ParseExact(ADJUST_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        txtValidtime.Text = txtValidtime.Text.Split(' ')[0];
                    }
                }
                else if (rowData is V_HIS_BID_MATY_ADJUST)
                {
                    this.CurrentdataMaty = (V_HIS_BID_MATY_ADJUST)rowData;
                    cbboSupplier.EditValue = this.CurrentdataMaty.SUPPLIER_ID;
                    txtAmount.Text = this.CurrentdataMaty.AMOUNT.ToString();
                    txtNote.Text = this.CurrentdataMaty.DESCRIPTION;
                    string ADJUST_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CurrentdataMaty.ADJUST_TIME ?? 0);
                    if (ADJUST_TIME != null)
                    {
                        txtValidtime.DateTime = DateTime.ParseExact(ADJUST_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        txtValidtime.Text = txtValidtime.Text.Split(' ')[0];
                    }
                }
                
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void loadSupplier()
        {
            try
            {
                CommonParam param = new CommonParam();

                List<HIS_SUPPLIER> data = BackendDataWorker.Get<HIS_SUPPLIER>().Where(s=>s.IS_ACTIVE == 1).ToList();
                
                List<ColumnInfo> columInfos = new List<ColumnInfo>();
                columInfos.Add(new ColumnInfo("SUPPLIER_CODE", "", 100, 1));
                columInfos.Add(new ColumnInfo("SUPPLIER_NAME", "", 200, 2));
                ControlEditorADO controlEditorDAO = new ControlEditorADO("SUPPLIER_NAME", "ID", columInfos, false,300);
                ControlEditorLoader.Load(cbboSupplier, data, controlEditorDAO);
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        #endregion
        #region Handle button click

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (dataMety != null)
                {
                    var data = (MOS.EFMODEL.DataModels.V_HIS_BID_METY_ADJUST)grvListBid.GetFocusedRow();
                    if (data == null) return;
                    var selectedBid = listBid.FirstOrDefault(s => s.ID == this.dataMety.BID_ID);
                    if (selectedBid != null && selectedBid.IS_ACTIVE != 1)
                    {
                        DialogResult rs = MessageBox.Show("Thầu " + selectedBid.BID_NAME + " đã bị khóa, không cho phép xóa dữ liệu");
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            bool result = new BackendAdapter(param).Post<bool>("/api/HisBidMetyAdjust/Delete", ApiConsumers.MosConsumer, data.ID, param);
                            if (result)
                            {
                                success = true;
                                FillDataToControl();
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var data = (MOS.EFMODEL.DataModels.V_HIS_BID_MATY_ADJUST)grvListBid.GetFocusedRow();
                    if (data == null) return;
                    var selectedBid = listBid.FirstOrDefault(s => s.ID == this.dataMaty.BID_ID);
                    if (selectedBid != null && selectedBid.IS_ACTIVE != 1)
                    {
                        DialogResult rs = MessageBox.Show("Thầu " + selectedBid.BID_NAME + " đã bị khóa, không cho phép xóa dữ liệu");
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            bool result = new BackendAdapter(param).Post<bool>("/api/HisBidMatyAdjust/Delete", ApiConsumers.MosConsumer, data.ID, param);
                            if (result)
                            {
                                success = true;
                                FillDataToControl();
                            }
                            
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                //WaitingManager.Hide();
                MessageManager.Show(this, param, success);

            }
            

            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || dxErrorProvider1.HasErrors) return;
                if (this.dataMaty != null)
                {
                    var selectedBid = listBid.FirstOrDefault(s => s.ID == this.dataMaty.BID_ID);
                    if (selectedBid != null && selectedBid.IS_ACTIVE != 1)
                    {
                        DialogResult rs = MessageBox.Show("Thầu " + selectedBid.BID_NAME + " đã bị khóa, không cho phép thao tác dữ liệu");
                        return;
                    }
                }
                else if (this.dataMety != null)
                {
                    var selectedBid = listBid.FirstOrDefault(s => s.ID == this.dataMety.BID_ID);
                    if (selectedBid != null && selectedBid.IS_ACTIVE != 1)
                    {
                        DialogResult rs = MessageBox.Show("Thầu " + selectedBid.BID_NAME + " đã bị khóa, không cho phép thao tác dữ liệu");
                        return;
                    }
                }
                CommonParam param = new CommonParam();
                positionHandle = -1;
                bool success = false;
                //WaitingManager.Show();
                if (this.dataMaty == null && this.dataMety == null) return;
                success = this.ProcessSave(ref param, GlobalVariables.ActionAdd);
                SetDefaultValue();
                //WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {

                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate() || dxErrorProvider1.HasErrors || !btnEdit.Enabled) return;
                if (this.dataMaty != null)
                {
                    var selectedBid = listBid.FirstOrDefault(s => s.ID == this.dataMaty.BID_ID);
                    if (selectedBid != null && selectedBid.IS_ACTIVE != 1)
                    {
                        DialogResult rs = MessageBox.Show("Thầu " + selectedBid.BID_NAME + " đã bị khóa, không cho phép thao tác dữ liệu");
                        return;
                    }
                }
                else if (this.dataMety != null)
                {
                    var selectedBid = listBid.FirstOrDefault(s => s.ID == this.dataMety.BID_ID);
                    if (selectedBid != null && selectedBid.IS_ACTIVE != 1)
                    {
                        DialogResult rs = MessageBox.Show("Thầu " + selectedBid.BID_NAME + " đã bị khóa, không cho phép thao tác dữ liệu");
                        return;
                    }
                }
                CommonParam param = new CommonParam();
                bool success = false;
                //WaitingManager.Show();
                if (this.dataMaty == null && this.dataMety == null) return;
                success = this.ProcessUpdate(ref param);
                SetDefaultValue();
                //WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool ProcessUpdate(ref CommonParam param)
        {
            try
            {
                bool success = false;
                grvListBid.UpdateCurrentRow();

                HIS_BID_MATY_ADJUST dataMatyDTO = new HIS_BID_MATY_ADJUST();
                HIS_BID_METY_ADJUST dataMetyDTO = new HIS_BID_METY_ADJUST();
                if (this.CurrentdataMaty != null && this.CurrentdataMaty.ID != 0)
                {
                    if (cbboSupplier.EditValue != null)
                    {
                        dataMatyDTO.SUPPLIER_ID = Convert.ToInt64(cbboSupplier.EditValue);
                    }
                    if (txtAmount != null)
                    {
                        decimal amount = decimal.Parse(txtAmount.Text);
                        
                        dataMatyDTO.AMOUNT = amount;
                    }
                    dataMatyDTO.TDL_BID_ID = this.dataMaty.BID_ID;
                    if (txtValidtime.DateTime != null && txtValidtime.DateTime != DateTime.MinValue && txtValidtime.Text != null) dataMatyDTO.ADJUST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtValidtime.DateTime);
                    dataMatyDTO.DESCRIPTION = txtNote.Text;
                    dataMatyDTO.BID_MATERIAL_TYPE_ID = dataMaty.ID;

                    HisBidMatyAdjustFilter filter = new HisBidMatyAdjustFilter();
                    filter.ID = this.CurrentdataMaty.ID;
                    var dataAPI = new BackendAdapter(param).Get<List<HIS_BID_MATY_ADJUST>>("/api/HisBidMatyAdjust/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dataAPI != null)
                    {
                        dataMatyDTO.ID = this.CurrentdataMaty.ID;
                        LogSystem.Debug("Update__dataMaty DTO: " + LogUtil.TraceData("dataMatyDTO:", dataMatyDTO));
                        var rs = new BackendAdapter(param).Post<HIS_BID_MATY_ADJUST>("/api/HisBidMatyAdjust/Update", ApiConsumers.MosConsumer, dataMatyDTO, param);
                        if (rs != null)
                        {
                            success = true;
                            FillDataToControl();
                        }
                    }
                }
                else if (this.CurrentdataMety != null && this.CurrentdataMety.ID != 0 )
                {
                    
                    if (cbboSupplier.EditValue != null)
                    {

                        dataMetyDTO.SUPPLIER_ID = Convert.ToInt64(cbboSupplier.EditValue);
                    }
                    if (txtAmount != null)
                    {
                        decimal amount = decimal.Parse(txtAmount.Text);
                        
                        dataMetyDTO.AMOUNT = amount;
                    }
                    dataMetyDTO.TDL_BID_ID = this.dataMety.BID_ID;
                    if (txtValidtime.DateTime != null && txtValidtime.DateTime != DateTime.MinValue && txtValidtime.Text != null) dataMetyDTO.ADJUST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtValidtime.DateTime);
                    dataMetyDTO.DESCRIPTION = txtNote.Text;
                    dataMetyDTO.BID_MEDICINE_TYPE_ID = dataMety.ID;
                    HisBidMetyAdjustFilter filter = new HisBidMetyAdjustFilter();
                    filter.ID = this.CurrentdataMety.ID;
                    
                    var dataAPI = new BackendAdapter(param).Get<List<HIS_BID_METY_ADJUST>>("/api/HisBidMetyAdjust/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dataAPI != null)
                    {
                        dataMetyDTO.ID = this.CurrentdataMety.ID;
                        LogSystem.Debug("Update__dataMety DTO: " + LogUtil.TraceData("dataMetyDTO:", dataMetyDTO));
                        var rs = new BackendAdapter(param).Post<HIS_BID_METY_ADJUST>("/api/HisBidMetyAdjust/Update", ApiConsumers.MosConsumer, dataMetyDTO, param);
                        if (rs != null)
                        {
                            success = true;
                            FillDataToControl();
                        }
                    }
                    
                }

                return success;
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool ProcessSave(ref CommonParam param,int action)
        {
            try
            {
                bool success = false;
                grvListBid.UpdateCurrentRow();
               
                HIS_BID_MATY_ADJUST dataMatyDTO = new HIS_BID_MATY_ADJUST();
                HIS_BID_METY_ADJUST dataMetyDTO = new HIS_BID_METY_ADJUST();
                if (this.dataMaty != null)
                {
                    
                    if (cbboSupplier.EditValue != null)
                    {

                        dataMatyDTO.SUPPLIER_ID = Convert.ToInt64(cbboSupplier.EditValue);
                    }
                    if (txtAmount != null)
                    {
                        decimal amount = decimal.Parse(txtAmount.Text);
                        
                        dataMatyDTO.AMOUNT = amount;
                    }
                    dataMatyDTO.TDL_BID_ID = this.dataMaty.BID_ID;
                    if (txtValidtime.DateTime != null && txtValidtime.DateTime != DateTime.MinValue && txtValidtime.Text != null) dataMatyDTO.ADJUST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtValidtime.DateTime);
                    dataMatyDTO.DESCRIPTION = txtNote.Text;
                    dataMatyDTO.BID_MATERIAL_TYPE_ID = dataMaty.ID;
                    LogSystem.Debug("Create__dataMaty DTO: "+LogUtil.TraceData("dataMatyDTO:",dataMatyDTO));
                    var result = new BackendAdapter(param).Post<HIS_BID_MATY_ADJUST>("/api/HisBidMatyAdjust/Create", ApiConsumers.MosConsumer, dataMatyDTO, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToControl();
                    }
                    
                }
                else if (this.dataMety != null)
                {
                    
                    if (cbboSupplier.EditValue != null)
                    {

                        dataMetyDTO.SUPPLIER_ID = Convert.ToInt64(cbboSupplier.EditValue);
                    }
                    if (txtAmount != null)
                    {
                        decimal amount = decimal.Parse(txtAmount.Text);
                        
                        dataMetyDTO.AMOUNT = amount;
                        
                    }
                    dataMetyDTO.TDL_BID_ID = this.dataMety.BID_ID;
                    if (txtValidtime.DateTime != null && txtValidtime.DateTime != DateTime.MinValue && txtValidtime.Text != null) dataMetyDTO.ADJUST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(txtValidtime.DateTime);
                    dataMetyDTO.DESCRIPTION = txtNote.Text;
                    dataMetyDTO.BID_MEDICINE_TYPE_ID = dataMety.ID;
                    LogSystem.Debug("Create__dataMety DTO: "+LogUtil.TraceData("dataMetyDTO:",dataMetyDTO));
                    var result = new BackendAdapter(param).Post<HIS_BID_METY_ADJUST>("/api/HisBidMetyAdjust/Create", ApiConsumers.MosConsumer, dataMetyDTO, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToControl();
                    }
                    
                }
                return success;
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
        #endregion
        #region custom data
        private void grvListBid_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    if (this.dataMaty != null)
                    {
                        V_HIS_BID_MATY_ADJUST pData = (V_HIS_BID_MATY_ADJUST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (e.Column.FieldName == "STT")
                        {

                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.CREATE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "ADJUST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.ADJUST_TIME ?? 0);
                        }
                    }
                    else if (this.dataMety != null)
                    {
                        V_HIS_BID_METY_ADJUST pData = (V_HIS_BID_METY_ADJUST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (e.Column.FieldName == "STT")
                        {

                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.CREATE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.MODIFY_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "ADJUST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.ADJUST_TIME ?? 0);
                        }
                    }
                    
                    
                }
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }

        private void grvListBid_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                FillDataToEditControl();
            }
            catch (Exception ex)
            {
                
                   Inventec.Common.Logging.LogSystem.Warn(ex);;
            }
        }
#endregion

        
        #region handle value change
        private void txtValidtime_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try 
	        {	        
		        DateEdit editor = sender as DateEdit;

                if (editor != null && e.NewValue != null)
                {
                    string input = e.NewValue.ToString();

                    if (input.Length == 8)
                    {
                        try
                        {

                            string formattedDate = string.Format(input.Substring(0, 2)) + "/" + string.Format(input.Substring(2, 2)) + "/" + string.Format(input.Substring(4, 4));
                            DateTime parsedDate;

                            // Kiểm tra nếu giá trị là ngày tháng hợp lệ
                            if (DateTime.TryParseExact(formattedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                editor.EditValue = parsedDate;
                                e.Cancel = true; // Hủy sự kiện thay đổi để không ghi đè lại giá trị mới
                            }
                            else
                            {
                                // Hiển thị thông báo lỗi nếu giá trị không hợp lệ
                                dxErrorProvider1.SetError(editor, "Ngày tháng không hợp lệ.", ErrorType.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Xử lý ngoại lệ nếu có
                            
                        }
                    }
                    
                }
	        }
	        catch (Exception ex)
	        {
		
		        Inventec.Common.Logging.LogSystem.Warn(ex);
	        }
        }
        #endregion

    }
}
