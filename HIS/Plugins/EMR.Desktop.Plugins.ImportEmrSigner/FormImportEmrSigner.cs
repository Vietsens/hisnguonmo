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
using DevExpress.XtraGrid.Views.Base;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
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

namespace EMR.Desktop.Plugins.ImportEmrSigner
{
    public partial class FormImportEmrSigner : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private bool checkClick;
        private List<ADO.EmrSignerAdo> ListDataImport;
        List<EMR_SIGNER> _ListEmrSigner { get; set; }

        public FormImportEmrSigner()
        {
            InitializeComponent();
        }

        public FormImportEmrSigner(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormImportEmrSigner_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //LoadDataDeafult();

                BtnSave.Enabled = false;
                BtnLineError.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDeafult()
        {
            throw new NotImplementedException();
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BtnDownloadTemplate.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_DOWNLOAD_TEMPLATE");
                this.BtnImport.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_IMPORT");
                this.BtnLineError.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__ERROR");
                this.BtnSave.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_SAVE");
                this.Gc_Delete.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_DELETE");
                this.Gc_DepartmentCode.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_DEPARTMENT_CODE");
                this.Gc_DepartmentName.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_DEPARTMENT_NAME");
                this.Gc_LineError.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_LINE_ERROR");
                this.Gc_Loginname.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_LOGINNAME");
                this.Gc_NumOrder.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_NUM_ORDER");
                this.Gc_PcaSerial.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_PCA_SERIAL");
                this.Gc_CmdNumber.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_CMD_NUMBER");
                this.Gc_Stt.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__STT");
                this.Gc_Title.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_TITLE");
                this.Gc_Username.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_USERNAME");
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__RP_BTN_DELETE");
                this.repositoryItemBtnLineError.Buttons[0].ToolTip = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__RP_BTN_LINE_ERROR");
                this.Gc_ImageSign.Caption = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_SIGN_IMAGE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void barButtonISave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath + "/Tmp/Imp", "IMPORT_EMR_SIGNER.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_EMR_SIGNER";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var ImpListProcessor = import.Get<ADO.EmrSignerAdo>(0);
                        if (ImpListProcessor != null && ImpListProcessor.Count > 0)
                        {
                            this.ListDataImport = new List<ADO.EmrSignerAdo>();
                            AddListBloodTypeToProcessList(ImpListProcessor);

                            if (this.ListDataImport != null && this.ListDataImport.Count > 0)
                            {
                                SetDataSource(ListDataImport);

                                checkClick = false;
                                //BtnSave.Enabled = true;
                                BtnLineError.Enabled = true;
                            }
                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddListBloodTypeToProcessList(List<ADO.EmrSignerAdo> ImpListProcessor)
        {
            try
            {
                if (ImpListProcessor != null && ImpListProcessor.Count > 0)
                {
                    long i = 0;
                    foreach (var item in ImpListProcessor)
                    {
                        i++;
                        List<string> errors = new List<string>();
                        var ado = new ADO.EmrSignerAdo();
                        ado.IdRow = i;

                        ado.DEPARTMENT_CODE = item.DEPARTMENT_CODE;
                        if (!string.IsNullOrEmpty(item.DEPARTMENT_CODE))
                        {
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE.ToLower() == item.DEPARTMENT_CODE.ToLower());
                            if (department != null)
                            {
                                ado.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_DEPARTMENT_CODE")));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_DEPARTMENT_CODE")));
                        }

                        ado.LOGINNAME = item.LOGINNAME;
                        if (!string.IsNullOrEmpty(item.LOGINNAME))
                        {
                            var check = BackendDataWorker.Get<EMR_SIGNER>().FirstOrDefault(o => o.LOGINNAME.ToLower() == item.LOGINNAME.ToLower());
                            if (check != null)
                                errors.Add(string.Format(Resources.ResourceLanguageManager.DaTonTai, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_LOGINNAME")));

                            var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToLower() == item.LOGINNAME.ToLower());
                            if (user != null)
                            {
                                ado.LOGINNAME = user.LOGINNAME;
                                ado.USERNAME = user.USERNAME;
                            }

                            var checkTrung = _ListEmrSigner.Where(p => p.LOGINNAME == item.LOGINNAME).ToList();
                                if(checkTrung != null && checkTrung.Count >1)

                                    errors.Add(string.Format(Resources.ResourceLanguageManager.TonTaiTrungNhauTrongFileImport, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_LOGINNAME")));
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_LOGINNAME")));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_LOGINNAME")));
                        }

                        ado.NUM_ORDER_STR = item.NUM_ORDER_STR;
                        if (!string.IsNullOrEmpty(item.NUM_ORDER_STR))
                        {
                            if (Inventec.Common.Number.Check.IsNumber(item.NUM_ORDER_STR))
                            {
                                ado.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.NUM_ORDER_STR);
                                if (ado.NUM_ORDER > 99999999999999 || ado.NUM_ORDER < 0)
                                {
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_NUM_ORDER")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_NUM_ORDER")));
                            }
                        }

                        ado.PCA_SERIAL = item.PCA_SERIAL;
                        if (!string.IsNullOrWhiteSpace(item.PCA_SERIAL))
                        {
                            if (Inventec.Common.String.CountVi.Count(item.PCA_SERIAL) > 50)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_PCA_SERIAL"), 50));
                            }
                        }
                        ado.CMND_NUMBER = item.CMND_NUMBER;
                        if (!string.IsNullOrWhiteSpace(item.CMND_NUMBER))
                        {
                            if (Inventec.Common.String.CountVi.Count(item.CMND_NUMBER) > 12)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_CMND_NUMBER"), 12));
                            }
                        }

                        ado.TITLE = item.TITLE;
                        if (!string.IsNullOrWhiteSpace(item.TITLE))
                        {
                            if (Inventec.Common.String.CountVi.Count(item.TITLE) > 100)
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.Maxlength, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_TITLE"), 100));
                            }
                        }
                        else
                        {
                            errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_TITLE")));
                        }

                        if (!String.IsNullOrWhiteSpace(item.SIGN_IMAGE_STR))
                        {
                            if (File.Exists(item.SIGN_IMAGE_STR))
                            {
                                try
                                {
                                    ado.IMAGE_SIGN = Image.FromFile(item.SIGN_IMAGE_STR);
                                    if (ado.IMAGE_SIGN.Width < 140 || ado.IMAGE_SIGN.Width > 160 || ado.IMAGE_SIGN.Height != 40)
                                    {
                                        errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe + Resources.ResourceLanguageManager.GioiHanAnh, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_SIGN_IMAGE")));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                    errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_SIGN_IMAGE")));
                                }
                            }
                            else
                            {
                                errors.Add(string.Format(Resources.ResourceLanguageManager.KhongHopLe, GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__GC_SIGN_IMAGE")));
                            }
                        }

                        ado.ERROR = string.Join(";", errors);
                        ListDataImport.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSource(List<ADO.EmrSignerAdo> ListDataImport)
        {
            try
            {
                gridControl.BeginUpdate();
                gridControl.DataSource = null;
                gridControl.DataSource = ListDataImport;
                gridControl.EndUpdate();
                CheckErrorLine(ListDataImport);
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
                if (dataSource != null && dataSource.Count > 0)
                {
                    var checkError = dataSource.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                    if (!checkError)
                    {
                        BtnSave.Enabled = true;
                    }
                    else
                    {
                        BtnSave.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnLineError.Enabled) return;

                checkClick = true;
                if (BtnLineError.Text == GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__ERROR"))
                {
                    BtnLineError.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__OK");
                    var data = ListDataImport.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
                else
                {
                    BtnLineError.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__ERROR");
                    var data = ListDataImport.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSave.Enabled) return;
                BtnSave.Focus();
                var listData = (List<ADO.EmrSignerAdo>)gridControl.DataSource;

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
                        BtnSave.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
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

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.EmrSignerAdo)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (ListDataImport != null && ListDataImport.Count > 0)
                    {
                        ListDataImport.Remove(row);

                        SetDataSource(ListDataImport);

                        if (checkClick)
                        {
                            if (BtnLineError.Text == GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__ERROR"))
                            {
                                BtnLineError.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__OK");
                            }
                            else
                            {
                                BtnLineError.Text = GetLanguageControl("EMR_DESKTOP_PLUGINS_IMPORT_EMR_SIGNER__BTN_LINE_ERROR__ERROR");
                            }
                            BtnLineError_Click(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnLineError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.EmrSignerAdo)gridView.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string ERROR = (view.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(ERROR))
                        {
                            e.RepositoryItem = repositoryItemBtnLineError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    ADO.EmrSignerAdo data = (ADO.EmrSignerAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
    }
}
