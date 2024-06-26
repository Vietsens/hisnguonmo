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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ScnAccidentHurt.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using HIS.Desktop.ApiConsumer;
using SCN.EFMODEL.DataModels;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using SCN.SDO;

namespace HIS.Desktop.Plugins.ScnAccidentHurt
{
    public partial class frmAccidentHurt : HIS.Desktop.Utility.FormBase
    {
        List<HIS_ACCIDENT_BODY_PART> accidentBodyParts { get; set; }
        List<HIS_ACCIDENT_CARE> accidentCares { get; set; }
        List<HIS_ACCIDENT_HELMET> accidentHelmets { get; set; }
        List<HIS_ACCIDENT_HURT_TYPE> accidentHurtTypes { get; set; }
        List<HIS_ACCIDENT_LOCATION> accidentLocations { get; set; }
        List<HIS_ACCIDENT_POISON> accidentPoisons { get; set; }
        List<HIS_ACCIDENT_RESULT> accidentResults { get; set; }
        List<HIS_ACCIDENT_VEHICLE> accidentVehicles { get; set; }
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        public int positionHandle = -1;

        SCN_ACCIDENT_HURT _AccidentHurtRowClick { get; set; }

        int action = 0;

        string _PersonCode = "";

        public frmAccidentHurt()
        {
            InitializeComponent();
        }

