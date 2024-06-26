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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using AutoMapper;
using HIS.Desktop.Plugins.CareTypeList;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.IdentityMaterialInformation.ADO;
using System.Text;
using MOS.SDO;

namespace HIS.Desktop.Plugins.IdentityMaterialInformation
{
    public partial class frmIdentityMaterialInformation : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        Inventec.Desktop.Common.Modules.Module moduleData;
        long ImpMestId;
        List<IMaterialADO> materials = new List<IMaterialADO>();
        List<IMaterialADO> listMaterialSerials = new List<IMaterialADO>();
        HIS.Desktop.Common.DelegateImpTime SendImpTime;
        bool ShowImpTime;

        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        HIS_IMP_MEST ImpMest;
        #endregion


        #region FormConstructor

        public frmIdentityMaterialInformation(Inventec.Desktop.Common.Modules.Module moduleData, long ImpMestId, bool ShowImpTime, HIS.Desktop.Common.DelegateImpTime SendImpTime)
        : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.SendImpTime = SendImpTime;
                this.moduleData = moduleData;
                this.ImpMestId = ImpMestId;
                try
                {
                    this.ShowImpTime = ShowImpTime;
                    layoutControlItem1.Visibility = ShowImpTime ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmIdentityMaterialInformation_Load(object sender, EventArgs e)
        {

            try
            {
                if (ImpMestId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisImpMestFilter ifilter = new HisImpMestFilter();
                    ifilter.ID = ImpMestId;
                    ImpMest = (new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, ifilter, param)).FirstOrDefault();

                    HisImpMestMaterialViewFilter filter = new HisImpMestMaterialViewFilter();
                    filter.IMP_MEST_ID = ImpMestId;
                    filter.IS_REUSABLE__OR__IS_IDENTITY_MANAGEMENT = true;
                    var data = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, filter, param);

                    if (data == null || data.Count == 0)
                        return;
                    List<HIS_MATERIAL> mate = new List<HIS_MATERIAL>();
                    var materialIdList = data.Select(p => p.MATERIAL_ID).ToList();
                    if (materialIdList != null && materialIdList.Count > 0)
                    {
                        MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                        materialFilter.IDs = materialIdList;
                        mate = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                    }

                    foreach (var item in data)
                    {
                        IMaterialADO ado = new IMaterialADO(item, mate);
                        materials.Add(ado);
                    }
                    gridControl1.DataSource = materials;
                }
                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.moduleData.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkAutoSerialNumber.Name)
                        {
                            chkAutoSerialNumber.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }
        private void gridView1_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "SERIAL_NUMBER" || e.ColumnName == "MATERIAL_SIZE")
                {
                    this.gridViewServiceProcess_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewServiceProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridView1.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControl1.DataSource as List<IMaterialADO>;
                var row = listDatas[index];
                if (e.ColumnName == "SERIAL_NUMBER")
                {
                    if (row.ErrorTypeSerialNumber == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeSerialNumber);
                        e.Info.ErrorText = (string)(row.ErrorMessageSerialNumber);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "MATERIAL_SIZE")
                {
                    if (row.ErrorTypeSize == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeSize);
                        e.Info.ErrorText = (string)(row.ErrorMessageSize);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckSerialNumber(string input, ref string SerialNumber, ref string SerialNumberGenSequence)
        {

            bool checkNumber(string year, int len)
            {
                bool rs = false;
                try
                {
                    int number;
                    rs = year.Length == len && int.TryParse(year, out number);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return rs;
            }

            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var splt = input.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (splt.Length == 3)
                    {
                        var ser = splt[0].Trim();
                        var year = splt[1].Trim();
                        SerialNumberGenSequence = splt[2].Trim();
                        if (!checkNumber(year, 2) || !checkNumber(SerialNumberGenSequence, 6))
                            return false;
                        SerialNumber = string.Format("{0}.{1}.{2}", ser, year, SerialNumberGenSequence);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var ado = (IMaterialADO)this.gridView1.GetFocusedRow();
                if (ado != null)
                {
                    if (e.Column.FieldName == "SERIAL_NUMBER")
                    {
                        string SerialNumberGenSequence = null;
                        string SerialNumber = null;
                        long num = 0;
                        if (ado.IsAllowEditSerialNumber.HasValue && chkAutoSerialNumber.Checked)
                            if (CheckSerialNumber(ado.SERIAL_NUMBER.Trim(), ref SerialNumber, ref SerialNumberGenSequence) && long.TryParse(SerialNumberGenSequence, out num))
                            {
                                ado.SERIAL_NUMBER_GEN_SEQUENCE = num;
                                ado.SERIAL_NUMBER = SerialNumber;
                            }
                            else
                            {
                                ado.ErrorMessageSerialNumber = "Nhập sai định dạng. Số seri cần nhập theo định dạng XXXX.YY.NNNNNN. Trong đó: XXXX: là mã vật tư; YY: 2 chữ số cuối năm tạo phiếu nhập; NNNNNN: Số thứ tự tăng dần";
                                ado.ErrorTypeSerialNumber = ErrorType.Warning;
                                XtraMessageBox.Show(ado.ErrorMessageSerialNumber);
                                return;
                            }
                        else
                        {
                            ado.ErrorMessageSerialNumber = "";
                            ado.ErrorTypeSerialNumber = ErrorType.None;
                        }
                        if (!String.IsNullOrEmpty(ado.SERIAL_NUMBER.Trim()) && Encoding.UTF8.GetByteCount(ado.SERIAL_NUMBER.Trim()) > 50)
                        {
                            ado.ErrorMessageSerialNumber = "Trường dữ liệu vượt quá ký tự cho phép (50 ký tự)";
                            ado.ErrorTypeSerialNumber = ErrorType.Warning;
                        }
                        else
                        {
                            ado.ErrorMessageSerialNumber = "";
                            ado.ErrorTypeSerialNumber = ErrorType.None;
                        }
                    }
                    else if (e.Column.FieldName == "MATERIAL_SIZE")
                    {

                        if (!String.IsNullOrEmpty(ado.MATERIAL_SIZE.Trim()) && Encoding.UTF8.GetByteCount(ado.MATERIAL_SIZE.Trim()) > 50)
                        {
                            ado.ErrorMessageSize = "Trường dữ liệu vượt quá ký tự cho phép (50 ký tự)";
                            ado.ErrorTypeSize = ErrorType.Warning;
                        }
                        else
                        {
                            ado.ErrorMessageSize = "";
                            ado.ErrorTypeSize = ErrorType.None;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (IMaterialADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                try
                {
                    if (e.RowHandle < 0)
                        return;
                    var ReuseCount = Int32.Parse((gridView1.GetRowCellValue(e.RowHandle, "IS_REUSABLE") ?? "0").ToString());
                    if (e.Column.FieldName == "REUSE_COUNT")
                    {
                        if (ReuseCount == 1)
                        {
                            e.RepositoryItem = repMaxReuseEnable;
                        }
                        else
                        {
                            e.RepositoryItem = repMaxReuseDisable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repMaxReuseEnable_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool SaveProcess()
        {
            try
            {
                if (materials == null || materials.Count == 0)
                    return false;
                if (materials.FirstOrDefault(o => !string.IsNullOrEmpty(o.ErrorMessageSerialNumber) || !string.IsNullOrEmpty(o.ErrorMessageSize)) != null)
                    return false;
                if (ShowImpTime && Int64.Parse(dte.DateTime.ToString("yyyyMMddHHmm")) >= Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmm")))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian nhập không được lớn hơn hoặc bằng thời gian hiện tại", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return false;
                }
                var mate = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(o => materials.Where(p => string.IsNullOrEmpty(p.MATERIAL_SIZE)).ToList().Exists(p => p.MATERIAL_TYPE_ID == o.ID)).ToList();
                if (mate != null && mate.Count > 0 && mate.FirstOrDefault(o => o.IS_SIZE_REQUIRED == 1) != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Vật tư {0} chưa nhập kích thước", string.Join(",", mate.Where(o => o.IS_SIZE_REQUIRED == 1).Select(o => o.MATERIAL_TYPE_CODE).Distinct())), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    gridView1.FocusedColumn = gridColumn15;
                    gridView1.FocusedRowHandle = materials.IndexOf(materials.Where(p => string.IsNullOrEmpty(p.MATERIAL_SIZE)).ToList().FirstOrDefault(o => mate.Exists(p => p.ID == o.MATERIAL_TYPE_ID)));
                    return false;
                }
                var checkSeri = materials.Where(o => string.IsNullOrEmpty(o.SERIAL_NUMBER));
                if (checkSeri.Count() > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Vật tư {0} chưa nhập số seri", string.Join(",", checkSeri.Select(o => o.MATERIAL_TYPE_CODE).Distinct())), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    gridView1.FocusedColumn = gridColumn13;
                    gridView1.FocusedRowHandle = materials.IndexOf(checkSeri.First());
                    return false;
                }
                var checkSeriDup = materials.GroupBy(o => new { o.MATERIAL_TYPE_CODE, o.SERIAL_NUMBER }).Select(o => new { name = o.Key, count = o.Count() });
                List<string> lstname = new List<string>();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkSeriDup), checkSeriDup));
                foreach (var item in checkSeriDup)
                {
                    if (item.count > 1)
                        lstname.Add(item.name.MATERIAL_TYPE_CODE);
                }
                if (lstname.Count() > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Vật tư {0} nhập trùng số seri", string.Join(",", lstname.Distinct())), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    gridView1.FocusedColumn = gridColumn13;
                    gridView1.FocusedRowHandle = materials.IndexOf(materials.FirstOrDefault(o => o.MATERIAL_TYPE_CODE == lstname.First()));
                    return false;
                }
                var checkReuse = materials.Where(o => o.IS_REUSABLE == 1 && (o.REUSE_COUNT == null || o.REUSE_COUNT == 0));
                if (checkReuse.Count() > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Vật tư {0} chưa nhập số lần tái sử dụng", string.Join(",", checkReuse.Select(o => o.MATERIAL_TYPE_CODE).Distinct())), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    gridView1.FocusedColumn = gridColumn13;
                    gridView1.FocusedRowHandle = materials.IndexOf(checkReuse.First());
                    return false;
                }
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                List<IdentityMaterialInformationSDO> lst = new List<IdentityMaterialInformationSDO>();
                foreach (var item in materials)
                {
                    IdentityMaterialInformationSDO sdo = new IdentityMaterialInformationSDO();
                    sdo.SerialNumber = item.SERIAL_NUMBER;
                    sdo.MaterialSize = item.MATERIAL_SIZE;
                    sdo.ImpMestMaterialId = item.ID;
                    sdo.SerialNumberGenSequence = item.SERIAL_NUMBER_GEN_SEQUENCE ?? 0;
                    sdo.SerialNumberSeedCode = item.SERIAL_NUMBER_GEN_SEED_CODE;
                    lst.Add(sdo);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lst), lst));
                var rs = new BackendAdapter(param).Post<bool>("api/HisImpMest/UpdateIdentityMaterialInformation", ApiConsumers.MosConsumer, lst, ProcessLostToken, param);
                WaitingManager.Hide();

                MessageManager.Show(this, param, rs);
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
                return rs;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = SaveProcess();
                if (success)
                {
                    this.Close();
                    SendImpTime((dte.EditValue != null && dte.DateTime != DateTime.MinValue) ? (long?)(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dte.DateTime) ?? 0) : null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.PerformClick();
        }

        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
        }

        private void chkAutoSerialNumber_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (!isNotLoadWhileChangeControlStateInFirst)
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoSerialNumber.Name && o.MODULE_LINK == this.moduleData.ModuleLink).FirstOrDefault() : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = chkAutoSerialNumber.Checked ? "1" : "0";
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = chkAutoSerialNumber.Name;
                        csAddOrUpdate.VALUE = chkAutoSerialNumber.Checked ? "1" : "0";
                        csAddOrUpdate.MODULE_LINK = this.moduleData.ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }
                if (!chkAutoSerialNumber.Checked)
                {
                    materials.ForEach(item =>
                    {
                        item.ErrorMessageSerialNumber = "";
                        item.ErrorTypeSerialNumber = ErrorType.None;
                    });
                    return;
                }
                else
                {
                    materials.ForEach(item =>
                    {
                        string SerialNumberGenSequence = null;
                        string SerialNumber = null;
                        if (!string.IsNullOrEmpty(item.SERIAL_NUMBER) &&!CheckSerialNumber(item.SERIAL_NUMBER.Trim(), ref SerialNumber, ref SerialNumberGenSequence))
                        {
                            item.IsAllowEditSerialNumber = null;
                            item.SERIAL_NUMBER_GEN_SEED_CODE = null;
                            item.SERIAL_NUMBER = null;
                            item.SERIAL_NUMBER_GEN_SEQUENCE = null;
                        }
                    });
                }
                List<string> SNSCs = new List<string>();
                foreach (var item in materials.Where(o => string.IsNullOrEmpty(o.SERIAL_NUMBER)))
                {
                    item.SerialNumberSeedCode = string.Format("{0}.{1}", item.MATERIAL_TYPE_CODE, ImpMest.CREATE_TIME != null ? ImpMest.CREATE_TIME.ToString().Substring(2, 2) : null);
                    SNSCs.Add(item.SerialNumberSeedCode);
                }
                SNSCs = SNSCs.Distinct().ToList();
                CommonParam param = new CommonParam();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SNSCs), SNSCs));
                if (SNSCs != null && SNSCs.Count > 0)
                {
                    var rs = new BackendAdapter(param).Get<List<SerialNumberGenSdo>>("api/HisImpMestMaterial/GetSerialNumberGenerateSequence", ApiConsumers.MosConsumer, SNSCs, param);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs == null || rs.Count == 0)
                        return;
                    materials.ForEach(item =>
                    {
                        if (rs.FirstOrDefault(o => o.SerialNumberSeedCode == item.SerialNumberSeedCode) != null)
                        {
                            item.SERIAL_NUMBER_GEN_SEQUENCE = null;
                            item.SERIAL_NUMBER = null;
                        }    
                    });
                    foreach (var item in materials)
                    {
                        if (rs.FirstOrDefault(o => o.SerialNumberSeedCode == item.SerialNumberSeedCode) != null)
                        {
                            if (materials.FirstOrDefault(o => o.SerialNumberSeedCode == item.SerialNumberSeedCode && o.SERIAL_NUMBER_GEN_SEQUENCE.HasValue) != null)
                            {
                                item.SERIAL_NUMBER_GEN_SEQUENCE = materials.Where(o => o.SerialNumberSeedCode == item.SerialNumberSeedCode && o.SERIAL_NUMBER_GEN_SEQUENCE.HasValue).ToList().Max(o => o.SERIAL_NUMBER_GEN_SEQUENCE) + 1;
                            }else
                                item.SERIAL_NUMBER_GEN_SEQUENCE = rs.FirstOrDefault(o => o.SerialNumberSeedCode == item.SerialNumberSeedCode).SerialNumberGenSequence + 1;
                            item.SERIAL_NUMBER_GEN_SEED_CODE = item.SerialNumberSeedCode;
                            item.SERIAL_NUMBER = string.Format("{0}.{1}", item.SERIAL_NUMBER_GEN_SEED_CODE, string.Format("{0:000000}", Convert.ToInt64(item.SERIAL_NUMBER_GEN_SEQUENCE ?? 0)));
                            item.IsAllowEditSerialNumber = false;
                            item.ErrorMessageSerialNumber = "";
                            item.ErrorTypeSerialNumber = ErrorType.None;
                        }
                    }
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = materials;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSaveAndPrintSeri_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = SaveProcess();
                if (success)
                {
                    if (materials != null && materials.Count > 0)
                    {
                        listMaterialSerials = materials.Where(o => !String.IsNullOrEmpty(o.SERIAL_NUMBER)).ToList();
                        if (listMaterialSerials != null && listMaterialSerials.Count > 0)
                        {
                            PrintProcess();
                            this.Close();
                            SendImpTime((dte.EditValue != null && dte.DateTime != DateTime.MinValue) ? (long?)(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dte.DateTime) ?? 0) : null);
                        }
                        else
                        {
                            XtraMessageBox.Show("Không có thông tin số Seri");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000494", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                InTemTheoSoSeri(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void InTemTheoSoSeri(string printTypeCode, string fileName, ref bool result)
        {

            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                List<MPS.Processor.Mps000494.PDO.SerialADO> listSerial = new List<MPS.Processor.Mps000494.PDO.SerialADO>();
                foreach (var item in listMaterialSerials)
                {
                    MPS.Processor.Mps000494.PDO.SerialADO serial = new MPS.Processor.Mps000494.PDO.SerialADO();
                    serial.SERIAL_NUMBER = item.SERIAL_NUMBER;
                    serial.NEXT_REUSABLE_NUMBER = null;
                    serial.SIZE = item.MATERIAL_SIZE;
                    listSerial.Add(serial);
                }

                MPS.Processor.Mps000494.PDO.Mps000494PDO mps000494PDO = new MPS.Processor.Mps000494.PDO.Mps000494PDO(listSerial);
                WaitingManager.Hide();

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData("Mps000494", fileName, mps000494PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData("Mps000494", fileName, mps000494PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
