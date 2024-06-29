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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using HIS.Desktop.Utility;
//using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;

using HIS.Desktop.Plugins.PatientDocumentIssued;

using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.Message;

using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.PatientDocumentIssued.Form
{
    public partial class frmViewEmrDocument : FormBase
    {
        V_EMR_DOCUMENT documentData;
        bool IsSigning = false;
        private Inventec.Desktop.Common.Modules.Module moduleData;

        public frmViewEmrDocument(V_EMR_DOCUMENT _documentData, Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            this.documentData = _documentData;
            InitializeComponent();
        }

        private void frmViewEmrDocument_Load(object sender, EventArgs e)
        {
            LoadPdfViewer(this.documentData);
        }

        private void LoadPdfViewer(V_EMR_DOCUMENT data)
        {
            try
            {
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADO(data.TREATMENT_CODE, data.DOCUMENT_CODE, data.DOCUMENT_NAME, 0);


                if (this.IsSigning && data.NEXT_SIGNER == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                {
                    inputADO.IsSign = true;
                }
                else
                    inputADO.IsSign = false;

                inputADO.IsSave = false;
                inputADO.IsPrint = false;
                inputADO.IsExport = false;
                inputADO.IsShowPatientSign = true;
                //inputADO.RoomCode = room != null ? room.ROOM_CODE : "";
                //inputADO.RoomTypeCode = room != null ? room.ROOM_TYPE_CODE : "";
                //inputADO.RoomName = room != null ? room.ROOM_NAME : "";
                inputADO.ActPrintSuccess = UpdateIsPatientIsssued;

                if (data.WIDTH != null && data.HEIGHT != null && data.RAW_KIND != null)
                {
                    inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(data.PAPER_NAME, (int)data.WIDTH, (int)data.HEIGHT);
                    if (data.RAW_KIND != null)
                    {
                        inputADO.PaperSizeDefault.RawKind = (int)data.RAW_KIND;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.PaperSizeDefault), inputADO.PaperSizeDefault));

                CommonParam paramCommon = new CommonParam();
                EmrDocumentDownloadFileSDO filter = new EmrDocumentDownloadFileSDO();
                filter.EmrDocumentViewFilter = new EmrDocumentViewFilter();
                filter.EmrDocumentViewFilter.ID = data.ID;
                filter.HisCode = this.documentData.HIS_CODE;
                filter.IsMerge = true;
                filter.IsShowPatientSign = true;
                filter.IsShowWatermark = false;
                if (moduleData != null && moduleData.RoomId > 0)
                {
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == moduleData.RoomId);
                    if (room != null)
                    {
                        filter.RoomCode = room.ROOM_CODE;
                        filter.DepartmentCode = room.DEPARTMENT_CODE;
                    }
                }
                filter.IsView = null;

                List<EmrDocumentFileSDO> apiResult = new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, filter, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    string output = Utils.GenerateTempFileWithin();
                    List<string> joinStreams = new List<string>();

                    List<MemoryStream> documentData = new List<MemoryStream>();
                    foreach (var item in apiResult)
                    {
                        if (item.Extension.ToLower().Equals("pdf"))
                        {
                            string pdfAddFile = Utils.GenerateTempFileWithin();
                            Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                            joinStreams.Add(pdfAddFile);
                        }
                    }

                    Stream currentStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                    var pages = new List<int>();
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

                    var uc = libraryProcessor.GetUC(Utils.FileToBase64String(output), FileType.Pdf, inputADO);
                    if (uc != null)
                    {
                        uc.Dock = DockStyle.Fill;
                        this.panel1.Controls.Clear();
                        this.panel1.Controls.Add(uc);

                        string message = "Xem văn bản. Mã văn bản: " + data.DOCUMENT_CODE + ", TREATMENT_CODE: " + data.TREATMENT_CODE + ". Thời gian xem: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + ". Người xem: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                    }
                    else
                    {
                        this.panel1.Controls.Clear();
                    }
                }
                else
                {
                    this.panel1.Controls.Clear();
                    #region Hien thi message thong bao
                    MessageManager.Show(this, paramCommon, false);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                this.panel1.Controls.Clear();
                this.panel1 = new Panel();
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateIsPatientIsssued()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LogSystem.Debug(documentData.ID.ToString());
                long idEmrDpcument = documentData.ID;
                var apiResult = new BackendAdapter(param).Post<EMR_DOCUMENT>(RequestUriStore.EMR_DOCUMENT_ISSUED_UPDATE, ApiConsumers.EmrConsumer, idEmrDpcument, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
