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
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggregateAndIssuePrescriptionOrderNumber.Run
{
    public partial class frmAggregateAndIssuePrescriptionOrderNumber
    {
        private void ProcessAggregate()
        {
            try
            {
                if (string.IsNullOrEmpty(txtTreatmentCode.Text))
                    return;
                bool success = false;
                string code = txtTreatmentCode.Text.Trim();
                if (code.Length < 12)
                {
                    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtTreatmentCode.Text = code;
                }

                lblThongBao.Text = Resources.ResourceMessage.LoadingData;

                CommonParam param = new CommonParam();
                MOS.SDO.AggrExamByTreatAndStockSDO sdo = new MOS.SDO.AggrExamByTreatAndStockSDO();
                sdo.MediStockId = this.WorkPlaceSDO != null ? (WorkPlaceSDO.MediStockId ?? 0) : 0;
                sdo.TreatmentCode = code;
                var resultData = new BackendAdapter(param).Post<HIS_EXP_MEST>(HisRequestUriStore.MOS_HIS_EXP_MEST_AggrExamByTreatAndStock, ApiConsumers.MosConsumer, sdo, param);
                if (resultData != null)
                {
                    success = true;
                    this._expMest_ForPrint = resultData;
                    FillDataExpMest(resultData);
                    //ThreadXuLyThanhCong(param);
                    PrintProcess(MPS.Processor.Mps000479.PDO.Mps000479PDO.printTypeCode, true);
                }
                else
                {
                    FillDataExpMest(new HIS_EXP_MEST());
                    ThreadXuLyThatBai(param);
                }

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataExpMest(HIS_EXP_MEST resultData)
        {
            try
            {
                if (resultData == null)
                    return;
                lblNumOrder.Text = resultData.NUM_ORDER != null ? resultData.NUM_ORDER.ToString() : "";

                lblTDLTreatmentCode.Text = resultData.TDL_TREATMENT_CODE;
                lblTDLPatientName.Text = resultData.TDL_PATIENT_NAME;
                if (resultData.TDL_PATIENT_DOB != null)
                {
                    if (resultData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        string time = resultData.TDL_PATIENT_DOB.ToString();
                        lblTDLPatientDOB.Text = new StringBuilder().Append(time.Substring(0, 4)).ToString();
                    }
                    else
                    {
                        lblTDLPatientDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultData.TDL_PATIENT_DOB ?? 0);
                    }
                }
                else
                {
                    lblTDLPatientDOB.Text = "";
                }
                lblTDLPatientAddress.Text = resultData.TDL_PATIENT_ADDRESS;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThreadXuLyThanhCong(CommonParam param)
        {
            try
            {
                lblThongBao.Text = "";
                var message = param.GetMessage();
                lblThongBao.Text = String.Format("Xử lý thành công. {0}", message);
                this.isResetThongBao = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThreadXuLyThatBai(CommonParam param)
        {
            try
            {
                lblThongBao.Text = "";
                var message = param.GetMessage();
                if (String.IsNullOrWhiteSpace(message))
                {
                    lblThongBao.Text = "Không tìm thấy dữ liệu.";
                }
                else
                {
                    lblThongBao.Text = String.Format("Xử lý thất bại. {0}", message);
                }
                this.isResetThongBao = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetThongBao()
        {
            try
            {
                if (this.isResetThongBao)
                {
                    this.isResetThongBao = false;
                    lblThongBao.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //ResetThongBaoAsync();
        }

        private async Task ResetThongBaoAsync()
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                );
                t.Start();
                await t;
                lblThongBao.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
