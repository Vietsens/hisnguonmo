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
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LisSampleAggregation
{
    public delegate void DelegateReloadData();
    public partial class frmBarcodeSampleAggregation : HIS.Desktop.Utility.FormBase
    {
        List<LIS_SAMPLE> samples;
        DelegateReloadData actReloadData;

        public frmBarcodeSampleAggregation(List<LIS_SAMPLE> _samples, DelegateReloadData _actReloadData)
        {
            InitializeComponent();
            this.samples = _samples;
            this.actReloadData = _actReloadData;
        }

        private void frmBarcodeSampleAggregation_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultFocus()
        {
            try
            {
                txtBarcode.Focus();
                txtBarcode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSave()
        {
            try
            {
                if (this.samples == null || this.samples.Count() == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong co du lieu cac mau can gop!");
                    return;
                }

                if (txtBarcode.Text.Length > 15)
                {
                    //string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon
                    MessageBox.Show("Độ dài Barcode mẫu gộp lớn hơn 15", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                WaitingManager.Show();
                var listSampleId = this.samples.Select(s => s.ID).ToList();
                string barcode = txtBarcode.Text.Trim();
                if (String.IsNullOrEmpty(barcode))
                    barcode = null;
                CommonParam param = new CommonParam();
                bool success = false;
                List<LIS_SAMPLE> apiresult = null;

                AggregateSDO sdo = new AggregateSDO();
                sdo.SampleIds = listSampleId;
                sdo.Barcode = barcode;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<List<LIS_SAMPLE>>("api/LisSample/Aggregate", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null && apiresult.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));
                    
                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    this.actReloadData();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSave.Enabled)
                        btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
