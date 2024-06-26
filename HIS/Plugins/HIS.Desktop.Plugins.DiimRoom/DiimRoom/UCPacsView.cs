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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using PacsOne.AppControl.AppControl;
using System.IO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.DiimRoom.ADO;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;

namespace HIS.Desktop.Plugins.DiimRoom.DiimRoom
{
    public partial class UCPacsView : UserControlBase
    {
        #region Reclare variable
        RefeshReference refeshReference;
        private string ConnectPacsByFss = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ConnectPacsByFss");
        int rowCount = 0;
        int startPage = 0;
        int dataTotal = 0;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        V_HIS_SERE_SERV_6 sereServ6Current;
        const int SERVICE_CODE__MAX_LENGTH = 6;
        const int SERVICE_REQ_CODE__MAX_LENGTH = 9;
        List<ImagesADO> images;
        #endregion

        #region Construct
        public UCPacsView(Inventec.Desktop.Common.Modules.Module module, V_HIS_SERE_SERV_6 sereServ6)
            : this(module, sereServ6, null)
        {
        }

        public UCPacsView(Inventec.Desktop.Common.Modules.Module module, V_HIS_SERE_SERV_6 sereServ6, RefeshReference _refeshReference)
            : base(module)
        {
            InitializeComponent();
            this.refeshReference = _refeshReference;
            this.sereServ6Current = sereServ6;
        }
        #endregion

