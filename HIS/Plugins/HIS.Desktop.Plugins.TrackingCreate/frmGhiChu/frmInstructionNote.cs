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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.TrackingCreate.frmGhiChu
{
    public partial class frmInstructionNote : HIS.Desktop.Utility.FormBase
    {
        long _Sere_serv_id { get; set; }

        public frmInstructionNote()
        {
            InitializeComponent();
        }

        public frmInstructionNote(long _sere_serv_id)
        {
            InitializeComponent();
            try
            {
                this._Sere_serv_id = _sere_serv_id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmInstructionNote_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIconFrm();
                LoadSereServExt();
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmInstructionNote = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(frmInstructionNote).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInstructionNote.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmInstructionNote, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInstructionNote.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__frmInstructionNote, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmInstructionNote.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmInstructionNote, LanguageManager.GetCulture());
                this.barButtonI__Save.Caption = Inventec.Common.Resource.Get.Value("frmInstructionNote.barButtonI__Save.Caption", Resources.ResourceLanguageManager.LanguageResource__frmInstructionNote, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInstructionNote.Text", Resources.ResourceLanguageManager.LanguageResource__frmInstructionNote, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServExt()
        {
            try
            {
                if (this._Sere_serv_id > 0)
                {
                    MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                    filter.SERE_SERV_ID = this._Sere_serv_id;
                    filter.IS_ACTIVE = 1;

                    var datas = new BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, null);
                    if (datas != null && datas.Count > 0)
                    {
                        this.txtGhiChu.Text = datas[0].INSTRUCTION_NOTE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtGhiChu.Text))
                {
                    if (Encoding.UTF8.GetByteCount(this.txtGhiChu.Text.Trim()) > 500)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dài quá số ký tự được cho phép (500)", "Thông báo");
                        return;
                    }
                }
                WaitingManager.Show();
                HIS_SERE_SERV_EXT ado = new HIS_SERE_SERV_EXT();
                ado.INSTRUCTION_NOTE = this.txtGhiChu.Text;
                ado.SERE_SERV_ID = this._Sere_serv_id;

                CommonParam param = new CommonParam();
                bool success = false;
                var result = new BackendAdapter(param).Post<HIS_SERE_SERV_EXT>("/api/HisSereServExt/SetInstructionNote", ApiConsumers.MosConsumer, ado, param);
                if (result != null)
                {
                    success = true;
                    // this.Close();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
