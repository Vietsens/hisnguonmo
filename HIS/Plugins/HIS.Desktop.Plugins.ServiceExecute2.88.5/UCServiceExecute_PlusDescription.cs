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
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        private void ProcessChoiceSereServTempl(MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    WaitingManager.Show();
                    txtConclude.Text = data.CONCLUDE;
                    txtNote.Text = data.NOTE;

                    GettxtDescription().RtfText = Utility.TextLibHelper.BytesToStringConverted(data.DESCRIPTION);

                    //WordProcess.zoomFactor(GettxtDescription());

                    ProcessDescriptionContent();

                    //this.positionFinded = 0;
                    WordProtectedProcess protectedProcess = new WordProtectedProcess();
                    //protectedProcess.InitialProtected(txtDescription, ref positionFinded);
                    positionProtect = "";
                    protectedProcess.InitialProtected(GettxtDescription(), ref positionProtect);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ProcessSereServFileExecute(List<FileHolder> listFileHolder)
        {
            try
            {
                if (listImage == null || listImage.Count <= 0) return;
                List<ADO.ImageADO> imageItems = listImage.Where(o => o.IsChecked == true).ToList();
                if (imageItems != null && imageItems.Count > 0)
                {
                    for (int i = 0; i < imageItems.Count; i++)
                    {
                        FileHolder file = new FileHolder();
                        file.FileName = imageItems[i].FileName;
                        file.Content = new System.IO.MemoryStream();
                        imageItems[i].IMAGE_DISPLAY.Save(file.Content, System.Drawing.Imaging.ImageFormat.Png);
                        file.Content.Position = 0;
                        listFileHolder.Add(file);
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
                WordProtectedProcess protectedProcess = new WordProtectedProcess();
                //protectedProcess.InitialProtected(txtDescription, ref positionFinded);
                protectedProcess.InitialProtected(GettxtDescription(), ref positionProtect);

                //Khi luu se lay anh duoc chon load vao noi dung word editor
                //noi dung nay luu vao sarprint & co truong json_description_id trong sereserv
                //data.DESCRIPTION_SAR_PRINT_ID = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                //Xu ly txtDescription -> replace key data + image
                if (this.ActionType == GlobalVariables.ActionEdit && currentSarPrint != null && currentSarPrint.ID > 0)
                {
                    currentSarPrint.CONTENT = Encoding.UTF8.GetBytes(GettxtDescription().RtfText);
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, currentSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (currentSarPrint != null) result = false;
                }
                else
                {
                    this.currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    SAR.EFMODEL.DataModels.SAR_PRINT dataSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    dataSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.CONTENT = Encoding.UTF8.GetBytes(GettxtDescription().RtfText);
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
                }

                //this.sereServExt.DOC_PROTECTED_LOCATION = (positionFinded > 0 ? positionFinded + "" : "");
                this.sereServExt.DOC_PROTECTED_LOCATION = positionProtect;
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
                WordProtectedProcess protectedProcess = new WordProtectedProcess();
                //protectedProcess.InitialProtected(txtDescription, ref positionFinded);
                protectedProcess.InitialProtected(GettxtDescription(), ref positionProtect);

                //Khi luu se lay anh duoc chon load vao noi dung word editor
                //noi dung nay luu vao sarprint & co truong json_description_id trong sereserv
                //data.DESCRIPTION_SAR_PRINT_ID = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                //Xu ly txtDescription -> replace key data + image
                if (sereServ.isSave && !String.IsNullOrEmpty(sereServExt.DESCRIPTION_SAR_PRINT_ID))
                {
                    if (currentSarPrint == null)
                    {
                        currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                        currentSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                        currentSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    }
                    currentSarPrint.ID = Convert.ToInt64(sereServExt.DESCRIPTION_SAR_PRINT_ID);
                    currentSarPrint.CONTENT = Encoding.UTF8.GetBytes(GettxtDescription().RtfText);
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, currentSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (currentSarPrint != null) result = false;
                }
                else
                {
                    this.currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    SAR.EFMODEL.DataModels.SAR_PRINT dataSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    dataSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.CONTENT = Encoding.UTF8.GetBytes(GettxtDescription().RtfText);
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
                }

                //sereServExt.DOC_PROTECTED_LOCATION = (positionFinded > 0 ? positionFinded + "" : "");
                this.sereServExt.DOC_PROTECTED_LOCATION = positionProtect;
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
                if (!String.IsNullOrEmpty(GettxtDescription().Text))
                {
                    ProcessDicParam();

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
                document.BeginUpdate();
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
                document.EndUpdate();
            }
            catch (Exception ex)
            {
                document.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

                txtNumberOfFilm.Text = sereServ.NUMBER_OF_FILM.ToString();
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
                    dicParam.Add("STR_YEAR", currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    dicParam.Add("VIR_PATIENT_NAME", currentServiceReq.TDL_PATIENT_NAME);

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

                if (this.sereServExt != null)
                {
                    if (!dicParam.ContainsKey("END_TIME_FULL_STR"))
                        dicParam.Add("END_TIME_FULL_STR", MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                                Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.END_TIME ?? 0) ?? DateTime.Now));
                    else
                        dicParam["END_TIME_FULL_STR"] = MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                                Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.END_TIME ?? 0) ?? DateTime.Now);
                    if (!dicParam.ContainsKey("BEGIN_TIME_FULL_STR"))
                        dicParam.Add("BEGIN_TIME_FULL_STR", MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                                Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.BEGIN_TIME ?? 0) ?? DateTime.Now));
                    else
                        dicParam["BEGIN_TIME_FULL_STR"] = MPS.ProcessorBase.GlobalQuery.GetCurrentTimeSeparateNoSecond(
                                Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.sereServExt.BEGIN_TIME ?? 0) ?? DateTime.Now);
                }

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
                        //image[i].IsChecked = false;
                    }

                    if (image.Count < 10)
                    {
                        for (int i = image.Count; i < 10; i++)
                        {
                            dicImage.Add("IMAGE_DATA_" + (i + 1), null);
                        }
                    }
                }
                else if (isPressButtonSave)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        dicImage.Add("IMAGE_DATA_" + (i + 1), null);
                    }
                }

                //bỏ key để phục vụ đổ dữ liệu khi in
                foreach (var item in keyPrint)
                {
                    dicParam.Remove(item);
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
                        }
                        cardControl.RefreshDataSource();
                    }
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
