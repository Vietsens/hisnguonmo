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
using HIS.Desktop.Plugins.AssignService.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// Man hinh xac nhan phong xu ly truoc khi luu chi dinh dich vu ky thuat.
    /// Chi hien thi khi cau hinh HIS.Desktop.Plugins.AssignService.ConfirmExecuteRoomWhenSave = 1.
    /// Chi mang tinh xac nhan: khong kiem tra tinh hop le giua dich vu va phong xu ly.
    /// </summary>
    public partial class frmConfirmExecuteRoom : HIS.Desktop.Utility.FormBase
    {
        private List<ExecuteRoomConfirmADO> executeRoomConfirms;
        private List<string> serviceNamesWithoutRoom;

        /// <summary>
        /// True khi bac si chon "Dong y" — cho phep tiep tuc luu chi dinh.
        /// Mac dinh false: dong cua so bang nut X hoac "Khong dong y" deu la khong luu.
        /// </summary>
        internal bool IsAgreed { get; private set; }

        internal frmConfirmExecuteRoom(
            Inventec.Desktop.Common.Modules.Module currentModule,
            List<ExecuteRoomConfirmADO> _executeRoomConfirms,
            List<string> _serviceNamesWithoutRoom)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.executeRoomConfirms = _executeRoomConfirms;
                this.serviceNamesWithoutRoom = _serviceNamesWithoutRoom;
                this.IsAgreed = false;
                this.IsUseApplyFormClosingOption = false;
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmConfirmExecuteRoom_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                FillDataToGrid();
                FillServiceWithoutRoom();

                // Mac dinh focus vao "Khong dong y" de bac si khong lo nhan Enter thanh luu nham
                this.btnDisagree.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham xet ngon ngu cho giao dien frmConfirmExecuteRoom
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmConfirmExecuteRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lblQuestion.Text = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.lblQuestion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExecuteRoomName.Caption = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.gridColumnExecuteRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceCount.Caption = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.gridColumnServiceCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceCodes.Caption = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.gridColumnServiceCodes.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceNoRoom.Text = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.lciServiceNoRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAgree.Text = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.btnAgree.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDisagree.Text = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.btnDisagree.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmConfirmExecuteRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                this.gridViewExecuteRoom.BeginUpdate();
                try
                {
                    this.grdExecuteRoom.DataSource = this.executeRoomConfirms;
                }
                finally
                {
                    this.gridViewExecuteRoom.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Vung "Dich vu chua chon phong xu ly" chi hien khi co du lieu.
        /// Danh sach da duoc lam rong tu ben goi khi vien bat co che he thong tu phan phong xu ly.
        /// </summary>
        private void FillServiceWithoutRoom()
        {
            try
            {
                if (this.serviceNamesWithoutRoom == null || this.serviceNamesWithoutRoom.Count == 0)
                {
                    this.lciServiceNoRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    return;
                }

                this.lciServiceNoRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.txtServiceNoRoom.Text = String.Join(Environment.NewLine, this.serviceNamesWithoutRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAgree_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsAgreed = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDisagree_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsAgreed = false;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                this.executeRoomConfirms = null;
                this.serviceNamesWithoutRoom = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
