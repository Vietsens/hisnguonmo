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
using DevExpress.Utils;
using DevExpress.XtraEditors;
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
    /// tai HIS_SERE_SERV_EXT.PACS_END_TIME. Hien thi bang label (chi doc, khong go, khong dan),
    /// khong tham gia luu / validate cua form.
    /// Y lenh co nhieu dich vu co du lieu: label hien moc muon nhat kem "(n dich vu)"; di chuot vao
    /// hoac bam vao label se hien bang chi tiet tung dich vu + gio tuong ung (SuperToolTip).
    /// Control duoc tao luc chay va chen duoi o "Thoi gian ket thuc" (cot phai) de khong phai sua Designer.
    /// </summary>
    public partial class frmServiceReqUpdateInstruction
    {
        private LabelControl lblPacsEndTime;
        private LayoutControlItem lciPacsEndTime;
        private ToolTipController pacsEndTimeToolTip;
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
                if (lblPacsEndTime != null) return;

                pacsEndTimeToolTip = new ToolTipController();
                pacsEndTimeToolTip.ToolTipType = ToolTipType.SuperTip;
                pacsEndTimeToolTip.AutoPopDelay = 20000;

                lblPacsEndTime = new LabelControl();
                lblPacsEndTime.Name = "lblPacsEndTime";
                lblPacsEndTime.AutoSizeMode = LabelAutoSizeMode.None;
                lblPacsEndTime.Size = new System.Drawing.Size(200, 20);
                lblPacsEndTime.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
                lblPacsEndTime.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                lblPacsEndTime.ToolTipController = pacsEndTimeToolTip;
                lblPacsEndTime.Click += lblPacsEndTime_Click;

                layoutControl1.BeginUpdate();
                try
                {
                    layoutControl1.Controls.Add(lblPacsEndTime);
                    lciPacsEndTime = layoutControlGroup1.AddItem("Thời gian thực hiện xong:", lblPacsEndTime);
                    lciPacsEndTime.Name = "lciPacsEndTime";
                    lciPacsEndTime.AppearanceItemCaption.Options.UseTextOptions = true;
                    lciPacsEndTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    //nhan rong dung bang cac nhan cot phai (160) de noi dung bat dau thang hang voi "Thoi gian ket thuc" ben tren
                    lciPacsEndTime.TextAlignMode = TextAlignModeItem.CustomSize;
                    lciPacsEndTime.TextSize = lciEndTime.TextSize;
                    lciPacsEndTime.TextToControlDistance = lciEndTime.TextToControlDistance;
                    //dat o cot phai, ngay duoi "Thoi gian ket thuc"
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

                //panel1 (Thu ky) duoc layout keo rong theo cot trai nhung 2 control con co vi tri co dinh trong Designer
                //-> dat lai cho khop hang "Nguoi yeu cau" ben tren (o ma rong 113, o ten chiem phan con lai) va neo phai de theo resize
                txtSecretaryLoginName.Width = txtRequestUser.Width;
                cboSecretaryUserName.Left = txtSecretaryLoginName.Right;
                cboSecretaryUserName.Width = panel1.ClientSize.Width - cboSecretaryUserName.Left;
                cboSecretaryUserName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

                this.Height += lciPacsEndTime.Height + 8;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nap gio thuc hien xong cua cac dich vu thuoc y lenh. Loi lay du lieu chi de trong, khong chan form.
        /// </summary>
        private void LoadPacsEndTime()
        {
            List<PacsEndTimeADO> ados = new List<PacsEndTimeADO>();
            try
            {
                if (lblPacsEndTime == null) return;

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
                if (pacsEndTimes.Count == 0)
                {
                    lblPacsEndTime.Text = "";
                    lblPacsEndTime.SuperTip = null;
                    lblPacsEndTime.Cursor = System.Windows.Forms.Cursors.Default;
                }
                else if (pacsEndTimes.Count == 1)
                {
                    lblPacsEndTime.Text = pacsEndTimes[0].PACS_END_TIME_STR;
                    lblPacsEndTime.SuperTip = BuildPacsEndTimeSuperTip();
                    lblPacsEndTime.Cursor = System.Windows.Forms.Cursors.Default;
                }
                else
                {
                    //moc muon nhat trong cac dich vu (danh sach da sap xep tang dan) + so dich vu; chi tiet xem o tooltip
                    lblPacsEndTime.Text = string.Format("{0}  ({1} dịch vụ)", pacsEndTimes[pacsEndTimes.Count - 1].PACS_END_TIME_STR, pacsEndTimes.Count);
                    lblPacsEndTime.SuperTip = BuildPacsEndTimeSuperTip();
                    lblPacsEndTime.Cursor = System.Windows.Forms.Cursors.Hand;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private SuperToolTip BuildPacsEndTimeSuperTip()
        {
            SuperToolTip tip = new SuperToolTip();
            tip.Items.AddTitle("Thời gian thực hiện xong theo dịch vụ");
            foreach (PacsEndTimeADO ado in pacsEndTimes)
            {
                tip.Items.Add(string.Format("{0}   {1}", ado.PACS_END_TIME_STR, ado.TDL_SERVICE_NAME));
            }
            return tip;
        }

        private void lblPacsEndTime_Click(object sender, EventArgs e)
        {
            try
            {
                //bam vao label cung hien bang chi tiet (khong can cho tooltip)
                if (lblPacsEndTime.SuperTip == null) return;
                ToolTipControllerShowEventArgs args = pacsEndTimeToolTip.CreateShowArgs();
                args.SuperTip = lblPacsEndTime.SuperTip;
                args.ToolTipType = ToolTipType.SuperTip;
                pacsEndTimeToolTip.ShowHint(args, lblPacsEndTime.PointToScreen(new System.Drawing.Point(0, lblPacsEndTime.Height)));
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
    }
}
