using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO;
using HIS.Desktop.Plugins.SurgServiceReqExecute2.Config;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    public partial class UCSurgServiceReqExecute2 : HIS.Desktop.Utility.UserControlBase
    {
        private void WirePopupMenu_v45072()
        {
            try
            {
                if (gridControl1 == null) return;
                gridControl1.MouseUp -= GridControl1_MouseUp_v45072;
                gridControl1.MouseDown -= GridControl1_MouseDown_v45072;
                gridControl1.MouseDown += GridControl1_MouseDown_v45072;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridControl1_MouseDown_v45072(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right) return;

                int rowHandle = -1;
                var hitInfo = gridView1.CalcHitInfo(e.Location);
                if (hitInfo.InRow && !hitInfo.InGroupRow)
                {
                    rowHandle = hitInfo.RowHandle;
                    gridView1.FocusedRowHandle = rowHandle;
                }
                else
                {
                    rowHandle = gridView1.FocusedRowHandle;
                }

                if (rowHandle < 0) return;
                var row = gridView1.GetRow(rowHandle) as SereServView1ADO;
                if (row == null) return;
                this.currentRow = row;

                var ctx = new ContextMenuStrip();
                if (row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    var miUnstart = new ToolStripMenuItem(Resources.ResourceMessage.HuyBatDau);
                    miUnstart.Click += (s2, e2) => UnstartProcess_v45072(row);
                    ctx.Items.Add(miUnstart);
                }
                else if (row.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    var miUnfinish = new ToolStripMenuItem(Resources.ResourceMessage.HuyKetThuc);
                    miUnfinish.Click += (s2, e2) => UnfinishProcess_v45072(row);
                    ctx.Items.Add(miUnfinish);
                }
                if (ctx.Items.Count > 0)
                    ctx.Show(gridControl1, e.Location);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridControl1_MouseUp_v45072(object sender, MouseEventArgs e) { }

        private void UnstartProcess_v45072(SereServView1ADO row)
        {
            try
            {
                if (row == null || row.SERVICE_REQ_ID == null) return;
                if (XtraMessageBox.Show(
                        Resources.ResourceMessage.BanCoMuonHuyBatDauKhong,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                long reqId = row.SERVICE_REQ_ID ?? 0;
                var serviceReq = new BackendAdapter(param)
                    .Post<HIS_SERVICE_REQ>(HisRequestUriStore.MOSHIS_HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, reqId, param);
                WaitingManager.Hide();
                bool success = (serviceReq != null);
                if (success)
                {
                    row.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                    gridControl1.RefreshDataSource();
                    UpdateFooter45072();

                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    Inventec.Common.Logging.LogUtil.LogActionSuccess(
                        "UCSurgServiceReqExecute2", "Unstart", loginName);
                }
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UnfinishProcess_v45072(SereServView1ADO row)
        {
            try
            {
                if (row == null || row.SERVICE_REQ_ID == null) return;
                if (XtraMessageBox.Show(
                        Resources.ResourceMessage.BanCoMuonHuyKetThucKhong,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (HisConfigCFG.AutoDeleteEmrDocumentWhenEditReq == "1" && HisConfigCFG.IsHasConnectionEmr)
                {
                    bool emrOk = ProcessDeleteSignedEmrDocs_v45072(row);
                    if (!emrOk) return; // user cancel
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                long reqId = row.SERVICE_REQ_ID ?? 0;
                var serviceReq = new BackendAdapter(param)
                    .Post<HIS_SERVICE_REQ>(HisRequestUriStore.MOSHIS_HIS_SERVICE_REQ_UNFINISH, ApiConsumers.MosConsumer, reqId, param);
                WaitingManager.Hide();
                bool success = (serviceReq != null);
                if (success)
                {
                    row.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                    gridControl1.RefreshDataSource();
                    UpdateFooter45072();

                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    Inventec.Common.Logging.LogUtil.LogActionSuccess(
                        "UCSurgServiceReqExecute2", "Unfinish", loginName);
                }
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessDeleteSignedEmrDocs_v45072(SereServView1ADO row)
        {
            try
            {
                if (string.IsNullOrEmpty(row.TDL_TREATMENT_CODE) || string.IsNullOrEmpty(row.TDL_SERVICE_REQ_CODE))
                    return true;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var filter = new EMR.Filter.EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = row.TDL_TREATMENT_CODE;
                filter.IS_DELETE = false;
                filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                var allDocs = new BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>(
                    HisRequestUriStore.EMR_DOCUMENT_GET_VIEW, ApiConsumers.EmrConsumer, filter, param);
                WaitingManager.Hide();

                if (allDocs == null || allDocs.Count == 0) return true;

                string srMarker = "SERVICE_REQ_CODE:" + row.TDL_SERVICE_REQ_CODE;
                var matchedDocs = allDocs.Where(o => !string.IsNullOrEmpty(o.HIS_CODE) && o.HIS_CODE.Contains(srMarker)).ToList();
                if (matchedDocs.Count == 0) return true;

                if (XtraMessageBox.Show(
                        Resources.ResourceMessage.YLenhDaTonTaiVanBanKy,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return false;

                WaitingManager.Show();
                foreach (var doc in matchedDocs)
                {
                    try
                    {
                        CommonParam delParam = new CommonParam();
                        new BackendAdapter(delParam).Post<bool>(HisRequestUriStore.EMR_DOCUMENT_DELETE, ApiConsumers.EmrConsumer, doc.ID, delParam);
                    }
                    catch (Exception exDel)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exDel);
                    }
                }
                WaitingManager.Hide();
                return true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
