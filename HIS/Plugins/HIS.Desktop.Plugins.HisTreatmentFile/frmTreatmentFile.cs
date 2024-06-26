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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisTreatmentFile.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Integrate;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid;
namespace HIS.Desktop.Plugins.HisTreatmentFile
{
    public partial class frmTreatmentFile : FormBase
    {
        #region Declare

        DelegateSelectData dlgGetImageFromModuleCamera;

        private BindingSource _bdSource = new BindingSource();
        string[] fullfileNameAttack;
        AttackADO fileNameAttack;
        List<AttackADO> ListfileNameAttack = new List<AttackADO>();
        HIS_TREATMENT currentTreatment;
        private AttackADO currentDataClick;
        Action actRefesh;
        ImageFormat format;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        List<HIS_FILE_TYPE> currentFileType;
        V_HIS_ROOM room = null;
        long _TreatmentId = 0;
        string urlTreatmentFile = "";
        long idTreatmentFile;
        int positionHandle = -1;
        Dictionary<long, string> DicoutPdfFile = new Dictionary<long, string>();
        /// <summary>
        /// lấy các dữ liệu có ID > 0
        /// </summary>
        int currentPage = 0;

        string loginName = null;

        bool checkColumn = false;
        string outPdfFile = "";
        Dictionary<long, string> DicoutPdfFilePrint;

