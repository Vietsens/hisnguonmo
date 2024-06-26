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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisMedicalContractImport.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicalContractImport
{
    public partial class frmHisMedicalContractImport : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference _DelegateRefresh;

        List<HisMedicalContractADO> _MedicalContractAdos;
        List<HisMedicalContractADO> _CurrentAdos;
        private Dictionary<string, HisMedicalContractADO> dicMedicalContract = new Dictionary<string, HisMedicalContractADO>();
        int checkButtonErrorLine = 0;
        bool isFirstTime = true;
        CommonParam paramDB = new CommonParam();
        HisMedicalContractFilter filterDB = new HisMedicalContractFilter();
        List<V_HIS_MEDICAL_CONTRACT> rsDB = new List<V_HIS_MEDICAL_CONTRACT>();

        List<HIS_BID> ListBid = new List<HIS_BID>();
        #endregion

        #region Construct
        public frmHisMedicalContractImport(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this._Module = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmHisMedicalContractImport(Inventec.Desktop.Common.Modules.Module module, RefeshReference delegateRefresh)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this._Module = module;
                this._DelegateRefresh = delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Gan gia tri mac dinh
                SetDefaultValue();

                //Focus default
                SetDefaultFocus();

                ProcessLoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLoadData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBidFilter bidFilter = new HisBidFilter();
                bidFilter.IS_ACTIVE = 1;
                this.ListBid = new BackendAdapter(param).Get<List<HIS_BID>>("api/HisBid/Get", ApiConsumers.MosConsumer, bidFilter, param);

                HisBidMedicineTypeFilter bidMedicinefilter = new HisBidMedicineTypeFilter();
                bidMedicinefilter.IS_ACTIVE = 1;
                this.storeBidMedicineType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumers.MosConsumer, bidMedicinefilter, param);

                HisBidMaterialTypeFilter bidMaterialfilter = new HisBidMaterialTypeFilter();
                bidMaterialfilter.IS_ACTIVE = 1;
                this.storeBidMaterialType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumers.MosConsumer, bidMaterialfilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisMedicalContractImport_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
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

        /// <summary>
        /// Gan Values mac dinh
        /// </summary>
        private void SetDefaultValue()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDataSource(List<HisMedicalContractADO> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.RefreshDataSource();
                gridControlData.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void CheckErrorLine()
        {
            try
            {
                var checkError = this._MedicalContractAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;
                    SetDataSource(this._MedicalContractAdos);
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        #endregion

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_HIS_MEDICAL_CONTRACT.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_HIS_MEDICAL_CONTRACT";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnShowLineError.Text = "Dòng lỗi";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var medicalContractImport = import.GetWithCheck<HisMedicalContractADO>(0);
                        if (medicalContractImport != null && medicalContractImport.Count > 0)
                        {
                            List<HisMedicalContractADO> listAfterRemove = new List<HisMedicalContractADO>();


                            foreach (var item in medicalContractImport)
                            {
                                bool checkNull =
                                string.IsNullOrEmpty(item.MEDICINE_TYPE_CODEorMATERIAL_TYPE_CODE) && string.IsNullOrEmpty(item.MEDICAL_CONTRACT_CODE);
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }


                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._MedicalContractAdos = new List<HisMedicalContractADO>();
                                rsDB = new BackendAdapter(paramDB).Get<List<V_HIS_MEDICAL_CONTRACT>>(HisRequestUriStore.MOS_HIS_MEDICAL_CONTRACT_GETVIEW, ApiConsumers.MosConsumer, filterDB, paramDB).ToList();
                                addServiceToProcessList(_CurrentAdos, ref this._MedicalContractAdos);
                                isFirstTime = false;
                                SetDataSource(this._MedicalContractAdos);
                                CheckErrorLine();
                            }
                            WaitingManager.Hide();
                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại!");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file!");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._MedicalContractAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 1;

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._MedicalContractAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 2;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisMedicalContractADO pData = (HisMedicalContractADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IMP_VAT_STR")
                    {
                        e.Value = pData.IMP_VAT_RATIO.HasValue ? pData.IMP_VAT_RATIO.Value * 100 : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR_")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButton_ER;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton_Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisMedicalContractADO)gridViewData.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisMedicalContractADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._MedicalContractAdos != null && this._MedicalContractAdos.Count > 0)
                    {
                        this._MedicalContractAdos.Remove(row);
                        List<HisMedicalContractADO> rs = this._MedicalContractAdos;
                        addServiceToProcessList(rs, ref this._MedicalContractAdos);
                        switch (checkButtonErrorLine)
                        {
                            case 0:
                                {
                                    SetDataSource(this._MedicalContractAdos);
                                    CheckErrorLine();
                                    break;
                                }
                            case 1:
                                {
                                    SetDataSource(this._MedicalContractAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine();
                                    break;
                                }
                            case 2:
                                {
                                    SetDataSource(this._MedicalContractAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine();
                                    break;
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

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                    btnSave_Click(null, null);
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
                ProcessSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
