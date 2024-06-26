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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ConfigPrinter.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConfigPrinter.Run
{
    public partial class frmConfigPrinters : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        private const string HIS_CONFIG__PRINT_TYPE__PRINTER = "His.Config.PrintType.Printer";
        Dictionary<string, string> dicPrinter = new Dictionary<string, string>();
        List<SAR_PRINT_TYPE> printTypes = null;
        List<PrintTypeADO> listPrintTypeADO = new List<PrintTypeADO>();
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        public frmConfigPrinters()
        {
            InitializeComponent();
        }

        public frmConfigPrinters(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmConfigPrinters_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                LoadDataToComboPrinterName();
                LoadDataToGridControl();
                LoadConfigPrinter();
                InitPrintType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboPrinterName()
        {
            try
            {
                repositoryItemCboBoxPrinter.Items.AddRange(Inventec.Common.Print.FlexCelPrintStore.GetSystemPrintNames());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                listPrintTypeADO = new List<PrintTypeADO>();
                printTypes = new List<SAR_PRINT_TYPE>();
                SAR.Filter.SarPrintTypeFilter filter = new SAR.Filter.SarPrintTypeFilter();
                printTypes = new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>("/api/SarPrintType/Get", ApiConsumers.SarConsumer, filter, param);
                LoadDataToComboPrintType(repositoryItemGridLookUp__PrintType, printTypes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitPrintType()
        {
            try
            {
                this.listPrintTypeADO = new List<PrintTypeADO>();
                if (dicPrinter != null && dicPrinter.Count > 0)
                {
                    foreach (var item in dicPrinter)
                    {
                        PrintTypeADO PrintTypeADO = new PrintTypeADO();
                        PrintTypeADO.Action = GlobalVariables.ActionView;
                        PrintTypeADO.PRINTER_NAME = item.Value;
                        PrintTypeADO.PRINT_TYPE_CODE = item.Key;
                        PrintTypeADO.PRINT_TYPE_ID = this.printTypes.FirstOrDefault(p => p.PRINT_TYPE_CODE == item.Key).ID;
                        this.listPrintTypeADO.Add(PrintTypeADO);
                    }
                }
                else
                {
                    PrintTypeADO PrintTypeADO = new PrintTypeADO();
                    PrintTypeADO.Action = GlobalVariables.ActionAdd;
                    this.listPrintTypeADO.Add(PrintTypeADO);
                }

                gridControlPrintType.DataSource = null;
                gridControlPrintType.DataSource = this.listPrintTypeADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadConfigPrinter()
        {
            try
            {
                dicPrinter = new Dictionary<string, string>();
                dicPrinter = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter;

                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicPrinter), dicPrinter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                PrintTypeADO PrintTypeADO = new PrintTypeADO();
                PrintTypeADO.Action = GlobalVariables.ActionEdit;
                this.listPrintTypeADO.Add(PrintTypeADO);
                gridControlPrintType.DataSource = null;
                gridControlPrintType.DataSource = this.listPrintTypeADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (PrintTypeADO)gridViewPrintType.GetFocusedRow();
                if (row != null)
                {
                    this.listPrintTypeADO.Remove(row);
                    gridControlPrintType.DataSource = null;
                    gridControlPrintType.DataSource = this.listPrintTypeADO;
                }
                if (this.listPrintTypeADO == null || this.listPrintTypeADO.Count == 0)
                {
                    this.listPrintTypeADO = new List<PrintTypeADO>();
                    PrintTypeADO PrintTypeADO = new PrintTypeADO();
                    PrintTypeADO.Action = GlobalVariables.ActionAdd;
                    this.listPrintTypeADO.Add(PrintTypeADO);
                    gridControlPrintType.DataSource = null;
                    gridControlPrintType.DataSource = this.listPrintTypeADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal static void LoadDataToComboPrintType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cboPrintTypeName, List<SAR_PRINT_TYPE> data)
        {
            try
            {
                cboPrintTypeName.DataSource = data;
                cboPrintTypeName.DisplayMember = "PRINT_TYPE_NAME";
                cboPrintTypeName.ValueMember = "ID";
                cboPrintTypeName.NullText = "";

                cboPrintTypeName.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboPrintTypeName.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboPrintTypeName.ImmediatePopup = true;
                cboPrintTypeName.View.Columns.Clear();

                GridColumn aColumnCode = cboPrintTypeName.View.Columns.AddField("PRINT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = false;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 200;

                GridColumn aColumnName = cboPrintTypeName.View.Columns.AddField("PRINT_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 400;

                cboPrintTypeName.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrintType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                PrintTypeADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (PrintTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle > 0)
                {
                    if (e.Column.FieldName == "Add")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemButton__Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUp__PrintType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    GridLookUpEdit gridLookUp = sender as GridLookUpEdit;
                    if (gridLookUp != null && gridLookUp.EditValue != null)
                    {
                        var data = printTypes.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUp.EditValue.ToString()));
                        if (data != null)
                        {
                            gridViewPrintType.SetFocusedRowCellValue("PRINT_TYPE_CODE", data.PRINT_TYPE_CODE);
                        }
                    }
                    this.repositoryItemCboBoxPrinter.AllowFocused = true;
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
                if (!btnSave.Enabled)
                    return;
                var listData = listPrintTypeADO.Where(o => !String.IsNullOrEmpty(o.PRINT_TYPE_CODE) && !String.IsNullOrEmpty(o.PRINTER_NAME)).ToList();
                if (listData == null || listData.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thiếu trường dữ liệu bắt buộc", "Thông báo");
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.ConfigPrinter");
                bool success = false;
                string value = SetValueConfig(listData);


                if (!String.IsNullOrEmpty(value))
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == HIS_CONFIG__PRINT_TYPE__PRINTER && o.MODULE_LINK == "HIS.Desktop.Plugins.ConfigPrinter").FirstOrDefault() : null;

                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = HIS_CONFIG__PRINT_TYPE__PRINTER;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.ConfigPrinter";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);

                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings[HIS_CONFIG__PRINT_TYPE__PRINTER].Value = "";
                    config.AppSettings.Settings[HIS_CONFIG__PRINT_TYPE__PRINTER].Value = value;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                    success = true;
                }
                WaitingManager.Hide();
                if (success)
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.LoadConfigPrintType();
                    MessageManager.Show(this, param, success);
                    //this.Close();
                }
                else
                {
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string SetValueConfig(List<PrintTypeADO> listData)
        {
            string result = "";
            try
            {
                List<string> listConfig = new List<string>();
                dicPrinter = new Dictionary<string, string>();
                foreach (var item in listData)
                {
                    dicPrinter[item.PRINT_TYPE_CODE] = item.PRINTER_NAME;
                }
                if (dicPrinter.Count > 0)
                {
                    foreach (var dic in dicPrinter)
                    {
                        listConfig.Add(dic.Key + ":" + dic.Value);
                    }
                }
                if (listConfig.Count > 0)
                {
                    result = String.Join(";", listConfig);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewPrintType.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTextEdit__PrintTypeCode_Enter(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
