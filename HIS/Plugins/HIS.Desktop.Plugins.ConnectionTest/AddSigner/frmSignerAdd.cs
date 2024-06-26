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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.TDO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.SignLibrary.Integrate;
using Inventec.Common.SignLibrary.LibraryMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Common.Integrate;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ConnectionTest.AddSigner
{
    public partial class frmSignerAdd : Form
    {
        List<SignTDO> listSign;
        List<EMR_SIGNER> signers;
        List<EMR_SIGN_TEMP> signTemplates;
        Action<List<SignTDO>, bool> actAfterSave;
        internal const long stepNumOrder = 1;
        bool SignParanel;

        public frmSignerAdd(List<SignTDO> listsign, Action<List<SignTDO>, bool> actAfterSave, bool checkSignParanel)
        {
            try
            {
                InitializeComponent();

                this.listSign = listsign;
                this.actAfterSave = actAfterSave;
                //this.SignParanel = checkSignParanel;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSignerAdd_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                this.InitComboCommon(this.cboSigner, this.GetSigner(), "ID", "USERNAME", "USERNAME", 150, "TITLE", 150, "DEPARTMENT_NAME", 200, "", 0);
                this.InitComboCommon(this.cboSignTemplate, this.GetSignTemplate(), "ID", "SIGN_TEMP_NAME", "SIGN_TEMP_CODE");

                GetDataSignParanel();

                this.chkISignParanel.Checked = this.SignParanel;
                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = this.listSign;
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataSignParanel()
        {
            try
            {
                var sarPrint96 = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096);
                if (sarPrint96 != null && !String.IsNullOrWhiteSpace(sarPrint96.EMR_DOCUMENT_TYPE_CODE))
                {
                    var documentType = BackendDataWorker.Get<EMR_DOCUMENT_TYPE>().FirstOrDefault(o => o.DOCUMENT_TYPE_CODE == sarPrint96.EMR_DOCUMENT_TYPE_CODE);
                    if (documentType != null && documentType.IS_SIGN_PARALLEL == 1)
                    {
                        this.SignParanel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<EMR_SIGNER> GetSigner()
        {
            this.signers = BackendDataWorker.Get<EMR_SIGNER>().Where(o => o.IS_ACTIVE == 1).OrderBy(o => o.USERNAME).ThenBy(o => o.NUM_ORDER).ToList();
            return signers;
        }

        /// <summary>
        /// - Bổ sung combobox "Mẫu thiết lập ký": Load dữ liệu từ bảng EMR_SIGN_TEMP. Chỉ hiển thị các dữu liệu đang mở khóa và do người dùng tạo order theo mã mẫu.
        ///- Nếu người dùng chọn "Mẫu thiết lập ký thì thực hiện:
        /// </summary>
        /// <returns></returns>
        private List<EMR_SIGN_TEMP> GetSignTemplate()
        {
            Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
            EmrSignTempFilter filter = new EmrSignTempFilter();
            filter.IS_ACTIVE = 1;
            this.signTemplates = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<EMR_SIGN_TEMP>>("api/EmrSignTemp/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, param);
            return signTemplates;
        }

        private EMR.EFMODEL.DataModels.EMR_SIGNER GetSignerById(long id)
        {
            return this.signers.FirstOrDefault(o => o.ID == id);
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, displayMember, 0, displayMemberCode, 0, "", 0, "", 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string col1Name, int col1Width, string col2Name, int col2Width, string col3Name, int col3Width, string col4Name, int col4Width)
        {
            try
            {
                int popupWidth = 0;
                int visibleIndex = 1;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(col1Name))
                {
                    columnInfos.Add(new ColumnInfo(col1Name, "", (col1Width > 0 ? col1Width : 250), visibleIndex, true));
                    popupWidth += (col1Width > 0 ? col1Width : 250);
                    visibleIndex += 1;
                }
                if (!String.IsNullOrEmpty(col2Name))
                {
                    columnInfos.Add(new ColumnInfo(col2Name, "", (col2Width > 0 ? col2Width : 100), visibleIndex, true));
                    popupWidth += (col2Width > 0 ? col2Width : 100);
                    visibleIndex += 1;
                }
                if (!String.IsNullOrEmpty(col3Name))
                {
                    columnInfos.Add(new ColumnInfo(col3Name, "", (col3Width > 0 ? col3Width : 100), visibleIndex, true));
                    popupWidth += (col3Width > 0 ? col3Width : 100);
                    visibleIndex += 1;
                }
                if (!String.IsNullOrEmpty(col4Name))
                {
                    columnInfos.Add(new ColumnInfo(col4Name, "", (col4Width > 0 ? col4Width : 100), visibleIndex, true));
                    popupWidth += (col4Width > 0 ? col4Width : 100);
                    visibleIndex += 1;
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);

                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        long GetMaxNumOrder()
        {
            long max = stepNumOrder;
            if (this.listSign != null && this.listSign.Count > 0)
            {
                max = this.listSign.Max(o => o.NumOrder) + stepNumOrder;
            }
            return max;
        }

        bool CheckExistsPatientSign()
        {
            bool valid = false;
            try
            {
                valid = this.listSign.Any(o => o.Loginname == "%me%");
            }
            catch (Exception ex)
            {
                valid = false;
            }
            return valid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboSigner.EditValue == null)
                {
                    MessageBox.Show("Chưa chọn người ký");
                    FocusShowpopup(cboSigner, false);
                    return;
                }

                SignTDO sign = new SignTDO();
                EMR_SIGNER signerFind = GetSignerById((long)cboSigner.EditValue);

                sign.SignerId = signerFind.ID;
                sign.Loginname = signerFind.LOGINNAME;
                sign.Username = signerFind.USERNAME;
                sign.FullName = signerFind.USERNAME;
                sign.FirstName = signerFind.USERNAME;
                sign.NumOrder = GetMaxNumOrder();
                sign.Title = signerFind.TITLE;
                sign.DepartmentCode = signerFind.DEPARTMENT_CODE;
                sign.DepartmentName = signerFind.DEPARTMENT_NAME;
                sign.SignTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                if (this.listSign == null)
                    this.listSign = new List<SignTDO>();

                this.listSign.Add(sign);

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = this.listSign.OrderBy(o => o.NumOrder).ToList();
                gridView1.EndUpdate();

                cboSigner.EditValue = null;
                txtLoginName.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.actAfterSave != null)
                {
                    var bchk = ((this.listSign != null && this.listSign.Count > 0) ? this.listSign.Any(o => o.NumOrder > 0) : true);
                    if (bchk)
                    {
                        this.actAfterSave(this.listSign, chkISignParanel.Checked);
                        this.Close();
                    }
                    else
                    {
                        MessageManager.Show("Danh sách người ký phải gán thứ tự ký");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                SignTDO data = (SignTDO)gridView1.GetFocusedRow();

                if (MessageBox.Show("Bạn có muốn xóa dữ liệu không", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.listSign.Remove(data);

                    gridView1.BeginUpdate();
                    gridView1.GridControl.DataSource = (this.listSign != null ? this.listSign.OrderBy(o => o.NumOrder).ToList() : null);
                    gridView1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SignTDO signTDO = (SignTDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (signTDO != null)
                    {
                        if (e.Column.FieldName == "UsernameDisplay")
                        {
                            e.Value = (String.IsNullOrEmpty(signTDO.Loginname) ? signTDO.FullName + " (Bệnh nhân)" : signTDO.Username);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboSigner.EditValue = null;
                        this.FocusShowpopup(cboSigner, true);
                    }
                    else
                    {
                        var data = this.signers
                            .Where(o => o.IS_ACTIVE == 1 && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboSigner.EditValue = searchResult[0].ID;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.btnAdd.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            this.cboSigner.EditValue = null;
                            this.FocusShowpopup(cboSigner, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSigner_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboSigner.EditValue != null)
                    {
                        var data = this.signers.FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == TypeConvertParse.ToInt64((this.cboSigner.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }

                    this.btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSigner_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboSigner.EditValue != null)
                    {
                        var data = this.signers.FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == TypeConvertParse.ToInt64((this.cboSigner.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                            this.btnAdd.Focus();
                        }
                    }
                }
                else
                {
                    this.cboSigner.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(this.cboSigner);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSignTemplate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboSignTemplate.EditValue != null)
                    {
                        var data = this.signTemplates.FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == TypeConvertParse.ToInt64((this.cboSignTemplate.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            ProcessSelectSignTemp(data);
                            this.btnAdd.Focus();
                        }
                    }
                }
                else
                {
                    this.cboSigner.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(this.cboSigner);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSelectSignTemp(EMR_SIGN_TEMP signTemp)
        {
            try
            {
                gridControl1.DataSource = null;
                this.listSign = new List<SignTDO>();

                List<EMR_SIGN_ORDER> signOrders = EmrSignOrderGetByTemp(signTemp.ID);
                if (signOrders != null && signOrders.Count > 0)
                {
                    foreach (var item in signOrders)
                    {
                        SignTDO sign = new SignTDO();
                        if (item.SIGNER_ID.HasValue && item.SIGNER_ID.Value > 0 && (item.IS_PATIENT_SIGN ?? 0) != 1)
                        {
                            EMR_SIGNER signerFind = GetSignerById(item.SIGNER_ID.Value);
                            if (signerFind != null)
                            {
                                sign.SignerId = signerFind.ID;
                                sign.Loginname = signerFind.LOGINNAME;
                                sign.Username = signerFind.USERNAME;
                                sign.FullName = signerFind.USERNAME;
                                sign.FirstName = signerFind.USERNAME;

                                sign.Title = signerFind.TITLE;
                                sign.DepartmentCode = signerFind.DEPARTMENT_CODE;
                                sign.DepartmentName = signerFind.DEPARTMENT_NAME;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("ProcessSelectSignTemp: khong tim thay signer theo thong tin signorder____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        sign.NumOrder = item.NUM_ORDER;
                        sign.SignTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        if (this.listSign == null)
                            this.listSign = new List<SignTDO>();

                        this.listSign.Add(sign);
                    }
                }

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = this.listSign.OrderBy(o => o.NumOrder).ToList();
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<EMR_SIGN_ORDER> EmrSignOrderGetByTemp(long signTemId)
        {
            List<EMR_SIGN_ORDER> result = null;
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                EmrSignOrderFilter filter = new EmrSignOrderFilter();
                filter.IS_ACTIVE = 1;
                filter.SIGN_TEMP_ID = signTemId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<EMR_SIGN_ORDER>>("api/EmrSignOrder/Get", ApiConsumer.ApiConsumers.EmrConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void cboSignTemplate_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboSignTemplate.EditValue != null)
                    {
                        var data = this.signTemplates.FirstOrDefault(o => o.IS_ACTIVE == 1 && o.ID == TypeConvertParse.ToInt64((this.cboSignTemplate.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            ProcessSelectSignTemp(data);
                        }
                    }

                    this.btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddMe_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckExistsPatientSign())
                {
                    MessageManager.Show("Tôi đã có trong luồng ký");
                    return;
                }

                SignTDO sign = new SignTDO();
                sign.Username = "Tôi (người sử dụng)";
                sign.NumOrder = GetMaxNumOrder();
                sign.Loginname = "%me%";

                if (this.listSign == null)
                    this.listSign = new List<SignTDO>();

                this.listSign.Add(sign);

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = this.listSign.OrderBy(o => o.NumOrder).ToList();
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmSignerAdd
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd = new ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Lang", typeof(frmSignerAdd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.btnAddMe.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.btnAddMe.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSignerAdd.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmSignerAdd.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmSignerAdd.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmSignerAdd.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmSignerAdd.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.cboSigner.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSignerAdd.cboSigner.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.chkISignParanel.Properties.Caption = Inventec.Common.Resource.Get.Value("frmSignerAdd.chkISignParanel.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.cboSignTemplate.Properties.NullText = Inventec.Common.Resource.Get.Value("frmSignerAdd.cboSignTemplate.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSignerAdd.Text", Resources.ResourceLanguageManager.LanguageResource__frmSignerAdd, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
