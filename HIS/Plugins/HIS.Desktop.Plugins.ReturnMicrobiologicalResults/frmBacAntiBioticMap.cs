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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Integrate.EditorLoader;
using LIS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    public partial class frmBacAntiBioticMap : FormBase
    {
        List<LisResultADO> testLisResultADOs;
        Action<List<LisResultADO>> actTestLisResultADOs;
        TestLisResultADO rowTstLisResultADO;
        public List<LIS_BACTERIUM> listBacteriumSelecteds = new List<LIS_BACTERIUM>();
        List<LIS_ANTIBIOTIC> listAntibiotics = new List<LIS_ANTIBIOTIC>();
        List<LIS_BACTERIUM_FAMILY> listBacteriumFamilies = new List<LIS_BACTERIUM_FAMILY>();
        List<LIS_BACTERIUM> listBacteriums = new List<LIS_BACTERIUM>();
        public frmBacAntiBioticMap(TestLisResultADO _rowTstLisResultADO, Action<List<LisResultADO>> _actTestLisResultADOs)
            : base()
        {
            InitializeComponent();
            this.rowTstLisResultADO = _rowTstLisResultADO;
            this.testLisResultADOs = _rowTstLisResultADO.LResultDetails;
            this.actTestLisResultADOs = _actTestLisResultADOs;
            SetIconFrm();
        }

        void SetIconFrm()
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

        private void frmBacAntiBioticMap_Load(object sender, EventArgs e)
        {
            try
            {
                listAntibiotics = BackendDataWorker.Get<LIS_ANTIBIOTIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                listBacteriumFamilies = BackendDataWorker.Get<LIS_BACTERIUM_FAMILY>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                listBacteriums = BackendDataWorker.Get<LIS_BACTERIUM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                LoadDataComboBacFamily(listBacteriumFamilies);
                InitComboBacterium(listBacteriums);

                if (this.testLisResultADOs != null && this.testLisResultADOs.Count > 0)
                {
                    gridView1.BeginUpdate();
                    gridView1.GridControl.DataSource = this.testLisResultADOs;
                    gridView1.EndUpdate();
                    gridView1.SelectAll();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => testLisResultADOs), testLisResultADOs));

                    var bacFamily = listBacteriumFamilies.Where(o => o.BACTERIUM_FAMILY_CODE == this.testLisResultADOs.First().BACTERIUM_FAMILY_CODE).FirstOrDefault();
                    cboBacFamily.EditValue = bacFamily != null ? (long?)bacFamily.ID : null;

                    var bac = listBacteriums.Where(o => o.BACTERIUM_CODE == this.testLisResultADOs.First().BACTERIUM_CODE).FirstOrDefault();
                    cboBacterium.EditValue = bac != null ? (long?)bac.ID : null;
                }
            }
            catch (Exception ex)
            {
                gridView1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataComboBacFamily(List<LIS_BACTERIUM_FAMILY> data)
        {
            try
            {
                List<ColumnInfo> columnInfosBacFamily = new List<ColumnInfo>();
                columnInfosBacFamily.Add(new ColumnInfo("BACTERIUM_FAMILY_CODE", "Mã", 50, 1));
                columnInfosBacFamily.Add(new ColumnInfo("BACTERIUM_FAMILY_NAME", "Tên", 100, 2));
                ControlEditorADO controlEditorADOBacFamily = new ControlEditorADO("BACTERIUM_FAMILY_NAME", "ID", columnInfosBacFamily, false, 150);
                ControlEditorLoader.Load(cboBacFamily, data, controlEditorADOBacFamily);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void InitComboBacterium(List<LIS_BACTERIUM> data)
        {
            try
            {
                cboBacterium.Properties.View.Columns.Clear();
                InitCheck(cboBacterium, SelectionGrid__cboBacterium);
                cboBacterium.Properties.DataSource = data;
                cboBacterium.Properties.DisplayMember = "BACTERIUM_NAME";
                cboBacterium.Properties.ValueMember = "ID";

                DevExpress.XtraGrid.Columns.GridColumn col1 = cboBacterium.Properties.View.Columns.AddField("BACTERIUM_CODE");
                col1.VisibleIndex = 1;
                col1.Width = 100;
                col1.Caption = "ALL";

                DevExpress.XtraGrid.Columns.GridColumn col2 = cboBacterium.Properties.View.Columns.AddField("BACTERIUM_NAME");
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = "Tất cả";

                cboBacterium.Properties.PopupFormWidth = 300;
                cboBacterium.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboBacterium.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cboBacterium.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboBacterium.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
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

        private void SelectionGrid__cboBacterium(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                this.testLisResultADOs = new List<LisResultADO>();
                if (gridCheckMark != null)
                {
                    List<LIS_BACTERIUM> sgSelectedNews = new List<LIS_BACTERIUM>();
                    foreach (LIS_BACTERIUM bac in (gridCheckMark).Selection)
                    {
                        if (bac != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(bac.BACTERIUM_NAME.ToString());
                            sgSelectedNews.Add(bac);
                            LIS_BACTERIUM_FAMILY bacFamily = new LIS_BACTERIUM_FAMILY();
                            if (bac.BACTERIUM_FAMILY_ID != null)
                                bacFamily = listBacteriumFamilies.Where(o => o.ID == bac.BACTERIUM_FAMILY_ID).FirstOrDefault();
                            var bacAntiBioticDatas = BackendDataWorker.Get<LIS_BAC_ANTIBIOTIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE && o.BACTERIUM_ID == bac.ID).ToList();
                            if (bacAntiBioticDatas != null && bacAntiBioticDatas.Count > 0)
                            {
                                foreach (var item in bacAntiBioticDatas)
                                {
                                    LisResultADO testLisResultAdd = new LisResultADO();
                                    testLisResultAdd.BACTERIUM_CODE = bac.BACTERIUM_CODE;
                                    testLisResultAdd.BACTERIUM_NAME = bac.BACTERIUM_NAME;

                                    testLisResultAdd.BACTERIUM_FAMILY_CODE = bacFamily != null ? bacFamily.BACTERIUM_FAMILY_CODE : "";
                                    testLisResultAdd.BACTERIUM_FAMILY_NAME = bacFamily != null ? bacFamily.BACTERIUM_FAMILY_NAME : "";

                                    var anti = listAntibiotics.Where(o => o.ID == item.ANTIBIOTIC_ID).OrderBy(o => o.NUM_ORDER).FirstOrDefault();
                                    if (anti == null)
                                        continue;
                                    testLisResultAdd.ANTIBIOTIC_CODE = anti.ANTIBIOTIC_CODE;
                                    testLisResultAdd.ANTIBIOTIC_NAME = anti.ANTIBIOTIC_NAME;

                                    testLisResultAdd.ANTIBIOTIC_ORDER = anti.NUM_ORDER ?? 999999;

                                    testLisResultAdd.SAMPLE_SERVICE_ID = this.rowTstLisResultADO.SAMPLE_SERVICE_ID;
                                    testLisResultAdd.SERVICE_CODE = this.rowTstLisResultADO.SERVICE_CODE;
                                    testLisResultAdd.SERVICE_NAME = this.rowTstLisResultADO.SERVICE_NAME;
                                    testLisResultAdd.SAMPLE_ID = this.rowTstLisResultADO.SAMPLE_ID;
                                    testLisResultAdd.MIC = item.MIC != null ? item.MIC.ToString() : "";

                                    this.testLisResultADOs.Add(testLisResultAdd);
                                }
                            }
                        }
                    }
                    this.listBacteriumSelecteds = new List<LIS_BACTERIUM>();
                    this.listBacteriumSelecteds.AddRange(sgSelectedNews);
                }
                gridView1.BeginUpdate();
                if (this.testLisResultADOs != null && this.testLisResultADOs.Count > 0)
                {
                    gridView1.GridControl.DataSource = this.testLisResultADOs.GroupBy(o => o.ANTIBIOTIC_CODE).Select(s => s.First()).OrderBy(o => o.ANTIBIOTIC_ORDER).ThenBy(t => t.ANTIBIOTIC_NAME).ToList();
                }
                else
                    gridView1.GridControl.DataSource = null;

                gridView1.EndUpdate();
                gridView1.SelectAll();
                this.cboBacterium.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                gridView1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LisResultADO data = (LisResultADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBacFamily_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBacFamily.EditValue != null)
                {
                    var bacData = listBacteriums.Where(o => o.BACTERIUM_FAMILY_ID == (long)cboBacFamily.EditValue).ToList();
                    InitComboBacterium(bacData);
                    gridControl1.DataSource = null;
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
                var dataFilter = this.testLisResultADOs.Where(o =>
                    o.ANTIBIOTIC_CODE.ToLower().Contains(txtKeyword.Text.ToLower())
                    || o.ANTIBIOTIC_NAME.ToLower().Contains(txtKeyword.Text.ToLower())
                    ).ToList();

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = dataFilter;
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                gridView1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                List<LisResultADO> testLisResultSelecteds = new List<LisResultADO>();
                int[] rowHandles = gridView1.GetSelectedRows();
                bool valid = (rowHandles != null && rowHandles.Length > 0);

                if (valid)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (LisResultADO)gridView1.GetRow(i);
                        if (row != null)
                        {
                            var testLisResultAdos = this.testLisResultADOs.Where(o => o.ANTIBIOTIC_CODE == row.ANTIBIOTIC_CODE).ToList();
                            testLisResultSelecteds.AddRange(testLisResultAdos);
                        }
                    }
                }
                if (listBacteriumSelecteds == null || listBacteriumSelecteds.Count == 0)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show("Không có dữ liệu kháng sinh đồ");
                    return;
                }
                if (testLisResultSelecteds == null || testLisResultSelecteds.Count == 0)
                {
                    if (XtraMessageBox.Show("Không có dữ liệu kháng sinh đồ. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    foreach (var item in listBacteriumSelecteds)
                    {
                        LisResultADO resultADO = new LisResultADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LisResultADO>(resultADO, item);
                        testLisResultSelecteds.Add(resultADO);
                    }
                }

                if (this.actTestLisResultADOs != null)
                {
                    this.actTestLisResultADOs(testLisResultSelecteds);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBacterium_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (LIS_BACTERIUM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.BACTERIUM_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBacterium_Click(object sender, EventArgs e)
        {
            try
            {
                cboBacterium.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