        #region private Method
        private void UCRiimRoom_Load(object sender, EventArgs e)
        {
            try
            {
                if (ConnectPacsByFss != "1")
                {
                    PacsCFG.DIC_SERVER_PACS = null;
                    var pi = PacsCFG.DIC_SERVER_PACS;
                }
                SetCaptionByLanguageKey();
                ViewExecuteService();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DiimRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.DiimRoom.DiimRoom.UCPacsView).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRiimRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSeriesDate.Caption = Inventec.Common.Resource.Get.Value("UCRiimRoom.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCRiimRoom.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImageFromPacsService(string soPhieu)
        {
            try
            {
                CommonParam param = new CommonParam();
                ImageRequestADO layThongTinAnhInputADO = new ImageRequestADO();
                layThongTinAnhInputADO.SoPhieu = soPhieu;
                var resultData = new Inventec.Common.Adapter.BackendAdapter(param).PostWithouApiParam<ImageResponseADO>(RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH, PacsApiConsumer.PacsConsumer, layThongTinAnhInputADO, null, param);
                if (resultData != null && resultData.TrangThai && resultData.Series != null)
                {
                    images = new List<ImagesADO>();
                    foreach (var item in resultData.Series)
                    {
                        item.Images.ForEach(o => o.SeriesDateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(item.SeriesDateTime)));
                        images.AddRange(item.Images);
                    }
                    images = images.OrderBy(o => o.SeriesDateTime).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Control

        private void ViewExecuteService()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (sereServ6Current != null)
                {
                    gridControlPacsImage.DataSource = null;
                    string sophieu = String.Format("{0}-{1}", ReduceForCode(sereServ6Current.TDL_SERVICE_REQ_CODE, SERVICE_REQ_CODE__MAX_LENGTH), ReduceForCode(sereServ6Current.TDL_SERVICE_CODE, SERVICE_CODE__MAX_LENGTH));
                    LoadImageFromPacsService(sophieu);
                    gridControlPacsImage.DataSource = this.images;
                }

                pciViewDicomImg.Image = null;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPacsImage_Click(object sender, EventArgs e)
        {
            try
            {
                var rowImagesADO = (ImagesADO)gridViewPacsImage.GetFocusedRow();
                if (rowImagesADO != null)
                {
                    WaitingManager.Show();
                    if (ConnectPacsByFss == "1")
                    {
                        string[] fss = new string[] { "FSS\\" };
                        string direct = rowImagesADO.ImageDirectory.Split(fss, StringSplitOptions.None).LastOrDefault();
                        string fullDirect = direct + "\\" + rowImagesADO.ImageThumbFileName;

                        Stream jpg = Inventec.Fss.Client.FileDownload.GetFile(fullDirect, ConfigSystems.URI_API_FSS_FOR_PACS);
                        pciViewDicomImg.Image = Image.FromStream(jpg);
                    }
                    else
                    {
                        var listIp = rowImagesADO.ImageDirectory.Split('\\');
                        if (listIp != null && listIp.Count() > 1 &&
                            (PacsCFG.DIC_SERVER_PACS.ContainsKey(listIp[0]) || PacsCFG.DIC_SERVER_PACS.ContainsKey(listIp[1])))
                        {
                            pciViewDicomImg.Image = Image.FromFile(String.Format("{0}\\{1}", rowImagesADO.ImageDirectory, rowImagesADO.ImageThumbFileName));
                        }
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPacsImage_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound && ((BaseView)sender).DataSource != null)
                {
                    ImagesADO data = (ImagesADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "SeriesDateTimeDisPlay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(data.SeriesDateTime));
                        }
                        else if (e.Column.FieldName == "ReceivedDateDisplay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(data.ReceivedDate));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPacsImage_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = (this.gridViewPacsImage.GetGroupRowValue(e.RowHandle, this.gridColumnSeriesDate) ?? "").ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        private string ReduceForCode(string orderCode, int maxlength)
        {
            if (!string.IsNullOrWhiteSpace(orderCode) && orderCode.Length >= maxlength)
            {
                return orderCode.Substring(orderCode.Length - maxlength);
            }
            return orderCode;
        }

        #endregion

        #region Button

        //private void UpdateStt(long sampleSTT)
        //{
        //    try
        //    {
        //        //WaitingManager.Show();
        //        //bool result = false;
        //        //CommonParam param = new CommonParam();
        //        //LIS.SDO.UpdateSampleSttSDO updateStt = new LIS.SDO.UpdateSampleSttSDO();
        //        //var row = (LIS.EFMODEL.DataModels.LIS_SAMPLE)gridViewExecute.GetFocusedRow();
        //        //if (row != null)
        //        //{
        //        //    updateStt.Id = row.ID;
        //        //    updateStt.SampleSttId = sampleSTT;

        //        //    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/UpdateStt", ApiConsumer.ApiConsumers.LisConsumer, updateStt, param);
        //        //    if (curentSTT != null)
        //        //    {
        //        //        FillDataToGridControl();
        //        //        result = true;
        //        //        FillDataToGridControl();
        //        //        WaitingManager.Hide();
        //        //    }
        //        //}
        //        //WaitingManager.Hide();
        //        //#region Show message
        //        //MessageManager.Show(this.ParentForm, param, result);
        //        //#endregion

        //        //#region Process has exception
        //        //SessionManager.ProcessTokenLost(param);
        //        //#endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}


        //private void TraKetQuaE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        //WaitingManager.Show();
        //        //bool result = false;
        //        //CommonParam param = new CommonParam();
        //        //var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
        //        //row.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT;
        //        //if (row != null)
        //        //{
        //        //    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/ReturnResult", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
        //        //    if (curentSTT != null)
        //        //    {
        //        //        WaitingManager.Hide();
        //        //        FillDataToGridControl();
        //        //        result = true;
        //        //        //gridViewExecute.RefreshData();
        //        //    }
        //        //}
        //        //WaitingManager.Hide();
        //        //#region Show message
        //        //MessageManager.Show(this.ParentForm, param, result);
        //        //#endregion

        //        //#region Process has exception
        //        //SessionManager.ProcessTokenLost(param);
        //        //#endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        WaitingManager.Hide();
        //    }
        //}

        //private void HuyDuyetE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        //WaitingManager.Show();
        //        //bool result = false;
        //        //CommonParam param = new CommonParam();
        //        //var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
        //        //row.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__UNSPECIMEN;
        //        //if (row != null)
        //        //{
        //        //    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, row, param);
        //        //    if (curentSTT != null)
        //        //    {
        //        //        WaitingManager.Hide();
        //        //        FillDataToGridControl();
        //        //        result = true;
        //        //        gridViewExecute.RefreshData();
        //        //    }
        //        //}
        //        //WaitingManager.Hide();
        //        //#region Show message
        //        //MessageManager.Show(this.ParentForm, param, result);
        //        //#endregion

        //        //#region Process has exception
        //        //SessionManager.ProcessTokenLost(param);
        //        //#endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        WaitingManager.Hide();
        //    }
        //}

        //private void DuyetE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        //WaitingManager.Show();
        //        //bool result = false;
        //        //CommonParam param = new CommonParam();
        //        //var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
        //        //row.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__SPECIMEN;
        //        //if (row != null)
        //        //{
        //        //    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, row, param);
        //        //    if (curentSTT != null)
        //        //    {
        //        //        WaitingManager.Hide();
        //        //        FillDataToGridControl();
        //        //        result = true;
        //        //        gridViewExecute.RefreshData();
        //        //    }
        //        //}
        //        //WaitingManager.Hide();
        //        //#region Show message
        //        //MessageManager.Show(this.ParentForm, param, result);
        //        //#endregion

        //        //#region Process has exception
        //        //SessionManager.ProcessTokenLost(param);
        //        //#endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        WaitingManager.Hide();
        //    }
        //}

        //private void HuyKetQuaE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        //WaitingManager.Show();
        //        //bool result = false;
        //        //CommonParam param = new CommonParam();
        //        //var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
        //        //if (row != null)
        //        //{
        //        //    result = new BackendAdapter(param).Post<bool>("api/LisSample/Delete", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
        //        //    if (result == true)
        //        //    {
        //        //        gridControlExecute.BeginUpdate();
        //        //        gridViewExecute.DeleteRow(gridViewExecute.FocusedRowHandle);
        //        //        gridViewExecute.RefreshData();
        //        //        gridControlExecute.RefreshDataSource();
        //        //        gridControlExecute.EndUpdate();
        //        //        WaitingManager.Hide();
        //        //    }
        //        //}
        //        //WaitingManager.Hide();
        //        //#region Show message
        //        //MessageManager.Show(this.ParentForm, param, result);
        //        //#endregion

        //        //#region Process has exception
        //        //SessionManager.ProcessTokenLost(param);
        //        //#endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void HuyTraE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        //UpdateStt(StatusSampleCFG.SAMPLE_STT_ID__RESULT);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        #endregion

    }
}