        public frmAccidentHurt(Inventec.Desktop.Common.Modules.Module _currentModule, string _personCode)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this._PersonCode = _personCode;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAccidentHurt_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ValidationControl();
                btnNew_Click(null, null);
                LoadDataToCombo();
                LoadDataAccidenHurt();
                LoadKeysFromLanguage();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataAccidenHurt()
        {
            try
            {
                SCN.Filter.ScnAccidentHurtFilter filter = new SCN.Filter.ScnAccidentHurtFilter();
                filter.PERSON_CODE__EXACT = this._PersonCode;
                CommonParam param = new CommonParam();
                var resultData = ApiConsumers.ScnWrapConsumer.Get<List<SCN_ACCIDENT_HURT>>(true, "api/ScnAccidentHurt/Get", param, filter);

                gridControlAccidentHurt.DataSource = null;

                gridControlAccidentHurt.DataSource = resultData;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisAccidentHurtViewFilter AccidentHurtFilter = new MOS.Filter.HisAccidentHurtViewFilter();
                accidentBodyParts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_BODY_PART>>(
                    "/api/HisAccidentBodyPart/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentCares = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_CARE>>(
                    "/api/HisAccidentCare/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentHelmets = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_HELMET>>(
                    "/api/HisAccidentHelmet/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentHurtTypes = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_HURT_TYPE>>(
                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ACCIDENT_HURT_TYPE_GET,
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentLocations = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_LOCATION>>(
                    "/api/HisAccidentLocation/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentPoisons = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_POISON>>(
                    "/api/HisAccidentPoison/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentResults = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_RESULT>>(
                    "/api/HisAccidentResult/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentVehicles = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_VEHICLE>>(
                    "/api/HisAccidentVehicle/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                LoadDataToComboAccidentBodyPart(cboAccidentBodyPart, accidentBodyParts);


                LoadDataToComboAccidentCare(cboAccidentCare, accidentCares);


                LoadDataToComboAccidentHelmet(cboHelmet, accidentHelmets);


                LoadDataToComboAccidentHurtType(cboAccidentHurtType, accidentHurtTypes);


                LoadDataToComboAccidentLocation(cboAccidentLocaltion, accidentLocations);


                LoadDataToComboAccidentPoison(cboAccidentPoison, accidentPoisons);


                LoadDataToComboAccidentResult(cboAccidentResult, accidentResults);


                LoadDataToComboAccidentVehicle(cboAccidentVehicle, accidentVehicles);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccidentLocaltion_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentLocaltion.EditValue != null)
                    {
                        HIS_ACCIDENT_LOCATION data = accidentLocations.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentLocaltion.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentLocaltion.Text = data.ACCIDENT_LOCATION_CODE;
                            cboAccidentLocaltion.Properties.Buttons[1].Visible = true;
                            txtAccidentBodyPart.Focus();
                            txtAccidentBodyPart.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentBodyPart_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentBodyPart.EditValue != null)
                    {
                        HIS_ACCIDENT_BODY_PART data = accidentBodyParts.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentBodyPart.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentBodyPart.Text = data.ACCIDENT_BODY_PART_CODE;
                            cboAccidentBodyPart.Properties.Buttons[1].Visible = true;
                            txtAccidentHurtType.Focus();
                            txtAccidentHurtType.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentHurtType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentHurtType.EditValue != null)
                    {
                        HIS_ACCIDENT_HURT_TYPE data = accidentHurtTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentHurtType.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentHurtType.Text = data.ACCIDENT_HURT_TYPE_CODE;
                            cboAccidentHurtType.Properties.Buttons[1].Visible = true;
                            txtAccidentResult.Focus();
                            txtAccidentResult.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentPoison_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentPoison.EditValue != null)
                    {
                        HIS_ACCIDENT_POISON data = accidentPoisons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentPoison.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentPoison.Text = data.ACCIDENT_POISON_CODE;
                            cboAccidentPoison.Properties.Buttons[1].Visible = true;
                            txtAccidentVehicle.Focus();
                            txtAccidentVehicle.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHelmet_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHelmet.EditValue != null)
                    {
                        HIS_ACCIDENT_HELMET data = accidentHelmets.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboHelmet.EditValue.ToString()));
                        if (data != null)
                        {
                            txtHelmet.Text = data.ACCIDENT_HELMET_CODE;
                            cboHelmet.Properties.Buttons[1].Visible = true;
                            chkUseAlcohol.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentVehicle_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentVehicle.EditValue != null)
                    {
                        HIS_ACCIDENT_VEHICLE data = accidentVehicles.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentVehicle.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentVehicle.Text = data.ACCIDENT_VEHICLE_CODE;
                            cboAccidentVehicle.Properties.Buttons[1].Visible = true;
                            txtHelmet.Focus();
                            txtHelmet.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentResult_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentResult.EditValue != null)
                    {
                        HIS_ACCIDENT_RESULT data = accidentResults.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentResult.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentResult.Text = data.ACCIDENT_RESULT_CODE;
                            cboAccidentResult.Properties.Buttons[1].Visible = true;
                            txtAccidentCare.Focus();
                            txtAccidentCare.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentCare_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentCare.EditValue != null)
                    {
                        HIS_ACCIDENT_CARE data = accidentCares.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentCare.EditValue.ToString()));
                        if (data != null)
                        {
                            cboAccidentCare.Properties.Buttons[1].Visible = true;
                            txtAccidentCare.Text = data.ACCIDENT_CARE_CODE;
                            txtAccidentPoison.Focus();
                            txtAccidentPoison.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtAccidentTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtAccidentTime.EditValue != null)
                    {
                        txtAccidentLocaltion.Focus();
                        txtAccidentLocaltion.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentLocaltion_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentLocation(strValue, cboAccidentLocaltion, txtAccidentLocaltion, txtAccidentBodyPart);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtAccidentBodyPart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentBodyPart(strValue, cboAccidentBodyPart, txtAccidentBodyPart, txtAccidentHurtType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentHurtType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentHurtType(strValue, cboAccidentHurtType, txtAccidentHurtType, txtContent);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentPoison_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentPoison(strValue, cboAccidentPoison, txtAccidentPoison, txtHelmet);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHelmet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentHelmet(strValue, cboHelmet, txtHelmet, txtAccidentVehicle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentVehicle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentVehicle(strValue, cboAccidentVehicle, txtAccidentVehicle, txtAccidentResult);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentResult(strValue, cboAccidentResult, txtAccidentResult, txtAccidentCare);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentCare_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentCare(strValue, cboAccidentCare, txtAccidentCare, btnSave);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtAccidentTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(dtAccidentTime.Text))
                    {
                        try
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                txtAccidentLocaltion.Focus();
                                txtAccidentLocaltion.SelectAll();
                            }
                            if (e.KeyCode == Keys.Tab)
                            {
                                String enddatetime = Inventec.Common.TypeConvert.Parse.ToDateTime(dtAccidentTime.Text).ToString("yyyyMMddHHmm");
                                long endTime = Inventec.Common.TypeConvert.Parse.ToInt64(enddatetime + "00");
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        dtAccidentTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentHurtType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentResult.Focus();
                    txtAccidentResult.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentLocaltion_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentBodyPart.Focus();
                    txtAccidentBodyPart.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentBodyPart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentHurtType.Focus();
                    txtAccidentHurtType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentPoison_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHelmet.Focus();
                    txtHelmet.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHelmet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentVehicle.Focus();
                    txtAccidentVehicle.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentVehicle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkUseAlcohol.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUseAlcohol_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtContent.Focus();
                    txtContent.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentCare.Focus();
                    txtAccidentCare.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentCare_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentPoison.Focus();
                    txtAccidentPoison.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentLocaltion_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentLocaltion.Properties.Buttons[1].Visible = false;
                    cboAccidentLocaltion.EditValue = null;
                    txtAccidentLocaltion.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentBodyPart_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentBodyPart.Properties.Buttons[1].Visible = false;
                    cboAccidentBodyPart.EditValue = null;
                    txtAccidentBodyPart.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentHurtType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentHurtType.Properties.Buttons[1].Visible = false;
                    cboAccidentHurtType.EditValue = null;
                    txtAccidentHurtType.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentPoison_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentPoison.Properties.Buttons[1].Visible = false;
                    cboAccidentPoison.EditValue = null;
                    txtAccidentPoison.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHelmet_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHelmet.Properties.Buttons[1].Visible = false;
                    cboHelmet.EditValue = null;
                    txtHelmet.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentVehicle_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentVehicle.Properties.Buttons[1].Visible = false;
                    cboAccidentVehicle.EditValue = null;
                    txtAccidentVehicle.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentResult_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentResult.Properties.Buttons[1].Visible = false;
                    cboAccidentResult.EditValue = null;
                    txtAccidentResult.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentCare_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentCare.Properties.Buttons[1].Visible = false;
                    cboAccidentCare.EditValue = null;
                    txtAccidentCare.Text = "";
                }

                HisAccidentHurtFilter filter = new HisAccidentHurtFilter();

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
                //ValidationControl();
                //this.positionHandle = -1;
                //if (!dxValidationProvider.Validate())
                //{
                //    return;
                //}

                WaitingManager.Show(this);
                CommonParam param = new CommonParam();

                HID.EFMODEL.DataModels.HID_PERSON _HidPerson = new HID.EFMODEL.DataModels.HID_PERSON();
                _HidPerson = new Inventec.Common.Adapter.BackendAdapter(param).Get<HID.EFMODEL.DataModels.HID_PERSON>(
                    "/api/HidPerson/GetByPersonCode",
                HIS.Desktop.ApiConsumer.ApiConsumers.HidConsumer,
                this._PersonCode,
                param);

                SCN.EFMODEL.DataModels.SCN_ACCIDENT_HURT _AccidentHurt = new SCN_ACCIDENT_HURT();
                if (_HidPerson != null)
                {
                    HID.Filter.HidGenderFilter filterGender = new HID.Filter.HidGenderFilter();
                    filterGender.ID = _HidPerson.GENDER_ID;
                    var _HidGender = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HID.EFMODEL.DataModels.HID_GENDER>>(
                           "/api/HidGender/Get",
                       HIS.Desktop.ApiConsumer.ApiConsumers.HidConsumer,
                       filterGender,
                       param);
                    _AccidentHurt.GENDER_NAME = (_HidGender != null && _HidGender.Count > 0) ? _HidGender[0].GENDER_NAME : "";
                    _AccidentHurt.FIRST_NAME = _HidPerson.FIRST_NAME;
                    _AccidentHurt.LAST_NAME = _HidPerson.LAST_NAME;
                    _AccidentHurt.DOB = _HidPerson.DOB;
                    _AccidentHurt.IS_HAS_NOT_DAY_DOB = _HidPerson.IS_HAS_NOT_DAY_DOB;
                    _AccidentHurt.PERSON_ADDRESS = _HidPerson.VIR_ADDRESS; //(Lấy từ VIR_ADDRESS)
                    _AccidentHurt.CAREER_NAME = _HidPerson.CAREER_NAME;
                    _AccidentHurt.ETHNIC_NAME = _HidPerson.ETHNIC_NAME;
                }

                _AccidentHurt.PERSON_CODE = this._PersonCode;


                if (this.dtAccidentTime.EditValue != null)
                {
                    _AccidentHurt.ACCIDENT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAccidentTime.DateTime);
                }

                if (cboAccidentLocaltion.EditValue != null)
                {
                    var data = this.accidentLocations.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentLocaltion.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_LOCATION_NAME = data != null ? data.ACCIDENT_LOCATION_NAME : "";
                }
                if (cboAccidentBodyPart.EditValue != null)
                {
                    var data = this.accidentBodyParts.FirstOrDefault(p => p.ID == (Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentBodyPart.EditValue.ToString())));
                    _AccidentHurt.ACCIDENT_BODY_PART_NAME = data != null ? data.ACCIDENT_BODY_PART_NAME : "";
                }
                if (cboAccidentHurtType.EditValue != null)
                {
                    var data = this.accidentHurtTypes.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentHurtType.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_HURT_TYPE_NAME = data != null ? data.ACCIDENT_HURT_TYPE_NAME : "";
                }

                if (cboAccidentPoison.EditValue != null)
                {
                    var data = this.accidentPoisons.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentPoison.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_POISON_NAME = data != null ? data.ACCIDENT_POISON_NAME : "";
                }

                if (cboHelmet.EditValue != null)
                {
                    var data = this.accidentHelmets.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboHelmet.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_HELMET_NAME = data != null ? data.ACCIDENT_HELMET_NAME : "";
                }
                if (cboAccidentVehicle.EditValue != null)
                {
                    var data = this.accidentVehicles.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentVehicle.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_VEHICLE_NAME = data != null ? data.ACCIDENT_VEHICLE_NAME : "";
                }
                if (chkUseAlcohol.Checked)
                {
                    _AccidentHurt.IS_USE_ALCOHOL = 1;
                }
                if (cboAccidentResult.EditValue != null)
                {
                    var data = this.accidentResults.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentResult.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_RESULT_NAME = data != null ? data.ACCIDENT_RESULT_NAME : "";
                }
                if (cboAccidentCare.EditValue != null)
                {
                    var data = this.accidentCares.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentCare.EditValue.ToString()));
                    _AccidentHurt.ACCIDENT_CARE_NAME = data != null ? data.ACCIDENT_CARE_NAME : "";
                }
                _AccidentHurt.CONTENT = txtContent.Text;
                if (this._AccidentHurtRowClick != null && this._AccidentHurtRowClick.ID > 0)
                    _AccidentHurt.ID = this._AccidentHurtRowClick.ID;

