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
using HIS.Desktop.Plugins.HisIcdImport.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.HisIcdImport
{
    public partial class frmIcdImport : HIS.Desktop.Utility.FormBase
    {

        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<IcdADO> _IcdAdos;
        List<IcdADO> _CurrentAdos;
        List<HIS_ICD> _ListIcd { get; set; }

        public frmIcdImport()
        {
            InitializeComponent();
        }

        public frmIcdImport(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmIcdImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmIcdImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataIcd()
        {
            try
            {
                _ListIcd = new List<HIS_ICD>();
                MOS.Filter.HisIcdFilter filter = new MOS.Filter.HisIcdFilter();
                _ListIcd = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD>>("api/HisIcd/Get", ApiConsumers.MosConsumer, filter, null);
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ICD.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_ICD";
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
                        var hisServiceImport = import.GetWithCheck<IcdADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<IcdADO> listAfterRemove = new List<IcdADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.ICD_CODE)
                                    && string.IsNullOrEmpty(item.ICD_NAME);

                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();

                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._IcdAdos = new List<IcdADO>();
                                addServiceToProcessList(_CurrentAdos, ref this._IcdAdos);
                                SetDataSource(this._IcdAdos);
                            }


                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addServiceToProcessList(List<IcdADO> _listIcd, ref List<IcdADO> _IcdRef)
        {
            try
            {
                _IcdRef = new List<IcdADO>();
                long i = 0;
                foreach (var item in _listIcd)
                {
                    i++;
                    string error = "";
                    var serAdo = new IcdADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<IcdADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.ICD_CODE))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ICD_CODE, 10))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã");
                    }

                    if (!string.IsNullOrEmpty(item.ICD_NAME))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ICD_NAME, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên");
                    }
                    var checkTrung12 = _listIcd.Where(p => p.ICD_CODE == item.ICD_CODE).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.ICD_CODE);
                    }

                    if (!string.IsNullOrEmpty(item.ICD_NAME_COMMON))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ICD_NAME_COMMON, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên thường gọi");
                        }
                        if (item.ICD_NAME_COMMON != null)
                        {
                            serAdo.ICD_NAME_COMMON = item.ICD_NAME_COMMON;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Tên thường gọi");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ICD_NAME_EN))
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.ICD_NAME_EN, 500))
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên tiếng anh");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.KhongHopLe, "Tên tiếng anh");
                    }

                    if (!string.IsNullOrEmpty(item.ICD_GROUP_CODE))
                    {
                        var icdIsCaurse = BackendDataWorker.Get<HIS_ICD_GROUP>().FirstOrDefault(p => p.ICD_GROUP_CODE == item.ICD_GROUP_CODE);
                        if (icdIsCaurse != null)
                        {
                            serAdo.ICD_GROUP_ID = icdIsCaurse.ID;
                            serAdo.ICD_GROUP_NAME = icdIsCaurse.ICD_GROUP_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_CAUSE_STR))
                    {
                        if (item.IS_CAUSE_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã");
                        }
                        else
                        {
                            serAdo.IS_CAUSE = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_HEIN_NDS_STR))
                    {
                        if (item.IS_HEIN_NDS_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã");
                        }
                        else
                        {
                            serAdo.IS_HEIN_NDS = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_REQUIRE_CAUSE_STR))
                    {
                        if (item.IS_REQUIRE_CAUSE_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã");
                        }
                        else
                        {
                            serAdo.IS_REQUIRE_CAUSE = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_TRADITIONAL_STR))
                    {
                        if (item.IS_TRADITIONAL_STR.Trim().ToUpper() != "X")
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã");
                        }
                        else
                        {
                            serAdo.IS_TRADITIONAL = 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.GENDER_CODE_STR))
                    {
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.GENDER_CODE == item.GENDER_CODE_STR);
                        if (gender != null)
                        {
                            serAdo.GENDER_ID = gender.ID;
                            serAdo.GENDER_NAME_STR = gender.GENDER_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Giới tính");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.ATTACH_ICD_CODES_STR))
                    {
                        List<string> attachIcdCodes = new List<string>();
                        List<string> attachIcdErrors = new List<string>();
                        List<string> attachIcdCodeAvaiables = new List<string>();
                        List<string> attachIcdNameAvaiables = new List<string>();
                        if (item.ATTACH_ICD_CODES_STR.Contains(";"))
                        {
                            attachIcdCodes = item.ATTACH_ICD_CODES_STR.Split(';').ToList();
                        }
                        if (attachIcdCodes.Count() > 0)
                        {
                            var icds = BackendDataWorker.Get<HIS_ICD>();
                            foreach (var icdCode in attachIcdCodes)
                            {
                                var icd = icds.FirstOrDefault(o => o.ICD_CODE == icdCode);
                                if (icd != null)
                                {
                                    attachIcdCodeAvaiables.Add(icdCode);
                                    attachIcdNameAvaiables.Add(icd.ICD_NAME);
                                }
                                else
                                {
                                    if (_IcdRef == null || _IcdRef.Count() == 0)
                                    {
                                        attachIcdErrors.Add(icdCode);
                                        continue;
                                    }
                                    var icdFromImport = _IcdRef.FirstOrDefault(o => o.ICD_CODE == icdCode);
                                    if (icdFromImport != null)
                                    {
                                        attachIcdCodeAvaiables.Add(icdCode);
                                        attachIcdNameAvaiables.Add(icdFromImport.ICD_NAME);
                                    }
                                    else
                                    {
                                        attachIcdErrors.Add(icdCode);
                                    }
                                }
                            }
                        }
                        if (attachIcdCodeAvaiables.Count() > 0)
                        {
                            serAdo.ATTACH_ICD_CODES = string.Join(";", attachIcdCodeAvaiables);
                            serAdo.ATTACH_ICD_NAMES_STR = string.Join(";", attachIcdNameAvaiables);
                        }
                        if (attachIcdErrors.Count() > 0)
                        {
                            error += string.Format("ICD {0} không hợp lệ", string.Join(";", attachIcdErrors));
                        }
                    }

                    if (!string.IsNullOrEmpty(item.AGE_TYPE_CODE_STR))
                    {
                        var ageType = BackendDataWorker.Get<HIS_AGE_TYPE>().FirstOrDefault(p => p.AGE_TYPE_CODE == item.AGE_TYPE_CODE_STR);
                        if (ageType != null)
                        {
                            serAdo.AGE_TYPE_ID = ageType.ID;
                            serAdo.AGE_TYPE_NAME_STR = ageType.AGE_TYPE_NAME;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Loại tuổi");
                        }
                        if (string.IsNullOrEmpty(item.AGE_FROM_STR) && string.IsNullOrEmpty(item.AGE_TO_STR))
                        {
                            error += "Thiếu thông tin tuổi từ hoặc tuổi đến";
                        }
                    }
                    if (!string.IsNullOrEmpty(item.AGE_FROM_STR))
                    {
                        if (string.IsNullOrEmpty(item.AGE_TYPE_CODE_STR))
                        {
                            error += string.Format(Message.MessageImport.ThieuTruongDL, "Loại tuổi");
                        }
                        serAdo.AGE_FROM = Convert.ToInt64(item.AGE_FROM_STR);
                        serAdo.AGE_FROM_DISPLAY_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(item.AGE_FROM_STR));

                    }
                    if (!string.IsNullOrEmpty(item.AGE_TO_STR))
                    {
                        if (string.IsNullOrEmpty(item.AGE_TYPE_CODE_STR) && string.IsNullOrEmpty(item.AGE_FROM_STR))
                        {
                            error += string.Format(Message.MessageImport.ThieuTruongDL, "Loại tuổi");
                        }
                        serAdo.AGE_TO = Convert.ToInt64(item.AGE_TO_STR);
                        serAdo.AGE_TO_DISPLAY_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(item.AGE_TO_STR));
                    }

                    if (!string.IsNullOrEmpty(item.IS_SUBCODE_STR))
                    {
                        if (item.IS_SUBCODE_STR.Trim().ToUpper() != "X")
                        {
                            error += "Thông tin bệnh phụ không hợp lệ";
                        }
                        else
                        {
                            serAdo.IS_SUBCODE = 1;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_SWORD_STR))
                    {
                        if (item.IS_SWORD_STR.Trim().ToUpper() != "X")
                        {
                            error += "Thông tin mã kiếm không hợp lệ";
                        }
                        else
                        {
                            serAdo.IS_SWORD = 1;
                        }
                        if (string.IsNullOrEmpty(serAdo.ATTACH_ICD_CODES))
                        {
                            error += "Thiếu thông tin mã bệnh kèm theo";
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_COVID_STR))
                    {
                        if (item.IS_COVID_STR.Trim().ToUpper() != "X")
                        {
                            error += "Thông tin bệnh covid không hợp lệ";
                        }
                        else
                        {
                            serAdo.IS_COVID = 1;
                        }
                    }


                    //if (!string.IsNullOrEmpty(item.IS_CAUSE.ToString()))
                    //{
                    //    serAdo.IS_CAUSE = item.IS_CAUSE;
                    //    var icdIsCaurse = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(p => p.IS_CAUSE == item.IS_CAUSE);
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã");
                    //}

                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _IcdRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataSource(List<IcdADO> dataSource)
        {
            try
            {
                gridControl1.BeginUpdate();
                gridControl1.DataSource = null;
                gridControl1.DataSource = dataSource;
                gridControl1.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    IcdADO pData = (IcdADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IS_CAUSE_CBO")
                    {
                        e.Value = pData.IS_CAUSE == 1;
                    }
                    else if (e.Column.FieldName == "IS_HEIN_NDS_CBO")
                    {
                        e.Value = pData.IS_HEIN_NDS == 1;
                    }
                    else if (e.Column.FieldName == "IS_REQUIRE_CAUSE_CBO")
                    {
                        e.Value = pData.IS_REQUIRE_CAUSE == 1;
                    }
                    else if (e.Column.FieldName == "IS_TRADITIONAL_CBO")
                    {
                        e.Value = pData.IS_TRADITIONAL == 1;
                    }
                    else if (e.Column.FieldName == "IS_SUBCODE_DISPLAY_STR")
                    {
                        e.Value = pData.IS_SUBCODE == 1;
                    }
                    else if (e.Column.FieldName == "IS_SWORD_DISPLAY_STR")
                    {
                        e.Value = pData.IS_SWORD == 1;
                    }
                    else if (e.Column.FieldName == "IS_COVID_DISPLAY_STR")
                    {
                        e.Value = pData.IS_COVID == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckErrorLine(List<IcdADO> dataSource)
        {
            try
            {
                var checkError = this._IcdAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnImport.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._IcdAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._IcdAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (IcdADO)gridView1.GetFocusedRow();
                if (row != null)
                {
                    if (this._IcdAdos != null && this._IcdAdos.Count > 0)
                    {
                        this._IcdAdos.Remove(row);
                        var dataCheck = this._IcdAdos.Where(p => p.ICD_CODE == row.ICD_CODE).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].ICD_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._IcdAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<HIS_ICD> datas = new List<HIS_ICD>();

                if (this._IcdAdos != null && this._IcdAdos.Count > 0)
                {
                    foreach (var item in this._IcdAdos)
                    {
                        HIS_ICD ado = new HIS_ICD();
                        ado.ICD_NAME = item.ICD_NAME;
                        ado.ICD_CODE = item.ICD_CODE;
                        ado.ICD_NAME_COMMON = item.ICD_NAME_COMMON;
                        ado.ICD_NAME_EN = item.ICD_NAME_EN;
                        ado.ICD_GROUP_ID = item.ICD_GROUP_ID;
                        ado.IS_HEIN_NDS = item.IS_HEIN_NDS;
                        ado.IS_REQUIRE_CAUSE = item.IS_REQUIRE_CAUSE;
                        ado.IS_CAUSE = item.IS_CAUSE;
                        ado.IS_TRADITIONAL = item.IS_TRADITIONAL;
                        //
                        ado.GENDER_ID = item.GENDER_ID;
                        ado.ATTACH_ICD_CODES = item.ATTACH_ICD_CODES;
                        ado.AGE_FROM = item.AGE_FROM;
                        ado.AGE_TO = item.AGE_TO;
                        ado.AGE_TYPE_ID = item.AGE_TYPE_ID;
                        ado.IS_SUBCODE = item.IS_SUBCODE;
                        ado.IS_SWORD = item.IS_SWORD;
                        ado.IS_COVID = item.IS_COVID;
                        datas.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<HIS_ICD>>("api/HisIcd/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataIcd();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridView1.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR_")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButton_ER;
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
                var row = (IcdADO)gridView1.GetFocusedRow();
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

    }
}
