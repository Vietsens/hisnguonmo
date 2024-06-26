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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImportBlood.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class UCImportBloodPlus
    {
        bool _isHienMau = false;
        private bool isFirstLoad_BloodGiverForm = true;
        Dictionary<string, List<VHisBloodADO>> dicHisBloodGiver_BloodAdo = new Dictionary<string, List<VHisBloodADO>>();
        Dictionary<string, HisBloodGiverADO> dicHisBloodGiver = new Dictionary<string, HisBloodGiverADO>();
        List<HisBloodGiverADO> listImportBloodGiverADO = new List<HisBloodGiverADO>();
        List<ADO.HisBloodGiverADO> listErrorImport;
        HisBloodGiverADO updatingBloodGiverADO = null;
        VHisBloodADO currentBlood_BloodGiver_ForAdd = null;
        ActionType bloodGiverActionType = ActionType.Add;

        enum ActionType
        {
            Add,
            Update
        }

        private void ThreadLoadDataBloodGiverForm()
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(LoadDataBloodGiverForm));
                //thread.Priority = System.Threading.ThreadPriority.Highest;
                thread.IsBackground = true;
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                try
                {
                    WaitingManager.Show();
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    thread.Abort();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodGiverForm()
        {
            try
            {

                BackendDataWorker.Get<HIS_GENDER>();
                BackendDataWorker.Get<HIS_WORK_PLACE>();
                BackendDataWorker.Get<HIS_CAREER>();
                BackendDataWorker.Get<SDA_NATIONAL>();
                BackendDataWorker.Get<SDA_PROVINCE>();
                BackendDataWorker.Get<SDA_COMMUNE>();
                BackendDataWorker.Get<HIS_BLOOD_VOLUME>();
                BackendDataWorker.Get<ACS_USER>();
                BackendDataWorker.Get<HIS_BLOOD_ABO>();
                BackendDataWorker.Get<HIS_BLOOD_RH>();
                BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();

                Invoke(new Action(() =>
                {
                    InitComboGender();
                    InitComboWorkPlaceID();
                    InitComboGiveType();
                    InitComboCareer();
                    InitComboNational();
                    InitComboProvince();
                    InitComboDistrict();
                    InitComboCommune();
                    InitComboBloodVolume();
                    InitComboUser();
                    InitComboBloodABO();
                    InitComboBloodRh();
                    InitComboTestResult();
                    InitComboVirAddress();

                    ValidControl_BloodGiverForm();

                    SetDefaultDataBloodGiverForm();
                    this.bloodGiverActionType = ActionType.Add;
                    EnableControlsByActionType_BloodGiverForm();
                    //
                    this.cboBloodVolumeID_BloodGiver.Size = new System.Drawing.Size(cboBloodVolumeID_BloodGiver.Size.Width + (lciXuTri.Location.X - lciBSKhamHM.Location.X), cboBloodVolumeID_BloodGiver.Size.Height);
                    lciBSKhamHM.Location = new Point(lciXuTri.Location.X, lciBSKhamHM.Location.Y);
                    lciTinhTrangLS.Location = new Point(lciLanHM.Location.X, lciTinhTrangLS.Location.Y);
                    WaitingManager.Hide();
                }));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSourceGridBlood_BloodGiver()
        {
            try
            {
                gridColumn_ForGroupingBloodDonation.GroupIndex = 0;
                gridColumn_ImpMestBlood_Stt.Visible = false;
                gridColumn_ImpMestBlood_Edit.Visible = false;

                gridControlImpMestBlood.BeginUpdate();
                if (this.dicHisBloodGiver_BloodAdo != null && this.dicHisBloodGiver_BloodAdo.Count > 0)
                {
                    List<VHisBloodADO> listData = new List<VHisBloodADO>();
                    foreach (var item in this.dicHisBloodGiver_BloodAdo)
                    {
                        if (item.Value != null)
                            listData.AddRange(item.Value);
                    }
                    listData.Reverse();
                    gridControlImpMestBlood.DataSource = listData;
                }
                else
                {
                    gridControlImpMestBlood.DataSource = null;
                }
                gridControlImpMestBlood.EndUpdate();
                gridControlImpMestBlood.RefreshDataSource();

                gridViewImpMestBlood.CollapseAllGroups();
                gridViewImpMestBlood.ExpandAllGroups();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFormData_BloodGiverForm()
        {
            try
            {
                RemoveControlErrorDXValidation3();

                if (!layoutControlBloodGiverFORM.IsInitialized) return;
                layoutControlBloodGiverFORM.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlBloodGiverFORM.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            if (lci.Control is DevExpress.XtraEditors.CheckEdit)
                            {
                                fomatFrm.EditValue = null;
                            }
                            else
                            {
                                fomatFrm.ResetText();
                                fomatFrm.EditValue = null;
                            }
                        }
                    }
                    txtDOB_BloodGiver.Text = "";
                    dtDOB_BloodGiver.EditValue = null;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControlBloodGiverFORM.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultDataBloodGiverForm()
        {
            try
            {
                ResetFormData_BloodGiverForm();
                this.bloodGiverActionType = ActionType.Add;
                EnableControlsByActionType_BloodGiverForm();

                chkInWorkPlace.Checked = false;
                chkInPermanentAddress.Checked = false;
                txtGiveCode_BloodGiver.Text = "";
                txtGiveName_BloodGiver.Text = "";
                dtDOB_BloodGiver.EditValue = null;
                cboGender_BloodGiver.EditValue = null;
                cboBloodABO_BloodGiver.EditValue = null;
                cboBloodRh_BloodGiver.EditValue = null;

                txtWorkPlace_BloodGiver.Text = "Tự do";
                cboNational_BloodGiver.EditValue = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => (o.NATIONAL_CODE ?? "").ToUpper().Trim() == "VN").Select(o => o.ID).FirstOrDefault();
                dtExamTime_BloodGiver.DateTime = DateTime.Now;
                dtExecuteTime_BloodGiver.DateTime = dtExamTime_BloodGiver.DateTime.AddMinutes(5);
                txtTestBeforeHB_BloodGiver.Text = "Đạt";
                cboTestBeforeHBV_BloodGiver.EditValue = 0; // Âm tính

                cboTestAfterHBV.EditValue = 0; // Không phản ứng
                cboTestAfterHCV.EditValue = 0; //Không phản ứng
                cboTestAfterHIV.EditValue = 0; //Không phản ứng
                cboTestAfterGM.EditValue = 0; //Không phản ứng
                cboTestAfterKTBT.EditValue = 0; //Không phản ứng
                cboTestAfterNAT_HBV.EditValue = 0; //Không phản ứng
                cboTestAfterNAT_HCV.EditValue = 0; //Không phản ứng
                cboTestAfterNAT_HIV.EditValue = 0; //Không phản ứng

                // gán giá trị mặc định
                spWeight_BloodGiver.EditValue = 60;
                spBloodPressureMax_BloodGiver.EditValue = 120;
                spBloodPressureMin_BloodGiver.EditValue = 80;
                spPulse_BloodGiver.EditValue = 80;
                txtNoteSubclinical_BloodGiver.Text = "Bình thường";
                spAmount_BloodGiver.EditValue = 1;
                txtExecute_BloodGiver.Text = "Không";
                txtGiveCode_BloodGiver.Focus();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessBtnNew_BloodGiver()
        {
            try
            {
                RemoveControlErrorDXValidation3();
                txtGiveCode_BloodGiver.Text = "";
                txtGiveName_BloodGiver.Text = "";
                dtDOB_BloodGiver.EditValue = null;
                cboGender_BloodGiver.EditValue = null;
                txtGiveCard.Text = "";
                txtPhone.Text = "";
                txtCMNDNumber.Text = "";
                dtCMNDDate.EditValue = null;
                txtCMNDPlace.Text = "";
                cboBloodABO_BloodGiver.EditValue = null;
                cboBloodRh_BloodGiver.EditValue = null;
                cboTestAfterHBV.EditValue = 0; // Không phản ứng
                cboTestAfterHCV.EditValue = 0; //Không phản ứng
                cboTestAfterHIV.EditValue = 0; //Không phản ứng
                cboTestAfterGM.EditValue = 0; //Không phản ứng
                cboTestAfterKTBT.EditValue = 0; //Không phản ứng
                cboTestAfterNAT_HBV.EditValue = 0; //Không phản ứng
                cboTestAfterNAT_HCV.EditValue = 0; //Không phản ứng
                cboTestAfterNAT_HIV.EditValue = 0; //Không phản ứng
                if (Convert.ToInt16(cboImpMestType.EditValue) == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__HM)
                {
                    dtExamTime_BloodGiver.DateTime = dtExamTime_BloodGiver.EditValue != null ? dtExamTime_BloodGiver.DateTime.AddMinutes(1) : DateTime.Now;
                    dtExecuteTime_BloodGiver.DateTime = dtExamTime_BloodGiver.DateTime.AddMinutes(15);
                }
                else
                {
                    dtExamTime_BloodGiver.DateTime = DateTime.Now;
                    dtExecuteTime_BloodGiver.DateTime = dtExamTime_BloodGiver.DateTime.AddMinutes(5);

                    // thêm giá trị mặc định
                    spWeight_BloodGiver.EditValue = 60;
                    spBloodPressureMax_BloodGiver.EditValue = 120;
                    spBloodPressureMin_BloodGiver.EditValue = 80;
                    spPulse_BloodGiver.EditValue = 80;
                    txtNoteSubclinical_BloodGiver.Text = "Bình thường";
                    spAmount_BloodGiver.EditValue = 1;
                    txtExecute_BloodGiver.Text = "Không";
                }
                txtGiveCode_BloodGiver.Focus();
                this.bloodGiverActionType = ActionType.Add;
                EnableControlsByActionType_BloodGiverForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdate_BloodGiver()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessUpdate_BloodGiver()_Start");
                if (this.bloodGiverActionType != ActionType.Update)
                    return;
                string messageError = "";
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }

                if (this.updatingBloodGiverADO == null || this.updatingBloodGiverADO.GIVE_CODE == null || !this.dicHisBloodGiver.ContainsKey(this.updatingBloodGiverADO.GIVE_CODE))
                    return;
                if (!dxValidationProvider3.Validate())
                    return;

                if (this.updatingBloodGiverADO.GIVE_CODE != txtGiveCode_BloodGiver.Text.Trim() && this.dicHisBloodGiver.ContainsKey(txtGiveCode_BloodGiver.Text.Trim()))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Mã hồ sơ {0} đã có trong danh sách nhập. Bạn có muốn thay đổi?", txtGiveCode_BloodGiver.Text.Trim()), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }

                WaitingManager.Show();
                HisBloodGiverADO bloodGiverADO = new HisBloodGiverADO();
                bloodGiverADO = GetBloodGiverFormData();
                if (bloodGiverADO == null)
                {
                    throw new Exception("GetBloodGiverFormData() return null!");
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("updatingBloodGiverADO", updatingBloodGiverADO));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("bloodGiverADO", bloodGiverADO));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dicHisBloodGiver", dicHisBloodGiver));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dicHisBloodGiver_BloodAdo", dicHisBloodGiver_BloodAdo));
                this.dicHisBloodGiver.Remove(this.updatingBloodGiverADO.GIVE_CODE);

                if (!this.dicHisBloodGiver.ContainsKey(bloodGiverADO.GIVE_CODE))
                {
                    this.dicHisBloodGiver.Add(bloodGiverADO.GIVE_CODE, bloodGiverADO);
                }
                else
                {
                    this.dicHisBloodGiver[bloodGiverADO.GIVE_CODE] = bloodGiverADO;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dicHisBloodGiver(after)", dicHisBloodGiver));

                if (this.dicHisBloodGiver_BloodAdo != null && this.dicHisBloodGiver_BloodAdo.ContainsKey(this.updatingBloodGiverADO.GIVE_CODE))
                {
                    List<VHisBloodADO> listBloodADO_New = new List<VHisBloodADO>();
                    var listBloodADO_Old = this.dicHisBloodGiver_BloodAdo[this.updatingBloodGiverADO.GIVE_CODE];
                    this.dicHisBloodGiver_BloodAdo.Remove(this.updatingBloodGiverADO.GIVE_CODE);
                    foreach (var item in listBloodADO_Old)
                    {
                        VHisBloodADO newItem = new VHisBloodADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<VHisBloodADO>(newItem, item);
                        newItem.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                        newItem.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1} - {2} - {3}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME, bloodGiverADO.DOB_ForDisplay, bloodGiverADO.GENDER_ForDisplay);
                        newItem.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                        newItem.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                        newItem.DOB_ForDisplay = bloodGiverADO.DOB_ForDisplay;
                        newItem.GENDER_ForDisplay = bloodGiverADO.GENDER_ForDisplay;
                        newItem.BLOOD_CODE = (item.BLOOD_TYPE_CODE ?? "") + (bloodGiverADO.GIVE_CODE ?? "");
                        listBloodADO_New.Add(newItem);
                    }
                    if (!this.dicHisBloodGiver_BloodAdo.ContainsKey(bloodGiverADO.GIVE_CODE))
                    {
                        this.dicHisBloodGiver_BloodAdo.Add(bloodGiverADO.GIVE_CODE, listBloodADO_New);
                    }
                    else
                    {
                        this.dicHisBloodGiver_BloodAdo[bloodGiverADO.GIVE_CODE] = listBloodADO_New;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dicHisBloodGiver_BloodAdo(after)", dicHisBloodGiver_BloodAdo));
                SetDataSourceGridBlood_BloodGiver();
                WaitingManager.Hide();
                ProcessBtnNew_BloodGiver();
                this.bloodGiverActionType = ActionType.Add;
                EnableControlsByActionType_BloodGiverForm();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAdd_BloodGiver()
        {
            try
            {
                string messageError = "";
                if (!CheckAllowAdd(ref messageError))
                {
                    MessageManager.Show(messageError);
                    return;
                }

                if (!dxValidationProvider3.Validate())
                    return;
                if (this.dicHisBloodGiver.ContainsKey(txtGiveCode_BloodGiver.Text.Trim()))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Mã hồ sơ {0} đã có trong danh sách nhập. Bạn có muốn thay đổi?", txtGiveCode_BloodGiver.Text.Trim()), Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }
                WaitingManager.Show();
                HisBloodGiverADO bloodGiverADO = new HisBloodGiverADO();
                bloodGiverADO = GetBloodGiverFormData();
                if (bloodGiverADO == null)
                {
                    throw new Exception("GetBloodGiverFormData() return null!");
                }

                if (!this.dicHisBloodGiver.ContainsKey(bloodGiverADO.GIVE_CODE))
                {
                    this.dicHisBloodGiver.Add(bloodGiverADO.GIVE_CODE, bloodGiverADO);
                }
                else
                {
                    this.dicHisBloodGiver[bloodGiverADO.GIVE_CODE] = bloodGiverADO;
                }

                //Adding Bloods
                List<long> listbloodVolumeID = new List<long>();
                if (bloodGiverADO.BLOOD_VOLUME_ID != null)
                    listbloodVolumeID.Add(bloodGiverADO.BLOOD_VOLUME_ID ?? 0);
                List<HIS_BLTY_VOLUME> list_HIS_BLTY_VOLUME = GetListHisBltyVolume_From_BLOOD_VOLUME_ID(listbloodVolumeID);
                List<long> listBloodTypeID = list_HIS_BLTY_VOLUME != null ? list_HIS_BLTY_VOLUME.Select(o => o.BLOOD_TYPE_ID).ToList() : new List<long>();

                List<V_HIS_BLOOD_TYPE> listBloodType = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().Where(o => listBloodTypeID.Contains(o.ID)).ToList();

                List<VHisBloodADO> listBloodADO = new List<VHisBloodADO>();
                if (listBloodType != null && listBloodType.Count > 0)
                {
                    foreach (var item in listBloodType)
                    {
                        VHisBloodADO bloodADO = new VHisBloodADO(item);
                        bloodADO.IsBloodDonation = true;
                        bloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                        bloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1} - {2} - {3}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME, bloodGiverADO.DOB_ForDisplay, bloodGiverADO.GENDER_ForDisplay);
                        bloodADO.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                        bloodADO.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                        bloodADO.DOB_ForDisplay = bloodGiverADO.DOB_ForDisplay;
                        bloodADO.GENDER_ForDisplay = bloodGiverADO.GENDER_ForDisplay;

                        bloodADO.BLOOD_ABO_ID = bloodGiverADO.BLOOD_ABO_ID ?? 0;
                        var blood_Abo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == (bloodGiverADO.BLOOD_ABO_ID ?? 0));
                        if (blood_Abo != null)
                            bloodADO.BLOOD_ABO_CODE = blood_Abo.BLOOD_ABO_CODE;

                        bloodADO.BLOOD_RH_ID = bloodGiverADO.BLOOD_RH_ID;
                        var blood_Rh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == (bloodGiverADO.BLOOD_RH_ID ?? 0));
                        if (blood_Rh != null)
                            bloodADO.BLOOD_RH_CODE = blood_Rh.BLOOD_RH_CODE;

                        DateTime? executeTime = null;
                        if (bloodGiverADO.EXECUTE_TIME != null)
                            executeTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiverADO.EXECUTE_TIME ?? 0);
                        TimeSpan alertExpiredDate = new TimeSpan();
                        if (item.ALERT_EXPIRED_DATE != null)
                            alertExpiredDate = new TimeSpan((int)(item.ALERT_EXPIRED_DATE ?? 0), 0, 0, 0);
                        DateTime? expiredDate = null;
                        if (executeTime != null)
                            expiredDate = executeTime + alertExpiredDate;

                        if (expiredDate != null)
                            bloodADO.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(expiredDate);

                        bloodADO.BLOOD_CODE = (item.BLOOD_TYPE_CODE ?? "") + (bloodGiverADO.GIVE_CODE ?? "");

                        listBloodADO.Add(bloodADO);
                    }
                }
                else
                {
                    VHisBloodADO emptyBloodADO = new VHisBloodADO();
                    emptyBloodADO.IsEmptyRow = true;
                    emptyBloodADO.IsBloodDonation = true;
                    emptyBloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                    emptyBloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME);
                    emptyBloodADO.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                    emptyBloodADO.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                    listBloodADO.Add(emptyBloodADO);
                }
                if (!this.dicHisBloodGiver_BloodAdo.ContainsKey(bloodGiverADO.GIVE_CODE))
                {
                    this.dicHisBloodGiver_BloodAdo.Add(bloodGiverADO.GIVE_CODE, listBloodADO);
                }
                else
                {
                    this.dicHisBloodGiver_BloodAdo[bloodGiverADO.GIVE_CODE] = listBloodADO;
                }
                SetDataSourceGridBlood_BloodGiver();
                WaitingManager.Hide();
                ProcessBtnNew_BloodGiver();
                this.bloodGiverActionType = ActionType.Add;
                EnableControlsByActionType_BloodGiverForm();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_BLTY_VOLUME> GetListHisBltyVolume_From_BLOOD_VOLUME_ID(List<long> listId)
        {
            List<HIS_BLTY_VOLUME> result = null;
            try
            {
                if (listId == null || listId.Count == 0)
                    return null;
                CommonParam param = new CommonParam();
                HisBltyVolumeFilter filter = new HisBltyVolumeFilter();
                filter.BLOOD_VOLUME_IDs = listId;
                result = new BackendAdapter(param).Get<List<HIS_BLTY_VOLUME>>(HisRequestUri.HIS_HIS_BLTY_VOLUME_GET, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HisBloodGiverADO GetBloodGiverFormData()
        {
            HisBloodGiverADO result = new HisBloodGiverADO();
            try
            {
                if (this.updatingBloodGiverADO != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisBloodGiverADO>(result, this.updatingBloodGiverADO);
                }
                //I.Thông tin người hiến máu
                result.GIVE_CODE = txtGiveCode_BloodGiver.Text.Trim();
                result.GIVE_NAME = txtGiveName_BloodGiver.Text;
                if (dtDOB_BloodGiver.DateTime != null && dtDOB_BloodGiver.DateTime != DateTime.MinValue)
                {
                    result.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)(dtDOB_BloodGiver.DateTime.Date)) ?? 0;
                    result.DOB_ForDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString((DateTime?)(dtDOB_BloodGiver.DateTime.Date));
                }

                result.GENDER_ID = cboGender_BloodGiver.EditValue != null ? (long?)cboGender_BloodGiver.EditValue : null;

                result.GENDER_ForDisplay = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.ID == Convert.ToInt64(cboGender_BloodGiver.EditValue ?? 0)).Select(o => o.GENDER_NAME).FirstOrDefault();
                result.WORK_PLACE_ID = cboWorkPlaceID_BloodGiver.EditValue != null ? (long?)(Convert.ToInt64(cboWorkPlaceID_BloodGiver.EditValue)) : null;
                result.GIVE_TYPE = Convert.ToInt16(cboGiveType_BloodGiver.EditValue ?? 0);
                result.CAREER_ID = cboCareer_BloodGiver.EditValue != null ? (long?)(Convert.ToInt64(cboCareer_BloodGiver.EditValue)) : null;
                result.CAREER_NAME = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.ID == Convert.ToInt64(cboCareer_BloodGiver.EditValue ?? 0)).Select(o => o.CAREER_NAME).FirstOrDefault();
                if (chkInWorkPlace.Checked)
                {
                    result.WORK_PLACE = BackendDataWorker.Get<HIS_WORK_PLACE>().Where(o => o.ID == Convert.ToInt64(cboWorkPlaceID_BloodGiver.EditValue ?? 0)).Select(o => o.WORK_PLACE_NAME).FirstOrDefault();
                }
                else
                {
                    result.WORK_PLACE = txtWorkPlace_BloodGiver.Text;
                }
                var national = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.ID == Convert.ToInt64(cboNational_BloodGiver.EditValue ?? 0)).FirstOrDefault();
                result.NATIONAL_CODE = national != null ? national.NATIONAL_CODE : null;
                result.NATIONAL_NAME = national != null ? national.NATIONAL_NAME : null;
                if (chkInPermanentAddress.Checked)
                {
                    var provinceBlood = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.ID == Convert.ToInt64(cboProvince.EditValue ?? 0)).FirstOrDefault();
                    result.PROVINCE_CODE_BLOOD = provinceBlood != null ? provinceBlood.PROVINCE_CODE : null;
                    result.PROVINCE_NAME_BLOOD = provinceBlood != null ? provinceBlood.PROVINCE_NAME : null;
                    var districtBlood = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.ID == Convert.ToInt64(cboDistrict.EditValue ?? 0)).FirstOrDefault();
                    result.DISTRICT_CODE_BLOOD = districtBlood != null ? districtBlood.DISTRICT_CODE : null;
                    result.DISTRICT_NAME_BLOOD = districtBlood != null ? districtBlood.DISTRICT_NAME : null;
                }
                else
                {
                    var provinceBlood = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.ID == Convert.ToInt64(cboProvinceBlood_BloodGiver.EditValue ?? 0)).FirstOrDefault();
                    result.PROVINCE_CODE_BLOOD = provinceBlood != null ? provinceBlood.PROVINCE_CODE : null;
                    result.PROVINCE_NAME_BLOOD = provinceBlood != null ? provinceBlood.PROVINCE_NAME : null;
                    var districtBlood = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.ID == Convert.ToInt64(cboDistrictBlood_BloodGiver.EditValue ?? 0)).FirstOrDefault();
                    result.DISTRICT_CODE_BLOOD = districtBlood != null ? districtBlood.DISTRICT_CODE : null;
                    result.DISTRICT_NAME_BLOOD = districtBlood != null ? districtBlood.DISTRICT_NAME : null;
                }
                if (cboVirAddress.EditValue != null)
                {
                    var communeADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboVirAddress.EditValue ?? "").ToString());

                    if (communeADO != null)
                    {
                        result.VirAddress_ID_RAW = communeADO.ID_RAW;
                        result.VIR_ADDRESS = communeADO.RENDERER_PDC_NAME;
                    }

                }

                var province = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.ID == Convert.ToInt64(cboProvince.EditValue ?? 0)).FirstOrDefault();
                result.PROVINCE_CODE = province != null ? province.PROVINCE_CODE : null;
                result.PROVINCE_NAME = province != null ? province.PROVINCE_NAME : null;
                var district = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.ID == Convert.ToInt64(cboDistrict.EditValue ?? 0)).FirstOrDefault();
                result.DISTRICT_CODE = district != null ? district.DISTRICT_CODE : null;
                result.DISTRICT_NAME = district != null ? district.DISTRICT_NAME : null;
                var commune = BackendDataWorker.Get<SDA_COMMUNE>().Where(o => o.ID == Convert.ToInt64(cboCommune.EditValue ?? 0)).FirstOrDefault();
                result.COMMUNE_CODE = commune != null ? commune.COMMUNE_CODE : null;
                result.COMMUNE_NAME = commune != null ? commune.COMMUNE_NAME : null;

                result.ADDRESS = txtAddress.Text;
                result.GIVE_CARD = txtGiveCard.Text;
                result.PHONE = txtPhone.Text;
                var CMNDNumber = txtCMNDNumber.Text;
                if (!String.IsNullOrWhiteSpace(CMNDNumber))
                {
                    if (CMNDNumber.Trim().Length == 12 && CMNDNumber.Trim().All(char.IsDigit))
                    {
                        result.CCCD_NUMBER = CMNDNumber.Trim();
                        if (dtCMNDDate.DateTime != null && dtCMNDDate.DateTime != DateTime.MinValue)
                        {
                            result.CCCD_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)(dtCMNDDate.DateTime.Date));
                        }
                        result.CCCD_PLACE = txtCMNDPlace.Text;
                    }
                    else if (CMNDNumber.Trim().Length == 9 && CMNDNumber.Trim().All(char.IsDigit))
                    {
                        result.CMND_NUMBER = CMNDNumber.Trim();
                        if (dtCMNDDate.DateTime != null && dtCMNDDate.DateTime != DateTime.MinValue)
                        {
                            result.CMND_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)(dtCMNDDate.DateTime.Date));
                        }
                        result.CMND_PLACE = txtCMNDPlace.Text;
                    }
                    else if (CMNDNumber.Trim().Length == 9 && CMNDNumber.Trim().All(char.IsLetterOrDigit))
                    {
                        result.PASSPORT_NUMBER = CMNDNumber.Trim();
                        if (dtCMNDDate.DateTime != null && dtCMNDDate.DateTime != DateTime.MinValue)
                        {
                            result.PASSPORT_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)(dtCMNDDate.DateTime.Date));
                        }
                        result.PASSPORT_PLACE = txtCMNDPlace.Text;
                    }
                }
                //II.Khám lâm sàng
                if (dtExamTime_BloodGiver.DateTime != null && dtExamTime_BloodGiver.DateTime != DateTime.MinValue)
                {
                    result.EXAM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)(dtExamTime_BloodGiver.DateTime));
                }
                result.TURN = spTurn_BloodGiver.EditValue != null ? (long?)Convert.ToInt64(spTurn_BloodGiver.Value) : null;
                result.WEIGHT = spWeight_BloodGiver.EditValue != null ? (decimal?)spWeight_BloodGiver.Value : null;
                result.PULSE = spPulse_BloodGiver.EditValue != null ? (long?)Convert.ToInt64(spPulse_BloodGiver.Value) : null;
                result.BLOOD_PRESSURE_MAX = spBloodPressureMax_BloodGiver.EditValue != null ? (long?)Convert.ToInt64(spBloodPressureMax_BloodGiver.Value) : null;
                result.BLOOD_PRESSURE_MIN = spBloodPressureMin_BloodGiver.EditValue != null ? (long?)Convert.ToInt64(spBloodPressureMin_BloodGiver.Value) : null;
                result.NOTE_SUBCLINICAL = txtNoteSubclinical_BloodGiver.Text;
                //III.Xét nghiệm trước lấy máu
                result.TEST_BEFORE_HB = txtTestBeforeHB_BloodGiver.Text;
                result.TEST_BEFORE_HBV = cboTestBeforeHBV_BloodGiver.EditValue != null ? (short?)(Convert.ToInt16(cboTestBeforeHBV_BloodGiver.EditValue)) : null;
                //IV.Kết luận lấy máu
                result.BLOOD_VOLUME_ID = cboBloodVolumeID_BloodGiver.EditValue != null ? (long?)(Convert.ToInt64(cboBloodVolumeID_BloodGiver.EditValue)) : null;
                var examUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == Convert.ToInt64(cboExamLoginName_UserName.EditValue ?? 0)).FirstOrDefault();
                result.EXAM_LOGINNAME = examUser != null ? examUser.LOGINNAME : null;
                result.EXAM_USERNAME = examUser != null ? examUser.USERNAME : null;
                //V.Lấy máu
                if (dtExecuteTime_BloodGiver.DateTime != null && dtExecuteTime_BloodGiver.DateTime != DateTime.MinValue)
                {
                    result.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber((DateTime?)(dtExecuteTime_BloodGiver.DateTime));
                }
                result.AMOUNT = spAmount_BloodGiver.EditValue != null ? (decimal?)spAmount_BloodGiver.Value : null;
                var executeUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.ID == Convert.ToInt64(cboExecuteLoginName_UserName.EditValue ?? 0)).FirstOrDefault();
                result.EXECUTE_LOGINNAME = executeUser != null ? executeUser.LOGINNAME : null;
                result.EXECUTE_USERNAME = executeUser != null ? executeUser.USERNAME : null;
                result.EXECUTE = txtExecute_BloodGiver.Text;
                //VI.Xét nghiệm sau lấy máu
                result.BLOOD_ABO_ID = cboBloodABO_BloodGiver.EditValue != null ? (long?)(Convert.ToInt64(cboBloodABO_BloodGiver.EditValue)) : null;
                result.BLOOD_RH_ID = cboBloodRh_BloodGiver.EditValue != null ? (long?)(Convert.ToInt64(cboBloodRh_BloodGiver.EditValue)) : null;
                result.TEST_AFTER_HBV = cboTestAfterHBV.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterHBV.EditValue)) : null;
                result.TEST_AFTER_HCV = cboTestAfterHCV.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterHCV.EditValue)) : null;
                result.TEST_AFTER_HIV = cboTestAfterHIV.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterHIV.EditValue)) : null;
                result.TEST_AFTER_GM = cboTestAfterGM.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterGM.EditValue)) : null;
                result.TEST_AFTER_KTBT = cboTestAfterKTBT.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterKTBT.EditValue)) : null;
                result.TEST_AFTER_NAT_HBV = cboTestAfterNAT_HBV.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterNAT_HBV.EditValue)) : null;
                result.TEST_AFTER_NAT_HCV = cboTestAfterNAT_HCV.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterNAT_HCV.EditValue)) : null;
                result.TEST_AFTER_NAT_HIV = cboTestAfterNAT_HIV.EditValue != null ? (short?)(Convert.ToInt16(cboTestAfterNAT_HIV.EditValue)) : null;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetDicBloodGiverData_FromCurrentImpMest()
        {
            try
            {
                var listBloodGiver = GetListBloodGiverByIMP_MEST_ID(this.impMestId);
                var listBlood = GetListBloodByIMP_MEST_ID(this.impMestId);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listBloodGiver", listBloodGiver));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listBlood", listBlood));
                this.dicHisBloodGiver = new Dictionary<string, HisBloodGiverADO>();
                this.dicHisBloodGiver_BloodAdo = new Dictionary<string, List<VHisBloodADO>>();

                if (listBloodGiver != null)
                {
                    foreach (var BloodGiver in listBloodGiver)
                    {
                        HisBloodGiverADO bloodGiverADO = new HisBloodGiverADO(BloodGiver);

                        if (!this.dicHisBloodGiver.ContainsKey(bloodGiverADO.GIVE_CODE))
                        {
                            this.dicHisBloodGiver.Add(bloodGiverADO.GIVE_CODE, bloodGiverADO);
                        }
                        else
                        {
                            this.dicHisBloodGiver[bloodGiverADO.GIVE_CODE] = bloodGiverADO;
                        }

                        List<VHisBloodADO> listBloodADOempty = new List<VHisBloodADO>();
                        if (listBlood == null || listBlood.Count == 0)
                        {
                            VHisBloodADO emptyBloodADO = new VHisBloodADO();
                            emptyBloodADO.IsEmptyRow = true;
                            emptyBloodADO.IsBloodDonation = true;
                            emptyBloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                            emptyBloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME);
                            emptyBloodADO.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                            emptyBloodADO.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                            listBloodADOempty.Add(emptyBloodADO);
                        }
                        else
                        {
                            var listblood1 = listBlood.Where(o => o.GIVE_CODE != null && o.GIVE_CODE == bloodGiverADO.GIVE_CODE).ToList();
                            if (listblood1 == null || listblood1.Count == 0)
                            {
                                VHisBloodADO emptyBloodADO = new VHisBloodADO();
                                emptyBloodADO.IsEmptyRow = true;
                                emptyBloodADO.IsBloodDonation = true;
                                emptyBloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                                emptyBloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME);
                                emptyBloodADO.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                                emptyBloodADO.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                                listBloodADOempty.Add(emptyBloodADO);
                            }
                        }
                        if (!this.dicHisBloodGiver_BloodAdo.ContainsKey(bloodGiverADO.GIVE_CODE))
                        {
                            this.dicHisBloodGiver_BloodAdo.Add(bloodGiverADO.GIVE_CODE, listBloodADOempty);
                        }
                        else
                        {
                            this.dicHisBloodGiver_BloodAdo[bloodGiverADO.GIVE_CODE] = listBloodADOempty;
                        }
                    }
                }

                List<VHisBloodADO> listBloodADO = new List<VHisBloodADO>();
                if (listBlood != null)
                {
                    foreach (var Blood in listBlood)
                    {
                        VHisBloodADO bloodADO = new VHisBloodADO(Blood);
                        HisBloodGiverADO bloodGiverADO = this.dicHisBloodGiver[bloodADO.GIVE_CODE];
                        bloodADO.IsBloodDonation = true;
                        bloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                        bloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1} - {2} - {3}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME, bloodGiverADO.DOB_ForDisplay, bloodGiverADO.GENDER_ForDisplay);
                        bloodADO.GENDER_ForDisplay = bloodGiverADO.GENDER_ForDisplay;
                        bloodADO.BLOOD_ABO_ID = bloodGiverADO.BLOOD_ABO_ID ?? 0;
                        var blood_Abo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == (bloodGiverADO.BLOOD_ABO_ID ?? 0));
                        if (blood_Abo != null)
                            bloodADO.BLOOD_ABO_CODE = blood_Abo.BLOOD_ABO_CODE;

                        bloodADO.BLOOD_RH_ID = bloodGiverADO.BLOOD_RH_ID;
                        var blood_Rh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == (bloodGiverADO.BLOOD_RH_ID ?? 0));
                        if (blood_Rh != null)
                            bloodADO.BLOOD_RH_CODE = blood_Rh.BLOOD_RH_CODE;
                        listBloodADO.Add(bloodADO);
                    }
                }

                var group = listBloodADO.GroupBy(o => o.GIVE_CODE).ToList();
                if (group != null)
                {
                    foreach (var item in group)
                    {
                        if (!this.dicHisBloodGiver_BloodAdo.ContainsKey(item.Key))
                        {
                            this.dicHisBloodGiver_BloodAdo.Add(item.Key, item.ToList());
                        }
                        else
                        {
                            this.dicHisBloodGiver_BloodAdo[item.Key] = item.ToList();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_BLOOD_GIVER> GetListBloodGiverByIMP_MEST_ID(long id)
        {
            List<HIS_BLOOD_GIVER> result = new List<HIS_BLOOD_GIVER>();
            try
            {
                CommonParam param = new CommonParam();
                if (id > 0)
                {
                    MOS.Filter.HisBloodGiverFilter filter = new HisBloodGiverFilter();
                    filter.IMP_MEST_ID = id;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BLOOD_GIVER>>("api/HisBloodGiver/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                }
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_BLOOD> GetListBloodByIMP_MEST_ID(long id)
        {
            List<V_HIS_BLOOD> result = new List<V_HIS_BLOOD>();
            try
            {
                CommonParam param = new CommonParam();
                if (id > 0)
                {
                    MOS.Filter.HisImpMestBloodFilter filter = new HisImpMestBloodFilter();
                    filter.IMP_MEST_ID = id;
                    var impMestBloods = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                    if (impMestBloods == null || impMestBloods.Count() == 0)
                        return null;

                    List<long> bloodIds = impMestBloods.Select(o => o.BLOOD_ID).ToList();

                    MOS.Filter.HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                    bloodFilter.IDs = bloodIds;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BLOOD>>("api/HisBlood/GetView", ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, param);


                }
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void RemoveControlErrorDXValidation3()
        {
            try
            {
                IList<Control> controls = dxValidationProvider3.GetInvalidControls();
                while (controls.Count > 0)
                    dxValidationProvider3.RemoveControlError(controls[controls.Count - 1]);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToBloodGiverForm(HisBloodGiverADO hisBloodGiverADO)
        {
            try
            {
                ResetFormData_BloodGiverForm();
                this.bloodGiverActionType = ActionType.Update;
                EnableControlsByActionType_BloodGiverForm();

                //I.Thông tin người hiến máu
                txtGiveCode_BloodGiver.Text = hisBloodGiverADO.GIVE_CODE;
                txtGiveName_BloodGiver.Text = hisBloodGiverADO.GIVE_NAME;
                dtDOB_BloodGiver.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBloodGiverADO.DOB) ?? new DateTime();
                if (dtDOB_BloodGiver.DateTime == DateTime.MinValue)
                    dtDOB_BloodGiver.EditValue = null;
                cboGender_BloodGiver.EditValue = hisBloodGiverADO.GENDER_ID;
                cboWorkPlaceID_BloodGiver.EditValue = hisBloodGiverADO.WORK_PLACE_ID;
                cboGiveType_BloodGiver.EditValue = hisBloodGiverADO.GIVE_TYPE;
                cboCareer_BloodGiver.EditValue = hisBloodGiverADO.CAREER_ID;
                txtWorkPlace_BloodGiver.Text = hisBloodGiverADO.WORK_PLACE;
                cboNational_BloodGiver.EditValue = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.NATIONAL_CODE == hisBloodGiverADO.NATIONAL_CODE).Select(o => o.ID).FirstOrDefault();

                cboProvinceBlood_BloodGiver.EditValue = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.PROVINCE_CODE == hisBloodGiverADO.PROVINCE_CODE_BLOOD).Select(o => o.ID).FirstOrDefault();

                cboDistrictBlood_BloodGiver.EditValue = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.DISTRICT_CODE == hisBloodGiverADO.DISTRICT_CODE_BLOOD).Select(o => o.ID).FirstOrDefault();

                cboVirAddress.EditValue = hisBloodGiverADO.VirAddress_ID_RAW;

                cboProvince.EditValue = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.PROVINCE_CODE == hisBloodGiverADO.PROVINCE_CODE).Select(o => o.ID).FirstOrDefault();

                cboDistrict.EditValue = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.DISTRICT_CODE == hisBloodGiverADO.DISTRICT_CODE).Select(o => o.ID).FirstOrDefault();

                cboCommune.EditValue = BackendDataWorker.Get<SDA_COMMUNE>().Where(o => o.COMMUNE_CODE == hisBloodGiverADO.COMMUNE_CODE).Select(o => o.ID).FirstOrDefault();

                txtAddress.Text = hisBloodGiverADO.ADDRESS;
                txtGiveCard.Text = hisBloodGiverADO.GIVE_CARD;
                txtPhone.Text = hisBloodGiverADO.PHONE;
                string CMNDNumber = "";
                DateTime? CMNDDate = null;
                string CMNDPlace = "";
                if (!String.IsNullOrEmpty(hisBloodGiverADO.CMND_NUMBER))
                {
                    CMNDNumber = hisBloodGiverADO.CMND_NUMBER;
                    CMNDDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBloodGiverADO.CMND_DATE ?? 0);
                    CMNDPlace = hisBloodGiverADO.CMND_PLACE;
                }
                else if (!String.IsNullOrEmpty(hisBloodGiverADO.CCCD_NUMBER))
                {
                    CMNDNumber = hisBloodGiverADO.CCCD_NUMBER;
                    CMNDDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBloodGiverADO.CCCD_DATE ?? 0);
                    CMNDPlace = hisBloodGiverADO.CCCD_PLACE;
                }
                else if (!String.IsNullOrEmpty(hisBloodGiverADO.PASSPORT_NUMBER))
                {
                    CMNDNumber = hisBloodGiverADO.PASSPORT_NUMBER;
                    CMNDDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBloodGiverADO.PASSPORT_DATE ?? 0);
                    CMNDPlace = hisBloodGiverADO.PASSPORT_PLACE;
                }
                txtCMNDNumber.Text = CMNDNumber;
                dtCMNDDate.DateTime = CMNDDate ?? new DateTime();
                if (dtCMNDDate.DateTime == DateTime.MinValue)
                    dtCMNDDate.EditValue = null;
                txtCMNDPlace.Text = CMNDPlace;

                //II.Khám lâm sàng
                dtExamTime_BloodGiver.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBloodGiverADO.EXAM_TIME ?? 0) ?? new DateTime();
                if (dtExamTime_BloodGiver.DateTime == DateTime.MinValue)
                    dtExamTime_BloodGiver.EditValue = null;
                spTurn_BloodGiver.EditValue = hisBloodGiverADO.TURN;
                spWeight_BloodGiver.EditValue = hisBloodGiverADO.WEIGHT;
                spPulse_BloodGiver.EditValue = hisBloodGiverADO.PULSE;
                spBloodPressureMax_BloodGiver.EditValue = hisBloodGiverADO.BLOOD_PRESSURE_MAX;
                spBloodPressureMin_BloodGiver.EditValue = hisBloodGiverADO.BLOOD_PRESSURE_MIN;
                txtNoteSubclinical_BloodGiver.Text = hisBloodGiverADO.NOTE_SUBCLINICAL;
                //III.Xét nghiệm trước lấy máu
                txtTestBeforeHB_BloodGiver.Text = hisBloodGiverADO.TEST_BEFORE_HB; ;
                cboTestBeforeHBV_BloodGiver.EditValue = hisBloodGiverADO.TEST_BEFORE_HBV;
                //IV.Kết luận lấy máu
                cboBloodVolumeID_BloodGiver.EditValue = hisBloodGiverADO.BLOOD_VOLUME_ID;
                cboExamLoginName_UserName.EditValue = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == hisBloodGiverADO.EXAM_LOGINNAME).Select(o => o.ID).FirstOrDefault();

                //V.Lấy máu
                dtExecuteTime_BloodGiver.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisBloodGiverADO.EXECUTE_TIME ?? 0) ?? new DateTime();
                if (dtExecuteTime_BloodGiver.DateTime == DateTime.MinValue)
                    dtExecuteTime_BloodGiver.EditValue = null;

                spAmount_BloodGiver.EditValue = hisBloodGiverADO.AMOUNT;
                cboExecuteLoginName_UserName.EditValue = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == hisBloodGiverADO.EXECUTE_LOGINNAME).Select(o => o.ID).FirstOrDefault();

                txtExecute_BloodGiver.Text = hisBloodGiverADO.EXECUTE;
                //VI.Xét nghiệm sau lấy máu
                cboBloodABO_BloodGiver.EditValue = hisBloodGiverADO.BLOOD_ABO_ID;
                cboBloodRh_BloodGiver.EditValue = hisBloodGiverADO.BLOOD_RH_ID;
                cboTestAfterHBV.EditValue = hisBloodGiverADO.TEST_AFTER_HBV;
                cboTestAfterHCV.EditValue = hisBloodGiverADO.TEST_AFTER_HCV;
                cboTestAfterHIV.EditValue = hisBloodGiverADO.TEST_AFTER_HIV;
                cboTestAfterGM.EditValue = hisBloodGiverADO.TEST_AFTER_GM;
                cboTestAfterKTBT.EditValue = hisBloodGiverADO.TEST_AFTER_KTBT;
                cboTestAfterNAT_HBV.EditValue = hisBloodGiverADO.TEST_AFTER_NAT_HBV;
                cboTestAfterNAT_HCV.EditValue = hisBloodGiverADO.TEST_AFTER_NAT_HCV;
                cboTestAfterNAT_HIV.EditValue = hisBloodGiverADO.TEST_AFTER_NAT_HIV;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControlsByActionType_BloodGiverForm()
        {
            try
            {
                if (this.bloodGiverActionType == ActionType.Add)
                {
                    btnAdd_BloodGiver.Enabled = true;
                    btnUpdate_BloodGiver.Enabled = false;
                    this.updatingBloodGiverADO = null;
                }
                else if (this.bloodGiverActionType == ActionType.Update)
                {
                    btnAdd_BloodGiver.Enabled = false;
                    btnUpdate_BloodGiver.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ImportProcess
        private void ProcessImport_BloodGiver()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        this.listErrorImport = new List<ADO.HisBloodGiverADO>();
                        var ImpMestListProcessor = import.GetWithCheck<ADO.HisBloodGiverADO>(0);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ImpMestListProcessor), ImpMestListProcessor));
                        if (ImpMestListProcessor != null && ImpMestListProcessor.Count > 0)
                        {
                            this.dicHisBloodGiver = new Dictionary<string, HisBloodGiverADO>();
                            this.listImportBloodGiverADO = new List<HisBloodGiverADO>();

                            List<string> listGiveCode_BloodGiver = new List<string>();
                            if (this.dicHisBloodGiver != null)
                            {
                                listGiveCode_BloodGiver = new List<string>(this.dicHisBloodGiver.Keys);
                            }

                            addListBloodGiverToProcessList(ImpMestListProcessor, listGiveCode_BloodGiver);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listImportBloodGiverADO), listImportBloodGiverADO));

                            ProcessImport_AddDataTo_dicHisBloodGiver(listImportBloodGiverADO);
                            SetDataSourceGridBlood_BloodGiver();

                            WaitingManager.Hide();
                            if (this.listErrorImport != null && this.listErrorImport.Count > 0)
                            {
                                Forms.frmImportError frm = new Forms.frmImportError(this.listErrorImport);
                                frm.ShowDialog();
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.XuLyThatBai, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.XuLyThatBai, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addListBloodGiverToProcessList(List<HisBloodGiverADO> ImpMestListProcessor, List<string> listGiveCodeExisted)
        {
            try
            {
                if (listGiveCodeExisted == null)
                    listGiveCodeExisted = new List<string>();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("addListBloodGiverToProcessList(): " + Inventec.Common.Logging.LogUtil.GetMemberName(() => ImpMestListProcessor), ImpMestListProcessor));
                if (ImpMestListProcessor == null || ImpMestListProcessor.Count == 0)
                    return;
                foreach (var bloodGiverImport in ImpMestListProcessor)
                {
                    //GIVE_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.GIVE_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Mã hồ sơ'");
                    }
                    else
                    {
                        bloodGiverImport.GIVE_CODE = bloodGiverImport.GIVE_CODE.Trim();
                        if (listGiveCodeExisted.Contains(bloodGiverImport.GIVE_CODE))
                        {
                            bloodGiverImport.ErrorDescriptions.Add(String.Format("Mã hồ sơ '{0}' đã tồn tại trong danh sách nhập", bloodGiverImport.GIVE_CODE));
                        }
                    }
                    //GIVE_NAME
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.GIVE_NAME))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Họ tên'");
                    }
                    else
                    {
                        bloodGiverImport.GIVE_NAME = bloodGiverImport.GIVE_NAME.Trim();
                    }
                    //DOB_str
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.DOB_str))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Ngày sinh'");
                    }
                    else
                    {
                        bloodGiverImport.DOB_str = bloodGiverImport.DOB_str.Trim();
                        if (Helpers.DateTimeUtil.IsValidDateStr(bloodGiverImport.DOB_str))
                        {
                            System.DateTime? dob = Helpers.DateTimeUtil.DateStrToSystemDateTime(bloodGiverImport.DOB_str);
                            if (dob != null && dob != DateTime.MinValue)
                            {
                                bloodGiverImport.DOB = Helpers.DateTimeUtil.SystemDateTimeToTimeNumber(dob) ?? 0;
                                bloodGiverImport.DOB_ForDisplay = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dob);
                            }
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Ngày sinh' không đúng định dạng");
                        }
                    }
                    //GENDER_NAME
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.GENDER_NAME))
                    {
                        bloodGiverImport.GENDER_NAME = bloodGiverImport.GENDER_NAME.Trim();
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.GENDER_NAME != null && o.GENDER_NAME.ToUpper() == (bloodGiverImport.GENDER_NAME ?? "").ToUpper());
                        if (gender != null)
                        {
                            bloodGiverImport.GENDER_ID = gender.ID;
                            bloodGiverImport.GENDER_ForDisplay = gender.GENDER_NAME;
                        }
                        else
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Giới tính' không chính xác");
                    }

                    //WORK_PLACE_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.WORK_PLACE_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Đơn vị'");
                    }
                    else
                    {
                        bloodGiverImport.WORK_PLACE_CODE = bloodGiverImport.WORK_PLACE_CODE.Trim();
                        var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.WORK_PLACE_CODE != null && o.WORK_PLACE_CODE.ToUpper() == (bloodGiverImport.WORK_PLACE_CODE ?? "").ToUpper());
                        if (workPlace != null)
                        {
                            bloodGiverImport.WORK_PLACE_ID = workPlace.ID;
                            bloodGiverImport.WORK_PLACE = workPlace.WORK_PLACE_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Đơn vị' không chính xác");
                        }
                    }
                    //GIVE_TYPE
                    if (bloodGiverImport.GIVE_TYPE <= 0)
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Đối tượng'");
                    }
                    else
                    {
                        var giveType = GiveTypeADO.ListGiveType.FirstOrDefault(o => o.ID == (long)bloodGiverImport.GIVE_TYPE);
                        if (giveType == null)
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Đối tượng' không chính xác");
                    }
                    //CAREER_CODE
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.CAREER_CODE))
                    {
                        bloodGiverImport.CAREER_CODE = bloodGiverImport.CAREER_CODE.Trim();
                        var career = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.CAREER_CODE != null && o.CAREER_CODE.ToUpper() == (bloodGiverImport.CAREER_CODE ?? "").ToUpper());
                        if (career != null)
                        {
                            bloodGiverImport.CAREER_ID = career.ID;
                            bloodGiverImport.CAREER_NAME = career.CAREER_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Nghề nghiệp' không chính xác");
                        }
                    }

                    //IS_WORK_PLACE
                    bool isWorkPlace = false;
                    if (!String.IsNullOrWhiteSpace(bloodGiverImport.IS_WORK_PLACE))
                    {
                        bloodGiverImport.IS_WORK_PLACE = bloodGiverImport.IS_WORK_PLACE.Trim();
                        if (bloodGiverImport.IS_WORK_PLACE.ToUpper() == "X")
                            isWorkPlace = true;
                    }
                    //WORK_PLACE
                    if (isWorkPlace = false)
                    {
                        if (String.IsNullOrWhiteSpace(bloodGiverImport.WORK_PLACE))
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Nơi công tác'");
                        }
                        else
                        {
                            bloodGiverImport.WORK_PLACE = bloodGiverImport.WORK_PLACE.Trim();
                        }
                    }

                    //NATIONAL_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.NATIONAL_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Quốc gia'");
                    }
                    else
                    {
                        bloodGiverImport.NATIONAL_CODE = bloodGiverImport.NATIONAL_CODE.Trim();
                        var national = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE != null && o.NATIONAL_CODE.ToUpper() == (bloodGiverImport.NATIONAL_CODE ?? "").ToUpper());
                        if (national != null)
                        {
                            bloodGiverImport.NATIONAL_CODE = national.NATIONAL_CODE;
                            bloodGiverImport.NATIONAL_NAME = national.NATIONAL_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Quốc gia' không chính xác");
                        }
                    }
                    //IS_VIR_ADDRESS
                    bool isVirAddress = false;
                    if (!String.IsNullOrWhiteSpace(bloodGiverImport.IS_VIR_ADDRESS))
                    {
                        bloodGiverImport.IS_VIR_ADDRESS = bloodGiverImport.IS_VIR_ADDRESS.Trim();
                        if (bloodGiverImport.IS_VIR_ADDRESS.ToUpper() == "X")
                            isVirAddress = true;
                    }

                    //PROVINCE_CODE_BLOOD
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.PROVINCE_CODE_BLOOD))
                    {
                        bloodGiverImport.PROVINCE_CODE_BLOOD = bloodGiverImport.PROVINCE_CODE_BLOOD.Trim();
                        var provinceBlood = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE != null && o.PROVINCE_CODE.ToUpper() == (bloodGiverImport.PROVINCE_CODE_BLOOD ?? "").ToUpper());
                        if (provinceBlood != null)
                        {
                            bloodGiverImport.PROVINCE_CODE_BLOOD = provinceBlood.PROVINCE_CODE;
                            bloodGiverImport.PROVINCE_NAME_BLOOD = provinceBlood.PROVINCE_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Tỉnh/TP' không chính xác");
                        }
                    }
                    //DISTRICT_CODE_BLOOD
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.DISTRICT_CODE_BLOOD))
                    {
                        bloodGiverImport.DISTRICT_CODE_BLOOD = bloodGiverImport.DISTRICT_CODE_BLOOD.Trim();
                        var districtBlood = BackendDataWorker.Get<SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE != null && o.DISTRICT_CODE.ToUpper() == (bloodGiverImport.DISTRICT_CODE_BLOOD ?? "").ToUpper());
                        if (districtBlood != null)
                        {
                            bloodGiverImport.DISTRICT_CODE_BLOOD = districtBlood.DISTRICT_CODE;
                            bloodGiverImport.DISTRICT_NAME_BLOOD = districtBlood.DISTRICT_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Quận/Huyện' không chính xác");
                        }
                    }

                    //VIR_ADDRESS
                    //if (String.IsNullOrWhiteSpace(bloodGiverImport.VIR_ADDRESS))
                    //{
                    //    bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Địa chỉ thường trú'");
                    //}
                    //else
                    //{
                    //    bloodGiverImport.VIR_ADDRESS = bloodGiverImport.VIR_ADDRESS.Trim();
                    //}

                    //PROVINCE_CODE
                    SDA_PROVINCE province = null;
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.PROVINCE_CODE))
                    {
                        bloodGiverImport.PROVINCE_CODE = bloodGiverImport.PROVINCE_CODE != null ? bloodGiverImport.PROVINCE_CODE.Trim() : "";
                        province = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE != null && o.PROVINCE_CODE.ToUpper() == (bloodGiverImport.PROVINCE_CODE ?? "").ToUpper());
                        if (province != null)
                        {
                            bloodGiverImport.PROVINCE_CODE = province.PROVINCE_CODE;
                            bloodGiverImport.PROVINCE_NAME = province.PROVINCE_NAME;
                            if (isVirAddress)
                            {
                                bloodGiverImport.PROVINCE_CODE_BLOOD = province.PROVINCE_CODE;
                                bloodGiverImport.PROVINCE_NAME_BLOOD = province.PROVINCE_NAME;
                            }
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Mã tỉnh' không chính xác");
                        }
                    }
                    //DISTRICT_CODE
                    SDA_DISTRICT district = null;
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.DISTRICT_CODE))
                    {
                        bloodGiverImport.DISTRICT_CODE = bloodGiverImport.DISTRICT_CODE != null ? bloodGiverImport.DISTRICT_CODE.Trim() : "";
                        district = BackendDataWorker.Get<SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE != null && o.DISTRICT_CODE.ToUpper() == (bloodGiverImport.DISTRICT_CODE ?? "").ToUpper());
                        if (district != null)
                        {
                            bloodGiverImport.DISTRICT_CODE = district.DISTRICT_CODE;
                            bloodGiverImport.DISTRICT_NAME = district.DISTRICT_NAME;
                            if (isVirAddress)
                            {
                                bloodGiverImport.DISTRICT_CODE_BLOOD = district.DISTRICT_CODE;
                                bloodGiverImport.DISTRICT_NAME_BLOOD = district.DISTRICT_NAME;
                            }
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Mã huyện' không chính xác");
                        }
                    }
                    //COMMUNE_CODE
                    bloodGiverImport.COMMUNE_CODE = bloodGiverImport.COMMUNE_CODE != null ? bloodGiverImport.COMMUNE_CODE.Trim() : "";
                    var commune = BackendDataWorker.Get<SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE != null && o.COMMUNE_CODE.ToUpper() == (bloodGiverImport.COMMUNE_CODE ?? "").ToUpper());
                    if (!String.IsNullOrWhiteSpace(bloodGiverImport.COMMUNE_CODE))
                    {
                        if (commune != null)
                        {
                            bloodGiverImport.COMMUNE_CODE = commune.COMMUNE_CODE;
                            bloodGiverImport.COMMUNE_NAME = commune.COMMUNE_NAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Mã xã' không chính xác");
                        }
                    }

                    //VIR_ADDRESS (*)
                    if (commune == null)
                    {
                        var dataNoCommunes = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().Where(o => o.ID < 0).ToList();
                        if (district != null && province != null)
                        {
                            var dataNoCommunes_Result = dataNoCommunes.Where(o => o.DISTRICT_CODE != null && o.DISTRICT_CODE == district.DISTRICT_CODE
                                                                                && district.PROVINCE_ID == province.ID).FirstOrDefault();
                            if (dataNoCommunes_Result != null)
                            {
                                bloodGiverImport.VirAddress_ID_RAW = dataNoCommunes_Result.ID_RAW;
                                bloodGiverImport.VIR_ADDRESS = dataNoCommunes_Result.RENDERER_PDC_NAME;
                            }
                        }
                    }
                    else
                    {
                        var communeADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.COMMUNE_CODE != null && o.COMMUNE_CODE == commune.COMMUNE_CODE);
                        if (communeADO != null)
                        {
                            bloodGiverImport.VirAddress_ID_RAW = communeADO.ID_RAW;
                            bloodGiverImport.VIR_ADDRESS = communeADO.RENDERER_PDC_NAME;
                        }
                    }

                    //ADDRESS
                    bloodGiverImport.ADDRESS = bloodGiverImport.ADDRESS != null ? bloodGiverImport.ADDRESS.Trim() : "";
                    //GIVE_CARD
                    bloodGiverImport.GIVE_CARD = bloodGiverImport.GIVE_CARD != null ? bloodGiverImport.GIVE_CARD.Trim() : "";
                    //PHONE
                    bloodGiverImport.PHONE = bloodGiverImport.PHONE != null ? bloodGiverImport.PHONE.Trim() : "";

                    //CMND_CCCD_DATE
                    bloodGiverImport.CMND_CCCD_DATE = bloodGiverImport.CMND_CCCD_DATE != null ? bloodGiverImport.CMND_CCCD_DATE.Trim() : "";
                    if (!String.IsNullOrWhiteSpace(bloodGiverImport.CMND_CCCD_DATE))
                    {
                        if (Helpers.DateTimeUtil.IsValidDateStr(bloodGiverImport.CMND_CCCD_DATE))
                        {
                            System.DateTime? date = Helpers.DateTimeUtil.DateStrToSystemDateTime(bloodGiverImport.CMND_CCCD_DATE);
                            if (date != null && date != DateTime.MinValue)
                            {
                                bloodGiverImport.CMND_CCCD_DATE_number = Helpers.DateTimeUtil.SystemDateTimeToTimeNumber(date) ?? 0;
                            }
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Ngày cấp' không đúng định dạng");
                        }
                    }
                    //CMND_CCCD_PLACE
                    bloodGiverImport.CMND_CCCD_PLACE = bloodGiverImport.CMND_CCCD_PLACE != null ? bloodGiverImport.CMND_CCCD_PLACE.Trim() : "";
                    //CMND_CCCD_NUMBER
                    if (!String.IsNullOrWhiteSpace(bloodGiverImport.CMND_CCCD_NUMBER))
                    {
                        bloodGiverImport.CMND_CCCD_NUMBER = bloodGiverImport.CMND_CCCD_NUMBER.Trim();
                        var CMNDNumber = bloodGiverImport.CMND_CCCD_NUMBER;
                        if (CMNDNumber.Length == 12 && CMNDNumber.All(char.IsDigit))
                        {
                            bloodGiverImport.CCCD_NUMBER = bloodGiverImport.CMND_CCCD_NUMBER;
                            bloodGiverImport.CCCD_DATE = bloodGiverImport.CMND_CCCD_DATE_number;
                            bloodGiverImport.CCCD_PLACE = bloodGiverImport.CMND_CCCD_PLACE;
                        }
                        else if (CMNDNumber.Length == 9 && CMNDNumber.All(char.IsDigit))
                        {
                            bloodGiverImport.CMND_NUMBER = bloodGiverImport.CMND_CCCD_NUMBER;
                            bloodGiverImport.CMND_DATE = bloodGiverImport.CMND_CCCD_DATE_number;
                            bloodGiverImport.CMND_PLACE = bloodGiverImport.CMND_CCCD_PLACE;
                        }
                        else if (CMNDNumber.Length == 9 && CMNDNumber.All(char.IsLetterOrDigit))
                        {
                            bloodGiverImport.PASSPORT_NUMBER = bloodGiverImport.CMND_CCCD_NUMBER;
                            bloodGiverImport.PASSPORT_DATE = bloodGiverImport.CMND_CCCD_DATE_number;
                            bloodGiverImport.PASSPORT_PLACE = bloodGiverImport.CMND_CCCD_PLACE;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'CMND/CCCD/HC' không đúng định dạng");
                        }
                    }
                    //EXAM_TIME_str
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.EXAM_TIME_str))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Thời gian khám'");
                    }
                    else
                    {
                        bloodGiverImport.EXAM_TIME_str = bloodGiverImport.EXAM_TIME_str.Trim();
                        if (Helpers.DateTimeUtil.IsValidDateTimeStr(bloodGiverImport.EXAM_TIME_str))
                        {
                            System.DateTime? time = Helpers.DateTimeUtil.DateTimeStrToSystemDateTime(bloodGiverImport.EXAM_TIME_str);
                            if (time != null && time != DateTime.MinValue)
                            {
                                bloodGiverImport.EXAM_TIME = Helpers.DateTimeUtil.SystemDateTimeToTimeNumber(time);
                            }
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Thời gian khám' không đúng định dạng");
                        }
                    }
                    //NOTE_SUBCLINICAL 
                    if (!string.IsNullOrWhiteSpace(bloodGiverImport.NOTE_SUBCLINICAL))
                    {
                        bloodGiverImport.NOTE_SUBCLINICAL = bloodGiverImport.NOTE_SUBCLINICAL.Trim();
                    }
                    //TEST_BEFORE_HB
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.TEST_BEFORE_HB))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'HB'");
                    }
                    else
                    {
                        bloodGiverImport.TEST_BEFORE_HB = bloodGiverImport.TEST_BEFORE_HB.Trim();
                    }
                    //TEST_BEFORE_HBV

                    //VOLUME
                    if (bloodGiverImport.VOLUME == null)
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Kết luận'");
                    }
                    else
                    {
                        var bloodVolume = BackendDataWorker.Get<HIS_BLOOD_VOLUME>().FirstOrDefault(o => o.VOLUME != null && o.VOLUME == bloodGiverImport.VOLUME);
                        if (bloodVolume != null)
                        {
                            bloodGiverImport.BLOOD_VOLUME_ID = bloodVolume.ID;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add(String.Format("Thông tin 'Kết luận' không chính xác"));
                        }
                    }
                    //EXAM_LOGINNAME
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.EXAM_LOGINNAME))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'BS khám hiến máu'");
                    }
                    else
                    {
                        bloodGiverImport.EXAM_LOGINNAME = bloodGiverImport.EXAM_LOGINNAME.Trim();
                        var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME != null && o.LOGINNAME.Trim().ToUpper() == (bloodGiverImport.EXAM_LOGINNAME ?? "").ToUpper());
                        if (user != null)
                        {
                            bloodGiverImport.EXAM_LOGINNAME = user.LOGINNAME;
                            bloodGiverImport.EXAM_USERNAME = user.USERNAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'BS khám hiến máu' không chính xác");
                        }
                    }
                    //EXECUTE_TIME_str
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.EXECUTE_TIME_str))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Lấy máu lúc'");
                    }
                    else
                    {
                        bloodGiverImport.EXECUTE_TIME_str = bloodGiverImport.EXECUTE_TIME_str.Trim();
                        if (Helpers.DateTimeUtil.IsValidDateTimeStr(bloodGiverImport.EXECUTE_TIME_str))
                        {
                            System.DateTime? time = Helpers.DateTimeUtil.DateTimeStrToSystemDateTime(bloodGiverImport.EXECUTE_TIME_str);
                            if (time != null && time != DateTime.MinValue)
                            {
                                bloodGiverImport.EXECUTE_TIME = Helpers.DateTimeUtil.SystemDateTimeToTimeNumber(time);
                            }
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Lấy máu lúc' không đúng định dạng");
                        }
                    }
                    //EXECUTE_LOGINNAME
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.EXECUTE_LOGINNAME))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Người lấy máu'");
                    }
                    else
                    {
                        bloodGiverImport.EXECUTE_LOGINNAME = bloodGiverImport.EXECUTE_LOGINNAME.Trim();
                        var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME != null && o.LOGINNAME.Trim().ToUpper() == (bloodGiverImport.EXECUTE_LOGINNAME ?? "").ToUpper());
                        if (user != null)
                        {
                            bloodGiverImport.EXECUTE_LOGINNAME = user.LOGINNAME;
                            bloodGiverImport.EXECUTE_USERNAME = user.USERNAME;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Người lấy máu' không chính xác");
                        }
                    }
                    //EXECUTE

                    //BLOOD_ABO_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.BLOOD_ABO_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'Nhóm máu'");
                    }
                    else
                    {
                        bloodGiverImport.BLOOD_ABO_CODE = bloodGiverImport.BLOOD_ABO_CODE.Trim();
                        var bloodABO = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.BLOOD_ABO_CODE != null && o.BLOOD_ABO_CODE.Trim().ToUpper() == (bloodGiverImport.BLOOD_ABO_CODE ?? "").ToUpper());
                        if (bloodABO != null)
                        {
                            bloodGiverImport.BLOOD_ABO_ID = bloodABO.ID;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'Nhóm máu' không chính xác");
                        }
                    }
                    //BLOOD_RH_CODE
                    if (String.IsNullOrWhiteSpace(bloodGiverImport.BLOOD_RH_CODE))
                    {
                        bloodGiverImport.ErrorDescriptions.Add("Thiếu thông tin 'RH'");
                    }
                    else
                    {
                        bloodGiverImport.BLOOD_RH_CODE = bloodGiverImport.BLOOD_RH_CODE.Trim();
                        var bloodRH = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.BLOOD_RH_CODE != null && o.BLOOD_RH_CODE.Trim().ToUpper() == (bloodGiverImport.BLOOD_RH_CODE ?? "").ToUpper());
                        if (bloodRH != null)
                        {
                            bloodGiverImport.BLOOD_RH_ID = bloodRH.ID;
                        }
                        else
                        {
                            bloodGiverImport.ErrorDescriptions.Add("Thông tin 'RH' không chính xác");
                        }
                    }
                    //TEST_AFTER_HBV
                    //TEST_AFTER_HCV
                    //TEST_AFTER_HIV
                    //TEST_AFTER_GM
                    //TEST_AFTER_KTBT
                    //TEST_AFTER_NAT_HBV
                    //TEST_AFTER_NAT_HCV
                    //TEST_AFTER_NAT_HIV
                    //TEST_AFTER_SLKTB


                    // add to listError or Valid
                    if (bloodGiverImport.ErrorDescriptions.Count > 0)
                    {
                        listErrorImport.Add(bloodGiverImport);
                    }
                    else
                    {
                        //this.listImportBloodGiverADO.Insert(0, bloodGiverImport);
                        this.listImportBloodGiverADO.Add(bloodGiverImport);
                        listGiveCodeExisted.Add(bloodGiverImport.GIVE_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessImport_AddDataTo_dicHisBloodGiver(List<HisBloodGiverADO> listImportBloodGiverADO)
        {
            try
            {
                if (listImportBloodGiverADO == null || listImportBloodGiverADO.Count == 0)
                    return;
                if (this.dicHisBloodGiver == null)
                    this.dicHisBloodGiver = new Dictionary<string, HisBloodGiverADO>();
                if (this.dicHisBloodGiver_BloodAdo == null)
                    this.dicHisBloodGiver_BloodAdo = new Dictionary<string, List<VHisBloodADO>>();

                List<long> listbloodVolumeID = new List<long>();
                foreach (var item in listImportBloodGiverADO)
                {
                    if (item.BLOOD_VOLUME_ID != null)
                        listbloodVolumeID.Add(item.BLOOD_VOLUME_ID ?? 0);
                }
                List<HIS_BLTY_VOLUME> listAll_HIS_BLTY_VOLUME = GetListHisBltyVolume_From_BLOOD_VOLUME_ID(listbloodVolumeID);

                foreach (var bloodGiverADO in listImportBloodGiverADO)
                {
                    this.dicHisBloodGiver.Add(bloodGiverADO.GIVE_CODE, bloodGiverADO);
                    if (bloodGiverADO.BLOOD_VOLUME_ID != null)
                    {
                        List<HIS_BLTY_VOLUME> list_HIS_BLTY_VOLUME = listAll_HIS_BLTY_VOLUME.Where(o => o.BLOOD_VOLUME_ID == bloodGiverADO.BLOOD_VOLUME_ID).ToList();
                        List<long> listBloodTypeID = list_HIS_BLTY_VOLUME != null ? list_HIS_BLTY_VOLUME.Select(o => o.BLOOD_TYPE_ID).ToList() : new List<long>();

                        List<V_HIS_BLOOD_TYPE> listBloodType = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().Where(o => listBloodTypeID.Contains(o.ID)).ToList();

                        List<VHisBloodADO> listBloodADO = new List<VHisBloodADO>();
                        if (listBloodType != null && listBloodType.Count > 0)
                        {
                            foreach (var item in listBloodType)
                            {
                                VHisBloodADO bloodADO = new VHisBloodADO(item);
                                bloodADO.IsBloodDonation = true;
                                bloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                                bloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1} - {2} - {3}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME, bloodGiverADO.DOB_ForDisplay, bloodGiverADO.GENDER_ForDisplay);
                                bloodADO.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                                bloodADO.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                                bloodADO.DOB_ForDisplay = bloodGiverADO.DOB_ForDisplay;
                                bloodADO.GENDER_ForDisplay = bloodGiverADO.GENDER_ForDisplay;

                                bloodADO.BLOOD_ABO_ID = bloodGiverADO.BLOOD_ABO_ID ?? 0;
                                var blood_Abo = BackendDataWorker.Get<HIS_BLOOD_ABO>().FirstOrDefault(o => o.ID == (bloodGiverADO.BLOOD_ABO_ID ?? 0));
                                if (blood_Abo != null)
                                    bloodADO.BLOOD_ABO_CODE = blood_Abo.BLOOD_ABO_CODE;

                                bloodADO.BLOOD_RH_ID = bloodGiverADO.BLOOD_RH_ID;
                                var blood_Rh = BackendDataWorker.Get<HIS_BLOOD_RH>().FirstOrDefault(o => o.ID == (bloodGiverADO.BLOOD_RH_ID ?? 0));
                                if (blood_Rh != null)
                                    bloodADO.BLOOD_RH_CODE = blood_Rh.BLOOD_RH_CODE;

                                DateTime? executeTime = null;
                                if (bloodGiverADO.EXECUTE_TIME != null)
                                    executeTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(bloodGiverADO.EXECUTE_TIME ?? 0);
                                TimeSpan alertExpiredDate = new TimeSpan();
                                if (item.ALERT_EXPIRED_DATE != null)
                                    alertExpiredDate = new TimeSpan((int)(item.ALERT_EXPIRED_DATE ?? 0), 0, 0, 0);
                                DateTime? expiredDate = null;
                                if (executeTime != null)
                                    expiredDate = executeTime + alertExpiredDate;

                                if (expiredDate != null)
                                    bloodADO.EXPIRED_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(expiredDate);

                                bloodADO.BLOOD_CODE = (item.BLOOD_TYPE_CODE ?? "") + (bloodGiverADO.GIVE_CODE ?? "");

                                listBloodADO.Add(bloodADO);
                            }
                        }
                        else
                        {
                            VHisBloodADO emptyBloodADO = new VHisBloodADO();
                            emptyBloodADO.IsEmptyRow = true;
                            emptyBloodADO.IsBloodDonation = true;
                            emptyBloodADO.BloodDonationCode = bloodGiverADO.GIVE_CODE;
                            emptyBloodADO.BloodDonationStr_ForDisplayGroup = String.Format("Hồ sơ hiến máu: {0} - {1}", bloodGiverADO.GIVE_CODE, bloodGiverADO.GIVE_NAME);
                            emptyBloodADO.GIVE_CODE = bloodGiverADO.GIVE_CODE;
                            emptyBloodADO.GIVE_NAME = bloodGiverADO.GIVE_NAME;
                            listBloodADO.Add(emptyBloodADO);
                        }

                        if (!this.dicHisBloodGiver_BloodAdo.ContainsKey(bloodGiverADO.GIVE_CODE))
                        {
                            this.dicHisBloodGiver_BloodAdo.Add(bloodGiverADO.GIVE_CODE, listBloodADO);
                        }
                        else
                        {
                            this.dicHisBloodGiver_BloodAdo[bloodGiverADO.GIVE_CODE] = listBloodADO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Validation
        private void ValidControl_BloodGiverForm()
        {
            try
            {
                ValidationSingleControl3(txtGiveCode_BloodGiver);
                ValidationSingleControl3(txtGiveName_BloodGiver);
                //ValidationSingleControl3(dtDOB_BloodGiver);
                ValidationDOB_BloodGiver();
                //ValidationSingleControl3(cboGender_BloodGiver);
                ValidationSingleControl3(cboWorkPlaceID_BloodGiver);
                ValidationSingleControl3(cboGiveType_BloodGiver);
                //ValidationSingleControl3(cboCareer_BloodGiver);
                ValidationSingleControl3(txtWorkPlace_BloodGiver);
                ValidationSingleControl3(cboNational_BloodGiver);
                ValidationSingleControl3(cboProvinceBlood_BloodGiver);
                ValidationSingleControl3(cboDistrictBlood_BloodGiver);
                //
                //ValidationVIRAddress();
                ValidationCMNDNumber();
                ValidationSingleControl3(dtExamTime_BloodGiver);
                //ValidationSingleControl3(spTurn_BloodGiver);
                //ValidationSingleControl3(spWeight_BloodGiver);
                //ValidationSingleControl3(spPulse_BloodGiver);
                //ValidationSingleControl3(spBloodPressureMax_BloodGiver);
                //ValidationSingleControl3(spBloodPressureMin_BloodGiver);
                //ValidationSingleControl3(txtNoteSubclinical_BloodGiver);
                ValidationSingleControl3(cboTestBeforeHBV_BloodGiver);
                ValidationSingleControl3(cboBloodVolumeID_BloodGiver);
                ValidationSingleControl3(cboExamLoginName_UserName);
                ValidationSingleControl3(dtExecuteTime_BloodGiver);
                //ValidationSingleControl3(spAmount_BloodGiver);
                ValidationSingleControl3(cboExecuteLoginName_UserName);
                ValidationSingleControl3(cboBloodABO_BloodGiver);
                ValidationSingleControl3(cboBloodRh_BloodGiver);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationDOB_BloodGiver()
        {
            try
            {
                Validation.DOBValidationRule validRule = new Validation.DOBValidationRule();
                validRule.dtPatientDob = this.dtDOB_BloodGiver;
                validRule.txtPatientDob = this.txtDOB_BloodGiver;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider3.SetValidationRule(txtDOB_BloodGiver, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationVIRAddress()
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = cboVirAddress;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider3.SetValidationRule(txtVirAddress, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationCMNDNumber()
        {
            try
            {
                Validation.CMNDNumberValidationRule validRule = new Validation.CMNDNumberValidationRule();
                validRule.txtCMND = txtCMNDNumber;
                validRule.isValid = false;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider3.SetValidationRule(txtCMNDNumber, validRule);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void ValidationSingleControl3(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider3.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region InitCombo
        private void InitComboCommonUtil(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth, string displayMember1, int displayMember1Width, int displayMember1VisibleIndex)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 150), 1, true));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 150);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 550), 2, true));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 550);
                }
                if (!String.IsNullOrEmpty(displayMember1))
                {
                    columnInfos.Add(new ColumnInfo(displayMember1, "", (displayMember1Width > 0 ? displayMember1Width : 250), (displayMember1VisibleIndex > 0 ? displayMember1VisibleIndex : -1), true));
                    popupWidth += (displayMember1Width > 0 ? displayMember1Width : (displayMember1VisibleIndex > 0 ? 250 : 0));
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopupExt(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    Inventec.Common.Controls.PopupLoader.PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboVirAddress(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs)
        {
            try
            {
                if (communeADOs != null)
                {
                    this.InitComboCommonUtil(this.cboVirAddress, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", 650, "SEARCH_CODE_COMMUNE", 150, "RENDERER_PDC_NAME_UNSIGNED", 5, 0);
                }
                this.cboVirAddress.EditValue = null;
                this.cboVirAddress.Properties.Buttons[1].Visible = false;
                this.FocusShowpopupExt(this.cboVirAddress, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboVirAddress()
        {
            try
            {
                List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                if (communeADOs != null)
                {
                    this.InitComboCommonUtil(this.cboVirAddress, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", 650, "SEARCH_CODE_COMMUNE", 150, "RENDERER_PDC_NAME_UNSIGNED", 5, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTestResult()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Value", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTestBeforeHBV_BloodGiver, TestResultADO.ListTestResult_01, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterHBV, TestResultADO.ListTestResult_02, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterHCV, TestResultADO.ListTestResult_02, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterHIV, TestResultADO.ListTestResult_02, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterGM, TestResultADO.ListTestResult_02, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterKTBT, TestResultADO.ListTestResult_02, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterNAT_HBV, TestResultADO.ListTestResult_03, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterNAT_HCV, TestResultADO.ListTestResult_03, controlEditorADO);
                ControlEditorLoader.Load(cboTestAfterNAT_HIV, TestResultADO.ListTestResult_03, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboGender()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboGender_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboWorkPlaceID()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 350);
                var dataSource = BackendDataWorker.Get<HIS_WORK_PLACE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboWorkPlaceID_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboGiveType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GiveType", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GiveType", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboGiveType_BloodGiver, GiveTypeADO.ListGiveType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCareer()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("CAREER_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_NAME", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboCareer_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboNational()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NATIONAL_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NATIONAL_NAME", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<SDA_NATIONAL>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboNational_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboProvince()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PROVINCE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PROVINCE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROVINCE_NAME", "ID", columnInfos, false, 200);
                List<SDA_PROVINCE> dataSource = null;
                if (cboNational_BloodGiver.EditValue != null)
                {
                    dataSource = BackendDataWorker.Get<SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                                            && o.NATIONAL_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboNational_BloodGiver.EditValue.ToString())).ToList();
                }

                ControlEditorLoader.Load(cboProvinceBlood_BloodGiver, dataSource, controlEditorADO);
                cboProvinceBlood_BloodGiver.EditValue = null;
                ControlEditorLoader.Load(cboProvince, dataSource, controlEditorADO);
                cboProvince.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboDistrict()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DISTRICT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("DISTRICT_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DISTRICT_NAME", "ID", columnInfos, false, 200);
                List<SDA_DISTRICT> dataSource = null;
                if (cboProvinceBlood_BloodGiver.EditValue != null)
                {
                    dataSource = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                                            && o.PROVINCE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboProvinceBlood_BloodGiver.EditValue.ToString())).ToList();
                }
                ControlEditorLoader.Load(cboDistrictBlood_BloodGiver, dataSource, controlEditorADO);
                if (dataSource == null || (cboDistrictBlood_BloodGiver.EditValue != null && !dataSource.Select(o => o.ID).Contains(Inventec.Common.TypeConvert.Parse.ToInt64(cboDistrictBlood_BloodGiver.EditValue.ToString()))))
                {
                    cboDistrictBlood_BloodGiver.EditValue = null;
                }
                //
                dataSource = null;
                if (cboProvince.EditValue != null)
                {
                    dataSource = BackendDataWorker.Get<SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                                                && o.PROVINCE_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboProvince.EditValue.ToString())).ToList();
                }
                ControlEditorLoader.Load(cboDistrict, dataSource, controlEditorADO);
                if (dataSource == null || (cboDistrict.EditValue != null && !dataSource.Select(o => o.ID).Contains(Inventec.Common.TypeConvert.Parse.ToInt64(cboDistrict.EditValue.ToString()))))
                {
                    cboDistrict.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCommune()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("COMMUNE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("COMMUNE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("COMMUNE_NAME", "ID", columnInfos, false, 200);
                List<SDA_COMMUNE> dataSource = null;
                if (cboDistrict.EditValue != null)
                {
                    dataSource = BackendDataWorker.Get<SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                                            && o.DISTRICT_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDistrict.EditValue.ToString())).ToList();
                }

                ControlEditorLoader.Load(cboCommune, dataSource, controlEditorADO);
                if (dataSource == null || (cboCommune.EditValue != null && !dataSource.Select(o => o.ID).Contains(Inventec.Common.TypeConvert.Parse.ToInt64(cboCommune.EditValue.ToString()))))
                {
                    cboCommune.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBloodVolume()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("VOLUME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("VOLUME", "ID", columnInfos, false, 150);
                var dataSource = BackendDataWorker.Get<HIS_BLOOD_VOLUME>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DONATION == 1).ToList();
                ControlEditorLoader.Load(cboBloodVolumeID_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboExamLoginName_UserName, dataSource, controlEditorADO);
                ControlEditorLoader.Load(cboExecuteLoginName_UserName, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBloodABO()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<HIS_BLOOD_ABO>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboBloodABO_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBloodRh()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 200);
                var dataSource = BackendDataWorker.Get<HIS_BLOOD_RH>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ControlEditorLoader.Load(cboBloodRh_BloodGiver, dataSource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Custom button Group's header
        Rectangle GetButtonBounds(GridGroupRowInfo info)
        {
            Rectangle r = info.ButtonBounds;
            r.Offset(r.Width * 2, 0);
            r.Width = 40;
            r.Location = new Point(info.Bounds.Right - info.ButtonBounds.Width - 30, r.Location.Y);
            return r;
        }

        protected void DrawButton(Graphics g, Rectangle r)
        {
            DevExpress.XtraEditors.ViewInfo.ButtonEditViewInfo info;
            DevExpress.XtraEditors.Drawing.ButtonEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = buttonEdit1.Properties.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.ButtonEditViewInfo;
            painter = buttonEdit1.Properties.CreatePainter() as DevExpress.XtraEditors.Drawing.ButtonEditPainter;
            info.Bounds = r;
            info.CalcViewInfo(g);
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }
        #endregion
    }
}
