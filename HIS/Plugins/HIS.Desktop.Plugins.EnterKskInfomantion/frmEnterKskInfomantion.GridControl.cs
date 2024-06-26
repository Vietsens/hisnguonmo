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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion
{
    partial class frmEnterKskInfomantion
    {
        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlServiceReq);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                listData = new List<ADO.ServiceReqADO>();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_2>> apiResult = null;
                MOS.Filter.HisServiceReqView2Filter filter = new HisServiceReqView2Filter();
                SetFilter(ref filter);
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridControlServiceReq.BeginUpdate();
                gridControlServiceReq.DataSource = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_2>>(HisRequestUriStore.MOS_HIS_SERVICE_REQ_GETVIEW2, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_2>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        foreach (var item in data)
                        {
                            ADO.ServiceReqADO ado = new ADO.ServiceReqADO(item);
                            listData.Add(ado);
                        }
                        listData = listData.OrderByDescending(o => o.ID).ToList();
                        gridControlServiceReq.DataSource = listData;
                    }
                }

                gridControlServiceReq.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisServiceReqView2Filter filter)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtServiceReqCodeForSearch.Text.Trim()))
                {
                    string codeServiceReq = txtServiceReqCodeForSearch.Text.Trim();
                    if (codeServiceReq.Length < 12 && checkDigit(codeServiceReq))
                    {
                        codeServiceReq = string.Format("{0:000000000000}", Convert.ToInt64(codeServiceReq));
                        txtServiceReqCodeForSearch.Text = codeServiceReq;
                    }
                    filter.SERVICE_REQ_CODE__EXACT = codeServiceReq;
                }
                else if (!String.IsNullOrEmpty(txtTreatmentCodeForSearch.Text.Trim()))
                {
                    string codeTreatment = txtTreatmentCodeForSearch.Text.Trim();
                    if (codeTreatment.Length < 12 && checkDigit(codeTreatment))
                    {
                        codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                        txtTreatmentCodeForSearch.Text = codeTreatment;
                    }
                    filter.TREATMENT_CODE__EXACT = codeTreatment;
                }
                else if (!String.IsNullOrEmpty(txtPatientCodeForSearch.Text.Trim()))
                {
                    string codePatient = txtPatientCodeForSearch.Text.Trim();
                    if (codePatient.Length < 10 && checkDigit(codePatient))
                    {
                        codePatient = string.Format("{0:0000000000}", Convert.ToInt64(codePatient));
                        txtPatientCodeForSearch.Text = codePatient;
                    }
                    filter.TDL_PATIENT_CODE__EXACT = codePatient;
                }
                else
                {
                    filter.PATIENT_NAME = txtPatientNameForSearch.Text.Trim();
                    if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
                    {
                        filter.INTRUCTION_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
                    {
                        filter.INTRUCTION_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTo.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                    if (serviceReqSttSelecteds != null && serviceReqSttSelecteds.Count > 0)
                    {
                        filter.SERVICE_REQ_STT_IDs = serviceReqSttSelecteds.Select(o => o.ID).ToList();
                    }
                    if (cboKSKContract.EditValue != null)
                    {
                        filter.TDL_KSK_CONTRACT_ID = (long)cboKSKContract.EditValue;
                    }
                    if (_DepartmentSearchSelecteds != null && _DepartmentSearchSelecteds.Count > 0 && _DepartmentSearchSelecteds.Count != listDepartment.Count)
                    {
                        filter.EXECUTE_DEPARTMENT_IDs = _DepartmentSearchSelecteds.Select(p => p.ID).ToList();
                    }
                    if (_ExecuteRoomSearchSelecteds != null && _ExecuteRoomSearchSelecteds.Count > 0 && _ExecuteRoomSearchSelecteds.Count != listExecuteRoom.Count)
                    {
                        filter.EXECUTE_ROOM_IDS = _ExecuteRoomSearchSelecteds.Select(p => p.ROOM_ID).ToList();
                    }
                }
            }
            catch (Exception ex)

            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void ResetFormData()
        {
            try
            {
                ResetTabKhamChungFormData();
                ResetTabKetLuanFormData();
                ResetTabBenhNgheNghiepFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetPatientInfoDisplayed()
        {
            try
            {
                if (!layoutControlPatientInfo.IsInitialized) return;
                layoutControlPatientInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlPatientInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.LabelControl)
                        {
                            DevExpress.XtraEditors.LabelControl label = lci.Control as DevExpress.XtraEditors.LabelControl;

                            label.ResetText();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControlPatientInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetTabKhamChungFormData()
        {
            try
            {
                if (!layoutControlTab1.IsInitialized) return;
                layoutControlTab1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlTab1.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControlTab1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetTabKetLuanFormData()
        {
            try
            {
                if (!layoutControlTab2.IsInitialized) return;
                layoutControlTab2.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlTab2.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControlTab2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetTabBenhNgheNghiepFormData()
        {
            try
            {
                if (!layoutControlTab3.IsInitialized) return;
                layoutControlTab3.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlTab3.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControlTab3.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
