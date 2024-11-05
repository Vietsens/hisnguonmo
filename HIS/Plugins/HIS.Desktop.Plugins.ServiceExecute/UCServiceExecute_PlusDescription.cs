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
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.QRCoder;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        private DocumentRange[] RangeAllService;
        private string ViewPacsUrlFormat = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ViewPacsUrlFormat);
        private string ViewPacsSecretKey = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ViewPacsSecretKey);

        private void ProcessChoiceSereServTempl(MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP data)
        {
            try
            {
                RangeAllService = null;
                if (data != null)
                {
                    WaitingManager.Show();

                    if (this.currentSereServExt == null || this.currentSereServExt.CONCLUDE == null || this.currentSereServExt.NOTE == null)
                    {
                        if (!String.IsNullOrWhiteSpace(data.CONCLUDE) && this.currentSereServExt.CONCLUDE == null)
                        {
                            txtConclude.Text = data.CONCLUDE;
                        }

                        if (!String.IsNullOrWhiteSpace(data.NOTE) && this.currentSereServExt.NOTE == null)
                        {
                            txtNote.Text = data.NOTE;
                        }
                    }


                    //đã có dữ liệu thì load luôn. Ngược lại gọi api cập nhật lại
                    if (data.DESCRIPTION != null && data.DESCRIPTION.Length > 0)
                    {
                        ProcessSetRtfText(Utility.TextLibHelper.BytesToStringConverted(data.DESCRIPTION));
                    }
                    else
                    {

                        HIS_SERE_SERV_TEMP DescriptionData = SereServTempProcess.GetDescription(data);
                        //không có hoặc khác thời gian sửa
                        if (DescriptionData == null || DescriptionData.MODIFY_TIME != data.MODIFY_TIME)
                        {
                            CommonParam param = new CommonParam();
                            HisSereServTempFilter filter = new HisSereServTempFilter();
                            filter.ID = data.ID;
                            var apiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEMP>>("api/HisSereServTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                            if (apiResult != null && apiResult.Count > 0)
                            {
                                DescriptionData = apiResult.FirstOrDefault();
                            }
                        }

                        try
                        {
                            if (DescriptionData.ID > 0)
                            {
                                ProcessSetRtfText(Utility.TextLibHelper.BytesToStringConverted(DescriptionData.DESCRIPTION));
                                var updateData = this.listTemplate.FirstOrDefault(o => o.ID == DescriptionData.ID);
                                if (updateData != null)
                                {
                                    updateData.DESCRIPTION = DescriptionData.DESCRIPTION;
                                }
                            }

                            SereServTempProcess.SetDescription(DescriptionData);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    ProcessDescriptionContent();

                    positionProtect = "";
                    ProcessWordProtected(true);

                    ProcessSelectDataForDescription("");

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ProcessSelectDataForDescription(string positionDescriptionData)
        {
            try
            {
                if (this.panelDescription != null && this.panelDescription.Controls != null)
                {
                    foreach (var item in this.panelDescription.Controls)
                    {
                        if (item is UcWord)
                        {
                            UcWord control = (UcWord)item;
                            control.CreateRange(positionDescriptionData);
                            break;
                        }
                        else if (item is UcWordFull)
                        {
                            UcWordFull control = (UcWordFull)item;
                            control.CreateRange(positionDescriptionData);
                            break;
                        }
                        else if (item is UcWords.UcTelerik)
                        {
                            UcWords.UcTelerik control = (UcWords.UcTelerik)item;
                            control.CreateRange(positionDescriptionData);
                            break;
                        }
                        else if (item is UcWords.UcTelerikFullWord)
                        {
                            var control = (UcWords.UcTelerikFullWord)item;
                            control.CreateRange(positionDescriptionData);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetRangeAndData(ref string rangeDescription, ref string dataDescription)
        {
            try
            {
                if (this.panelDescription != null && this.panelDescription.Controls != null)
                {
                    foreach (var item in this.panelDescription.Controls)
                    {
                        if (item is UcWord)
                        {
                            UcWord control = (UcWord)item;
                            rangeDescription = control.GetRange();
                            dataDescription = control.GetDataRange();
                            break;
                        }
                        else if (item is UcWordFull)
                        {
                            UcWordFull control = (UcWordFull)item;
                            rangeDescription = control.GetRange();
                            dataDescription = control.GetDataRange();
                            break;
                        }
                        else if (item is UcWords.UcTelerik)
                        {
                            UcWords.UcTelerik control = (UcWords.UcTelerik)item;
                            rangeDescription = control.GetRange();
                            dataDescription = control.GetDataRange();
                            break;
                        }
                        else if (item is UcWords.UcTelerikFullWord)
                        {
                            var control = (UcWords.UcTelerikFullWord)item;
                            rangeDescription = control.GetRange();
                            dataDescription = control.GetDataRange();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessSereServExt__DescriptionPrint(CommonParam param, HIS_SERE_SERV sereServ)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServExt), sereServExt));
                ProcessWordProtected(true);

                //Khi luu se lay anh duoc chon load vao noi dung word editor
                //noi dung nay luu vao sarprint & co truong json_description_id trong sereserv
                //data.DESCRIPTION_SAR_PRINT_ID = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                //Xu ly txtDescription -> replace key data + image
                if (this.ActionType == GlobalVariables.ActionEdit && currentSarPrint != null && currentSarPrint.ID > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.1");
                    currentSarPrint.CONTENT = Encoding.UTF8.GetBytes(ProcessGetRtfTextFromUc());
                    if (cboSereServTemp.EditValue != null)
                    {
                        var temp = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? "").ToString()));
                        if (temp != null)
                        {
                            currentSarPrint.EMR_BUSINESS_CODES = temp.EMR_BUSINESS_CODES;
                            currentSarPrint.EMR_DOCUMENT_TYPE_CODE = temp.EMR_DOCUMENT_TYPE_CODE;
                            currentSarPrint.EMR_DOCUMENT_GROUP_CODE = temp.EMR_DOCUMENT_GROUP_CODE;
                            currentSarPrint.ADDITIONAL_INFO = string.Format("SERE_SERV_TEMP_CODE:{0}", temp.SERE_SERV_TEMP_CODE);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.1.1");
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, currentSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (currentSarPrint != null) result = false;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.2");
                    this.currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    SAR.EFMODEL.DataModels.SAR_PRINT dataSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    dataSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.3");
                    dataSarPrint.CONTENT = Encoding.UTF8.GetBytes(ProcessGetRtfTextFromUc());
                    if (cboSereServTemp.EditValue != null)
                    {
                        var temp = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? "").ToString()));
                        if (temp != null)
                        {
                            dataSarPrint.EMR_BUSINESS_CODES = temp.EMR_BUSINESS_CODES;
                            dataSarPrint.EMR_DOCUMENT_TYPE_CODE = temp.EMR_DOCUMENT_TYPE_CODE;
                            dataSarPrint.EMR_DOCUMENT_GROUP_CODE = temp.EMR_DOCUMENT_GROUP_CODE;
                            dataSarPrint.ADDITIONAL_INFO = string.Format("SERE_SERV_TEMP_CODE:{0}", temp.SERE_SERV_TEMP_CODE);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.4");
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_CREATE, ApiConsumer.ApiConsumers.SarConsumer, dataSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (this.currentSarPrint != null)
                    {
                        sereServExt.DESCRIPTION_SAR_PRINT_ID = this.currentSarPrint.ID + "";
                        result = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Goi api tao du lieu SAR_PRINT that bai. currentSarPrint is null.");
                        result = true;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.5");
                }

                this.sereServExt.DOC_PROTECTED_LOCATION = positionProtect;
                Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private bool ProcessSereServExt__DescriptionPrint(CommonParam param, ADO.ServiceADO sereServ, HIS_SERE_SERV_EXT sereServExt)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServExt), sereServExt));
                ProcessWordProtected(true);

                //Khi luu se lay anh duoc chon load vao noi dung word editor
                //noi dung nay luu vao sarprint & co truong json_description_id trong sereserv
                //data.DESCRIPTION_SAR_PRINT_ID = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                //Xu ly txtDescription -> replace key data + image
                if (!String.IsNullOrEmpty(sereServExt.DESCRIPTION_SAR_PRINT_ID))
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.1");
                    if (currentSarPrint == null)
                    {
                        currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                        currentSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                        currentSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.1.1");
                    currentSarPrint.ID = Convert.ToInt64(sereServExt.DESCRIPTION_SAR_PRINT_ID);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.1.2");
                    currentSarPrint.CONTENT = Encoding.UTF8.GetBytes(ProcessGetRtfTextFromUc());
                    if (cboSereServTemp.EditValue != null)
                    {
                        var temp = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? "").ToString()));
                        if (temp != null)
                        {
                            currentSarPrint.EMR_BUSINESS_CODES = temp.EMR_BUSINESS_CODES;
                            currentSarPrint.EMR_DOCUMENT_TYPE_CODE = temp.EMR_DOCUMENT_TYPE_CODE;
                            currentSarPrint.EMR_DOCUMENT_GROUP_CODE = temp.EMR_DOCUMENT_GROUP_CODE;
                            currentSarPrint.ADDITIONAL_INFO = string.Format("SERE_SERV_TEMP_CODE:{0}", temp.SERE_SERV_TEMP_CODE);
                        }
                    }

                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, currentSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (currentSarPrint != null) result = false;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.2");
                    this.currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    SAR.EFMODEL.DataModels.SAR_PRINT dataSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    dataSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.2.1");
                    dataSarPrint.CONTENT = Encoding.UTF8.GetBytes(ProcessGetRtfTextFromUc());
                    if (cboSereServTemp.EditValue != null)
                    {
                        var temp = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? "").ToString()));
                        if (temp != null)
                        {
                            dataSarPrint.EMR_BUSINESS_CODES = temp.EMR_BUSINESS_CODES;
                            dataSarPrint.EMR_DOCUMENT_TYPE_CODE = temp.EMR_DOCUMENT_TYPE_CODE;
                            dataSarPrint.EMR_DOCUMENT_GROUP_CODE = temp.EMR_DOCUMENT_GROUP_CODE;
                            dataSarPrint.ADDITIONAL_INFO = string.Format("SERE_SERV_TEMP_CODE:{0}", temp.SERE_SERV_TEMP_CODE);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.2.2");
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_CREATE, ApiConsumer.ApiConsumers.SarConsumer, dataSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.2.3");
                    if (this.currentSarPrint != null)
                    {
                        sereServExt.DESCRIPTION_SAR_PRINT_ID = this.currentSarPrint.ID + "";
                        result = false;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Goi api tao du lieu SAR_PRINT that bai. currentSarPrint is null.");
                        result = true;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.2.4");
                }

                sereServExt.DOC_PROTECTED_LOCATION = positionProtect;
                Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private void ProcessDescriptionContent()
        {
            DevExpress.XtraRichEdit.API.Native.Document doc = GettxtDescription().Document;
            try
            {
                if (!String.IsNullOrEmpty(doc.Text))
                {
                    ProcessDicParam();
                    if (RangeAllService == null)
                        RangeAllService = doc.FindAll(string.Format("<#{0};>", ServiceExecuteCFG.SingleKeyAllInOne), SearchOptions.CaseSensitive);

                    if (this.dicParam != null && this.dicParam.Count > 0)
                    {
                        Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag singleTag = new Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag();

                        doc.BeginUpdate();

                        Inventec.Common.RichEditor.ProcessTag.Store store = new Inventec.Common.RichEditor.ProcessTag.Store(doc);
                        Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag processSingleTag = new Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag();
                        Inventec.Common.RichEditor.ProcessTag.ProcessImageTag processImageTag = new Inventec.Common.RichEditor.ProcessTag.ProcessImageTag();

                        processSingleTag.ProcessData(store, this.dicParam);

                        if (this.dicImage != null && this.dicImage.Count > 0)
                            processImageTag.ProcessData(store, this.dicImage);

                        doc.EndUpdate();
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("txtDescription.Text is empty");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                doc.EndUpdate();
            }

            ProcessSetRtfText(doc.RtfText);
        }

        private void InsertRow(ADO.ServiceADO sereServ)
        {
            Document document = GettxtDescription().Document;
            try
            {
                CheckAllInOne.ReadOnly = true;//đã tiến hành xử lý thì không cho đổi nữa

                //nếu trùng với biến toàn cục thì là cập nhật dữ liệu
                if (sereServ.ID == this.sereServ.ID)
                {
                    sereServ.note = txtNote.Text.Trim();
                    sereServ.conclude = txtConclude.Text.Trim();

                    if (!String.IsNullOrEmpty(txtNumberOfFilm.Text))
                        sereServ.NUMBER_OF_FILM = long.Parse(txtNumberOfFilm.Text);
                    else
                        sereServ.NUMBER_OF_FILM = null;
                }
                else
                {
                    txtConclude.Text = null;
                    txtNote.Text = null;
                    txtNumberOfFilm.Text = null;
                }
                ProcessUpdateConclude(sereServ);
                bool inserted = true;
                var isAdd = listServiceADOForAllInOne.Where(o => o.ID == sereServ.ID).ToList();
                if (isAdd == null || isAdd.Count <= 0)
                {
                    inserted = false;
                    listServiceADOForAllInOne.Add(sereServ);
                }

                //cập nhật danh sách
                foreach (var item in listServiceADOForAllInOne)
                {
                    if (item.ID == sereServ.ID)
                    {
                        item.conclude = sereServ.conclude;
                        item.note = sereServ.note;
                        item.MACHINE_ID = sereServ.MACHINE_ID;
                        item.NUMBER_OF_FILM = sereServ.NUMBER_OF_FILM;
                        break;
                    }
                }

                this.sereServ = sereServ;

                document.BeginUpdate();
                if (RangeAllService != null && RangeAllService.Count() > 0)
                {
                    string serviceName = string.Join("; ", listServiceADOForAllInOne.Select(s => s.TDL_SERVICE_NAME).ToList());
                    SubDocument doc = RangeAllService[0].BeginUpdateDocument();

                    string plainText = doc.GetText(RangeAllService[0]);
                    if (!String.IsNullOrWhiteSpace(plainText))
                    {
                        document.ReplaceAll(plainText, serviceName, SearchOptions.CaseSensitive);
                        doc.ReplaceAll(plainText, serviceName, SearchOptions.CaseSensitive);
                    }
                    else
                    {
                        foreach (var item in RangeAllService)
                        {
                            var pos = document.CreatePosition(item.Start.ToInt());
                            document.InsertText(pos, serviceName);
                            doc.Replace(item, serviceName);
                        }
                    }

                    RangeAllService[0].EndUpdateDocument(doc);
                    RangeAllService = doc.FindAll(serviceName, SearchOptions.CaseSensitive);
                }
                else
                {
                    Table table = null;
                    if (document.Tables != null && document.Tables.Count > 0)
                    {
                        foreach (var item in document.Tables)
                        {
                            if (item.FirstRow.Cells.Count > 2)
                            {
                                table = item;
                                break;
                            }
                        }
                    }
                    if (table == null) return;
                    int index = 1;
                    //lấy ra vị trí của dịch vụ đang xử lý
                    if (!inserted)
                    {
                        var row = table.Rows.InsertAfter(table.Rows.Last.Index);
                        index = row.Index;
                        foreach (var item in listServiceADOForAllInOne)
                        {
                            if (item.ID == sereServ.ID)
                            {
                                item.tableIndex = row.Index;
                                break;
                            }
                        }
                    }
                    else if (mainSereServ.ID == sereServ.ID && (isAdd == null || isAdd.Count <= 0))
                    {
                        var mainSs = listServiceADOForAllInOne.FirstOrDefault(o => o.ID == mainSereServ.ID);
                        int lastIndex = listServiceADOForAllInOne.Count > 0 ? listServiceADOForAllInOne.Min(o => o.tableIndex) - 1 : table.Rows.Last.Index;
                        mainSs.tableIndex = lastIndex > 0 ? lastIndex : table.Rows.Last.Index;
                    }
                    else
                    {
                        index = sereServ.tableIndex;
                    }
                    // Insert Cells Values and format the columns
                    // Xóa dữ liệu cũ và chèn lại
                    document.Delete(table[index, 1].Range);
                    document.InsertText(table[index, 1].Range.Start, sereServ.TDL_SERVICE_NAME);
                    document.Delete(table[index, 2].Range);
                    document.InsertText(table[index, 2].Range.Start, txtConclude.Text);
                }
                document.EndUpdate();
            }
            catch (Exception ex)
            {
                document.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            ProcessSetRtfText(document.RtfText);
        }

        private void ProcessUpdateConclude(ADO.ServiceADO sereServ)
        {
            try
            {
                //lọc theo đúng service để lấy ra kết luận gắn với mẫu tương ứng
                var listtemplate = listTemplate.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID).ToList();
                if (listtemplate != null && listtemplate.Count > 0)
                {
                    var temp = listtemplate.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                    if (String.IsNullOrWhiteSpace(txtConclude.Text))
                    {
                        txtConclude.Text = temp.CONCLUDE;
                        txtNote.Text = temp.NOTE;
                    }
                }

                if (!String.IsNullOrEmpty(sereServ.conclude) || !String.IsNullOrEmpty(sereServ.note))
                {
                    txtConclude.Text = sereServ.conclude;
                    txtNote.Text = sereServ.note;
                }

                if (sereServ.NUMBER_OF_FILM.HasValue)
                {
                    txtNumberOfFilm.Text = sereServ.NUMBER_OF_FILM.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParam()
        {
            try
            {
                // chế biến dữ liệu thành các key đơn thêm vào biểu mẫu tương tự như mps excel
                this.dicParam = new Dictionary<string, object>();
                this.dicImage = new Dictionary<string, Image>();

                CommonParam param = new CommonParam();
                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(this.dicParam);//commonkey
                if (currentServiceReq != null)
                {
                    dicParam.Add("INTRUCTION_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReq.INTRUCTION_TIME) ?? DateTime.Now));

                    dicParam.Add("INTRUCTION_DATE_FULL_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        currentServiceReq.INTRUCTION_TIME));

                    dicParam.Add("INTRUCTION_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.INTRUCTION_TIME));

                    dicParam.Add("START_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.START_TIME ?? 0));

                    dicParam.Add("START_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReq.START_TIME ?? 0) ?? DateTime.Now));

                    dicParam.Add("ICD_MAIN_TEXT", currentServiceReq.ICD_NAME);

                    dicParam.Add("NATIONAL_NAME", currentServiceReq.TDL_PATIENT_NATIONAL_NAME);
                    dicParam.Add("WORK_PLACE", currentServiceReq.TDL_PATIENT_WORK_PLACE_NAME);
                    dicParam.Add("ADDRESS", currentServiceReq.TDL_PATIENT_ADDRESS);
                    dicParam.Add("CAREER_NAME", currentServiceReq.TDL_PATIENT_CAREER_NAME);
                    dicParam.Add("PATIENT_CODE", currentServiceReq.TDL_PATIENT_CODE);
                    dicParam.Add("DISTRICT_CODE", currentServiceReq.TDL_PATIENT_DISTRICT_CODE);
                    dicParam.Add("GENDER_NAME", currentServiceReq.TDL_PATIENT_GENDER_NAME);
                    dicParam.Add("MILITARY_RANK_NAME", currentServiceReq.TDL_PATIENT_MILITARY_RANK_NAME);
                    dicParam.Add("VIR_ADDRESS", currentServiceReq.TDL_PATIENT_ADDRESS);
                    dicParam.Add("AGE", CalculatorAge(currentServiceReq.TDL_PATIENT_DOB, false));
                    dicParam.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(currentServiceReq.TDL_PATIENT_DOB, "", "", "", "", TreatmentWithPatientTypeAlter.IN_TIME));
                    dicParam.Add("STR_YEAR", currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    dicParam.Add("VIR_PATIENT_NAME", currentServiceReq.TDL_PATIENT_NAME);

                    dicParam.Add("SAMPLE_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.SAMPLE_TIME ?? 0));
                    dicParam.Add("RECEIVE_SAMPLE_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.RECEIVE_SAMPLE_TIME ?? 0));
                    var paanPosition = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAAN_POSITION>().FirstOrDefault(o => o.ID == currentServiceReq.PAAN_POSITION_ID);
                    if (paanPosition != null)
                        dicParam.Add("PAAN_POSITION_NAME", paanPosition.PAAN_POSITION_NAME);
                    else
                        dicParam.Add("PAAN_POSITION_NAME", "");

                    var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        dicParam.Add("EXECUTE_DEPARTMENT_CODE", executeRoom.DEPARTMENT_CODE);
                        dicParam.Add("EXECUTE_DEPARTMENT_NAME", executeRoom.DEPARTMENT_NAME);
                        dicParam.Add("EXECUTE_ROOM_CODE", executeRoom.ROOM_CODE);
                        dicParam.Add("EXECUTE_ROOM_NAME", executeRoom.ROOM_NAME);
                    }

                    var reqRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        dicParam.Add("REQUEST_DEPARTMENT_CODE", reqRoom.DEPARTMENT_CODE);
                        dicParam.Add("REQUEST_DEPARTMENT_NAME", reqRoom.DEPARTMENT_NAME);
                        dicParam.Add("REQUEST_ROOM_CODE", reqRoom.ROOM_CODE);
                        dicParam.Add("REQUEST_ROOM_NAME", reqRoom.ROOM_NAME);
                    }

                    if (lstBedLogData != null && lstBedLogData.Count > 0)
                    {
                        var bedLogData = lstBedLogData.Where(o => o.TREATMENT_ID == currentServiceReq.TREATMENT_ID).ToList();
                        if (bedLogData != null && bedLogData.Count > 0)
                        {
                            bedLogData = bedLogData.Where(o => o.START_TIME < currentServiceReq.INTRUCTION_TIME && (o.FINISH_TIME > currentServiceReq.INTRUCTION_TIME || o.FINISH_TIME == null)).OrderByDescending(o => o.FINISH_TIME).ToList();
                            if (bedLogData != null && bedLogData.Count > 0)
                            {
                                dicParam.Add("BED_NAME", bedLogData.First().BED_NAME);
                                dicParam.Add("BED_CODE", bedLogData.First().BED_CODE);
                            }
                        }
                    }
                }

                if (TreatmentWithPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE",
                            HeinCardHelper.SetHeinCardNumberDisplayByNumber(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER));
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME));
                        dicParam.Add("STR_HEIN_CARD_TO_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME));
                        dicParam.Add("HEIN_CARD_ADDRESS", TreatmentWithPatientTypeAlter.HEIN_CARD_ADDRESS);
                    }
                    else
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                        dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                        dicParam.Add("HEIN_CARD_ADDRESS", "");
                    }

                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE);
                    if (patientType != null)
                        dicParam.Add("PATIENT_TYPE_NAME", patientType.PATIENT_TYPE_NAME);
                    else
                        dicParam.Add("PATIENT_TYPE_NAME", "");

                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == TreatmentWithPatientTypeAlter.TREATMENT_TYPE_CODE);
                    if (treatmentType != null)
                        dicParam.Add("TREATMENT_TYPE_NAME", treatmentType.TREATMENT_TYPE_NAME);
                    else
                        dicParam.Add("TREATMENT_TYPE_NAME", "");

                    dicParam.Add("TREATMENT_ICD_CODE", TreatmentWithPatientTypeAlter.ICD_CODE);
                    dicParam.Add("TREATMENT_ICD_NAME", TreatmentWithPatientTypeAlter.ICD_NAME);
                    dicParam.Add("TREATMENT_ICD_SUB_CODE", TreatmentWithPatientTypeAlter.ICD_SUB_CODE);
                    dicParam.Add("TREATMENT_ICD_TEXT", TreatmentWithPatientTypeAlter.ICD_TEXT);

                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(TreatmentWithPatientTypeAlter, this.dicParam, false);

                    int AGE_NUM = Inventec.Common.DateTime.Calculation.Age(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB, TreatmentWithPatientTypeAlter.IN_TIME);
                    dicParam.Add("AGE_NUM", AGE_NUM);
                }
                else
                {
                    dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                    dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                    dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                    dicParam.Add("HEIN_CARD_ADDRESS", "");
                    var typeAlter = new HisTreatmentWithPatientTypeInfoSDO();
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(typeAlter, this.dicParam, false);
                }

                if (patient != null)
                    AddKeyIntoDictionaryPrint<ADO.PatientADO>(patient, this.dicParam, false);

                AddKeyIntoDictionaryPrint<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(this.currentServiceReq, this.dicParam, true);
                AddKeyIntoDictionaryPrint<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(this.sereServ, this.dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExt, this.dicParam, true);

                if (lstDhstData != null && lstDhstData.Count > 0)
                {
                    var dhstData = lstDhstData.Where(o => o.TREATMENT_ID == currentServiceReq.TREATMENT_ID).ToList();
                    if (dhstData != null && dhstData.Count > 0)
                    {
                        dhstData = dhstData.Where(o => o.EXECUTE_TIME < currentServiceReq.INTRUCTION_TIME).OrderByDescending(o => o.EXECUTE_TIME).ToList();
                        if (dhstData != null && dhstData.Count > 0)
                        {
                            AddKeyIntoDictionaryPrint<MOS.EFMODEL.DataModels.HIS_DHST>(dhstData.First(), this.dicParam, false);
                        }
                    }
                }
                dicParam[ServiceExecuteCFG.SingleKeyAllInOne] = "";
                if (CheckAllInOne.Checked && this.sereServ != null)
                {
                    dicParam[ServiceExecuteCFG.SingleKeyAllInOne] = this.sereServ.TDL_SERVICE_NAME;
                }

                if (this.sereServExt != null)
                {
                    //không thêm key để khi sửa vẫn giữ lại key trên template.
                    //khi in sẽ xử lý để hiển thị
                    //if (!dicParam.ContainsKey("END_TIME_FULL_STR"))
                    //    dicParam.Add("END_TIME_FULL_STR", MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                    //            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.END_TIME ?? 0) ?? DateTime.Now));
                    //else
                    //    dicParam["END_TIME_FULL_STR"] = MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                    //            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.END_TIME ?? 0) ?? DateTime.Now);
                    //if (!dicParam.ContainsKey("BEGIN_TIME_FULL_STR"))
                    //    dicParam.Add("BEGIN_TIME_FULL_STR", MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                    //            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.BEGIN_TIME ?? 0) ?? DateTime.Now));
                    //else
                    //    dicParam["BEGIN_TIME_FULL_STR"] = MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                    //            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.BEGIN_TIME ?? 0) ?? DateTime.Now);

                    if (this.sereServExt.MACHINE_ID.HasValue)
                    {
                        var machine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>().FirstOrDefault(o => o.ID == this.sereServExt.MACHINE_ID.Value);
                        if (machine != null)
                        {
                            dicParam["MACHINE_NAME"] = machine.MACHINE_NAME;
                        }
                    }

                    if (sereServExt.END_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExt.END_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServExt.END_TIME.Value);
                    }
                    else if (sereServExt.MODIFY_TIME.HasValue)
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(sereServExt.MODIFY_TIME.Value);
                        dicParam["EXECUTE_TIME_FULL_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(sereServExt.MODIFY_TIME.Value);
                    }
                    else
                    {
                        dicParam["EXECUTE_DATE_FULL_STR"] = "";
                        dicParam["EXECUTE_TIME_FULL_STR"] = "";
                    }
                }
                else
                {
                    dicParam["EXECUTE_DATE_FULL_STR"] = "";
                    dicParam["EXECUTE_TIME_FULL_STR"] = "";
                }

                dicParam.Add("USER_NAME", UserName);
                if (!isPressButtonSave)
                {
                    dicParam.Remove("CONCLUDE");
                    dicParam.Remove("NOTE");
                }

                dicParam.Add("CONCLUDE_NEW", "<#CONCLUDE;>");
                dicParam.Add("NOTE_NEW", "<#NOTE;>");

                List<ADO.ImageADO> image = listImage != null ? listImage.Where(o => o.IsChecked == true).OrderBy(p => p.STTImage).ToList() : null;
                dicImage = new Dictionary<string, Image>();
                if (image != null && image.Count > 0)
                {
                    for (int i = 0; i < image.Count; i++)
                    {
                        //fix cứng size ảnh thành 250x140 để add vào kết quả
                        dicImage.Add("IMAGE_DATA_" + (i + 1), ResizeImage(image[i].IMAGE_DISPLAY, 250, 140));
                        dicParam.Add("IMAGE_CAPTION_" + (i + 1), image[i].CAPTION);
                        //image[i].IsChecked = false;
                    }

                    if (image.Count < 10)
                    {
                        for (int i = image.Count; i < 10; i++)
                        {
                            dicImage.Add("IMAGE_DATA_" + (i + 1), null);
                            dicParam.Add("IMAGE_CAPTION_" + (i + 1), "");
                        }
                    }
                }
                else if (isPressButtonSave)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        dicImage.Add("IMAGE_DATA_" + (i + 1), null);
                        dicParam.Add("IMAGE_CAPTION_" + (i + 1), "");
                    }
                }

                if (!String.IsNullOrWhiteSpace(this.ViewPacsUrlFormat))
                {
                    string content = this.ViewPacsUrlFormat;
                    if (patient != null)
                    {
                        content = content.Replace("<MA_BN>", patient.PATIENT_CODE);
                    }
                    else
                    {
                        content = content.Replace("<MA_BN>", this.currentServiceReq.TDL_PATIENT_CODE);
                    }

                    content = content.Replace("<ACCESSNUMBER>", this.sereServ.ID.ToString());
                    content = content.Replace("<ENCODE_ACCESSNUMBER>", Encrypt(this.sereServ.ID.ToString()));
                    QRCodeGenerator qrGeneratorViewResult = new QRCodeGenerator();
                    QRCodeData qrCodeDataViewResult = qrGeneratorViewResult.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCodeViewResult = new BitmapByteQRCode(qrCodeDataViewResult);
                    byte[] qrCodeImageViewResult = qrCodeViewResult.GetGraphic(20);
                    using (MemoryStream ms = new MemoryStream(qrCodeImageViewResult, 0, qrCodeImageViewResult.Length))
                    {
                        try
                        {
                            ms.Write(qrCodeImageViewResult, 0, qrCodeImageViewResult.Length);
                            Image qrCode = Image.FromStream(ms, true);
                            dicImage.Add("SERE_SERV_QRCODE", ResizeImage(qrCode, 250, 250));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                //bỏ key để phục vụ đổ dữ liệu khi in
                foreach (var item in dicParam)
                {
                    if (item.Key.EndsWith("_PRINT"))
                    {
                        dicParam.Remove(item.Key);
                    }
                }

                if (this.dicSereServPttt != null && this.dicSereServPttt.Count > 0 && this.dicSereServPttt.ContainsKey(this.sereServ.ID) & this.dicSereServPttt[this.sereServ.ID] != null)
                {
                    dicParam.Add("ICD_CM_CODE", this.dicSereServPttt[this.sereServ.ID].ICD_CM_CODE);
                    dicParam.Add("ICD_CM_NAME", this.dicSereServPttt[this.sereServ.ID].ICD_CM_NAME);
                    dicParam.Add("ICD_CM_SUB_CODE", this.dicSereServPttt[this.sereServ.ID].ICD_CM_SUB_CODE);
                    dicParam.Add("ICD_CM_TEXT", this.dicSereServPttt[this.sereServ.ID].ICD_CM_TEXT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataForTemplate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin ProcessDataForTemplate ");
                patient = GetPatientById(this.ServiceReqConstruct.TDL_PATIENT_ID);
                Inventec.Common.Logging.LogSystem.Info("1. End ProcessDataForTemplate ");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public string Encrypt(string TextToEncrypt)
        {
            string str = null;
            try
            {
                byte[] MyEncryptedArray = UTF8Encoding.UTF8.GetBytes(TextToEncrypt);

                byte[] ivBytes = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();

                byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(ViewPacsSecretKey));

                MyMD5CryptoService.Clear();

                var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider();

                MyTripleDESCryptoService.Key = MysecurityKeyArray;
                MyTripleDESCryptoService.Mode = CipherMode.CBC;
                MyTripleDESCryptoService.IV = ivBytes;
                MyTripleDESCryptoService.Padding = PaddingMode.PKCS7;

                var MyCrytpoTransform = MyTripleDESCryptoService
                   .CreateEncryptor();

                byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock(MyEncryptedArray, 0, MyEncryptedArray.Length);

                MyTripleDESCryptoService.Clear();

                str = Convert.ToBase64String(MyresultArray, 0,
                   MyresultArray.Length);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return str;
        }

        private void UncheckImage()
        {
            try
            {
                if (listImage != null && listImage.Count > 0)
                {
                    List<ADO.ImageADO> image = listImage.Where(o => o.IsChecked == true).ToList();
                    if (image != null && image.Count > 0)
                    {
                        foreach (var item in image)
                        {
                            item.IsChecked = false;
                            item.STTImage = null;
                        }
                        cardControl.RefreshDataSource();
                        lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessGetRtfTextFromUc()
        {
            string result = "";
            try
            {
                if (this.panelDescription != null && this.panelDescription.Controls != null)
                {
                    foreach (var item in this.panelDescription.Controls)
                    {
                        if (item is UcWord)
                        {
                            var control = (UcWord)item;
                            result = control.txtDescription.RtfText;
                            break;
                        }
                        else if (item is UcWordFull)
                        {
                            var control = (UcWordFull)item;
                            result = control.txtDescription.RtfText;
                            break;
                        }
                        else if (item is UcWords.UcTelerik)
                        {
                            var control = (UcWords.UcTelerik)item;
                            result = control.GetRtfText();
                            break;
                        }
                        else if (item is UcWords.UcTelerikFullWord)
                        {
                            var control = (UcWords.UcTelerikFullWord)item;
                            result = control.GetRtfText();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessSetRtfText(string rtfText)
        {
            try
            {
                if (wordFullDocument != null)
                {
                    wordFullDocument.txtDescription.RtfText = rtfText;
                }

                if (wordDocument != null)
                {
                    wordDocument.txtDescription.RtfText = rtfText;
                }

                if (UcTelerikDocument != null)
                {
                    UcTelerikDocument.SetRtfText(rtfText);
                }

                if (UcTelerikFullDocument != null)
                {
                    UcTelerikFullDocument.SetRtfText(rtfText);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessWordProtected(bool isProtected)
        {
            try
            {
                WordProtectedProcess protectedProcess = new WordProtectedProcess();
                if (this.panelDescription != null && this.panelDescription.Controls != null)
                {
                    foreach (var item in this.panelDescription.Controls)
                    {
                        if (item is UcWord)
                        {
                            var control = (UcWord)item;
                            if (isProtected)
                            {
                                protectedProcess.InitialProtected(control.txtDescription, ref positionProtect);
                            }
                            else
                            {
                                control.txtDescription.Document.Unprotect();
                            }

                            break;
                        }
                        else if (item is UcWordFull)
                        {
                            var control = (UcWordFull)item;
                            if (isProtected)
                            {
                                protectedProcess.InitialProtected(control.txtDescription, ref positionProtect);
                            }
                            else
                            {
                                control.txtDescription.Document.Unprotect();
                            }

                            break;
                        }
                        else if (item is UcWords.UcTelerik)
                        {
                            var control = (UcWords.UcTelerik)item;
                            if (isProtected)
                            {
                                WordProcess.InitialProtected(control.radRichTextEditor1, ref positionProtect);
                            }
                            else
                            {
                                control.radRichTextEditor1.DeleteReadOnlyRange();
                            }

                            break;
                        }
                        else if (item is UcWords.UcTelerikFullWord)
                        {
                            var control = (UcWords.UcTelerikFullWord)item;
                            if (isProtected)
                            {
                                WordProcess.InitialProtected(control.radRichTextEditor1, ref positionProtect);
                            }
                            else
                            {
                                control.radRichTextEditor1.DeleteReadOnlyRange();
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessFocusWord()
        {
            try
            {
                if (this.panelDescription != null && this.panelDescription.Controls != null)
                {
                    foreach (Control item in this.panelDescription.Controls)
                    {
                        if (item.Visible)
                        {
                            item.Focus();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSetWordZoom(long trackBarZoom)
        {
            try
            {
                //chỉ gọi khi Init nên sẽ chỉ có 1 control được gán.
                if (wordFullDocument != null)
                {
                    this.wordFullDocument.txtDescription.ActiveView.ZoomFactor = (float)(trackBarZoom / 100);
                }
                else if (wordDocument != null)
                {
                    this.wordDocument.txtDescription.ActiveView.ZoomFactor = (float)(trackBarZoom / 100);
                }
                else if (UcTelerikDocument != null)
                {
                    UcTelerikDocument.radRichTextEditor1.ScaleFactor = new Telerik.WinForms.Documents.Model.SizeF((float)(trackBarZoom / 100), (float)(trackBarZoom / 100));
                }
                else if (UcTelerikFullDocument != null)
                {
                    UcTelerikFullDocument.radRichTextEditor1.ScaleFactor = new Telerik.WinForms.Documents.Model.SizeF((float)(trackBarZoom / 100), (float)(trackBarZoom / 100));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChangeImage(Control edit, ADO.ImageADO imageADO)
        {
            try
            {
                if (edit == null)
                {
                    return;
                }

                if (edit is DevExpress.XtraRichEdit.RichEditControl)
                {
                    ProcessChangeImageDev((DevExpress.XtraRichEdit.RichEditControl)edit, imageADO);
                }
                else if (edit is DevExpress.XtraRichEdit.RichEditControl)
                {
                    ProcessChangeImageTel((Telerik.WinControls.UI.RadRichTextEditor)edit, imageADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region library
        /// <summary>
        /// gán dữ liệu vào diction để fill data vào word
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dicParamPlus"></param>
        /// <param name="autoOveride"> ghi đè dữ liệu</param>
        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus, bool autoOveride)
        {
            try
            {
                if (data != null)
                {
                    PropertyInfo[] pis = typeof(T).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;

                            var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                            if (String.IsNullOrEmpty(searchKey.Key))
                            {
                                dicParamPlus.Add(pi.Name, pi.GetValue(data));
                            }
                            else
                            {
                                if (autoOveride)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                                else if (dicParamPlus[pi.Name] == null)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
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

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage((Image)destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, width, height);
            }

            return destImage;
        }

        /// <summary>
        /// Hiển thị định dạng 23:59 Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentTimeSeparateBeginTime(System.DateTime now)
        {
            string result = "";
            try
            {
                if (now != DateTime.MinValue)
                {
                    string month = string.Format("{0:00}", now.Month);
                    string day = string.Format("{0:00}", now.Day);
                    string hour = string.Format("{0:00}", now.Hour);
                    string hours = string.Format("{0:00}", now.Hour);
                    string minute = string.Format("{0:00}", now.Minute);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, now.Day, now.Month, now.Year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private string CalculatorAge(long ageYearNumber, bool isHl7)
        {
            string tuoi = "";
            try
            {
                string caption__Tuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__AGE");
                string caption__ThangTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__MONTH_OLDS");
                string caption__NgayTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__DAY_OLDS");
                string caption__GioTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__HOURS_OLDS");

                if (isHl7)
                {
                    caption__Tuoi = "T";
                    caption__ThangTuoi = "TH";
                    caption__NgayTuoi = "NT";
                    caption__GioTuoi = "GT";
                }

                if (ageYearNumber > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        tuoi = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    tuoi = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    tuoi = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                tuoi = "";
            }
            return tuoi;
        }
        #endregion
    }
}
