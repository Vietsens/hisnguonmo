﻿using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Loader
{
    class MaterialTypeLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit1)
        {
            try
            {
                comboBoxEdit1.Properties.DataSource = Config.HisFormTypeConfig.VHisMaterialTypes;
                comboBoxEdit1.Properties.DisplayMember = "MATERIAL_TYPE_NAME";
                comboBoxEdit1.Properties.ValueMember = "ID";

                comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboBoxEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboBoxEdit1.Properties.ImmediatePopup = true;
                comboBoxEdit1.ForceInitialize();
                comboBoxEdit1.Properties.View.Columns.Clear();
                comboBoxEdit1.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = comboBoxEdit1.Properties.View.Columns.AddField("MATERIAL_TYPE_NAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = comboBoxEdit1.Properties.View.Columns.AddField("MATERIAL_TYPE_CODE");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo1(DevExpress.XtraEditors.GridLookUpEdit comboBoxEdit1)
        {
            try
            {
                comboBoxEdit1.Properties.DataSource = Config.HisFormTypeConfig.VHisMedicineTypes;
                comboBoxEdit1.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                comboBoxEdit1.Properties.ValueMember = "ID";

                comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                comboBoxEdit1.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                comboBoxEdit1.Properties.ImmediatePopup = true;
                comboBoxEdit1.ForceInitialize();
                comboBoxEdit1.Properties.View.Columns.Clear();
                comboBoxEdit1.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = comboBoxEdit1.Properties.View.Columns.AddField("MEDICINE_TYPE_NAME");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = comboBoxEdit1.Properties.View.Columns.AddField("MEDICINE_TYPE_CODE");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboRoom, List<MOS.EFMODEL.DataModels.V_HIS_ROOM> listData)
        {
            try
            {
                cboRoom.Properties.DataSource = listData;
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";

                cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.ForceInitialize();
                cboRoom.Properties.View.Columns.Clear();
                cboRoom.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboRoom.Properties.View.Columns.AddField("ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboRoom.Properties.View.Columns.AddField("ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboRoom)
        {
            try
            {
                cboRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisRooms;
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";
                cboRoom.Properties.ForceInitialize();
                cboRoom.Properties.Columns.Clear();
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 100));
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 200));
                cboRoom.Properties.ShowHeader = false;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.Properties.DropDownRows = 10;
                cboRoom.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToCombo(DevExpress.XtraEditors.LookUpEdit cboRoom, List<MOS.EFMODEL.DataModels.V_HIS_ROOM> listData)
        {
            try
            {
                cboRoom.Properties.DataSource = listData;
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";
                cboRoom.Properties.ForceInitialize();
                cboRoom.Properties.Columns.Clear();
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_CODE", "", 100));
                cboRoom.Properties.Columns.Add(new LookUpColumnInfo("ROOM_NAME", "", 200));
                cboRoom.Properties.ShowHeader = false;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.Properties.DropDownRows = 10;
                cboRoom.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
