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
    /// O "Thoi gian bat dau thuc hien": gio bat dau chup do PACS gui ve (OBR.7), luu theo tung dich vu
    /// tai HIS_SERE_SERV_EXT.PACS_BEGIN_TIME. Chi doc, khong tham gia luu / validate cua form.
    /// Y lenh co nhieu dich vu co du lieu thi o hien moc som nhat va co nut so xuong liet ke tung dich vu.
    /// Control duoc tao luc chay va chen duoi o "Thoi gian ket thuc" (cot phai) de khong phai sua Designer.
    /// </summary>
    public partial class frmServiceReqUpdateInstruction
    {
        private PopupContainerEdit cboPacsBeginTime;
        private PopupContainerControl popupPacsBeginTime;
        private GridControl gridPacsBeginTime;
        private GridView gridViewPacsBeginTime;
        private LayoutControlItem lciPacsBeginTime;
        private string pacsBeginTimeDisplay = "";
        private List<PacsBeginTimeADO> pacsBeginTimes = new List<PacsBeginTimeADO>();

        private class PacsBeginTimeADO
        {
            public string TDL_SERVICE_NAME { get; set; }
            public long PACS_BEGIN_TIME { get; set; }
            public string PACS_BEGIN_TIME_STR { get; set; }
        }

        private void InitPacsBeginTimeControl()
        {
            try
            {
                if (cboPacsBeginTime != null) return;

                gridViewPacsBeginTime = new GridView();
                gridViewPacsBeginTime.OptionsBehavior.Editable = false;
                gridViewPacsBeginTime.OptionsBehavior.ReadOnly = true;
                gridViewPacsBeginTime.OptionsView.ShowGroupPanel = false;
                gridViewPacsBeginTime.OptionsView.ShowIndicator = false;
                gridViewPacsBeginTime.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewPacsBeginTime.OptionsSelection.EnableAppearanceFocusedRow = false;
                GridColumn colService = gridViewPacsBeginTime.Columns.AddVisible("TDL_SERVICE_NAME", "Dịch vụ");
                colService.Width = 300;
                GridColumn colTime = gridViewPacsBeginTime.Columns.AddVisible("PACS_BEGIN_TIME_STR", "Thời gian bắt đầu thực hiện");
                colTime.Width = 150;

                gridPacsBeginTime = new GridControl();
                gridPacsBeginTime.Dock = System.Windows.Forms.DockStyle.Fill;
                gridPacsBeginTime.ViewCollection.Add(gridViewPacsBeginTime);
                gridPacsBeginTime.MainView = gridViewPacsBeginTime;
                gridViewPacsBeginTime.GridControl = gridPacsBeginTime;

                popupPacsBeginTime = new PopupContainerControl();
                popupPacsBeginTime.Size = new System.Drawing.Size(470, 160);
                popupPacsBeginTime.Controls.Add(gridPacsBeginTime);
                this.Controls.Add(popupPacsBeginTime);

                cboPacsBeginTime = new PopupContainerEdit();
                cboPacsBeginTime.Name = "cboPacsBeginTime";
                cboPacsBeginTime.Properties.PopupControl = popupPacsBeginTime;
                cboPacsBeginTime.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                cboPacsBeginTime.Properties.CloseOnOuterMouseClick = true;
                cboPacsBeginTime.Properties.ShowPopupCloseButton = false;
                cboPacsBeginTime.Properties.PopupSizeable = false;
                cboPacsBeginTime.Properties.QueryPopUp += cboPacsBeginTime_QueryPopUp;
                cboPacsBeginTime.Properties.QueryResultValue += cboPacsBeginTime_QueryResultValue;
                cboPacsBeginTime.Properties.QueryDisplayText += cboPacsBeginTime_QueryDisplayText;

                layoutControl1.BeginUpdate();
                try
                {
                    layoutControl1.Controls.Add(cboPacsBeginTime);
                    lciPacsBeginTime = layoutControlGroup1.AddItem("Thời gian bắt đầu thực hiện:", cboPacsBeginTime);
                    lciPacsBeginTime.Name = "lciPacsBeginTime";
                    lciPacsBeginTime.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciPacsBeginTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    //nhan dai hon cac nhan cung cot -> de layout tu tinh do rong theo chu (AutoSize), khong do tay de khoi lech theo font/DPI
                    lciPacsBeginTime.TextAlignMode = TextAlignModeItem.AutoSize;
                    lciPacsBeginTime.TextToControlDistance = lciEndTime.TextToControlDistance;
                    //dat o cot phai, ngay duoi "Thoi gian ket thuc" (cot phai nhan rong 160 nen gan thang hang)
                    lciPacsBeginTime.Move(lciEndTime, InsertType.Bottom);
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
        /// Nap gio bat dau thuc hien cua cac dich vu thuoc y lenh. Loi lay du lieu chi de trong, khong chan form.
        /// </summary>
        private void LoadPacsBeginTime()
        {
            List<PacsBeginTimeADO> ados = new List<PacsBeginTimeADO>();
            try
            {
                if (cboPacsBeginTime == null) return;

                CommonParam param = new CommonParam();
                HisSereServExtFilter extFilter = new HisSereServExtFilter();
                extFilter.TDL_SERVICE_REQ_ID = this.service_req_id;
                List<HIS_SERE_SERV_EXT> exts = new BackendAdapter(param)
                    .Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, extFilter, param);
                List<HIS_SERE_SERV_EXT> extHasValues = exts != null ? exts.Where(o => o.PACS_BEGIN_TIME.HasValue).ToList() : null;

                if (extHasValues != null && extHasValues.Count > 0)
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.SERVICE_REQ_ID = this.service_req_id;
                    List<HIS_SERE_SERV> sereServs = new BackendAdapter(param)
                        .Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, param);

                    foreach (HIS_SERE_SERV_EXT ext in extHasValues.OrderBy(o => o.PACS_BEGIN_TIME))
                    {
                        HIS_SERE_SERV ss = sereServs != null ? sereServs.FirstOrDefault(o => o.ID == ext.SERE_SERV_ID) : null;
                        if (ss != null && ss.IS_DELETE == 1) continue;

                        PacsBeginTimeADO ado = new PacsBeginTimeADO();
                        ado.TDL_SERVICE_NAME = ss != null ? ss.TDL_SERVICE_NAME : "";
                        ado.PACS_BEGIN_TIME = ext.PACS_BEGIN_TIME.Value;
                        ado.PACS_BEGIN_TIME_STR = FormatPacsBeginTime(ext.PACS_BEGIN_TIME.Value);
                        ados.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                ados = new List<PacsBeginTimeADO>();
            }

            try
            {
                pacsBeginTimes = ados;
                gridPacsBeginTime.DataSource = pacsBeginTimes;
                //moc som nhat trong cac dich vu (danh sach da sap xep tang dan)
                pacsBeginTimeDisplay = pacsBeginTimes.Count > 0 ? pacsBeginTimes[0].PACS_BEGIN_TIME_STR : "";
                cboPacsBeginTime.EditValue = pacsBeginTimeDisplay;
                //nhieu dich vu moi hien nut so xuong
                cboPacsBeginTime.Properties.Buttons[0].Visible = pacsBeginTimes.Count > 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string FormatPacsBeginTime(long timeNumber)
        {
            DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber);
            return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy HH:mm") : "";
        }

        private void cboPacsBeginTime_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //1 dich vu tro xuong thi khong co gi de so
            e.Cancel = pacsBeginTimes == null || pacsBeginTimes.Count < 2;
        }

        private void cboPacsBeginTime_QueryResultValue(object sender, QueryResultValueEventArgs e)
        {
            //dong popup khong lam doi gia tri dang hien thi
            e.Value = pacsBeginTimeDisplay;
        }

        private void cboPacsBeginTime_QueryDisplayText(object sender, QueryDisplayTextEventArgs e)
        {
            e.DisplayText = pacsBeginTimeDisplay;
        }
    }
}
