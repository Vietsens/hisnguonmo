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
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.ChooseServiceType
{
    public partial class FormChooseServiceType : Form
    {
        List<HIS_SERE_SERV> ListSereServ;
        Action<List<HIS_SERE_SERV>> ChooseData;

        List<HIS_SERVICE_TYPE> ListServiceTypeSelected;
        List<HIS_PATIENT_TYPE> ListPatientTypeSelected;

        public FormChooseServiceType(List<HIS_SERE_SERV> listSereServ, Action<List<HIS_SERE_SERV>> chooseData)
        {
            InitializeComponent();
            this.ListSereServ = listSereServ;
            this.ChooseData = chooseData;
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

        private void FormChooseServiceType_Load(object sender, EventArgs e)
        {
            try
            {
                this.LostFocus += FormChooseServiceType_LostFocus;
                InitCboCheck(this.cboPatientType, SelectionGrid__PatientType);
                InitCboCheck(this.cboServiceType, SelectionGrid__ServiceType);

                List<HIS_SERVICE_TYPE> ListServiceTypes = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<HIS_PATIENT_TYPE> ListPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();

                //Hiển thị các loại và đối tượng có trong dịch vụ
                if (this.ListSereServ != null && this.ListSereServ.Count > 0)
                {
                    ListServiceTypes = ListServiceTypes.Where(o => this.ListSereServ.Exists(p => p.TDL_SERVICE_TYPE_ID == o.ID)).ToList();
                    ListPatientTypes = ListPatientTypes.Where(o => this.ListSereServ.Exists(p => p.PATIENT_TYPE_ID == o.ID)).ToList();
                }

                InitComboData(this.cboPatientType, ListPatientTypes, "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE", "ID");
                InitComboData(this.cboServiceType, ListServiceTypes, "SERVICE_TYPE_NAME", "SERVICE_TYPE_CODE", "ID");

                cboServiceType.Select();
                cboPatientType.Select();
                btnChoose.Select();
                btnChoose.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void FormChooseServiceType_LostFocus(object sender, EventArgs e)
        {
            try
            {
                this.cboPatientType.ClosePopup();
                this.cboServiceType.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__PatientType(object sender, EventArgs e)
        {
            try
            {
                ListPatientTypeSelected = new List<HIS_PATIENT_TYPE>();
                foreach (HIS_PATIENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        ListPatientTypeSelected.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ServiceType(object sender, EventArgs e)
        {
            try
            {
                ListServiceTypeSelected = new List<HIS_SERVICE_TYPE>();
                foreach (HIS_SERVICE_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        ListServiceTypeSelected.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboCheck(DevExpress.XtraEditors.GridLookUpEdit cbo, HIS.Desktop.Utilities.Extensions.GridCheckMarksSelection.SelectionChangedEventHandler SelectionChanged)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionChanged);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboData(DevExpress.XtraEditors.GridLookUpEdit cbo, object datas, string displayMember, string code, string valueMember)
        {
            try
            {
                cbo.Properties.DataSource = datas;
                cbo.Properties.DisplayMember = displayMember;
                cbo.Properties.ValueMember = valueMember;
                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(code);
                col1.VisibleIndex = 1;
                col1.Width = 80;
                col1.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(displayMember);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = "";
                cbo.Properties.PopupFormWidth = 300;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && datas != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                    gridCheckMark.SelectAll(datas);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ListSereServ != null && this.ListSereServ.Count > 0 && this.ChooseData != null)
                {
                    if (this.ListPatientTypeSelected != null && this.ListPatientTypeSelected.Count > 0)
                    {
                        this.ListSereServ = this.ListSereServ.Where(o => this.ListPatientTypeSelected.Exists(p => p.ID == o.PATIENT_TYPE_ID)).ToList();
                    }

                    if (this.ListServiceTypeSelected != null && this.ListServiceTypeSelected.Count > 0)
                    {
                        this.ListSereServ = this.ListSereServ.Where(o => this.ListServiceTypeSelected.Exists(p => p.ID == o.TDL_SERVICE_TYPE_ID)).ToList();
                    }

                    this.ChooseData(this.ListSereServ);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboServiceType.Focus();
                cboPatientType.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboPatientType.Focus();
                btnChoose.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
