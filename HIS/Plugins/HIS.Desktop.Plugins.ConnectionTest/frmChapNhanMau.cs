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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmChapNhanMau : Form
    {
        LisSampleADO rowData;
        Action<LIS_SAMPLE> refesh;
        public frmChapNhanMau(Action<LIS_SAMPLE> refesh, LisSampleADO _rowData)
        {
            InitializeComponent();
            this.refesh = refesh;
            this.rowData = _rowData;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtApproveTime.EditValue == null || dtApproveTime.DateTime == DateTime.MinValue) return;

                if (rowData.SAMPLE_TIME != null)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtApproveTime.DateTime) < rowData.SAMPLE_TIME)
                    {
                        MessageBox.Show(string.Format("Thời gian nhận mẫu không được nhỏ hơn thời gian lấy mẫu: {0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowData.SAMPLE_TIME ?? 0), "Thông báo"));
                        return;
                    }
                    if (LisConfigCFG.WARNING_APPROVE_TIME > 0)
                    {
                        TimeSpan time = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(dtApproveTime.DateTime.ToString("yyyyMMddHHmm00"))) - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(rowData.SAMPLE_TIME.ToString().Substring(0, 12) + "00"));
                        if (time.TotalMinutes > LisConfigCFG.WARNING_APPROVE_TIME)
                        {
                            if (XtraMessageBox.Show(String.Format("Bệnh nhân có thời gian duyệt mẫu xét nghiệm lớn hơn thời gian lấy mẫu {0} phút. Bạn có muốn tiếp tục?", LisConfigCFG.WARNING_APPROVE_TIME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                    }

                }

                bool success = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                sdo.SampleId = rowData.ID;
                sdo.ApproveTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtApproveTime.DateTime);
                var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Approve", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                if (curentSTT != null)
                {
                    success = true;
                    if (refesh != null)
                    {
                        this.refesh(curentSTT);
                        this.Close();
                    }
                }
                MessageManager.Show(this.ParentForm, param, success);
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void frmChapNhanMau_Load(object sender, EventArgs e)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);

            this.SetCaptionByLanguageKey();

            dtApproveTime.DateTime = DateTime.Now;

            btnSave.Select();
            btnSave.Focus();

        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmChapNhanMau
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau = new ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Lang", typeof(frmChapNhanMau).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChapNhanMau.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmChapNhanMau.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmChapNhanMau.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmChapNhanMau.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmChapNhanMau.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChapNhanMau.Text", Resources.ResourceLanguageManager.LanguageResource__frmChapNhanMau, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
