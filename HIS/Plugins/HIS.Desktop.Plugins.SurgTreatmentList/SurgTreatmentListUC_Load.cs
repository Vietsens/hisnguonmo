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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgTreatmentList
{
    public partial class SurgTreatmentListUC : HIS.Desktop.Utility.UserControlBase
    {
        public void Search()
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SurgTreatmentList.Resources.Lang", typeof(HIS.Desktop.Plugins.SurgTreatmentList.SurgTreatmentListUC).Assembly);

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BarIntructionTime.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.BarIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciIntructionTimeFrom.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.LciIntructionTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciIntructionTimeTo.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.LciIntructionTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkInTreat.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkInTreat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkOutTreat.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkOutTreat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkSA.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkSA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkNS.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkNS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkCDHA.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkCDHA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkCDHA.ToolTip = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkCDHA.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkTT.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkTT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkPT.Properties.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.ChkPT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BarTreatmentType.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.BarTreatmentType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BarServiceType.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.BarServiceType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnRefresh.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.BtnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnSearch.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.BtnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcStt.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcStt.ToolTip = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcStt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcGatherData.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcGatherData.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcFee.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcFee.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPatientName.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPtttMethodCode.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPtttMethodCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPtttMethodCode.ToolTip = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPtttMethodCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPtttGroupName.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPtttGroupName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPtttGroupName.ToolTip = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPtttGroupName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcEndTime.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcEndTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcServiceCode.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcServiceName.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.TxtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.TxtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcEmotionlessMethodName.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcEmotionlessMethodName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPtttPriority.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPtttPriority.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcPtttPriority.ToolTip = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcPtttPriority.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GvSS_GcExecuteLoginname.Caption = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.GvSS_GcExecuteLoginname.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboExecuteRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.CboExecuteRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciExecuteRoom.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.LciExecuteRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIs_Gather_Data.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.lcIs_Fee.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcIs_Fee.Text = Inventec.Common.Resource.Get.Value("SurgTreatmentListUC.lcIs_Gather_Data.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                TxtKeyword.Text = null;
                DtIntructionTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                DtIntructionTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                ChkInTreat.Checked = false;
                ChkOutTreat.Checked = false;
                ChkPT.Checked = false;
                ChkTT.Checked = false;
                ChkCDHA.Checked = false;
                ChkNS.Checked = false;
                ChkSA.Checked = false;
                CboExecuteRoom.EditValue = null;
                GridCheckMarksSelection gridCheckMark = CboPtttPriorityName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(CboPtttPriorityName.Properties.View);
                }
                CboPtttPriorityName.Focus();
                TxtKeyword.Focus();
                //CboPtttPriorityName.EditValue = null;
                //ListPtttPriority = new List<HIS_PTTT_PRIORITY>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCotrol()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pagingSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = (int)ConfigApplications.NumPageSize;
                }

                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.GridControlSereServ);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_8>> apiResult = null;
                MOS.Filter.HisSereServView8Filter filter = new MOS.Filter.HisSereServView8Filter();
                SetFilter(ref filter);
                GridViewSereServ.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_8>>
                    ("api/HisSereServ/GetView8", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter___:", filter));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult.Data___:", apiResult.Data));
                if (apiResult != null)
                {
                    //listData = (from m in apiResult.Data select new ADO.SereServADO(m)).ToList();


                    listData = ProcessSereServADO(apiResult.Data);

                    if (listData != null && listData.Count > 0)
                    {
                        GridControlSereServ.DataSource = listData;
                        rowCount = (listData == null ? 0 : listData.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        GridControlSereServ.DataSource = null;
                        rowCount = (listData == null ? 0 : listData.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                WaitingManager.Hide();
                GridViewSereServ.EndUpdate();
                GridViewSereServ.ExpandAllGroups();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                GridViewSereServ.EndUpdate();
                GridViewSereServ.ExpandAllGroups();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisSereServView8Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = TxtKeyword.Text.Trim();

                //filter.TDL_HEIN_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT;
                filter.HAS_SERVICE_PTTT_GROUP_ID = true;

                if (!string.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    filter.SERVICE_REQ_CODE__EXACT = code;
                }

                if (DtIntructionTimeFrom.EditValue != null && DtIntructionTimeFrom.DateTime != DateTime.MinValue)
                    filter.END_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(DtIntructionTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");

                if (DtIntructionTimeTo.EditValue != null && DtIntructionTimeTo.DateTime != DateTime.MinValue)
                    filter.END_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(DtIntructionTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");

                if (ChkInTreat.Checked)
                {
                    filter.REQ_SURG_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
                }
                else if (ChkOutTreat.Checked)
                {
                    filter.REQ_SURG_TREATMENT_TYPE_IDs = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU,
                        IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY,
                        IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                    };
                }

                var serviceType = new List<long>();
                if (ChkPT.Checked)
                {
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                }
                if (ChkTT.Checked)
                {
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT);
                }
                if (ChkCDHA.Checked)
                {
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA);
                }
                if (ChkNS.Checked)
                {
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS);
                }
                if (ChkSA.Checked)
                {
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA);
                }
                if (chkTDCN.Checked)
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN);
                if (chkGPBL.Checked)
                    serviceType.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL);

                if (serviceType != null && serviceType.Count > 0)
                {
                    filter.SERVICE_TYPE_IDs = serviceType;
                }
                else
                {
                    filter.SERVICE_TYPE_IDs = ClsServiceType;
                }

                if (ListPtttPriority != null && ListPtttPriority.Count > 0)
                {
                    filter.PTTT_PRIORITY_IDs = ListPtttPriority.Select(s => s.ID).ToList();
                }
                if (SelectedFees != null && SelectedFees.Count == 1)
                {
                    if (SelectedFees[0].Value == 1)//1 có, 2 là ko
                    {
                        filter.IS_FEE = true;
                    }
                    else
                        filter.IS_FEE = false;
                }
                if (SelectedGatherdatas != null && SelectedGatherdatas.Count == 1)
                {
                    if (SelectedGatherdatas[0].Value == 1)//1 có, 2 là ko
                    {
                        filter.IS_GATHER_DATA = true;
                    }
                    else
                        filter.IS_GATHER_DATA = false;
                }
                if (CboExecuteRoom.EditValue != null)
                {
                    filter.TDL_EXECUTE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(CboExecuteRoom.EditValue.ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<ADO.SereServADO> ProcessSereServADO(List<V_HIS_SERE_SERV_8> list)
        {
            List<ADO.SereServADO> result = null;
            try
            {
                if (list != null && list.Count > 0)
                {
                    result = new List<ADO.SereServADO>();
                    var ekipIds = list.Select(s => s.EKIP_ID ?? 0).Distinct().ToList();
                    if (ekipIds != null && ekipIds.Count > 0)//lớn hơn 1 vì luôn có 1 giá trị bằng 0
                    {
                        var lstEkip = GetListEkip(ekipIds);

                        foreach (var item in list)
                        {
                            ADO.SereServADO ado = new ADO.SereServADO(item);

                            if (item.EKIP_ID.HasValue)
                            {
                                var ekip = lstEkip.Where(o => o.EKIP_ID == item.EKIP_ID.Value).ToList();
                                if (ekip != null && ekip.Count > 0)
                                {
                                    ProcessRoleSereServADO(ado, ekip);
                                }
                            }

                            result.Add(ado);
                        }
                    }
                    else
                    {
                        result = (from m in list select new ADO.SereServADO(m)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessRoleSereServADO(ADO.SereServADO ado, List<HIS_EKIP_USER> ekip)
        {
            try
            {
                if (ado == null) return;

                if (ekip == null) return;

                foreach (var item in ekip)
                {
                    if (!DicMapData.ContainsKey(item.EXECUTE_ROLE_ID)) continue;

                    switch (DicMapData[item.EXECUTE_ROLE_ID])
                    {
                        case 1:
                            ado.EXECUTE_ROLE_NAME_1 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_1 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 2:
                            ado.EXECUTE_ROLE_NAME_2 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_2 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 3:
                            ado.EXECUTE_ROLE_NAME_3 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_3 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 4:
                            ado.EXECUTE_ROLE_NAME_4 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_4 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 5:
                            ado.EXECUTE_ROLE_NAME_5 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_5 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 6:
                            ado.EXECUTE_ROLE_NAME_6 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_6 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 7:
                            ado.EXECUTE_ROLE_NAME_7 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_7 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 8:
                            ado.EXECUTE_ROLE_NAME_8 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_8 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 9:
                            ado.EXECUTE_ROLE_NAME_9 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_9 = item.USERNAME + " - " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        case 10:
                            ado.EXECUTE_ROLE_NAME_10 = item.USERNAME.Split(' ').Last();
                            ado.REMUNERATION_PRICE_10 = item.USERNAME + "-  " + Inventec.Common.Number.Convert.NumberToString(item.REMUNERATION_PRICE ?? 0, ConfigApplications.NumberSeperator);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_EKIP_USER> GetListEkip(List<long> ekipIds)
        {
            List<HIS_EKIP_USER> result = new List<HIS_EKIP_USER>();
            try
            {
                if (ekipIds != null && ekipIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisEkipUserFilter filter = new HisEkipUserFilter();
                    filter.EKIP_IDs = ekipIds;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void UpdateRowData(ADO.SereServADO row, bool isFee)
        {
            try
            {
                if (row != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    //goi api update IS_NOT_GATHER_DATA, IS_NOT_FEE
                    if (isFee)
                    {
                        HisSereServExtIsFeeSDO sdo = new HisSereServExtIsFeeSDO();
                        sdo.SereServId = row.ID;
                        sdo.WorkingRoomId = this.moduleData.RoomId;
                        sdo.IsFee = row.Fee;//MouseDown chua cap nhat datasuorce nen se gui value khac.
                        var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_EXT>("api/HisSereServExt/SetIsFee", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiResult != null)
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        HisSereServExtIsGatherDataSDO sdo = new HisSereServExtIsGatherDataSDO();
                        sdo.SereServId = row.ID;
                        sdo.WorkingRoomId = this.moduleData.RoomId;
                        sdo.IsGatherData = row.GatherData;//MouseDown chua cap nhat datasuorce nen se gui value khac.
                        var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERE_SERV_EXT>("api/HisSereServExt/SetIsGatherData", ApiConsumer.ApiConsumers.MosConsumer, sdo, param);
                        if (apiResult != null)
                        {
                            success = true;
                        }
                    }

                    MessageManager.Show(this.ParentForm, param, success);

                    //thanh cong
                    if (success)
                    {
                        CommonParam paramCommon = new CommonParam();
                        MOS.Filter.HisSereServView8Filter filter = new MOS.Filter.HisSereServView8Filter();
                        filter.ID = row.ID;
                        var apiResult = new Inventec.Common.Adapter.BackendAdapter
                             (paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_8>>
                             ("api/HisSereServ/GetView8", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            foreach (var item in apiResult)
                            {
                                var ado = this.listData.FirstOrDefault(o => o.ID == item.ID);
                                if (ado != null)
                                {
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.SereServADO>(ado, item);
                                    ado.Fee = item.IS_FEE == 1;
                                    ado.GatherData = item.IS_GATHER_DATA == 1;
                                    ado.END_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.END_TIME ?? 0);
                                    if (item.EKIP_ID.HasValue)
                                    {
                                        var lstEkip = GetListEkip(new List<long>() { item.EKIP_ID.Value });
                                        ProcessRoleSereServADO(ado, lstEkip);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboExecuteRoom()
        {
            try
            {
                var surgRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.IS_SURGERY == 1).ToList();
                CboExecuteRoom.Properties.DataSource = BackendDataWorker.Get<HIS_EXECUTE_ROOM>();
                CboExecuteRoom.Properties.DisplayMember = "EXECUTE_ROOM_NAME";
                CboExecuteRoom.Properties.ValueMember = "ROOM_ID";
                CboExecuteRoom.Properties.TextEditStyle = TextEditStyles.Standard;
                CboExecuteRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                CboExecuteRoom.Properties.ImmediatePopup = true;
                CboExecuteRoom.ForceInitialize();
                CboExecuteRoom.Properties.View.Columns.Clear();
                CboExecuteRoom.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = CboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = CboExecuteRoom.Properties.View.Columns.AddField("EXECUTE_ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFormatColumn()
        {
            try
            {
                string formatStr = "#,##0";
                if (ConfigApplications.NumberSeperator > 0)
                {
                    formatStr += ".";
                    for (int i = 0; i < ConfigApplications.NumberSeperator; i++)
                    {
                        formatStr += "0";
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitPtttPriorityCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(CboPtttPriorityName.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__PtttGroupName);
                CboPtttPriorityName.Properties.Tag = gridCheck;
                CboPtttPriorityName.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = CboPtttPriorityName.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(CboPtttPriorityName.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PtttGroupName(object sender, EventArgs e)
        {
            try
            {
                ListPtttPriority = new List<HIS_PTTT_PRIORITY>();
                foreach (MOS.EFMODEL.DataModels.HIS_PTTT_PRIORITY rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        ListPtttPriority.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPtttPriority()
        {
            try
            {
                var datas = BackendDataWorker.Get<HIS_PTTT_PRIORITY>();
                if (datas != null)
                {
                    CboPtttPriorityName.Properties.DataSource = datas;
                    CboPtttPriorityName.Properties.DisplayMember = "PTTT_PRIORITY_NAME";
                    CboPtttPriorityName.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = CboPtttPriorityName.Properties.View.Columns.AddField("PTTT_PRIORITY_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    CboPtttPriorityName.Properties.PopupFormWidth = 200;
                    CboPtttPriorityName.Properties.View.OptionsView.ShowColumnHeaders = false;
                    CboPtttPriorityName.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = CboPtttPriorityName.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(CboPtttPriorityName.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCheck(DevExpress.XtraEditors.GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<ADO.SearchADO> data = new List<ADO.SearchADO>();
                data.Add(new ADO.SearchADO() { Display = "Có", Value = 1 });
                data.Add(new ADO.SearchADO() { Display = "Không", Value = 2 });
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "Display";
                cbo.Properties.ValueMember = "Value";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("Display");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessColumnRole()
        {
            try
            {
                Dictionary<int, HIS_EXECUTE_ROLE> DicRole = Config.HisExecuteRoleCFG.ProcessDicExecuteRole();
                if (DicRole != null)
                {
                    if (DicRole.ContainsKey(1))
                    {
                        GvSS_GcExecuteRoleName1.Caption = DicRole[1].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[1].ID, 1);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName1.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(2))
                    {
                        GvSS_GcExecuteRoleName2.Caption = DicRole[2].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[2].ID, 2);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName2.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(3))
                    {
                        GvSS_GcExecuteRoleName3.Caption = DicRole[3].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[3].ID, 3);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName3.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(4))
                    {
                        GvSS_GcExecuteRoleName4.Caption = DicRole[4].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[4].ID, 4);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName4.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(5))
                    {
                        GvSS_GcExecuteRoleName5.Caption = DicRole[5].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[5].ID, 5);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName5.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(6))
                    {
                        GvSS_GcExecuteRoleName6.Caption = DicRole[6].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[6].ID, 6);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName6.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(7))
                    {
                        GvSS_GcExecuteRoleName7.Caption = DicRole[7].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[7].ID, 7);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName7.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(8))
                    {
                        GvSS_GcExecuteRoleName8.Caption = DicRole[8].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[8].ID, 8);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName8.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(9))
                    {
                        GvSS_GcExecuteRoleName9.Caption = DicRole[9].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[9].ID, 9);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName9.VisibleIndex = -1;
                    }

                    if (DicRole.ContainsKey(10))
                    {
                        GvSS_GcExecuteRoleName10.Caption = DicRole[10].EXECUTE_ROLE_NAME;
                        DicMapData.Add(DicRole[10].ID, 10);
                    }
                    else
                    {
                        GvSS_GcExecuteRoleName10.VisibleIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
