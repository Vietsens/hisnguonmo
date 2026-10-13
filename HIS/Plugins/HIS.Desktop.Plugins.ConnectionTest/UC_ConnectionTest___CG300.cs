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
using DevExpress.XtraLayout;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using HIS.Desktop.Plugins.ConnectionTest.CG300;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class UC_ConnectionTest : HIS.Desktop.Utility.UserControlBase
    {
        #region CG300 - Declare

        private const int CG300_CHECK_WIDTH = 95;

        /// <summary>Checkbox "In tem CG300" - created at runtime, only visible when CG300 is connected.</summary>
        private CheckEdit chkPrintBarcodeCG300;
        private LayoutControlItem lciPrintBarcodeCG300;

        /// <summary>Last code used to search in CG300 mode. Scanning the same code again = print.</summary>
        private string lastScanCodeCG300;

        /// <summary>Blocks overlapping scans while a scan is being processed.</summary>
        private bool isProcessingCG300 = false;

        /// <summary>Barcodes already sent to CG300 in this session (RePrint flag + reprint confirm).</summary>
        private HashSet<string> printedBarcodesCG300 = new HashSet<string>();

        /// <summary>SERVICE_ID -> TEST_SAMPLE_TYPE_ID (HIS_SERVICE_TESA), lazy-loaded once.</summary>
        private ILookup<long, long> testSampleTypeIdsByServiceIdCG300;

        /// <summary>HIS_TEST_SAMPLE_TYPE by ID, lazy-loaded once.</summary>
        private Dictionary<long, HIS_TEST_SAMPLE_TYPE> testSampleTypeByIdCG300;

        private EnumCg300ConnectState cg300State = EnumCg300ConnectState.NotConfigured;
        private string defaultSearchNullPrompt;

        #endregion

        #region CG300 - Init + connection

        /// <summary>
        /// Called in Load, after InitControlState: build the checkbox, load workstation config,
        /// restore the saved checked state and check the connection.
        /// </summary>
        private void InitCG300()
        {
            try
            {
                Cg300CFG.LoadConfig();
                defaultSearchNullPrompt = txtSearchKey.Properties.NullValuePrompt;
                if (String.IsNullOrWhiteSpace(Cg300CFG.FolderPath))
                {
                    // Workstation does not use CG300 -> add nothing to the layout
                    cg300State = EnumCg300ConnectState.NotConfigured;
                    return;
                }

                CreateCG300Control();
                RestoreCG300ControlState();
                RefreshConnectCG300();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateCG300Control()
        {
            try
            {
                layoutControl1.BeginUpdate();
                try
                {
                    chkPrintBarcodeCG300 = new CheckEdit();
                    chkPrintBarcodeCG300.Name = "chkPrintBarcodeCG300";
                    chkPrintBarcodeCG300.Properties.Caption = GetLangValue("UC_ConnectionTest.chkPrintBarcodeCG300.Properties.Caption", "In tem CG300");
                    chkPrintBarcodeCG300.ToolTip = GetLangValue("UC_ConnectionTest.chkPrintBarcodeCG300.ToolTip", "In tem qua máy CG300: quét mã để tìm, quét lại cùng mã để in tem và nhả ống");
                    chkPrintBarcodeCG300.StyleController = layoutControl1;
                    chkPrintBarcodeCG300.CheckedChanged += chkPrintBarcodeCG300_CheckedChanged;
                    layoutControl1.Controls.Add(chkPrintBarcodeCG300);

                    // Put the checkbox right of the search box (layoutControlItem7 = txtSearchKey)
                    lciPrintBarcodeCG300 = layoutControlGroup1.AddItem();
                    lciPrintBarcodeCG300.Name = "lciPrintBarcodeCG300";
                    lciPrintBarcodeCG300.Control = chkPrintBarcodeCG300;
                    lciPrintBarcodeCG300.TextVisible = false;
                    lciPrintBarcodeCG300.Move(layoutControlItem7, DevExpress.XtraLayout.Utils.InsertType.Right);
                    lciPrintBarcodeCG300.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciPrintBarcodeCG300.MinSize = new Size(CG300_CHECK_WIDTH, layoutControlItem7.Height);
                    lciPrintBarcodeCG300.MaxSize = new Size(CG300_CHECK_WIDTH, layoutControlItem7.Height);
                    lciPrintBarcodeCG300.Size = new Size(CG300_CHECK_WIDTH, layoutControlItem7.Height);
                    lciPrintBarcodeCG300.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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

        private void RestoreCG300ControlState()
        {
            try
            {
                if (chkPrintBarcodeCG300 == null || currentControlStateRDO == null) return;
                var item = currentControlStateRDO.FirstOrDefault(o => o.KEY == ControlStateConstant.CHECK_PRINT_BARCODE_CG300
                    && o.MODULE_LINK == ControlStateConstant.MODULE_LINK);
                if (item == null) return;

                isNotLoadWhileChangeControlStateInFirst = true;
                chkPrintBarcodeCG300.Checked = item.VALUE == "1";
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Check the connection and show/hide the checkbox. Called on Load, on Search and before each write.</summary>
        private bool RefreshConnectCG300()
        {
            try
            {
                if (chkPrintBarcodeCG300 == null) return false;

                cg300State = Cg300ConnectionChecker.Check();
                bool isConnected = cg300State == EnumCg300ConnectState.Connected;
                lciPrintBarcodeCG300.Visibility = isConnected
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!isConnected)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CG300 chua ket noi: " + GetCG300StateMessage(cg300State));
                }
                UpdateSearchPromptCG300();
                return isConnected;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>CG300 mode = checkbox visible (connected) and checked.</summary>
        private bool IsCG300Mode()
        {
            return chkPrintBarcodeCG300 != null
                && lciPrintBarcodeCG300 != null
                && lciPrintBarcodeCG300.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                && chkPrintBarcodeCG300.Checked;
        }

        private void UpdateSearchPromptCG300()
        {
            try
            {
                txtSearchKey.Properties.NullValuePrompt = IsCG300Mode()
                    ? GetLangValue("UC_ConnectionTest.txtSearchKey.NullValuePromptCG300", "Quét barcode...")
                    : defaultSearchNullPrompt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintBarcodeCG300_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateSearchPromptCG300();
                lastScanCodeCG300 = null;
                if (isNotLoadWhileChangeControlStateInFirst) return;

                var item = currentControlStateRDO != null
                    ? currentControlStateRDO.FirstOrDefault(o => o.KEY == ControlStateConstant.CHECK_PRINT_BARCODE_CG300 && o.MODULE_LINK == ControlStateConstant.MODULE_LINK)
                    : null;
                if (item != null)
                {
                    item.VALUE = chkPrintBarcodeCG300.Checked ? "1" : "";
                }
                else
                {
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = ControlStateConstant.CHECK_PRINT_BARCODE_CG300,
                        MODULE_LINK = ControlStateConstant.MODULE_LINK,
                        VALUE = chkPrintBarcodeCG300.Checked ? "1" : ""
                    });
                }
                if (controlStateWorker != null)
                    controlStateWorker.SetData(currentControlStateRDO);

                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region CG300 - Scan

        /// <summary>
        /// Scan handler in CG300 mode: same code as the last search and grid has data -> print; otherwise -> search.
        /// The cursor always goes back to the search box with its content selected.
        /// </summary>
        private void ProcessScanCG300(string code)
        {
            if (isProcessingCG300) return;
            isProcessingCG300 = true;
            try
            {
                code = code != null ? code.Trim() : "";
                var data = gridControlSample.DataSource as List<LisSampleADO>;
                bool isPrint = !String.IsNullOrEmpty(code)
                    && String.Equals(code, lastScanCodeCG300, StringComparison.OrdinalIgnoreCase)
                    && data != null && data.Any();

                if (isPrint)
                {
                    PrintBarcodeCG300(data);
                }
                else
                {
                    lastScanCodeCG300 = code;
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isProcessingCG300 = false;
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
        }

        #endregion

        #region CG300 - Print

        private void PrintBarcodeCG300(List<LisSampleADO> data)
        {
            try
            {
                if (!RefreshConnectCG300())
                {
                    XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.CG300MatKetNoi, GetCG300StateMessage(cg300State)),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    return;
                }

                List<LisSampleADO> samples = data.Where(o => !String.IsNullOrWhiteSpace(o.BARCODE)).ToList();
                List<string> warnings = new List<string>();
                var noBarcode = data.Where(o => String.IsNullOrWhiteSpace(o.BARCODE)).ToList();
                if (noBarcode.Any())
                {
                    warnings.Add(String.Format(Resources.ResourceMessage.CG300MauChuaCoBarcode,
                        String.Join(", ", noBarcode.Select(o => o.SERVICE_REQ_CODE).Distinct())));
                }
                if (!samples.Any())
                {
                    ShowCG300Warnings(warnings, true);
                    return;
                }

                // Reprint confirmation: barcode already sent to CG300 in this session
                if (samples.Any(o => printedBarcodesCG300.Contains(o.BARCODE)))
                {
                    if (XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.CG300MauDaInTemInLai,
                            String.Join(", ", samples.Where(o => printedBarcodesCG300.Contains(o.BARCODE)).Select(o => o.BARCODE))),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                WaitingManager.Show();
                Cg300BuildResult build = BuildCG300Labels(samples);
                if (build.UnmappedServiceNames.Any())
                {
                    warnings.Add(String.Format(Resources.ResourceMessage.CG300DichVuChuaGanLoaiMau,
                        String.Join("; ", build.UnmappedServiceNames)));
                }
                if (!build.Labels.Any())
                {
                    WaitingManager.Hide();
                    ShowCG300Warnings(warnings, true);
                    return;
                }

                string filePath = Cg300FileWriter.Write(Cg300CFG.FolderPath, build.Labels);
                if (String.IsNullOrEmpty(filePath))
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(
                        String.Format(Resources.ResourceMessage.CG300KhongGhiDuocFile, Cg300CFG.FolderPath),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi));
                    return;
                }

                foreach (var sample in build.SentSamples)
                {
                    printedBarcodesCG300.Add(sample.BARCODE);
                    UpdateSampleTakenAfterPrint(sample);
                }
                gridViewSample.RefreshData();
                WaitingManager.Hide();

                Inventec.Common.Logging.LogUtil.LogActionSuccess("UC_ConnectionTest", "PrintBarcodeCG300",
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                ShowCG300Warnings(warnings, false);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Cg300BuildResult BuildCG300Labels(List<LisSampleADO> samples)
        {
            Cg300BuildResult result = new Cg300BuildResult();
            try
            {
                LoadCG300Catalogs();

                CommonParam param = new CommonParam();
                LisSampleServiceViewFilter filter = new LisSampleServiceViewFilter();
                filter.SAMPLE_IDs = samples.Select(o => o.ID).Distinct().ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                List<V_LIS_SAMPLE_SERVICE> sampleServices = new BackendAdapter(param)
                    .Get<List<V_LIS_SAMPLE_SERVICE>>("api/LisSampleService/GetView", ApiConsumers.LisConsumer, filter, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

                Dictionary<string, V_HIS_SERVICE> serviceByCode = new Dictionary<string, V_HIS_SERVICE>();
                foreach (var sv in BackendDataWorker.Get<V_HIS_SERVICE>())
                {
                    if (sv != null && !String.IsNullOrEmpty(sv.SERVICE_CODE) && !serviceByCode.ContainsKey(sv.SERVICE_CODE))
                        serviceByCode.Add(sv.SERVICE_CODE, sv);
                }

                result = Cg300LabelBuilder.Build(
                    samples,
                    sampleServices,
                    serviceByCode,
                    testSampleTypeIdsByServiceIdCG300,
                    testSampleTypeByIdCG300,
                    printedBarcodesCG300,
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    GetLocalIpCG300());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>HIS_SERVICE_TESA + HIS_TEST_SAMPLE_TYPE are small catalogs without SERVICE_IDs filter -> load all active rows once.</summary>
        private void LoadCG300Catalogs()
        {
            try
            {
                if (testSampleTypeIdsByServiceIdCG300 != null && testSampleTypeByIdCG300 != null) return;

                CommonParam param = new CommonParam();
                HisTestSampleTypeFilter typeFilter = new HisTestSampleTypeFilter();
                typeFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var types = new BackendAdapter(param).Get<List<HIS_TEST_SAMPLE_TYPE>>(
                    "api/HisTestSampleType/Get", ApiConsumers.MosConsumer, typeFilter, param);

                HisServiceTesaFilter tesaFilter = new HisServiceTesaFilter();
                tesaFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var tesas = new BackendAdapter(param).Get<List<HIS_SERVICE_TESA>>(
                    "api/HisServiceTesa/Get", ApiConsumers.MosConsumer, tesaFilter, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

                if (types == null || tesas == null)
                {
                    // Keep the cache null so the next print retries the load
                    Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc danh muc loai mau benh pham / dich vu - loai mau cho CG300");
                    return;
                }

                testSampleTypeByIdCG300 = types.GroupBy(o => o.ID).ToDictionary(o => o.Key, o => o.First());
                testSampleTypeIdsByServiceIdCG300 = tesas
                    .Where(o => o.SERVICE_ID.HasValue)
                    .OrderBy(o => o.ID)
                    .ToLookup(o => o.SERVICE_ID.Value, o => o.TEST_SAME_TYPE_ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region CG300 - Helpers

        private void ShowCG300Warnings(List<string> warnings, bool isNothingSent)
        {
            try
            {
                List<string> lines = new List<string>();
                if (isNothingSent) lines.Add(Resources.ResourceMessage.CG300KhongCoDuLieuIn);
                if (warnings != null) lines.AddRange(warnings);
                if (!lines.Any()) return;

                XtraMessageBox.Show(String.Join(Environment.NewLine, lines),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetCG300StateMessage(EnumCg300ConnectState state)
        {
            switch (state)
            {
                case EnumCg300ConnectState.FolderNotFound:
                    return String.Format(Resources.ResourceMessage.CG300LyDoThuMucKhongTonTai, Cg300CFG.FolderPath);
                case EnumCg300ConnectState.FolderNotWritable:
                    return String.Format(Resources.ResourceMessage.CG300LyDoKhongGhiDuoc, Cg300CFG.FolderPath);
                case EnumCg300ConnectState.ProcessNotRunning:
                    return String.Format(Resources.ResourceMessage.CG300LyDoPhanMemChuaChay, Cg300CFG.ProcessName);
                default:
                    return "";
            }
        }

        private string GetLangValue(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return !String.IsNullOrEmpty(value) ? value : defaultValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }

        private string GetLocalIpCG300()
        {
            try
            {
                var ip = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                    .LastOrDefault(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return ip != null ? ip.ToString() : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion
    }
}