        #endregion
        public frmTreatmentFile(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            currentModule = module;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        public frmTreatmentFile(Inventec.Desktop.Common.Modules.Module module, long _treatmentId)
            : base(module)
        {
            InitializeComponent();
            currentModule = module;
            this._TreatmentId = _treatmentId;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitControlForm()
        {
            try
            {
                InitCboFileType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboFileType()
        {
            Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
            HisFileTypeFilter filter = new HisFileTypeFilter();
            filter.IS_ACTIVE = 1;
            var dataFileType = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_FILE_TYPE>>("api/HisFileType/Get",
                    ApiConsumers.MosConsumer, filter, param);

            if (dataFileType != null && dataFileType.Count() > 0)
            {
                currentFileType = dataFileType;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("FILE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("FILE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FILE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboFileType, dataFileType, controlEditorADO);
                cboFileType.EditValue = null;
            }
        }
        public void MeShow()
        {

        }

        private void FillDataFormList()
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                int numPageSize = 0;
                if (ucPagingTreatmentFile.pagingGrid != null)
                {
                    numPageSize = ucPagingTreatmentFile.pagingGrid.PageSize;
                }
                else
                {

                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new Inventec.Core.CommonParam(0, numPageSize));

                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingTreatmentFile.Init(LoadPaging, param, numPageSize, this.gridControl1);

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((Inventec.Core.CommonParam)param).Start ?? 0;
                int limit = ((Inventec.Core.CommonParam)param).Limit ?? 0;
                Inventec.Core.CommonParam paramCommon = null;
                Inventec.Core.ApiResultObject<List<HIS_TREATMENT_FILE>> apiResult = null;

                HisTreatmentFileFilter filter = new HisTreatmentFileFilter();
                SetFilterNavBar(ref filter);
                filter.TREATMENT_ID = _TreatmentId;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<HIS_TREATMENT_FILE>>("api/HisTreatmentFile/Get",
                    ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_TREATMENT_FILE>)apiResult.Data;
                    if (data != null)
                    {
                        grvFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisTreatmentFileFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadTreatment()
        {
            Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
            HisTreatmentFilter filter = new HisTreatmentFilter();
            filter.ID = _TreatmentId;
            var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get",
                    ApiConsumers.MosConsumer, filter, param).First();
            if (result != null)
            {
                currentTreatment = result;
            }
        }

        private void cboFileType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisFileType", WorkPlace.GetRoomId(), WorkPlace.GetRoomTypeIds().FirstOrDefault(), listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessImage();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageDefaultPath;
        }

        private void ProcessImage()
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Multiselect = true;
            openFile.Filter = "Ảnh(*.jpg, *.png, *.jpeg, *.bmp,*.gif,*.pdf)|*.jpg;*.png;*.jpeg;*.bmp;*.gif;*.pdf";
            openFile.DefaultExt = ".jpg;.png;.jpeg;.bmp;.gif;.pdf";
            //.JPG,.PNG,.JPEG,.BMP,.GIF,.PDF
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                this.fullfileNameAttack = openFile.FileNames;
                if (this.fullfileNameAttack != null)
                {
                    List<FileHolder> files = new List<FileHolder>();
                    string url = "";
                    bool sucess = false;
                    string dstFileName = "";
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullfileNameAttack), fullfileNameAttack));

                    for (int i = 0; i < this.fullfileNameAttack.Length; i++)
                    {
                        string item = fullfileNameAttack[i];


                        int lIndex = item.LastIndexOf("\\");
                        int lIndex1 = item.LastIndexOf(".");
                        string fileName = item.Split('\\').LastOrDefault();
                        fileName = fileName.Split('.').FirstOrDefault();
                        string ext = Path.GetExtension(item);


                        this.fileNameAttack = new AttackADO();
                        this.fileNameAttack.FILE_NAME = item.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                        this.fileNameAttack.Extension = item.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                        this.fileNameAttack.Base64Data = Inventec.Common.SignLibrary.Utils.FileToBase64String(item);
                        this.fileNameAttack.FullName = item;
                        this.fileNameAttack.IsFss = false;
                        string extension = System.IO.Path.GetExtension(item);
                        if (extension.ToLower() != ".pdf" )
                        {
                            this.fileNameAttack.image = System.Drawing.Image.FromFile(item);
                        }
                        else
                        {
                            string pathLocal = GetPathDefault();
                            this.fileNameAttack.image = System.Drawing.Image.FromFile(pathLocal);
                            //byte[] buffPDF = System.IO.File.ReadAllBytes(item);
                            //byte[] imageBytes = Convert.FromBase64String(this.fileNameAttack.Base64Data);
                            //MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                            //ms.Write(imageBytes, 0, imageBytes.Length);
                            //this.fileNameAttack.image = System.Drawing.Image.FromStream(ms, true);

                        }

                        byte[] buff = System.IO.File.ReadAllBytes(item);
                        MemoryStream stream = new MemoryStream(buff);
                        stream.Position = 0;
                        FileHolder file = new FileHolder();
                        file.FileName = Inventec.Common.DateTime.Get.Now().ToString() + "_" + fileName + ext;
                        file.Content = stream;
                        files.Add(file);
                        url = currentTreatment.TREATMENT_CODE + "\\ATTACHMENT_FILE";
                        fileNameAttack.Url = url + "\\" + file.FileName;
                        //this.fileNameAttack.FullName = url;
                        this.ListfileNameAttack.Add(fileNameAttack);
                    }
                    var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, url, files, true);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsUpload), rsUpload));
                    if (rsUpload != null)
                    {
                        foreach (var itemUpload in rsUpload)
                        {
                            foreach (var itemAttack in this.ListfileNameAttack)
                            {
                                if (!string.IsNullOrEmpty(itemAttack.Url) && (itemAttack.Url).Contains(itemUpload.OriginalName))
                                {
                                    itemAttack.Url = itemUpload.Url;
                                    itemAttack.IsChecked = true;
                                    break;
                                }
                            }
                        }
                        sucess = true;
                        FilldataToTittleView(this.ListfileNameAttack);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.",
                       "Thông báo");
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    MessageManager.Show(this, param, sucess);
                }
            }
        }

        private void FilldataToTittleView(List<AttackADO> lstfileNameAttack)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstfileNameAttack.Count()), lstfileNameAttack.Count()));
                cardControl.BeginUpdate();
                cardControl.DataSource = null;
                List<AttackADO> listImage = new List<AttackADO>();
                if (lstfileNameAttack.Count() > 0)
                {
                    foreach (var item in lstfileNameAttack)
                    {
                        if (item.FullName != null && item.image != null)
                        {
                            listImage.Add(item);
                        }
                    }
                    if (listImage != null && listImage.Count > 0)
                    {
                        cardControl.DataSource = listImage;
                    }
                    cardControl.EndUpdate();
                    toggleSwitch2.IsOn = true;
                    //lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
                }
            }
            catch (Exception ex)
            {
                cardControl.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected static float CalculateFolderSize(string file)
        {
            float folderSize = 0.0f;
            try
            {
                // Kiểm tra đường dẫn
                if (!File.Exists(file))
                    return folderSize;
                else
                {
                    try
                    {
                        FileInfo finfo = new FileInfo(file);
                        folderSize += finfo.Length;
                    }
                    catch (NotSupportedException e)
                    {
                        Inventec.Common.Logging.LogSystem.Error(e);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Inventec.Common.Logging.LogSystem.Error(e);
            }
            return folderSize;
        }

        private void frmTreatmentFile_Load(object sender, EventArgs e)
        {
            this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.btnEdit.Enabled = false;
            InitControlForm();
            LoadTreatment();
            FillDataFormList();
            ValidateForm();
        }

        //private void grvShowFile_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        AttackADO data = (AttackADO)grvShowFile.GetFocusedRow();
        //        if (data != null && data.FullName != null)
        //        {
        //            if (System.IO.Path.GetExtension(data.FullName) == ".pdf")
        //            {
        //                this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                if (data.SttPdf == 1)
        //                {
        //                    var stream = Inventec.Fss.Client.FileDownload.GetFile(data.FILE_URLS);
        //                    pdfViewer1.LoadDocument(stream);
        //                }
        //                else
        //                {
        //                    pdfViewer1.LoadDocument(data.FullName);
        //                }

        //            }
        //            else
        //            {
        //                this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                if (data.image != null)
        //                {
        //                    pteAnhChupFileDinhKem.Image = data.image;
        //                }
        //                else
        //                {
        //                    if (data.FILE_URLS != null)
        //                    {
        //                        var stream = Inventec.Fss.Client.FileDownload.GetFile(data.FILE_URLS);
        //                        pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromStream(stream);
        //                    }

        //                }
        //            }
        //            pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
        //            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pteAnhChupFileDinhKem.Image.Tag), pteAnhChupFileDinhKem.Image.Tag));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Warn(ex);
        //    }
        //}

        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Camera").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Camera");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    this.dlgGetImageFromModuleCamera = this.FillImageFromModuleCamereToUC;
                    listArgs.Add(this.dlgGetImageFromModuleCamera);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                }
                if (ListfileNameAttack != null && ListfileNameAttack.Count > 0)
                {
                    toggleSwitch2.IsOn = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillImageFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    List<FileHolder> files = new List<FileHolder>();
                    bool sucess = false;
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Info("dataImage: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataImage), dataImage));
                    System.Drawing.Image image = (System.Drawing.Image)dataImage;

                    var check = this.ListfileNameAttack.OrderByDescending(o => o.Dem).FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Info("dem max: " + check);
                    int dem = 0;
                    if (check == null || check.Dem == 0)
                    {
                        dem = 1;
                    }
                    else
                    {
                        dem = check.Dem + 1;
                    }
                    fileNameAttack = new AttackADO();
                    string fileName = Inventec.Common.DateTime.Get.Now().ToString() + "_" + dem + ".jpg";
                    string ext = Path.GetExtension(fileName);
                    this.fileNameAttack.FILE_NAME = fileName;
                    this.fileNameAttack.FullName = fileName;
                    this.fileNameAttack.image = (System.Drawing.Image)dataImage;
                    this.fileNameAttack.Dem = dem;
                    this.fileNameAttack.IsFss = true;
                    this.fileNameAttack.Extension = ext;
                    this.fileNameAttack.Url = currentTreatment.TREATMENT_CODE + "\\ATTACHMENT_FILE" + "\\" + fileName;
                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.fileNameAttack.IsChecked = true;
                    this.ListfileNameAttack.Add(this.fileNameAttack);
                    Inventec.Common.Logging.LogSystem.Info("dữ liệu ảnh chụp: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), this.ListfileNameAttack));
                    using (MemoryStream memoryStreamImage = new MemoryStream())
                    {
                        using (Bitmap bmp = new Bitmap(image))//image là Image của từ ảnh chụp
                        {
                            bmp.Save(memoryStreamImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                            memoryStreamImage.Position = 0;
                            List<FileHolder> fileHolders = new List<FileHolder>();
                            FileHolder fHolder = new FileHolder(memoryStreamImage, fileName);//filename là tên của ảnh chụp
                            fileHolders.Add(fHolder);
                            string url = currentTreatment.TREATMENT_CODE + "\\ATTACHMENT_FILE";
                            var rsUpload = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, url, fileHolders, true);
                            if (rsUpload != null)
                            {
                                foreach (var itemUpload in rsUpload)
                                {
                                    foreach (var itemAttack in this.ListfileNameAttack)
                                    {
                                        if ((itemAttack.Url).Contains(itemUpload.OriginalName))
                                        {
                                            itemAttack.Url = itemUpload.Url;
                                        }
                                    }
                                }
                                sucess = true;
                            }
                            else
                            {
                                MessageBox.Show("Upload file thất bại, vui lòng liên hệ quản trị hệ thống để được hỗ trợ.");
                            }
                            Inventec.Desktop.Common.Message.WaitingManager.Hide();
                            MessageManager.Show(this, param, sucess);
                        }
                    }

                    cardControl.DataSource = null;
                    cardControl.DataSource = this.ListfileNameAttack;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var lstData = cardControl.DataSource as List<AttackADO>;

                if (lstData != null && lstData.Count > 0)
                {

                    positionHandle = -1;
                    if (!dxValidationProvider.Validate())
                        return;

                    HIS_TREATMENT_FILE treatmentFileCreate = new HIS_TREATMENT_FILE();
                    treatmentFileCreate.TREATMENT_ID = _TreatmentId;
                    if (cboFileType.EditValue != null)
                    {
                        treatmentFileCreate.FILE_TYPE_ID = (long)cboFileType.EditValue;
                    }
                    if (txtDescription.Text != null)
                    {
                        treatmentFileCreate.DESCRIPTION = txtDescription.Text;
                    }
                    //lstFileName1.Add(GlobalVariables.APPLICATION_CODE + "\\ATTACHMENT_FILE\\" + lstFileName);
                    Inventec.Common.Logging.LogSystem.Debug("ListfileNameAttack " + lstData.Count());
                    foreach (var item in lstData)
                    {
                        
                            if (!string.IsNullOrEmpty(this.urlTreatmentFile))
                            {
                                this.urlTreatmentFile += "|" + item.Url;
                            }
                            else
                            {
                                this.urlTreatmentFile += item.Url;
                            }
                        
                    }

                    if (!string.IsNullOrEmpty(this.urlTreatmentFile))
                    {
                        treatmentFileCreate.FILE_URLS = urlTreatmentFile;
                    }

                    //TODO
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentFileCreate), treatmentFileCreate));
                    if (treatmentFileCreate != null)
                    {
                        Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                        var resultData = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT_FILE>("api/HisTreatmentFile/Create", ApiConsumers.MosConsumer, treatmentFileCreate, param);
                        //var resultData = new BackendAdapter(param).PostWithFile<DocumentTDO>(EMR.URI.EmrDocument.CREATE_WITH_FILE, ApiConsumers.EmrConsumer, docCreate, files, param);

                        MessageManager.Show(this, param, resultData != null);
                        if (resultData != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("KQ:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                            FillDataFormList();
                            SetDefaultValue();
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn file ảnh từ máy tính hoặc chụp ảnh từ camera.",
                       "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private MemoryStream GetMemoryStreamFileData(string outFile)
        {
            MemoryStream streamData = null;
            try
            {
                if (!String.IsNullOrEmpty(outFile))
                {
                    streamData = new MemoryStream();
                    using (FileStream file = new FileStream(outFile, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        streamData.Write(bytes, 0, (int)file.Length);
                    }
                    streamData.Position = 0;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                streamData = null;
            }
            return streamData;
        }

        private string CombineMultiplePDFs()
        {
            Document document = new Document();
            string outFile = Path.GetTempFileName();
            PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));

            document.Open();


            foreach (var item in this.ListfileNameAttack)
            {
                string extensionc = System.IO.Path.GetExtension(item.FullName);
                if (extensionc == ".pdf")
                {
                    PdfReader reader = new PdfReader(item.FullName);
                    reader.ConsolidateNamedDestinations();


                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.NewPage();
                        writer.NewPage();
                        writer.AddPage(page);

                    }

                    PRAcroForm form = reader.AcroForm;
                    if (form != null)
                    {
                        writer.CopyDocumentFields(reader);
                    }

                    reader.Close();
                }
                else
                {
                    //FileInfo fi = new FileInfo(GeneratePdfFileFromImages());
                    //if (!string.IsNullOrEmpty(GeneratePdfFileFromImages()) && File.Exists(GeneratePdfFileFromImages()) && fi.Length > 0)
                    //{
                    //    PdfReader reader = new PdfReader(GeneratePdfFileFromImages());
                    //    reader.ConsolidateNamedDestinations();
                    //    for (int i = 1; i <= reader.NumberOfPages; i++)
                    //    {
                    //        PdfImportedPage page = writer.GetImportedPage(reader, i);
                    //        document.NewPage();
                    //        writer.NewPage();
                    //        writer.AddPage(page);

                    //    }
                    //} 

                    string outputss = Path.GetTempFileName();
                    FileStream fss = new FileStream(outputss, FileMode.Create, FileAccess.Write, FileShare.None);
                    Document docc = new Document();
                    PdfWriter.GetInstance(docc, fss);
                    docc.Open();
                    iTextSharp.text.Image image;
                    image = iTextSharp.text.Image.GetInstance(item.image, BaseColor.BLACK);
                    if (image.Height > image.Width)
                    {
                        float percentage = 0.0f;
                        percentage = document.PageSize.Height / image.Height;
                        image.ScalePercent(percentage * 90);
                    }
                    else
                    {
                        float percentage = 0.0f;
                        percentage = document.PageSize.Width / image.Width;
                        image.ScalePercent(percentage * 90);
                    }
                    docc.NewPage();
                    docc.Add(image);
                    docc.Close();
                    PdfReader reader = new PdfReader(outputss);
                    reader.ConsolidateNamedDestinations();
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.NewPage();
                        writer.NewPage();
                        writer.AddPage(page);
                    }

                }
            }

            writer.Close();
            document.Close();
            return outFile;
        }

        private void grvFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_TREATMENT_FILE pData = (HIS_TREATMENT_FILE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "FILE_TYPE_NAME_STR")
                    {
                        var typeName = currentFileType.Find(o => o.ID == pData.FILE_TYPE_ID);
                        if (typeName != null)
                        {
                            e.Value = typeName.FILE_TYPE_NAME;
                        }
                    }
                }
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var pdata = (HIS_TREATMENT_FILE)grvFormList.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pdata), pdata));
                if (pdata != null)
                {
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = true;

                    this.idTreatmentFile = pdata.ID;
                    cboFileType.EditValue = pdata.FILE_TYPE_ID;
                    txtDescription.Text = pdata.DESCRIPTION;

                    this.ListfileNameAttack = new List<AttackADO>();
                    if (!string.IsNullOrEmpty(pdata.FILE_URLS))
                    {
                        this.urlTreatmentFile = pdata.FILE_URLS;
                        string[] arrFileName = (pdata.FILE_URLS).Split('|');
                        foreach (var item in arrFileName)
                        {
                            int lIndex = item.LastIndexOf("\\");
                            int lIndex1 = item.LastIndexOf(".");
                            string fileName = item.Split('\\').LastOrDefault();
                            fileName = fileName.Split('.').FirstOrDefault();
                            string ext = Path.GetExtension(item);

                            this.fileNameAttack = new AttackADO();
                            this.fileNameAttack.FILE_NAME = item.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                            this.fileNameAttack.Extension = item.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                            this.fileNameAttack.FullName = item;
                            this.fileNameAttack.Url = item;
                            this.fileNameAttack.SttPdf = 1;
                            string extension = System.IO.Path.GetExtension(item);
                            this.fileNameAttack.IsFss = true;
                            if (extension.ToLower() != ".pdf")
                            {
                                MemoryStream fs = null;
                                fs = Inventec.Fss.Client.FileDownload.GetFile(item);
                                this.fileNameAttack.image = System.Drawing.Image.FromStream(fs);
                            }
                            else
                            {
                                string pathLocal = GetPathDefault();
                                this.fileNameAttack.image = System.Drawing.Image.FromFile(pathLocal);
                            }
                            this.ListfileNameAttack.Add(this.fileNameAttack);
                        }
                    }

                    cardControl.DataSource = null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), ListfileNameAttack));
                    cardControl.DataSource = ListfileNameAttack;
                    cardControl.EndUpdate();
                    if (ListfileNameAttack != null && ListfileNameAttack.Count > 0)
                    {
                        toggleSwitch2.IsOn = true;
                    }

                }
                else
                {
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDeleteFile_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            //try
            //{
            //    AttackADO data = (AttackADO)grvShowFile.GetFocusedRow();
            //    if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        this.ListfileNameAttack.Remove(data);
            //        var lstProcessUrlTreatmentFile = this.urlTreatmentFile.Split('|');
            //        lstProcessUrlTreatmentFile = lstProcessUrlTreatmentFile.Where(o => !o.Contains(data.FILE_NAME)).ToArray();
            //        var processUrlTreatmentFile = string.Join("|", lstProcessUrlTreatmentFile);
            //        this.urlTreatmentFile = processUrlTreatmentFile;
            //        //grvShowFile.BeginUpdate();
            //        //grvShowFile.GridControl.DataSource = (this.ListfileNameAttack != null ? this.ListfileNameAttack.ToList() : null);
            //        //grvShowFile.EndUpdate();

            //        pteAnhChupFileDinhKem.Image = null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = true;
                btnEdit.Enabled = false;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider, dxErrorProvider1);
                SetDefaultValue();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultValue()
        {
            try
            {
                toggleSwitch2.IsOn = false;
                cboFileType.EditValue = null;
                txtDescription.Text = null;
                pteAnhChupFileDinhKem.Image = null;
                pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                this.ListfileNameAttack = new List<AttackADO>();
                this.urlTreatmentFile = "";
                cardControl.DataSource = ListfileNameAttack;
                cardControl.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                HIS_TREATMENT_FILE rowData = (HIS_TREATMENT_FILE)grvFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ID________________________" + rowData.ID);
                        bool success = false;
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatmentFile/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataFormList();
                            ListfileNameAttack = new List<AttackADO>();
                            //grvFormList.BeginDataUpdate();
                            //grvShowFile.GridControl.DataSource = ListfileNameAttack;
                            //grvFormList.EndDataUpdate();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveFileStream(String path, Stream stream)
        {
            var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.CopyTo(fileStream);
            fileStream.Dispose();
        }

        private void btnGDownload_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                OpenFileDialog openFolder = new OpenFileDialog();
                openFolder.ValidateNames = false;
                openFolder.CheckFileExists = false;
                openFolder.CheckPathExists = true;
                openFolder.FileName = "Folder Selection.";
                if (openFolder.ShowDialog() == DialogResult.OK)
                {
                    bool success = false;
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    HIS_TREATMENT_FILE rowData = (HIS_TREATMENT_FILE)grvFormList.GetFocusedRow();
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                    string directoryPath = Path.GetDirectoryName(openFolder.FileName) + @"\" + currentTreatment.TREATMENT_CODE;
                    if (!System.IO.Directory.Exists(directoryPath))
                    {
                        System.IO.Directory.CreateDirectory(directoryPath);
                    }
                    if (!string.IsNullOrEmpty(rowData.FILE_URLS))
                    {
                        var fileUrl = rowData.FILE_URLS.Split('|');
                        for (int i = 0; i < fileUrl.Count(); i++)
                        {
                            var stream = Inventec.Fss.Client.FileDownload.GetFile(fileUrl[i]);
                            var filename = fileUrl[i].Split('\\').Last();
                            Inventec.Common.Logging.LogSystem.Debug("FILE_NAME___________________________" + filename);
                            SaveFileStream(directoryPath + @"\" + filename, stream);
                            success = true;
                        }
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_Leave(object sender, EventArgs e)
        {
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ListfileNameAttack != null && this.ListfileNameAttack.Count > 0)
                {
                    positionHandle = -1;
                    if (!dxValidationProvider.Validate())
                        return;
                    bool sucess = false;

                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                    HIS_TREATMENT_FILE updateDTO = new HIS_TREATMENT_FILE();
                    updateDTO.TREATMENT_ID = _TreatmentId;
                    UpdateDTOFromdataForm(ref updateDTO);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    var resultData = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT_FILE>("api/HisTreatmentFile/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        sucess = true;
                        FillDataFormList();
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();

                    MessageManager.Show(this, param, sucess);
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn file ảnh từ máy tính hoặc chụp ảnh từ camera");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromdataForm(ref HIS_TREATMENT_FILE updateDTO)
        {
            try
            {
                if (this.idTreatmentFile != null)
                {
                    updateDTO.ID = this.idTreatmentFile;
                }
                if (cboFileType.EditValue != null)
                {
                    updateDTO.FILE_TYPE_ID = (long)cboFileType.EditValue;
                }
                if (txtDescription.Text != null)
                {
                    updateDTO.DESCRIPTION = txtDescription.Text;
                }
                this.urlTreatmentFile = "";

                foreach (var item in this.ListfileNameAttack)
                {
                   // if (item.IsChecked == true)
                    {
                        if (!string.IsNullOrEmpty(this.urlTreatmentFile))
                        {
                            this.urlTreatmentFile += "|" + item.Url;
                        }
                        else
                        {
                            this.urlTreatmentFile += item.Url;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(urlTreatmentFile))
                {
                    updateDTO.FILE_URLS = this.urlTreatmentFile;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(cboFileType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataFormList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        private void tileView1_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemClick");
                e.Item.Checked = !e.Item.Checked;
                TileView tileView = sender as TileView;
                this.currentDataClick = (AttackADO)tileView.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsChecked = e.Item.Checked;
                foreach (var item in this.ListfileNameAttack)
                {
                    if (item == currentDataClick)
                    {
                        item.IsChecked = currentDataClick.IsChecked;
                    }
                }
                #region FillData
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentDataClick), currentDataClick));
                if (System.IO.Path.GetExtension(currentDataClick.FullName).ToLower() == ".pdf")
                {

                    this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    if (currentDataClick.IsFss)
                    {
                        Stream stream = Inventec.Fss.Client.FileDownload.GetFile(currentDataClick.Url);
                        stream.Position = 0;
                         //string dstFileName1 = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin("." + currentDataClick.Extension);
                         //Inventec.Common.SignLibrary.Utils.ByteToFile(Inventec.Common.SignLibrary.Utils.StreamToByte(stream), dstFileName1);
                         //MemoryStream ms = new MemoryStream();
                         //using (FileStream file = new FileStream(dstFileName1, FileMode.Open, FileAccess.Read))
                         //    file.CopyTo(ms);
                        pdfViewer1.LoadDocument(stream);
                        //stream.Close();
                      
                    }
                    else
                    {
                        pdfViewer1.LoadDocument(currentDataClick.FullName);
                    }

                }
                else
                {
                    this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    if (currentDataClick.IsFss)
                    {
                        if (currentDataClick.Url != null)
                        {
                            var stream = Inventec.Fss.Client.FileDownload.GetFile(currentDataClick.Url);
                            pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromStream(stream);
                            pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                            stream.Close();
                        }
                    }
                    else
                    {
                        if (currentDataClick.image != null)
                        {
                            pteAnhChupFileDinhKem.Image = currentDataClick.image;
                            pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                        }
                    }


                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                var lstData = cardControl.DataSource as List<AttackADO>;
                if (lstData.Where(o => o.IsChecked).ToList() == null)
                {
                    toggleSwitch2.IsOn = false;
                }

                
                #endregion
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pteAnhChupFileDinhKem.Image.Tag), pteAnhChupFileDinhKem.Image.Tag));

                //timerDoubleClick.Start();
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Name == "btnDelete")
                {
                    if (XtraMessageBox.Show("Bạn có muốn xóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var dataItem = (DevExpress.XtraGrid.Views.Tile.TileViewItem)e.DataItem;
                        var item = (AttackADO)tileView1.GetRow(dataItem.RowHandle);
                        //nếu đã lưu thì gọi api xóa và check document
                        if (item.ID > 0)
                        {
                            Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                            HIS_TREATMENT_FILE data = new HIS_TREATMENT_FILE();
                            data.ID = item.ID;
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatmentFile/Delete", ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                            //gọi api xóa thành công thì xóa ở danh sách và xóa document
                            if (apiResult)
                            {
                                this.ListfileNameAttack.Remove(item);
                                if (!string.IsNullOrEmpty(this.urlTreatmentFile) && this.urlTreatmentFile.Contains(item.Url))
                                {
                                    var lstProcessUrlTreatmentFile = this.urlTreatmentFile.Split('|');
                                    lstProcessUrlTreatmentFile = lstProcessUrlTreatmentFile.Where(o => !o.Contains(item.Url)).ToArray();
                                    var processUrlTreatmentFile = string.Join("|", lstProcessUrlTreatmentFile);
                                    this.urlTreatmentFile = processUrlTreatmentFile;
                                }
                                cardControl.DataSource = null;
                                cardControl.DataSource = this.ListfileNameAttack;
                                cardControl.EndUpdate();
                                if (!(this.ListfileNameAttack != null && this.ListfileNameAttack.Count > 0))
                                {
                                    if (toggleSwitch2.IsOn)
                                    {
                                        toggleSwitch2.IsOn = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.ListfileNameAttack.Remove(item);
                            if (!string.IsNullOrEmpty(this.urlTreatmentFile) && this.urlTreatmentFile.Contains(item.Url))
                            {
                                var lstProcessUrlTreatmentFile = this.urlTreatmentFile.Split('|');
                                lstProcessUrlTreatmentFile = lstProcessUrlTreatmentFile.Where(o => !o.Contains(item.Url)).ToArray();
                                var processUrlTreatmentFile = string.Join("|", lstProcessUrlTreatmentFile);
                                this.urlTreatmentFile = processUrlTreatmentFile;
                            }
                            cardControl.DataSource = null;
                            cardControl.DataSource = this.ListfileNameAttack;
                            cardControl.EndUpdate();
                            if (!(this.ListfileNameAttack != null && this.ListfileNameAttack.Count > 0))
                            {
                                if (toggleSwitch2.IsOn)
                                {
                                    toggleSwitch2.IsOn = false;
                                }
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var lstData = cardControl.DataSource as List<AttackADO>;
                if (lstData != null && lstData.Count > 0)
                {
                    lstData = lstData.Where(o => o.IsChecked).ToList();
                    if (lstData != null && lstData.Count > 0)
                    {
                        List<FileHolder> files = new List<FileHolder>();
                        Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();

                        string output = Utils.GenerateTempFileWithin();
                        string streamSourceStr = null;
                        MemoryStream streamSource = null;
                        string dstFileName = null;
                        List<string> lstURL = new List<string>();
                        List<AttackADOTemp> adoTemp = new List<AttackADOTemp>();

                        #region
                        List<AttackADO> lstAdo = new List<AttackADO>();
                        for (int i = 0; i < lstData.Count; i++)
                        {
                            var streamSourceTmp = new MemoryStream();
                            string dstFileName1 = "";
                            if (lstData[i].IsFss)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("1");
                                streamSourceTmp = Inventec.Fss.Client.FileDownload.GetFile(lstData[i].Url);
                                streamSourceTmp.Position = 0;
                                dstFileName1 = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin("." + lstData[i].Extension);
                                Inventec.Common.SignLibrary.Utils.ByteToFile(Inventec.Common.SignLibrary.Utils.StreamToByte(streamSourceTmp), dstFileName1);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dstFileName1______" + Inventec.Common.Logging.LogUtil.GetMemberName(() => dstFileName1), dstFileName1));
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("2");
                                dstFileName1 = lstData[i].FullName;
                            }

                            if (dstFileName1.ToLower().EndsWith(".png") || dstFileName1.ToLower().EndsWith(".jpg") || dstFileName1.ToLower().EndsWith(".jpeg"))
                            {
                                dstFileName = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin();
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dstFileName), dstFileName));
                                ConvertImageToPdf(dstFileName1, @dstFileName);
                            }
                            else
                            {

                                dstFileName = dstFileName1;
                            }
                            lstURL.Add(dstFileName);
                            AttackADOTemp temp = new AttackADOTemp();
                            temp.IsFssTemp = false;
                            temp.Url = dstFileName;
                            adoTemp.Add(temp);
                            if (i == 0)
                            {
                                streamSourceStr = dstFileName;
                            }

                        }
                        #endregion
                        Inventec.Common.Logging.LogSystem.Debug("btnPrint_Click_)__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => adoTemp), adoTemp));

                        InsertPage1(streamSource, streamSourceStr, adoTemp, output);

                        Inventec.Common.Logging.LogSystem.Warn("output: " + output);

                        Inventec.Common.Logging.LogSystem.Info("url: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstURL), lstURL));

                        Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                        DocumentView.Text = "In";

                        DocumentView.ShowDialog();



                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn file để in",
                        "Thông báo");
                        return;
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn file để in",
                    "Thông báo");
                    return;
                }



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ConvertImageToPdf(string srcFilename, string dstFilename)
        {
            iTextSharp.text.Rectangle pageSize = null;
            int imageWidth = 0;
            int imageHeight = 0;
            using (var srcImage = new Bitmap(srcFilename))
            {
                int screenWidth = (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.8);
                int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                imageWidth = srcImage.Width;
                imageHeight = srcImage.Height;

                if (imageWidth > screenWidth)
                {
                    imageHeight = (int)(imageHeight * ((float)screenWidth / (float)imageWidth));
                    imageWidth = screenWidth;
                }

                pageSize = new iTextSharp.text.Rectangle(0, 0, imageWidth, imageHeight);
            }

            var destImage = new Bitmap(imageWidth, imageHeight);

            using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(System.Drawing.Image.FromFile(srcFilename), 0, 0, imageWidth, imageHeight);
            }

            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();
                var image = iTextSharp.text.Image.GetInstance((System.Drawing.Image)destImage, ImageFormat.Jpeg);
                document.Add(image);
                document.Close();

                File.WriteAllBytes(dstFilename, ms.ToArray());
            }
        }

        internal static void InsertPage1(Stream sourceStream, string sourceFile, List<AttackADOTemp> fileListJoin, string desFileJoined)
        {
            List<string> joinStreams = new List<string>();
            if (fileListJoin != null && fileListJoin.Count > 0)
            {
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(fileListJoin[0].Url);
                //if (!String.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
                //{
                //    reader1 = new iTextSharp.text.pdf.PdfReader(sourceFile);
                //}
                //else if (sourceStream != null)
                //{
                //    reader1 = new iTextSharp.text.pdf.PdfReader(sourceStream);
                //}
                int pageCount = reader1.NumberOfPages;
                iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
                iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

                foreach (var item in fileListJoin)
                {
                    int lIndex1 = item.Url.LastIndexOf(".");
                    string EXTENSION = item.Url.ToLower().Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));

                    if (item.IsFssTemp)
                    {
                        #region isFss
                        if (EXTENSION != "pdf")
                        {

                            var stream = Inventec.Fss.Client.FileDownload.GetFile(item.Url);
                            stream.Position = 0;

                            string convertTpPdf = Utils.GenerateTempFileWithin();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu convertTpPdf: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => convertTpPdf), convertTpPdf));
                            Stream streamConvert = new FileStream(convertTpPdf, FileMode.Create, FileAccess.Write);
                            iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                            iTextdocument.Open();
                            writer.Open();

                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(stream);
                            if (img.Height > img.Width)
                            {
                                float percentage = 0.0f;
                                percentage = pageSize.Height / img.Height;
                                img.ScalePercent(percentage * 100);
                            }
                            else
                            {
                                float percentage = 0.0f;
                                percentage = pageSize.Width / img.Width;
                                img.ScalePercent(percentage * 100);
                            }
                            iTextdocument.Add(img);
                            iTextdocument.Close();
                            writer.Close();

                            joinStreams.Add(convertTpPdf);
                        }
                        else
                        {

                            //string joinFileResult = Utils.GenerateTempFileWithin();
                            //var streamSource = FssFileDownload.GetFile(item);
                            //streamSource.Position = 0;
                            //Stream streamConvert = new FileStream(joinFileResult, FileMode.Create, FileAccess.Write);
                            //iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                            //iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                            var stream = Inventec.Fss.Client.FileDownload.GetFile(item.Url);
                            stream.Position = 0;
                            if (stream != null && stream.Length > 0)
                            {

                                string pdfAddFile = Utils.GenerateTempFileWithin();
                                Utils.ByteToFile(Utils.StreamToByte(stream), pdfAddFile);
                                joinStreams.Add(pdfAddFile);
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram____item=" + item);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        joinStreams.Add(item.Url);
                    }
                }

                Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                var pages = new List<int>();
                //for (int i = 0; i <= reader1.NumberOfPages; i++)
                //{
                //    pages.Add(i);
                //}
                //reader1.SelectPages(pages);
                //pdfConcat.AddPages(reader1);
                Inventec.Common.Logging.LogSystem.Error("pages.Count=" + pages.Count);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                foreach (var file in joinStreams)
                {
                    iTextSharp.text.pdf.PdfReader pdfReader = null;
                    pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                    pages = new List<int>();
                    for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                    {
                        pages.Add(i);
                    }
                    pdfReader.SelectPages(pages);
                    pdfConcat.AddPages(pdfReader);
                    pdfReader.Close();
                }

                try
                {
                    reader1.Close();
                }
                catch { }

                try
                {
                    if (sourceStream != null)
                        sourceStream.Close();
                }
                catch { }

                try
                {
                    pdfConcat.Close();
                }
                catch { }

                foreach (var file in joinStreams)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        private string GeneratePdfFileFromImages()
        {
            string output = Path.GetTempFileName();
            try
            {

                using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (Document doc = new Document())
                    {
                        try
                        {
                            PdfWriter.GetInstance(doc, fs);

                            doc.Open();
                            foreach (var item in this.ListfileNameAttack)
                            {
                                if (item.IsChecked == true)
                                {
                                    string extensionc = System.IO.Path.GetExtension(item.FullName);
                                    if (extensionc != ".pdf")
                                    {
                                        iTextSharp.text.Image image;
                                        image = iTextSharp.text.Image.GetInstance(item.image, BaseColor.BLACK);
                                        if (image.Height > image.Width)
                                        {
                                            float percentage = 0.0f;
                                            percentage = doc.PageSize.Height / image.Height;
                                            image.ScalePercent(percentage * 90);
                                        }
                                        else
                                        {
                                            float percentage = 0.0f;
                                            percentage = doc.PageSize.Width / image.Width;
                                            image.ScalePercent(percentage * 90);
                                        }
                                        doc.NewPage();
                                        doc.Add(image);
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return output;
        }

        private void btnGPrint_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HIS_TREATMENT_FILE rowData = (HIS_TREATMENT_FILE)grvFormList.GetFocusedRow();
                List<AttackADOTemp> lstURL = new List<AttackADOTemp>();
                if (!string.IsNullOrEmpty(rowData.FILE_URLS))
                {
                    string[] strList = rowData.FILE_URLS.Split('|');
                    string item = "";
                    string dstFileName = "";
                    string streamSourceStr = null;
                    if (strList != null && strList.Length > 0)
                    {
                        for (int i = 0; i < strList.Length; i++)
                        {
                            var streamSourceTmp = new MemoryStream();
                            streamSourceTmp = Inventec.Fss.Client.FileDownload.GetFile(strList[i]);
                            streamSourceTmp.Position = 0;

                            string dstFileName1 = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin(Path.GetExtension(strList[i]));
                            Inventec.Common.SignLibrary.Utils.ByteToFile(Inventec.Common.SignLibrary.Utils.StreamToByte(streamSourceTmp), dstFileName1);


                            if (dstFileName1.ToLower().EndsWith(".png") || dstFileName1.ToLower().EndsWith(".jpg") || dstFileName1.ToLower().EndsWith(".jpeg"))
                            {
                                dstFileName = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin();
                                ConvertImageToPdf(dstFileName1, @dstFileName);
                            }
                            else
                            {
                                dstFileName = dstFileName1;
                            }

                            AttackADOTemp ado = new AttackADOTemp();
                            ado.IsFssTemp = false;
                            ado.Url = dstFileName;
                            lstURL.Add(ado);
                            if (i == 0)
                            {
                                streamSourceStr = dstFileName;
                            }
                        }

                    }

                    string output = Utils.GenerateTempFileWithin();

                    MemoryStream streamSource = null;
                    Inventec.Common.Logging.LogSystem.Debug("btnGPRINT______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstURL), lstURL));
                    InsertPage1(streamSource, streamSourceStr, lstURL.ToList(), output);
                    Inventec.Common.Logging.LogSystem.Warn("output: " + output);

                    Inventec.Common.Logging.LogSystem.Info("url: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstURL), lstURL));

                    Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                    DocumentView.Text = "In";

                    DocumentView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

     

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnEdit_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnReset_Click(null, null);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            try
            {
                btnAdd.Enabled = true;
                btnEdit.Enabled = false;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider, dxErrorProvider1);
                SetDefaultValue();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSwitch2_Toggled(object sender, EventArgs e)
        {
            try
            {
                bool check = false;
                if (toggleSwitch2.IsOn)
                {
                    check = true;
                    layoutControlItem8.Text = "Bỏ chọn tất cả";
                }
                else
                {
                    check = false;
                    layoutControlItem8.Text = "Chọn tất cả";
                }
                var lstData = cardControl.DataSource as List<AttackADO>;
                if (lstData != null && lstData.Count > 0)
                {
                    foreach (var item in lstData)
                    {
                        item.IsChecked = check;
                    }

                    cardControl.DataSource = null;
                    cardControl.DataSource = lstData;

                }

            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

       
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                 if (e.SelectedControl is DevExpress.XtraGrid.GridControl)
                {
                    DevExpress.Utils.ToolTipControlInfo info = null;
                    var hi = tileView1.CalcHitInfo(e.ControlMousePosition);

                    if (hi != null )
                    {
                        var o = hi.HitPoint;
                        
                            string textx = "";
                            var datax = (AttackADO)tileView1.GetRow(hi.RowHandle);
                            if (datax != null)
                            {                               
                                    textx = datax.FILE_NAME;                               
                            }
                            info = new DevExpress.Utils.ToolTipControlInfo(o, textx);
                            e.Info = info;
                        
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
