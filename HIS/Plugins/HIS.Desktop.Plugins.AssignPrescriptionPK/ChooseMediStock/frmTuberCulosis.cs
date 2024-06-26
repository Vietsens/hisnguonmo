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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseMediStock
{
    public partial class frmTuberCulosis : Form
    {
        private int positionHandle = -1;
        Action<TuberCulosisADO> actionADO;
        long treatmentId; 
        TuberCulosisADO ado;
        public frmTuberCulosis(TuberCulosisADO ado ,Action<TuberCulosisADO> actionADO,long treatmentId)
        {
            try
            {
                InitializeComponent();
                this.ado = ado;
                this.treatmentId = treatmentId;
                this.actionADO = actionADO;
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
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null) return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null) return;

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTuberCulosis_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboMediOrg();
                ValidControl();
                SetDefaultValue();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                cbo.EditValue = BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                if (!string.IsNullOrEmpty(ado.MediOrgCode))
                    cbo.EditValue = ado.MediOrgCode;
                dte.DateTime = DateTime.Now;
                if (ado.TuberCulosisTime != null)
                    dte.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.TuberCulosisTime ?? 0) ?? DateTime.Now;
                btn.Focus();
                btn.Select();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                CommonBaseEditor.ValidationSingleControl(dte,dxValidationProvider1);
                CommonBaseEditor.ValidationSingleControl(cbo, dxValidationProvider1);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboMediOrg()
        {
            try
            {
                var dt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().OrderBy(o => o.MEDI_ORG_CODE).ToList();
                cbo.Properties.DataSource = dt;
                cbo.Properties.DisplayMember = "MEDI_ORG_NAME";
                cbo.Properties.ValueMember = "MEDI_ORG_CODE";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(400, 200);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("MEDI_ORG_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("MEDI_ORG_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 350;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;

                MOS.SDO.HisTreatmentTuberculosisIssuedInfoSDO sdo = new MOS.SDO.HisTreatmentTuberculosisIssuedInfoSDO();
                TuberCulosisADO ado = new TuberCulosisADO();
                ado.MediOrgCode = sdo.TuberculosisIssuedOrgCode = cbo.EditValue.ToString();
                ado.MediOrgName = cbo.Text.ToString();
                ado.TuberCulosisTime = sdo.TuberculosisIssuedDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dte.DateTime);
                CommonParam param = new CommonParam();
                sdo.TreatmentId = treatmentId;
                var dt = new BackendAdapter(param).Post<HisTreatmentTuberculosisIssuedInfoSDO>("api/HisTreatment/UpdateTuberculosisIssuedInfo", ApiConsumers.MosConsumer, sdo, param);
                actionADO(dt!= null ? ado : null);
                MessageManager.Show(this, param, dt != null);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btn_Click(null,null);
        }
    }
}
