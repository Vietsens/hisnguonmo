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
using DevExpress.XtraGrid.Columns;
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

namespace HIS.Desktop.Plugins.ScnAccidentHurt
{
    public partial class frmAccidentHurt : HIS.Desktop.Utility.FormBase
    {
        public static void LoadDataToComboAccidentBodyPart(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_BODY_PART_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_BODY_PART_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_BODY_PART_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentCare(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_CARE_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_CARE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_CARE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentHelmet(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_HELMET_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_HELMET_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_HELMET_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentHurtType(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_HURT_TYPE_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_HURT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_HURT_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentLocation(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_LOCATION_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_LOCATION_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_LOCATION_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentPoison(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_POISON_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_POISON_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_POISON_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentResult(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_RESULT_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_RESULT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_RESULT_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboAccidentVehicle(DevExpress.XtraEditors.GridLookUpEdit cbo, object data)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ACCIDENT_VEHICLE_NAME";
                cbo.Properties.ValueMember = "ID";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ACCIDENT_VEHICLE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ACCIDENT_VEHICLE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentLocation(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentLocations.Where(o => o.ACCIDENT_LOCATION_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_LOCATION_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentBodyPart(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentBodyParts.Where(o => o.ACCIDENT_BODY_PART_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_BODY_PART_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentHurtType(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentHurtTypes.Where(o => o.ACCIDENT_HURT_TYPE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_HURT_TYPE_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentPoison(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentPoisons.Where(o => o.ACCIDENT_POISON_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_POISON_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentHelmet(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentHelmets.Where(o => o.ACCIDENT_HELMET_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_HELMET_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentVehicle(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentVehicles.Where(o => o.ACCIDENT_VEHICLE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_VEHICLE_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentResult(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.TextEdit txtFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentResults.Where(o => o.ACCIDENT_RESULT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_RESULT_CODE;
                            txtFocus.Focus();
                            txtFocus.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadComboAccidentCare(string searchCode, DevExpress.XtraEditors.GridLookUpEdit cbo, DevExpress.XtraEditors.TextEdit txtTextEdit, DevExpress.XtraEditors.SimpleButton btnFocus)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cbo.EditValue = null;
                    cbo.Focus();
                    cbo.ShowPopup();
                }
                else
                {
                    var data = accidentCares.Where(o => o.ACCIDENT_CARE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cbo.EditValue = data[0].ID;
                            txtTextEdit.Text = data[0].ACCIDENT_CARE_CODE;
                            btnFocus.Focus();

                        }
                        else if (data.Count > 1)
                        {
                            cbo.EditValue = null;
                            cbo.Focus();
                            cbo.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                        }
                    }
                    else
                    {
                        cbo.EditValue = null;
                        cbo.Focus();
                        cbo.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboMediOrg);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadKeysFromLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ScnAccidentHurt.Resources.Lang", typeof(HIS.Desktop.Plugins.ScnAccidentHurt.frmAccidentHurt).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmAccidentHurt.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentHurtType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentHurtType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentLocaltion.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentLocaltion.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentCare.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentCare.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentResult.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentBodyPart.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentBodyPart.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
               
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentPoison.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentPoison.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHelmet.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboHelmet.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAccidentVehicle.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAccidentHurt.cboAccidentVehicle.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUseAlcohol.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.chkUseAlcohol.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciContent.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciContent.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccidentLocaltion.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciAccidentLocaltion.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccidentBodyPart.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciAccidentBodyPart.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccidentResult.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciAccidentResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccidentCare.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciAccidentCare.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccidentHurtType.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciAccidentHurtType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAccidentTime.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.lciAccidentTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmAccidentHurt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
