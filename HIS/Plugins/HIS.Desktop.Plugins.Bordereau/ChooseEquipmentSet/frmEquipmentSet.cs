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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Bordereau.Base;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChooseEquipmentSet
{

    public partial class frmEquipmentSet : Form
    {
        public enum CONTROL_TYPE
        {
            DEFAULT,
            HIDE_NUMORDER
        }

        public int positionHandle = -1;
        CONTROL_TYPE? controlType;
        public long? equipmentId;
        public long? numOrder;
        public bool success;


        public frmEquipmentSet(long? _equipmentId, long? _numOrder, [Optional]CONTROL_TYPE? _controlType)
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
            try
            {
                this.equipmentId = _equipmentId;
                this.numOrder = _numOrder;
                this.controlType = _controlType;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmEquipmentSet_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                LoadComboEquipment();

                LoadDataDefault();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool valid = true;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (valid)
                {
                    if (spinNumOrder.EditValue != null)
                        this.numOrder = (long)spinNumOrder.Value;
                    else
                        this.numOrder = null;

                    if (cboEquipmentSet.EditValue != null)
                        this.equipmentId = Inventec.Common.TypeConvert.Parse.ToInt64(cboEquipmentSet.EditValue.ToString());
                    else
                    {
                        this.numOrder = null;
                        this.equipmentId = null;
                    }

                    this.success = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboEquipmentSet_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEquipmentSet.EditValue != null)
                {
                    cboEquipmentSet.Properties.Buttons[1].Visible = true;
                    List<HIS_EQUIPMENT_SET> listEquipment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true);
                    if (listEquipment == null)
                        return;
                    HIS_EQUIPMENT_SET equipmentSet = listEquipment.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboEquipmentSet.EditValue.ToString()));
                    if (equipmentSet != null)
                    {
                        lblGiaTran.Text = Inventec.Common.Number.Convert.NumberToString(equipmentSet.HEIN_SET_LIMIT_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else
                    {
                        lblGiaTran.Text = "";
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEquipmentSet_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
            {
                cboEquipmentSet.EditValue = null;
                cboEquipmentSet.Properties.Buttons[1].Visible = false;
            }
        }

        private void cboEquipmentSet_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboEquipmentSet.EditValue != null)
                {
                    spinNumOrder.Focus();
                    spinNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void cboEquipmentSet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNumOrder.Focus();
                    spinNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSave.Focus();
                    }
                   
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
			
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinNumOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.Focus();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.layoutControl1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.btnSave.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.cboEquipmentSet.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEquipmentSet.cboEquipmentSet.Properties.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.layoutControlItem1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.lblNumOrder.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.lblNumOrder.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.bar1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.barButtonItemCtrlS.Caption = Inventec.Common.Resource.Get.Value("frmEquipmentSet.barButtonItemCtrlS.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.layoutControlItem2.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmEquipmentSet.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmEquipmentSet_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                DisposeControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