                //SCN_DEATH _death = new SCN_DEATH();
                //_death.PERSON_CODE = this._PersonCode;
                //_death.DEATH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAccidentTime.DateTime) ?? 0;
                //DeathAndAccidentHurtSDO sdo = new DeathAndAccidentHurtSDO();
                //sdo.Deaths = new List<SCN_DEATH>();
                //sdo.Deaths.Add(_death);

                //sdo.AccidentHurts = new List<SCN_ACCIDENT_HURT>();
                //sdo.AccidentHurts.Add(_AccidentHurt);

                //var result123 = ApiConsumers.ScnWrapConsumer.Post<bool>(true, "api/ScnPersonalHealth/CreateSDO", param, sdo);


                //API
                bool success = false;
                SCN_ACCIDENT_HURT _result = new SCN_ACCIDENT_HURT();
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    _result = ApiConsumers.ScnWrapConsumer.Post<SCN_ACCIDENT_HURT>(true, "api/ScnAccidentHurt/Update", param, _AccidentHurt);
                }
                else
                {
                    _result = ApiConsumers.ScnWrapConsumer.Post<SCN_ACCIDENT_HURT>(true, "api/ScnAccidentHurt/Create", param, _AccidentHurt);
                }

                if (_result != null)
                {
                    success = true;
                    this._AccidentHurtRowClick = _result;
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    this.LoadDataAccidenHurt();
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridViewAccidentHurt_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SCN_ACCIDENT_HURT data = (SCN_ACCIDENT_HURT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "ACCIDENT_TIME_DISPLAY")
                        {
                            if (data.ACCIDENT_TIME.HasValue)
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ACCIDENT_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccidentHurt_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                this.btnSave.Text = "Sửa (Ctrl S)";
                this._AccidentHurtRowClick = new SCN_ACCIDENT_HURT();

                var _accidentHurt = (SCN_ACCIDENT_HURT)gridViewAccidentHurt.GetFocusedRow();
                if (_accidentHurt != null)
                {
                    this._AccidentHurtRowClick = _accidentHurt;
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    if (_accidentHurt.ACCIDENT_TIME.HasValue)
                        dtAccidentTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)_accidentHurt.ACCIDENT_TIME) ?? DateTime.MinValue;
                    else
                        dtAccidentTime.EditValue = null;

                    txtContent.Text = _accidentHurt.CONTENT;

                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_LOCATION_NAME))
                    {
                        var data = this.accidentLocations.FirstOrDefault(p => p.ACCIDENT_LOCATION_NAME == _accidentHurt.ACCIDENT_LOCATION_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentLocaltion.Text = data.ACCIDENT_LOCATION_CODE;
                            cboAccidentLocaltion.EditValue = data.ID;
                            cboAccidentLocaltion.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentLocaltion.Text = "";
                        cboAccidentLocaltion.Properties.Buttons[1].Visible = false;
                        cboAccidentLocaltion.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_BODY_PART_NAME))
                    {
                        var data = this.accidentBodyParts.FirstOrDefault(p => p.ACCIDENT_BODY_PART_NAME == _accidentHurt.ACCIDENT_BODY_PART_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentBodyPart.Text = data.ACCIDENT_BODY_PART_CODE;
                            cboAccidentBodyPart.EditValue = data.ID;
                            cboAccidentBodyPart.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentBodyPart.Text = "";
                        cboAccidentBodyPart.Properties.Buttons[1].Visible = false;
                        cboAccidentBodyPart.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_HURT_TYPE_NAME))
                    {
                        var data = this.accidentHurtTypes.FirstOrDefault(p => p.ACCIDENT_HURT_TYPE_NAME == _accidentHurt.ACCIDENT_HURT_TYPE_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentHurtType.Text = data.ACCIDENT_HURT_TYPE_CODE;
                            cboAccidentHurtType.EditValue = data.ID;
                            cboAccidentHurtType.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentHurtType.Text = "";
                        cboAccidentHurtType.Properties.Buttons[1].Visible = false;
                        cboAccidentHurtType.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_POISON_NAME))
                    {
                        var data = this.accidentPoisons.FirstOrDefault(p => p.ACCIDENT_POISON_NAME == _accidentHurt.ACCIDENT_POISON_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentPoison.Text = data.ACCIDENT_POISON_CODE;
                            cboAccidentPoison.EditValue = data.ID;
                            cboAccidentPoison.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentPoison.Text = "";
                        cboAccidentPoison.Properties.Buttons[1].Visible = false;
                        cboAccidentPoison.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_HELMET_NAME))
                    {
                        var data = this.accidentHelmets.FirstOrDefault(p => p.ACCIDENT_HELMET_NAME == _accidentHurt.ACCIDENT_HELMET_NAME.Trim());
                        if (data != null)
                        {
                            txtHelmet.Text = data.ACCIDENT_HELMET_CODE;
                            cboHelmet.EditValue = data.ID;
                            cboHelmet.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtHelmet.Text = "";
                        cboHelmet.Properties.Buttons[1].Visible = false;
                        cboHelmet.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_VEHICLE_NAME))
                    {
                        var data = this.accidentVehicles.FirstOrDefault(p => p.ACCIDENT_VEHICLE_NAME == _accidentHurt.ACCIDENT_VEHICLE_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentVehicle.Text = data.ACCIDENT_VEHICLE_CODE;
                            cboAccidentVehicle.EditValue = data.ID;
                            cboAccidentVehicle.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentVehicle.Text = "";
                        cboAccidentVehicle.Properties.Buttons[1].Visible = false;
                        cboAccidentVehicle.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_RESULT_NAME))
                    {
                        var data = this.accidentResults.FirstOrDefault(p => p.ACCIDENT_RESULT_NAME == _accidentHurt.ACCIDENT_RESULT_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentResult.Text = data.ACCIDENT_RESULT_CODE;
                            cboAccidentResult.EditValue = data.ID;
                            cboAccidentResult.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentResult.Text = "";
                        cboAccidentResult.Properties.Buttons[1].Visible = false;
                        cboAccidentResult.EditValue = null;
                    }

                    if (!string.IsNullOrEmpty(_accidentHurt.ACCIDENT_CARE_NAME))
                    {
                        var data = this.accidentCares.FirstOrDefault(p => p.ACCIDENT_CARE_NAME == _accidentHurt.ACCIDENT_CARE_NAME.Trim());
                        if (data != null)
                        {
                            txtAccidentCare.Text = data.ACCIDENT_CARE_CODE;
                            cboAccidentCare.EditValue = data.ID;
                            cboAccidentCare.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                    {
                        txtAccidentCare.Text = "";
                        cboAccidentCare.Properties.Buttons[1].Visible = false;
                        cboAccidentCare.EditValue = null;
                    }

                    if (_accidentHurt.IS_USE_ALCOHOL == 1)
                    {
                        chkUseAlcohol.Checked = true;
                    }
                    else
                        chkUseAlcohol.Checked = false;
                }
                else
                {
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnSave.Text = "Lưu (Ctrl S)";
                this._AccidentHurtRowClick = new SCN_ACCIDENT_HURT();
                this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                dtAccidentTime.DateTime = DateTime.Now;

                txtContent.Text = "";

                txtAccidentLocaltion.Text = "";
                cboAccidentLocaltion.EditValue = null;
                cboAccidentLocaltion.Properties.Buttons[1].Visible = false;
                txtAccidentBodyPart.Text = "";
                cboAccidentBodyPart.EditValue = null;
                cboAccidentBodyPart.Properties.Buttons[1].Visible = false;
                txtAccidentHurtType.Text = "";
                cboAccidentHurtType.EditValue = null;
                cboAccidentHurtType.Properties.Buttons[1].Visible = false;
                txtAccidentPoison.Text = "";
                cboAccidentPoison.EditValue = null;
                cboAccidentPoison.Properties.Buttons[1].Visible = false;
                txtHelmet.Text = "";
                cboHelmet.EditValue = null;
                cboHelmet.Properties.Buttons[1].Visible = false;
                txtAccidentVehicle.Text = "";
                cboAccidentVehicle.EditValue = null;
                cboAccidentVehicle.Properties.Buttons[1].Visible = false;
                txtAccidentResult.Text = "";
                cboAccidentResult.EditValue = null;
                cboAccidentResult.Properties.Buttons[1].Visible = false;

                txtAccidentCare.Text = "";
                cboAccidentCare.EditValue = null;
                cboAccidentCare.Properties.Buttons[1].Visible = false;

                chkUseAlcohol.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
