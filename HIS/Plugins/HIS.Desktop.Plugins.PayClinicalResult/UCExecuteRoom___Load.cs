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
using HIS.UC.TreeSereServ7;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.ADO;
using System.Reflection;
using HIS.Desktop.Plugins.PayClinicalResult.Base;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.LocalStorage.BackendData;
using System.IO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        int dem = 0;

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
            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridServiceReq, param, numPageSize, gridControlServiceReq);
        }

        internal void FillDataToGridServiceReq(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<HIS_SERVICE_REQ>> apiResult = new ApiResultObject<List<HIS_SERVICE_REQ>>();
                MOS.Filter.HisServiceReqFilter hisServiceReqFilter = new HisServiceReqFilter();
                //hisServiceReqFilter.EXECUTE_ROOM_ID = roomId;//TODO
                hisServiceReqFilter.EXECUTE_DEPARTMENT_ID = this.currentDepartment.ID;
                hisServiceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN, 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA};

                hisServiceReqFilter.HAS_EXECUTE = true;
                hisServiceReqFilter.KEYWORD__SERVICE_REQ_CODE__TREATMENT_CODE__PATIENT_NAME__PATIENT_CODE = txtSearchKey.Text.Trim();
                if (dtCreatefrom != null && dtCreatefrom.DateTime != DateTime.MinValue)
                    hisServiceReqFilter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreatefrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtCreateTo != null && dtCreateTo.DateTime != DateTime.MinValue)
                    hisServiceReqFilter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMddHHmm") + "59");

                if (!String.IsNullOrEmpty(txtServiceReqCodeSearch.Text))
                {
                    hisServiceReqFilter.SERVICE_REQ_CODE__EXACT = txtServiceReqCodeSearch.Text;
                    string str = string.Format("{0:000000000000}", Convert.ToInt64(this.txtServiceReqCodeSearch.Text));
                    this.txtServiceReqCodeSearch.Text = str;
                    hisServiceReqFilter.SERVICE_REQ_CODE__EXACT = this.txtServiceReqCodeSearch.Text;
                }

                List<long> lstServiceReqSTT = new List<long>();

                //Đang xử lý 0
                //Kết thúc 1
                //Tất cả 2 (kết thúc và đang xử lý)
                //Filter yeu cau chua ket thuc
                if (cboFind.EditValue != null)
                {

                    //Tất cả
                    if (cboFind.SelectedIndex == 0)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL);
                    }
                    //Đang xử lý
                    else if (cboFind.SelectedIndex == 1)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT);
                    }
                    //Kết thúc
                    else if (cboFind.SelectedIndex == 2)
                    {
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL);
                        lstServiceReqSTT.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT);
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


                if (lstServiceReqSTT.Count > 0)
                    hisServiceReqFilter.SERVICE_REQ_STT_IDs = lstServiceReqSTT;

                hisServiceReqFilter.ORDER_FIELD = "INTRUCTION_DATE";
                hisServiceReqFilter.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                hisServiceReqFilter.ORDER_FIELD2 = "PRIORITY";
                hisServiceReqFilter.ORDER_FIELD3 = "NUM_ORDER";

                hisServiceReqFilter.ORDER_DIRECTION = "DESC";
                hisServiceReqFilter.ORDER_DIRECTION1 = "ASC";
                hisServiceReqFilter.ORDER_DIRECTION2 = "DESC";
                hisServiceReqFilter.ORDER_DIRECTION3 = "ASC";

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MUST_BE_APPROVED_BEFORE_PROCESS_SURGERY") == "1")
                    hisServiceReqFilter.IS_NOT_SURGERY__OR__IS__APPROVED__OR__IS_EMERGENCY = true;

                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, hisServiceReqFilter, paramCommon);

                gridControlServiceReq.DataSource = null;

                if (apiResult != null)
                {
                    serviceReqs = (List<HIS_SERVICE_REQ>)apiResult.Data;
                    rowCount = (serviceReqs == null ? 0 : serviceReqs.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    if (rowCount > 0)
                    {
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

                maxTimeReload = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.MAX_TIME_RELOAD));
                lblAutoReload.Text = "";
                if ((EnumUtil.REFESH_ENUM)btnRefesh.Tag == EnumUtil.REFESH_ENUM.OFF && maxTimeReload > 0)
                {
                    timeCount = 0;
                    timerAutoReload.Start();
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

        private void InitTreeSereServ()
        {
            try
            {
                ssTreeProcessor = new TreeSereServ7Processor();
                TreeSereServ7ADO ado = new TreeSereServ7ADO();
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                ado.TreeSereServ7_GetStateImage = treeSereServ_GetStateImage;
                ado.TreeSereServ7_StateImageClick = treeSereServ_StateImageClick;
                ado.TreeSereServ7_GetSelectImage = treeSereServ_GetSelectImage;
                ado.TreeSereServ7_CustomNodeCellEdit = treeSereServ_CustomNodeCellEdit;
                ado.SereServNodeCellStyle = treeSereServ_NodeCellStyle;
                ado.TreeSereServ7_DoubleClick = treeSereServ_DoubleClick;
                ado.TreeSereServ7_CustomUnboundColumnData = treeSereServ_CustomUnboundColumnData;
                ado.IsShowSearchPanel = false;
                ado.TreeSereServ7Columns = new List<TreeSereServ7Column>();
                //ado.TreeSereServ7_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;

                //Gửi lại yêu cầu xét nghiệm
                TreeSereServ7Column colSendTestServiceReq = new TreeSereServ7Column("   ", "SendTestServiceReq", 30, true);
                colSendTestServiceReq.VisibleIndex = 1;
                ado.TreeSereServ7Columns.Add(colSendTestServiceReq);

                //Column mã dịch vụ
                TreeSereServ7Column serviceCodeCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 2;
                ado.TreeSereServ7Columns.Add(serviceCodeCol);

                //Column tên dịch vụ
                TreeSereServ7Column serviceNameCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 250, false);
                serviceNameCol.VisibleIndex = 3;
                ado.TreeSereServ7Columns.Add(serviceNameCol);

                //Column mã yêu cầu
                TreeSereServ7Column serviceReqCodeCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_EXECUTE_ROOM__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 4;
                ado.TreeSereServ7Columns.Add(serviceReqCodeCol);

                //Column ghi chú
                TreeSereServ7Column noteCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__COLUMN_NOTE", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "NOTE_ADO", 200, false);
                noteCol.VisibleIndex = 5;
                ado.TreeSereServ7Columns.Add(noteCol);

                //Column ghi chú
                TreeSereServ7Column departmentCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__REQUEST_DEPARTMENT_NAME", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REQUEST_DEPARTMENT_NAME", 250, false);
                departmentCol.VisibleIndex = 6;
                ado.TreeSereServ7Columns.Add(departmentCol);

                TreeSereServ7Column soPhieuCol = new TreeSereServ7Column(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_BED_ROOM_PARTIAL__TREE_SERE_SERV__REQUEST_SOPHIEU", Base.ResourceLangManager.LanguageUCExecuteRoom, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SO_PHIEU", 250, false);
                soPhieuCol.VisibleIndex = 7;
                ado.TreeSereServ7Columns.Add(soPhieuCol);

                //Khoa hien tai
                ado.DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId).DepartmentId;
                this.ucTreeSereServ7 = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucTreeSereServ7 != null)
                {
                    this.xtraScrollableTreeSereServ.Controls.Add(this.ucTreeSereServ7);
                    this.ucTreeSereServ7.Dock = DockStyle.Fill;
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
                departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
                rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
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
                            this.PacsCallModuleProccess(sereServ6);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientFromServiceReq(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (serviceReq != null)
                {
                    lblPatientCode.Text = serviceReq.TDL_PATIENT_CODE;
                    lblPatientName.Text = serviceReq.TDL_PATIENT_NAME;
                    lblGender.Text = serviceReq.TDL_PATIENT_GENDER_NAME;
                    lblDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.TDL_PATIENT_DOB);
                    lblAddress.Text = serviceReq.TDL_PATIENT_ADDRESS;
                    // lblCardNumber.Text = serviceReq.TDL_HEIN_CARD_NUMBER;
                    //lblKCBBD.Text = serviceReq.TDL_HEIN_MEDI_ORG_CODE;


                    HisPatientTypeAlterViewFilter filterPatienTypeAlter = new HisPatientTypeAlterViewFilter();
                    filterPatienTypeAlter.TREATMENT_ID = serviceReq.TREATMENT_ID;
                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filterPatienTypeAlter, param).OrderByDescending(o => o.ID).FirstOrDefault();

                    //Mức hưởng BHYT

                    if (patientTypeAlter != null &&
                        !String.IsNullOrEmpty(patientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        lblCardNumber.Text = patientTypeAlter.HEIN_CARD_NUMBER;
                        lblKCBBD.Text = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                        decimal ratio = 0;
                        ratio = GetDefaultHeinRatio(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.TREATMENT_TYPE_CODE, patientTypeAlter.LEVEL_CODE, patientTypeAlter.RIGHT_ROUTE_CODE);
                        lblRatio.Text = ratio * 100 + " %";
                    }
                    else
                    {
                        lblKCBBD.Text = "";
                        lblRatio.Text = "";
                        lblCardNumber.Text = "";
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
                    lblRatio.Text = "";
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

        private void LoadTreeListSereServChild(HIS_SERVICE_REQ serviceReq)
        {
            try
            {

                if (ucTreeSereServ7 != null && SereServCurrentTreatment != null)
                {
                    if (serviceReq != null
                        && serviceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        sereServ7s = new List<V_HIS_SERE_SERV_7>();

                        List<HIS_SERVICE_REQ> serviceReqChilds = ServiceReqCurrentTreatment.Where(o => o.PARENT_ID == serviceReq.ID).ToList();
                        if (serviceReqChilds != null && serviceReqChilds.Count > 0)
                        {

                            List<HIS_SERE_SERV> sereServParentServiceReq = SereServCurrentTreatment.Where(o => serviceReqChilds.Select(p => p.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList();

                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERE_SERV, V_HIS_SERE_SERV_7>();
                            sereServ7s = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>, List<V_HIS_SERE_SERV_7>>(sereServParentServiceReq);

                            List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                            List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();
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

                                HIS_SERVICE_REQ serviceReqItem = ServiceReqCurrentTreatment != null ? ServiceReqCurrentTreatment.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID) : null;
                                if (serviceReqItem != null)
                                {
                                    item.SERVICE_REQ_STT_ID = serviceReqItem.SERVICE_REQ_STT_ID;
                                }
                            }
                        }
                        //}
                    }
                    //V_HIS_SERE_SERV_6 sereServ6 = (sereServ6s != null && sereServ6s.Count > 0) ? sereServ6s[0] : null;
                    //LoadTreeSereServ7(sereServ6);
                    ssTreeProcessor.Reload(ucTreeSereServ7, sereServ7s);
                    WaitingManager.Hide();
                }
                else
                {
                    ssTreeProcessor.Reload(ucTreeSereServ7, new List<V_HIS_SERE_SERV_7>());
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreeSereServ7(V_HIS_SERE_SERV_6 sereServ6)
        {
            try
            {
                if (sereServ6 != null && this.sereServ7s != null && this.sereServ7s.Count > 0)
                {
                    List<V_HIS_SERE_SERV_7> sereServChilds = sereServ7s.Where(o => o.PARENT_ID == sereServ6.ID).ToList();
                    ssTreeProcessor.Reload(ucTreeSereServ7, sereServChilds);
                }
                else
                {
                    ssTreeProcessor.Reload(ucTreeSereServ7, new List<V_HIS_SERE_SERV_7>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadSereServServiceReq(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                gridControlSereServServiceReq.DataSource = null;
                sereServ6s = new List<V_HIS_SERE_SERV_6>();
                if (serviceReq != null && SereServCurrentTreatment != null)
                {
                    List<HIS_SERE_SERV> sereServByServiceReqs = SereServCurrentTreatment.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).OrderBy(o => o.IS_NO_EXECUTE ?? 0).ToList();
                    if (sereServByServiceReqs != null && sereServByServiceReqs.Count > 0)
                    {
                        foreach (var item in sereServByServiceReqs)
                        {
                            V_HIS_SERE_SERV_6 ss = new V_HIS_SERE_SERV_6();
                            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_6>(ss, item);
                            HIS_SERVICE_UNIT serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            if (serviceUnit != null)
                            {
                                ss.SERVICE_UNIT_CODE = serviceUnit.SERVICE_UNIT_CODE;
                                ss.SERVICE_UNIT_NAME = serviceUnit.SERVICE_UNIT_NAME;
                            }

                            HIS_SERVICE_TYPE serviceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                                .FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID);
                            if (serviceType != null)
                            {
                                ss.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                                ss.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                            }
                            sereServ6s.Add(ss);
                        }
                    }
                    gridControlSereServServiceReq.DataSource = sereServ6s;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void LoadDataToPanelRight(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                WaitingManager.Show();
                List<Action> methods = new List<Action>();
                methods.Add(LoadSereServByTreatment);
                methods.Add(LoadServiceReqByTreatment);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                WaitingManager.Hide();
                LoadPatientFromServiceReq(serviceReq);
                LoadSereServServiceReq(serviceReq);
                LoadTreeListSereServChild(serviceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadModuleExecuteService(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    //Bắt đầu
                    bool isStart = false;
                    StartEvent(ref isStart, serviceReqInput);
                    if (isStart == false)
                        return;
                }

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

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == exeServiceModule.MODULE_LINK).FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + exeServiceModule.MODULE_LINK);
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        V_HIS_SERVICE_REQ serviceReq = new V_HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(serviceReq, serviceReqInput);
                        currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId);

                        //Neu la xu ly kham can phai reload thong tin module
                        if (exeServiceModule.MODULE_LINK == ModuleLink.MODULE_LINK__CDHA_TDCN_NS_SA_GPBL)
                        {
                            ServiceExecuteADO serviceExecute = new ServiceExecuteADO(serviceReq, ReLoadExecuteRoom);
                            serviceExecute.IsReadResult = true;
                            listArgs.Add(serviceExecute);

                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE, serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule, SaveDataBeforeCloseV2);
                        }
                        else if (exeServiceModule.MODULE_LINK == ModuleLink.MODULE_LINK__XU_LY_KHAM)
                        {
                            listArgs.Add(serviceReq);
                            listArgs.Add((DelegateSelectData)ReLoadServiceReq);

                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE, serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule, SaveDataBeforeClose);
                        }
                        else
                        {
                            listArgs.Add(serviceReq);
                            listArgs.Add((DelegateSelectData)ReLoadServiceReq);
                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE, serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Chưa gán Module Link tương ứng với yêu cầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadModuleExecuteService__Old(string moduleLink, HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Start Load Data Module");
                if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    //Hủy kết thúc
                    //CancelFinish(serviceReqInput);
                    //SetTextButtonExecute(serviceReqInput);
                    //return;
                }
                else if (serviceReqInput.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    //Bắt đầu
                    bool isStart = false;
                    StartEvent(ref isStart, serviceReqInput);
                    if (isStart == false)
                        return;
                }

                Inventec.Common.Logging.LogSystem.Info("Finish Load Data Module");

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                   .Where(o => o.ModuleLink == moduleLink).FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + moduleLink);
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>();
                    V_HIS_SERVICE_REQ serviceReq = AutoMapper.Mapper.Map<HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>(serviceReqInput);
                    ServiceExecuteADO serviceExecute = new ServiceExecuteADO(serviceReq, ReLoadExecuteRoom);
                    serviceExecute.IsReadResult = true;
                    listArgs.Add(serviceExecute);
                    currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId);
                    listArgs.Add(serviceReq);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE, serviceReq.SERVICE_REQ_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);

                    //}
                }

                Inventec.Common.Logging.LogSystem.Info("Finish Load Module");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                var executeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(o => o.IS_EMERGENCY == 1 && o.ROOM_ID == roomId).FirstOrDefault();
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

        private void SetTextButtonExecute(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (serviceReq != null)
                {
                    if (serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        btnExecute.Text = "Hủy kết thúc (Ctrl X)";
                    }
                    else
                    {
                        btnExecute.Text = "Xử lý (Ctrl X)";
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
            //Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.NotFinished", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.Processing", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.Finish", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.All", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture())});
                //Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.NotYetProcessed", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture()),
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

        public async Task LoadServiceReqCount()
        {
            try
            {
                CommonParam param = new CommonParam();
                long today = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(System.DateTime.Today) ?? 0;

                HisServiceReqCountFilter filter = new HisServiceReqCountFilter();
                filter.INTRUCTION_DATE = today;
                filter.IS_BHYT = true;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.EXECUTE_LOGINNAME = loginName;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                long serviceReqCount = new BackendAdapter(param)
                    .Get<long>("api/HisServiceReq/GetCount", ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCount), serviceReqCount));

                lblServiceReqCount.Text = serviceReqCount.ToString();
                lblServiceReqCount.ToolTip = String.Format("Bạn đã xử lý {0} yêu cầu BHYT", serviceReqCount);
                lblServiceReqCount.Appearance.ForeColor = new Color();
                //Set canh bao
                List<HIS_EMPLOYEE> employees = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();
                if (employees == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach nhan vien tu backenddata");
                    return;
                }
                HIS_EMPLOYEE employee = employees.FirstOrDefault(o => o.LOGINNAME == loginName);
                if (employee == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Tai khoan hien tai khong phai la nhan vien");
                    return;
                }

                if (employee.MAX_BHYT_SERVICE_REQ_PER_DAY.HasValue)
                {
                    lblServiceReqCount.Text = lblServiceReqCount.Text + "/" + employee.MAX_BHYT_SERVICE_REQ_PER_DAY;

                    if (serviceReqCount > employee.MAX_BHYT_SERVICE_REQ_PER_DAY)
                    {
                        lblServiceReqCount.Appearance.ForeColor = Color.Red;
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
