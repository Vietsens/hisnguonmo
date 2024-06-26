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
using SDA.EFMODEL.DataModels;
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
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Process;
using EMR_MAIN;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord.FromConfig
{
    public partial class frmPhieu : Form
    {
        private List<SDA_HIDE_CONTROL> _HideControls;
        List<HIS_EMR_FORM> listEmrForm;
        private EmrInputADO _inputAdo;
        
        public frmPhieu(EmrInputADO ado)
        {
            InitializeComponent();
            this._inputAdo = ado;

            try
            {
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPhieu_Load(object sender, EventArgs e)
        {
            try
            {
                this._HideControls = HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.GetByModule("HIS.Desktop.Plugins.Library.FormMedicalRecord");
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void FillDataToControl()
        {
            try
            {
                listEmrForm = new List<HIS_EMR_FORM>();
                CommonParam param = new CommonParam();
                HisEmrFormFilter filter = new HisEmrFormFilter();
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "ID";

                gridView1.BeginUpdate();
                var apiResult = new BackendAdapter(param).Get<List<HIS_EMR_FORM>>("api/HisEmrForm/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                if (apiResult != null)
                {
                    listEmrForm = (List<HIS_EMR_FORM>)apiResult;
                    if (listEmrForm != null)
                    {
                        gridView1.GridControl.DataSource = listEmrForm;
                    }
                }
                gridView1.EndUpdate();
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_EMR_FORM data = (HIS_EMR_FORM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                }  
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (HIS_EMR_FORM)gridView1.GetFocusedRow();
                    if (rowData != null) 
                    {
                        MediRecordProcessor processor = new MediRecordProcessor();
                        if (this._inputAdo != null)
                        {
                            if (this._inputAdo.EmrCoverTypeId > 0)
                            {
                                processor.LoadDataEmr((LoaiBenhAnEMR)this._inputAdo.EmrCoverTypeId, this._inputAdo, rowData.EMR_FORM_CODE);
                            }
                            else
                            {
                                processor.LoadDataEmr(0, this._inputAdo, rowData.EMR_FORM_CODE);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_EMR_FORM)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    MediRecordProcessor processor = new MediRecordProcessor();
                    if (this._inputAdo != null)
                    {
                        if (this._inputAdo.EmrCoverTypeId > 0)
                        {
                            processor.LoadDataEmr((LoaiBenhAnEMR)this._inputAdo.EmrCoverTypeId, this._inputAdo, rowData.EMR_FORM_CODE);
                        }
                        else
                        {
                            processor.LoadDataEmr(0, this._inputAdo, rowData.EMR_FORM_CODE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditOpen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (HIS_EMR_FORM)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    MediRecordProcessor processor = new MediRecordProcessor();
                    if (this._inputAdo != null)
                    {
                        if (this._inputAdo.EmrCoverTypeId > 0)
                        {
                            processor.LoadDataEmr((LoaiBenhAnEMR)this._inputAdo.EmrCoverTypeId, this._inputAdo, rowData.EMR_FORM_CODE);
                        }
                        else
                        {
                            processor.LoadDataEmr(0, this._inputAdo, rowData.EMR_FORM_CODE);
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
