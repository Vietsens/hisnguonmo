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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
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

namespace HIS.Desktop.Plugins.ServiceExecute.EkipTemp
{
    public partial class frmEkipTemp : Form
    {
        public int positionHandle = -1;
        List<HisEkipUserADO> ekipUsers { get; set; }
        DelegateRefreshData refeshData { get; set; }
        Inventec.Desktop.Common.Modules.Module Module;

        public frmEkipTemp(List<HisEkipUserADO> _ekipUsers, DelegateRefreshData _refeshData,Inventec.Desktop.Common.Modules.Module _Module)
        {
            InitializeComponent();
            try
            {
                this.ekipUsers = _ekipUsers;
                this.refeshData = _refeshData;
                this.Module = _Module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool valid = true;
            bool success = false;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (valid)
                {

                    CommonParam param = new CommonParam();
                    HIS_EKIP_TEMP ekipTemp = new HIS_EKIP_TEMP();
                    ekipTemp.EKIP_TEMP_NAME = txtEkipTempName.Text;
                    ekipTemp.IS_PUBLIC = chkPublic.Checked ? (short?)1 : null;

                    if (this.Module.RoomId > 0)
                    {
                        var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;

                        ekipTemp.DEPARTMENT_ID = DepartmentID;
                        ekipTemp.IS_PUBLIC_IN_DEPARTMENT = chkPublicDepartment.Checked ? (short?)1 : null;
                    }
                    else
                    {
                        ekipTemp.DEPARTMENT_ID = null;
                        this.chkPublicDepartment.Checked = false;
                        this.chkPublicDepartment.Enabled = false;
                        this.chkPublicDepartment.ToolTip = " Để sử dụng tính năng này, bạn cần mở chức năng \"kíp mẫu\" từ phòng làm việc";
                    }

                    foreach (var item in ekipUsers)
                    {
                        HIS_EKIP_TEMP_USER ekipTempUser = new HIS_EKIP_TEMP_USER();
                        ekipTempUser.EXECUTE_ROLE_ID = item.EXECUTE_ROLE_ID;
                        ekipTempUser.USERNAME = item.USERNAME;
                        ekipTempUser.LOGINNAME = item.LOGINNAME;
                        ekipTemp.HIS_EKIP_TEMP_USER.Add(ekipTempUser);
                    }
                    WaitingManager.Show();
                    var ekipTempRS = new BackendAdapter(param)
                    .Post<HIS_EKIP_TEMP>("api/HisEkipTemp/Create", ApiConsumers.MosConsumer, ekipTemp, param);
                    WaitingManager.Hide();
                    if (ekipTempRS != null)
                    {
                        success = true;
                        if (refeshData != null)
                            refeshData();
                        this.Close();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmEkipTemp_Load(object sender, EventArgs e)
        {
            try
            {
                //Load icon
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                this.SetCaptionByLanguageKey();
                //Validate
                Validate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmEkipTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(frmEkipTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.chkPublicDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.chkPublicDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmEkipTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmEkipTemp.Text", Resources.ResourceLanguageManager.LanguageResourcefrmEkipTemp, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmEkipTemp_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Module = null;
                refeshData = null;
                ekipUsers = null;
                positionHandle = 0;
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.frmEkipTemp_Load);
                layoutControlItem5 = null;
                labelControl1 = null;
                layoutControlItem4 = null;
                chkPublicDepartment = null;
                layoutControlItem1 = null;
                chkPublic = null;
                dxValidationProvider1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                layoutControlItem2 = null;
                txtEkipTempName = null;
                btnSave = null;
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
