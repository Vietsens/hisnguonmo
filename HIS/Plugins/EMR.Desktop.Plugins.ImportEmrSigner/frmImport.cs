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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.Desktop.Plugins.ImportEmrSigner.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using EMR.EFMODEL.DataModels;
using EMR.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Controls.Session;

namespace EMR.Desktop.Plugins.ImportEmrSigner
{
    public partial class frmImport : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<ADO.EmrSignerAdo> _ImportAdos;
        List<ADO.EmrSignerAdo> _CurrentAdos;
        List<EMR_SIGNER> _ListSigner { get; set; }

        public frmImport()
        {
            InitializeComponent();
        }

        public frmImport(Inventec.Desktop.Common.Modules.Module _module)
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

        public frmImport(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
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

        private void frmImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                    btnShowLineError.Enabled = false;
                    btnImport.Enabled = false;
                }
                SetIcon();
                LoadCurrentData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentData()
        {
            try
            {
                _ListSigner = new List<EMR_SIGNER>();
                EMR.Filter.EmrSignerFilter filter = new Filter.EmrSignerFilter();
                _ListSigner = new BackendAdapter(new CommonParam()).Get<List<EMR_SIGNER>>("api/EmrSigner/Get", ApiConsumers.EmrConsumer, filter, null);

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

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_EMR_SIGNER.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_EMR_SIGNER";
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
                        var emrSignerImport = import.GetWithCheck<ADO.EmrSignerAdo>(0);
                        if (emrSignerImport != null && emrSignerImport.Count > 0)
                        {
                            //List<ImportADO> listAfterRemove = new List<ImportADO>();
                            //foreach (var item in hisServiceImport)
                            //{
                            //    bool checkNull = string.IsNullOrEmpty(item.LOGINNAME)
                            //        && string.IsNullOrEmpty(item.USERNAME)
                            //        ;

                            //    if (!checkNull)
                            //    {
                            //        listAfterRemove.Add(item);
                            //    }
                            //}
                            WaitingManager.Hide();

                            this._CurrentAdos = emrSignerImport.Where(p => checkNull(p)).ToList();

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._ImportAdos = new List<ADO.EmrSignerAdo>();
                                addServiceToProcessList(_CurrentAdos, ref this._ImportAdos);
                                SetDataSource(this._ImportAdos);
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

        bool checkNull(ADO.EmrSignerAdo data)
        {
            bool result = true;
            try
            {
                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.LOGINNAME)
                        && string.IsNullOrEmpty(data.USERNAME) 
                        && string.IsNullOrEmpty(data.DEPARTMENT_CODE)
                        && string.IsNullOrEmpty(data.DEPARTMENT_NAME)
                        && string.IsNullOrEmpty(data.TITLE)
                        )
                    {
                        result = false;
                    }
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void addServiceToProcessList(List<ADO.EmrSignerAdo> _service, ref List<ADO.EmrSignerAdo> _importAdoRef)
        {
            try
            {
                _importAdoRef = new List<ADO.EmrSignerAdo>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new ADO.EmrSignerAdo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.EmrSignerAdo>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.LOGINNAME))
                    {
                        if (item.LOGINNAME.Length > 50)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.LOGINNAME);
                        }
                        else
                        {
                            var dataOld = this._ListSigner.FirstOrDefault(p => p.LOGINNAME.ToLower() == item.LOGINNAME.ToLower());
                            if (dataOld != null)
                            {
                                error += string.Format(Message.ResourceLanguageManager.DaTonTai, item.LOGINNAME);
                            }
                            
                                var checkTrung = _service.Where(p => p.LOGINNAME == item.LOGINNAME).ToList();
                                if (checkTrung != null && checkTrung.Count > 1)
                                {
                                    error += string.Format(Message.ResourceLanguageManager.FileImportDaTonTai, item.LOGINNAME.ToLower());
                                }
                            

                        }
                        var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToLower() == item.LOGINNAME.ToLower());
                        if (user != null)
                        {
                            serAdo.LOGINNAME = user.LOGINNAME;
                            if (!string.IsNullOrEmpty(user.USERNAME))
                            {
                                serAdo.USERNAME = user.USERNAME;
                            }
                            else 
                            {
                                error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Họ tên");
                            }

                        }
                        else
                        {
                            error += string.Format(Message.ResourceLanguageManager.Khongtontai, item.LOGINNAME);

                        }
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Tên đăng nhập");
                    }

                    //if (!string.IsNullOrEmpty(item.USERNAME))
                    //{
                    //    if (item.USERNAME.Length > 100)
                    //    {
                    //        error += string.Format(Message.ResourceLanguageManager.Maxlength, item.USERNAME);

                    //    }
                    //}
                    //else
                    //{
                    //    error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Họ tên");
                    //}



