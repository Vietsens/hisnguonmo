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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using HIS.UC.UCPatientRaw.ADO;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.UCPatientRaw
{
    public partial class frmPersonHome : FormBase
    {
        DelegateSetDataRegisterBeforeSerachPatient dlgSearchPatient1;
        DelegateUpdatePersonHomeInfo dlgReturnData;

        public frmPersonHome(UCRelativeInfo.ADO.UCRelativeADO data)
            : this(null, data)
        {
        }

        public frmPersonHome(Inventec.Desktop.Common.Modules.Module module, UCRelativeInfo.ADO.UCRelativeADO data)
            : base(module)
        {
            InitializeComponent();
            this.ucRelativeInfo1.FocusNextUserControl(FocusMoveOutUserControl);
            this.ucRelativeInfo1.SetValidateControl(true);
            FillOldDataIntoForm(data);
            SetCaptionByLanguageKey();
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmPersonHome
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                 ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.UCPatientRaw. Lang", typeof(frmPersonHome).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPersonHome.layoutControl1.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPersonHome.btnSave.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPersonHome.bar1.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPersonHome.barButtonItem1.Caption",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPersonHome.Text",  ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillOldDataIntoForm(UCRelativeInfo.ADO.UCRelativeADO data)
        {
            try
            {
                this.ucRelativeInfo1.SetValue(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusMoveOutUserControl()
        {
            try
            {
                this.btnSave.Focus();
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
                btnSave_Click(null,null);
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
                DataResultADO data = new DataResultADO();
                data.UCRelativeADO = this.ucRelativeInfo1.GetValue();
                data.OldPatient = false;
                dlgReturnData(data);
                this.Close();
                this.dlgSearchPatient1(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DlgFillDataToUCRelative(DelegateSetDataRegisterBeforeSerachPatient _dlgFillData)
        {
            try
            {
                this.dlgSearchPatient1 = _dlgFillData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DlgFillReturnDataForUCPatientRaw(DelegateUpdatePersonHomeInfo _dlgGetData)
        {
            try
            {
                this.dlgReturnData = _dlgGetData;
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
                dlgReturnData = null;
                dlgSearchPatient1 = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                ucRelativeInfo1 = null;
                btnSave = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
