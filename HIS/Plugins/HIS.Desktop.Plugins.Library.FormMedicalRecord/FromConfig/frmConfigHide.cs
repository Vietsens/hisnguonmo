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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord.FromConfig
{
    public partial class frmConfigHide : Form
    {
        private List<SDA_HIDE_CONTROL> _HideControls;
        //List<BarItemADO> _LstBarItem = null;
        List<HisEmrCoverTypeSDO> _LstEmrCoverTypeSDO = null;
        List<HIS_EMR_COVER_TYPE> _lstEmrCoverType = new List<HIS_EMR_COVER_TYPE>();


        public frmConfigHide(List<HIS_EMR_COVER_TYPE> lstItem)
        {
            InitializeComponent();
            this._lstEmrCoverType = lstItem;
            try
            {
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmConfigHide_Load(object sender, EventArgs e)
        {
            try
            {
                this._HideControls = HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.GetByModule("HIS.Desktop.Plugins.Library.FormMedicalRecord");
                gridControlMediRecord.BeginUpdate();
                gridControlMediRecord.DataSource = this._lstEmrCoverType;
                gridControlMediRecord.EndUpdate();
                _LstEmrCoverTypeSDO = new List<HisEmrCoverTypeSDO>();
                if (this._lstEmrCoverType != null && this._lstEmrCoverType.Count > 0)
                {
                    foreach (var item in this._lstEmrCoverType)
                    {
                        HisEmrCoverTypeSDO typeSdo = new HisEmrCoverTypeSDO();
                        typeSdo.IsActive = item.IS_ACTIVE == 1 ? true : false;
                        typeSdo.EmrCoverTypeId = item.ID;
                        this._LstEmrCoverTypeSDO.Add(typeSdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediRecord_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_EMR_COVER_TYPE data = (HIS_EMR_COVER_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = data.IS_ACTIVE == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChoiceAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnChoiceAll.Enabled) return;
                if (this._lstEmrCoverType != null)
                {
                    this._lstEmrCoverType.ForEach(o => o.IS_ACTIVE = 1);
                    gridControlMediRecord.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnchoiceAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnUnchoiceAll.Enabled) return;
                if (this._lstEmrCoverType != null)
                {
                    this._lstEmrCoverType.ForEach(o => o.IS_ACTIVE = 0);
                    gridControlMediRecord.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<HisEmrCoverTypeSDO> LstEmrCoverTypeSDO = new List<HisEmrCoverTypeSDO>();
                WaitingManager.Show();
                if (this._lstEmrCoverType != null && this._lstEmrCoverType.Count > 0)
                {
                    foreach (var item in this._lstEmrCoverType)
                    {
                        var Active = item.IS_ACTIVE == 1 ? true : false;
                        //var checkId = this._LstEmrCoverTypeSDO.Where(o => o.EmrCoverTypeId == item.ID && o.IsActive == Active);
                        //if (checkId == null || checkId.Count() == 0)
                        //{
                            HisEmrCoverTypeSDO typeSdo = new HisEmrCoverTypeSDO();
                            typeSdo.IsActive = Active;
                            typeSdo.EmrCoverTypeId = item.ID;
                            LstEmrCoverTypeSDO.Add(typeSdo);
                        //}
                    }
                }

                if (!btnSave.Enabled || this._LstEmrCoverTypeSDO == null || this._LstEmrCoverTypeSDO.Count <= 0) return;
                CommonParam param = new CommonParam();

                bool resultData = false;
                if (LstEmrCoverTypeSDO != null && LstEmrCoverTypeSDO.Count > 0)
                {
                    resultData = new BackendAdapter(param).Post<bool>("api/HisEmrCoverType/ChangeActive", ApiConsumers.MosConsumer, LstEmrCoverTypeSDO, param);

                    if (!resultData)
                    {
                        Inventec.Common.Logging.LogSystem.Info("output: " + resultData + " input: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => LstEmrCoverTypeSDO), LstEmrCoverTypeSDO) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    }                    

                    WaitingManager.Hide();
                    MessageManager.Show(this, param, resultData);
                }
                //else
                //{
                //    MessageManager.Show("Không có dữ liệu thay đổi");
                //}

                if (resultData)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewMediRecord.PostEditor();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_EMR_COVER_TYPE)gridViewMediRecord.GetFocusedRow();
                HIS_EMR_COVER_TYPE dataCreat = new HIS_EMR_COVER_TYPE();
                dataCreat = rowData;
                this._lstEmrCoverType.Remove(rowData);
                if (rowData.IS_ACTIVE == 1)
                {
                    dataCreat.IS_ACTIVE = 0;
                }
                else
                {
                    dataCreat.IS_ACTIVE = 1;
                }

                this._lstEmrCoverType.Add(dataCreat);

                gridControlMediRecord.BeginUpdate();
                gridControlMediRecord.DataSource = this._lstEmrCoverType.OrderByDescending(o=>o.IS_ACTIVE);
                gridControlMediRecord.EndUpdate();
                               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
        }


    }
}
