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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
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
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.ChooseDepartment
{
    public delegate void DelegateLoadData(List<HIS_SERE_SERV> _sereServs, HIS_SERE_SERV _servServKTC, HIS_DEPARTMENT departmentResult);
    public delegate void DelegateLoadData2(List<HIS_SERE_SERV> _sereServs, HIS_SERE_SERV _servServKTC, HIS_DEPARTMENT departmentResult, List<HIS_SERE_SERV_EXT> _sereServExts);
    public partial class frmBordereauChooseDepartment : Form
    {
        internal List<HIS_SERE_SERV> sereServKTCs { get; set; }
        internal List<HIS_SERE_SERV> sereServs { get; set; }
        internal HIS_SERE_SERV sereServKTC { get; set; }
        internal List<HIS_DEPARTMENT> departments { get; set; }
        internal long currentDepartmentId { get; set; }
        internal DelegateLoadData DelegateLoadData { get; set; }
        internal DelegateLoadData2 DelegateLoadData2 { get; set; }
        internal List<HIS_SERE_SERV_EXT> SereServExts { get; set; }
        internal List<SereServADO> SereServADOs { get; set; }
        int positionHandleControlLeft = -1;

        public frmBordereauChooseDepartment()
        {
            InitializeComponent();
        }

        public frmBordereauChooseDepartment(List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV> _sereServKTCs, long _departmentId, DelegateLoadData _delegateLoadData)
        {
            InitializeComponent();
            try
            {
                this.sereServKTCs = _sereServKTCs;
                this.currentDepartmentId = _departmentId;
                this.DelegateLoadData = _delegateLoadData;
                this.sereServs = _sereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmBordereauChooseDepartment(List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV> _sereServKTCs, long _departmentId, DelegateLoadData2 _delegateLoadData2)
        {
            InitializeComponent();
            try
            {
                this.sereServKTCs = _sereServKTCs;
                this.currentDepartmentId = _departmentId;
                this.DelegateLoadData2 = _delegateLoadData2;
                this.sereServs = _sereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmBordereauChooseDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.sereServs != null)
                {
                    foreach (var item in this.sereServs)
                    {
                        if (item.TDL_HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH
                            || item.TDL_HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT
                            || item.TDL_HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT)
                        {
                            item.TDL_REQUEST_DEPARTMENT_ID = item.TDL_EXECUTE_DEPARTMENT_ID;
                        }
                    }
                }
                LoadSereServExt();
                Valid__Department();
                LoadDepartmentCbo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServExt()
        {
            try
            {
                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.SereServExts = new List<HIS_SERE_SERV_EXT>();
                    int skip = 0;
                    var sereServIds = sereServs.Select(o => o.ID).ToList();
                    CommonParam param = new CommonParam();
                    while (sereServIds.Count - skip > 0)
                    {
                        var listIds = sereServIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                        sereServExtFilter.SERE_SERV_IDs = listIds;
                        var dt = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);
                        if (dt != null && dt.Count > 0)
                        {
                            SereServExts.AddRange(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void  LoadDepartmentCbo()
        {
            try
            {
                List<long> departmentIds = new List<long>();
                if (sereServs != null && sereServs.Count > 0)
                {
                    departmentIds.AddRange(sereServs.Select(o => o.TDL_REQUEST_DEPARTMENT_ID).ToList());
                }
                if (sereServKTCs != null && sereServKTCs.Count > 0)
                {
                    departmentIds.AddRange(sereServKTCs.Select(o => o.TDL_EXECUTE_DEPARTMENT_ID).ToList());
                }
                departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => departmentIds.Distinct().Contains(o.ID)).ToList();

                if (currentDepartmentId > 0)
                {
                    var currentDepartment = departments.FirstOrDefault(o => o.ID == currentDepartmentId);
                    if (currentDepartment != null)
                    {
                        cboDepartment.EditValue = currentDepartment.ID;
                        txtDepartmentCode.Text = currentDepartment.DEPARTMENT_CODE;
                        if (sereServKTCs != null)
                        {
                            var sereServKTCDepartments = sereServKTCs.Where(o => o.TDL_EXECUTE_DEPARTMENT_ID == currentDepartmentId).ToList();
                            LoadServiceCbo(sereServKTCDepartments);
                            txtServiceCode.Focus();
                        }
                        cboDepartment.Properties.Buttons[1].Visible = true;
                    }
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboDepartment, departments, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceCbo(List<HIS_SERE_SERV> sereServs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                foreach (var item in sereServs)
                {
                    AutoMapper.Mapper.CreateMap<HIS_SERE_SERV, SereServADO>();
                    SereServADO sereServADO = AutoMapper.Mapper.Map<HIS_SERE_SERV, SereServADO>(item);

                    var SereServExt = this.SereServExts.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                    if (SereServExt != null)
                    {
                        sereServADO.BEGIN_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(SereServExt.BEGIN_TIME ?? 0);
                    }
                    SereServADOs.Add(sereServADO);
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TDL_SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("TDL_SERVICE_NAME", "", 300, 2));
                columnInfos.Add(new ColumnInfo("BEGIN_TIME", "", 150, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_SERVICE_NAME", "ID", columnInfos, false, 600);
                ControlEditorLoader.Load(cboService, SereServADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                HIS_SERE_SERV sereServKTC = null;
                List<HIS_SERE_SERV> sereServResults = null;
                long assignServiceMergerTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFrom.DateTime) ?? 0;
                long assignServiceMergerTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTo.DateTime) ?? 0;
                if (cboService.EditValue != null)
                {
                    sereServKTC = sereServKTCs.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboService.EditValue.ToString()));

                    if (dtFrom.EditValue != null && dtTo.EditValue != null)
                    {

                        sereServResults = sereServs.Where(o =>
                            o.TDL_REQUEST_DEPARTMENT_ID == currentDepartmentId
                            && o.PARENT_ID == sereServKTC.ID
                            && TimeNumberToDateNumber(assignServiceMergerTimeFrom) <= TimeNumberToDateNumber(o.TDL_INTRUCTION_TIME)
                            && TimeNumberToDateNumber(o.TDL_INTRUCTION_TIME) <= TimeNumberToDateNumber(assignServiceMergerTimeTo))
                            .ToList();
                    }
                    else
                    {
                        sereServResults = sereServs.Where(o =>
                                o.TDL_REQUEST_DEPARTMENT_ID == currentDepartmentId
                                && o.PARENT_ID == sereServKTC.ID).ToList();
                    }

                }
                else if (dtFrom.EditValue != null && dtTo.EditValue != null)
                {
                    sereServResults = sereServs.Where(o =>
                                o.TDL_REQUEST_DEPARTMENT_ID == currentDepartmentId
                                && TimeNumberToDateNumber(assignServiceMergerTimeFrom) <= TimeNumberToDateNumber(o.TDL_INTRUCTION_TIME)
                                && TimeNumberToDateNumber(o.TDL_INTRUCTION_TIME) <= TimeNumberToDateNumber(assignServiceMergerTimeTo))
                                .ToList();
                }
                else
                {
                    sereServResults = sereServs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == currentDepartmentId).ToList();
                }

                if (DelegateLoadData != null)
                {
                    HIS_DEPARTMENT departmentResult = departments.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString()));
                    DelegateLoadData(sereServResults, sereServKTC, departmentResult);
                    this.Close();
                }

                if (DelegateLoadData2 != null)
                {
                    HIS_DEPARTMENT departmentResult = departments.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString()));
                    DelegateLoadData2(sereServResults, sereServKTC, departmentResult, this.SereServExts);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long TimeNumberToDateNumber(long? timeNumber)
        {
            long result = 0;
            try
            {
                string dateNumberString = timeNumber.ToString().Substring(0, 8);
                result = Inventec.Common.TypeConvert.Parse.ToInt64(dateNumberString);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboDepartment.EditValue != null)
                {
                    HIS_DEPARTMENT data = departments.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                        cboDepartment.Properties.Buttons[1].Visible = true;
                        currentDepartmentId = data.ID;
                        if (sereServKTCs != null)
                        {
                            var sereServKTCDepartments = sereServKTCs.Where(o => o.TDL_EXECUTE_DEPARTMENT_ID == currentDepartmentId).ToList();
                            LoadServiceCbo(sereServKTCDepartments);
                            txtServiceCode.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboService.EditValue != null)
                    {
                        HIS_SERE_SERV data = sereServKTCs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboService.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtServiceCode.Text = data.TDL_SERVICE_CODE;
                            cboService.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.Properties.Buttons[1].Visible = false;
                    cboDepartment.EditValue = null;
                    txtDepartmentCode.Text = "";


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboService.Properties.Buttons[1].Visible = false;
                    cboService.EditValue = null;
                    txtServiceCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadDepartmentCombo(strValue, false, cboDepartment, txtDepartmentCode, departments);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadDepartmentCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboDepartment, DevExpress.XtraEditors.TextEdit txtDepartment, List<HIS_DEPARTMENT> departments)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Focus();
                    cboDepartment.ShowPopup();
                }
                else
                {
                    var data = departments.Where(o => o.DEPARTMENT_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboDepartment.EditValue = data[0].ID;
                            txtDepartment.Text = data[0].DEPARTMENT_CODE;
                            currentDepartmentId = data[0].ID;
                        }
                        else if (data.Count > 1)
                        {
                            cboDepartment.EditValue = null;
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDepartment.EditValue = null;
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadServiceCombo(strValue, false, cboService, txtServiceCode, sereServKTCs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadServiceCombo(string searchCode, bool isExpand, DevExpress.XtraEditors.GridLookUpEdit cboService, DevExpress.XtraEditors.TextEdit txtServiceCode, List<HIS_SERE_SERV> sereServKTCs)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboService.EditValue = null;
                    cboService.Focus();
                    cboService.ShowPopup();
                }
                else
                {
                    var data = sereServKTCs.Where(o => o.TDL_SERVICE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboService.EditValue = data[0].ID;
                            txtServiceCode.Text = data[0].TDL_SERVICE_CODE;
                        }
                        else if (data.Count > 1)
                        {
                            cboService.EditValue = null;
                            cboService.Focus();
                            cboService.ShowPopup();
                        }
                    }
                    else
                    {
                        cboService.EditValue = null;
                        cboService.Focus();
                        cboService.ShowPopup();
                    }
                }
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

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
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
