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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using Inventec.Desktop.Common.Modules;
using Inventec.Core;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Fss.Client;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System.IO;
using DevExpress.XtraEditors;
using Inventec.Common.SignLibrary;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.LocalStorage.LocalData;
using EMR.SDO;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;

namespace EMR.Desktop.Plugins.SignedDocument
{
    public partial class frmSignedDocument : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Module currentModule;
        string hisCode = null;
        string treatmentCode = null;
        EmrDocumentInfoADO emrAdo;
        #endregion

        public frmSignedDocument(Module _Module, EmrDocumentInfoADO _EmrAdo)
            : base(_Module)
        {
            InitializeComponent();
            this.emrAdo = _EmrAdo;
            this.currentModule = _Module;
            this.hisCode = _EmrAdo.HisCode;
            this.treatmentCode = _EmrAdo.TreatmentCode;
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSignedDocument_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultLanguage();
                FillDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultLanguage()
        {
            try
            {
                //HIS.Desktop.Plugins.SignedDocument.Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.SignedDocument.Resources.Lang", typeof(frmSignedDocument).Assembly);
                //this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_APPROVESERVICEREQCLS__RIGHT_ROIVT_LANGUAGE_KEY__FRM_NAME", HIS.Desktop.Plugins.SignedDocument.Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataGrid()
        {
            try
            {
                List<EmrDocumentFileSDO> documents = new List<EmrDocumentFileSDO>();
                CommonParam param = new CommonParam();

                EmrDocumentViewFilter emrFilter = new EmrDocumentViewFilter();
                emrFilter.HAS_SIGNERS = true;
                emrFilter.IS_REJECTER_NOT_NULL = false;
                emrFilter.IS_DELETE = false;
                emrFilter.TREATMENT_CODE__EXACT = treatmentCode;

                EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
                sdo.EmrDocumentViewFilter = emrFilter;
                sdo.HisCode = hisCode;
                sdo.IsMerge = false;
                sdo.IsShowPatientSign = null;
                sdo.IsShowWatermark = null;
                sdo.RoomCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).ROOM_CODE;
                sdo.DepartmentCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).DEPARTMENT_CODE;
                sdo.IsView = true;
                Inventec.Common.Logging.LogSystem.Debug("____________"
                         + "Input___ EmrDocumentDownloadFileSDO:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                documents = new BackendAdapter(param).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, param);
                if ((documents == null || documents.Count == 0) && param.Messages != null && param.Messages.Count > 0)
                {
                    MessageManager.Show(param, null);
                }
                
                gridView1.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug("____________"
                         + "Output___ EmrDocumentFileSDO:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => documents), documents));

                if (documents != null)
                {
                    gridView1.GridControl.DataSource = documents;
                }
                else
                {
                    gridView1.GridControl.DataSource = null;
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
                
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (EmrDocumentFileSDO)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ChangedDataRow(EmrDocumentFileSDO data)
        {
            if (data != null)
            {
                LoadPdfViewer(data);
            }
        }
        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
        }
        private void FillDataToEditorControl(EmrDocumentFileSDO data)
        {
            try
            {
                List<EmrDocumentFileSDO> listEmr = new List<EmrDocumentFileSDO>();
                listEmr.Add(data);
                this.ucViewEmrDocument1.ShowBar = true;
                this.ucViewEmrDocument1.ReloadDocument(listEmr);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (EmrDocumentFileSDO)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);

                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadPdfViewer(EmrDocumentFileSDO sdo)
        {
            try
            {
                this.ucViewEmrDocument1.ReloadDocument(new List<EmrDocumentFileSDO>() { sdo });
            }
            catch (Exception ex)
            {
                this.ucViewEmrDocument1.Controls.Clear();
                this.ucViewEmrDocument1 = new HIS.UC.ViewEmrDocument.UcEmrDocument.UcViewEmrDocument();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
