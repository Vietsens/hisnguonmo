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
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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
using MOS.SDO;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.UpdatePatientClassify
{
    public partial class frmUpdatePatientClassify : HIS.Desktop.Utility.FormBase
    {
        MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM listBedRoom;
        private int positionHandleControlPatientInfo;

        public frmUpdatePatientClassify(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmUpdatePatientClassify(Inventec.Desktop.Common.Modules.Module module, MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM _listBedRoom)
            : this(module)
        {
            try
            {
                this.listBedRoom = _listBedRoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmUpdatePatientClassify_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                InitCboPatientClasstify();
                ValidationClassify();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationClassify()
        {
            try
            {
                if (Config.IsPatientClassify)
                {
                    Valid_Grid_Control valid = new Valid_Grid_Control();
                    valid.cbo = cboPatientClassify;
                    dxValidationProvider1.SetValidationRule(cboPatientClassify, valid);
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCboPatientClasstify()
        {
            try
            {
                var lsPatientClassify = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>();

                if (this.listBedRoom != null && !string.IsNullOrEmpty(listBedRoom.PATIENT_TYPE_CODE))
                {
                    var patientype = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == listBedRoom.PATIENT_TYPE_CODE);
                    lsPatientClassify = lsPatientClassify.Where(o => o.PATIENT_TYPE_ID == patientype.ID || o.PATIENT_TYPE_ID == null).ToList();
                }
                else
                {
                    lsPatientClassify = lsPatientClassify.Where(o => o.PATIENT_TYPE_ID == null).ToList();
                }
                if (lsPatientClassify != null && lsPatientClassify.Count > 0)
                {
                    List<ColumnInfo> colum = new List<ColumnInfo>();
                    colum.Add(new ColumnInfo("PATIENT_CLASSIFY_CODE", "Mã", 100, 1));
                    colum.Add(new ColumnInfo("PATIENT_CLASSIFY_NAME", "Tên", 250, 2));
                    ControlEditorADO controlEditADO = new ControlEditorADO("PATIENT_CLASSIFY_NAME", "ID", colum, true, 350);
                    ControlEditorLoader.Load(cboPatientClassify, lsPatientClassify, controlEditADO);
                    cboPatientClassify.Properties.ImmediatePopup = true;
                    if (this.listBedRoom != null && listBedRoom.TDL_PATIENT_CLASSIFY_ID != null)
                    {
                        cboPatientClassify.EditValue = listBedRoom.TDL_PATIENT_CLASSIFY_ID;

                    }
                }


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
                if (!layoutControlItem1.Visible)
                    dxValidationProvider1.SetValidationRule(cboPatientClassify, null);
                positionHandleControlPatientInfo = -1;
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                HisPatientUpdateClassifySDO sdo = new HisPatientUpdateClassifySDO();
                sdo.PatientId = listBedRoom.PATIENT_ID;
                if (cboPatientClassify.EditValue != null)
                    sdo.PatientClassifyId = (long)cboPatientClassify.EditValue;

                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sdo______", sdo));
                var result = new BackendAdapter(param).Post<bool>("api/HisPatient/UpdateClassify", ApiConsumers.MosConsumer, sdo, param);
                if (result)
                {
                    success = true;
                    this.Close();
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                //ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientClassify_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientClassify.EditValue = null;
                }
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

                if (positionHandleControlPatientInfo == -1)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlPatientInfo > edit.TabIndex)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
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
    }
}