                    if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                    {
                        if (item.DEPARTMENT_CODE.Length > 10)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.DEPARTMENT_CODE);

                        }
                        
                        else
                        {
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE.ToLower() == item.DEPARTMENT_CODE.ToLower());
                            if (department != null)
                            {
                                serAdo.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                serAdo.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            else
                            {
                                error += string.Format(Message.ResourceLanguageManager.KhongHopLe, item.DEPARTMENT_CODE);
                            }
                        }
                      
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Mã đơn vị");

                    }

                    if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                    {
                        if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                        {
                            serAdo.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                            if (serAdo.NUM_ORDER > 99999999999999 || serAdo.NUM_ORDER < 0)
                            {
                                error += string.Format(Message.ResourceLanguageManager.KhongHopLe, item.NUM_ORDER);
                            }
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(item.PCA_SERIAL))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.PCA_SERIAL) > 50)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.PCA_SERIAL);
                        }
 
                    }
                    if (!string.IsNullOrWhiteSpace(item.CMND_NUMBER))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.CMND_NUMBER) > 12)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.CMND_NUMBER);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(item.TITLE))
                    {
                        if (Inventec.Common.String.CountVi.Count(item.TITLE) > 100)
                        {
                            error += string.Format(Message.ResourceLanguageManager.Maxlength, item.TITLE);
                        }
                    }
                    else
                    {
                        error += string.Format(Message.ResourceLanguageManager.ThieuTruongDL, "Chức danh ký");
                    }
                    if (!String.IsNullOrWhiteSpace(item.SIGN_IMAGE_STR))
                    {
                        if (File.Exists(item.SIGN_IMAGE_STR))
                        {
                            try
                            {
                                serAdo.IMAGE_SIGN = Image.FromFile(item.SIGN_IMAGE_STR);
                                //if (serAdo.IMAGE_SIGN.Width < 140 || serAdo.IMAGE_SIGN.Width > 160 || serAdo.IMAGE_SIGN.Height != 40)
                                if (serAdo.IMAGE_SIGN.Width > 600 || serAdo.IMAGE_SIGN.Height > 200)
                                {
                                    error += string.Format(Message.ResourceLanguageManager.KhongHopLe + Message.ResourceLanguageManager.GioiHanAnh, "IMAGE_SIGN");
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                                error += string.Format(Message.ResourceLanguageManager.KhongHopLe, "Ảnh");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.ResourceLanguageManager.KhongHopLe, "Ảnh");
                        }
                    }
                    serAdo.ERROR = error;
                    serAdo.ID = i;

                    _importAdoRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<ADO.EmrSignerAdo> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<ADO.EmrSignerAdo> dataSource)
        {
            try
            {
                var checkError = this._ImportAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
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
                    var errorLine = this._ImportAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._ImportAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
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
                var row = (ADO.EmrSignerAdo)gridViewData.GetFocusedRow();
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
                var row = (ADO.EmrSignerAdo)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._ImportAdos != null && this._ImportAdos.Count > 0)
                    {
                        this._ImportAdos.Remove(row);
                        var dataCheck = this._ImportAdos.Where(p => p.LOGINNAME == row.LOGINNAME).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.ResourceLanguageManager.FileImportDaTonTai, dataCheck[0].LOGINNAME);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._ImportAdos);
                    }
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
                    ADO.EmrSignerAdo pData = (ADO.EmrSignerAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
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
                if (!btnImport.Enabled) return;
                btnImport.Focus();
                var listData = (List<ADO.EmrSignerAdo>)gridControlData.DataSource;

                if (listData == null || listData.Count <= 0) return;
                if (listData.Exists(o => !String.IsNullOrWhiteSpace(o.ERROR))) return;

                bool success = false;
                WaitingManager.Show();

                List<EMR.SDO.EmrSignerSDO> listSigner = new List<EMR.SDO.EmrSignerSDO>();

                foreach (var item in listData)
                {
                    EMR_SIGNER signer = new EMR_SIGNER();
                    signer.ID = 0;
                    signer.DEPARTMENT_CODE = item.DEPARTMENT_CODE;
                    signer.DEPARTMENT_NAME = item.DEPARTMENT_NAME;
                    signer.USERNAME = item.USERNAME;
                    signer.LOGINNAME = item.LOGINNAME;
                    signer.NUM_ORDER = item.NUM_ORDER;
                    signer.PCA_SERIAL = item.PCA_SERIAL;
                    signer.CMND_NUMBER = item.CMND_NUMBER;
                    signer.SIGN_IMAGE = item.SIGN_IMAGE;
                    signer.TITLE = item.TITLE;
                    signer.USERNAME = item.USERNAME;



                    EMR.SDO.EmrSignerSDO sdo = new SDO.EmrSignerSDO();
                    sdo.EmrSigner = signer;
                    if (item.IMAGE_SIGN != null)
                    {
                        sdo.ImgBase64Data = Convert.ToBase64String(ImageToByteArray(item.IMAGE_SIGN));
                    }
                    else
                    {
                        sdo.EmrSigner.SIGN_IMAGE = null;
                        sdo.ImgBase64Data = null;
                    }

                    listSigner.Add(sdo);
                }
                CommonParam param = new CommonParam();
                if (listSigner != null && listSigner.Count > 0)
                {
                    var rs = new BackendAdapter(param).Post<List<SDO.EmrSignerSDO>>(EMR.URI.EmrSigner.CREATE_LIST, ApiConsumers.EmrConsumer, listSigner, SessionManager.ActionLostToken, param);
                    if (rs != null)
                    {
                        success = true;
                        btnImport.Enabled = false;
                    }
                }

                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                //MessageManager.Show("Không thể import cùng lúc nhiều dữ liệu với 1 tên đăng nhập. Vui lòng chỉ chọn 1 ");
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedRoomADO data = (BedRoomADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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
        private byte[] ImageToByteArray(Image image)
        {
            byte[] result = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
