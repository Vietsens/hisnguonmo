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
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.EmrSignDocumentList
{
    public partial class FormChooseSignType : Form
    {
        private V_EMR_DOCUMENT row;
        private V_HIS_ROOM currentRoom = null;

        private Inventec.Desktop.Common.Modules.Module ModuleData;

        public FormChooseSignType()
        {
            InitializeComponent();
        }

        public FormChooseSignType(V_EMR_DOCUMENT row, Inventec.Desktop.Common.Modules.Module _moduleData, V_HIS_ROOM room)
            : this()
        {
            this.row = row;
            this.ModuleData = _moduleData;
            this.currentRoom = room;
        }

        private void FormChooseSignType_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnUsbToken_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataForSign(SignType.USB);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnServer_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataForSign(SignType.HMS);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void LoadDataForSign(SignType signType)
        {
            try
            {
                if (row != null)
                {
                    EMR.Filter.EmrVersionFilter versionFilter = new Filter.EmrVersionFilter();
                    versionFilter.DOCUMENT_ID = row.ID;

                    var listVersion = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR_VERSION>>(EMR.URI.EmrVersion.GET, ApiConsumers.EmrConsumer, versionFilter, SessionManager.ActionLostToken, null);
                    if (listVersion != null && listVersion.Count > 0)
                    {
                        EMR_VERSION version = new EMR_VERSION();
                        version = listVersion.OrderByDescending(o => o.ID).FirstOrDefault();
                        if (version != null && !String.IsNullOrWhiteSpace(version.URL))
                        {
                            //goi tool view
                            using (MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(version.URL))
                            {
                                if (stream != null)
                                {
                                    var temFile = System.IO.Path.Combine(Application.StartupPath + "\\temp\\");
                                    if (!Directory.Exists(temFile)) Directory.CreateDirectory(temFile);

                                    temFile = System.IO.Path.Combine(temFile, string.Format("{0}.pdf", Guid.NewGuid()));
                                    using (var fileStream = new FileStream(temFile, FileMode.Create, FileAccess.Write))
                                    {
                                        stream.CopyTo(fileStream);
                                    }
                                    ShowPopupSign(signType, temFile);
                                    if (File.Exists(temFile)) File.Delete(temFile);
                                }
                                else
                                {
                                    XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                                }
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongTimDuocVanBanKy);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowPopupSign(SignType signType, string file)
        {
            try
            {
                if (row != null)
                {
                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADO(row.TREATMENT_CODE, row.DOCUMENT_CODE, row.DOCUMENT_NAME, currentRoom.ID);

                    inputADO.IsReject = true;
                    //inputADO.RoomCode = currentRoom.ROOM_CODE;
                    //inputADO.RoomName = currentRoom.ROOM_NAME;
                    //inputADO.RoomTypeCode = currentRoom.ROOM_TYPE_CODE;

                    if (row.WIDTH != null && row.HEIGHT != null && row.RAW_KIND != null)
                    {
                        inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(row.PAPER_NAME, (int)row.WIDTH, (int)row.HEIGHT);
                        if (row.RAW_KIND != null)
                        {
                            inputADO.PaperSizeDefault.RawKind = (int)row.RAW_KIND;
                        }
                    }
                    libraryProcessor.ShowPopup(file, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSignConfig(EMR.TDO.DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;
                        _listObj.Add(apiResult.Max(o => o.PAPER_NAME));
                        _listObj.Add(apiResult.Max(o => o.RAW_KIND));
                        _listObj.Add(apiResult.Max(o => o.WIDTH));
                        _listObj.Add(apiResult.Max(o => o.HEIGHT));
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", ModuleData.RoomId, ModuleData.RoomTypeId, _listObj);
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
