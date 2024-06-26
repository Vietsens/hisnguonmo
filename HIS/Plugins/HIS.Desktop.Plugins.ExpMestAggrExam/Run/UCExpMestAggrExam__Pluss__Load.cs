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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggrExam.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggrExam
{
    public partial class UCExpMestAggrExam : HIS.Desktop.Utility.UserControlBase
    {
        List<ExpMestADO> _ExpMestADOs = new List<ExpMestADO>();
        List<V_HIS_EXP_MEST> _ExpMest_PLs = new List<V_HIS_EXP_MEST>();
        /// <summary>
        /// Khoi tao du lieu danh sach phong
        /// </summary>
        /// <summary>
        /// Khoi tao du lieu danh sach don thuoc
        /// </summary>
        private void LoadDataExpMestThuocNT()
        {
            try
            {
                WaitingManager.Show();
                gridControlExpMestReq.DataSource = null;
                #region Filter
                MOS.Filter.HisExpMestViewFilter _expMestFilter = new HisExpMestViewFilter();
                _expMestFilter.ORDER_FIELD = "MODIFY_TIME";
                _expMestFilter.ORDER_DIRECTION = "DESC";

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    _expMestFilter.TDL_TREATMENT_CODE__EXACT = code;
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.TREATMENT_CODE__EXACT = code;
                    var treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, null);
                    if (treatments != null && treatments.Count > 0)
                    {
                        var fistExpMest = treatments.First();
                        this.treatment = new V_HIS_TREATMENT_4();
                        this.treatment.TREATMENT_CODE = fistExpMest.TREATMENT_CODE;
                        this.treatment.ID = fistExpMest.ID;
                        this.treatment.PATIENT_ID = fistExpMest.PATIENT_ID;
                        this.treatment.TDL_PATIENT_NAME = fistExpMest.TDL_PATIENT_NAME;
                        this.treatment.TDL_PATIENT_CODE = fistExpMest.TDL_PATIENT_CODE;
                        this.LoadDataToTreatmentInfo(this.treatment);
                    }
                    else
                    {
                        this.treatment = null;
                        this.LoadDataToTreatmentInfo(null);
                    }
                }
                else if (this.treatment != null)
                {
                    _expMestFilter.TDL_TREATMENT_ID = this.treatment.ID; // filter theo hsdt được chọn
                }
                else
                {
                    SetDefaultValue();
                    txtPatientName.Text = "";
                    txtPatientCode.Text = "";
                    return;
                }

                _expMestFilter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK;//La Don Phong Kham
                if (chkNotSynthetic.CheckState == CheckState.Checked && chkSynthesized.CheckState == CheckState.Unchecked)
                {
                    //HAS_AGGR 
                    //Nếu chưa thuộc phiếu nào thì = false
                    //Nếu đã đc tổng hợp = true
                    //Chua thuoc phieu nao va phieu do chua xuat
                    _expMestFilter.HAS_AGGR = false;
                    _expMestFilter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                }
                else if (chkNotSynthetic.CheckState == CheckState.Unchecked && chkSynthesized.CheckState == CheckState.Checked)
                    _expMestFilter.HAS_AGGR = true;
                #endregion

                #region DateTime
                #endregion

                CommonParam param = new CommonParam();
                _ExpMestADOs = new List<ExpMestADO>();

                var _ExpMest_NTs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, _expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (_ExpMest_NTs != null && _ExpMest_NTs.Count > 0)
                {


                    foreach (var item in _ExpMest_NTs)
                    {
                        ExpMestADO ado = new ExpMestADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_EXP_MEST>(ado, item);
                        if ( item.AGGR_EXP_MEST_ID == null && item.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            ado.IsCheck = true;// mặc định check all
                        }
                        
                        _ExpMestADOs.Add(ado);
                    }
                }
                else
                {
                    SetDefaultValue();
                }
                gridControlExpMestReq.DataSource = _ExpMestADOs;
                WaitingManager.Hide();
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi tao du lieu danh sach phieu linh
        /// </summary>
        private void LoadDataAggrExpMest()
        {
            try
            {
                int pageSize = ucPagingControlAggrExpMest.pagingGrid != null ? ucPagingControlAggrExpMest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingAggrExpMest(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingControlAggrExpMest.Init(PagingAggrExpMest, param, pageSize, gridControlAggrExpMest);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void PagingAggrExpMest(object param)
        {
            try
            {
                // WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlAggrExpMest.DataSource = null;

                HisExpMestViewFilter filter = new HisExpMestViewFilter();
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        //txtTreatmentCode.Text = code;
                    }
                    filter.TDL_TREATMENT_CODE__EXACT = code;
                }
                else if (this.treatment != null)
                {
                    filter.TDL_TREATMENT_ID = this.treatment.ID;
                }
                else
                {
                    return;
                }

                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                _ExpMest_PLs = new List<V_HIS_EXP_MEST>();
                if (apiResult != null)
                {
                    _ExpMest_PLs = (List<V_HIS_EXP_MEST>)apiResult.Data;
                    if (_ExpMest_PLs != null)
                    {
                        gridControlAggrExpMest.DataSource = _ExpMest_PLs;
                        rowCount = _ExpMest_PLs.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                    }
                }
                // WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi tao du lieu chi tieu phieu linh
        /// </summary>
        private void LoadDetailAggrExpMestByAggrExpMestId()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = ucPagingAggregateRequest.pagingGrid != null ? ucPagingAggregateRequest.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                PagingDetailAggrExpMest(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCountExpM;
                param.Count = dataTotalExpM;
                ucPagingAggregateRequest.Init(PagingDetailAggrExpMest, param, pageSize, gridControlAggregateRequest);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void PagingDetailAggrExpMest(object param)
        {
            try
            {
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                gridControlAggregateRequest.DataSource = null;
                MOS.Filter.HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.AGGR_EXP_MEST_ID = this.aggrExpMestId;
                expMestFilter.ORDER_DIRECTION = "DESC";
                expMestFilter.ORDER_FIELD = "MODIFY_TIME";
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);



                //List<long> expMestIds = (currentExpMest != null && currentExpMest.Count > 0) ? currentExpMest.Select(o => o.ID).ToList() : null;
                //HisPrescriptionViewFilter prescriptionFilter = new HisPrescriptionViewFilter();
                //prescriptionFilter.EXP_MEST_IDs = expMestIds;
                //Review
                //var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_PRESCRIPTION>>(HisRequestUriStore.HIS_PRESCRIPTION_GETVIEW, ApiConsumers.MosConsumer, prescriptionFilter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_EXP_MEST>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlAggregateRequest.DataSource = data;
                        rowCountExpM = data.Count;
                        dataTotalExpM = apiResult.Param.Count ?? 0;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
