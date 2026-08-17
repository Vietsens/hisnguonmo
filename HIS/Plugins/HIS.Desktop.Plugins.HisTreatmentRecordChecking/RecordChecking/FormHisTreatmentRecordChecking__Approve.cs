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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Record approval actions: Dat / Khong dat / Duyet / Huy duyet and their bar items.
    /// </summary>
    public partial class FormHisTreatmentRecordChecking
    {
        private void btnKhongDat_Click(object sender, EventArgs e)
        {
            try
            {

                frmContentFailed ContentFailed = new frmContentFailed(moduleData, this.treatmentId, DgSeccess);
                ContentFailed.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DgSeccess(bool success)
        {
            try
            {
                if (success)
                {
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDat_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<long?> Input = new List<long?>();
                Input.Add(this.treatmentId);

                var resultData = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/ApprovalStore", ApiConsumers.MosConsumer, Input, param);

                if (resultData != null && resultData.Count > 0)
                {
                    success = true;
                }
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDuyet_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<long?> Input = new List<long?>();
                Input.Add(this.treatmentId);

                Inventec.Common.Logging.LogSystem.Debug("ApprovalStore (Duyet) INPUT____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => Input), Input));

                var resultData = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/ApprovalStore", ApiConsumers.MosConsumer, Input, param);

                if (resultData != null && resultData.Count > 0)
                {
                    success = true;
                }
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnDuyet.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHuyDuyet_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<long?> Input = new List<long?>();
                Input.Add(this.treatmentId);

                var resultData = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/UnapprovalStore", ApiConsumers.MosConsumer, Input, param);

                if (resultData != null && resultData.Count > 0)
                {
                    success = true;
                }
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnKhongDat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnKhongDat.Enabled == true)
                {
                    btnKhongDat_Click(null, null);
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnDat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnDat.Enabled == true)
                {

                    btnDat_Click(null, null);
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnHuyDuyet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnHuyDuyet.Enabled == true)
                {
                    btnHuyDuyet_Click(null, null);
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
