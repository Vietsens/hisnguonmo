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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    public partial class UC_ImageBHYT : UserControl
    {
        int demBHYT = 0;
        public UC_ImageBHYT()
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            try
            {
                CallModuleCamera();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }       /// <summary>
                ///Hàm xét ngôn ngữ cho giao diện UC_ImageBHYT
                /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientTypeAlter.Resources.Lang", typeof(UC_ImageBHYT).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ImageBHYT.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCamera.Text = Inventec.Common.Resource.Get.Value("UC_ImageBHYT.btnCamera.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void CallModuleCamera()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add((DelegateSelectData)FillImageFromModuleCamereToUC);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Camera", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetImageDefaultForPictureEdit(Image imageData)
        {
            try
            {
                if (imageData != null)
                {
                    pictureEditImageBHYT.Image = (Image)imageData;
                    pictureEditImageBHYT.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.pictureEditImageBHYT.Tag = "Image";
                }
                else
                {
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_ImageBHYT));
                    this.pictureEditImageBHYT.EditValue = ((object)(resources.GetObject("pictureEditImageBHYT.EditValue")));
                    this.pictureEditImageBHYT.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal void FillImageFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    pictureEditImageBHYT.Image = (Image)dataImage;
                    pictureEditImageBHYT.Tag = ((Image)dataImage).Tag;
                    pictureEditImageBHYT.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.pictureEditImageBHYT.Tag = "Image";
                }
                else
                {
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_ImageBHYT));
                    this.pictureEditImageBHYT.EditValue = ((object)(resources.GetObject("pictureEditImageBHYT.EditValue")));
                    this.pictureEditImageBHYT.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pictureEditImageBHYT_ImageChanged(object sender, EventArgs e)
        {
            try
            {
                this.demBHYT++;
                if (this.demBHYT != 0 && this.pictureEditImageBHYT.Image != null)
                {
                    this.pictureEditImageBHYT.Tag = "Image";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void DisposeControl()
        {
            try
            {
                demBHYT = 0;
                this.btnCamera.Click -= new System.EventHandler(this.btnCamera_Click);
                this.pictureEditImageBHYT.ImageChanged -= new System.EventHandler(this.pictureEditImageBHYT_ImageChanged);
                pictureEditImageBHYT = null;
                btnCamera = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
