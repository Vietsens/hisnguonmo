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
using HIS.UC.UCImageInfo.ADO;
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using HIS.UC.UCImageInfo.Properties;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCImageInfo.Base;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.UCImageInfo
{
    public partial class UCImageInfo : UserControl
    {
        #region Declare
        Action<object> dlgFocusNextUserControl;
        Action<object> ReloadDataByCmndBefore;
        Action<object> ReloadDataByCmndAfter;
        ImageType Type;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.UCImageInfo";
        #endregion

        #region Constructor - Load
        public UCImageInfo()
        {
            InitializeComponent();
        }

        private void UCImageInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCImageInfo_Load .1");
                SetCaptionByLanguageKeyNew();
                RefreshUserControl();
                InitControlState();
                //LoadDataByCheckBox();
                Inventec.Common.Logging.LogSystem.Debug("UCImageInfo_Load .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataByCheckBox()
        {
            try
            {
                if (chkAuto.Checked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("chkAuto.Checked " + chkAuto.Checked);
                    btnRecognizeInfo_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.ResourceUCImage = new ResourceManager("HIS.UC.UCImageInfo.Resources.Lang", typeof(HIS.UC.UCImageInfo.UCImageInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.layoutControl1.Text", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.btnAnhTheBHYT.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnAnhTheBHYT.Text", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.btnAnhChanDung.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnAnhChanDung.Text", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.pteAnhTheBHYT.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteAnhTheBHYT.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.pteAnhChanDung.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteAnhChanDung.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.layoutControlGroup1.Text", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.btnCmndBefore.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndBefore.Text", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.btnCmndAfter.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndAfter.Text", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.btnCmndBefore.ToolTip = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndBefore.ToolTip", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.btnCmndAfter.ToolTip = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndAfter.ToolTip", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.pteCmndBefore.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteCmndBefore.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
                this.pteCmndAfter.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteCmndAfter.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCImage, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCImageInfo
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCImageInfo.Resources.Lang", typeof(UCImageInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRecognizeInfo.ToolTip = Inventec.Common.Resource.Get.Value("UCImageInfo.btnRecognizeInfo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAuto.Properties.Caption = Inventec.Common.Resource.Get.Value("UCImageInfo.chkAuto.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAuto.ToolTip = Inventec.Common.Resource.Get.Value("UCImageInfo.chkAuto.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCmndAfter.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndAfter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCmndAfter.ToolTip = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndAfter.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCmndBefore.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndBefore.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCmndBefore.ToolTip = Inventec.Common.Resource.Get.Value("UCImageInfo.btnCmndBefore.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pteCmndAfter.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteCmndAfter.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pteCmndBefore.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteCmndBefore.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAnhTheBHYT.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnAnhTheBHYT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAnhChanDung.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.btnAnhChanDung.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pteAnhTheBHYT.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteAnhTheBHYT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pteAnhChanDung.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImageInfo.pteAnhChanDung.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCImageInfo.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control
        private void btnAnhChanDung_Click(object sender, EventArgs e)
        {
            try
            {
                this.Type = ImageType.CHAN_DUNG;
                CallModuleCamera();

                if (this.btnCmndBefore.Enabled == true && this.pteCmndBefore.Enabled == true && (this.pteCmndBefore.Image == null || String.IsNullOrEmpty(pteCmndBefore.Tag.ToString())))
                {
                    this.btnCmndBefore.Focus();
                    btnCmndBefore_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAnhTheBHYT_Click(object sender, EventArgs e)
        {
            try
            {
                this.Type = ImageType.THE_BHYT;
                CallModuleCamera();
                if (dlgFocusNextUserControl != null)
                {
                    this.dlgFocusNextUserControl(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCmndBefore_Click(object sender, EventArgs e)
        {
            try
            {
                this.Type = ImageType.CMND_CCCD_TRUOC;
                CallModuleCamera();
                if (chkAuto.Checked)
                {
                    if (ReloadDataByCmndBefore != null && pteCmndBefore.Image != null && pteCmndBefore.Image.Tag != null && !pteCmndBefore.Image.Tag.Equals("noImage"))
                    {
                        ReloadDataByCmndBefore(pteCmndBefore.Image);
                    }
                }
                if (this.btnCmndAfter.Enabled == true && this.pteCmndAfter.Enabled == true && (this.pteCmndAfter.Image == null || String.IsNullOrEmpty(pteCmndAfter.Tag.ToString())))
                {
                    this.btnCmndAfter.Focus();
                    btnCmndAfter_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCmndAfter_Click(object sender, EventArgs e)
        {
            try
            {
                this.Type = ImageType.CMND_CCCD_SAU;
                CallModuleCamera();
                if (chkAuto.Checked)
                {
                    if (ReloadDataByCmndAfter != null && pteCmndAfter.Image != null && pteCmndAfter.Image.Tag != null && !pteCmndAfter.Image.Tag.Equals("noImage"))
                    {
                        ReloadDataByCmndAfter(pteCmndAfter.Image);
                    }
                }
                FocusToImageHein();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnRecognizeInfo_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (ReloadDataByCmndBefore != null && pteCmndBefore.Image != null && pteCmndBefore.Image.Tag != null && !pteCmndBefore.Image.Tag.Equals("noImage"))
                {
                    ReloadDataByCmndBefore(pteCmndBefore.Image);
                }
                if (pteCmndBefore.Image != null && !pteCmndBefore.Tag.Equals("noImage"))
                {
                    ReloadDataByCmndBefore(pteCmndBefore.Image);
                }
                if (pteCmndAfter.Image != null && !pteCmndAfter.Tag.Equals("noImage"))
                {
                    ReloadDataByCmndAfter(pteCmndAfter.Image);
                }
                if (ReloadDataByCmndAfter != null && pteCmndAfter.Image != null && pteCmndAfter.Image.Tag != null && !pteCmndAfter.Image.Tag.Equals("noImage"))
                {
                    ReloadDataByCmndAfter(pteCmndAfter.Image);
                }
                else if ((pteCmndAfter.Image == null && pteCmndBefore.Image == null) || (pteCmndBefore.Image != null && pteCmndBefore.Tag.Equals("noImage") && pteCmndAfter.Image != null && pteCmndAfter.Tag.Equals("noImage")))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có ảnh CMND/CCCD", "Thông Báo", MessageBoxButtons.OK) != DialogResult.OK)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkAuto_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAuto.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAuto.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAuto.Name;
                    csAddOrUpdate.VALUE = (chkAuto.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                if (chkAuto.Checked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("chkAuto.Checked " + chkAuto.Checked);
                    btnRecognizeInfo_Click(null, null);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkAuto.Name)
                        {
                            chkAuto.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void pteCmndBefore_Modified(object sender, EventArgs e)
        {
            try
            {
                pteCmndBefore.Tag = "changed";
                //LoadDataByCheckBox();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pteCmndAfter_Modified(object sender, EventArgs e)
        {
            try
            {
                pteCmndAfter.Tag = "changed";
                //LoadDataByCheckBox();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



    }
}
