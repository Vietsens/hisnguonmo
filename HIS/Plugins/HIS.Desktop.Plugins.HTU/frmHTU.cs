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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HTU
{
    public partial class frmHTU : Form
    {
        HIS_HTU Hishtu {get;set;}
        HIS_HTU dehishtu = null;
        private List<HIS_HTU> lhishtu;
        int positionHandleControl = -1;
        internal int action = -1;

        public frmHTU(HIS_HTU _hishtu)
        {
            InitializeComponent();
            this.Hishtu = _hishtu;
        }

        private void frmHTU_Load(object sender, EventArgs e)
        {
            SetIcon();
            Initdatagrid();
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void Initdatagrid()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisHtuFilter filter = new MOS.Filter.HisHtuFilter();
                //filter.ID = Hishtu.ID;
                var htu = new BackendAdapter(param).Get<List<HIS_HTU>>(HisRequestUriStore.HIS_HTU_GET, ApiConsumers.MosConsumer, filter, param);
                gridControlHTU.DataSource = htu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridViewHTU_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.HIS_HTU dataRow = (MOS.EFMODEL.DataModels.HIS_HTU)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        //e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                        e.Value = e.ListSourceRowIndex +1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                this.Hishtu = new HIS_HTU();
                this.Hishtu.HTU_NAME = txtHtuName.Text;
                var dataResult = new BackendAdapter(param).Post<HIS_HTU>(HisRequestUriStore.HIS_HTU_CREATE, ApiConsumer.ApiConsumers.MosConsumer, this.Hishtu, null);
                if (dataResult != null)
                {
                    success = true;
                    Initdatagrid();
                    txtHtuName.Text = "";
                }
                //if (this.action == GlobalVariables.ActionAdd)
                //{
                //    //this.depositReq.TREATMENT_ID = treatmentID;
                //    var dataResult = new BackendAdapter(param).Post<HIS_HTU>(HisRequestUriStore.HIS_HTU_CREATE, ApiConsumer.ApiConsumers.MosConsumer, this.Hishtu, null);
                //    if (dataResult != null)
                //    {
                //        this.Hishtu = dataResult;
                //        success = true;
                //        //UpdateHisHtuToGrid(dataResult, action);
                //    }               
                //}

                WaitingManager.Hide();

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            var edit = (HIS_HTU)gridViewHTU.GetFocusedRow();
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                this.Hishtu = new HIS_HTU();
                this.Hishtu.HTU_NAME = txtHtuName.Text;
                this.Hishtu.ID = edit.ID;

                var data = new BackendAdapter(param).Post<HIS_HTU>(HisRequestUriStore.HIS_HTU_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, this.Hishtu, null);
                if (data != null)
                {
                    success = true;
                    Initdatagrid();
                    txtHtuName.Text = "";
                }
                WaitingManager.Hide();

                #region Show message
                ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (HIS_HTU)gridViewHTU.GetFocusedRow();
                if (data != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_HTU_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                    if (success)
                    {
                        Initdatagrid();
                        txtHtuName.Text = "";
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHTU_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    Hishtu = (HIS_HTU)gridViewHTU.GetFocusedRow();
                    txtHtuName.Text = Hishtu.HTU_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
               btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDelete_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //UpdateHisHtuToGrid
    }
}
