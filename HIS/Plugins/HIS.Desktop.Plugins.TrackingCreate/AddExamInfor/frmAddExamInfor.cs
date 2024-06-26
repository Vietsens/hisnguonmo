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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TrackingCreate;
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

namespace HIS.Desktop.Plugins.AddExamInfor
{
    public partial class frmAddExamInfor : HIS.Desktop.Utility.FormBase
    {
        HIS_TRACKING _currTracking { get; set; }

        internal frmTrackingCreateNew Form;
        internal delegate void GetString(HIS_TRACKING dataAdd);
        internal GetString MyGetData;

        public frmAddExamInfor()
        {
            InitializeComponent();
        }

        public frmAddExamInfor(HIS_TRACKING data)
            : base()
        {
            InitializeComponent();
            try
            {
                this._currTracking = data;
                SetIconFrm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void frmAddExamInfor_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                SetDataDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor = new ResourceManager("HIS.Desktop.Plugins.TrackingCreate.Resources.Lang", typeof(frmAddExamInfor).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.barButtonItem__Edit.Caption = Inventec.Common.Resource.Get.Value("frmAddExamInfor.barButtonItem__Edit.Caption", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmAddExamInfor.barButtonItem__Save.Caption", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.barButtonItem__Cancel.Caption = Inventec.Common.Resource.Get.Value("frmAddExamInfor.barButtonItem__Cancel.Caption", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.barButtonItem__Delete.Caption = Inventec.Common.Resource.Get.Value("frmAddExamInfor.barButtonItem__Delete.Caption", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControl1.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.btnSave.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem1.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem2.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem3.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem4.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem5.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem6.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem7.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem8.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem9.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem10.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem11.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.layoutControlItem12.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.bar1.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.barButtonItem______SV.Caption = Inventec.Common.Resource.Get.Value("frmAddExamInfor.barButtonItem______SV.Caption", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAddExamInfor.Text", TrackingCreate.Resources.ResourceLanguageManager.LanguageResource__frmAddExamInfor, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataDefault()
        {
            try
            {
                if (this._currTracking != null)
                {
                    txtBieuHienChung.Text = this._currTracking.GENERAL_EXPRESSION;
                    txtDinhHuong.Text = this._currTracking.ORIENTATION_CAPACITY;
                    txtCamXuc.Text = this._currTracking.EMOTION;
                    txtTriGiac.Text = this._currTracking.PERCEPTION;
                    txtTuDuy.Text = this._currTracking.CONTENT_OF_THINKING;
                    txtYChi.Text = this._currTracking.AWARENESS_BEHAVIOR;
                    txtBanNang.Text = this._currTracking.INSTINCTIVELY_BEHAVIOR;
                    txtTriNho.Text = this._currTracking.MEMORY;
                    txtTriTue.Text = this._currTracking.INTELLECTUAL;
                    txtTapTrung.Text = this._currTracking.CONCENTRATION;
                    txtTimMach.Text = this._currTracking.CARDIOVASCULAR;
                    txtHoHap.Text = this._currTracking.RESPIRATORY;
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
                HIS_TRACKING ado = new HIS_TRACKING();

                ado.GENERAL_EXPRESSION = txtBieuHienChung.Text;
                ado.ORIENTATION_CAPACITY = txtDinhHuong.Text;
                ado.EMOTION = txtCamXuc.Text;
                ado.PERCEPTION = txtTriGiac.Text;
                ado.CONTENT_OF_THINKING = txtTuDuy.Text;
                ado.AWARENESS_BEHAVIOR = txtYChi.Text;
                ado.INSTINCTIVELY_BEHAVIOR = txtBanNang.Text;
                ado.MEMORY = txtTriNho.Text;
                ado.INTELLECTUAL = txtTriTue.Text;
                ado.CONCENTRATION = txtTapTrung.Text;
                ado.CARDIOVASCULAR = txtTimMach.Text;
                ado.RESPIRATORY = txtHoHap.Text;

                MyGetData(ado);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem______SV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }
    }
}
