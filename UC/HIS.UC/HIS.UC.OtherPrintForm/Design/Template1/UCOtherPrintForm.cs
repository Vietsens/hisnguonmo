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
using HIS.UC.OtherPrintForm;
using EXE.DAL.Base;
using EXE.LOGIC.Base;
using EXE.LOGIC.HisTreatment;
using MOS.EFMODEL.DataModels;
using HIS.UC.OtherPrintForm.Data;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.UC.OtherPrintForm.Design.Template1
{
    public partial class UCOtherPrintForm : UserControl
    {
        public InitData data { get; set; }
        List<SAR.EFMODEL.DataModels.SAR_PRINT> prints { get; set; }
        Dictionary<string, object> dicParam;
        Dictionary<string, System.Drawing.Image> dicImage;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        public UCOtherPrintForm()
        {
            InitializeComponent();
        }

        public UCOtherPrintForm(InitData Data)
        {
            InitializeComponent();
            this.data = Data;
        }

        private void grdControlSarPrint_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                prints = new List<SAR.EFMODEL.DataModels.SAR_PRINT>();
                SAR.Filter.SarPrintFilter printFilter = new SAR.Filter.SarPrintFilter();
                if (data.Treatment != null)
                {
                    var printIds = PrintIdByJsonPrint(data.Treatment.JSON_PRINT_ID);
                    if (printIds == null || printIds.Count == 0)
                    {
                        printFilter.ID = -1;
                    }
                    else
                        printFilter.IDs = printIds;

                    var printTreatments = new EXE.LOGIC.Sar.SarPrint.SarPrintLogic().Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(printFilter);
                    prints.AddRange(printTreatments);
                }

                if (data.SereServ != null)
                {
                    var printIds = PrintIdByJsonPrint(data.SereServ.JSON_PRINT_ID);
                    if (printIds == null || printIds.Count == 0)
                    {
                        printFilter.ID = -1;
                    }
                    else
                        printFilter.IDs = printIds;

                    var printSereServs = new EXE.LOGIC.Sar.SarPrint.SarPrintLogic().Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(printFilter);
                    prints.AddRange(printSereServs);
                }

                if (data.ServiceReq != null)
                {
                    var printIds = PrintIdByJsonPrint(data.ServiceReq.JSON_PRINT_ID);
                    if (printIds == null || printIds.Count == 0)
                    {
                        printFilter.ID = -1;
                    }
                    else
                        printFilter.IDs = printIds;

                    var printServiceReqs = new EXE.LOGIC.Sar.SarPrint.SarPrintLogic().Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(printFilter);
                    prints.AddRange(printServiceReqs);
                }

                grdControlSarPrint.DataSource = prints;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_Edit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var sarPrint = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, EXE.DAL.Base.UriBaseStore.URI_API_SAR, EXE.LOGIC.Base.LanguageManager.GetLanguage(), EXE.LOGIC.LocalStore.GlobalStore.TemnplatePathFolder);
                richEditorMain.RunPrintEditor(sarPrint, updateDataSuccess);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool updateDataSuccess(SAR.EFMODEL.DataModels.SAR_PRINT sarPrint)
        {
            FillDataToGridControl();
            return true;
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (EXE.LOGIC.Base.MsgBox.Show(MessageUtil.GetMessage(EXE.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), EXE.LOGIC.Base.MsgBox.CaptionEnum.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var sarDelete = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                    DeleteJsonPrint(sarDelete);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSarPrint_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SAR.EFMODEL.DataModels.SAR_PRINT data = (SAR.EFMODEL.DataModels.SAR_PRINT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(EXE.DAL.Base.ApiConsumerStore.SarConsumer, EXE.DAL.Base.UriBaseStore.URI_API_SAR, EXE.LOGIC.Base.LanguageManager.GetLanguage(), EXE.LOGIC.LocalStore.GlobalStore.TemnplatePathFolder);
                if (richEditorMain == null) throw new ArgumentNullException("richEditorMain is null");

                var print = (SAR.EFMODEL.DataModels.SAR_PRINT)gridViewSarPrint.GetFocusedRow();
                if (print != null)
                {
                    List<long> currentPrintIds = new List<long>();
                    currentPrintIds.Add(print.ID);
                    richEditorMain.RunPrint(currentPrintIds, dicParam, dicImage, null, ShowPrintPreview);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowPrintPreview(byte[] CONTENT_B)
        {
            try
            {
                richEditorMain.ShowPrintPreview(CONTENT_B, null, dicParam, dicImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
