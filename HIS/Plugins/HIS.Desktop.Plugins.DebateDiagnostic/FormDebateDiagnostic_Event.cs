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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.DebateDiagnostic.Resources;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    public partial class FormDebateDiagnostic : HIS.Desktop.Utility.FormBase
    {
        #region click
        private void ButtonAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ADO.HisDebateUserADO participant = new ADO.HisDebateUserADO();
                lstParticipantDebate = gridControl.DataSource as List<ADO.HisDebateUserADO>;
                if (lstParticipantDebate != null && lstParticipantDebate.Count > 0)
                {
                    ADO.HisDebateUserADO parti = new ADO.HisDebateUserADO();
                    lstParticipantDebate.Add(parti);
                    lstParticipantDebate.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    lstParticipantDebate.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl.DataSource = null;
                    gridControl.DataSource = lstParticipantDebate;
                }
                else
                {
                    ADO.HisDebateUserADO parti = new ADO.HisDebateUserADO();
                    lstParticipantDebate.Add(parti);
                    lstParticipantDebate.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl.DataSource = null;
                    gridControl.DataSource = lstParticipantDebate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TextEditLoginName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DevExpress.XtraEditors.BaseEdit edit = sender as DevExpress.XtraEditors.BaseEdit;
                    if (edit != null)
                    {
                        var icds = Base.GlobalStore.HisAcsUser.Where(o => o.LOGINNAME.ToLower().Contains(edit.EditValue.ToString().ToLower())).ToList();
                        if (icds != null && icds.Count > 0)
                        {
                            if (icds.Count == 1)
                            {
                                gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_LoginName, icds[0].LOGINNAME);
                                gridView.SetRowCellValue(gridView.FocusedRowHandle, gridColumnParticipants_Id, icds[0].ID);
                                gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_UserName, icds[0].ID);
                            }
                            else
                            {
                                gridView.FocusedColumn = Gc_UserName;
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

        private void LookUpEditUserName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit edit = sender as HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        var participant = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == edit.EditValue.ToString());
                        if (participant != null)
                        {
                            gridView.SetRowCellValue(gridView.FocusedRowHandle, Gc_LoginName, participant.LOGINNAME);
                            gridView.SetRowCellValue(gridView.FocusedRowHandle, gridColumnParticipants_Id, participant.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var participant = (ADO.HisDebateUserADO)gridView.GetFocusedRow();
                if (participant != null)
                {
                    lstParticipantDebate.Remove(participant);
                    lstParticipantDebate.ForEach(o => o.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit);
                    lstParticipantDebate.LastOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    gridControl.DataSource = null;
                    gridControl.DataSource = lstParticipantDebate;
                }
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
                if (treatment_id == 0)
                    return;
                this.positionHandleControl = -1;

                //ValidationControl();
                if (!dxValidationProvider1.Validate() || (detailProcessor != null && detailProcessor.ValidateControl(GetTypeDetail())))
                    return;

                WaitingManager.Show();

                MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate = new MOS.EFMODEL.DataModels.HIS_DEBATE();
                if (hisService != null)
                {
                    hisDebate.SERVICE_ID = hisService.ID;
                }
                if (this.action == GlobalVariables.ActionAdd)
                {
                    ProcessHisDebate(hisDebate);
                    hisDebate.TREATMENT_ID = this.treatment_id;
                    if (!string.IsNullOrEmpty(cboRequestLoggin.EditValue.ToString()))
                    {
                        hisDebate.REQUEST_LOGINNAME = acsUsers.FirstOrDefault(o => o.LOGINNAME == cboRequestLoggin.EditValue.ToString()).LOGINNAME;
                        hisDebate.REQUEST_USERNAME = acsUsers.FirstOrDefault(o => o.LOGINNAME == cboRequestLoggin.EditValue.ToString()).USERNAME;
                    }
                    if (cboDepartment.EditValue != null)
                    {
                        hisDebate.DEPARTMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
                    }
                }
                else if (this.action == GlobalVariables.ActionEdit && currentHisDebate != null)
                {
                    ProcessHisDebate(currentHisDebate);
                    hisDebate = currentHisDebate;
                }
                if (cboPhieuDieuTri.EditValue != null)
                {
                    hisDebate.TRACKING_ID = (long)cboPhieuDieuTri.EditValue;
                }
                else
                {
                    hisDebate.TRACKING_ID = null;
                }

                // luu dich vu hoi chuan
                ProcessHisDebateUser(hisDebate);
                ProcessHisDebateInvateUser(hisDebate);
                // check debateUser
                if (lciAutoCreateEmr.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always &&
                    chkAutoCreateEmr.Checked &&
                    chkAutoSign.Checked &&
                    hisDebate.HIS_DEBATE_USER != null &&
                    hisDebate.HIS_DEBATE_USER.Count > 0)
                {
                    var emrSigner = BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_SIGNER>();
                    var checkNotConfig = hisDebate.HIS_DEBATE_USER.Where(o => emrSigner == null || !emrSigner.Select(p => p.LOGINNAME).Contains(o.LOGINNAME)).ToList();
                    if (checkNotConfig != null && checkNotConfig.Count > 0)
                    {
                        string mess = String.Join(",", checkNotConfig.Select(o => o.LOGINNAME));
                        MessageBox.Show(String.Format("Tài khoản {0} chưa được tạo thông tin người ký", mess), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (hisDebate != null)
                {
                    if (hisDebate.HIS_DEBATE_USER != null && hisDebate.HIS_DEBATE_USER.Count > 0)
                    {
                        List<string> checkUser = new List<string>();

                        foreach (var item in hisDebate.HIS_DEBATE_USER)
                        {
                            var DemUser = hisDebate.HIS_DEBATE_USER.Where(o => o.LOGINNAME == item.LOGINNAME).Count();
                            if (DemUser >= 2)
                            {
                                checkUser.Add(item.LOGINNAME);
                            }
                        }
                        if (checkUser != null && checkUser.Count > 0)
                        {
                            string messUser = String.Join(",", checkUser.Distinct().ToList());
                            WaitingManager.Hide();
                            MessageBox.Show(String.Format(ResourceMessage.TaiKhoanBilap, messUser), ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (chkAutoSign.Checked && (!hisDebate.HIS_DEBATE_USER.ToList().Exists(o => o.IS_PRESIDENT == 1) || !hisDebate.HIS_DEBATE_USER.ToList().Exists(o => o.IS_SECRETARY == 1)))
                        {
                            WaitingManager.Hide();
                            MessageBox.Show("Bắt buộc có thành phần tham gia là Chủ tọa/Thư ký", ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else if (chkAutoSign.Checked)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Bắt buộc có thành phần tham gia là Chủ tọa/Thư ký", ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (hisDebate != null && hisDebate.HIS_DEBATE_INVITE_USER != null && hisDebate.HIS_DEBATE_INVITE_USER.Count > 0)
                {
                    List<string> checkUser = new List<string>();

                    foreach (var item in hisDebate.HIS_DEBATE_INVITE_USER)
                    {
                        var DemUser = hisDebate.HIS_DEBATE_INVITE_USER.Where(o => o.LOGINNAME == item.LOGINNAME).Count();
                        if (DemUser >= 2)
                        {
                            checkUser.Add(item.LOGINNAME);
                        }
                    }
                    if (checkUser != null && checkUser.Count > 0)
                    {
                        string messUser = String.Join(",", checkUser.Distinct().ToList());
                        WaitingManager.Hide();
                        MessageBox.Show(String.Format(ResourceMessage.TaiKhoanBilap, messUser), ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                //if (this.medicinePrints != null && this.medicinePrints.Count > 0)
                //{
                //    hisDebate.MEDICINE_CONCENTRA = this.medicinePrints[0].MEDICINE_CONCENTRA;
                //    hisDebate.MEDICINE_TUTORIAL = this.medicinePrints[0].MEDICINE_TUTORIAL;
                //    hisDebate.MEDICINE_TYPE_NAME = this.medicinePrints[0].MEDICINE_TYPE_NAME;
                //    hisDebate.MEDICINE_USE_FORM_NAME = this.medicinePrints[0].MEDICINE_USE_FORM_NAME;
                //    hisDebate.MEDICINE_USE_TIME = this.medicinePrints[0].MEDICINE_USE_TIME;
                //}

                if (!CheckValidation(hisDebate)) return;
                //trong hàm dưới là gọi api để lưu
                SaveHisDebate(hisDebate);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private bool CheckValidation(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {
            //bool result = true;
            try
            {
                if (hisDebate == null) return false;
                if (hisDebate.DEBATE_TIME != null && hisDebate.DEBATE_TIME < vHisTreatment.IN_TIME)
                {
                    MessageBox.Show(String.Format(
                        Resources.ResourceMessage.ThoiGianHoiChanKhongDuocNhoHonThoiGianVao,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vHisTreatment.IN_TIME)));
                    return false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private void cboDebateTemp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtDebateTemp.Text = "";
                    cboDebateTemp.EditValue = null;
                    FillDatatoControlByHisDebateTemp(null);
                    txtDebateTemp.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDebateTemp_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDebateTemp.EditValue != null && cboDebateTemp.EditValue != cboDebateTemp.OldEditValue)
                    {
                        var DebateTemp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEBATE_TEMP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDebateTemp.EditValue ?? "").ToString()));
                        if (DebateTemp != null)
                        {
                            FillDatatoControlByHisDebateTemp(DebateTemp);
                            txtDebateTemp.Text = DebateTemp.DEBATE_TEMP_CODE;
                            txtIcdMain.Focus();
                        }
                    }
                    else
                    {
                        txtIcdMain.Focus();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRequestLoggin_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboRequestLoggin.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = acsUsers.FirstOrDefault(o => o.LOGINNAME == (this.cboRequestLoggin.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtRequestLoggin.Text = data.LOGINNAME;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDebateTemp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadDebateTempCombo(strValue);
                    if (cboDebateTemp.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_DEBATE_TEMP>().FirstOrDefault(p => p.ID == Int64.Parse(cboDebateTemp.EditValue.ToString()));
                        FillDatatoControlByHisDebateTemp(data);
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DEBATE_TEMP data = new HIS_DEBATE_TEMP();
                ProcessHisDebateTemp(data);
                List<object> ListArgs = new List<object>();
                ListArgs.Add(data);
                if (this.moduleData != null)
                {
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisDebateTemp", this.moduleData.RoomId, this.moduleData.RoomTypeId, ListArgs);
                }
                else
                {
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisDebateTemp", 0, 0, ListArgs);
                }
                InitComboDebateTemp();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSaveTemp.Enabled)
                {
                    btnSaveTemp_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region enter
        private void dtDebateTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtRequestLoggin.Enabled == true)
                    {
                        txtRequestLoggin.Focus();
                        txtRequestLoggin.SelectAll();
                    }
                    else
                    {
                        cboDebateType.Focus();
                        if (cboDebateType.EditValue == null)
                            cboDebateType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRequestContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboRequestLoggin.EditValue = acsUsers[0].LOGINNAME;

                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboRequestLoggin.EditValue = searchResult[0].LOGINNAME;
                            this.txtRequestLoggin.Text = searchResult[0].LOGINNAME;

                        }
                        else
                        {
                            this.cboRequestLoggin.EditValue = acsUsers[0].LOGINNAME;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void icdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    checkEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtIcdMain.Text.Trim()))
                    {
                        string code = txtIcdMain.Text.Trim();
                        var listData = Base.GlobalStore.HisIcds.Where(o => o.ICD_CODE.Equals(code)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            showCbo = false;
                            txtIcdMain.Text = listData.First().ICD_CODE;
                            cboIcdMain.EditValue = listData.First().ID;
                            icdMainText.Text = listData.First().ICD_NAME;
                            checkEdit.Focus();
                            if (checkEdit.Checked)
                            {
                                icdMainText.Focus();
                                icdMainText.SelectAll();
                            }
                            else
                            {
                                cboIcdMain.Focus();
                                cboIcdMain.SelectAll();
                            }
                        }
                    }
                    if (showCbo)
                    {
                        cboIcdMain.Focus();
                        cboIcdMain.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void cboIcdMain_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboIcdMain.Text != null)
                    {
                        var data = Base.GlobalStore.HisIcds.FirstOrDefault(o => o.ID == (long)(cboIcdMain.EditValue ?? 0));
                        if (data != null)
                        {
                            txtIcdMain.Text = data.ICD_CODE;
                            checkEdit.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboIcdMain.Text != null)
                    {
                        var data = Base.GlobalStore.HisIcds.FirstOrDefault(o => o.ID == (long)(cboIcdMain.EditValue ?? 0));
                        if (data != null)
                        {
                            txtIcdMain.Text = data.ICD_CODE;
                            checkEdit.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkEdit.Checked == true)
                {
                    cboIcdMain.Visible = false;
                    icdMainText.Visible = true;
                    icdMainText.Text = cboIcdMain.Text;
                    icdMainText.Focus();
                    icdMainText.SelectAll();
                }
                else if (checkEdit.Checked == false)
                {
                    icdMainText.Visible = false;
                    cboIcdMain.Visible = true;
                    icdMainText.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdTextCode.Focus();
                    txtIcdTextCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdTextName.Focus();
                    txtIcdTextName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdTextCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtIcdTextCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        //var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = Base.GlobalStore.HisIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdTextCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdTextCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdTextCode.Focus();
                            txtIcdTextCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtIcdTextCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdTextName.Text == txtIcdTextName.Properties.NullValuePrompt ? "" : txtIcdTextName.Text);
                txtIcdTextName.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdTextName.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdTextCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = Base.GlobalStore.HisIcds.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void lciIcdTextName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    //hien thi popup chon icd
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SecondaryIcd");
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(stringIcds, txtIcdTextCode.Text, txtIcdTextName.Text);

                        List<object> listArgs = new List<object>();
                        listArgs.Add(secondaryIcdADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(Resources.ResourceMessage.TaiKhoanKhongCoQuyenThucHienChucNang);
                    }
                }
                if (e.KeyCode == Keys.Enter)
                {
                    dtInTime.Focus();
                    dtInTime.SelectAll();
                    dtInTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void stringIcds(string _icdCode, string _icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(_icdCode))
                {
                    txtIcdTextCode.Text = _icdCode;
                }
                if (!string.IsNullOrEmpty(_icdName))
                {
                    txtIcdTextName.Text = _icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtInTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtOutTime.Focus();
                    dtOutTime.SelectAll();
                    dtOutTime.ShowPopup();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepartment.Enabled == true)
                    {
                        cboDepartment.SelectAll();
                        cboDepartment.ShowPopup();
                    }
                    else
                    {
                        dtDebateTime.SelectAll();
                        dtDebateTime.ShowPopup();
                        dtDebateTime.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtDebateTime.SelectAll();
                    dtDebateTime.ShowPopup();
                    dtDebateTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLocation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    txtMedicineName.SelectAll();
                //    txtMedicineName.Focus();

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void txtMedicineName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtConcena.SelectAll();
        //            txtConcena.Focus();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void txtConcena_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtUseForm.SelectAll();
        //            txtUseForm.Focus();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void txtUseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            dtTimeUse.SelectAll();
        //            dtTimeUse.ShowPopup();
        //            dtTimeUse.Focus();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void dtTimeUse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtUserManual.SelectAll();
        //            txtUserManual.Focus();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void txtUserManual_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtRequestContent.SelectAll();
        //            txtRequestContent.Focus();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion

        private void SetDataHisDebate(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {
            try
            {

                txtIcdTextName.Text = hisDebate.ICD_TEXT;
                txtIcdTextCode.Text = hisDebate.ICD_SUB_CODE;
                if (!string.IsNullOrEmpty(hisDebate.ICD_CODE))
                {
                    var icd = Base.GlobalStore.HisIcds.Where(o => o.ICD_CODE == hisDebate.ICD_CODE).FirstOrDefault();
                    cboIcdMain.EditValue = icd.ICD_CODE;
                    txtIcdMain.Text = icd.ICD_CODE;
                }
                if (!string.IsNullOrEmpty(hisDebate.ICD_NAME))
                {
                    checkEdit.Checked = true;
                    icdMainText.Text = hisDebate.ICD_NAME;
                }

                if (hisDebate.DEBATE_TYPE_ID != null)
                    cboDebateType.EditValue = hisDebate.DEBATE_TYPE_ID;
                else
                    cboDebateType.EditValue = null;

                cboDebateReason.EditValue = hisDebate.DEBATE_REASON_ID;

                txtLocation.Text = hisDebate.LOCATION;
                List<ADO.HisDebateUserADO> lstHisDebateUserADO = new List<ADO.HisDebateUserADO>();
                if (hisDebate.ID > 0)
                {
                    MOS.Filter.HisDebateUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateUserFilter();
                    hisDebateUserFilter.DEBATE_ID = hisDebate.ID;
                    List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_USER_GET, ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    if (lstHisDebateUser != null && lstHisDebateUser.Count > 0)
                    {
                        foreach (var item_DebateUser in lstHisDebateUser)
                        {
                            ADO.HisDebateUserADO hisDebateUserADO = new ADO.HisDebateUserADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisDebateUserADO>(hisDebateUserADO, item_DebateUser);
                            if (item_DebateUser.IS_PRESIDENT == 1)
                            {
                                hisDebateUserADO.PRESIDENT = true;
                            }

                            if (item_DebateUser.IS_SECRETARY == 1)
                            {
                                hisDebateUserADO.SECRETARY = true;
                            }
                            if (item_DebateUser.IS_OUT_OF_HOSPITAL == 1)
                            {
                                hisDebateUserADO.OUT_OF_HOSPITAL = true;
                            }
                            hisDebateUserADO.Action = GlobalVariables.ActionEdit;
                            lstHisDebateUserADO.Add(hisDebateUserADO);
                        }
                    }
                }
                if (lstHisDebateUserADO != null && lstHisDebateUserADO.Count > 0)
                {
                    lstHisDebateUserADO[0].Action = GlobalVariables.ActionAdd;
                    gridControl.DataSource = lstHisDebateUserADO;
                    lstParticipantDebate = lstHisDebateUserADO;
                }
                else
                {
                    FillDataToParticipants();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataDebateInvateUser(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate)
        {
            try
            {
                if (hisDebate.ID > 0)
                {
                    MOS.Filter.HisDebateInviteUserFilter hisDebateUserFilter = new MOS.Filter.HisDebateInviteUserFilter();
                    hisDebateUserFilter.DEBATE_ID = hisDebate.ID;
                    List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER> lstHisDebateUser = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>>("apiHisDebateInvateUser/Get", ApiConsumer.ApiConsumers.MosConsumer, hisDebateUserFilter, null);
                    if (lstHisDebateUser != null && lstHisDebateUser.Count > 0)
                    {
                        foreach (var item_DebateUser in lstHisDebateUser)
                        {
                            ADO.HisDebateInvateUserADO hisDebateUserADO = new ADO.HisDebateInvateUserADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ADO.HisDebateInvateUserADO>(hisDebateUserADO, item_DebateUser);
                            if (item_DebateUser.IS_PRESIDENT == 1)
                            {
                                hisDebateUserADO.PRESIDENT = true;
                            }

                            if (item_DebateUser.IS_SECRETARY == 1)
                            {
                                hisDebateUserADO.SECRETARY = true;
                            }
                            hisDebateUserADO.Action = GlobalVariables.ActionEdit;
                            lstDebateInvateUserADO.Add(hisDebateUserADO);
                        }
                    }
                }
                if (lstDebateInvateUserADO != null && lstDebateInvateUserADO.Count > 0)
                {
                    lstDebateInvateUserADO[0].Action = GlobalVariables.ActionAdd;
                    gridControl1.DataSource = lstDebateInvateUserADO;
                }
                else
                {
                    FillDataToInvateUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Xử lý lưu hội chẩn
        private void ProcessHisDebate(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebateSave)
        {
            try
            {
                if (string.IsNullOrEmpty(icdMainText.Text))
                {
                    if (cboIcdMain != null)
                    {
                        hisDebateSave.ICD_NAME = cboIcdMain.Text;
                        hisDebateSave.ICD_CODE = txtIcdMain.Text;
                    }
                }
                else
                {
                    hisDebateSave.ICD_NAME = icdMainText.Text;
                    if (cboIcdMain != null)
                    {
                        hisDebateSave.ICD_CODE = txtIcdMain.Text;
                    }
                }

                if (!string.IsNullOrEmpty(txtIcdTextName.Text))
                {
                    hisDebateSave.ICD_TEXT = txtIcdTextName.Text;
                    hisDebateSave.ICD_SUB_CODE = txtIcdTextCode.Text;
                }
                else
                {
                    hisDebateSave.ICD_TEXT = null;
                    hisDebateSave.ICD_SUB_CODE = null;
                }

                if (dtOutTime.EditValue != null && dtOutTime.DateTime != DateTime.MinValue)
                    hisDebateSave.TREATMENT_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtOutTime.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "00");
                else
                    hisDebateSave.TREATMENT_TO_TIME = null;

                if (dtInTime.EditValue != null && dtInTime.DateTime != DateTime.MinValue)
                    hisDebateSave.TREATMENT_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtInTime.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "00");
                else
                    hisDebateSave.TREATMENT_FROM_TIME = null;

                if (dtDebateTime.EditValue != null && dtDebateTime.DateTime != DateTime.MinValue)
                    hisDebateSave.DEBATE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDebateTime.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "00");
                else
                    hisDebateSave.DEBATE_TIME = null;

                if (cboDebateType.EditValue != null)
                    hisDebateSave.DEBATE_TYPE_ID = (long)cboDebateType.EditValue;
                else
                    hisDebateSave.DEBATE_TYPE_ID = null;

                hisDebateSave.DEBATE_REASON_ID = cboDebateReason.EditValue != null ? (long?)cboDebateReason.EditValue : null;

                hisDebateSave.LOCATION = txtLocation.Text;

                if (detailProcessor != null)
                {
                    HIS_DEBATE data = new HIS_DEBATE();

                    detailProcessor.GetData(GetTypeDetail(), ref data);
                    switch (GetTypeDetail())
                    {
                        case HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail.DetailEnum.Thuoc:
                            //qtcode
                            hisDebateSave.SUBCLINICAL_PROCESSES = data.SUBCLINICAL_PROCESSES;
                            //qtcode
                            hisDebateSave.CONTENT_TYPE = IMSys.DbConfig.HIS_RS.HIS_DEBATE.CONTENT_TYPE__MEDICINE;
                            hisDebateSave.MEDICINE_TYPE_IDS = data.MEDICINE_TYPE_IDS;
                            hisDebateSave.ACTIVE_INGREDIENT_IDS = data.ACTIVE_INGREDIENT_IDS;
                            hisDebateSave.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                            hisDebateSave.MEDICINE_CONCENTRA = data.MEDICINE_CONCENTRA;
                            hisDebateSave.MEDICINE_USE_FORM_NAME = data.MEDICINE_USE_FORM_NAME;
                            hisDebateSave.MEDICINE_TUTORIAL = data.MEDICINE_TUTORIAL;
                            hisDebateSave.DISCUSSION = data.DISCUSSION;
                            hisDebateSave.BEFORE_DIAGNOSTIC = data.BEFORE_DIAGNOSTIC;
                            hisDebateSave.TREATMENT_METHOD = data.TREATMENT_METHOD;
                            hisDebateSave.CARE_METHOD = data.CARE_METHOD;
                            hisDebateSave.CONCLUSION = data.CONCLUSION;
                            hisDebateSave.DIAGNOSTIC = data.DIAGNOSTIC;
                            hisDebateSave.HOSPITALIZATION_STATE = data.HOSPITALIZATION_STATE;
                            hisDebateSave.PATHOLOGICAL_HISTORY = data.PATHOLOGICAL_HISTORY;
                            hisDebateSave.TREATMENT_TRACKING = data.TREATMENT_TRACKING;
                            hisDebateSave.MEDICINE_USE_TIME = data.MEDICINE_USE_TIME;
                            hisDebateSave.REQUEST_CONTENT = "";
                            hisDebateSave.EMOTIONLESS_METHOD_ID = null;
                            hisDebateSave.INTERNAL_MEDICINE_STATE = "";
                            hisDebateSave.PROGNOSIS = "";
                            hisDebateSave.SURGERY_SERVICE_ID = null;
                            hisDebateSave.SURGERY_TIME = null;
                            hisDebateSave.HIS_DEBATE_EKIP_USER = null;
                            hisDebateSave.PTTT_METHOD_ID = null;
                            hisDebateSave.PTTT_METHOD_NAME = null;
                            break;
                        case HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail.DetailEnum.Pttt:
                            hisDebateSave.CONTENT_TYPE = IMSys.DbConfig.HIS_RS.HIS_DEBATE.CONTENT_TYPE__PRE_SURGRY;
                            hisDebateSave.EMOTIONLESS_METHOD_ID = data.EMOTIONLESS_METHOD_ID;
                            hisDebateSave.PTTT_METHOD_ID = data.PTTT_METHOD_ID;
                            hisDebateSave.PTTT_METHOD_NAME = data.PTTT_METHOD_NAME;
                            hisDebateSave.INTERNAL_MEDICINE_STATE = data.INTERNAL_MEDICINE_STATE;
                            hisDebateSave.PROGNOSIS = data.PROGNOSIS;
                            hisDebateSave.TREATMENT_METHOD = data.TREATMENT_METHOD;
                            hisDebateSave.CONCLUSION = data.CONCLUSION;
                            hisDebateSave.SUBCLINICAL_PROCESSES = data.SUBCLINICAL_PROCESSES;
                            hisDebateSave.SURGERY_SERVICE_ID = data.SURGERY_SERVICE_ID;
                            hisDebateSave.SURGERY_TIME = data.SURGERY_TIME;
                            hisDebateSave.HIS_DEBATE_EKIP_USER = data.HIS_DEBATE_EKIP_USER;
                            hisDebateSave.TREATMENT_TRACKING = data.TREATMENT_TRACKING;
                            hisDebateSave.MEDICINE_TYPE_IDS = "";
                            hisDebateSave.ACTIVE_INGREDIENT_IDS = "";
                            hisDebateSave.MEDICINE_TYPE_NAME = "";
                            hisDebateSave.MEDICINE_CONCENTRA = "";
                            hisDebateSave.MEDICINE_USE_FORM_NAME = "";
                            hisDebateSave.MEDICINE_USE_TIME = null;
                            hisDebateSave.MEDICINE_TUTORIAL = "";
                            hisDebateSave.DISCUSSION = data.DISCUSSION;
                            hisDebateSave.BEFORE_DIAGNOSTIC = data.BEFORE_DIAGNOSTIC;
                            hisDebateSave.CARE_METHOD = data.CARE_METHOD;
                            hisDebateSave.CONCLUSION = data.CONCLUSION;
                            hisDebateSave.DIAGNOSTIC = data.DIAGNOSTIC;
                            hisDebateSave.HOSPITALIZATION_STATE = data.HOSPITALIZATION_STATE;
                            hisDebateSave.PATHOLOGICAL_HISTORY = data.PATHOLOGICAL_HISTORY;
                            hisDebateSave.REQUEST_CONTENT = "";
                            break;
                        case HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail.DetailEnum.Khac:
                            //qtcode
                            hisDebateSave.SUBCLINICAL_PROCESSES = data.SUBCLINICAL_PROCESSES;
                            //qtcode
                            hisDebateSave.CONTENT_TYPE = IMSys.DbConfig.HIS_RS.HIS_DEBATE.CONTENT_TYPE__OTHER;
                            hisDebateSave.DISCUSSION = data.DISCUSSION;
                            hisDebateSave.BEFORE_DIAGNOSTIC = data.BEFORE_DIAGNOSTIC;
                            hisDebateSave.CARE_METHOD = data.CARE_METHOD;
                            hisDebateSave.CONCLUSION = data.CONCLUSION;
                            hisDebateSave.DIAGNOSTIC = data.DIAGNOSTIC;
                            hisDebateSave.TREATMENT_METHOD = data.TREATMENT_METHOD;
                            hisDebateSave.HOSPITALIZATION_STATE = data.HOSPITALIZATION_STATE;
                            hisDebateSave.PATHOLOGICAL_HISTORY = data.PATHOLOGICAL_HISTORY;
                            hisDebateSave.TREATMENT_TRACKING = data.TREATMENT_TRACKING;
                            hisDebateSave.REQUEST_CONTENT = data.REQUEST_CONTENT;
                            hisDebateSave.MEDICINE_TYPE_IDS = "";
                            hisDebateSave.ACTIVE_INGREDIENT_IDS = "";
                            hisDebateSave.MEDICINE_TYPE_NAME = "";
                            hisDebateSave.MEDICINE_CONCENTRA = "";
                            hisDebateSave.MEDICINE_USE_FORM_NAME = "";
                            hisDebateSave.MEDICINE_USE_TIME = null;
                            hisDebateSave.MEDICINE_TUTORIAL = "";
                            hisDebateSave.EMOTIONLESS_METHOD_ID = null;
                            hisDebateSave.INTERNAL_MEDICINE_STATE = "";
                            hisDebateSave.PROGNOSIS = "";
                            
                            hisDebateSave.SURGERY_SERVICE_ID = null;
                            hisDebateSave.SURGERY_TIME = null;
                            hisDebateSave.HIS_DEBATE_EKIP_USER = null;
                            hisDebateSave.PTTT_METHOD_ID = null;
                            hisDebateSave.PTTT_METHOD_NAME = null;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    hisDebateSave.MEDICINE_TYPE_IDS = "";
                    hisDebateSave.ACTIVE_INGREDIENT_IDS = "";
                    hisDebateSave.MEDICINE_TYPE_NAME = "";
                    hisDebateSave.MEDICINE_CONCENTRA = "";
                    hisDebateSave.MEDICINE_USE_FORM_NAME = "";
                    hisDebateSave.MEDICINE_USE_TIME = null;
                    hisDebateSave.MEDICINE_TUTORIAL = "";
                    hisDebateSave.DISCUSSION = "";
                    hisDebateSave.BEFORE_DIAGNOSTIC = "";
                    hisDebateSave.CARE_METHOD = "";
                    hisDebateSave.CONCLUSION = "";
                    hisDebateSave.DIAGNOSTIC = "";
                    hisDebateSave.TREATMENT_METHOD = "";
                    hisDebateSave.HOSPITALIZATION_STATE = "";
                    hisDebateSave.PATHOLOGICAL_HISTORY = "";
                    hisDebateSave.REQUEST_CONTENT = "";
                    hisDebateSave.CONTENT_TYPE = null;
                    hisDebateSave.EMOTIONLESS_METHOD_ID = null;
                    hisDebateSave.INTERNAL_MEDICINE_STATE = "";
                    hisDebateSave.PROGNOSIS = "";
                    hisDebateSave.SUBCLINICAL_PROCESSES = "";
                    hisDebateSave.SURGERY_SERVICE_ID = null;
                    hisDebateSave.SURGERY_TIME = null;
                    hisDebateSave.TREATMENT_TRACKING = "";
                    hisDebateSave.HIS_DEBATE_EKIP_USER = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessHisDebateTemp(MOS.EFMODEL.DataModels.HIS_DEBATE_TEMP hisDebatetemp)
        {
            try
            {
                if (detailProcessor != null)
                {
                    HIS_DEBATE data = new HIS_DEBATE();

                    detailProcessor.GetData(GetTypeDetail(), ref data);
                    ProcessHisDebateUser(data);

                    hisDebatetemp.BEFORE_DIAGNOSTIC = data.BEFORE_DIAGNOSTIC;
                    hisDebatetemp.CARE_METHOD = data.CARE_METHOD;
                    hisDebatetemp.CONCLUSION = data.CONCLUSION;
                    hisDebatetemp.DIAGNOSTIC = data.DIAGNOSTIC;
                    hisDebatetemp.TREATMENT_METHOD = data.TREATMENT_METHOD;
                    hisDebatetemp.HOSPITALIZATION_STATE = data.HOSPITALIZATION_STATE;
                    hisDebatetemp.PATHOLOGICAL_HISTORY = data.PATHOLOGICAL_HISTORY;
                    hisDebatetemp.REQUEST_CONTENT = data.REQUEST_CONTENT;
                    hisDebatetemp.TREATMENT_TRACKING = data.TREATMENT_TRACKING;
                    hisDebatetemp.HIS_DEBATE_USER = data.HIS_DEBATE_USER;
                    hisDebatetemp.DEPARTMENT_ID = WorkPlaceSDO.DepartmentId;
                    hisDebatetemp.DISCUSSION = data.DISCUSSION;

                }
                else
                {
                    HIS_DEBATE data = new HIS_DEBATE();
                    ProcessHisDebateUser(data);
                    hisDebatetemp.BEFORE_DIAGNOSTIC = "";
                    hisDebatetemp.CARE_METHOD = "";
                    hisDebatetemp.CONCLUSION = "";
                    hisDebatetemp.DIAGNOSTIC = "";
                    hisDebatetemp.HOSPITALIZATION_STATE = "";
                    hisDebatetemp.PATHOLOGICAL_HISTORY = "";
                    hisDebatetemp.REQUEST_CONTENT = "";
                    hisDebatetemp.TREATMENT_METHOD = "";
                    hisDebatetemp.TREATMENT_TRACKING = "";
                    hisDebatetemp.HIS_DEBATE_USER = data.HIS_DEBATE_USER;
                    hisDebatetemp.DEPARTMENT_ID = WorkPlaceSDO.DepartmentId;
                    hisDebatetemp.DISCUSSION = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessHisDebateUser(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebateSave)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> lstHisDebateUser = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
                List<ADO.HisDebateUserADO> lstDebateUser = gridControl.DataSource as List<ADO.HisDebateUserADO>;
                lstDebateUser = lstDebateUser != null ? lstDebateUser.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME) || (!String.IsNullOrEmpty(o.USERNAME) && o.OUT_OF_HOSPITAL)).ToList() : null;
                if (lstDebateUser != null && lstDebateUser.Count > 0)
                {
                    foreach (var item_DebateUser in lstDebateUser)
                    {
                        if (String.IsNullOrWhiteSpace(item_DebateUser.LOGINNAME) && !item_DebateUser.OUT_OF_HOSPITAL) continue;

                        MOS.EFMODEL.DataModels.HIS_DEBATE_USER hisDebateUser = new MOS.EFMODEL.DataModels.HIS_DEBATE_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>(hisDebateUser, item_DebateUser);
                        if (item_DebateUser.OUT_OF_HOSPITAL)
                        {
                            hisDebateUser.USERNAME = hisDebateUser.USERNAME;
                            hisDebateUser.IS_OUT_OF_HOSPITAL = 1;
                            hisDebateUser.DEBATE_TEMP_ID = null;
                            hisDebateUser.ID = 0;
                            if (item_DebateUser.PRESIDENT == true)
                                hisDebateUser.IS_PRESIDENT = 1;
                            else
                                hisDebateUser.IS_PRESIDENT = null;

                            if (item_DebateUser.SECRETARY == true)
                                hisDebateUser.IS_SECRETARY = 1;
                            else
                                hisDebateUser.IS_SECRETARY = null;
                            lstHisDebateUser.Add(hisDebateUser);
                        }
                        else
                        {
                            var name = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == item_DebateUser.LOGINNAME);
                            if (name != null)
                            {
                                hisDebateUser.LOGINNAME = name.LOGINNAME;
                                hisDebateUser.DEBATE_TEMP_ID = null;
                                if (!string.IsNullOrEmpty(name.USERNAME))
                                {
                                    hisDebateUser.USERNAME = name.USERNAME;
                                }
                                hisDebateUser.ID = 0;
                                if (item_DebateUser.PRESIDENT == true)
                                    hisDebateUser.IS_PRESIDENT = 1;
                                else
                                    hisDebateUser.IS_PRESIDENT = null;

                                if (item_DebateUser.SECRETARY == true)

                                    hisDebateUser.IS_SECRETARY = 1;
                                else
                                    hisDebateUser.IS_SECRETARY = null;

                                hisDebateUser.IS_OUT_OF_HOSPITAL = null;

                                lstHisDebateUser.Add(hisDebateUser);
                            }
                        }
                    }
                    if(lstHisDebateUser !=null && lstHisDebateUser.Count > 0)
                    {
                        hisDebateSave.HIS_DEBATE_USER = lstHisDebateUser;
                    }
                    else
                        hisDebateSave.HIS_DEBATE_USER = null;
                }
                else
                    hisDebateSave.HIS_DEBATE_USER = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessHisDebateInvateUser(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebateSave)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER> lstHisDebateUser = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>();
                List<ADO.HisDebateInvateUserADO> lstDebateUser = gridControl1.DataSource as List<ADO.HisDebateInvateUserADO>;
                lstDebateUser = lstDebateUser != null ? lstDebateUser.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).ToList() : null;
                if (lstDebateUser != null && lstDebateUser.Count > 0)
                {
                    if (lstDebateUser != null && lstDebateUser.Count > 0 && lstDebateUser.Where(o => o.PRESIDENT).ToList() != null && lstDebateUser.Where(o => o.PRESIDENT).ToList().Count > 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không được có hơn một chủ tọa", "Thông báo danh sách mời tham gia", MessageBoxButtons.OK);
                        return;
                    }
                    if (lstDebateUser != null && lstDebateUser.Count > 0 && lstDebateUser.Where(o => !string.IsNullOrEmpty(o.COMMENT_DOCTOR)).ToList() != null && lstDebateUser.Where(o => !string.IsNullOrEmpty(o.COMMENT_DOCTOR)).ToList().Count > 1)
                    {
                        string strName = "";
                        var lstMaxLengthCommentDoctor = lstDebateUser.Where(o => !string.IsNullOrEmpty(o.COMMENT_DOCTOR)).ToList();
                        foreach (var item in lstMaxLengthCommentDoctor)
                        {
                            if (Inventec.Common.String.CountVi.Count(item.COMMENT_DOCTOR) > 1000)
                            {
                                strName = item.LOGINNAME + " - " + Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME).USERNAME + " có \"Nhận xét\" vượt quá 1000 ký tự.\r\n";
                            }
                        }
                        if (!string.IsNullOrEmpty(strName))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(strName, "Thông báo danh sách mời tham gia", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    foreach (var item_DebateUser in lstDebateUser)
                    {
                        if (String.IsNullOrWhiteSpace(item_DebateUser.LOGINNAME)) continue;

                        MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER hisDebateUser = new MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEBATE_INVITE_USER>(hisDebateUser, item_DebateUser);
                        var name = Base.GlobalStore.HisAcsUser.FirstOrDefault(o => o.LOGINNAME == item_DebateUser.LOGINNAME);
                        if (name != null)
                        {
                            hisDebateUser.LOGINNAME = name.LOGINNAME;
                            if (!string.IsNullOrEmpty(name.USERNAME))
                            {
                                hisDebateUser.USERNAME = name.USERNAME;
                            }
                            hisDebateUser.ID = 0;
                            if (item_DebateUser.ID > 0)
                            {
                                hisDebateUser.ID = item_DebateUser.ID;
                            }
                            if (item_DebateUser.EXECUTE_ROLE_ID > 0)
                                hisDebateUser.DESCRIPTION = ListExecuteRole.FirstOrDefault(o => o.ID == item_DebateUser.EXECUTE_ROLE_ID).EXECUTE_ROLE_NAME;
                            if (item_DebateUser.PRESIDENT == true)
                                hisDebateUser.IS_PRESIDENT = 1;
                            else
                                hisDebateUser.IS_PRESIDENT = null;

                            if (item_DebateUser.SECRETARY == true)

                                hisDebateUser.IS_SECRETARY = 1;
                            else
                                hisDebateUser.IS_SECRETARY = null;

                            lstHisDebateUser.Add(hisDebateUser);
                            hisDebateSave.HIS_DEBATE_INVITE_USER = lstHisDebateUser;
                        }
                        else
                        {
                            hisDebateSave.HIS_DEBATE_INVITE_USER = null;
                        }
                    }
                }
                else
                    hisDebateSave.HIS_DEBATE_INVITE_USER = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveHisDebate(MOS.EFMODEL.DataModels.HIS_DEBATE hisDebateSave)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_DEBATE hisDebateResult = new MOS.EFMODEL.DataModels.HIS_DEBATE();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hisDebateSave__:", hisDebateSave));

                if (this.action == GlobalVariables.ActionAdd)
                {

                    //if (hisDebate == null) Inventec.Common.Logging.LogSystem.Warn("hisDebate = null"+"__________________________________");
                    //ProcessHisDebate(hisDebate);
                    //hisDebateSave.TREATMENT_ID = this.treatment_id;
                    //if (cboPhieuDieuTri.EditValue != null)
                    //{
                    //    hisDebateSave.TRACKING_ID = (long)cboPhieuDieuTri.EditValue;
                    //}
                    //else
                    //{
                    //    hisDebateSave.TRACKING_ID = null;
                    //}
                    //hisDebateSave.REQUEST_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    //hisDebateSave.REQUEST_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    //hisDebateSave.DEPARTMENT_ID = this.WorkPlaceSDO.DepartmentId;
                    //hisDebate.TREATMENT_ID = this.treatment_id;


                    hisDebateResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DEBATE>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, hisDebateSave, param);

                }
                else if (this.action == GlobalVariables.ActionEdit && currentHisDebate != null)
                {
                    //      ProcessHisDebate(currentHisDebate);
                    hisDebateResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DEBATE>(ApiConsumer.HisRequestUriStore.HIS_DEBATE_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, hisDebateSave, param);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisDebateResult), hisDebateResult));
                if (hisDebateResult != null)
                {
                    LoadDataInviteDebate(hisDebateResult);
                    success = true;
                    btnPrint.Enabled = true;
                    btnSendTMP.Enabled = true;
                    this.currentHisDebate = hisDebateResult;
                    this.action = GlobalVariables.ActionEdit;
                    if (lciAutoCreateEmr.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && chkAutoCreateEmr.Checked)
                    {
                        isCreateEmrDocument = true;
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                        richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__TRICH_BIEN_BAN_HOI_CHAN__MPS000019, DelegateRunPrinter);

                        richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__HOI_CHAN__SO_BIEN_BAN_HOI_CHAN__MPS000020, DelegateRunPrinter);
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);

                if (success && hisDebateSave.HIS_DEBATE_INVITE_USER != null && hisDebateSave.HIS_DEBATE_INVITE_USER.Count > 0)
                {
                    List<string> lstName = new List<string>();
                    foreach (var item in hisDebateSave.HIS_DEBATE_INVITE_USER)
                    {
                        if (item.IS_PARTICIPATION == null)
                            lstName.Add(item.LOGINNAME);
                    }
                    if (lstName == null || lstName.Count == 0)
                        return;

                    //string strName = lstName != null && lstName.Count() > 0 ? string.Join(",", lstName) : "";
                    SDA.EFMODEL.DataModels.SDA_NOTIFY updateDTO = new SDA.EFMODEL.DataModels.SDA_NOTIFY();
                    updateDTO.CONTENT = String.Format("Bạn có lời mời tham gia hội chẩn cho bệnh nhân {0} – {1}, {2}. Mời bạn vào chức năng “Biên bản hội chẩn” để xem chi tiết",
                        vHisTreatment.TREATMENT_CODE,
                        vHisTreatment.TDL_PATIENT_NAME,
                        listDepartment.FirstOrDefault(o => o.ID == Int64.Parse((cboDepartment.EditValue ?? "").ToString())).DEPARTMENT_NAME);
                    updateDTO.TITLE = "Thông báo mời tham gia hội chẩn";
                    updateDTO.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    updateDTO.TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtDebateTime.DateTime).ToString("yyyyMMdd") + "235959");
                    foreach (var item in lstName)
                    {
                        updateDTO.RECEIVER_LOGINNAME = item;
                        Inventec.Common.Logging.LogSystem.Debug("updateDTO___SDA" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));

                        var resultData = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_NOTIFY>("api/SdaNotify/Create", ApiConsumers.SdaConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            //TODO
                        }
                    }

                }
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDebateType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDebateType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDebateType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLocation.Focus();
                    txtLocation.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDebateType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtLocation.Focus();
                    txtLocation.SelectAll();
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
