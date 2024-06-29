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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using DevExpress.XtraExport;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
namespace HIS.UC.WorkPlace
{
    public partial class UCWorkPlaceCombo : DevExpress.XtraEditors.XtraUserControl
    {
        List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces;
        DelegateFocusMoveout focusMoveout;
        HIS.UC.WorkPlace.DelegatePlusClick plusClick;
        int positionHandleControlPatientInfo = -1;
        bool validate = false;

        public UCWorkPlaceCombo(DelegateFocusMoveout focusMoveout, HIS.UC.WorkPlace.DelegatePlusClick plusClick, List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces)
        {
            try
            {
                InitializeComponent();
                Language_ucWorkPlaceCombo();
                this.focusMoveout = focusMoveout;
                this.plusClick = plusClick;
                this.worlPlaces = worlPlaces;
                LoadDataCombo(cboWorkPlace, this.worlPlaces);
                cboWorkPlace.Properties.Buttons[2].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public UCWorkPlaceCombo(DelegateFocusMoveout focusMoveout, HIS.UC.WorkPlace.DelegatePlusClick plusClick, List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> worlPlaces, bool validate)
        {
            try
            {
                this.validate = validate;
                InitializeComponent();
                //if (this.validate)
                //{
                //    this.lblNoiLamViec.AppearanceItemCaption.ForeColor = Color.Maroon;
                //    ValidateGridLookupWithTextEdit(cboWorkPlace, txtWorkPlaceCode);
                //}
                Language_ucWorkPlaceCombo();
                this.focusMoveout = focusMoveout;
                this.plusClick = plusClick;
                this.worlPlaces = worlPlaces;
                LoadDataCombo(cboWorkPlace, this.worlPlaces);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Language_ucWorkPlaceCombo()
        {
            try
            {
                ////Khoi tao doi tuong resource
                His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.WorkPlace.Resources.Lang", typeof(UCWorkPlaceCombo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceCombo.layoutControl1.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboWorkPlace.Properties.NullText = Inventec.Common.Resource.Get.Value("UCWorkPlaceCombo.cboWorkPlace.Properties.NullText", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCWorkPlaceCombo.layoutControlItem2.Text", His.UC.WorkPlace.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCWorkPlaceCombo_Load(object sender, EventArgs e)
        {
            LoadDataCombo(cboWorkPlace, this.worlPlaces);
        }

        public long? GetValue()
        {
            long? result = null;
            try
            {
                if (this.validate)
                {
                    if (!dxValidationProvider1.Validate())
                        return null;
                    else
                    {
                        result = (long?)cboWorkPlace.EditValue;
                        dxValidationProvider1.RemoveControlError(cboWorkPlace);
                    }

                    //if (cboWorkPlace.EditValue != null)
                    //{
                    //    result = (long?)cboWorkPlace.EditValue;
                    //    dxValidationProvider1.RemoveControlError(cboWorkPlace);
                    //}
                    //else
                    //{
                    //    if (dxValidationProvider1.Validate())
                    //        return null;
                    //}
                }
                else
                {
                    result = (long?)cboWorkPlace.EditValue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public void FocusControl()
        {
            try
            {
                cboWorkPlace.Focus();
                cboWorkPlace.SelectAll();
                cboWorkPlace.ShowPopup();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(object data)
        {
            try
            {
                if (data is MOS.EFMODEL.DataModels.HIS_WORK_PLACE)
                {
                    //txtWorkPlaceCode.Text = ((MOS.EFMODEL.DataModels.HIS_WORK_PLACE)data).WORK_PLACE_CODE;
                    cboWorkPlace.EditValue = ((MOS.EFMODEL.DataModels.HIS_WORK_PLACE)data).ID;
                    cboWorkPlace.Properties.Buttons[2].Visible = true;
                }
                else
                {
                    //txtWorkPlaceCode.Text = "";
                    cboWorkPlace.Properties.Buttons[2].Visible = false;
                    cboWorkPlace.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadWorkPlaceCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboWorkPlace.EditValue = null;
                    cboWorkPlace.Focus();
                    cboWorkPlace.ShowPopup();
                }
                else
                {
                    var data = this.worlPlaces.Where(o => o.WORK_PLACE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboWorkPlace.EditValue = data[0].ID;
                            //txtWorkPlaceCode.Text = data[0].WORK_PLACE_CODE;
                            if (focusMoveout != null)
                            {
                                focusMoveout();
                            }
                        }
                        else
                        {
                            var singleData = this.worlPlaces.FirstOrDefault(o => o.WORK_PLACE_CODE.ToLower().Equals(searchCode.ToLower()));
                            if (singleData != null)
                            {
                                cboWorkPlace.EditValue = singleData.ID;
                                //txtWorkPlaceCode.Text = singleData.WORK_PLACE_CODE;
                                if (focusMoveout != null)
                                {
                                    focusMoveout();
                                }
                            }
                            else
                            {
                                cboWorkPlace.EditValue = null;
                                cboWorkPlace.Focus();
                                cboWorkPlace.ShowPopup();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboWorkPlace.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_WORK_PLACE ethnic = this.worlPlaces.SingleOrDefault(o => o.ID == (long)(cboWorkPlace.EditValue ?? 0));
                        if (ethnic != null)
                        {
                            //txtWorkPlaceCode.Text = ethnic.WORK_PLACE_CODE;
                            cboWorkPlace.Properties.Buttons[2].Visible = true;
                            if (focusMoveout != null)
                            {
                                focusMoveout();
                            }
                        }
                        else
                        {
                            cboWorkPlace.Properties.Buttons[2].Visible = false;
                        }
                    }
                    else
                    {
                        focusMoveout();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadData(object data)
        {
            try
            {
                LoadDataCombo(cboWorkPlace, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataCombo(DevExpress.XtraEditors.GridLookUpEdit cboWorkPlace, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 180, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboWorkPlace, (data != null ? data : this.worlPlaces), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtWorkPlaceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadWorkPlaceCombo((sender as DevExpress.XtraEditors.TextEdit).Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisWorkPlace").FirstOrDefault();
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData,currentModule.RoomId,currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData,currentModule.RoomId,currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                        //CommonParam param = new CommonParam();
                        //HisWorkPlaceFilter filter = new HisWorkPlaceFilter();
                        //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        //this.worlPlaces = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>>("api/HisWorkPlace/Get", ApiConsumers.MosConsumer, filter, param).ToList();

                        //LoadDataCombo(cboWorkPlace, this.worlPlaces);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.worlPlaces), this.worlPlaces));
                    }     
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboWorkPlace.EditValue = null;
                    cboWorkPlace.Properties.Buttons[2].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboWorkPlace_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboWorkPlace.Properties.Buttons[2].Visible = false;
                if (cboWorkPlace.EditValue != null)
                {
                    cboWorkPlace.Properties.Buttons[2].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboWorkPlace.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_WORK_PLACE data = this.worlPlaces.SingleOrDefault(o => o.ID == (long)(cboWorkPlace.EditValue ?? 0));
                        if (data != null)
                        {
                            //txtWorkPlaceCode.Text = data.WORK_PLACE_CODE;
                            cboWorkPlace.Properties.Buttons[2].Visible = true;
                            if (focusMoveout != null)
                            {
                                focusMoveout();
                            }
                        }
                        else
                        {
                            cboWorkPlace.Properties.Buttons[2].Visible = false;
                        }
                    }
                    else
                    {
                        if (focusMoveout != null)
                        {
                            focusMoveout();
                        }
                    }
                }
                //else if (e.KeyCode == Keys.Tab)
                //{
                //    if (focusMoveout != null)
                //    {
                //        focusMoveout();
                //    }
                //}
                //else //if (e.KeyCode == Keys.Down)
                //{
                //    if (focusMoveout != null)
                //    {
                //        focusMoveout();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_Leave(object sender, EventArgs e)
        {

        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                try
                {
                    BaseEdit edit = e.InvalidControl as BaseEdit;
                    if (edit == null)
                        return;

                    BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                    if (viewInfo == null)
                        return;

                    if (positionHandleControlPatientInfo == -1)
                    {
                        positionHandleControlPatientInfo = edit.TabIndex;
                        if (edit.Visible)
                        {
                            edit.SelectAll();
                            edit.Focus();
                        }
                    }
                    if (positionHandleControlPatientInfo > edit.TabIndex)
                    {
                        positionHandleControlPatientInfo = edit.TabIndex;
                        if (edit.Visible)
                        {
                            edit.SelectAll();
                            edit.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
