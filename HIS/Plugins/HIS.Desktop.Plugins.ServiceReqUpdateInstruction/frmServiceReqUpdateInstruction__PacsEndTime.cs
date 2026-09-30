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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction
{
    /// <summary>
    /// O "Thoi gian thuc hien xong": gio chup xong do PACS gui ve (OBR.8), luu theo tung dich vu
    /// tai HIS_SERE_SERV_EXT.PACS_END_TIME. Chi doc, khong tham gia luu / validate cua form.
    /// Y lenh co nhieu dich vu co du lieu thi o hien moc muon nhat va co nut so xuong liet ke tung dich vu.
    /// Control duoc tao luc chay va chen duoi o "Thoi gian ket thuc" (cot phai) de khong phai sua Designer.
    /// </summary>
    public partial class frmServiceReqUpdateInstruction
    {
        private PopupContainerEdit cboPacsEndTime;
        private PopupContainerControl popupPacsEndTime;
        private GridControl gridPacsEndTime;
        private GridView gridViewPacsEndTime;
        private LayoutControlItem lciPacsEndTime;
        private string pacsEndTimeDisplay = "";
        private List<PacsEndTimeADO> pacsEndTimes = new List<PacsEndTimeADO>();

        private class PacsEndTimeADO
        {
            public string TDL_SERVICE_NAME { get; set; }
            public long PACS_END_TIME { get; set; }
            public string PACS_END_TIME_STR { get; set; }
        }

        private void InitPacsEndTimeControl()
        {
            try
            {
                if (cboPacsEndTime != null) return;

                gridViewPacsEndTime = new GridView();
                gridViewPacsEndTime.OptionsBehavior.Editable = false;
                gridViewPacsEndTime.OptionsBehavior.ReadOnly = true;
                gridViewPacsEndTime.OptionsView.ShowGroupPanel = false;
                gridViewPacsEndTime.OptionsView.ShowIndicator = false;
                gridViewPacsEndTime.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewPacsEndTime.OptionsSelection.EnableAppearanceFocusedRow = false;
                GridColumn colService = gridViewPacsEndTime.Columns.AddVisible("TDL_SERVICE_NAME", "Dịch vụ");
                colService.Width = 300;
                GridColumn colTime = gridViewPacsEndTime.Columns.AddVisible("PACS_END_TIME_STR", "Thời gian thực hiện xong");
                colTime.Width = 150;

                gridPacsEndTime = new GridControl();
                gridPacsEndTime.Dock = System.Windows.Forms.DockStyle.Fill;
                gridPacsEndTime.ViewCollection.Add(gridViewPacsEndTime);
                gridPacsEndTime.MainView = gridViewPacsEndTime;
                gridViewPacsEndTime.GridControl = gridPacsEndTime;

                popupPacsEndTime = new PopupContainerControl();
                popupPacsEndTime.Size = new System.Drawing.Size(470, 160);
                popupPacsEndTime.Controls.Add(gridPacsEndTime);
                this.Controls.Add(popupPacsEndTime);

                cboPacsEndTime = new PopupContainerEdit();
                cboPacsEndTime.Name = "cboPacsEndTime";
                cboPacsEndTime.Properties.PopupControl = popupPacsEndTime;
                cboPacsEndTime.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                cboPacsEndTime.Properties.CloseOnOuterMouseClick = true;
                cboPacsEndTime.Properties.ShowPopupCloseButton = false;
                cboPacsEndTime.Properties.PopupSizeable = false;
                cboPacsEndTime.Properties.QueryPopUp += cboPacsEndTime_QueryPopUp;
                cboPacsEndTime.Properties.QueryResultValue += cboPacsEndTime_QueryResultValue;
                cboPacsEndTime.Properties.QueryDisplayText += cboPacsEndTime_QueryDisplayText;

                layoutControl1.BeginUpdate();
                try
                {
                    layoutControl1.Controls.Add(cboPacsEndTime);
                    lciPacsEndTime = layoutControlGroup1.AddItem("Thời gian thực hiện xong:", cboPacsEndTime);
                    lciPacsEndTime.Name = "lciPacsEndTime";
                    lciPacsEndTime.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciPacsEndTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    //nhan dai hon cac nhan cung cot -> de layout tu tinh do rong theo chu (AutoSize), khong do tay de khoi lech theo font/DPI
                    lciPacsEndTime.TextAlignMode = TextAlignModeItem.AutoSize;
                    lciPacsEndTime.TextToControlDistance = lciEndTime.TextToControlDistance;
                    //dat o cot phai, ngay duoi "Thoi gian ket thuc" (cot phai nhan rong 160 nen gan thang hang)
                    lciPacsEndTime.Move(lciEndTime, InsertType.Bottom);

                    //can hai cot: o moi lam cot phai dai hon cot trai 1 hang -> chuyen "Thu ky" (1 item, control panel1)
                    //tu cot phai sang cot trai, ngay duoi "Nguoi tu van" (hang gom layoutControlItem10 + layoutControlItem11)
                    layoutControlItem20.Move(layoutControlItem10, InsertType.Bottom);
                    layoutControlItem20.Width = layoutControlItem10.Width + layoutControlItem11.Width;
                    layoutControlItem20.TextAlignMode = TextAlignModeItem.CustomSize;
                    layoutControlItem20.TextSize = lciStartTime.TextSize;
                    layoutControlItem20.TextToControlDistance = lciStartTime.TextToControlDistance;
                }
                finally
                {
                    layoutControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nap gio ket thuc thuc hien cua cac dich vu thuoc y lenh. Loi lay du lieu chi de trong, khong chan form.
        /// </summary>
        private void LoadPacsEndTime()
        {
            List<PacsEndTimeADO> ados = new List<PacsEndTimeADO>();
            try
            {
                if (cboPacsEndTime == null) return;

                CommonParam param = new CommonParam();
                HisSereServExtFilter extFilter = new HisSereServExtFilter();
                extFilter.TDL_SERVICE_REQ_ID = this.service_req_id;
                List<HIS_SERE_SERV_EXT> exts = new BackendAdapter(param)
                    .Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, extFilter, param);
                List<HIS_SERE_SERV_EXT> extHasValues = exts != null ? exts.Where(o => o.PACS_END_TIME.HasValue).ToList() : null;

                if (extHasValues != null && extHasValues.Count > 0)
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.SERVICE_REQ_ID = this.service_req_id;
                    List<HIS_SERE_SERV> sereServs = new BackendAdapter(param)
                        .Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, param);

                    foreach (HIS_SERE_SERV_EXT ext in extHasValues.OrderBy(o => o.PACS_END_TIME))
                    {
                        HIS_SERE_SERV ss = sereServs != null ? sereServs.FirstOrDefault(o => o.ID == ext.SERE_SERV_ID) : null;
                        if (ss != null && ss.IS_DELETE == 1) continue;

                        PacsEndTimeADO ado = new PacsEndTimeADO();
                        ado.TDL_SERVICE_NAME = ss != null ? ss.TDL_SERVICE_NAME : "";
                        ado.PACS_END_TIME = ext.PACS_END_TIME.Value;
                        ado.PACS_END_TIME_STR = FormatPacsEndTime(ext.PACS_END_TIME.Value);
                        ados.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                ados = new List<PacsEndTimeADO>();
            }

            try
            {
                pacsEndTimes = ados;
                gridPacsEndTime.DataSource = pacsEndTimes;
                //moc muon nhat trong cac dich vu (danh sach da sap xep tang dan)
                pacsEndTimeDisplay = pacsEndTimes.Count > 0 ? pacsEndTimes[pacsEndTimes.Count - 1].PACS_END_TIME_STR : "";
                cboPacsEndTime.EditValue = pacsEndTimeDisplay;
                //nhieu dich vu moi hien nut so xuong
                cboPacsEndTime.Properties.Buttons[0].Visible = pacsEndTimes.Count > 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string FormatPacsEndTime(long timeNumber)
        {
            DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber);
            return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy HH:mm") : "";
        }

        private void cboPacsEndTime_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //1 dich vu tro xuong thi khong co gi de so
            e.Cancel = pacsEndTimes == null || pacsEndTimes.Count < 2;
        }

        private void cboPacsEndTime_QueryResultValue(object sender, QueryResultValueEventArgs e)
        {
            //dong popup khong lam doi gia tri dang hien thi
            e.Value = pacsEndTimeDisplay;
        }

        private void cboPacsEndTime_QueryDisplayText(object sender, QueryDisplayTextEventArgs e)
        {
            e.DisplayText = pacsEndTimeDisplay;
        }
    }
}
