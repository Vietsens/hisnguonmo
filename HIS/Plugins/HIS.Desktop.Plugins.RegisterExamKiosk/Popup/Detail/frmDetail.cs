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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
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

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.Detail
{
    public partial class frmDetail : Form
    {

        long patientId;
        List<HIS_TREATMENT> listTreatments;

        DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        private bool stopThread;

        public frmDetail(long patientId)
        {
            InitializeComponent();
            try
            {
                this.patientId = patientId;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmDetail(long patientId, DelegateCloseForm_Uc closingForm)
        {
            InitializeComponent();
            try
            {
                this.patientId = patientId;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.DelegateClose = closingForm;

                CloseThread = new System.Threading.Thread(ClosingForm);
                CloseThread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {
            try
            {
                FillDateToGridControl(this.patientId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDateToGridControl(long patientId)
        {
            try
            {
                stopThread = true;
                //Lấy dữ liệu treatment load lên grid
                this.listTreatments = new List<HIS_TREATMENT>();
                CommonParam param = new CommonParam();
                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.PATIENT_ID = this.patientId;
                this.listTreatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);

                gridControlDetail.BeginUpdate();
                gridControlDetail.DataSource = this.listTreatments;
                gridControlDetail.EndUpdate();
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_TREATMENT pData = (HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.IN_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CLINICAL_IN_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.OUT_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "ICD_DETAIL")
                    {
                        e.Value = pData.ICD_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ClosingForm()
        {
            try
            {
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    bool time_out = false;
                    ResetLoopCount();
                    while (!time_out)
                    {
                        if (stopThread)
                        {
                            ResetLoopCount();
                        }

                        if (loopCount <= 0)
                        {
                            time_out = true;
                        }

                        System.Threading.Thread.Sleep(50);
                        loopCount--;
                    }

                    this.Invoke(new MethodInvoker(delegate() { this.Close(); }));
                    if (DelegateClose != null)
                    {
                        DelegateClose(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLoopCount()
        {
            try
            {
                this.loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
