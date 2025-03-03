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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.ChooseObject
{
    public partial class frmChooseObject : Form
    {
        HIS.Desktop.Common.DelegateSelectData registerData;
        string cancelId = "-1";
        HIS.Desktop.Common.DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        public frmChooseObject()
        {
            InitializeComponent();
        }

        public frmChooseObject(HIS.Desktop.Common.DelegateSelectData _registerData, HIS.Desktop.Common.DelegateCloseForm_Uc closeForm_Uc)
        {
            InitializeComponent();
            try
            {
                this.registerData = _registerData;
                this.DelegateClose = closeForm_Uc;
                CloseThread = new System.Threading.Thread(ClosingForm);
                CloseThread.Start();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmChooseObject_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            InitButtonNew();
            SetCaptionByLanguageKey();
            emptySpaceItem4.MinSize = new System.Drawing.Size((this.Width - layoutControlItem1.Width) / 2, 96);
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterExamKiosk.Resources.Lang", typeof(HIS.Desktop.Plugins.RegisterExamKiosk.Popup.ChooseObject.frmChooseObject).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmChooseObject.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitButtonNew()
        {
            try
            {
                stopThread = true;
                var hisPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.IS_NOT_USE_FOR_PATIENT != 1 && o.ID != HisConfigCFG.PATIENT_TYPE_ID__BHYT && o.IS_NOT_FOR_KIOSK != 1 && o.IS_NOT_USE_FOR_PAYMENT != 1).OrderByDescending(o => o.ID).ToList();
                for (int i = 0; i < hisPatientType.Count; i++)
                {
                    TileItem tile = new TileItem();
                    tile.Elements = new TileItemElementCollection(tile);

                    TileItemElement content = new TileItemElement();
                    if (hisPatientType[i].PATIENT_TYPE_NAME.ToUpper().Trim().StartsWith("KHÁM"))
                    {
                        content.Text = hisPatientType[i].PATIENT_TYPE_NAME.ToUpper();
                    }
                    else
                    {
                        content.Text = "KHÁM " + hisPatientType[i].PATIENT_TYPE_NAME.ToUpper();
                    }
                    content.Appearance.Normal.Font = new Font("Microsoft Sans Serif", 17, FontStyle.Regular);
                    content.TextAlignment = TileItemContentAlignment.MiddleCenter;
                    tile.Elements.Add(content);

                    tile.Checked = false;
                    tile.Visible = true;
                    tile.ItemSize = TileItemSize.Medium;
                    tile.AppearanceItem.Normal.BackColor = Color.DarkGreen;
                    tile.ItemClick += ItemClick;
                    tile.Tag = hisPatientType[i].ID;
                    tile.AppearanceItem.Normal.ForeColor = Color.White;
                    tileGroup1.Items.Add(tile);
                }

                var groupCancel = new TileGroup();
                TileItem tileCelcel = new TileItem();
                TileItemElement contentCancel = new TileItemElement();
                contentCancel.Text = "HỦY BỎ";
                contentCancel.Appearance.Normal.Font = new Font("Microsoft Sans Serif", 17, FontStyle.Regular);
                contentCancel.TextAlignment = TileItemContentAlignment.MiddleCenter;
                tileCelcel.Elements.Add(contentCancel);
                tileCelcel.AppearanceItem.Normal.BackColor = Color.Red;
                tileCelcel.ItemClick += ItemClick;
                tileCelcel.Tag = this.cancelId;
                tileCelcel.AppearanceItem.Normal.ForeColor = Color.White;
                tileGroup1.Items.Add(tileCelcel);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                stopThread = true;
                if (e.Item.Tag.ToString() != this.cancelId)
                {
                    if (this.registerData != null)
                        this.registerData(e.Item.Tag);
                    Inventec.Common.Logging.LogSystem.Debug("__________________*****" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.Item.Tag), e.Item.Tag));
                }
                this.Close();
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCustomBurtton_Click(object sender, EventArgs e)
        {
            try
            {
                stopThread = true;
                var data = sender as Button;
                this.Close();
                if (this.registerData != null)
                    this.registerData(data.Tag);
                Inventec.Common.Logging.LogSystem.Debug("__________________*****" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.Tag), data.Tag));
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseObject_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public bool stopThread = false;
        private void ClosingForm()
        {
            try
            {
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    bool time_out = false;
                    ResetLoopCount();
                    while (!time_out)
                    {
                        if (stopThread)
                        {
                            ResetLoopCount();
                        }
                        if (loopCount <= 0)
                        {
                            time_out = true;
                        }

                        System.Threading.Thread.Sleep(50);
                        loopCount--;
                    }

                    this.Invoke(new MethodInvoker(delegate () { this.Close(); }));
                    if (DelegateClose != null)
                    {
                        DelegateClose(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        private void ResetLoopCount()
        {
            try
            {
                this.loopCount = HisConfigCFG.timeWaitingMilisecond / 50;

                Inventec.Common.Logging.LogSystem.Info("ResetLoopCount");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
