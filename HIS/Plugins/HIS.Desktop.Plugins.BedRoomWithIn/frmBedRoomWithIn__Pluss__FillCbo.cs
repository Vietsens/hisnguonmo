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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Plugins.BedRoomWithIn;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomWithIn
{
    public partial class frmBedRoomWithIn : HIS.Desktop.Utility.FormBase
    {
        List<HIS_PATIENT_CLASSIFY> dataClassiFy;
        private void LoadComboEditor(DevExpress.XtraEditors.GridLookUpEdit cboEditor, string valueCode, string valueName, string valueId, object data)
        {
            try
            {
                cboEditor.Properties.DataSource = data;
                cboEditor.Properties.DisplayMember = valueName;
                cboEditor.Properties.ValueMember = valueId;

                cboEditor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboEditor.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.ForceInitialize();
                cboEditor.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboEditor.Properties.View.Columns.AddField(valueCode);
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboEditor.Properties.View.Columns.AddField(valueName);
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboBedRoom(DevExpress.XtraEditors.GridLookUpEdit cboBedRoom, object data)
        {
            try
            {
              
                cboBedRoom.Properties.DataSource = data;
                cboBedRoom.Properties.DisplayMember = "BED_ROOM_NAME";
                cboBedRoom.Properties.ValueMember = "ID";

                cboBedRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboBedRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboBedRoom.Properties.ImmediatePopup = true;
                cboBedRoom.ForceInitialize();
                cboBedRoom.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboBedRoom.Properties.View.Columns.AddField("BED_ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboBedRoom.Properties.View.Columns.AddField("BED_ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                GridColumn aColumnPATIENT = cboBedRoom.Properties.View.Columns.AddField("TT_PATIENT_BED_STR"); //+ "/" + "BED_COUNT"
                aColumnPATIENT.Caption = "Tổng BN/Tổng giường";
                aColumnPATIENT.Visible = true;
                aColumnPATIENT.VisibleIndex = 3;
                aColumnPATIENT.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        //private void LoadDataToComboTreatmentType(DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType, object data)
        //{
        //    try
        //    {
        //        cboTreatmentType.Properties.DataSource = data;
        //        cboTreatmentType.Properties.DisplayMember = "TREATMENT_TYPE_NAME";
        //        cboTreatmentType.Properties.ValueMember = "ID";

        //        cboTreatmentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        //        cboTreatmentType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
        //        cboTreatmentType.Properties.ImmediatePopup = true;
        //        cboTreatmentType.ForceInitialize();
        //        cboTreatmentType.Properties.View.Columns.Clear();

        //        GridColumn aColumnCode = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_CODE");
        //        aColumnCode.Caption = "Mã";
        //        aColumnCode.Visible = true;
        //        aColumnCode.VisibleIndex = 1;
        //        aColumnCode.Width = 50;

        //        GridColumn aColumnName = cboTreatmentType.Properties.View.Columns.AddField("TREATMENT_TYPE_NAME");
        //        aColumnName.Caption = "Tên";
        //        aColumnName.Visible = true;
        //        aColumnName.VisibleIndex = 2;
        //        aColumnName.Width = 100;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        private void LoadDataToComboPATIENT_CLASSIFY(DevExpress.XtraEditors.GridLookUpEdit cbo , string search)
        {
            try
            {

                //HIS_PATIENT_CLASSIFY 
                dataClassiFy = new List<HIS_PATIENT_CLASSIFY>();
                if (search == null) 
                {
                    dataClassiFy = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.IS_ACTIVE == 1).ToList();
                }
                else
                {
                    dataClassiFy = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.PATIENT_CLASSIFY_CODE.Contains(search) && o.IS_ACTIVE == 1).ToList();
                }
                dataClassiFy = dataClassiFy.Where(o => o.IS_ACTIVE == 1 && (!o.PATIENT_TYPE_ID.HasValue || o.PATIENT_TYPE_ID == currentTreatment.TDL_PATIENT_TYPE_ID)).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_CLASSIFY_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_CLASSIFY_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cbo, dataClassiFy, controlEditorADO);

                if (!string.IsNullOrEmpty(search) && dataClassiFy != null && dataClassiFy.Count > 0) 
                {
                    cbo.EditValue = dataClassiFy[0].ID;
                    txtPATIENT_CLASSIFY.Text = dataClassiFy[0].PATIENT_CLASSIFY_CODE;
                    txtPATIENT_CLASSIFY.Focus();
                    txtPATIENT_CLASSIFY.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentTypeCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType, DevExpress.XtraEditors.TextEdit txtTreatmentType, DevExpress.XtraEditors.TextEdit focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboTreatmentType.EditValue = null;
                    cboTreatmentType.Focus();
                    cboTreatmentType.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => o.TREATMENT_TYPE_CODE.Contains(searchCode)).ToList();
                    List<HIS_TREATMENT_TYPE> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.TREATMENT_TYPE_CODE == searchCode).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboTreatmentType.EditValue = result[0].ID;
                        txtTreatmentType.Text = result[0].TREATMENT_TYPE_CODE;
                        focusControl.Focus();
                        focusControl.SelectAll();
                    }
                    else
                    {
                        cboTreatmentType.EditValue = null;
                        cboTreatmentType.Focus();
                        cboTreatmentType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBedRoomCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboBedRoom, DevExpress.XtraEditors.TextEdit txtBedRoom, DevExpress.XtraEditors.SimpleButton focusControl)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboBedRoom.EditValue = null;
                    cboBedRoom.Focus();
                    cboBedRoom.ShowPopup();
                }
                else
                {
                    var data = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.BED_ROOM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        List<V_HIS_BED_ROOM> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.BED_ROOM_CODE == searchCode).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboBedRoom.EditValue = result[0].ID;
                            txtBedRoom.Text = result[0].BED_ROOM_CODE;
                            focusControl.Focus();
                            LoadCboGiuong();
                        }
                        else
                        {
                            cboBedRoom.EditValue = null;
                            cboBedRoom.Focus();
                            cboBedRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
