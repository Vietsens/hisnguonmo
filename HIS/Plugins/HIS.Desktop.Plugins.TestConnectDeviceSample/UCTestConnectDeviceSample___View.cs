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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.TestConnectDeviceSample.ADO;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Resources;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Worker;
using HIS.Desktop.Utility;
using LIS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    /// <summary>
    /// One tile per open port (green = signed in, amber = waiting for a staff card, red = scanner lost),
    /// the scan log grid and the non-blocking alerts.
    /// </summary>
    public partial class UCTestConnectDeviceSample : UserControlBase
    {
        #region Declare
        private const int MAX_SCAN_LOG_ROWS = 300;
        private const int ALERT_FORM_WIDTH = 460;
        private const int TILE_STATUS = 1;
        private const int TILE_INFO = 2;
        private static readonly Color COLOR_SIGNED_IN = Color.FromArgb(46, 125, 50);
        private static readonly Color COLOR_WAITING = Color.FromArgb(255, 179, 0);
        private static readonly Color COLOR_LOST = Color.FromArgb(198, 40, 40);
        private static readonly Color COLOR_TEXT_LIGHT = Color.White;
        private static readonly Color COLOR_TEXT_DARK = Color.FromArgb(33, 33, 33);

        private readonly Dictionary<string, TileItem> tileByPort = new Dictionary<string, TileItem>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, PendingScanADO> lastScanByPort = new Dictionary<string, PendingScanADO>(StringComparer.OrdinalIgnoreCase);
        private readonly List<ScanLogADO> scanLogs = new List<ScanLogADO>();
        // Lets the test harness run without beeping
        internal static bool muteSound = false;
        #endregion

        /// <summary>
        /// Adds/removes tiles to match the open ports and updates their texts and colors in place (no flicker
        /// on the 2-second timer).
        /// </summary>
        private void RefreshTiles()
        {
            try
            {
                this.tileControlPorts.BeginUpdate();
                try
                {
                    foreach (string portName in this.tileByPort.Keys.Where(o => !this.readers.ContainsKey(o)).ToList())
                    {
                        this.tileGroupPorts.Items.Remove(this.tileByPort[portName]);
                        this.tileByPort.Remove(portName);
                    }
                    foreach (string portName in this.readers.Keys.OrderBy(o => o.Length).ThenBy(o => o, StringComparer.OrdinalIgnoreCase))
                    {
                        TileItem tile;
                        if (!this.tileByPort.TryGetValue(portName, out tile))
                        {
                            tile = CreateTile(portName);
                            this.tileGroupPorts.Items.Add(tile);
                            this.tileByPort[portName] = tile;
                        }
                        UpdateTile(tile, portName);
                    }
                }
                finally
                {
                    this.tileControlPorts.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private TileItem CreateTile(string portName)
        {
            TileItem tile = new TileItem();
            try
            {
                tile.Name = "tile" + portName;
                tile.Tag = portName;
                tile.ItemSize = TileItemSize.Wide;
                tile.Elements.Add(CreateTileElement(portName, TileItemContentAlignment.TopLeft, 18F, FontStyle.Bold));
                tile.Elements.Add(CreateTileElement("", TileItemContentAlignment.MiddleLeft, 12F, FontStyle.Bold));
                tile.Elements.Add(CreateTileElement("", TileItemContentAlignment.BottomLeft, 9F, FontStyle.Regular));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return tile;
        }

        private static TileItemElement CreateTileElement(string text, TileItemContentAlignment alignment, float fontSize, FontStyle fontStyle)
        {
            TileItemElement element = new TileItemElement();
            try
            {
                element.Text = text;
                element.TextAlignment = alignment;
                element.Appearance.Normal.Font = new Font("Tahoma", fontSize, fontStyle);
                element.Appearance.Normal.Options.UseFont = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return element;
        }

        private void UpdateTile(TileItem tile, string portName)
        {
            try
            {
                string status;
                List<string> infoLines = new List<string>();
                Color backColor;
                Color foreColor = COLOR_TEXT_LIGHT;
                TestDeviceSampleADO session = SessionStore.GetByPort(portName);
                if (this.lostPorts.Contains(portName))
                {
                    status = GetLanguageText("UCTestConnectDeviceSample.Tile.MatKetNoi");
                    backColor = COLOR_LOST;
                }
                else if (session != null)
                {
                    status = session.Username;
                    infoLines.Add(session.Loginname + " – " + String.Format(GetLanguageText("UCTestConnectDeviceSample.Tile.DangNhapLuc"),
                        FormatTime(session.SessionTimeStart)));
                    backColor = COLOR_SIGNED_IN;
                }
                else
                {
                    status = GetLanguageText("UCTestConnectDeviceSample.Tile.ChoQuetThe");
                    backColor = COLOR_WAITING;
                    foreColor = COLOR_TEXT_DARK;
                }

                PendingScanADO lastScan;
                if (this.lastScanByPort.TryGetValue(portName, out lastScan))
                    infoLines.Add(String.Format(GetLanguageText("UCTestConnectDeviceSample.Tile.LuotGanNhat"),
                        lastScan.ReceivedAt.ToString("HH:mm:ss"), lastScan.Code));

                string info = String.Join(Environment.NewLine, infoLines);
                if (tile.Elements[TILE_STATUS].Text != status)
                    tile.Elements[TILE_STATUS].Text = status;
                if (tile.Elements[TILE_INFO].Text != info)
                    tile.Elements[TILE_INFO].Text = info;
                SetTileColor(tile, backColor, foreColor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void SetTileColor(TileItem tile, Color backColor, Color foreColor)
        {
            try
            {
                if (tile.AppearanceItem.Normal.BackColor == backColor && tile.AppearanceItem.Normal.ForeColor == foreColor)
                    return;
                foreach (DevExpress.Utils.AppearanceObject appearance in new[] { tile.AppearanceItem.Normal, tile.AppearanceItem.Hovered,
                    tile.AppearanceItem.Pressed, tile.AppearanceItem.Selected })
                {
                    appearance.BackColor = backColor;
                    appearance.BorderColor = backColor;
                    appearance.ForeColor = foreColor;
                    appearance.Options.UseBackColor = true;
                    appearance.Options.UseBorderColor = true;
                    appearance.Options.UseForeColor = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePendingLabel()
        {
            try
            {
                int waiting;
                lock (this.pendingLock)
                {
                    waiting = this.pendingScans.Count;
                }
                this.lblPending.Text = waiting > 0
                    ? String.Format(GetLanguageText("UCTestConnectDeviceSample.lblPending.Text"), waiting)
                    : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Result of a scan or a port event: scan log row, non-blocking alert (seen on any tab) and a sound.
        /// </summary>
        private void Notify(string portName, string code, DateTime time, string message, bool success)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(message))
                    return;
                AddScanLog(portName, code, time, message, !success);
                ShowAlert(portName, message, success);
                PlaySound(success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddScanLog(string portName, string code, DateTime time, string message, bool isError)
        {
            try
            {
                ScanLogADO row = new ScanLogADO();
                row.SCAN_TIME_STR = time.ToString("HH:mm:ss");
                row.PORT_NAME = portName;
                row.CODE = code;
                row.RESULT = message;
                row.IS_ERROR = isError;
                this.scanLogs.Insert(0, row);
                if (this.scanLogs.Count > MAX_SCAN_LOG_ROWS)
                    this.scanLogs.RemoveRange(MAX_SCAN_LOG_ROWS, this.scanLogs.Count - MAX_SCAN_LOG_ROWS);

                this.gridViewScanLog.BeginUpdate();
                try
                {
                    if (this.gridControlScanLog.DataSource == null)
                        this.gridControlScanLog.DataSource = this.scanLogs;
                    this.gridControlScanLog.RefreshDataSource();
                }
                finally
                {
                    this.gridViewScanLog.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Own AlertControl instead of MessageManager.ShowAlert: that one shows the message as a one-line caption
        /// with one color per control; here alerts stack, wrap long text and keep their own color.
        /// </summary>
        private void ShowAlert(string portName, string message, bool success)
        {
            try
            {
                Form owner = this.ParentForm ?? this.FindForm();
                if (owner == null || owner.IsDisposed)
                    return;
                string color = success ? "0,128,0" : "211,47,47";
                string caption = String.IsNullOrEmpty(portName) ? "" : String.Format("<color={0}>{1}</color>", color, portName);
                this.alertControlScan.Show(owner, caption, String.Format("<color={0}>{1}</color>", color, message));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void PlaySound(bool success)
        {
            try
            {
                if (muteSound)
                    return;
                if (success)
                    System.Media.SystemSounds.Asterisk.Play();
                else
                    System.Media.SystemSounds.Hand.Play();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void alertControlScan_GetDesiredAlertFormWidth(object sender, DevExpress.XtraBars.Alerter.AlertFormWidthEventArgs e)
        {
            try
            {
                e.Width = ALERT_FORM_WIDTH;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewScanLog_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                ScanLogADO row = this.gridViewScanLog.GetRow(e.RowHandle) as ScanLogADO;
                if (row != null && row.IS_ERROR)
                    e.Appearance.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string FormatUser(string username, string loginname)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(loginname))
                    return username ?? "";
                if (String.IsNullOrWhiteSpace(username))
                    return loginname;
                return String.Format("{0} ({1})", username, loginname);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return loginname ?? "";
            }
        }

        private static string FormatTime(long timeNumber)
        {
            try
            {
                DateTime? time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber);
                return time.HasValue ? time.Value.ToString("HH:mm") : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private string FormatScheduled(List<V_LIS_SAMPLE> futureSamples)
        {
            try
            {
                V_LIS_SAMPLE next = futureSamples.OrderBy(o => o.INTRUCTION_TIME ?? 0).First();
                return String.Format(ResourceMessage.YLenhHenGio, next.SERVICE_REQ_CODE,
                    Inventec.Common.DateTime.Convert.TimeNumberToTimeString(next.INTRUCTION_TIME ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private static string FormatPatient(V_LIS_SAMPLE sample)
        {
            try
            {
                return ((sample.LAST_NAME ?? "") + " " + (sample.FIRST_NAME ?? "")).Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
