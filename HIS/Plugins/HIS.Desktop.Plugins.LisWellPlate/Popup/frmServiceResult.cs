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
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.LisWellPlate.ADO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using LIS.SDO;
using HIS.Desktop.ApiConsumer;
using LIS.EFMODEL.DataModels;
namespace HIS.Desktop.Plugins.LisWellPlate.Popup
{
    public partial class frmServiceResult : FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long? serviceResultId;
        bool IsEnableControl;
        long? sampleId = null;
        List<V_LIS_RESULT> lstResult = null;
        List<TestIndexADO> lstAdo = null;
        List<LIS_SERVICE_RESULT> lst = null;
        Action<long?> DelegateSuccess;
        V_LIS_WELL_PLATE wellPlate;
        public frmServiceResult(Inventec.Desktop.Common.Modules.Module module, long? serviceResultId, V_LIS_WELL_PLATE wellPlate, long? sampleId, Action<long?> DelegateCheckSuccess)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.serviceResultId = serviceResultId;
                this.wellPlate = wellPlate;
                this.sampleId = sampleId;
                this.DelegateSuccess = DelegateCheckSuccess;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmServiceResult_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
                InitCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                lst = BackendDataWorker.Get<LIS_SERVICE_RESULT>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_RESULT_CODE", "Mã", 70, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_RESULT_NAME", "Tên", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_RESULT_NAME", "ID", columnInfos, true, 220);
                ControlEditorLoader.Load(cboOption, lst, controlEditorADO);
                cboOption.Properties.ImmediatePopup = true;
                if (wellPlate.WELL_PLATE_STATUS == 2)
                {
                    cboOption.Enabled = true;
                    gridView1.OptionsBehavior.Editable = false;
                }
                if (wellPlate.WELL_PLATE_STATUS == 3)
                {
                    cboOption.Properties.ReadOnly = true;
                    gridView1.OptionsBehavior.Editable = false;
                    btnSave.Enabled = false;
                    layoutControlItem2.Enabled = false;
                }

                if (serviceResultId != null && serviceResultId > 0)
                {
                    cboOption.EditValue = serviceResultId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                lstResult = new List<V_LIS_RESULT>();
                lstAdo = new List<TestIndexADO>();
                if (sampleId != null && sampleId > 0)
                {
                    CommonParam param = new CommonParam();
                    LIS.Filter.LisResultViewFilter resultFilter = new LisResultViewFilter();
                    resultFilter.SAMPLE_ID = sampleId;
                    lstResult = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);

                    var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();


                    foreach (var item in lstResult)
                    {
                        TestIndexADO ado = new TestIndexADO(item);
                        ado.TEST_INDEX_UNIT_NAME = testIndex.Where(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE).First().TEST_INDEX_UNIT_NAME;
                        lstAdo.Add(ado);
                    }

                    gridControl1.DataSource = null;
                    gridControl1.DataSource = lstAdo;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEditTextEdit_Ena_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                TestIndexADO testLisResultADO = new TestIndexADO();
                var data = gridControl1.DataSource as List<TestIndexADO>;
                if (data != null && data is TestIndexADO)
                {
                    TextEdit txt = sender as TextEdit;
                    if (txt.Text != null && txt.Text != "")
                    {
                        testLisResultADO.VALUE = txt.Text;
                    }
                    else
                    {
                        testLisResultADO.VALUE = null;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    if (gridView1.FocusedRowHandle == gridView1.DataRowCount - 1)
                    {
                        btnSave_Click(null, null);
                    }
                    else
                    {
                        gridView1.FocusedRowHandle = gridView1.FocusedRowHandle + 1;
                    }
                }
                if (e.KeyCode == Keys.Down)
                {
                    if (gridView1.FocusedRowHandle != gridView1.DataRowCount - 1)
                    {
                        if (gridView1.FocusedColumn.VisibleIndex == 2)
                        {
                            gridView1.FocusedRowHandle = gridView1.FocusedRowHandle + 1;
                            gridView1.FocusedColumn.VisibleIndex = 2;
                        }
                    }
                }
                if (e.KeyCode == Keys.Up)
                {
                    if (gridView1.FocusedRowHandle != 0)
                    {
                        if (gridView1.FocusedColumn.VisibleIndex == 2)
                        {
                            gridView1.FocusedRowHandle = gridView1.FocusedRowHandle - 1;
                            gridView1.FocusedColumn.VisibleIndex = 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOption_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateResult()
        {
            try
            {

                LisSampleResultSDO lisSdo = new LisSampleResultSDO();
                var data = gridControl1.DataSource as List<TestIndexADO>;
                lisSdo.SampleId = sampleId ?? 0;
                lisSdo.ResultValues = new List<UpdateResultValueSDO>();
                foreach (var item in data)
                {
                    UpdateResultValueSDO sdo = new UpdateResultValueSDO();
                    sdo.ResultId = item.ID;
                    //if (cboOption.EditValue != null)
                    //{
                    //    var check = lst.Where(o => o.ID == Int64.Parse(cboOption.EditValue.ToString())).First();
                    //    if (check != null && check.ID == IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__DUONG_TINH)
                    //    {
                    sdo.Value = item.VALUE;
                    //    }
                    //}
                    lisSdo.ResultValues.Add(sdo);
                }

                lisSdo.ServiceResultId = Int64.Parse(cboOption.EditValue.ToString());
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSdo, param);
                if (result)
                {
                    DelegateSuccess(Int64.Parse(cboOption.EditValue.ToString()));
                    this.Close();
                    WaitingManager.Hide();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, result);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateResult();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOption_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboOption.EditValue != null)
                {
                    var check = lst.Where(o => o.ID == Int64.Parse(cboOption.EditValue.ToString())).First();
                    if (check != null && check.ID == IMSys.DbConfig.LIS_RS.LIS_SERVICE_RESULT.ID__DUONG_TINH)
                    {
                        gridView1.OptionsBehavior.Editable = true;
                        if (lstAdo != null && lstAdo.Count > 0)
                        {
                            gridControl1.Focus();
                            gridView1.Focus();
                            gridView1.FocusedColumn = gridColumn3;
                            gridView1.FocusedRowHandle = 0;
                        }
                        if (wellPlate.WELL_PLATE_STATUS == 3)
                        {
                            cboOption.Properties.ReadOnly = true;
                            gridView1.OptionsBehavior.Editable = false;
                            btnSave.Enabled = false;
                            layoutControlItem2.Enabled = false;
                        }
                    }
                    else
                    {
                        gridView1.OptionsBehavior.Editable = false;
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
