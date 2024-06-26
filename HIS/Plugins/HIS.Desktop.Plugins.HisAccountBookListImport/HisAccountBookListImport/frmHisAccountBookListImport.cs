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
using HIS.Desktop.Common;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisAccountBookListImport.ADO;
using HIS.Desktop.Utility;
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

namespace HIS.Desktop.Plugins.HisAccountBookListImport.HisAccountBookListImport
{
    public partial class frmHisAccountBookListImport : FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        List<HisAccountBookListImportADO> _accountBookAdos;
        List<HisAccountBookListImportADO> errorLine;
        RefeshReference delegateRefresh;
        List<HisAccountBookListImportADO> _CurrentAdos;
        int checkButtonErrorLine = 0;
        bool isFirstTime = true;
        CommonParam paramDB = new CommonParam();
        HisAccountBookFilter filterDB = new HisAccountBookFilter();
        List<HIS_ACCOUNT_BOOK> _ListAccountBook { get; set; }
        List<HIS_ACCOUNT_BOOK> rsDB = new List<HIS_ACCOUNT_BOOK>();
        #endregion

        #region Construct
        public frmHisAccountBookListImport()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisAccountBookListImport(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this._Module = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private void frmHisAccountBookListImport_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
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
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ACCOUNT_BOOK.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_ACCOUNT_BOOK";
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
                        var hisAccBookImport = import.GetWithCheck<HisAccountBookListImportADO>(0);
                        if (hisAccBookImport != null && hisAccBookImport.Count > 0)
                        {
                            List<HisAccountBookListImportADO> listAfterRemove = new List<HisAccountBookListImportADO>();


                            foreach (var item in hisAccBookImport)
                            {
                                bool checkNull =
                                 string.IsNullOrEmpty(item.ACCOUNT_BOOK_CODE) && string.IsNullOrEmpty(item.ACCOUNT_BOOK_NAME) && string.IsNullOrEmpty(item.TOTAL_STR.ToString()) && string.IsNullOrEmpty(item.FROM_NUM_ORDER_STR.ToString()) && string.IsNullOrEmpty(item.BILL_TYPE_ID.ToString()) && string.IsNullOrEmpty(item.TEMPLATE_CODE) && string.IsNullOrEmpty(item.SYMBOL_CODE) && string.IsNullOrEmpty(item.EINVOICE_TYPE_ID.ToString()) && string.IsNullOrEmpty(item.RELEASE_TIME.ToString()) && string.IsNullOrEmpty(item.NUM_ORDER.ToString()) && string.IsNullOrEmpty(item.DESCRIPTION) && string.IsNullOrEmpty(item.MAX_ITEM_NUM_PER_TRANS.ToString());
                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }


                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._accountBookAdos = new List<HisAccountBookListImportADO>();
                                rsDB = new BackendAdapter(paramDB).Get<List<HIS_ACCOUNT_BOOK>>("api/HisAccountBook/Get", ApiConsumers.MosConsumer, filterDB, paramDB).ToList();
                                addServiceToProcessList(_CurrentAdos, ref this._accountBookAdos);
                                isFirstTime = false;
                                SetDataSource(this._accountBookAdos);
                                CheckErrorLine();
                            }
                            WaitingManager.Hide();
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

        private void CheckErrorLine()
        {
            try
            {
                var checkError = this._accountBookAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnSave.Enabled = true;
                    btnShowLineError.Enabled = false;
                    SetDataSource(this._accountBookAdos);
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

        private void SetDataSource(List<HisAccountBookListImportADO> dataSource)
        {
            try
            {
                gridControlAccountBookListImport.BeginUpdate();
                gridControlAccountBookListImport.DataSource = null;
                gridControlAccountBookListImport.DataSource = dataSource;
                gridControlAccountBookListImport.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addServiceToProcessList(List<HisAccountBookListImportADO> _CurrentAdosAdd, ref List<HisAccountBookListImportADO> _accountBookAdos)
        {
            try
            {
                _accountBookAdos = new List<HisAccountBookListImportADO>();
                long i = 0;
                CommonParam param = new CommonParam();
                HisWorkingShiftFilter filterWShift = new HisWorkingShiftFilter();
                filterWShift.IS_ACTIVE = 1;
                var rsWShift = new BackendAdapter(param).Get<List<HIS_WORKING_SHIFT>>("api/HisWorkingShift/Get", ApiConsumers.MosConsumer, filterWShift, param);
                HisEinvoiceTypeFilter filterEinvoice = new HisEinvoiceTypeFilter();
                filterEinvoice.IS_ACTIVE = 1;
                var rsEinvoice = new BackendAdapter(param).Get<List<HIS_EINVOICE_TYPE>>("api/HisEinvoiceType/Get", ApiConsumers.MosConsumer, filterEinvoice, param);

                foreach (var item in _CurrentAdosAdd)
                {
                    i++;
                    string error = "";
                    var accountBookAdo = new HisAccountBookListImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisAccountBookListImportADO>(accountBookAdo, item);
                    bool checkTrungDB = false;
                    bool checkTrungTrongFile = false;
                    bool checkTrungBookCodeTrongFile = false;
                    bool checkTrungBookNameTrongFile = false;
                    bool checkTonTaiAccBookCode = false;
                    int demSo = 0;

                    List<HIS_ACCOUNT_BOOK> lstAccBookCheck = new List<HIS_ACCOUNT_BOOK>();
                    lstAccBookCheck = null;
                    // check trung trong file import
                    if (!string.IsNullOrEmpty(item.ACCOUNT_BOOK_CODE) && item.ACCOUNT_BOOK_CODE != "" && !string.IsNullOrEmpty(item.ACCOUNT_BOOK_NAME) && item.ACCOUNT_BOOK_NAME != "" && !string.IsNullOrEmpty(item.TOTAL_STR.ToString()) && item.TOTAL_STR.ToString() != "" && !string.IsNullOrEmpty(item.FROM_NUM_ORDER_STR.ToString()) && item.FROM_NUM_ORDER_STR.ToString() != "")
                    {
                        var count = _CurrentAdosAdd.Where(o => o.ACCOUNT_BOOK_CODE == item.ACCOUNT_BOOK_CODE && o.ACCOUNT_BOOK_NAME == item.ACCOUNT_BOOK_NAME && o.TOTAL == Inventec.Common.TypeConvert.Parse.ToInt64(item.TOTAL_STR.ToString()) && o.FROM_NUM_ORDER == Inventec.Common.TypeConvert.Parse.ToInt64(item.FROM_NUM_ORDER_STR.ToString()) && o.BILL_TYPE_ID == item.BILL_TYPE_ID && o.TEMPLATE_CODE == item.TEMPLATE_CODE && o.SYMBOL_CODE == item.SYMBOL_CODE && o.RELEASE_TIME == item.RELEASE_TIME && o.NUM_ORDER == item.NUM_ORDER && o.DESCRIPTION == item.DESCRIPTION && o.MAX_ITEM_NUM_PER_TRANS == item.MAX_ITEM_NUM_PER_TRANS && o.IS_NOT_GEN_TRANSACTION_ORDER_STR == item.IS_NOT_GEN_TRANSACTION_ORDER_STR && o.WORKING_SHIFT_ID == item.WORKING_SHIFT_ID && o.EINVOICE_TYPE_ID == item.EINVOICE_TYPE_ID && o.IS_FOR_BILL_STR == item.IS_FOR_BILL_STR && o.IS_FOR_DEPOSIT_STR == item.IS_FOR_DEPOSIT_STR && o.IS_FOR_REPAY_STR == item.IS_FOR_REPAY_STR && o.IS_FOR_DEBT_STR == item.IS_FOR_DEBT_STR && o.IS_FOR_OTHER_SALE_STR == item.IS_FOR_OTHER_SALE_STR).ToList();
                        if (count.Count > 1)
                            checkTrungTrongFile = true;
                        else
                        {
                            var countBookCode = _CurrentAdosAdd.Where(o => o.ACCOUNT_BOOK_CODE == item.ACCOUNT_BOOK_CODE).ToList();
                            if (countBookCode.Count > 1)
                                checkTrungBookCodeTrongFile = true;
                            //else
                            //{
                            //    var countBookName = _CurrentAdosAdd.Where(o => o.ACCOUNT_BOOK_NAME == item.ACCOUNT_BOOK_NAME).ToList();
                            //    if (countBookName.Count > 1)
                            //        checkTrungBookNameTrongFile = true;
                            //}
                        }

                    }
                    if (checkTrungTrongFile)
                    {
                        error += string.Format(Message.MessageImport.TonTaiTrungNhauTrongFileImport);
                    }
                    else if (checkTrungBookCodeTrongFile)
                    {
                        error += string.Format(Message.MessageImport.TrungMaSo);
                    }
                    // check neu du lieu trong file import da ton tai trong DB
                    {
                        if (!string.IsNullOrEmpty(item.ACCOUNT_BOOK_CODE) && item.ACCOUNT_BOOK_CODE != "" && !string.IsNullOrEmpty(item.ACCOUNT_BOOK_NAME) && item.ACCOUNT_BOOK_NAME != "" && !string.IsNullOrEmpty(item.TOTAL_STR.ToString()) && item.TOTAL_STR.ToString() != "" && !string.IsNullOrEmpty(item.FROM_NUM_ORDER_STR.ToString()) && item.FROM_NUM_ORDER_STR.ToString() != "")
                        {
                            List<HIS_ACCOUNT_BOOK> rs = rsDB.Where(o => o.ACCOUNT_BOOK_CODE == item.ACCOUNT_BOOK_CODE && o.ACCOUNT_BOOK_NAME == item.ACCOUNT_BOOK_NAME && o.TOTAL == Inventec.Common.TypeConvert.Parse.ToInt64(item.TOTAL_STR.ToString()) && o.FROM_NUM_ORDER == Inventec.Common.TypeConvert.Parse.ToInt64(item.FROM_NUM_ORDER_STR.ToString())).ToList();
                            // chỉ so sánh UK của 1 bảng thôi
                            if (rs != null && rs.Count > 0)
                                checkTrungDB = true;


                        }
                        if (checkTrungDB)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.DBDaTonTai, item.ACCOUNT_BOOK_CODE);
                        }
                        else
                        {
                            List<HIS_ACCOUNT_BOOK> rsAccBookCode = rsDB.Where(o => o.ACCOUNT_BOOK_CODE == item.ACCOUNT_BOOK_CODE).ToList();
                            // chỉ so sánh UK của 1 bảng thôi
                            if (rsAccBookCode != null && rsAccBookCode.Count > 0)
                                checkTonTaiAccBookCode = true;
                            if (checkTonTaiAccBookCode)
                            {
                                if (error != "") error += " | ";
                                error += "Mã sổ " + item.ACCOUNT_BOOK_CODE + " đã tồn tại trên hệ thống";
                            }
                        }
                    }
                    // check tinh phu hop cua cac du lieu truyen vao
                    if (!string.IsNullOrEmpty(item.ACCOUNT_BOOK_CODE) || item.ACCOUNT_BOOK_CODE != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.ACCOUNT_BOOK_CODE) > 6)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mã sổ");
                        }
                        else
                        {
                            lstAccBookCheck = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().Where(p => p.ACCOUNT_BOOK_CODE == item.ACCOUNT_BOOK_CODE).ToList();
                            if (lstAccBookCheck != null && lstAccBookCheck.Count > 0)
                            {
                                var itemCurrent = lstAccBookCheck.FirstOrDefault();
                                if (itemCurrent != null)
                                    if (itemCurrent.IS_ACTIVE == 1)
                                    {
                                        // machineAdo.ID = itemCurrent.ID;
                                    }
                                    else
                                    {
                                        if (error != "") error += " | ";
                                        error += string.Format(Message.MessageImport.MaLoaiSoDaKhoa);
                                    }
                            }
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã sổ");
                    }

                    if (!string.IsNullOrEmpty(item.ACCOUNT_BOOK_NAME) || item.ACCOUNT_BOOK_NAME != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.ACCOUNT_BOOK_NAME) > 100)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Tên sổ");
                        }
                        else
                        {
                            lstAccBookCheck = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().Where(p => p.ACCOUNT_BOOK_NAME == item.ACCOUNT_BOOK_NAME).ToList();
                            if (lstAccBookCheck != null && lstAccBookCheck.Count > 0)
                            {
                                var itemCurrent = lstAccBookCheck.FirstOrDefault();
                                if (itemCurrent != null)
                                    if (itemCurrent.IS_ACTIVE == 1)
                                    {
                                        // machineAdo.ID = itemCurrent.ID;
                                    }
                                    else
                                    {
                                        if (error != "") error += " | ";
                                        error += string.Format(Message.MessageImport.TenLoaiSoDaKhoa);
                                    }
                            }
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên sổ");
                    }

                    if (!string.IsNullOrEmpty(item.TOTAL_STR.ToString()))
                    {
                        
                        accountBookAdo.TOTAL_STR_ = item.TOTAL_STR;
                        if (Inventec.Common.String.CountVi.Count(item.TOTAL_STR.ToString()) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Số phiếu");
                        }
                        else
                        {
                            item.TOTAL = Inventec.Common.TypeConvert.Parse.ToInt64(item.TOTAL_STR.ToString());
                            if (item.TOTAL <= 0)
                            {
                                if (error != "") error += " | ";
                                error += "Trường Số phiếu phải là số nguyên dương";
                            }
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Số phiếu");
                        item.TOTAL = 0;
                    }

                    if (!string.IsNullOrEmpty(item.FROM_NUM_ORDER_STR.ToString()))
                    {
                        Inventec.Common.Logging.LogSystem.Info("FROM_NUM_ORDER_STR: " + item.FROM_NUM_ORDER_STR);
                       
                        accountBookAdo.FROM_NUM_ORDER_STR_ = (item.FROM_NUM_ORDER_STR);
                        if (Inventec.Common.String.CountVi.Count(item.FROM_NUM_ORDER_STR.ToString()) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "trường Từ số");
                        }
                        else
                        {
                            item.FROM_NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.FROM_NUM_ORDER_STR.ToString());
                            if (item.FROM_NUM_ORDER <= 0)
                            {
                                if (error != "") error += " | ";
                                error += "Trường Từ số phải là số nguyên dương";
                            }
                        }
                    }
                    else
                    {
                        if (error != "") error += " | ";
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Từ số");
                    }

                    if (!string.IsNullOrEmpty(item.BILL_TYPE_ID.ToString()) && item.BILL_TYPE_ID.ToString() != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.BILL_TYPE_ID.ToString()) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Loại hóa đơn");
                        }
                        if (item.BILL_TYPE_ID != 1 && item.BILL_TYPE_ID != 2)
                        {
                            if (error != "") error += " | ";
                            error += "Loại hóa đơn không tồn tại";
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TEMPLATE_CODE) && item.TEMPLATE_CODE != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.TEMPLATE_CODE) > 11)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mẫu sổ ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.SYMBOL_CODE) && item.SYMBOL_CODE != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.SYMBOL_CODE) > 8)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Ký hiệu ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.EINVOICE_TYPE_ID_STR) && item.EINVOICE_TYPE_ID_STR != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.EINVOICE_TYPE_ID_STR.ToString()) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Hóa đơn điện tử ");
                        }
                        else
                        {
                            if (rsEinvoice != null && rsEinvoice.Count > 0)
                            {
                                var check = rsEinvoice.Where(o => o.EINVOICE_TYPE_CODE == item.EINVOICE_TYPE_ID_STR).ToList();
                                if (!(check != null && check.Count > 0))
                                {
                                    if (error != "") error += " | ";
                                    error += "Hóa đơn điện tử không tồn tại";
                                    accountBookAdo.EINVOICE_TYPE_ID_STR_ = item.EINVOICE_TYPE_ID_STR;
                                }
                                else
                                {
                                    accountBookAdo.EINVOICE_TYPE_ID = check.First().ID;
                                    accountBookAdo.EINVOICE_TYPE_ID_STR_ = check.First().EINVOICE_TYPE_NAME;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.RELEASE_TIME.ToString()) && item.RELEASE_TIME.ToString() != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.RELEASE_TIME.ToString()) > 14)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Ngày phát hành ");
                        }

                        else if (Inventec.Common.String.CountVi.Count(item.RELEASE_TIME.ToString()) < 14)
                        {
                            if (error != "") error += " | ";
                            error += "Dữ liệu ngày phát hành không đúng định dạng";
                        }
                        else
                        {
                            DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.RELEASE_TIME ?? 0);
                            if (dt == null || dt < DateTime.MinValue)
                            {
                                if (error != "") error += " | ";
                                error += "Dữ liệu ngày phát hành không đúng định dạng";
                            }
                            else
                                item.RELEASE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.RELEASE_TIME ?? 0);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.NUM_ORDER.ToString()) && item.NUM_ORDER.ToString() != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.NUM_ORDER.ToString()) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Thứ tự ưu tiên ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.DESCRIPTION) && item.DESCRIPTION != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.DESCRIPTION) > 2000)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Mô tả ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.MAX_ITEM_NUM_PER_TRANS.ToString()) && item.MAX_ITEM_NUM_PER_TRANS.ToString() != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.MAX_ITEM_NUM_PER_TRANS.ToString()) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Số mặt hàng tối đa ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.WORKING_SHIFT_ID_STR) && item.WORKING_SHIFT_ID_STR != "")
                    {
                        if (Inventec.Common.String.CountVi.Count(item.WORKING_SHIFT_ID_STR) > 19)
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.Maxlength, "Ca làm việc ");
                        }
                        else
                        {
                            if (rsWShift != null && rsWShift.Count > 0)
                            {
                                var check = rsWShift.Where(o => o.WORKING_SHIFT_CODE == item.WORKING_SHIFT_ID_STR).ToList();
                                if (!(check != null && check.Count > 0))
                                {
                                    if (error != "") error += " | ";
                                    error += "Ca làm việc không tồn tại";
                                    accountBookAdo.WORKING_SHIFT_ID_STR_ = item.WORKING_SHIFT_ID_STR;
                                }
                                else
                                {
                                    accountBookAdo.WORKING_SHIFT_ID = check.First().ID;
                                    accountBookAdo.WORKING_SHIFT_ID_STR_ = check.First().WORKING_SHIFT_NAME;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_NOT_GEN_TRANSACTION_ORDER_STR))
                    {
                        if (item.IS_NOT_GEN_TRANSACTION_ORDER_STR.ToLower().Trim() == "x")
                        {
                            accountBookAdo.IS_NOT_GEN_TRANSACTION_ORDER = 1;

                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sổ chứng từ ");
                        }
                    }

                    if (!string.IsNullOrEmpty(item.IS_FOR_BILL_STR))
                    {
                        if (item.IS_FOR_BILL_STR.ToLower().Trim() == "x")
                        {
                            accountBookAdo.IS_FOR_BILL = 1;
                            demSo += 1;
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sổ thanh toán ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_FOR_DEPOSIT_STR))
                    {
                        if (item.IS_FOR_DEPOSIT_STR.ToLower().Trim() == "x")
                        {
                            accountBookAdo.IS_FOR_DEPOSIT = 1;
                            demSo += 1;
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sổ tạm ứng ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_FOR_REPAY_STR))
                    {
                        if (item.IS_FOR_REPAY_STR.ToLower().Trim() == "x")
                        {
                            accountBookAdo.IS_FOR_REPAY = 1;
                            demSo += 1;
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sổ hoàn ứng ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_FOR_DEBT_STR))
                    {
                        if (item.IS_FOR_DEBT_STR.ToLower().Trim() == "x")
                        {
                            accountBookAdo.IS_FOR_DEBT = 1;
                            demSo += 1;
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sổ công nợ ");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.IS_FOR_OTHER_SALE_STR))
                    {
                        if (item.IS_FOR_OTHER_SALE_STR.ToLower().Trim() == "x")
                        {
                            accountBookAdo.IS_FOR_OTHER_SALE = 1;
                            demSo += 1;
                        }
                        else
                        {
                            if (error != "") error += " | ";
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sổ thanh toán khác ");
                        }
                    }
                    if (demSo == 0)
                    {
                        if (error != "") error += " | ";
                        error += "Bắt buộc chọn ít nhất 1 loại sổ";
                    }
                    else if ((demSo == 2 && (accountBookAdo.IS_FOR_DEBT == 1 || accountBookAdo.IS_FOR_OTHER_SALE == 1 || accountBookAdo.IS_FOR_BILL == 1)) || demSo > 2)
                    {
                        if (error != "") error += " | ";
                        error += "Chỉ được phép chọn 1 trong 5 loại sổ. Trừ loại hoàn ứng và tạm ứng là được tích chọn với nhau";
                    }


                    accountBookAdo.ERROR = error;
                    accountBookAdo.ID = i;

                    _accountBookAdos.Add(accountBookAdo);
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
                    errorLine = this._accountBookAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 1;

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    errorLine = this._accountBookAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                    CheckErrorLine();
                    checkButtonErrorLine = 2;
                    if (errorLine != null && errorLine.Count > 0)
                        btnSave.Enabled = true;
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
                bool success = false;
                WaitingManager.Show();
                List<HIS_ACCOUNT_BOOK> datas = new List<HIS_ACCOUNT_BOOK>();
                if (this.errorLine == null)
                    this.errorLine = this._accountBookAdos;
                if (this.errorLine != null && this.errorLine.Count > 0)
                {
                    foreach (var item in this.errorLine)
                    {
                        HIS_ACCOUNT_BOOK ado = new HIS_ACCOUNT_BOOK();
                        // ado.GROUP_CODE = item.GROUP_CODE;
                        var result = BackendDataWorker.Get<HIS_ACCOUNT_BOOK>().FirstOrDefault(p => p.ACCOUNT_BOOK_CODE == item.ACCOUNT_BOOK_CODE);
                        if (result != null)
                            ado.ID = result.ID;
                        ado.ACCOUNT_BOOK_CODE = item.ACCOUNT_BOOK_CODE;
                        ado.ACCOUNT_BOOK_NAME = item.ACCOUNT_BOOK_NAME;
                        ado.TOTAL = Inventec.Common.TypeConvert.Parse.ToInt64(item.TOTAL_STR.ToString());
                        ado.FROM_NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(item.FROM_NUM_ORDER_STR.ToString());
                        ado.BILL_TYPE_ID = item.BILL_TYPE_ID;
                        ado.TEMPLATE_CODE = item.TEMPLATE_CODE;
                        ado.SYMBOL_CODE = item.SYMBOL_CODE;
                        ado.RELEASE_TIME = item.RELEASE_TIME;
                        ado.EINVOICE_TYPE_ID = item.EINVOICE_TYPE_ID;
                        ado.NUM_ORDER = item.NUM_ORDER;
                        ado.DESCRIPTION = item.DESCRIPTION;
                        ado.MAX_ITEM_NUM_PER_TRANS = item.MAX_ITEM_NUM_PER_TRANS;
                        ado.IS_NOT_GEN_TRANSACTION_ORDER = item.IS_NOT_GEN_TRANSACTION_ORDER;
                        ado.WORKING_SHIFT_ID = item.WORKING_SHIFT_ID;
                        ado.IS_FOR_BILL = item.IS_FOR_BILL;
                        ado.IS_FOR_DEPOSIT = item.IS_FOR_DEPOSIT;
                        ado.IS_FOR_REPAY = item.IS_FOR_REPAY;
                        ado.IS_FOR_DEBT = item.IS_FOR_DEBT;
                        ado.IS_FOR_OTHER_SALE = item.IS_FOR_OTHER_SALE;
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
                if (datas != null && datas.Count > 0)
                {
                    var dataImports = new BackendAdapter(param).Post<List<HIS_ACCOUNT_BOOK>>("api/HisAccountBook/CreateList", ApiConsumers.MosConsumer, datas, param);
                    WaitingManager.Hide();
                    if (dataImports != null && dataImports.Count > 0)
                    {
                        success = true;
                        btnSave.Enabled = false;
                        if (this.delegateRefresh != null)
                        {
                            this.delegateRefresh();
                        }
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccountBookListImport_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewAccountBookListImport.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
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

        private void gridViewAccountBookListImport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisAccountBookListImportADO pData = (HisAccountBookListImportADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IS_FOR_BILL_STR_BOOL")
                    {
                        e.Value = pData.IS_FOR_BILL == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_FOR_DEPOSIT_STR_BOOL")
                    {
                        e.Value = pData.IS_FOR_DEPOSIT == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_FOR_REPAY_STR_BOOL")
                    {
                        e.Value = pData.IS_FOR_REPAY == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_FOR_DEBT_STR_BOOL")
                    {
                        e.Value = pData.IS_FOR_DEBT == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_FOR_OTHER_SALE_STR_BOOL")
                    {
                        e.Value = pData.IS_FOR_OTHER_SALE == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_NOT_GEN_TRANSACTION_ORDER_STR_BOOL")
                    {
                        e.Value = pData.IS_NOT_GEN_TRANSACTION_ORDER == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "RELEASE_TIME_STR_")
                    {
                        //e.Value = pData.RELEASE_TIME_STR;
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.RELEASE_TIME ?? 0);

                    }
                    else if (e.Column.FieldName == "BILL_TYPE_ID_STR")
                    {
                        switch (pData.BILL_TYPE_ID)
                        {
                            case 1: { e.Value = "Thường"; break; }
                            case 2: { e.Value = "Dịch vụ"; break; }
                            default: { e.Value = pData.BILL_TYPE_ID; break; }
                        }

                    }
                    //else if (e.Column.FieldName == "EINVOICE_TYPE_ID_STR")
                    //{
                    //    switch (pData.EINVOICE_TYPE_ID)
                    //    {
                    //        case 1: { e.Value = "BKAV"; break; }
                    //        case 2: { e.Value = "VIETTEL"; break; }
                    //        case 3: { e.Value = "VNPT"; break; }
                    //        default: { e.Value = pData.EINVOICE_TYPE_ID; break; }
                    //    }

                    //}
                    else if (e.Column.FieldName == "WORKING_SHIFT_ID_STR")
                    {
                        // e.Value = pData.WORKING_SHIFT_ID_STR;
                        //switch (pData.WORKING_SHIFT_ID)
                        //{
                        //    case 1: { e.Value = "BKAV"; break; }
                        //    case 2: { e.Value = "VIETTEL"; break; }
                        //    case 3: { e.Value = "VNPT"; break; }
                        //    default: { e.Value = pData.WORKING_SHIFT_ID; break; }
                        //}

                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisAccountBookListImportADO)gridViewAccountBookListImport.GetFocusedRow();
                if (row != null)
                {
                    if (this._accountBookAdos != null && this._accountBookAdos.Count > 0)
                    {
                        this._accountBookAdos.Remove(row);
                        List<HisAccountBookListImportADO> rs = this._accountBookAdos;
                        addServiceToProcessList(rs, ref this._accountBookAdos);
                        switch (checkButtonErrorLine)
                        {
                            case 0:
                                {
                                    SetDataSource(this._accountBookAdos);
                                    CheckErrorLine();
                                    break;
                                }
                            case 1:
                                {
                                    SetDataSource(this._accountBookAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList());
                                    CheckErrorLine();
                                    break;
                                }
                            case 2:
                                {
                                    SetDataSource(this._accountBookAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList()); CheckErrorLine();
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

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisAccountBookListImportADO)gridViewAccountBookListImport.GetFocusedRow();
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
