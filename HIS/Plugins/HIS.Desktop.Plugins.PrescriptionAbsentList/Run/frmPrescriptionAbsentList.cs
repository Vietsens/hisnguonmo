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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PrescriptionAbsentList.Run
{
    public partial class frmPrescriptionAbsentList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module _moduleData;
        MOS.SDO.WorkPlaceSDO WorkPlaceSDO;
        long counterGetDataExpMest = 0;
        long DefaultSeccondGetDataExpMest = 5;
        const string timer1Second = "timer1Second";
        const string timer2Second = "timer2Second";
        ConfigADO currentAdo;

        private static List<HIS_EXP_MEST> DisplayList;
        #endregion

        #region Construct
        public frmPrescriptionAbsentList()
        {
            InitializeComponent();
        }

        public frmPrescriptionAbsentList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this._moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPrescriptionAbsentList(Inventec.Desktop.Common.Modules.Module moduleData, ConfigADO ado)
            : this(moduleData)
        {
            try
            {
                this.currentAdo = ado;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmPrescriptionAbsentList_Load(object sender, EventArgs e)
        {
            try
            {
                RegisterTimer(this._moduleData.ModuleLink, timer1Second, 1000, timer1Second_Tick);
                RegisterTimer(this._moduleData.ModuleLink, timer2Second, 2000, timer2Second_Tick);
                StartTimer(this._moduleData.ModuleLink, timer1Second);
                StartTimer(this._moduleData.ModuleLink, timer2Second);
                lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                SettingDisplay(this.currentAdo);
                LoadCurrentBranch();
                Config.ConfigKeys.GetConfig();
                this.DefaultSeccondGetDataExpMest = Config.ConfigKeys.timerForAutoPatients;

                GetDataExpMest_PrescriptionAbsentList();
                InitRestoreLayoutGridViewFromXml(gridViewExpMest);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SettingDisplay(ConfigADO data)
        {
            try
            {
                if (data != null)
                {
                    List<decimal> sizes = new List<decimal>();
                    if (data.CoChuSTT != null)
                    {
                        gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font("Arial", (float)currentAdo.CoChuSTT);
                        gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font("Arial", (float)currentAdo.CoChuSTT);
                        sizes.Add((decimal)data.CoChuSTT);
                    }
                    if (data.CoChuTenBN != null)
                    {
                        gridColumnPatientName.AppearanceCell.Font = new System.Drawing.Font("Arial", (float)currentAdo.CoChuTenBN);
                        gridColumnPatientName.AppearanceHeader.Font = new System.Drawing.Font("Arial", (float)currentAdo.CoChuTenBN);
                        sizes.Add((decimal)data.CoChuTenBN);
                    }
                    if (data.CoChuTenQuay != null)
                    {
                        gridColumnGateCode.AppearanceCell.Font = new System.Drawing.Font("Arial", (float)currentAdo.CoChuTenQuay);
                        gridColumnGateCode.AppearanceHeader.Font = new System.Drawing.Font("Arial", (float)currentAdo.CoChuTenBN);
                        sizes.Add((decimal)data.CoChuTenQuay);
                    }
                    if (sizes.Count() > 0) gridViewExpMest.ColumnPanelRowHeight = gridViewExpMest.RowHeight = (int)(sizes.Max() * 2);



                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentBranch()
        {
            try
            {
                this.WorkPlaceSDO = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this._moduleData.RoomId);
                var branch = BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.ID == WorkPlaceSDO.BranchId).FirstOrDefault();
                if (branch != null)
                {
                    if (!string.IsNullOrEmpty(branch.LOGO_URL))
                    {
                        lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        MemoryStream fs = null;
                        try
                        {
                            fs = Inventec.Fss.Client.FileDownload.GetFile(branch.LOGO_URL);
                            pictureEditBranchImage.Image = System.Drawing.Image.FromStream(fs);
                            lciBranchImage.Size = pictureEditBranchImage.Image.Size;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                }
                else
                {
                    lciBranchImage.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                //Căn chữ ở giữa
                var oldPadding = lblDefault01.Padding;
                lblDefault01.Padding = new Padding(oldPadding.Left, oldPadding.Top, lciBranchImage.Width, oldPadding.Bottom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1Second_Tick()
        {
            try
            {
                this.counterGetDataExpMest += 1;
                if (this.counterGetDataExpMest >= this.DefaultSeccondGetDataExpMest)
                {
                    GetDataExpMest_PrescriptionAbsentList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataExpMest_PrescriptionAbsentList()
        {
            try
            {
                this.counterGetDataExpMest = 0;
                CreateThreadGetDataExpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadGetDataExpMest()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(GetDataExpMestNewThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void GetDataExpMestNewThread()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                filter.MEDI_STOCK_ID = this.WorkPlaceSDO.MediStockId;
                filter.LAST_APPROVAL_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Today);
                filter.IS_ABSENT = true;
                filter.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisExpMestFilter", filter));
                var apiResult = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.MOS_HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult", apiResult));
                if (apiResult != null)
                {
                    apiResult = apiResult.OrderBy(o => o.LAST_APPROVAL_TIME).ThenBy(o => o.MODIFY_TIME).ToList();
                    if (DisplayList == null)
                    {
                        DisplayList = apiResult;
                    }
                    else
                    {
                        var listIDs_api = apiResult.Select(o => o.ID).ToList();
                        for (int i = DisplayList.Count - 1; i > -1; i--)
                        {
                            if (listIDs_api.Contains(DisplayList[i].ID) == false)
                            {
                                DisplayList.RemoveAt(i);
                            }
                        }

                        var listIDs = DisplayList.Select(o => o.ID).ToList();
                        foreach (var item in apiResult)
                        {
                            if (listIDs.Contains(item.ID) == false)
                            {
                                DisplayList.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer2Second_Tick()
        {
            try
            {
                //CreateThreadDisplayToGridExpMest();
                DisplayToGridExpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadDisplayToGridExpMest()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplayToGridExpMestNewThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void DisplayToGridExpMestNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { DisplayToGridExpMest(); }));
                }
                else
                {
                    DisplayToGridExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisplayToGridExpMest()
        {
            try
            {
                var currentDisplayList = DisplayList;
                if (currentDisplayList != null && currentDisplayList.Count > this.currentAdo.rowNumber)
                {
                    currentDisplayList = currentDisplayList.Take(Convert.ToInt16(this.currentAdo.rowNumber)).ToList();
                }

                gridViewExpMest.BeginUpdate();
                gridControlExpMest.DataSource = currentDisplayList;
                gridViewExpMest.EndUpdate();

                if (DisplayList != null && DisplayList.Count > this.currentAdo.rowNumber)
                {
                    HIS_EXP_MEST firstItem = DisplayList.First();
                    DisplayList.RemoveAt(0);
                    DisplayList.Add(firstItem);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
