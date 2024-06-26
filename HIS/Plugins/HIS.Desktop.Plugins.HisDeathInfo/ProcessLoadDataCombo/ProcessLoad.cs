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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisDeathInfo.ProcessLoadDataCombo
{
    public class ProcessLoad
    {
        //Tử vong
        public static void LoadDataToComboDeathCause(DevExpress.XtraEditors.GridLookUpEdit cboDeathCause, object data)
        {
            try
            {
                cboDeathCause.Properties.DataSource = data;
                cboDeathCause.Properties.DisplayMember = "DEATH_CAUSE_NAME";
                cboDeathCause.Properties.ValueMember = "ID";

                cboDeathCause.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboDeathCause.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboDeathCause.Properties.ImmediatePopup = true;
                cboDeathCause.ForceInitialize();
                cboDeathCause.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboDeathCause.Properties.View.Columns.AddField("DEATH_CAUSE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboDeathCause.Properties.View.Columns.AddField("DEATH_CAUSE_NAME");
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

        public static void LoadDataToComboDeathWithin(DevExpress.XtraEditors.GridLookUpEdit cboDeathCause, object data)
        {
            try
            {
                cboDeathCause.Properties.DataSource = data;
                cboDeathCause.Properties.DisplayMember = "DEATH_WITHIN_NAME";
                cboDeathCause.Properties.ValueMember = "ID";

                cboDeathCause.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboDeathCause.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboDeathCause.Properties.ImmediatePopup = true;
                cboDeathCause.ForceInitialize();
                cboDeathCause.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboDeathCause.Properties.View.Columns.AddField("DEATH_WITHIN_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboDeathCause.Properties.View.Columns.AddField("DEATH_WITHIN_NAME");
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

        internal static void LoadDeathCauseCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboDeathCause, DevExpress.XtraEditors.TextEdit txtDeathCause, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDeathCause.EditValue = null;
                    cboDeathCause.Focus();
                    cboDeathCause.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboDeathCause);
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_DEATH_CAUSE>().Where(o => o.DEATH_CAUSE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDeathCause.EditValue = data[0].ID;
                            txtDeathCause.Text = data[0].DEATH_CAUSE_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                            cboDeathCause.Properties.Buttons[1].Visible = true;
                        }
                        else if (data.Count > 1)
                        {
                            cboDeathCause.EditValue = null;
                            cboDeathCause.Focus();
                            cboDeathCause.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboDeathCause);
                        }
                    }
                    else
                    {
                        cboDeathCause.EditValue = null;
                        cboDeathCause.Focus();
                        cboDeathCause.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboDeathCause);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDeathWithinCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboDeathWithin, DevExpress.XtraEditors.TextEdit txtDeathWithin, DevExpress.XtraEditors.CheckEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDeathWithin.EditValue = null;
                    cboDeathWithin.Focus();
                    cboDeathWithin.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboDeathWithin);
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_DEATH_WITHIN>().Where(o => o.DEATH_WITHIN_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDeathWithin.EditValue = data[0].ID;
                            txtDeathWithin.Text = data[0].DEATH_WITHIN_CODE;
                            focusControl.Focus();
                            focusControl.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cboDeathWithin.EditValue = null;
                            cboDeathWithin.Focus();
                            cboDeathWithin.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboDeathWithin);
                        }
                    }
                    else
                    {
                        cboDeathWithin.EditValue = null;
                        cboDeathWithin.Focus();
                        cboDeathWithin.ShowPopup();
                        //PopupProcess.SelectFirstRowPopup(control.cboDeathWithin);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
