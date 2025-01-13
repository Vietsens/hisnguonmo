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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SurgServiceReqExecute.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute
{
    public partial class SurgServiceReqExecuteControl : UserControlBase
    {
        private List<AcsUserADO> lstReAcsUserADO;

        private void ComboChuanDoanTD(DevExpress.XtraEditors.LookUpEdit cbo)
        {
            try
            {
                List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, icds, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboMethodPTTT()
        {
            try
            {
                List<HIS_PTTT_METHOD> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_METHOD>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_METHOD), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboMethod, datas, controlEditorADO);

                if (this.sereServPTTT != null && this.sereServPTTT.PTTT_METHOD_ID > 0)
                {
                    txtMethodCode.Text = this.sereServPTTT.PTTT_METHOD_CODE;
                    cboMethod.EditValue = this.sereServPTTT.PTTT_METHOD_ID;
                }
                else
                    this.SetDefaultCboPTMethod(sereServ, cboMethod, txtMethodCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboPTTTGroup()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SurgServiceReqExecute").FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Debug("moduleData" + Inventec.Common.Logging.LogUtil.TraceData("_moduleData", moduleData));
                List<HIS_PTTT_GROUP> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_GROUP>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_GROUP>>("api/HisPtttGroup/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_GROUP), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbbPtttGroup, datas, controlEditorADO);
                this.SetDefaultCboPTTTGroupOnly(sereServ);
                if (this.sereServPTTT != null && this.sereServPTTT.PTTT_GROUP_ID != null)
                {
                    txtPtttGroupCode.Text = this.sereServPTTT.PTTT_GROUP_CODE;
                    cbbPtttGroup.EditValue = this.sereServPTTT.PTTT_GROUP_ID;
                    cbbPtttGroup.Properties.Buttons[1].Visible = cbbPtttGroup.Properties.Buttons[1].Enabled ? true : false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboEmotionlessMothod()
        {
            try
            {
                //List<HIS_EMOTIONLESS_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(p => p.IS_ACTIVE == 1
                //   && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList();

                if (BackendDataWorker.IsExistsKey<HIS_EMOTIONLESS_METHOD>())
                {
                    dataEmotionlessMethod = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataEmotionlessMethod = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EMOTIONLESS_METHOD>>("api/HisEmotionLessMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (dataEmotionlessMethod != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMOTIONLESS_METHOD), dataEmotionlessMethod, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                dataEmotionlessMethod = dataEmotionlessMethod != null ? dataEmotionlessMethod.Where(p => p.IS_ACTIVE == 1
                  && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbbEmotionlessMethod, dataEmotionlessMethod, controlEditorADO);

                if (this.sereServPTTT != null && dataEmotionlessMethod.Exists(o => o.ID == this.sereServPTTT.EMOTIONLESS_METHOD_ID))
                {
                    cbbEmotionlessMethod.EditValue = this.sereServPTTT.EMOTIONLESS_METHOD_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboBlood()
        {
            try
            {
                //List<HIS_BLOOD_ABO> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>();
                if (BackendDataWorker.IsExistsKey<HIS_BLOOD_ABO>())
                {
                    dataBloodAbo = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataBloodAbo = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_BLOOD_ABO>>("api/HisBloodAbo/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (dataBloodAbo != null) BackendDataWorker.UpdateToRam(typeof(HIS_BLOOD_ABO), dataBloodAbo, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbbBlood, dataBloodAbo, controlEditorADO);
                if (!string.IsNullOrEmpty(this.Patient.BLOOD_ABO_CODE))
                {
                    var bloodAbo = dataBloodAbo.FirstOrDefault(o => o.BLOOD_ABO_CODE == Patient.BLOOD_ABO_CODE);
                    if (bloodAbo != null)
                    {
                        cbbBlood.EditValue = bloodAbo.ID;
                        txtBlood.Text = bloodAbo.BLOOD_ABO_CODE;
                    }
                    else
                    {
                        cbbBlood.EditValue = null;
                        txtBlood.Text = null;
                    }
                }
                else
                {
                    if (this.sereServPTTT != null)
                    {
                        txtBlood.Text = this.sereServPTTT.BLOOD_ABO_CODE;
                        cbbBlood.EditValue = this.sereServPTTT.BLOOD_ABO_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboBloodRh()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<HIS_BLOOD_RH>())
                {
                    dataBloodRh = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    dataBloodRh = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_BLOOD_RH>>("api/HisBloodRh/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (dataBloodRh != null) BackendDataWorker.UpdateToRam(typeof(HIS_BLOOD_RH), dataBloodRh, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbbBloodRh, dataBloodRh, controlEditorADO);

                if (!string.IsNullOrEmpty(this.Patient.BLOOD_RH_CODE))
                {
                    var bloodRh = dataBloodRh.FirstOrDefault(o => o.BLOOD_RH_CODE == Patient.BLOOD_RH_CODE);
                    if (bloodRh != null)
                    {
                        cbbBloodRh.EditValue = bloodRh.ID;
                        txtBloodRh.Text = bloodRh.BLOOD_RH_CODE;
                    }
                    else
                    {
                        cbbBloodRh.EditValue = null;
                        txtBloodRh.Text = null;
                    }
                }
                else
                {
                    if (this.sereServPTTT != null)
                    {
                        txtBloodRh.Text = this.sereServPTTT.BLOOD_RH_CODE;
                        cbbBloodRh.EditValue = this.sereServPTTT.BLOOD_RH_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboPtttCondition()
        {
            try
            {
                // List<HIS_PTTT_CONDITION> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>();
                List<HIS_PTTT_CONDITION> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_CONDITION>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_CONDITION>>("api/HisPtttCondition/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_CONDITION), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_CONDITION_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_CONDITION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_CONDITION_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboCondition, datas, controlEditorADO);
                if (this.sereServPTTT != null)
                {
                    txtCondition.Text = this.sereServPTTT.PTTT_CONDITION_CODE;
                    cboCondition.EditValue = this.sereServPTTT.PTTT_CONDITION_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboCatastrophe()
        {
            try
            {
                //List<HIS_PTTT_CATASTROPHE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>();
                List<HIS_PTTT_CATASTROPHE> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_CATASTROPHE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_CATASTROPHE>>("api/HisPtttCatastrophe/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_CATASTROPHE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_CATASTROPHE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_CATASTROPHE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_CATASTROPHE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboCatastrophe, datas, controlEditorADO);
                if (this.sereServPTTT != null)
                {
                    txtCatastrophe.Text = this.sereServPTTT.PTTT_CATASTROPHE_CODE;
                    cboCatastrophe.EditValue = this.sereServPTTT.PTTT_CATASTROPHE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboDeathWithin()
        {
            try
            {
                List<HIS_DEATH_WITHIN> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_DEATH_WITHIN>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_DEATH_WITHIN>>("api/HisDeathWithin/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_DEATH_WITHIN), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEATH_WITHIN_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEATH_WITHIN_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEATH_WITHIN_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDeathSurg, datas, controlEditorADO);
                if (this.sereServPTTT != null)
                {
                    txtDeathSurg.Text = this.sereServPTTT.DEATH_WITHIN_CODE;
                    cboDeathSurg.EditValue = this.sereServPTTT.DEATH_WITHIN_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataTocboUser()
        {
            try
            {
                this.lstReAcsUserADO = new List<AcsUserADO>();
                var acsUser = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                foreach (var item in acsUser)
                {
                    AcsUserADO ado = new AcsUserADO(item);
                    ado.DOB = (item.DOB ?? 0).ToString();
                    ado.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.DOB ?? 0);
                    ado.DIPLOMA = item.DIPLOMA;
                    ado.DEPARTMENT_CODE = item.DEPARTMENT_CODE;
                    ado.DEPARTMENT_ID = item.DEPARTMENT_ID;
                    ado.DEPARTMENT_NAME = item.DEPARTMENT_NAME;
                    ado.USERNAME = item.TDL_USERNAME;
                    this.lstReAcsUserADO.Add(ado);
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "Họ tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cboEndDeptSubs, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboEndDeptSubs.Properties.ImmediatePopup = true;
                ControlEditorLoader.Load(cboHospSubs, this.lstReAcsUserADO.Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                cboHospSubs.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidHeadDepartmentAndDirectorBranch()
        {

            try
            {
                var RoomV = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Module.RoomId);

                var Department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == RoomV.DEPARTMENT_ID);

                var empHead = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Department.HEAD_LOGINNAME);
                if (empHead != null && empHead.IS_NEED_SIGN_INSTEAD == 1)
                {
                    ValidationSigleControl(cboEndDeptSubs);
                    lciEndDeptSubs.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    dxValidationProvider1.RemoveControlError(cboEndDeptSubs);
                    dxValidationProvider1.SetValidationRule(cboEndDeptSubs, null);
                    lciEndDeptSubs.AppearanceItemCaption.ForeColor = Color.Black;
                }

                var Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(o => o.ID == Department.BRANCH_ID);
                var empDirec = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Branch.DIRECTOR_LOGINNAME);
                if (empDirec != null && empDirec.IS_NEED_SIGN_INSTEAD == 1)
                {
                    ValidationSigleControl(cboHospSubs);
                    lciHospSubs.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    dxValidationProvider1.RemoveControlError(cboHospSubs);
                    dxValidationProvider1.SetValidationRule(cboHospSubs, null);
                    lciHospSubs.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private async Task LoadDataToComboPtttTemp()
        {
            try
            {
                List<HIS_SERE_SERV_PTTT_TEMP> ptttTemp = new List<HIS_SERE_SERV_PTTT_TEMP>();
                Action myaction = () =>
                {
                    var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;
                    string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    ptttTemp = BackendDataWorker.Get<HIS_SERE_SERV_PTTT_TEMP>().Where(o => o.IS_ACTIVE == 1 && (o.IS_PUBLIC == 1 || (o.IS_PUBLIC_IN_DEPARTMENT == 1 && o.DEPARTMENT_ID == DepartmentID) || (o.CREATOR == loginname))).ToList();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERE_SERV_PTTT_TEMP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERE_SERV_PTTT_TEMP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERE_SERV_PTTT_TEMP_NAME", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboPtttTemp, ptttTemp, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void ComboAcsUser(SearchLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }
                else
                {
                    acsUserAlows = data;
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<AcsUserADO> ProcessAcsUser()
        {
            List<AcsUserADO> AcsUserADOList = null;
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                List<V_HIS_EMPLOYEE> employeeList = null;
                CommonParam paramCommon = new CommonParam();
                dynamic filter = new System.Dynamic.ExpandoObject();
                datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                employeeList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>>("api/HisEmployee/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (employeeList != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE), employeeList, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                var departmentList = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1 && o.IS_CLINICAL == 1).ToList();
                AcsUserADOList = new List<AcsUserADO>();

                foreach (var item in datas)
                {
                    AcsUserADO user = new AcsUserADO();
                    user.ID = item.ID;
                    user.LOGINNAME = item.LOGINNAME;
                    user.USERNAME = item.USERNAME;
                    user.MOBILE = item.MOBILE;
                    user.PASSWORD = item.PASSWORD;
                    user.IS_ACTIVE = item.IS_ACTIVE;

                    var check = employeeList.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (check != null)
                    {

                        user.DOB = Inventec.Common.DateTime.Convert.TimeNumberToDateString(check.DOB ?? 0);

                        user.DIPLOMA = check.DIPLOMA;
                        var checkDepartment = departmentList.FirstOrDefault(o => o.ID == check.DEPARTMENT_ID);

                        if (checkDepartment != null)
                        {
                            user.DEPARTMENT_NAME = checkDepartment.DEPARTMENT_NAME;

                        }
                    }
                    AcsUserADOList.Add(user);
                }

                AcsUserADOList = AcsUserADOList.OrderBy(o => o.USERNAME).ToList();
            }
            catch (Exception ex)
            {
                AcsUserADOList = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return AcsUserADOList;
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<AcsUserADO> acsUserAlows = new List<AcsUserADO>();
                if (loginNames != null && loginNames.Count > 0)
                {

                    acsUserAlows = this.AcsUserADOList.Where(o => loginNames.Contains(o.LOGINNAME) && o.IS_ACTIVE == 1).ToList();

                }
                else
                {
                    acsUserAlows = this.AcsUserADOList.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Họ tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

                GridColumn aColumDOB = cbo.Properties.View.Columns.AddField("DOB");
                aColumDOB.Caption = "Ngày sinh";
                aColumDOB.Visible = true;
                aColumDOB.VisibleIndex = 3;
                aColumDOB.Width = 100;

                GridColumn aColumnDIPLOMA = cbo.Properties.View.Columns.AddField("DIPLOMA");
                aColumnDIPLOMA.Caption = "CCHN";
                aColumnDIPLOMA.Visible = true;
                aColumnDIPLOMA.VisibleIndex = 4;
                aColumnDIPLOMA.Width = 100;

                GridColumn aColumnDepartment = cbo.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                aColumnDepartment.Caption = "Tên khoa";
                aColumnDepartment.Visible = true;
                aColumnDepartment.VisibleIndex = 5;
                aColumnDepartment.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public async Task LoadComboPtttTable(GridLookUpEdit cbo)
        {
            try
            {
                List<HIS_PTTT_TABLE> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_TABLE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_TABLE>>("api/HisPtttTable/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_TABLE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;
                if (datas != null && this.Module != null && this.Module.RoomId > 0)
                    datas = datas.Where(o => o.EXECUTE_ROOM_ID == this.Module.RoomId).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_TABLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_TABLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_TABLE_NAME", "ID", columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cbo, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_TABLE_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_TABLE_ID);
                    if (data != null)
                    {
                        txtBanMoCode.Text = data.PTTT_TABLE_CODE;
                        cboBanMo.EditValue = data.ID;
                    }
                    else
                    {
                        txtBanMoCode.Text = "";
                        cboBanMo.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboPhuongPhapThucTe()
        {
            try
            {
                //List<HIS_PTTT_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().Where(p => p.IS_ACTIVE == 1).ToList();
                List<HIS_PTTT_METHOD> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_METHOD>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_METHOD), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboPhuongPhapThucTe, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.REAL_PTTT_METHOD_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.REAL_PTTT_METHOD_ID);
                    if (data != null)
                    {
                        txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                        cboPhuongPhapThucTe.EditValue = data.ID;
                    }
                    else
                    {
                        this.SetDefaultCboPTMethod(this.sereServ, cboPhuongPhapThucTe, txtPhuongPhapTT);
                    }
                }
                else
                {
                    this.SetDefaultCboPTMethod(this.sereServ, cboPhuongPhapThucTe, txtPhuongPhapTT);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboLoaiPT()
        {
            try
            {
                List<HIS_PTTT_PRIORITY> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_PRIORITY>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_PRIORITY>>("api/HisPtttPriority/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_PRIORITY), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_PRIORITY_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_PRIORITY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_PRIORITY_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboLoaiPT, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_PRIORITY_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_PRIORITY_ID);
                    if (data != null)
                    {
                        txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                        cboLoaiPT.EditValue = data.ID;
                    }
                    //else
                    //{
                    //    txtLoaiPT.Text = "";
                    //    cboLoaiPT.EditValue = null;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboPhuongPhap2()
        {
            try
            {
                List<HIS_EMOTIONLESS_METHOD> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_EMOTIONLESS_METHOD>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EMOTIONLESS_METHOD>>("api/HisEmotionLessMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMOTIONLESS_METHOD), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1
                    && (p.IS_SECOND == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboPhuongPhap2, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID);
                    if (data != null)
                    {
                        txtPhuongPhap2.Text = data.EMOTIONLESS_METHOD_CODE;
                        cboPhuongPhap2.EditValue = data.ID;
                    }
                    else
                    {
                        txtPhuongPhap2.Text = "";
                        cboPhuongPhap2.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboKQVoCam()
        {
            try
            {
                List<HIS_EMOTIONLESS_RESULT> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_EMOTIONLESS_RESULT>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EMOTIONLESS_RESULT>>("api/HisEmotionLessResult/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMOTIONLESS_RESULT), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_RESULT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_RESULT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_RESULT_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboKQVoCam, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.EMOTIONLESS_RESULT_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_RESULT_ID);
                    if (data != null)
                    {
                        txtKQVoCam.Text = data.EMOTIONLESS_RESULT_CODE;
                        cboKQVoCam.EditValue = data.ID;
                    }
                    else
                    {
                        txtKQVoCam.Text = "";
                        cboKQVoCam.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ComboMoKTCao()
        {
            try
            {
                List<HIS_PTTT_HIGH_TECH> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_HIGH_TECH>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_HIGH_TECH>>("api/HisPtttHighTech/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_HIGH_TECH), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_HIGH_TECH_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_HIGH_TECH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_HIGH_TECH_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboMoKTCao, datas, controlEditorADO);

                if (datas != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_HIGH_TECH_ID > 0)
                {
                    var data = datas.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_HIGH_TECH_ID);
                    if (data != null)
                    {
                        txtMoKTCao.Text = data.PTTT_HIGH_TECH_CODE;
                        cboMoKTCao.EditValue = data.ID;
                    }
                    else
                    {
                        txtMoKTCao.Text = "";
                        cboMoKTCao.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //#16831
        private async Task ComboHisMachine()
        {
            try
            {
                List<HIS_MACHINE> datas = null;
                if (BackendDataWorker.IsExistsKey<HIS_MACHINE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_MACHINE>>("api/HisMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(HIS_MACHINE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(p => p.IS_ACTIVE == 1 && ("," +p.ROOM_IDS + ",").Contains("," + this.Module.RoomId + ",")).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(this.cboMachine, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
