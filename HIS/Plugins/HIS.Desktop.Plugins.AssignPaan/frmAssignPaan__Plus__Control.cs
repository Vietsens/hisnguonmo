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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPaan.Config;
using HIS.Desktop.Plugins.AssignPaan.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPaan
{
    public partial class frmAssignPaan : HIS.Desktop.Utility.FormBase
    {
        //private void txtIcdSubCode_Leave(object sender, EventArgs e)
        //{
        //try
        //{
        //    string seperate = ";";
        //    string strIcdNames = "";
        //    string strWrongIcdCodes = "";
        //    string[] periodSeparators = new string[1];
        //    periodSeparators[0] = seperate;
        //    string[] arrIcdExtraCodes = txtIcdSubCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
        //    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
        //    {
        //        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
        //        foreach (var itemCode in arrIcdExtraCodes)
        //        {
        //            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE == itemCode);
        //            if (icdByCode != null && icdByCode.ID > 0)
        //            {
        //                strIcdNames += (seperate + icdByCode.ICD_NAME + seperate + txtIcdText.Text);
        //            }
        //            else
        //            {
        //                strWrongIcdCodes += (seperate + itemCode);
        //            }
        //        }
        //        if (!String.IsNullOrEmpty(strWrongIcdCodes))
        //        {
        //            MessageManager.Show(String.Format(ResourceMessageLang.KhongTimThayIcdTuongUngVoiCacMa, strWrongIcdCodes));
        //        }
        //    }
        //    txtIcdText.Text = strIcdNames;
        //}
        //catch (Exception ex)
        //{
        //    Inventec.Common.Logging.LogSystem.Error(ex);
        //}
        //}

        //private void txtIcdSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtIcdText.Focus();
        //            txtIcdText.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.F1)
        //        {
        //            WaitingManager.Show();

        //            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
        //            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
        //            if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");

        //            HIS.Desktop.ADO.SecondaryIcdADO sereservInTreatmentADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtIcdSubCode.Text, txtIcdText.Text);
        //            List<object> listArgs = new List<object>();
        //            listArgs.Add(sereservInTreatmentADO);
        //            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
        //            if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

        //            WaitingManager.Hide();
        //            ((Form)extenceInstance).Show(this);
        //        }
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        WaitingManager.Hide();
        //        MessageBox.Show(Resources.ResourceMessageLang.HeThongKhongTimThayPluginCuaChucNangNay, Resources.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtIcdText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPatientType.EditValue != null)
                    {
                        txtPaanServiceTypeCode.Focus();
                        txtPaanServiceTypeCode.SelectAll();
                    }
                    else
                    {
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                long instructionTime = Convert.ToInt64(dtInstructionTime.DateTime.ToString("yyyyMMddHHmmss"));
                LoadDataPatientTypeByInstructionTime(instructionTime);
                SetDataSourceCboPatientType();
                if (this.currentPatientTypeAlter == null)
                {
                    cboPatientType.EditValue = this.currentPatientTypeAlter;
                    MessageManager.Show(String.Format(Resources.ResourceMessageLang.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, dtInstructionTime.Text));
                    dtInstructionTime.SelectAll();
                }
                else
                {
                    if (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) > dtInstructionTime.DateTime || Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) < dtInstructionTime.DateTime)
                    {

                        cboPatientType.EditValue = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;
                    }
                    else
                    {
                        cboPatientType.EditValue = this.currentPatientTypeAlter != null ? new Nullable<long>(this.currentPatientTypeAlter.PATIENT_TYPE_ID) : null;
                    }
                    txtPaanServiceTypeCode.Focus();
                    txtPaanServiceTypeCode.SelectAll();
                }
                if (dtInstructionTime.EditValue != null ) CheckTimeSereServ();
                
                //if (dtInstructionTime.EditValue != null && dtInstructionTime.DateTime != DateTime.MinValue)
                //{
                //    string instructionTimeTracking = (dtInstructionTime.DateTime.ToString("yyyyMMdd") + "000000");
                //    LoadDataToTrackingCombo(instructionTimeTracking);
                //}
                LoadDataToTrackingCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //thinhdt2
        private void CheckTimeSereServ()
        {

            try
            {
                if (cboUsername.EditValue == null) return;
                var config = BackendDataWorker.Get<HIS_CONFIG>().Where(s => s.KEY == "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION").FirstOrDefault();
                CommonParam param = new CommonParam();
                if (config != null)
                {
                    if (config.VALUE == "1" || config.VALUE == "2")
                    {
                        HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                        sdo.TreatmentId = this.treatmentId;
                        var username = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && Convert.ToInt64(cboUsername.EditValue) == p.ID).FirstOrDefault().LOGINNAME;
                        sdo.Loginnames = new List<string>() { username };
                        long sereTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0;
                        sdo.SereTimes = new List<long> { sereTime };
                        Inventec.Common.Logging.LogSystem.Debug("HisServiceReqCheckSereTimesSDO:" + LogUtil.TraceData("HisServiceReqCheckSereTimesSDO", sdo));
                        bool rs = new BackendAdapter(param).Post<bool>("/api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, param);
                        if (!rs)
                        {
                            if (config.VALUE == "1")
                            {
                                MessageManager.Show(this, param, rs);
                                btnSave.Enabled = btnSavePrint.Enabled = false;
                            }
                            else
                            {
                                btnSave.Enabled = btnSavePrint.Enabled = MessageBox.Show(this, param.GetMessage() + "Bạn có muốn tiếp tục?", "Thông Báo", MessageBoxButtons.YesNo) == DialogResult.Yes;

                            }
                        }
                        else btnSave.Enabled = btnSavePrint.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTracking_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTracking.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.treatmentId);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        //Load lại tracking
                        LoadDataToTrackingCombo();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPaanServiceTypeCode.Focus();
                    txtPaanServiceTypeCode.SelectAll();
                    ReloadCboExecuteRoom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.hisCurrentServicePatys = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                HisPatientType = new HIS_PATIENT_TYPE();
                if (cboPatientType.EditValue != null)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPatientType.EditValue) || (o.INHERIT_PATIENT_TYPE_IDS != null && ("," + o.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + Convert.ToInt64(cboPatientType.EditValue) + ",")));

                    if (patientType != null && dicServicePaty.ContainsKey(patientType.ID))
                    {
                        HisPatientType = patientType;

                        this.hisCurrentServicePatys = dicServicePaty[patientType.ID];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsSurgery_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsSurgery_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!string.IsNullOrEmpty(txtLoginname.Text))
                    {
                        var key = txtLoginname.Text.ToLower();
                        var listData = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.ToLower()== key).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboUsername.EditValue = listData.First().ID;
                            txtPaanServiceTypeCode.Focus();
                            txtPaanServiceTypeCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboUsername.Focus();
                        cboUsername.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPaanServiceTypeCode.Focus();
                    txtPaanServiceTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUsername_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPaanServiceTypeCode.Focus();
                    txtPaanServiceTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUsername_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtLoginname.Text = "";
                if (cboUsername.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboUsername.EditValue));
                    if (user != null)
                    {
                        txtLoginname.Text = user.LOGINNAME;
                    }
                }
                if (cboUsername.EditValue != null ) CheckTimeSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPaanServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPaanServiceTypeCode.Text))
                    {
                        string key = txtPaanServiceTypeCode.Text.ToLower();
                        var listData = this.HisService.Where(o => o.SERVICE_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboPaanServiceType.EditValue = listData.FirstOrDefault().ID;
                            if (cboExecuteRoom.EditValue == null)
                            {
                                cboExecuteRoom.Focus();
                                cboExecuteRoom.ShowPopup();
                            }
                            else
                            {
                                txtPaanPositionCode.Focus();
                                txtPaanPositionCode.SelectAll();
                            }
                        }
                    }

                    if (!valid)
                    {
                        cboPaanServiceType.Focus();
                        cboPaanServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPanServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboExecuteRoom.EditValue == null)
                    {
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                    else
                    {
                        txtPaanPositionCode.Focus();
                        txtPaanPositionCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPanServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPaanServiceTypeCode.Text = "";
                spinPrice.Value = 0;
                cboExecuteRoom.EditValue = null;
                dxValidationProvider1.SetValidationRule(cboTestSampleType, null);
                lciSampleType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                cboTestSampleType.EditValue = null;
                this.hisCurrentServiceRooms = new List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                if (cboPaanServiceType.EditValue != null)
                {
                    var service = this.HisService.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPaanServiceType.EditValue));
                    if (service != null)
                    {
                        if(service.IS_REQUIRED_SAMPLE_TYPE == 1 && cboTestSampleType.Properties.DataSource != null)
                        {
                            lciSampleType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                            ValidControlSampleType();
                            var sampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.TEST_SAMPLE_TYPE_CODE == service.SAMPLE_TYPE_CODE);
                            if (sampleType != null)
                                cboTestSampleType.EditValue = sampleType.ID;
                        }
                        var patientTypeIds = this.hisAllowServicePatys.Where(o => o.SERVICE_ID == service.ID).Select(o => o.PATIENT_TYPE_ID).ToList();
                        patientTypeIds = patientTypeIds.Distinct().ToList();
                        listPatientTypeAllowService = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => patientTypeIds.Contains(o.ID) && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        SetDataSourceCboPatientType();
                        if (listPatientTypeAllowService != null)
                        {
                            var patientType = listPatientTypeAllowService.FirstOrDefault(o => o.ID == this.treatment.TDL_PATIENT_TYPE_ID);
                            if (patientType != null)
                            {
                                cboPatientType.EditValue = patientType.ID;
                            }
                            else
                            {
                                cboPatientType.EditValue = listPatientTypeAllowService.First().ID;
                            }
                        }
                        txtPaanServiceTypeCode.Text = service.SERVICE_CODE;
                        //LogSystem.Debug("danh sach phong xu ly : " + LogUtil.TraceData("dicServiceRoom", dicServiceRoom));
                        LogSystem.Debug("service id: " + service.ID);
                        ReloadCboExecuteRoom();


                        long? intructionNumByType = 1;
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId) ?? new V_HIS_ROOM();
                        //var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(hisCurrentServicePatys, room.BRANCH_ID, null, room.ID, room.DEPARTMENT_ID, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0, this.treatment.IN_TIME, service.ID, Convert.ToInt64(cboPatientType.EditValue), null, intructionNumByType);

                        var currentPaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(hisCurrentServicePatys, room.BRANCH_ID, null, room.ID, room.DEPARTMENT_ID, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0, this.treatment.IN_TIME, service.ID, (HisPatientType != null ? HisPatientType.ID : 0), null, intructionNumByType);


                        if (currentPaty != null)
                        {
                            spinPrice.Value = currentPaty.PRICE;
                        }
                    }
                }
                this.SetDataSourceCboExecuteRoom();
                if (this.hisCurrentServiceRooms != null && this.hisCurrentServiceRooms.Count == 1)
                {
                    cboExecuteRoom.EditValue = this.hisCurrentServiceRooms.FirstOrDefault().ROOM_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadCboExecuteRoom()
        {
            try
            {
                //key moi
                //thinhdt2

                if (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_BY_PATIENT_TYPE") == "1")
                {
                    var listPatientTypeRoom = BackendDataWorker.Get<HIS_PATIENT_TYPE_ROOM>().Where(s => s.PATIENT_TYPE_ID == Convert.ToInt64(cboPatientType.EditValue)).ToList();
                    if (dicServiceRoom.ContainsKey(Convert.ToInt64(cboPaanServiceType.EditValue)))
                    {
                        hisCurrentServiceRooms = dicServiceRoom[Convert.ToInt64(cboPaanServiceType.EditValue)].Where(s => listPatientTypeRoom.Select(o => o.ROOM_ID).Contains(s.ROOM_ID)).ToList();
                        this.SetDataSourceCboExecuteRoom();
                    }
                    else
                    {
                        LogSystem.Debug("Khong thay phong xu ly cho dich vu.");
                    }
                    
                }
                else
                {
                    if (dicServiceRoom.ContainsKey(Convert.ToInt64(cboPaanServiceType.EditValue)))
                    {
                        hisCurrentServiceRooms = dicServiceRoom[Convert.ToInt64(cboPaanServiceType.EditValue)];//.Where(s => listPatientTypeRoom.Select(o => o.ROOM_ID).Contains(s.ROOM_ID)).ToList();
                        this.SetDataSourceCboExecuteRoom();
                    }
                    LogSystem.Debug("cau hinh MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_BY_PATIENT_TYPE khong duoc bat. khong load");
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPaanPositionCode.Focus();
                    txtPaanPositionCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPaanServiceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    this.cboPaanServiceType.Focus();
                    this.cboPaanServiceType.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (cboPaanServiceType.EditValue != null)
                    {
                        txtPaanPositionCode.Focus();
                        txtPaanPositionCode.SelectAll();
                    }
                    e.Handled = true;
                }
                else
                {
                    this.cboPaanServiceType.ShowPopup();
                    Inventec.Common.Controls.PopupLoader.PopupLoader.SelectFirstRowPopup(this.cboPaanServiceType);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExecuteRoom_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtPaanPositionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPaanPositionCode.Text))
                    {
                        string key = txtPaanPositionCode.Text.ToLower();
                        var listData = BackendDataWorker.Get<HIS_PAAN_POSITION>().Where(o => o.PAAN_POSITION_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboPaanPosition.EditValue = listData.FirstOrDefault().ID;
                            txtPaanLiquidCode.Focus();
                            txtPaanLiquidCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboPaanPosition.Focus();
                        cboPaanPosition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPaanPosition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPaanLiquidCode.Focus();
                    txtPaanLiquidCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPaanPosition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPaanPositionCode.Text = "";
                if (cboPaanPosition.EditValue != null)
                {
                    var paanPosition = BackendDataWorker.Get<HIS_PAAN_POSITION>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPaanPosition.EditValue));
                    if (paanPosition != null)
                    {
                        txtPaanPositionCode.Text = paanPosition.PAAN_POSITION_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPaanLiquidCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPaanLiquidCode.Text))
                    {
                        string key = txtPaanLiquidCode.Text.ToLower();
                        var listData = BackendDataWorker.Get<HIS_PAAN_LIQUID>().Where(o => o.PAAN_LIQUID_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboPaanLiquid.EditValue = listData.FirstOrDefault().ID;
                            dtLiquidTime.Focus();
                            dtLiquidTime.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboPaanLiquid.Focus();
                        cboPaanLiquid.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPaanLiquid_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtLiquidTime.Focus();
                    dtLiquidTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPaanLiquid_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPaanLiquidCode.Text = "";
                if (cboPaanLiquid.EditValue != null)
                {
                    var paanLiquid = BackendDataWorker.Get<HIS_PAAN_LIQUID>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPaanLiquid.EditValue));
                    if (paanLiquid != null)
                    {
                        txtPaanLiquidCode.Text = paanLiquid.PAAN_LIQUID_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtLiquidTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboTracking.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtLiquidTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTracking.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTracking_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(delegateIcdNames))
        //        {
        //            txtIcdText.Text = delegateIcdNames;
        //        }
        //        if (!string.IsNullOrEmpty(delegateIcdCodes))
        //        {
        //            txtIcdSubCode.Text = delegateIcdCodes;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
