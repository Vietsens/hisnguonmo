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
using HIS.UC.HisMaterialInStock.ADO;
using HIS.UC.HisMedicineInStock.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.MediStockSummary
{
    public partial class frmReasonLock : Form
    {
        Action<bool> IsSuccess;
        private int positionHandleControl;
        object obj;
        long MediStockId, RoomId;
        public frmReasonLock(object obj ,Action<bool> IsSuccess, long MediStockId, long RoomId)
        {
            InitializeComponent();
            this.IsSuccess = IsSuccess;
            this.obj = obj;
            this.MediStockId = MediStockId;
            this.RoomId = RoomId;
            SetIconFrm();
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

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void frmReasonLock_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                TextEditValidationRule vld = new TextEditValidationRule();
                vld.txt = memReason;
                vld.maxLength = 1000;
                dxValidationProvider1.SetValidationRule(memReason,vld);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmReasonLock
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockSummary.Resources.Lang", typeof(frmReasonLock).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmReasonLock.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmReasonLock.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmReasonLock.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmReasonLock.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmReasonLock.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmReasonLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmReasonLock_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                RoomId = 0;
                obj = null;
                positionHandleControl = 0;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.Load -= new System.EventHandler(this.frmReasonLock_Load);
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                dxValidationProvider1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                memReason = null;
                btnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
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
                if (!dxValidationProvider1.Validate())
                    return;

                string api = null;
                bool IsMedicine = false;
                HisMaterialChangeLockSDO dataMate = new HisMaterialChangeLockSDO();

                HisMedicineChangeLockSDO dataMedi = new HisMedicineChangeLockSDO();
                if (obj is HisMedicineInStockADO)
                {
                    var medi = obj as HisMedicineInStockADO;

                    if (medi != null)
                    {
                        IsMedicine = true;
                        dataMedi.LockingReason = memReason.Text;
                        dataMedi.MedicineId = medi.ID;
                        dataMedi.MediStockId = MediStockId;
                        dataMedi.WorkingRoomId = RoomId;
                        api = "/api/HisMedicine/Lock";
                    }
                }
                else if (obj is HisMaterialInStockADO)
                {
                    var mate = obj as HisMaterialInStockADO;
                    if (mate != null)
                    {
                        dataMate.LockingReason = memReason.Text;
                        dataMate.MaterialId = mate.ID;
                        dataMate.MediStockId = MediStockId;
                        dataMate.WorkingRoomId = RoomId;
                        api = "/api/HisMaterial/Lock";
                    }
                }
                CommonParam param = new CommonParam();
                
                    WaitingManager.Show();
                    bool succes = false;
                if (IsMedicine) 
                        succes = new BackendAdapter(param).Post<bool>(api, ApiConsumer.ApiConsumers.MosConsumer, dataMedi , param);
                    else
                        succes = new BackendAdapter(param).Post<bool>(api, ApiConsumer.ApiConsumers.MosConsumer, dataMate, param);
                    IsSuccess(succes);
                this.Close();
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, succes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
