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
using Inventec.Core;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.RehaServiceReqExecute.ADO;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    internal class RehaServiceReqExecuteControlProcess
    {
        internal static void LoadDataPatient(RehaServiceReqExecuteControl control, long treatmentId)
        {
            try
            {
                if (control != null)
                {
                    CommonParam param = new CommonParam();

                    MOS.Filter.HisDepartmentTranViewFilter hisDepartmentTranFilter = new HisDepartmentTranViewFilter();
                    hisDepartmentTranFilter.TREATMENT_ID = treatmentId;
                    List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> lsthisDepartmentTran = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW, ApiConsumers.MosConsumer, hisDepartmentTranFilter, param);
                    var lsthisDepartmentTranLast = lsthisDepartmentTran.OrderByDescending(o => o.DEPARTMENT_IN_TIME).ThenByDescending(o => o.ID).FirstOrDefault();//Lấy bản ghi sau cùng

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadEnableAndDisableButton(RehaServiceReqExecuteControl control)
        {
            try
            {
                if (control.HisServiceReqWithOrderSDO != null)
                {
                    if (control.HisServiceReqWithOrderSDO.SERVICE_REQ_STT_ID == IMSys. DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        control.btnSave.Enabled = false;
                        control.btnFinish.Enabled = false;
                    }
                    else
                    {
                        control.btnSave.Enabled = true;
                        control.btnFinish.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN> LoadDataRehaTrainForSereServReha(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_REHA currentSereServReha, V_HIS_SERVICE_REQ HisServiceReqWithOrderSDO)
        {
            List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN> result = null;
            try
            {
                MOS.Filter.HisRehaTrainViewFilter hisRehaTrainViewFilter = new MOS.Filter.HisRehaTrainViewFilter();
                if (currentSereServReha != null)
                {
                    hisRehaTrainViewFilter.SERE_SERV_REHA_ID = currentSereServReha.ID;
                    result = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN>>(HisRequestUriStore.HIS_REHA_TRAIN_GETVIEW, ApiConsumers.MosConsumer, hisRehaTrainViewFilter, new CommonParam()); ;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        //internal static void LoadIcdCombo(string searchCode, bool isExpand, RehaServiceReqExecuteControl control)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(searchCode))
        //        {
        //            control.cboIcdMainName.Properties.Buttons[1].Visible = false;
        //            control.cboIcdMainName.EditValue = null;
        //            control.cboIcdMainName.Focus();
        //            control.cboIcdMainName.ShowPopup();
        //            //PopupProcess.SelectFirstRowPopup(control.cboIcdMainName);
        //        }
        //        else
        //        {
        //            var data = EXE.LOGIC.LocalStore.HisDataLocalStore.HisIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
        //            if (data != null)
        //            {
        //                if (data.Count == 1)
        //                {
        //                    control.cboIcdMainName.Properties.Buttons[1].Visible = true;
        //                    control.cboIcdMainName.EditValue = data[0].ID;
        //                    control.txtIcdMainCode.Text = data[0].ICD_CODE;
        //                    control.txtIcdExtraName.Focus();
        //                }
        //                else
        //                {
        //                    var search = data.FirstOrDefault(m => m.ICD_CODE == searchCode);
        //                    if (search != null)
        //                    {
        //                        control.cboIcdMainName.Properties.Buttons[1].Visible = true;
        //                        control.cboIcdMainName.EditValue = search.ID;
        //                        control.txtIcdMainCode.Text = search.ICD_CODE;
        //                        control.txtIcdExtraName.Focus();
        //                    }
        //                    else
        //                    {
        //                        control.cboIcdMainName.Properties.Buttons[1].Visible = false;
        //                        control.cboIcdMainName.EditValue = null;
        //                        control.cboIcdMainName.Focus();
        //                        control.cboIcdMainName.ShowPopup();
        //                        //PopupProcess.SelectFirstRowPopup(control.cboIcdMainName);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
