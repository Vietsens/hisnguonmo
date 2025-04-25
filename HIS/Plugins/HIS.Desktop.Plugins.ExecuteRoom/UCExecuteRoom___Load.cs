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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7V2;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.ADO;
using System.Reflection;
using HIS.Desktop.Plugins.ExecuteRoom.Base;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.LocalStorage.BackendData;
using System.IO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.ExecuteRoom.Resources;
using HIS.Desktop.Plugins.ExecuteRoom.ADO;
using EMR.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.Utils;
using EMR.SDO;
using Inventec.Common.QrCodeBHYT;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
//using static Aspose.Pdf.Operator;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom : UserControlBase
    {
        V_HIS_SERE_SERV_7 TreeClickData;

        private bool ValidateFindControl()
        {
            bool result = true;
            try
            {
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate)
                {
                    if (String.IsNullOrWhiteSpace(txtServiceReqCode.Text) && (dtIntructionDate.EditValue == null || dtIntructionDateTo.EditValue == null))
                    {
                        MessageBox.Show("Bạn cần nhập đầy đủ thời gian trước khi thực hiện tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                        {
                            txtServiceReqCode.Focus();
                            txtServiceReqCode.SelectAll();
                        }

                        if (dtIntructionDate.EditValue == null)
                        {
                            dtIntructionDate.Focus();
                        }
                        else if (dtIntructionDateTo.EditValue == null)
                        {
                            dtIntructionDateTo.Focus();
                        }

                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal void FillDataToGridControl()
        {
            if (ucPaging1.pagingGrid != null)
            {
                numPageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE__EXECUTE_ROOM); //(int)ConfigApplications.NumPageSize;
                if (numPageSize == 0)
                    numPageSize = ConfigApplicationWorker.Get<int>(AppConfigKey.CONFIG_KEY__NUM_PAGESIZE);
            }

            FillDataToGridServiceReq(new CommonParam(0, (int)numPageSize));
            CreateThreadCallPatientRefresh();
            CPALoad = true;
            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridServiceReq, param, numPageSize, gridControlServiceReq);


        }

        internal void FillDataToGridServiceReq(object param)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridViewServiceReq);
                //transitionManager1.StartTransition(layoutControl2);
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<L_HIS_SERVICE_REQ>> apiResult = new ApiResultObject<List<L_HIS_SERVICE_REQ>>();
                MOS.Filter.HisServiceReqLViewFilter hisServiceReqFilter = new HisServiceReqLViewFilter();

                if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    hisServiceReqFilter.SERVICE_REQ_CODE__EXACT = code;
                }

                //hisServiceReqFilter.HAS_NO_WITHDRAW = true; can check lai
                hisServiceReqFilter.EXECUTE_ROOM_ID = roomId;
                hisServiceReqFilter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};
                hisServiceReqFilter.HAS_EXECUTE = true;

                if (selectedPatientTypeList != null && selectedPatientTypeList.Count > 0)
                {
                    hisServiceReqFilter.TDL_PATIENT_TYPE_IDs = selectedPatientTypeList.Select(o => o.ID).Distinct().ToList();
                }

                if (!string.IsNullOrEmpty(txtBedCodeBedName.Text))
                {
                    hisServiceReqFilter.BED_CODE__BED_NAME = txtBedCodeBedName.Text.Trim();

                }

                hisServiceReqFilter.IS_NOT_KSK_REQURIED_APROVAL__OR__IS_KSK_APPROVE = true;
                hisServiceReqFilter.KEYWORD__SERVICE_REQ_CODE__TREATMENT_CODE__PATIENT_NAME__PATIENT_CODE = txtSearchKey.Text.Trim();
                //if (dtCreatefrom.EditValue != null && dtCreatefrom.DateTime != DateTime.MinValue)
                //    hisServiceReqFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreatefrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                //if (dtCreateTo.EditValue != null && dtCreateTo.DateTime != DateTime.MinValue)
                //    hisServiceReqFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMddHHmm") + "59");

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                   && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue)
                {
                    hisServiceReqFilter.INTRUCTION_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                    && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue)
                {
                    hisServiceReqFilter.VIR_INTRUCTION_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMM") + "00000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind_RangeDate
                    && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue
                    && dtIntructionDateTo.EditValue != null && dtIntructionDateTo.DateTime != DateTime.MinValue)
                {
                    hisServiceReqFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMMdd") + "000000");

                    hisServiceReqFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                }

                List<long> lstServiceReqSTT = new List<long>();
                //Chưa kết thúc 0
                //Tất cả 1
                //Chưa xử lý 2
                //Đang xử lý 3
                //Kết thúc 4
                // Gọi nhỡ 5
                //Filter yeu cau chua ket thuc
                if (cboFind.EditValue != null)
                {
                    //Chưa kết thúc
                    if (cboFind.SelectedIndex == 0)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL);
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL);
                    }
                    //Tất cả
                    else if (cboFind.SelectedIndex == 1)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL);
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL);
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT);
                    }
                    //Chưa xử lý 
                    else if (cboFind.SelectedIndex == 2)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL);
                    }
                    //Đang xử lý
                    else if (cboFind.SelectedIndex == 3)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL);
                    }
                    //Kết thúc
                    else if (cboFind.SelectedIndex == 4)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT);
                    }
                    // Gọi nhỡ
                    else if (cboFind.SelectedIndex == 5)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL);
                    }
                }

                if (cboTreatmentType.EditValue != null)
                {
                    switch (cboTreatmentType.SelectedIndex)
                    {
                        case 1:
                            hisServiceReqFilter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                            break;
                        case 2:
                            hisServiceReqFilter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
                            break;
                        case 3:
                            hisServiceReqFilter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU;
                            break;
                        default:
                            break;
                    }
                }

                if (cboInDebt.EditValue != null && cboInDebt.SelectedIndex == 1)
                {
                    hisServiceReqFilter.IS_NOT_IN_DEBT = true;
                }
                if (cboInDebt.EditValue != null && cboInDebt.SelectedIndex == 2)
                {
                    hisServiceReqFilter.IS_NOT_IN_DEBT = false;
                }
                if (lstServiceReqSTT.Count > 0)
                    hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTT;

                if (cboFind.SelectedIndex == 5)
                {
                    hisServiceReqFilter.HAS_CALL_COUNT = true;
                }
                if (cboSucKhoe.EditValue != null)
                {
                    hisServiceReqFilter.TDL_KSK_CONTRACT_ID = (long)cboSucKhoe.EditValue;
                }
                else
                {
                    hisServiceReqFilter.TDL_KSK_CONTRACT_ID = null;
                }
                if (cboDaKe.SelectedIndex == 0)
                {
                    hisServiceReqFilter.IS_ENOUGH_SUBCLINICAL_PRES = null;
                }
                if (cboDaKe.SelectedIndex == 1)
                {
                    hisServiceReqFilter.IS_ENOUGH_SUBCLINICAL_PRES = false;
                }
                if (cboDaKe.SelectedIndex == 2)
                {
                    hisServiceReqFilter.IS_ENOUGH_SUBCLINICAL_PRES = true;
                }
                if (ckKQCLS.Checked == true)
                {
                    hisServiceReqFilter.ORDER_FIELD = "INTRUCTION_DATE";

                    hisServiceReqFilter.ORDER_FIELD1 = "ORDER_TIME";
                    hisServiceReqFilter.ORDER_FIELD2 = "SERVICE_REQ_STT_ID";
                    hisServiceReqFilter.ORDER_FIELD3 = "PRIORITY";
                    hisServiceReqFilter.ORDER_FIELD4 = "NUM_ORDER";

                    hisServiceReqFilter.ORDER_DIRECTION = "DESC";
                    hisServiceReqFilter.ORDER_DIRECTION1 = "ASC";
                    hisServiceReqFilter.ORDER_DIRECTION2 = "ASC";
                    hisServiceReqFilter.ORDER_DIRECTION3 = "DESC";
                    hisServiceReqFilter.ORDER_DIRECTION4 = "ASC";
                }

                else
                {
                    hisServiceReqFilter.ORDER_FIELD = "INTRUCTION_DATE";
                    hisServiceReqFilter.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                    hisServiceReqFilter.ORDER_FIELD2 = "PRIORITY";
                    hisServiceReqFilter.ORDER_FIELD3 = "NUM_ORDER";

                    hisServiceReqFilter.ORDER_DIRECTION = "DESC";
                    hisServiceReqFilter.ORDER_DIRECTION1 = "ASC";
                    hisServiceReqFilter.ORDER_DIRECTION2 = "DESC";
                    hisServiceReqFilter.ORDER_DIRECTION3 = "ASC";
                }


                if (chkIsResult.Checked)
                {
                    hisServiceReqFilter.IS_RESULTED = true;
                }

                if (serviceSelecteds != null && serviceSelecteds.Count == 0)
                {
                    hisServiceReqFilter.SERVICE_IDs = null;
                }
                else
                {
                    if (listServices != null && serviceSelecteds != null)
                    {
                        if (serviceSelecteds.Count == listServices.Count)
                        {
                            hisServiceReqFilter.SERVICE_IDs = null;
                        }
                        else
                        {
                            hisServiceReqFilter.SERVICE_IDs = serviceSelecteds.Select(o => o.ID).ToList();
                        }
                    }
                }

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MUST_BE_APPROVED_BEFORE_PROCESS_SURGERY") == "1")
                    hisServiceReqFilter.IS_NOT_SURGERY__OR__IS__APPROVED__OR__IS_EMERGENCY = true;

                Inventec.Common.Logging.LogSystem.Debug("HIS.Desktop.Plugins.ExecuteRoom FillDataToGridServiceReq hisServiceReqFilter" + Inventec.Common.Logging.LogUtil.TraceData("", hisServiceReqFilter));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hisServiceReqFilter  #######" + Inventec.Common.Logging.LogUtil.GetMemberName(() => hisServiceReqFilter), hisServiceReqFilter));

                apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon)
                    .GetRO<List<L_HIS_SERVICE_REQ>>("api/HisServiceReq/GetLView", ApiConsumers.MosConsumer, hisServiceReqFilter, paramCommon);

                gridControlServiceReq.DataSource = null;

                if (apiResult != null && apiResult.Data != null)
                {


                    serviceReqs = (from r in apiResult.Data select new ServiceReqADO(r)).ToList();// (List<L_HIS_SERVICE_REQ>)apiResult.Data;
                    rowCount = (serviceReqs == null ? 0 : serviceReqs.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    if (rowCount > 0)
                    {
                        foreach (var item in serviceReqs)
                        {
                            if (item.CALL_COUNT.HasValue)
                            {
                                if (item.CALL_COUNT >= 1)
                                {
                                    item.status = 12;
                                }
                                else
                                {
                                    item.status = 14;
                                }
                            }
                            else
                            {
                                item.status = 14;
                            }

                        }
                        gridControlServiceReq.DataSource = serviceReqs;
                    }
                    else
                    {
                        currentHisServiceReq = null;
                    }
                }
                else
                {
                    currentHisServiceReq = null;
                }


                #region Process has exception
                SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion

                gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = false;
                gridViewServiceReq.FocusedColumn = grdColTRANGTHAI_IMG;

                maxTimeReload = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.MAX_TIME_RELOAD));
                lblAutoReload.Text = "";
                if ((EnumUtil.REFESH_ENUM)btnRefesh.Tag == EnumUtil.REFESH_ENUM.OFF && maxTimeReload > 0)
                {
                    timeCount = 0;
                    StartTimer(currentModule.ModuleLink, "timerAutoReload");
                }

                //transitionManager1.EndTransition();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private TreeSereServ7ADO InitTreeSereServ(bool IsNotGroup, bool STT_)
        {
            TreeSereServ7ADO ado = new TreeSereServ7ADO();
            try
            {
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                ado.TreeSereServ7_GetStateImage = treeSereServ_GetStateImage;
                ado.TreeSereServ7_StateImageClick = treeSereServ_StateImageClick;
                ado.TreeSereServ7_GetSelectImage = treeSereServ_GetSelectImage;
                ado.TreeSereServ7_CustomNodeCellEdit = treeSereServ_CustomNodeCellEdit;
                ado.SereServNodeCellStyle = treeSereServ_NodeCellStyle;
                ado.TreeSereServ7_DoubleClick = treeSereServ_DoubleClick;
                ado.TreeSereServ7Click = treeSereServ_Click;
                ado.TreeSereServ7_CustomUnboundColumnData = treeSereServ_CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.IsNotGroupServiceType = IsNotGroup;
                ado.TreeSereServ7_ReloadFilter = treeSereServ7_ReloadFilter;
                ado.TreeSereServ7Columns = new List<TreeSereServ7V2Column>();
                //ado.TreeSereServ7_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;

                //Gửi lại yêu cầu xét nghiệm
                TreeSereServ7V2Column colSendTestServiceReq = new TreeSereServ7V2Column("   ", "SendTestServiceReq", 30, true);
                colSendTestServiceReq.VisibleIndex = 1;
                ado.TreeSereServ7Columns.Add(colSendTestServiceReq);

                //ColumnSTT
                if (STT_)
                {
                    TreeSereServ7V2Column STT = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__STT", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NUM_ORDER", 40, false);
                    STT.VisibleIndex = 2;
                    STT.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Bound;
                    ado.TreeSereServ7Columns.Add(STT);
                }


                //Column mã dịch vụ
                TreeSereServ7V2Column serviceCodeCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                if (STT_)
                {
                    serviceCodeCol.VisibleIndex = 3;

                }
                else
                {
                    serviceCodeCol.VisibleIndex = 2;
                }
                ado.TreeSereServ7Columns.Add(serviceCodeCol);

                //Column tên dịch vụ
                TreeSereServ7V2Column serviceNameCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 300, false);
                if (STT_)
                {
                    serviceNameCol.VisibleIndex = 4;
                }
                else
                {
                    serviceNameCol.VisibleIndex = 3;
                }
                ado.TreeSereServ7Columns.Add(serviceNameCol);

                ////Column mã yêu cầu
                //TreeSereServ7V2Column serviceReqCodeCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXECUTE_ROOM__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                //serviceReqCodeCol.VisibleIndex = ;
                //ado.TreeSereServ7Columns.Add(serviceReqCodeCol);

                ////Column phòng yêu cầu
                //TreeSereServ7V2Column roomReqNameCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXECUTE_ROOM__TREE_SERE_SERV__COLUMN_ROOM_NAME", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REQUEST_ROOM_NAME", 100, false);
                //roomReqNameCol.VisibleIndex = 5;
                //ado.TreeSereServ7Columns.Add(roomReqNameCol);

                //Column ghi chú
                TreeSereServ7V2Column noteCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__COLUMN_NOTE", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NOTE_ADO", 200, false);

                if (STT_)
                {
                    noteCol.VisibleIndex = 5;
                }
                else
                {
                    noteCol.VisibleIndex = 4;
                }

                ado.TreeSereServ7Columns.Add(noteCol);

                ////Column Khoa yêu cầu
                //TreeSereServ7V2Column departmentCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__REQUEST_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REQUEST_DEPARTMENT_NAME", 250, false);
                //departmentCol.VisibleIndex = 7;
                //ado.TreeSereServ7Columns.Add(departmentCol);

                TreeSereServ7V2Column soPhieuCol = new TreeSereServ7V2Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__REQUEST_SOPHIEU", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SO_PHIEU", 250, false);


                if (STT_)
                {
                    soPhieuCol.VisibleIndex = 6;
                }
                else
                {
                    soPhieuCol.VisibleIndex = 5;
                }
                ado.TreeSereServ7Columns.Add(soPhieuCol);

                //Khoa hien tai
                ado.DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;

            }
            catch (Exception ex)
            {
                ado = new TreeSereServ7ADO();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ado;
        }

        private void treeSereServ7_ReloadFilter()
        {
            try
            {
                LoadDataToPanelRight(currentHisServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddUctoPanel()
        {
            try
            {
                ssTreeProcessor = new TreeSereServ7V2Processor();
                ucTreeSereServ7 = (UserControl)ssTreeProcessor.Run(InitTreeSereServ(false, false));
                if (ucTreeSereServ7 != null)
                {
                    xtraScrollableControl1.Controls.Add(ucTreeSereServ7);
                    ucTreeSereServ7.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// bỏ đi, không load toàn bộ giao diện lên nếu chưa hiển thị ra, chuyển sang dạng lazyload từng phần cần trước, phần nào dùng đến thì load sau
        /// </summary>
        private void AddOrtherUc()
        {
            try
            {
                p1 = new TreeSereServ7V2Processor();
                u1 = (UserControl)p1.Run(InitTreeSereServ(true, false));
                if (u1 != null)
                {
                    xtraScrollableControl2.Controls.Add(u1);
                    u1.Dock = DockStyle.Fill;
                }
                p2 = new TreeSereServ7V2Processor();
                u2 = (UserControl)p2.Run(InitTreeSereServ(true, false));
                if (u2 != null)
                {
                    xtraScrollableControl3.Controls.Add(u2);
                    u2.Dock = DockStyle.Fill;
                }
                p3 = new TreeSereServ7V2Processor();
                u3 = (UserControl)p3.Run(InitTreeSereServ(true, false));
                if (u3 != null)
                {
                    xtraScrollableControl4.Controls.Add(u3);
                    u3.Dock = DockStyle.Fill;
                }
                p4 = new TreeSereServ7V2Processor();
                u4 = (UserControl)p4.Run(InitTreeSereServ(true, false));
                if (u4 != null)
                {
                    xtraScrollableControl5.Controls.Add(u4);
                    u4.Dock = DockStyle.Fill;
                }
                p5 = new TreeSereServ7V2Processor();
                u5 = (UserControl)p5.Run(InitTreeSereServ(true, false));
                if (u5 != null)
                {
                    xtraScrollableControl6.Controls.Add(u5);
                    u5.Dock = DockStyle.Fill;
                }
                p6 = new TreeSereServ7V2Processor();
                u6 = (UserControl)p6.Run(InitTreeSereServ(true, false));
                if (u6 != null)
                {
                    xtraScrollableControl7.Controls.Add(u6);
                    u6.Dock = DockStyle.Fill;
                }
                p7 = new TreeSereServ7V2Processor();
                u7 = (UserControl)p7.Run(InitTreeSereServ(true, false));
                if (u7 != null)
                {
                    xtraScrollableControl8.Controls.Add(u7);
                    u7.Dock = DockStyle.Fill;
                }
                p8 = new TreeSereServ7V2Processor();
                u8 = (UserControl)p8.Run(InitTreeSereServ(true, false));
                if (u8 != null)
                {
                    xtraScrollableControl9.Controls.Add(u8);
                    u8.Dock = DockStyle.Fill;
                }
                p9 = new TreeSereServ7V2Processor();
                u9 = (UserControl)p9.Run(InitTreeSereServ(true, false));
                if (u9 != null)
                {
                    xtraScrollableControl10.Controls.Add(u9);
                    u9.Dock = DockStyle.Fill;
                }
                p10 = new TreeSereServ7V2Processor();
                u10 = (UserControl)p10.Run(InitTreeSereServ(true, false));
                if (u10 != null)
                {
                    xtraScrollableControl11.Controls.Add(u10);
                    u10.Dock = DockStyle.Fill;
                }
                p11 = new TreeSereServ7V2Processor();
                u11 = (UserControl)p11.Run(InitTreeSereServ(true, false));
                if (u11 != null)
                {
                    xtraScrollableControl12.Controls.Add(u11);
                    u11.Dock = DockStyle.Fill;
                }
                p12 = new TreeSereServ7V2Processor();
                u12 = (UserControl)p12.Run(InitTreeSereServ(true, false));
                if (u12 != null)
                {
                    xtraScrollableControl13.Controls.Add(u12);
                    u12.Dock = DockStyle.Fill;
                }
                p13 = new TreeSereServ7V2Processor();
                u13 = (UserControl)p13.Run(InitTreeSereServ(true, false));
                if (u13 != null)
                {
                    xtraScrollableControl14.Controls.Add(u13);
                    u13.Dock = DockStyle.Fill;
                }
                p14 = new TreeSereServ7V2Processor();
                u14 = (UserControl)p14.Run(InitTreeSereServ(true, false));
                if (u14 != null)
                {
                    xtraScrollableControl15.Controls.Add(u14);
                    u14.Dock = DockStyle.Fill;
                }
                p15 = new TreeSereServ7V2Processor();
                u15 = (UserControl)p15.Run(InitTreeSereServ(true, false));
                if (u15 != null)
                {
                    xtraScrollableControl16.Controls.Add(u15);
                    u15.Dock = DockStyle.Fill;
                }
                p16 = new TreeSereServ7V2Processor();
                u16 = (UserControl)p16.Run(InitTreeSereServ(true, true));
                if (u14 != null)
                {
                    xtraScrollableControl17.Controls.Add(u16);
                    u16.Dock = DockStyle.Fill;
                }
                HideControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HideControl()
        {
            try
            {
                xtraTabPage2.PageVisible = false;
                xtraTabPage3.PageVisible = false;
                xtraTabPage4.PageVisible = false;
                xtraTabPage5.PageVisible = false;
                xtraTabPage6.PageVisible = false;
                xtraTabPage7.PageVisible = false;
                xtraTabPage8.PageVisible = false;
                xtraTabPage9.PageVisible = false;
                xtraTabPage10.PageVisible = false;
                xtraTabPage11.PageVisible = false;
                xtraTabPage12.PageVisible = false;
                xtraTabPage13.PageVisible = false;
                xtraTabPage14.PageVisible = false;
                xtraTabPage15.PageVisible = false;
                xtraTabPage16.PageVisible = false;
                xtraTabPage17.PageVisible = false;
                xtraTabPage18.PageVisible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_Click(SereServADO data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("treeSereServ_Click");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                TreeClickData = data;
                if (data != null && !String.IsNullOrWhiteSpace(data.TDL_SERVICE_REQ_CODE))
                {
                    ProcessLoadDocumentBySereServ(data);
                }
                else
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(new List<EmrDocumentFileSDO>());
                    this.ucViewEmrDocumentResult.ReloadDocument(new List<EmrDocumentFileSDO>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {

                //if (e.Column.FieldName == "NUM_ORDER")
                //{
                //    e.Value = data.NUM_ORDER;
                //}
                if (e.Column.FieldName == "SO_PHIEU")
                {
                    e.Value = this.GetSoPhieu(data.TDL_SERVICE_REQ_CODE, data.TDL_SERVICE_CODE);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDepartmentAndRoom()
        {
            try
            {
                this.executeRoom = lstExecuteRoom.FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                if (!(this.executeRoom == null
                    || this.executeRoom.IS_EXAM == (short)1
                    || this.executeRoom.IS_SURGERY == (short)1
                    || this.executeRoom.IS_USE_KIOSK == (short)1)
                    && (HisConfigCFG.IsMachineWarningOption == "1" || HisConfigCFG.IsMachineWarningOption == "2"))
                {
                    RegisterTimer(currentModule.ModuleLink, "timerReloadMachineCounter", timerReloadMachineCounter.Interval, timerReloadMachineCounter_Tick);
                    timerReloadMachineCounter.Enabled = true;
                    timerReloadMachineCounter_Tick();
                    StartTimer(currentModule.ModuleLink, "timerReloadMachineCounter");
                }
                else
                {
                    //timerReloadMachineCounter.Stop();
                    StopTimer(currentModule.ModuleLink, "timerReloadMachineCounter");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_DoubleClick(SereServADO data)
        {
            try
            {
                if (data != null)
                {
                    V_HIS_SERE_SERV_6 sereServ6 = new V_HIS_SERE_SERV_6();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_6>(sereServ6, data);
                    if (sereServ6 != null)
                    {
                        if (sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                        || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                        || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                        || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                        {
                            this._sereServRowMenu = (V_HIS_SERE_SERV_6)gridViewSereServServiceReq.GetFocusedRow();
                            this.frmShow();
                            //this.PacsCallModuleProccess(sereServ6);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadPatientFromServiceReq(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (serviceReq != null)
                {
                    lblPatientCode.Text = serviceReq.TDL_PATIENT_CODE;
                    lblPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    lblGender.Text = serviceReq.TDL_PATIENT_GENDER_NAME;
                    lblDOB.Text = String.Format("{0} ({1})", Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB), MPS.AgeUtil.CalculateFullAge(serviceReq.TDL_PATIENT_DOB));
                    lblAddress.Text = serviceReq.TDL_PATIENT_ADDRESS;
                    lblNote.Text = serviceReq.NOTE;
                    lblIcdName.Text = (!String.IsNullOrEmpty(serviceReq.ICD_CODE) || !String.IsNullOrEmpty(serviceReq.ICD_NAME)) ? serviceReq.ICD_CODE + " - " + serviceReq.ICD_NAME : "";
                    lblIcdText.Text = (!String.IsNullOrEmpty(serviceReq.ICD_SUB_CODE) || !String.IsNullOrEmpty(serviceReq.ICD_TEXT)) ? serviceReq.ICD_SUB_CODE + " - " + serviceReq.ICD_TEXT : "";
                    // lblCardNumber.Text = serviceReq.TDL_HEIN_CARD_NUMBER;
                    //lblKCBBD.Text = serviceReq.TDL_HEIN_MEDI_ORG_CODE;

                    var IsExamRoom = lstExecuteRoom.Where(o => o.ROOM_ID == this.roomId && o.IS_EXAM == 1).ToList();
                    if (IsExamRoom != null && IsExamRoom.Count > 0)
                    {
                        lciPhoneNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciExamEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        if (!string.IsNullOrEmpty(serviceReq.TDL_PATIENT_MOBILE))
                        {
                            lbPhoneNumber.Text = serviceReq.TDL_PATIENT_MOBILE;
                        }
                        else if (!string.IsNullOrEmpty(serviceReq.TDL_PATIENT_PHONE))
                        {
                            lbPhoneNumber.Text = serviceReq.TDL_PATIENT_PHONE;
                        }
                        else
                        {
                            lbPhoneNumber.Text = "";
                        }
                        if (serviceReq.EXAM_END_TYPE != null && serviceReq.EXAM_END_TYPE != 0)
                        {
                            if (serviceReq.EXAM_END_TYPE == 1)
                            {
                                lciExamTreatmentEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lciExamTreatmentResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lciTransferInMediOrg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lbExamEndType.Text = "Khám thêm";
                            }
                            else if (serviceReq.EXAM_END_TYPE == 2)
                            {
                                lciExamTreatmentEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lciExamTreatmentResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lciTransferInMediOrg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lbExamEndType.Text = "Nhập viện";
                            }
                            else if (serviceReq.EXAM_END_TYPE == 3)
                            {
                                var endType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().Where(o => o.ID == serviceReq.EXAM_TREATMENT_END_TYPE_ID).FirstOrDefault();
                                var endResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().Where(o => o.ID == serviceReq.EXAM_TREATMENT_RESULT_ID).FirstOrDefault();
                                lbExamEndType.Text = "Kết thúc điều trị";
                                lciExamTreatmentEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                lciExamTreatmentResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                lciTransferInMediOrg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                if (endType != null)
                                {
                                    lblExamTreatmentEndType.Text = endType.TREATMENT_END_TYPE_NAME;
                                }
                                else
                                {
                                    lblExamTreatmentEndType.Text = "";
                                }
                                if (endResult != null)
                                {
                                    lblExamTreatmentResult.Text = endResult.TREATMENT_RESULT_NAME;
                                }
                                else
                                {
                                    lblExamTreatmentResult.Text = "";
                                }

                            }
                            else if (serviceReq.EXAM_END_TYPE == 4)
                            {
                                lciExamTreatmentEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lciExamTreatmentResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lciTransferInMediOrg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                                lbExamEndType.Text = "Kết thúc khám";
                            }

                        }
                        else
                        {
                            lbExamEndType.Text = "";
                            lciExamTreatmentEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciExamTreatmentResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lciTransferInMediOrg.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }

                    }
                    //if (currentTreatment4 != null)
                    //{
                    //    lbHopitalizeDepartment.Text = currentTreatment4.HOPITALIZE_DEPARTMENT_NAME;
                    //    lbTransferInMediOrg.Text = currentTreatment4.MEDI_ORG_NAME;
                    //}
                    //else
                    //{
                    //    lbHopitalizeDepartment.Text = "";
                    //    lbTransferInMediOrg.Text = "";
                    //}
                    HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                    filterPatienTypeAlter.TREATMENT_ID = serviceReq.TREATMENT_ID;
                    var currentPatientTypeAlters = await new Inventec.Common.Adapter.BackendAdapter(param)
                        .GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param);
                    this.currentPatientTypeAlter = currentPatientTypeAlters != null ? currentPatientTypeAlters.OrderByDescending(o => o.ID).ThenByDescending(o => o.LOG_TIME).FirstOrDefault() : null;
                    //Mức hưởng BHYT
                    if (this.currentPatientTypeAlter != null &&
                        !String.IsNullOrEmpty(this.currentPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        decimal ratio = 0;
                        ratio = GetDefaultHeinRatio(this.currentPatientTypeAlter.HEIN_CARD_NUMBER, this.currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, this.currentPatientTypeAlter.LEVEL_CODE, this.currentPatientTypeAlter.RIGHT_ROUTE_CODE);
                        lblCardNumber.Text = this.currentPatientTypeAlter.HEIN_CARD_NUMBER + " (" + ratio * 100 + " %" + ")";
                        lblKCBBD.Text = this.currentPatientTypeAlter.HEIN_MEDI_ORG_CODE;

                        //lblRatio.Text = ratio * 100 + " %";
                        lblHanThe.Text = String.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0));
                        var heinRightRouteData = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(this.currentPatientTypeAlter.RIGHT_ROUTE_CODE);
                        lblLoai.Text = heinRightRouteData != null ? heinRightRouteData.HeinRightRouteName : "";
                    }
                    else
                    {
                        lblKCBBD.Text = "";
                        //lblRatio.Text = "";
                        lblCardNumber.Text = "";
                        lblHanThe.Text = "";
                        lblLoai.Text = "";
                    }

                    if (!String.IsNullOrEmpty(serviceReq.TDL_PATIENT_AVATAR_URL))
                    {
                        MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(serviceReq.TDL_PATIENT_AVATAR_URL);
                        pictureEditAvatar.Image = Image.FromStream(stream);
                        pictureEditAvatar.Image.Tag = serviceReq.TDL_PATIENT_AVATAR_URL;
                    }
                    else
                    {
                        string pathLocal = GetPathDefault();
                        pictureEditAvatar.Image = Image.FromFile(pathLocal);
                    }
                }
                else
                {
                    lblPatientCode.Text = "";
                    lblPatientName.Text = "";
                    lblGender.Text = "";
                    lblDOB.Text = "";
                    lblAddress.Text = "";
                    lblKCBBD.Text = "";
                    lblCardNumber.Text = "";
                    //lblRatio.Text = "";
                    lblHanThe.Text = "";
                    lblLoai.Text = "";
                    lblNote.Text = "";
                    string pathLocal = GetPathDefault();
                    pictureEditAvatar.Image = Image.FromFile(pathLocal);
                    //lblPatientTypeName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetDefaultHeinRatio(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }

        private async Task LoadTreeListSereServChild(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadTreeListSereServChild_1__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReq), serviceReq));
                HideControl();
                if (ucTreeSereServ7 != null && SereServCurrentTreatment != null)
                {
                    List<L_HIS_SERVICE_REQ> serviceReqChilds = null;
                    sereServ7s = new List<SereServADO>();

                    if (serviceReq != null)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();

                        AutoMapper.Mapper.CreateMap<ADOserserv7, V_HIS_SERE_SERV_7>();
                        var ss7Mapper = AutoMapper.Mapper.Map<List<ADOserserv7>, List<V_HIS_SERE_SERV_7>>(SereServCurrentTreatment);
                        //
                        AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_7, SereServADO>();
                        sereServ7s = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_7>, List<SereServADO>>(ss7Mapper);
                        CommonParam param_ = new CommonParam();
                        HisServiceReqFilter filter_ = new HisServiceReqFilter();
                        filter_.IDs = sereServ7s.Where(o => o.SERVICE_REQ_ID != null).Select(o => (long)o.SERVICE_REQ_ID).Distinct().ToList();
                        filter_.ColumnParams = new List<string>()
                    {
                        "ID",
                        "NUM_ORDER",
                        "SAMPLE_TIME",
                        "SERVICE_REQ_STT_ID",
                        "RECEIVE_SAMPLE_TIME"
                    };

                        var data_ = await new BackendAdapter(param_)
                            .GetAsync<List<HIS_SERVICE_REQ>>("api/HisServiceReq/GetDynamic", ApiConsumers.MosConsumer, filter_, param_);
                        if (data_ != null && data_.Count > 0)
                        {
                            foreach (var item in data_)
                            {
                                foreach (var item_ in sereServ7s)
                                {
                                    if (item.ID == item_.SERVICE_REQ_ID)
                                    {
                                        item_.NUM_ORDER = item.NUM_ORDER;
                                        item_.SAMPLE_TIME = item.SAMPLE_TIME;
                                        item_.RECEIVE_SAMPLE_TIME = item.RECEIVE_SAMPLE_TIME;
                                    }
                                }
                            }
                        }
                        if (ServiceReqCurrentTreatment != null && ServiceReqCurrentTreatment.Count > 0)
                        {
                            sereServ7s = ServiceReqCurrentTreatment != null ? sereServ7s.Where(o => ServiceReqCurrentTreatment.Select(p => p.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList() : sereServ7s;
                        }

                        serviceReqChilds = getListServiceReqChilds(sereServ7s, serviceReq);
                        #region
                        if (serviceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                        {
                            xtraTabPage2.PageVisible = false;
                        }
                        else
                        {
                            xtraTabPage2.PageVisible = true;
                            #region TabKhamBenh
                            HisServiceReqFilter filter = new HisServiceReqFilter();
                            filter.ID = serviceReq.ID;
                            var data = await new BackendAdapter(param).GetAsync<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                            if (data != null && data.Count > 0)
                            {
                                HIS_SERVICE_REQ currentHisServiceReq = data.FirstOrDefault();
                                txtLyDoKham.Text = currentHisServiceReq.HOSPITALIZATION_REASON;
                                txtQuaTrinhBenhLy.Text = currentHisServiceReq.PATHOLOGICAL_PROCESS;
                                txtKhamToanThan.Text = currentHisServiceReq.FULL_EXAM;
                                txtKhamBoPhan.Text = currentHisServiceReq.PART_EXAM;
                                txtTomTat.Text = currentHisServiceReq.SUBCLINICAL;
                                txtCdSoBo.Text = currentHisServiceReq.PROVISIONAL_DIAGNOSIS;
                                txtHDTCode.Text = currentHisServiceReq.NEXT_TREAT_INTR_CODE;
                                txtHDTName.Text = currentHisServiceReq.NEXT_TREATMENT_INSTRUCTION;
                                txtCdCode.Text = currentHisServiceReq.ICD_CODE;
                                txtCdName.Text = currentHisServiceReq.ICD_NAME;
                                txtNNNCode.Text = currentHisServiceReq.ICD_CAUSE_CODE;
                                txtNNNName.Text = currentHisServiceReq.ICD_CAUSE_NAME;
                                txtBenhPhuCode.Text = currentHisServiceReq.ICD_SUB_CODE;
                                txtBenhPhuName.Text = currentHisServiceReq.ICD_TEXT;
                                txtTienSuBenh.Text = currentHisServiceReq.PATHOLOGICAL_HISTORY;
                                if (currentHisServiceReq.DHST_ID != null && currentHisServiceReq.DHST_ID > 0)
                                {
                                    #region DHST
                                    HisDhstFilter dhstFilter = new HisDhstFilter();
                                    dhstFilter.ID = currentHisServiceReq.DHST_ID;
                                    var dataDHST = await new BackendAdapter(param).GetAsync<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                                    if (data != null && data.Count > 0)
                                    {
                                        HIS_DHST currentDhst = dataDHST.FirstOrDefault();
                                        txtMach.Text = currentDhst.PULSE != null ? currentDhst.PULSE.ToString() : "";
                                        txtNhietDo.Text = currentDhst.TEMPERATURE != null ? currentDhst.TEMPERATURE.ToString() : "";
                                        txtHACode.Text = currentDhst.BLOOD_PRESSURE_MAX != null ? currentDhst.BLOOD_PRESSURE_MAX.ToString() : "";
                                        txtHAName.Text = currentDhst.BLOOD_PRESSURE_MIN != null ? currentDhst.BLOOD_PRESSURE_MIN.ToString() : "";
                                        txtNhipTho.Text = currentDhst.BREATH_RATE != null ? currentDhst.BREATH_RATE.ToString() : "";
                                        txtCanNang.Text = currentDhst.WEIGHT != null ? currentDhst.WEIGHT.ToString() : "";
                                        txtChieuCao.Text = currentDhst.HEIGHT != null ? currentDhst.HEIGHT.ToString() : "";
                                        decimal bmi = currentDhst.VIR_BMI != null ? currentDhst.VIR_BMI ?? 0 : 0;

                                        txtBMI.Text = bmi + "";
                                        if (bmi < 16)
                                        {
                                            txtBMIDisplay.Text = "(Gầy độ III)";
                                        }
                                        else if (16 <= bmi && bmi < 17)
                                        {
                                            txtBMIDisplay.Text = "(Gầy độ II)";
                                        }
                                        else if (17 <= bmi && bmi < (decimal)18.5)
                                        {
                                            txtBMIDisplay.Text = "(Gầy độ I)";
                                        }
                                        else if ((decimal)18.5 <= bmi && bmi < 25)
                                        {
                                            txtBMIDisplay.Text = "(Bình thường)";
                                        }
                                        else if (25 <= bmi && bmi < 30)
                                        {
                                            txtBMIDisplay.Text = "(Thừa cân)";
                                        }
                                        else if (30 <= bmi && bmi < 35)
                                        {
                                            txtBMIDisplay.Text = "(Béo phì độ I)";
                                        }
                                        else if (35 <= bmi && bmi < 40)
                                        {
                                            txtBMIDisplay.Text = "(Béo phì độ II)";
                                        }
                                        else if (40 < bmi)
                                        {
                                            txtBMIDisplay.Text = "(Béo phì độ III)";
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    txtMach.Text = "";
                                    txtNhietDo.Text = "";
                                    txtHACode.Text = "";
                                    txtHAName.Text = "";
                                    txtNhipTho.Text = "";
                                    txtCanNang.Text = "";
                                    txtChieuCao.Text = "";
                                    txtBMI.Text = "" + 0;
                                    txtBMIDisplay.Text = "";
                                }
                            }
                            #endregion

                        }
                        #endregion
                    }

                    ssTreeProcessor.Reload(ucTreeSereServ7, sereServ7s, serviceReqChilds);
                    for (int x = 0; x < xtraTabControl1.TabPages.Count; x++)
                    {
                        if (xtraTabControl1.TabPages[x].Name == "xtraTabPage1")
                        {
                            xtraTabControl1.MakePageVisible(xtraTabControl1.TabPages[x]);
                            xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[x];
                            break;
                        }
                    }
                    //xtraTabControl1.SelectedTabPageIndex = 0;
                    //xtraTabControl1.MultiLine = DefaultBoolean.True;
                    //(xtraTabPage1 as IXtraTabPageExt).Pinned = true;  
                    if (ssTreeProcessor.GetListAll(ucTreeSereServ7) != null && ssTreeProcessor.GetListAll(ucTreeSereServ7).Count > 0)
                    {
                        var checkServiceType = sereServ7s.Select(o => o.TDL_SERVICE_TYPE_ID).Distinct().ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkServiceType), checkServiceType));

                        if (checkServiceType != null && checkServiceType.Count > 0)
                        {

                            for (int j = 0; j < checkServiceType.Count; j++)
                            {
                                var getData = sereServ7s.Where(o => o.TDL_SERVICE_TYPE_ID == checkServiceType[j]).ToList();
                                if (getData != null && getData.Count > 0)
                                {
                                    List<SereServADO> newList = new List<SereServADO>();
                                    newList.AddRange(getData);
                                    List<L_HIS_SERVICE_REQ> srChild = getListServiceReqChilds(getData, serviceReq);

                                    #region AddUC
                                    if (!ssTreeProcessor.GetListAll(ucTreeSereServ7).Select(o => o.TDL_SERVICE_TYPE_ID).ToList().Contains(checkServiceType[j]))
                                        continue;
                                    if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                                    {
                                        xtraTabPage18.PageVisible = true;
                                        if (p16 == null || u16 == null)
                                        {
                                            p16 = new TreeSereServ7V2Processor();
                                            u16 = (UserControl)p16.Run(InitTreeSereServ(true, true));
                                            if (u14 != null)
                                            {
                                                xtraScrollableControl17.Controls.Add(u16);
                                                u16.Dock = DockStyle.Fill;
                                            }
                                        }

                                        p16.Reload(u16, newList, srChild);
                                        xtraTabPage18.Text = "Khám " + p16.GetTitleCount(u16);

                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                    {
                                        xtraTabPage4.PageVisible = true;
                                        if (p1 == null || u1 == null)
                                        {
                                            p1 = new TreeSereServ7V2Processor();
                                            u1 = (UserControl)p1.Run(InitTreeSereServ(true, false));
                                            if (u1 != null)
                                            {
                                                xtraScrollableControl2.Controls.Add(u1);
                                                u1.Dock = DockStyle.Fill;
                                            }
                                        }

                                        p1.Reload(u1, newList, srChild);
                                        xtraTabPage4.Text = "Xét nghiệm " + p1.GetTitleCount(u1);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                                    {
                                        xtraTabPage5.PageVisible = true;
                                        if (p2 == null || u2 == null)
                                        {
                                            p2 = new TreeSereServ7V2Processor();
                                            u2 = (UserControl)p2.Run(InitTreeSereServ(true, false));
                                            if (u2 != null)
                                            {
                                                xtraScrollableControl3.Controls.Add(u2);
                                                u2.Dock = DockStyle.Fill;
                                            }
                                        }

                                        p2.Reload(u2, newList, srChild);
                                        xtraTabPage5.Text = "Chẩn đoán hình ảnh " + p2.GetTitleCount(u2);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                    {
                                        xtraTabPage6.PageVisible = true;
                                        if (p3 == null || u3 == null)
                                        {
                                            p3 = new TreeSereServ7V2Processor();
                                            u3 = (UserControl)p3.Run(InitTreeSereServ(true, false));
                                            if (u3 != null)
                                            {
                                                xtraScrollableControl4.Controls.Add(u3);
                                                u3.Dock = DockStyle.Fill;
                                            }

                                        }
                                        p3.Reload(u3, newList, srChild);
                                        xtraTabPage6.Text = "Thủ thuật " + p3.GetTitleCount(u3);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                                    {
                                        xtraTabPage7.PageVisible = true;
                                        if (p4 == null || u4 == null)
                                        {
                                            p4 = new TreeSereServ7V2Processor();
                                            u4 = (UserControl)p4.Run(InitTreeSereServ(true, false));
                                            if (u4 != null)
                                            {
                                                xtraScrollableControl5.Controls.Add(u4);
                                                u4.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p4.Reload(u4, newList, srChild);
                                        xtraTabPage7.Text = "Thăm dò chức năng " + p4.GetTitleCount(u4);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                                    {
                                        xtraTabPage8.PageVisible = true;
                                        if (p5 == null || u5 == null)
                                        {
                                            p5 = new TreeSereServ7V2Processor();
                                            u5 = (UserControl)p5.Run(InitTreeSereServ(true, false));
                                            if (u5 != null)
                                            {
                                                xtraScrollableControl6.Controls.Add(u5);
                                                u5.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p5.Reload(u5, newList, srChild);
                                        xtraTabPage8.Text = "Thuốc " + p5.GetTitleCount(u5);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                                    {
                                        xtraTabPage9.PageVisible = true;
                                        if (p6 == null || u6 == null)
                                        {
                                            p6 = new TreeSereServ7V2Processor();
                                            u6 = (UserControl)p6.Run(InitTreeSereServ(true, false));
                                            if (u6 != null)
                                            {
                                                xtraScrollableControl7.Controls.Add(u6);
                                                u6.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p6.Reload(u6, newList, srChild);
                                        xtraTabPage9.Text = "Vật tư " + p6.GetTitleCount(u6);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                                    {
                                        xtraTabPage10.PageVisible = true;
                                        if (p7 == null || u7 == null)
                                        {
                                            p7 = new TreeSereServ7V2Processor();
                                            u7 = (UserControl)p7.Run(InitTreeSereServ(true, false));
                                            if (u7 != null)
                                            {
                                                xtraScrollableControl8.Controls.Add(u7);
                                                u7.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p7.Reload(u7, newList, srChild);
                                        xtraTabPage10.Text = "Giường " + p7.GetTitleCount(u7);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS)
                                    {
                                        xtraTabPage11.PageVisible = true;
                                        if (p8 == null || u8 == null)
                                        {
                                            p8 = new TreeSereServ7V2Processor();
                                            u8 = (UserControl)p8.Run(InitTreeSereServ(true, false));
                                            if (u8 != null)
                                            {
                                                xtraScrollableControl9.Controls.Add(u8);
                                                u8.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p8.Reload(u8, newList, srChild);
                                        xtraTabPage11.Text = "Nội soi " + p8.GetTitleCount(u8);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                                    {
                                        xtraTabPage12.PageVisible = true;
                                        if (p9 == null || u9 == null)
                                        {
                                            p9 = new TreeSereServ7V2Processor();
                                            u9 = (UserControl)p9.Run(InitTreeSereServ(true, false));
                                            if (u9 != null)
                                            {
                                                xtraScrollableControl10.Controls.Add(u9);
                                                u9.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p9.Reload(u9, newList, srChild);
                                        xtraTabPage12.Text = "Siêu âm " + p9.GetTitleCount(u9);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                                    {
                                        xtraTabPage3.PageVisible = true;
                                        if (p10 == null || u10 == null)
                                        {
                                            p10 = new TreeSereServ7V2Processor();
                                            u10 = (UserControl)p10.Run(InitTreeSereServ(true, false));
                                            if (u10 != null)
                                            {
                                                xtraScrollableControl11.Controls.Add(u10);
                                                u10.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p10.Reload(u10, newList, srChild);
                                        xtraTabPage3.Text = "Phẫu thuật " + p10.GetTitleCount(u10);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                                    {
                                        xtraTabPage13.PageVisible = true;
                                        if (p11 == null || u11 == null)
                                        {
                                            p11 = new TreeSereServ7V2Processor();
                                            u11 = (UserControl)p11.Run(InitTreeSereServ(true, false));
                                            if (u11 != null)
                                            {
                                                xtraScrollableControl12.Controls.Add(u11);
                                                u11.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p11.Reload(u11, newList, srChild);
                                        xtraTabPage13.Text = "Khác " + p11.GetTitleCount(u11);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN)
                                    {
                                        xtraTabPage14.PageVisible = true;
                                        if (p12 == null || u12 == null)
                                        {
                                            p12 = new TreeSereServ7V2Processor();
                                            u12 = (UserControl)p12.Run(InitTreeSereServ(true, false));
                                            if (u12 != null)
                                            {
                                                xtraScrollableControl13.Controls.Add(u12);
                                                u12.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p12.Reload(u12, newList, srChild);
                                        xtraTabPage14.Text = "Phục hồi chức năng " + p12.GetTitleCount(u12);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL)
                                    {
                                        xtraTabPage16.PageVisible = true;
                                        if (p14 == null || u14 == null)
                                        {
                                            p14 = new TreeSereServ7V2Processor();
                                            u14 = (UserControl)p14.Run(InitTreeSereServ(true, false));
                                            if (u14 != null)
                                            {
                                                xtraScrollableControl15.Controls.Add(u14);
                                                u14.Dock = DockStyle.Fill;
                                            }

                                        }
                                        p14.Reload(u14, newList, srChild);
                                        xtraTabPage16.Text = "Giải phẫu bệnh lý " + p14.GetTitleCount(u14);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                                    {
                                        xtraTabPage15.PageVisible = true;
                                        if (p13 == null || u13 == null)
                                        {
                                            p13 = new TreeSereServ7V2Processor();
                                            u13 = (UserControl)p13.Run(InitTreeSereServ(true, false));
                                            if (u13 != null)
                                            {
                                                xtraScrollableControl14.Controls.Add(u13);
                                                u13.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p13.Reload(u13, newList, srChild);
                                        xtraTabPage15.Text = "Máu " + p13.GetTitleCount(u13);
                                    }
                                    else if (checkServiceType[j] == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                                    {
                                        xtraTabPage17.PageVisible = true;
                                        if (p15 == null || u15 == null)
                                        {
                                            p15 = new TreeSereServ7V2Processor();
                                            u15 = (UserControl)p15.Run(InitTreeSereServ(true, false));
                                            if (u15 != null)
                                            {
                                                xtraScrollableControl16.Controls.Add(u15);
                                                u15.Dock = DockStyle.Fill;
                                            }
                                        }
                                        p15.Reload(u15, newList, srChild);
                                        xtraTabPage17.Text = "Suất ăn " + p15.GetTitleCount(u15);
                                    }
                                    #endregion
                                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => newList), newList));

                                    //string nameType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == checkServiceType[j]).SERVICE_TYPE_NAME;

                                    //DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl = new XtraScrollableControl();
                                    //xtraScrollableControl.Dock = System.Windows.Forms.DockStyle.Fill;
                                    //xtraScrollableControl.Name = "xtraScrollableControl_" + nameType;

                                    //DevExpress.XtraTab.XtraTabPage xtraTabPage = new DevExpress.XtraTab.XtraTabPage();
                                    //xtraTabPage.Controls.Add(xtraScrollableControl);
                                    //xtraTabPage.Name = "xtraTabPage_" + checkServiceType[j];
                                    //xtraTabPage.Text = nameType;

                                    //xtraTabControl1.TabPages.Add(xtraTabPage);

                                    //UserControl uc = new UserControl();
                                    //TreeSereServ7V2Processor process = new TreeSereServ7V2Processor();

                                    //uc = (UserControl)process.Run(InitTreeSereServ());
                                    //if (uc != null)
                                    //{
                                    //    xtraScrollableControl.Controls.Add(uc);
                                    //    uc.Dock = DockStyle.Fill;
                                    //}

                                }
                            }
                        }

                    }

                    WaitingManager.Hide();
                }
                else
                {
                    ssTreeProcessor.Reload(ucTreeSereServ7, new List<V_HIS_SERE_SERV_7>(), new List<L_HIS_SERVICE_REQ>());
                }

                this.ucViewEmrDocumentReq.ReloadDocument(new List<EmrDocumentFileSDO>());
                this.ucViewEmrDocumentResult.ReloadDocument(new List<EmrDocumentFileSDO>());

                Inventec.Common.Logging.LogSystem.Debug("LoadTreeListSereServChild.2");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<L_HIS_SERVICE_REQ> getListServiceReqChilds(List<SereServADO> sereServ7s, L_HIS_SERVICE_REQ serviceReq)
        {
            List<L_HIS_SERVICE_REQ> serviceReqChilds = new List<L_HIS_SERVICE_REQ>();
            try
            {
                List<V_HIS_SERVICE> services = listServices;
                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                foreach (var item in sereServ7s)
                {
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                    if (service != null)
                    {
                        item.TDL_SERVICE_CODE = service.SERVICE_CODE;
                        item.TDL_SERVICE_NAME = service.SERVICE_NAME;
                        item.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                        item.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        item.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                        item.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                        item.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                    }

                    HIS_DEPARTMENT department = departments.FirstOrDefault(o => o.ID == item.TDL_REQUEST_DEPARTMENT_ID);
                    if (department != null)
                    {
                        item.TDL_REQUEST_DEPARTMENT_ID = department.ID;
                        item.REQUEST_DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                        item.REQUEST_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                    }

                    V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == item.TDL_REQUEST_ROOM_ID);
                    if (room != null)
                    {
                        item.REQUEST_ROOM_NAME = room.ROOM_NAME;
                        item.REQUEST_ROOM_CODE = room.ROOM_CODE;
                    }

                    L_HIS_SERVICE_REQ serviceReqItem = ServiceReqCurrentTreatment != null ? ServiceReqCurrentTreatment.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID) : null;
                    if (serviceReqItem != null)
                    {
                        item.SERVICE_REQ_STT_ID = serviceReqItem.SERVICE_REQ_STT_ID;
                    }
                }
                serviceReqChilds = ServiceReqCurrentTreatment != null ? ServiceReqCurrentTreatment.Where(o => o.PARENT_ID == serviceReq.ID).ToList() : null;
            }
            catch (Exception ex)
            {
                serviceReqChilds = new List<L_HIS_SERVICE_REQ>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serviceReqChilds;
        }

        private async Task LoadSereServServiceReq(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadSereServServiceReq.1");
                InitRestoreLayoutGridViewFromXml(gridViewSereServServiceReq);
                gridControlSereServServiceReq.DataSource = null;
                sereServ6s = new List<SereServ6ADO>();
                if (serviceReq != null && SereServCurrentTreatment != null)
                {
                    List<ADOserserv7> sereServByServiceReqs = SereServCurrentTreatment.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).OrderBy(o => o.IS_NO_EXECUTE ?? 0).ToList();
                    if (sereServByServiceReqs != null && sereServByServiceReqs.Count > 0)
                    {
                        List<HIS_SERE_SERV_EXT> lstExt = new List<HIS_SERE_SERV_EXT>();
                        if (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA ||
                            serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS ||
                            serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA ||
                             serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN ||
                            serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                        {
                            CommonParam param = new CommonParam();
                            HisSereServExtFilter extFilter = new HisSereServExtFilter();
                            extFilter.SERE_SERV_IDs = sereServByServiceReqs.Select(s => s.ID).ToList();
                            lstExt = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, extFilter, param);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstExV_HIS_SERE_SERV_6t: ", lstExt));
                        }

                        foreach (var item in sereServByServiceReqs)
                        {
                            //V_HIS_SERE_SERV_6 ss = new V_HIS_SERE_SERV_6();
                            //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_6>(ss, item);
                            //HIS_SERVICE_UNIT serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            //if (serviceUnit != null)
                            //{
                            //    ss.SERVICE_UNIT_CODE = serviceUnit.SERVICE_UNIT_CODE;
                            //    ss.SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
                            //}

                            //HIS_SERVICE_TYPE serviceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                            //    .FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID);
                            //if (serviceType != null)
                            //{
                            //    ss.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                            //    ss.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                            //}
                            var ss = new SereServ6ADO(item);
                            if (lstExt != null && lstExt.Count > 0)
                            {
                                var ext = lstExt.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                                if (ext != null)
                                {
                                    ss.SereServExt = ext;
                                    ss.INSTRUCTION_NOTE = ext.INSTRUCTION_NOTE;
                                    ss.MACHINE_NAME = GetMachineNameFromMachine(ext);
                                }
                            }

                            sereServ6s.Add(ss);
                        }
                    }

                    ShowColumns(sereServ6s);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sereServ6s: ", sereServ6s));
                    gridViewSereServServiceReq.BeginDataUpdate();
                    gridControlSereServServiceReq.DataSource = null;
                    gridControlSereServServiceReq.DataSource = sereServ6s;
                    gridViewSereServServiceReq.EndDataUpdate();
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadSereServServiceReq.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string GetMachineNameFromMachine(HIS_SERE_SERV_EXT data)
        {
            string name = null;
            try
            {
                if (data.MACHINE_ID == null)
                    return null;
                var machine = BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == data.MACHINE_ID);
                if (machine != null)
                    name = machine.MACHINE_NAME;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return name;
        }

        private void ShowColumns(List<SereServ6ADO> sereServ6s)
        {
            try
            {
                gridColumn12.VisibleIndex = -1;
                gridColumn13.VisibleIndex = -1;
                if (sereServ6s != null && sereServ6s.Count > 0)
                {
                    if (sereServ6s.Exists(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                    || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                    || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                    || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA))
                    {
                        gridColumn13.VisibleIndex = 1;
                        gridColumn12.VisibleIndex = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitButtonExecuteBySereServ(List<V_HIS_SERE_SERV_6> sereServ6s)
        {
            try
            {
                HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.SERE_SERV_IDs = sereServ6s.Select(o => o.ID).ToList();
                List<HIS_SERE_SERV_BILL> sereServBills = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, sereServBillFilter, new CommonParam()).ToList();
                if (sereServBills != null && sereServBills.Count > 0)
                {
                    btnExecute.Enabled = true;
                }
                else
                {
                    btnExecute.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi api và đổ dữ liệu vào vùng panel bên phải gồm: các tab page chi tiết dịch vụ(khám, xét nghiệm,...); vùng thông tin bệnh nhân, vùng thông tin văn bản,...
        /// </summary>
        /// <param name="serviceReq"></param>
        private void LoadDataToPanelRight(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                WaitingManager.Show();
                //Bỏ code này đi, không gọi theo multi thread không hiệu quả, cần tách code gọi bất đồng bộ async từng vùng giao diện
                //List<Action> methods = new List<Action>();
                //Inventec.Common.Logging.LogSystem.Debug("LoadSereServByTreatment");
                //methods.Add(LoadSereServByTreatment);
                //Inventec.Common.Logging.LogSystem.Debug("LoadServiceReqByTreatment");
                //methods.Add(LoadServiceReqByTreatment);
                //Inventec.Common.Logging.LogSystem.Debug("LoadTreatment4ByServiceReq");
                //methods.Add(LoadTreatment4ByServiceReq);
                //ThreadCustomManager.MultipleThreadWithJoin(methods);


                Inventec.Common.Logging.LogSystem.Debug("LoadSereServByTreatment");
                //Gọi hàm bất đồng bộ không await để cho phép chúng chạy mà không chờ đợi vẫn chạy các hàm tiếp sau
                //Lưu ý Không dùng Task.Run(...) trường hợp này vì có thể gây lỗi
                LoadSereServByTreatment();

                Inventec.Common.Logging.LogSystem.Debug("LoadTreatment4ByServiceReq");
                //Gọi hàm bất đồng bộ không await để cho phép chúng chạy mà không chờ đợi vẫn chạy các hàm tiếp sau
                //Lưu ý Không dùng Task.Run(...) trường hợp này vì có thể gây lỗi
                LoadTreatment4ByServiceReq();

                Inventec.Common.Logging.LogSystem.Debug("LoadPatientFromServiceReq");
                //Gọi hàm bất đồng bộ không await để cho phép chúng chạy mà không chờ đợi vẫn chạy các hàm tiếp sau
                //Lưu ý Không dùng Task.Run(...) trường hợp này vì có thể gây lỗi
                LoadPatientFromServiceReq(serviceReq);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_SERVICE_REQ GetDynamicData(L_HIS_SERVICE_REQ serviceReqInput)
        {
            V_HIS_SERVICE_REQ result = null;
            try
            {
                MOS.Filter.HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ColumnParams = new List<string>();
                filter.ColumnParams.Add("DHST_ID");
                //filter.ColumnParams.Add("EXE_SERVICE_MODULE_ID");
                filter.ColumnParams.Add("EXECUTE_LOGINNAME");
                filter.ColumnParams.Add("EXECUTE_ROOM_ID");
                filter.ColumnParams.Add("ID");
                filter.ColumnParams.Add("INTRUCTION_TIME");
                filter.ColumnParams.Add("START_TIME");
                filter.ColumnParams.Add("IS_EMERGENCY");
                filter.ColumnParams.Add("IS_NO_EXECUTE");
                filter.ColumnParams.Add("IS_WAIT_CHILD");
                filter.ColumnParams.Add("NUM_ORDER");
                filter.ColumnParams.Add("PARENT_ID");
                filter.ColumnParams.Add("PRIORITY");
                filter.ColumnParams.Add("REQUEST_DEPARTMENT_ID");
                filter.ColumnParams.Add("REQUEST_ROOM_ID");
                filter.ColumnParams.Add("SERVICE_REQ_CODE");
                filter.ColumnParams.Add("SERVICE_REQ_STT_ID");
                filter.ColumnParams.Add("SERVICE_REQ_TYPE_ID");
                filter.ColumnParams.Add("TDL_HEIN_CARD_NUMBER");
                filter.ColumnParams.Add("TDL_PATIENT_ADDRESS");
                filter.ColumnParams.Add("TDL_PATIENT_AVATAR_URL");
                filter.ColumnParams.Add("TDL_PATIENT_CODE");
                filter.ColumnParams.Add("TDL_PATIENT_DOB");
                filter.ColumnParams.Add("TDL_PATIENT_GENDER_NAME");
                filter.ColumnParams.Add("TDL_PATIENT_NAME");
                filter.ColumnParams.Add("TDL_PATIENT_GENDER_ID");
                filter.ColumnParams.Add("TDL_PATIENT_ID");
                filter.ColumnParams.Add("TDL_TREATMENT_CODE");
                filter.ColumnParams.Add("TDL_TREATMENT_TYPE_ID");
                filter.ColumnParams.Add("TREATMENT_ID");
                filter.ColumnParams.Add("IS_MAIN_EXAM");
                filter.ColumnParams.Add("HEALTH_EXAM_RANK_ID");
                filter.ColumnParams.Add("ICD_CODE");
                filter.ColumnParams.Add("ICD_NAME");
                filter.ColumnParams.Add("ICD_CAUSE_CODE");
                filter.ColumnParams.Add("ICD_CAUSE_NAME");
                filter.ColumnParams.Add("ICD_SUB_CODE");
                filter.ColumnParams.Add("ICD_TEXT");
                filter.ColumnParams.Add("NEXT_TREAT_INTR_CODE");
                filter.ColumnParams.Add("HOSPITALIZATION_REASON");
                filter.ColumnParams.Add("SICK_DAY");
                filter.ColumnParams.Add("PATHOLOGICAL_PROCESS");
                filter.ColumnParams.Add("SUBCLINICAL");
                filter.ColumnParams.Add("TREATMENT_INSTRUCTION");
                filter.ColumnParams.Add("NOTE");
                filter.ColumnParams.Add("PROVISIONAL_DIAGNOSIS");
                filter.ColumnParams.Add("PATHOLOGICAL_HISTORY_FAMILY");
                filter.ColumnParams.Add("PATHOLOGICAL_HISTORY");
                filter.ColumnParams.Add("EXECUTE_DEPARTMENT_ID");
                filter.ColumnParams.Add("FINISH_TIME");
                filter.ColumnParams.Add("IS_NOT_USE_BHYT");
                filter.ColumnParams.Add("APPOINTMENT_TIME");
                filter.ColumnParams.Add("APPOINTMENT_DESC");

                filter.ColumnParams.Add("TRADITIONAL_ICD_CODE");
                filter.ColumnParams.Add("TRADITIONAL_ICD_NAME");
                filter.ColumnParams.Add("TRADITIONAL_ICD_SUB_CODE");
                filter.ColumnParams.Add("TRADITIONAL_ICD_TEXT");
                //bỏ bớt các thông tin cần get Dynamic để tránh làm dài param
                if (serviceReqInput.EXE_SERVICE_MODULE_ID == IMSys.DbConfig.HIS_RS.HIS_EXE_SERVICE_MODULE.ID__KHAM)
                {
                    filter.ColumnParams.Add("FULL_EXAM");
                    filter.ColumnParams.Add("PART_EXAM");
                    filter.ColumnParams.Add("PART_EXAM_CIRCULATION");
                    filter.ColumnParams.Add("PART_EXAM_RESPIRATORY");
                    filter.ColumnParams.Add("PART_EXAM_DIGESTION");
                    filter.ColumnParams.Add("PART_EXAM_KIDNEY_UROLOGY");
                    filter.ColumnParams.Add("PART_EXAM_MENTAL");
                    filter.ColumnParams.Add("PART_EXAM_NUTRITION");
                    filter.ColumnParams.Add("PART_EXAM_MOTION");
                    filter.ColumnParams.Add("PART_EXAM_OBSTETRIC");
                    filter.ColumnParams.Add("PART_EXAM_NEUROLOGICAL");
                    filter.ColumnParams.Add("PART_EXAM_MUSCLE_BONE");
                    filter.ColumnParams.Add("PART_EXAM_EAR");
                    filter.ColumnParams.Add("PART_EXAM_NOSE");
                    filter.ColumnParams.Add("PART_EXAM_THROAT");
                    filter.ColumnParams.Add("PART_EXAM_STOMATOLOGY");
                    filter.ColumnParams.Add("PART_EXAM_EYE");
                    filter.ColumnParams.Add("PART_EXAM_EYE_TENSION_LEFT");
                    filter.ColumnParams.Add("PART_EXAM_EYE_TENSION_RIGHT");
                    filter.ColumnParams.Add("PART_EXAM_EYESIGHT_LEFT");
                    filter.ColumnParams.Add("PART_EXAM_EYESIGHT_RIGHT");
                    filter.ColumnParams.Add("PART_EXAM_EYESIGHT_GLASS_LEFT");
                    filter.ColumnParams.Add("PART_EXAM_EYESIGHT_GLASS_RIGHT");
                    filter.ColumnParams.Add("PART_EXAM_OEND");
                    filter.ColumnParams.Add("PART_EXAM_DERMATOLOGY");
                    filter.ColumnParams.Add("PART_EXAM_EAR_RIGHT_NORMAL");
                    filter.ColumnParams.Add("PART_EXAM_EAR_RIGHT_WHISPER");
                    filter.ColumnParams.Add("PART_EXAM_EAR_LEFT_NORMAL");
                    filter.ColumnParams.Add("PART_EXAM_EAR_LEFT_WHISPER");
                    filter.ColumnParams.Add("PART_EXAM_UPPER_JAW");
                    filter.ColumnParams.Add("PART_EXAM_LOWER_JAW");
                    filter.ColumnParams.Add("PART_EXAM_HORIZONTAL_SIGHT");
                    filter.ColumnParams.Add("PART_EXAM_VERTICAL_SIGHT");
                    filter.ColumnParams.Add("PART_EXAM_EYE_BLIND_COLOR");
                    filter.ColumnParams.Add("PART_EXAM_HOLE_GLASS_LEFT");
                    filter.ColumnParams.Add("PART_EXAM_HOLE_GLASS_RIGHT");

                    filter.ColumnParams.Add("PART_EXAM_EYE_TENSION");
                    filter.ColumnParams.Add("PART_EXAM_EYE_ST_PLUS");
                    filter.ColumnParams.Add("PART_EXAM_EYE_ST_MINUS");
                    filter.ColumnParams.Add("PART_EXAM_EYE_COUNT_FINGER");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_SPH_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_SPH_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_CYL_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_CYL_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_AXE_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_AXE_RIGHT");
                    filter.ColumnParams.Add("PART_EYESIGHT_GLASS_OLD_LEFT");
                    filter.ColumnParams.Add("PART_EYESIGHT_GLASS_OLD_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_KCDT_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_KCDT_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_ADD_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_OLD_ADD_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_SPH_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_SPH_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_CYL_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_CYL_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_AXE_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_AXE_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_KCDT_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_KCDT_RIGHT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_ADD_LEFT");
                    filter.ColumnParams.Add("PART_EYE_GLASS_ADD_RIGHT");
                    if (HisConfigCFG.isDisablePartExamByExecutor)
                    {
                        filter.ColumnParams.Add("PAEX_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_CIRC_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_RESP_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_DIGE_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_KIDN_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_NEUR_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_MUSC_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_ENT_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_STOM_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_EYE_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_OEND_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_MENT_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_NUTR_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_MOTI_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_OBST_LOGINNAME");
                        filter.ColumnParams.Add("PAEX_DERM_LOGINNAME");
                    }
                    if (HisConfigCFG.AutoFinishAfterUnfinish == "1")
                    {
                        filter.ColumnParams.Add("IS_AUTO_FINISHED");
                    }
                }
                else
                {
                    filter.ColumnParams.Add("JSON_PRINT_ID");
                    filter.ColumnParams.Add("PTTT_APPROVAL_STT_ID");
                    filter.ColumnParams.Add("EKIP_PLAN_ID");
                    filter.ColumnParams.Add("RESULT_APPROVER_LOGINNAME");
                    filter.ColumnParams.Add("RESULT_APPROVER_USERNAME");
                }
                filter.ColumnParams.Add("TDL_SERVICE_IDS");
                filter.ID = serviceReqInput.ID;
                var results = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetViewDynamic", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (results != null && results.Count() > 0)
                {
                    result = results.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadModuleExecuteService(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                LogTheadInSessionInfo(() => LoadModuleExecuteService_ActionAsync(serviceReqInput), "ProcessServiceReq");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string ProcessDate(string date)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    if (date.Length == 4)
                    {
                        result = date;
                    }
                    else if (date.Length == 6)
                    {
                        result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 4))
                            .ToString();
                    }
                    else if (date.Length == 8)
                    {
                        result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 2))
                            .Append("/")
                            .Append(date.Substring(4, 4))
                            .ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            return result;
        }

        private async Task LoadModuleExecuteService_ActionAsync(L_HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    if (!string.IsNullOrEmpty(serviceReqInput.NOTE))
                    {
                        XtraMessageBox.Show(serviceReqInput.NOTE, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }

                    long dtNow = (long)Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    if (serviceReqInput.INTRUCTION_TIME != null && HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1" && dtNow < serviceReqInput.INTRUCTION_TIME)
                    {
                        MessageBox.Show("Thời gian bắt đầu không được nhỏ hơn thời gian y lệnh", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }
                    else if (serviceReqInput.INTRUCTION_TIME != null && HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "2" && serviceReqInput.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && dtNow < serviceReqInput.INTRUCTION_TIME)
                    {
                        MessageBox.Show("Thời gian bắt đầu không được nhỏ hơn thời gian y lệnh", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }

                    long dtFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                    long dtTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "232359");

                    if (serviceReqInput.INTRUCTION_TIME != null && dtFrom <= serviceReqInput.INTRUCTION_TIME && serviceReqInput.INTRUCTION_TIME <= dtTo)
                    {
                        bool serviceReqCount = false;
                        if (HisConfigCFG.RequestLimitWarningOption == "2")
                            serviceReqCount = LoadServiceReqCountByDesk();
                        else
                            serviceReqCount = LoadServiceReqCount();



                        //CommonParam param = new CommonParam();
                        //HisServiceReqViewFilter Filter = new HisServiceReqViewFilter();
                        //Filter.ID = serviceReqInput.ID;

                        //var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, Filter, param);

                        if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && serviceReqCount)
                        {
                            DialogResult myResult;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.WarningOverExam)), HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.WarningOverExam)));

                            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.WarningOverExam) == "1")
                            {
                                string WaringString = "";
                                if (HisConfigCFG.RequestLimitWarningOption == "2")
                                {
                                    if (desk != null && serviceReqInput.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                    {
                                        long deskCount = desk != null && desk.MAX_REQ_PER_DAY.HasValue ? desk.MAX_REQ_PER_DAY.Value : 0;
                                        WaringString = String.Format("Bàn đã khám quá {0} bệnh nhân trong ngày, bạn có muốn tiếp tục?", deskCount);
                                    }
                                }
                                else
                                {
                                    if (employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                                    {
                                        WaringString = "Đã khám đủ số lượng bn cho phép trong ngày, bạn có chắc chắn muốn tiếp tục xử lý hay không?";
                                    }
                                    else
                                    {
                                        WaringString = ResourceMessage.DaKhamDuSoLuongBNBHYTChoPhepTrongNgay;
                                    }
                                }
                                if (!String.IsNullOrWhiteSpace(WaringString))
                                {
                                    myResult = MessageBox.Show(WaringString, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (myResult == DialogResult.No)
                                    {
                                        return;
                                    }
                                }

                            }
                            //Bổ sung xử lý chặn ko cho người dùng vào màn hình xử lý khi key cấu hình WarningOverBHYT=2
                            if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.WarningOverExam) == "2")
                            {
                                string WaringString = "";
                                if (HisConfigCFG.RequestLimitWarningOption == "2")
                                {
                                    if (desk != null && serviceReqInput.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                    {
                                        long deskCount = desk != null && desk.MAX_REQ_PER_DAY.HasValue ? desk.MAX_REQ_PER_DAY.Value : 0;
                                        WaringString = String.Format("Bàn đã khám quá {0} bệnh nhân trong ngày", deskCount);
                                    }
                                }
                                else
                                {
                                    if (employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                                    {
                                        WaringString = "Bạn đã xử lý quá " + employee.MAX_SERVICE_REQ_PER_DAY + " yêu cầu trong ngày";
                                    }
                                    else
                                    {
                                        if (this.executeRoom.IS_EXAM != 1 || (this.executeRoom.IS_EXAM == 1 && serviceReqInput.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                            WaringString = "Bạn đã xử lý quá " + employee.MAX_BHYT_SERVICE_REQ_PER_DAY + " yêu cầu BHYT trong ngày";
                                    }
                                }
                                if (!String.IsNullOrWhiteSpace(WaringString))
                                {
                                    MessageBox.Show(WaringString, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                    }

                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.LockExecuteCFG) == "1" && serviceReqInput.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                    {
                        var services = listServices;
                        CommonParam param = new CommonParam();
                        List<ADOserserv7> sereServInServiceReqs = new List<ADOserserv7>();
                        if (this.SereServCurrentTreatment == null || this.SereServCurrentTreatment.Count == 0 || !this.SereServCurrentTreatment.Any(o => o.SERVICE_REQ_ID == serviceReqInput.ID))
                        {
                            HisSereServFilter sereServFilter = new HisSereServFilter();
                            sereServFilter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                            sereServFilter.ORDER_DIRECTION = "DESC";
                            sereServFilter.TREATMENT_ID = serviceReqInput.TREATMENT_ID;
                            //sereServFilter.SERVICE_REQ_ID = serviceReqInput.ID;
                            //sereServFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA;
                            sereServFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            this.SereServCurrentTreatment = await new BackendAdapter(param)
                                .GetAsync<List<ADOserserv7>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SereServCurrentTreatment), SereServCurrentTreatment));
                        sereServInServiceReqs = this.SereServCurrentTreatment != null ? this.SereServCurrentTreatment.Where(o => o.SERVICE_REQ_ID == serviceReqInput.ID && o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA).ToList() : null;

                        List<long> listId = sereServInServiceReqs != null ? sereServInServiceReqs.Select(o => o.ID).ToList() : null;

                        param = new CommonParam();
                        MOS.Filter.HisSereServExtFilter sereServExtFilter = new MOS.Filter.HisSereServExtFilter();
                        sereServExtFilter.SERE_SERV_IDs = listId;
                        sereServExtFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        var sereServExts = listId != null ? await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServExtFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null) : null;

                        List<string> serviceWarnMustHavePress = new List<string>();
                        foreach (var itemss in sereServInServiceReqs)
                        {
                            var itemssext = (sereServExts != null && sereServExts.Count > 0) ? sereServExts.Where(o => o.SERE_SERV_ID == itemss.ID).FirstOrDefault() : null;
                            if (itemssext == null || (itemssext != null && (itemssext.NUMBER_OF_FILM ?? 0) <= 0))
                            {
                                serviceWarnMustHavePress.Add(itemss.TDL_SERVICE_NAME);
                            }
                        }

                        if (serviceWarnMustHavePress.Count > 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaKeThuocVatTu, string.Join(",", serviceWarnMustHavePress)));
                            //Inventec.Common.Logging.LogSystem.Warn(string.Format(ResourceMessage.DichVuChuaKeThuocVatTu, string.Join(",", serviceWarnMustHavePress)) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServInServiceReqs), sereServInServiceReqs));
                            return;
                        }
                        Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceWarnMustHavePress), serviceWarnMustHavePress) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServInServiceReqs), sereServInServiceReqs));
                    }
                    Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 1");

                    if (HisConfigCFG.IsCheckHeinCard && !string.IsNullOrEmpty(serviceReqInput.TDL_HEIN_CARD_NUMBER) && serviceReqInput.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && serviceReqInput.START_TIME == null && serviceReqInput.CREATE_TIME < Int64.Parse(DateTime.Now.ToString("yyyyMMdd") + "000000"))
                    {
                        if (this.SereServCurrentTreatment == null || this.SereServCurrentTreatment.Count == 0 || !this.SereServCurrentTreatment.Any(o => o.SERVICE_REQ_ID == serviceReqInput.ID))
                        {
                            HisSereServFilter sereServFilter = new HisSereServFilter();
                            sereServFilter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                            sereServFilter.ORDER_DIRECTION = "DESC";
                            sereServFilter.TREATMENT_ID = serviceReqInput.TREATMENT_ID;
                            //sereServFilter.SERVICE_REQ_ID = serviceReqInput.ID;
                            //sereServFilter.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA;
                            sereServFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                            this.SereServCurrentTreatment = await new BackendAdapter(new CommonParam())
                                .GetAsync<List<ADOserserv7>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, null);
                        }
                        if (SereServCurrentTreatment != null && SereServCurrentTreatment.Count > 0 && SereServCurrentTreatment.Exists(o => o.SERVICE_REQ_ID == serviceReqInput.ID && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT))
                        {
                            HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.LoadConfig();
                            HIS.Desktop.Plugins.Library.RegisterConfig.BHXHLoginCFG.LoadConfig();
                            if (currentTreatment4 == null)
                                LoadTreatment4ByServiceReq();
                            if (currentTreatment4.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                            {
                                HeinCardData heinCardDataForCheckGOV = new HeinCardData();
                                heinCardDataForCheckGOV.PatientName = currentTreatment4.TDL_PATIENT_NAME;
                                heinCardDataForCheckGOV.Address = currentTreatment4.TDL_PATIENT_ADDRESS;
                                heinCardDataForCheckGOV.Dob = currentTreatment4.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? currentTreatment4.TDL_PATIENT_DOB.ToString().Substring(0, 4) : ProcessDate(currentTreatment4.TDL_PATIENT_DOB.ToString().Substring(6, 2) + currentTreatment4.TDL_PATIENT_DOB.ToString().Substring(4, 2) + currentTreatment4.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                                heinCardDataForCheckGOV.Gender = currentTreatment4.TDL_PATIENT_GENDER_NAME;

                                heinCardDataForCheckGOV.HeinCardNumber = currentTreatment4.TDL_HEIN_CARD_NUMBER;
                                heinCardDataForCheckGOV.FromDate = ProcessDate(currentTreatment4.TDL_HEIN_CARD_FROM_TIME.HasValue ? currentTreatment4.TDL_HEIN_CARD_FROM_TIME.ToString().Substring(6, 2) + currentTreatment4.TDL_HEIN_CARD_FROM_TIME.ToString().Substring(4, 2) + currentTreatment4.TDL_HEIN_CARD_FROM_TIME.ToString().Substring(0, 4) : null);
                                heinCardDataForCheckGOV.MediOrgCode = currentTreatment4.TDL_HEIN_MEDI_ORG_CODE;
                                HeinGOVManager heinGOVManager = new HeinGOVManager(null);
                                var ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, null, DateTime.Now, false, false);
                                if (ResultDataADO != null && (ResultDataADO.ResultHistoryLDO.maKetQua != "000" && ResultDataADO.ResultHistoryLDO.maKetQua != "003" && ResultDataADO.ResultHistoryLDO.maKetQua != "004") && !string.IsNullOrEmpty(ResultDataADO.ResultHistoryLDO.ghiChu))
                                {
                                    XtraMessageBox.Show(ResultDataADO.ResultHistoryLDO.ghiChu, "Thông báo");
                                    List<object> listArgs = new List<object>();
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CallPatientTypeAlter").FirstOrDefault();
                                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CallPatientTypeAlter");
                                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                    {
                                        PatientTypeDepartmentADO a = new PatientTypeDepartmentADO();

                                        a.patientTypeAlter = PatientTYpeAlterView(serviceReqInput);
                                        a.CREATE_TIME = a.patientTypeAlter.CREATE_TIME;
                                        a.LOG_TIME = a.patientTypeAlter.LOG_TIME;
                                        a.MODIFY_TIME = a.patientTypeAlter.MODIFY_TIME;
                                        a.TREATMENT_ID = a.patientTypeAlter.TREATMENT_ID;
                                        a.CREATOR = a.patientTypeAlter.CREATOR;
                                        a.MODIFIER = a.patientTypeAlter.MODIFIER;
                                        a.Id = a.patientTypeAlter.DEPARTMENT_TRAN_ID.ToString();

                                        listArgs.Add(a);
                                        listArgs.Add(this.currentModule);
                                        listArgs.Add(true);
                                        listArgs.Add((RefeshReference)ReloadData);
                                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                        ((Form)extenceInstance).ShowDialog();
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 2");
                        //Bắt đầu
                        bool isStart = false;
                        StartEvent(ref isStart, serviceReqInput);
                        if (isStart == false)
                            return;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 3");

                    V_HIS_SERVICE_REQ serviceReqDynamic = GetDynamicData(serviceReqInput);

                    //Get module link
                    if (serviceReqInput.EXE_SERVICE_MODULE_ID.HasValue)
                    {
                        List<HIS_EXE_SERVICE_MODULE> exeServiceModules = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXE_SERVICE_MODULE>();
                        HIS_EXE_SERVICE_MODULE exeServiceModule = exeServiceModules != null && exeServiceModules.Count > 0 ?
                            exeServiceModules.FirstOrDefault(o => o.ID == serviceReqInput.EXE_SERVICE_MODULE_ID.Value) : null;
                        if (exeServiceModule == null)
                        {
                            MessageBox.Show("Không tìm thấy module!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeServiceModules), exeServiceModules));
                            return;
                        }
                        Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 4");
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                               .Where(o => o.ModuleLink == exeServiceModule.MODULE_LINK).FirstOrDefault();
                        if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + exeServiceModule.MODULE_LINK);
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                            currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId);
                            string dob = serviceReqDynamic.TDL_PATIENT_DOB > 0 ? serviceReqDynamic.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReqDynamic.TDL_PATIENT_DOB);
                            //Neu la xu ly kham can phai reload thong tin module
                            if (exeServiceModule.MODULE_LINK == ModuleLink.MODULE_LINK__CDHA_TDCN_NS_SA_GPBL)
                            {
                                ServiceExecuteADO serviceExecute = new ServiceExecuteADO(serviceReqDynamic, ReLoadExecuteRoom);
                                listArgs.Add(serviceExecute);

                                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReqDynamic.SERVICE_REQ_CODE, serviceReqDynamic.SERVICE_REQ_CODE + " - " + serviceReqDynamic.TDL_PATIENT_NAME + " - " + dob + " - " + serviceReqDynamic.TDL_PATIENT_GENDER_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule, SaveDataBeforeCloseV2);
                            }
                            else if (exeServiceModule.MODULE_LINK == ModuleLink.MODULE_LINK__XU_LY_KHAM)
                            {

                                Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 4.1");
                                listArgs.Add(serviceReqDynamic);
                                listArgs.Add(SereServCurrentTreatment);
                                listArgs.Add((DelegateSelectData)ReLoadServiceReq);

                                object extenceInstance = null;
                                string pageName = "";
                                //var pluginInstanceADO = PluginInstanceInitWorker.GetAvailaiblePlugin(ModuleLink.MODULE_LINK__XU_LY_KHAM);
                                //if (pluginInstanceADO != null && pluginInstanceADO.PluginInstance != null)
                                //{
                                //    Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 4.2");
                                //    extenceInstance = pluginInstanceADO.PluginInstance;
                                //    HIS.Desktop.Utility.PluginInstance.InitLoadPluginWithExistsInstance(extenceInstance, currentModule, listArgs);
                                //    pageName = pluginInstanceADO.PluginName + "__" + serviceReq.SERVICE_REQ_CODE + "__AvailaiblePlugin";

                                //    PluginInstanceInitWorker.UpdatePlugin(pluginInstanceADO.PluginName, extenceInstance, true);

                                //    HIS.Desktop.ModuleExt.TabControlBaseProcess.OpenPluginPage(SessionManager.GetTabControlMain(), currentModule, serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME, pluginInstanceADO.PluginName);
                                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pageName), pageName));
                                //}
                                //else
                                //{
                                Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 4.3");
                                extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                                pageName = currentModule.ExtensionInfo.Code + serviceReqDynamic.SERVICE_REQ_CODE;
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 4.4");
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pageName), pageName));
                                HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), pageName, serviceReqDynamic.SERVICE_REQ_CODE + " - " + serviceReqDynamic.TDL_PATIENT_NAME + " - " + dob + " - " + serviceReqDynamic.TDL_PATIENT_GENDER_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule, SaveDataBeforeClose);
                                //}                           

                            }
                            else
                            {
                                listArgs.Add(serviceReqDynamic);
                                listArgs.Add((DelegateSelectData)ReLoadServiceReq);
                                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReqDynamic.SERVICE_REQ_CODE, serviceReqDynamic.SERVICE_REQ_CODE + " - " + serviceReqDynamic.TDL_PATIENT_NAME + " - " + dob + " - " + serviceReqDynamic.TDL_PATIENT_GENDER_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Chưa gán Module Link tương ứng với yêu cầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("LoadModuleExecuteService. 5");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_PATIENT_TYPE_ALTER PatientTYpeAlterView(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                CommonParam param = new Inventec.Core.CommonParam();
                HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                filterPatienTypeAlter.TREATMENT_ID = serviceReq.TREATMENT_ID;
                return new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.ID).ThenByDescending(o => o.LOG_TIME).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
        private void ReloadData()
        {
            LoadTreatment4ByServiceReq();
            btnFind_Click(null, null);
        }

        //private void LoadModuleExecuteService__Old(string moduleLink, L_HIS_SERVICE_REQ serviceReqInput)
        //{
        //    try
        //    {
        //        Inventec.Common.Logging.LogSystem.Info("Start Load Data Module");
        //        if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
        //        {
        //            //Hủy kết thúc
        //            //CancelFinish(serviceReqInput);
        //            //SetTextButtonExecute(serviceReqInput);
        //            //return;
        //        }
        //        else if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
        //        {
        //            //Bắt đầu
        //            bool isStart = false;
        //            StartEvent(ref isStart, serviceReqInput);
        //            if (isStart == false)
        //                return;
        //        }

        //        Inventec.Common.Logging.LogSystem.Info("Finish Load Data Module");

        //        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
        //           .Where(o => o.ModuleLink == moduleLink).FirstOrDefault();
        //        if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + moduleLink);
        //        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
        //        {
        //            List<object> listArgs = new List<object>();
        //            Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
        //            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>();
        //            V_HIS_SERVICE_REQ serviceReq = AutoMapper.Mapper.Map<L_HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>(serviceReqInput);
        //            ServiceExecuteADO serviceExecute = new ServiceExecuteADO(serviceReq, ReLoadExecuteRoom);
        //            listArgs.Add(serviceExecute);
        //            currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId);
        //            listArgs.Add(serviceReq);
        //            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
        //            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
        //            HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE, serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);

        //            //}
        //        }

        //        Inventec.Common.Logging.LogSystem.Info("Finish Load Module");

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        void SaveDataBeforeClose(object uc)
        {
            try
            {
                MethodInfo methodInfo = (uc as UserControl).GetType().GetMethod("SaveShortCut");
                methodInfo.Invoke(uc, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SaveDataBeforeCloseV2(object uc)
        {
            try
            {
                HIS.Desktop.ModuleExt.TabControlBaseProcess.CloseCameraFormOpened();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadExecuteRoom(V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                btnFind_Click(null, null);

                //if (serviceReq != null)
                //{
                //    foreach (var item in serviceReqs)
                //    {
                //        if (item.ID == serviceReq.ID)
                //        {
                //            item.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                //        }
                //    }
                //    gridControlServiceReq.RefreshDataSource();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitEnableControl()
        {
            try
            {
                var executeRooms = lstExecuteRoom.Where(o => o.IS_EMERGENCY == 1).FirstOrDefault();
                //Phong cap cuu hien thi nut yeu cau tam ung
                if (executeRooms != null)
                    lciDepositReq.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                if (currentHisServiceReq == null)
                {
                    btnExecute.Enabled = false;
                    btnBordereau.Enabled = false;
                    btnRoomTran.Enabled = false;
                    btnDepositReq.Enabled = false;
                    btnTreatmentHistory.Enabled = false;
                    btnUnStart.Enabled = false;
                    btnServiceReqList.Enabled = false;
                }
                else
                {
                    if (currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                    //&& (sereServ7s == null || sereServ7s.Count == 0))
                    {
                        btnUnStart.Enabled = true;
                    }
                    else
                    {
                        btnUnStart.Enabled = false;
                    }

                    btnRoomTran.Enabled = true;
                    btnExecute.Enabled = true;
                    btnBordereau.Enabled = true;
                    btnDepositReq.Enabled = true;
                    btnTreatmentHistory.Enabled = true;
                    btnServiceReqList.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTextButtonExecute(L_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        btnExecute.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecuteHT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else
                    {
                        btnExecute.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecute.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load trạng thái: Đang xử lý, tất cả,...
        /// </summary>
        public void InitComboBoxEditStatus()
        {
            try
            {
                cboFind.Properties.Items.AddRange(new object[] {
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.NotFinished", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.All", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.NotYetProcessed", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.Processing", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.Finish", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.MissedCall", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture())});
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadActionButtonRefesh(bool isBtnRefesh)
        {
            try
            {
                if (isBtnRefesh)
                {
                    btnRefesh.Image = imageListRefesh.Images[1];
                    btnRefesh.Tag = EnumUtil.REFESH_ENUM.ON;
                }
                else
                {
                    btnRefesh.Image = imageListRefesh.Images[0];
                    btnRefesh.Tag = EnumUtil.REFESH_ENUM.OFF;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        long serviceReqCount = 0;
        HIS_EMPLOYEE employee { get; set; }
        HIS_DESK desk { get; set; }
        public void LoadServiceReqCount(bool isReLoad, int d)
        {
            try
            {
                if (HisConfigCFG.RequestLimitWarningOption == "2")
                {
                    if (desk == null)
                    {
                        var currentRoom = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.roomId);

                        List<HIS_DESK> deskAllList = deskList;
                        if (deskAllList == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach HIS_DESK tu backenddata");
                            return;
                        }
                        if (currentRoom == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong tim thay phong lam viec");
                            return;
                        }
                        desk = deskAllList.FirstOrDefault(o => o.ID == currentRoom.DeskId);
                        if (desk == null || desk.ID <= 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("desk is NULL");
                            return;
                        }
                    }

                    if (isReLoad)
                    {
                        CommonParam param = new CommonParam();
                        HisServiceReqCountFilter filter = new HisServiceReqCountFilter();
                        filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                        filter.IS_BHYT = true;
                        filter.EXE_DESK_ID = desk.ID;
                        filter.SERVICE_REQ_STT_IDs = new List<long>(){ IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };


                        var executeRoom = lstExecuteRoom.FirstOrDefault(o => o.ROOM_ID == roomId);
                        if (executeRoom.IS_EXAM == 1)
                        {
                            filter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH, };
                        }
                        else
                        {
                            filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                            {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN,
                            };
                        }


                        serviceReqCount = new BackendAdapter(param)
                           .Get<long>("api/HisServiceReq/GetCount", ApiConsumers.MosConsumer, filter, param);
                    }

                    if (this.currentHisServiceReq != null && this.currentHisServiceReq.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT &&
                        (this.executeRoom.IS_EXAM != 1 || (this.executeRoom.IS_EXAM == 1 && this.currentHisServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)))
                    {
                        serviceReqCount += d;
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCount), serviceReqCount));
                    lciRequestAndMaxRequest.Text = "SL yêu cầu bàn đã xử lý/SL tối đa: ";
                    lblServiceReqCount.Text = serviceReqCount + "/" + desk.MAX_REQ_PER_DAY;
                    lblServiceReqCount.ToolTip = "Số lượng yêu cầu bàn đã xử lý/Số lượng tối đa";
                    lblServiceReqCount.Appearance.ForeColor = new Color();

                    if (desk.MAX_REQ_PER_DAY.HasValue)
                    {
                        //lblServiceReqCount.Text = lblServiceReqCount.Text + "/" + desk.MAX_REQ_PER_DAY;

                        if (serviceReqCount > desk.MAX_REQ_PER_DAY)
                        {
                            lblServiceReqCount.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
                else
                {

                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                    if (employees == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach nhan vien tu backenddata");
                        return;
                    }

                    employee = employees.FirstOrDefault(o => o.LOGINNAME == loginName);
                    if (employee == null || employee.ID <= 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Tai khoan hien tai khong phai la nhan vien");
                        return;
                    }
                    if (isReLoad)
                    {

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => employee), employee));
                        CommonParam param = new CommonParam();
                        HisServiceReqCountFilter filter = new HisServiceReqCountFilter();
                        //Sửa lại yêu cầu chỉ lấy theo ngày hiện tại
                        filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");

                        var executeRoom = lstExecuteRoom.FirstOrDefault(o => o.ROOM_ID == roomId);

                        filter.EXECUTE_LOGINNAME = loginName;

                        if (employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue && !employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                            filter.IS_BHYT = true;
                        if (executeRoom.IS_EXAM == 1 || employee.IS_SERVICE_REQ_EXAM == 1)
                        {
                            filter.SERVICE_REQ_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH, };
                        }
                        else
                        {
                            filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN,
                        };
                        }



                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                        serviceReqCount = new BackendAdapter(param)
                           .Get<long>("api/HisServiceReq/GetCount", ApiConsumers.MosConsumer, filter, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCount), serviceReqCount));
                        //Set canh bao
                    }



                    if (this.currentHisServiceReq != null && this.currentHisServiceReq.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue &&
                    (this.executeRoom.IS_EXAM != 1 || (this.executeRoom.IS_EXAM == 1 && this.currentHisServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)))
                    {
                        serviceReqCount += d;
                    }
                    if (this.currentHisServiceReq != null && employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                    {
                        serviceReqCount += d;
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCount), serviceReqCount));

                    lblServiceReqCount.Text = serviceReqCount + "";
                    lblServiceReqCount.ToolTip = employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue ? String.Format(ResourceMessage.SoLuongBhytTrongNgay, serviceReqCount) : String.Format(ResourceMessage.SoLuongTrongNgay, serviceReqCount);
                    lblServiceReqCount.Appearance.ForeColor = new Color();

                    long NumberExamBhytInDay = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.NUMBER_EXAM_BHYT_IN_DAY));

                    lciRequestAndMaxRequest.Text = employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue ? Inventec.Common.Resource.Get.Value("UCExecuteRoom.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()) : Inventec.Common.Resource.Get.Value("UCExecuteRoom.MAX_BHYT_SERVICE_REQ_PER_DAY.NotValue.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    if (NumberExamBhytInDay == 1)
                    {
                        lblServiceReqCount.Text = serviceReqCount + "/" + (employee.MAX_BHYT_SERVICE_REQ_PER_DAY ?? employee.MAX_SERVICE_REQ_PER_DAY);
                    }
                    else if (NumberExamBhytInDay == 2)
                    {
                        serviceReqCount = 0;

                        CommonParam param1 = new CommonParam();
                        HisTreatmentFilter TreatmentFilter = new HisTreatmentFilter();
                        TreatmentFilter.IS_PAUSE = true;
                        TreatmentFilter.DOCTOR_LOGINNAME__EXACT = loginName;
                        TreatmentFilter.OUT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                        TreatmentFilter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;

                        var totalServiceReqCount = new BackendAdapter(param1).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, TreatmentFilter, param1);
                        Inventec.Common.Logging.LogSystem.Warn("Luot Kham ket thuc dieu tri: " + serviceReqCount);
                        if (totalServiceReqCount != null && employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue)
                        {
                            serviceReqCount = totalServiceReqCount.Count(o => o.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT);
                        }
                        else if (totalServiceReqCount != null && employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                        {
                            serviceReqCount = totalServiceReqCount.Count();
                        }
                        lblServiceReqCount.Text = serviceReqCount + "/" + (employee.MAX_BHYT_SERVICE_REQ_PER_DAY ?? employee.MAX_SERVICE_REQ_PER_DAY);
                    }

                    lblServiceReqCount.ToolTip = Inventec.Common.Resource.Get.Value("MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    lblServiceReqCount.Appearance.ForeColor = new Color();

                    if (employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue)
                    {
                        //lblServiceReqCount.Text = lblServiceReqCount.Text + "/" + employee.MAX_BHYT_SERVICE_REQ_PER_DAY;
                        if (serviceReqCount > employee.MAX_BHYT_SERVICE_REQ_PER_DAY)
                        {
                            lblServiceReqCount.Appearance.ForeColor = Color.Red;
                        }
                    }
                    else if (employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                    {
                        //lblServiceReqCount.Text = lblServiceReqCount.Text + "/" + employee.MAX_BHYT_SERVICE_REQ_PER_DAY;
                        if (serviceReqCount > employee.MAX_SERVICE_REQ_PER_DAY)
                        {
                            lblServiceReqCount.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool LoadServiceReqCount()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                if (employees == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach nhan vien tu backenddata");
                    return result;
                }
                employee = employees.FirstOrDefault(o => o.LOGINNAME == loginName);
                if (employee == null || employee.ID <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Tai khoan hien tai khong phai la nhan vien");
                    return result;
                }
                if (this.currentHisServiceReq != null && this.currentHisServiceReq.TDL_PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue)
                {
                    return result;
                }
                if (this.currentHisServiceReq != null && employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue && this.executeRoom.IS_EXAM == 1 && this.currentHisServiceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                {
                    return result;
                }
                if ((employee.IS_SERVICE_REQ_EXAM == 1 || executeRoom.IS_EXAM == 1) && this.currentHisServiceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                {
                    return result;
                }
                Inventec.Common.Logging.LogSystem.Debug("Service Req Count" + serviceReqCount);
                //Set canh bao

                if (employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue)
                {
                    if (serviceReqCount + 1 > employee.MAX_BHYT_SERVICE_REQ_PER_DAY)
                    {
                        result = true;
                    }
                }
                else if (employee.MAX_SERVICE_REQ_PER_DAY.HasValue)
                {
                    if (serviceReqCount + 1 > employee.MAX_SERVICE_REQ_PER_DAY)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public async Task LoadSereServCount()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServCountFilter filter = new HisSereServCountFilter();
                filter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                filter.EXECUTE_ROOM_ID = this.roomId;
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("api/HisSereServ/GetCount INPUT HisSereServCountFilter ", filter));
                var sereServCount = await new BackendAdapter(param)
                      .GetAsync<HisSereServCountSDO>("api/HisSereServ/GetCount", ApiConsumers.MosConsumer, filter, param);
                if (sereServCount != null)
                {
                    lblSereServCount.Text = String.Format("{0}/{1}", sereServCount.TotalProcess, sereServCount.TotalAssign);
                }
                else
                {
                    lblSereServCount.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool LoadServiceReqCountByDesk()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                var currentRoom = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId);

                var desk = currentRoom != null ? deskList.FirstOrDefault(o => o.ID == currentRoom.DeskId) : null;
                if (desk == null)
                {
                    return result;
                }
                //HisServiceReqCountFilter filter = new HisServiceReqCountFilter();
                //filter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                //filter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "232359");
                ////filter.IS_BHYT = true;
                //filter.EXE_DESK_ID = desk.ID;
                //filter.SERVICE_REQ_STT_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                //    , IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };
                //serviceReqCount = new BackendAdapter(param)
                //   .Get<long>("api/HisServiceReq/GetCount", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug("Service Req Count" + serviceReqCount);
                //Set canh bao
                if (deskList == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach HIS_DESK tu backenddata");
                    return result;
                }
                if (desk == null || desk.ID <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadServiceReqCountByDesk desk is NULL");
                    return result;
                }
                if (desk.MAX_REQ_PER_DAY.HasValue)
                {
                    if (serviceReqCount + 1 > desk.MAX_REQ_PER_DAY)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessLoadDocumentBySereServ(V_HIS_SERE_SERV_7 data)
        {
            try
            {
                if (LciGroupEmrDocument.Expanded)
                {
                    Inventec.Common.Logging.LogSystem.Info("ProcessLoadDocumentBySereServ");
                    WaitingManager.Show();
                    if (!HisConfigCFG.IsHasConnectionEmr)
                        return;
                    List<EmrDocumentFileSDO> documents = new List<EmrDocumentFileSDO>();
                    if (data != null)
                    {
                        string hisCode = "SERVICE_REQ_CODE:" + data.TDL_SERVICE_REQ_CODE;
                        CommonParam paramCommon = new CommonParam();
                        EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
                        var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                        emrFilter.TREATMENT_CODE__EXACT = data.TDL_TREATMENT_CODE;
                        emrFilter.IS_DELETE = false;
                        if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
                        {
                            emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
                        }
                        else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
                        {
                            emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                        }
                        sdo.EmrDocumentViewFilter = emrFilter;
                        sdo.HisCode = hisCode;
                        sdo.IsMerge = true;
                        sdo.IsShowPatientSign = false;
                        sdo.IsShowWatermark = false;
                        sdo.RoomCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).ROOM_CODE;
                        sdo.DepartmentCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).DEPARTMENT_CODE;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                        documents = new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramCommon), paramCommon));
                        Inventec.Common.Logging.LogSystem.Debug("document__" + (documents != null && documents.Count > 0));
                    }

                    if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
                    {
                        this.ucViewEmrDocumentReq.ReloadDocument(documents);
                    }
                    else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
                    {
                        this.ucViewEmrDocumentResult.ReloadDocument(documents);
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
