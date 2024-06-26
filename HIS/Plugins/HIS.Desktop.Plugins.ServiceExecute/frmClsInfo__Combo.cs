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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.EkipTemp;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmClsInfo : Form
    {
        private async Task ComboPTTTGroup()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_GROUP>())
                {
                    datasPtttGroup = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_GROUP>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttGroup = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_GROUP>>("api/HisPtttGroup/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttGroup != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_GROUP), datasPtttGroup, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbbPtttGroup, datasPtttGroup, controlEditorADO);
                this.SetDefaultCboPTTTGroupOnly();
                if (this.sereServPTTT != null && this.sereServPTTT.PTTT_GROUP_ID != null)
                {
                    var ptttGroup = datasPtttGroup != null ? datasPtttGroup.Where(o => o.ID == this.sereServPTTT.PTTT_GROUP_ID).FirstOrDefault() : null;
                    txtPtttGroupCode.Text = ptttGroup != null ? ptttGroup.PTTT_GROUP_CODE : "";
                    cbbPtttGroup.EditValue = this.sereServPTTT.PTTT_GROUP_ID;
                    cbbPtttGroup.Properties.Buttons[1].Visible = true;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cbbPtttGroup.EditValue), cbbPtttGroup.EditValue));
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
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_METHOD>())
                {
                    datasPtttMethod = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttMethod = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttMethod != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_METHOD), datasPtttMethod, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboMethod, datasPtttMethod, controlEditorADO);

                if (this.sereServPTTT != null && this.sereServPTTT.PTTT_METHOD_ID > 0)
                {
                    var method = datasPtttMethod != null ? datasPtttMethod.Where(o => o.ID == this.sereServPTTT.PTTT_METHOD_ID).FirstOrDefault() : null;
                    txtMethodCode.Text = method != null ? method.PTTT_METHOD_CODE : "";
                    cboMethod.EditValue = this.sereServPTTT.PTTT_METHOD_ID;
                }
                else
                    this.SetDefaultCboPTTTMethod(currentServiceADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboEmotionlessMothod()
        {
            try
            {
                //List<HIS_EMOTIONLESS_METHOD> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().Where(p => p.IS_ACTIVE == 1
                //   && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList();

                datasEmotionLessMethod = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();

                datasEmotionLessMethod = datasEmotionLessMethod != null ? datasEmotionLessMethod.Where(p => p.IS_ACTIVE == 1
                  && (p.IS_FIRST == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbbEmotionlessMethod, datasEmotionLessMethod, controlEditorADO);

                if (this.sereServPTTT != null)
                {
                    var emoless = this.sereServPTTT.EMOTIONLESS_METHOD_ID > 0 ? datasEmotionLessMethod.Where(o => o.ID == this.sereServPTTT.EMOTIONLESS_METHOD_ID).First() : null;
                    txtEmotionlessMethod.Text = emoless != null ? emoless.EMOTIONLESS_METHOD_CODE : "";
                    cbbEmotionlessMethod.EditValue = emoless != null ? (long?)emoless.ID : null;
                    cbbEmotionlessMethod.Properties.Buttons[1].Visible = this.sereServPTTT.EMOTIONLESS_METHOD_ID > 0;
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
                    datasBloodABO = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_ABO>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasBloodABO = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_BLOOD_ABO>>("api/HisBloodAbo/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasBloodABO != null) BackendDataWorker.UpdateToRam(typeof(HIS_BLOOD_ABO), datasBloodABO, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_ABO_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_ABO_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbbBlood, datasBloodABO, controlEditorADO);
                if (this.sereServPTTT != null && this.sereServPTTT.BLOOD_ABO_ID > 0)
                {
                    var blood = datasBloodABO != null ? datasBloodABO.Where(o => o.ID == this.sereServPTTT.BLOOD_ABO_ID).FirstOrDefault() : null;
                    txtBlood.Text = blood != null ? blood.BLOOD_ABO_CODE : "";
                    cbbBlood.EditValue = this.sereServPTTT.BLOOD_ABO_ID;
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
                    datasBloodRh = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BLOOD_RH>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasBloodRh = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_BLOOD_RH>>("api/HisBloodRh/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasBloodRh != null) BackendDataWorker.UpdateToRam(typeof(HIS_BLOOD_RH), datasBloodRh, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_RH_CODE", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbbBloodRh, datasBloodRh, controlEditorADO);
                if (this.sereServPTTT != null && this.sereServPTTT.BLOOD_RH_ID > 0)
                {
                    var bloodRh = datasBloodRh != null ? datasBloodRh.Where(o => o.ID == this.sereServPTTT.BLOOD_RH_ID).FirstOrDefault() : null;
                    txtBloodRh.Text = bloodRh != null ? bloodRh.BLOOD_RH_CODE : "";
                    cbbBloodRh.EditValue = this.sereServPTTT.BLOOD_RH_ID;
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
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_CONDITION>())
                {
                    datasPtttCondition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CONDITION>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttCondition = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_CONDITION>>("api/HisPtttCondition/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttCondition != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_CONDITION), datasPtttCondition, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_CONDITION_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_CONDITION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_CONDITION_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboCondition, datasPtttCondition, controlEditorADO);
                if (this.sereServPTTT != null && this.sereServPTTT.PTTT_CONDITION_ID > 0)
                {
                    var condition = datasPtttCondition != null ? datasPtttCondition.Where(o => o.ID == this.sereServPTTT.PTTT_CONDITION_ID).FirstOrDefault() : null;
                    txtCondition.Text = condition != null ? condition.PTTT_CONDITION_CODE : "";
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
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_CATASTROPHE>())
                {
                    datasPtttCatastrophe = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_CATASTROPHE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttCatastrophe = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_CATASTROPHE>>("api/HisPtttCatastrophe/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttCatastrophe != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_CATASTROPHE), datasPtttCatastrophe, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_CATASTROPHE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_CATASTROPHE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_CATASTROPHE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboCatastrophe, datasPtttCatastrophe, controlEditorADO);
                if (this.sereServPTTT != null && this.sereServPTTT.PTTT_CATASTROPHE_ID > 0)
                {
                    var catastrophe = datasPtttCatastrophe != null ? datasPtttCatastrophe.Where(o => o.ID == this.sereServPTTT.PTTT_CATASTROPHE_ID).FirstOrDefault() : null;
                    txtCatastrophe.Text = catastrophe != null ? catastrophe.PTTT_CATASTROPHE_CODE : "";
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
                if (BackendDataWorker.IsExistsKey<HIS_DEATH_WITHIN>())
                {
                    datasDeathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasDeathWithin = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_DEATH_WITHIN>>("api/HisDeathWithin/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasDeathWithin != null) BackendDataWorker.UpdateToRam(typeof(HIS_DEATH_WITHIN), datasDeathWithin, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEATH_WITHIN_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEATH_WITHIN_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEATH_WITHIN_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDeathSurg, datasDeathWithin, controlEditorADO);
                if (this.sereServPTTT != null && this.sereServPTTT.DEATH_WITHIN_ID > 0)
                {
                    var deathSurg = datasDeathWithin != null ? datasDeathWithin.Where(o => o.ID == this.sereServPTTT.DEATH_WITHIN_ID).FirstOrDefault() : null;
                    txtDeathSurg.Text = deathSurg != null ? deathSurg.DEATH_WITHIN_CODE : "";
                    cboDeathSurg.EditValue = this.sereServPTTT.DEATH_WITHIN_ID;
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
                if (BackendDataWorker.IsExistsKey<HIS_EMOTIONLESS_METHOD>())
                {
                    datasEmotionlessMethod2 = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasEmotionlessMethod2 = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EMOTIONLESS_METHOD>>("api/HisEmotionLessMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasEmotionlessMethod2 != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMOTIONLESS_METHOD), datasEmotionlessMethod2, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datasEmotionlessMethod2 = datasEmotionlessMethod2 != null ? datasEmotionlessMethod2.Where(p => p.IS_ACTIVE == 1
                    && (p.IS_SECOND == 1 || (p.IS_FIRST != 1 && p.IS_SECOND != 1))).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboPhuongPhap2, datasEmotionlessMethod2, controlEditorADO);

                if (datasEmotionlessMethod2 != null && this.sereServPTTT != null && this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID > 0)
                {
                    var data = datasEmotionlessMethod2.FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_METHOD_SECOND_ID);
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
                if (BackendDataWorker.IsExistsKey<HIS_EMOTIONLESS_RESULT>())
                {
                    datasEmotionlessResult = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_RESULT>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasEmotionlessResult = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_EMOTIONLESS_RESULT>>("api/HisEmotionLessResult/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasEmotionlessResult != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMOTIONLESS_RESULT), datasEmotionlessResult, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datasEmotionlessResult = datasEmotionlessResult != null ? datasEmotionlessResult.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_RESULT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EMOTIONLESS_RESULT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMOTIONLESS_RESULT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboKQVoCam, datasEmotionlessResult, controlEditorADO);

                if (datasEmotionlessResult != null && this.sereServPTTT != null && this.sereServPTTT.EMOTIONLESS_RESULT_ID > 0)
                {
                    var data = datasEmotionlessResult.FirstOrDefault(p => p.ID == this.sereServPTTT.EMOTIONLESS_RESULT_ID);
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
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_HIGH_TECH>())
                {
                    datasPtttHighTech = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_HIGH_TECH>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttHighTech = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_HIGH_TECH>>("api/HisPtttHighTech/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttHighTech != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_HIGH_TECH), datasPtttHighTech, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datasPtttHighTech = datasPtttHighTech != null ? datasPtttHighTech.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_HIGH_TECH_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_HIGH_TECH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_HIGH_TECH_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboMoKTCao, datasPtttHighTech, controlEditorADO);

                if (datasPtttHighTech != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_HIGH_TECH_ID > 0)
                {
                    var data = datasPtttHighTech.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_HIGH_TECH_ID);
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
                if (BackendDataWorker.IsExistsKey<HIS_MACHINE>())
                {
                    datasMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasMachine = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_MACHINE>>("api/HisMachine/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasMachine != null) BackendDataWorker.UpdateToRam(typeof(HIS_MACHINE), datasMachine, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var workingRoomIds = WorkPlace.GetRoomIds();
                if (datasMachine != null && datasMachine.Count > 0)
                    datasMachine =
                        (from m in datasMachine
                         from n in workingRoomIds
                         where m.IS_ACTIVE == 1 && m.ROOM_IDS != null && m.ROOM_IDS.Contains(n.ToString())
                         select m).Distinct().ToList();

                var ListServiceMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_MACHINE>();
                if (ListServiceMachine != null && ListServiceMachine.Count > 0)
                    ListServiceMachine = ListServiceMachine.Where(o => o.IS_ACTIVE == 1).ToList();

                var currentServiceMachine = ListServiceMachine.Where(o => o.SERVICE_ID == currentServiceADO.SERVICE_ID).Select(o => o.MACHINE_ID).ToList();
                datasMachine = (currentServiceMachine != null && currentServiceMachine.Count > 0) ? datasMachine.Where(o => currentServiceMachine.Contains(o.ID)).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboMachine, datasMachine, controlEditorADO);

                if (this.sereServExt != null && this.sereServExt.MACHINE_ID > 0 && datasMachine != null && datasMachine.Count > 0)
                {
                    var data = datasMachine.FirstOrDefault(p => p.ID == this.sereServExt.MACHINE_ID);
                    if (data != null)
                    {
                        txtMachineCode.Text = data.MACHINE_CODE;
                        cboMachine.EditValue = this.sereServExt.MACHINE_ID;
                    }
                }
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
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_TABLE>())
                {
                    datasPtttTable = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_TABLE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttTable = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_TABLE>>("api/HisPtttTable/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttTable != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_TABLE), datasPtttTable, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datasPtttTable = datasPtttTable != null ? datasPtttTable.Where(p => p.IS_ACTIVE == 1).ToList() : null;
                if (datasPtttTable != null && this.Module != null && this.Module.RoomId > 0)
                    datasPtttTable = datasPtttTable.Where(o => o.EXECUTE_ROOM_ID == this.Module.RoomId).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_TABLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_TABLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_TABLE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(cbo, datasPtttTable, controlEditorADO);

                if (datasPtttTable != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_TABLE_ID > 0)
                {
                    var data = datasPtttTable.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_TABLE_ID);
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
                if (BackendDataWorker.IsExistsKey<HIS_PTTT_METHOD>())
                {
                    datasPtttMethod = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasPtttMethod = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datasPtttMethod != null) BackendDataWorker.UpdateToRam(typeof(HIS_PTTT_METHOD), datasPtttMethod, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var datasPttt = datasPtttMethod != null ? datasPtttMethod.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboPhuongPhapThucTe, datasPttt, controlEditorADO);

                if (datasPttt != null && this.sereServPTTT != null && this.sereServPTTT.REAL_PTTT_METHOD_ID > 0)
                {
                    var data = datasPtttMethod.FirstOrDefault(p => p.ID == this.sereServPTTT.REAL_PTTT_METHOD_ID);
                    if (data != null)
                    {
                        txtPhuongPhapTT.Text = data.PTTT_METHOD_CODE;
                        cboPhuongPhapThucTe.EditValue = data.ID;
                    }
                    else
                    {
                        txtPhuongPhapTT.Text = "";
                        cboPhuongPhapThucTe.EditValue = null;
                    }
                }
                else
                    this.SetDefaultCboPPThucTeMethod(currentServiceADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultCboPPThucTeMethod(ServiceADO sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    if (sereServ.EKIP_ID == null)
                    {
                        long ptttMethodId = 0;

                        var surgMisuService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                        if (surgMisuService != null)
                        {
                            if (surgMisuService.PTTT_METHOD_ID.HasValue)
                            {
                                HIS_PTTT_METHOD ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_METHOD_ID);
                                ptttMethodId = ptttMethod.ID;
                                txtPhuongPhapTT.Text = ptttMethod.PTTT_METHOD_CODE;
                            }
                        }

                        if (ptttMethodId > 0)
                        {
                            cboPhuongPhapThucTe.EditValue = ptttMethodId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultCboPTTTMethod(ServiceADO sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    if (sereServ.EKIP_ID == null)
                    {
                        long ptttMethodId = 0;

                        var surgMisuService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT));
                        if (surgMisuService != null)
                        {
                            if (surgMisuService.PTTT_METHOD_ID.HasValue)
                            {
                                HIS_PTTT_METHOD ptttMethod = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == surgMisuService.PTTT_METHOD_ID);
                                ptttMethodId = ptttMethod.ID;
                                txtMethodCode.Text = ptttMethod.PTTT_METHOD_CODE;
                            }
                        }

                        if (ptttMethodId > 0)
                        {
                            cboMethod.EditValue = ptttMethodId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboLoaiPT()
        {
            try
            {
                 datasPtttPriority = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_PRIORITY>();

                datasPtttPriority = datasPtttPriority != null ? datasPtttPriority.Where(p => p.IS_ACTIVE == 1).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_PRIORITY_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PTTT_PRIORITY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_PRIORITY_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboLoaiPT, datasPtttPriority, controlEditorADO);

                if (datasPtttPriority != null && this.sereServPTTT != null && this.sereServPTTT.PTTT_PRIORITY_ID > 0)
                {
                    var data = datasPtttPriority.FirstOrDefault(p => p.ID == this.sereServPTTT.PTTT_PRIORITY_ID);
                    if (data != null)
                    {
                        txtLoaiPT.Text = data.PTTT_PRIORITY_CODE;
                        cboLoaiPT.EditValue = data.ID;
                        cboLoaiPT.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        txtLoaiPT.Text = "";
                        cboLoaiPT.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
