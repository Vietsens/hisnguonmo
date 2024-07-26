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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    public partial class frmListComment : Form
    {
        private int currentPage = 0;
        private int pageSize = 15;
        private int totalLoadedData = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        HisStoreServiceReportSDO resultData;
        ReportCommentResultSDO lstEvaluate;
        MemoEdit txtRepComment = new MemoEdit();
        TableLayoutPanel tableDetailComment = new TableLayoutPanel();
        Inventec.Desktop.Common.Modules.Module moduleData;
        Inventec.Core.ApiResultObject<List<ReportCommentGetPagingResultSDO>> apiResult = null;
        string IdParent;
        bool senRepply = false;
        bool isShowRepply = true;
        long? numReply;
        public frmListComment(HisStoreServiceReportSDO result, Inventec.Desktop.Common.Modules.Module moduleData, string Id , bool isRepply)
        {
            this.senRepply = isRepply;
            this.IdParent = Id;
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.WindowState = FormWindowState.Maximized;
            this.moduleData = moduleData;
            this.resultData = result;
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            SetCaptionByLanguageKey();
            FillDataToControl();
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.Resources.Lang", typeof(frmListComment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmListComment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtComment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmListComment.txtComment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtComment.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmListComment.txtComment.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSend.Text = Inventec.Common.Resource.Get.Value("frmListComment.btnSend.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmListComment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                tableAllComment.Controls.Clear();
                currentPage = 0;
                totalLoadedData = 0;

                Inventec.Core.CommonParam param = new  Inventec.Core.CommonParam() ;
               
                param.Count = totalLoadedData;

                tableAllComment.DataBindings.Clear();
                tableDetailComment.DataBindings.Clear();
               
                param.Limit = pageSize;
                LoadPagingPar(param, false);
                
                if (senRepply == true)
                {
                    param.Limit = 10;
                    LoadPagingRepply(param, false);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPagingPar(object param, bool isLoadMore)
        {
            try
            {
                //get data comment
                Inventec.Common.Logging.LogSystem.Debug("Load data to list");
                int limit = ((CommonParam)param).Limit ?? 0;
                int start = isLoadMore ? 0 : ((CommonParam)param).Start ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                ReportCommentGetPagingFilter Filter = new ReportCommentGetPagingFilter();
                Filter.ServiceCode = resultData.ServiceCode;
                Filter.ParentId = null;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ReportCommentGetPagingResultSDO>>("api/HisStoreService/ReportCommentGetPaging", ApiConsumers.MosConsumer, Filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        if (isLoadMore)
                        {
                            FillToUCControlPar(data);
                        }
                        else
                        {
                            tableAllComment.Controls.Clear();
                            FillToUCControlPar(data);
                        }
                        rowCount = data.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                        totalLoadedData += data.Count;
                    }
                    else
                    {

                        MessageManager.Show(this, paramCommon, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPagingRepply(object param, bool isLoadMore)
        {
            try
            {
                //get data comment
                Inventec.Common.Logging.LogSystem.Debug("Load data to list");
                int limit = ((CommonParam)param).Limit ?? 0;
                int start = isLoadMore ? 0 : ((CommonParam)param).Start ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<ReportCommentGetPagingResultSDO>> apiResult = null;

                ReportCommentGetPagingFilter Filter = new ReportCommentGetPagingFilter();
                Filter.ServiceCode = resultData.ServiceCode;
                Filter.ParentId = IdParent;
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ReportCommentGetPagingResultSDO>>("api/HisStoreService/ReportCommentGetPaging", ApiConsumers.MosConsumer, Filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        if (isLoadMore)
                        {
                            FillToUCControlRepply(data);
                        }
                        else
                        {
                            tableDetailComment.Controls.Clear();
                            FillToUCControlRepply(data);
                        }
                        rowCount = data.Count;
                        dataTotal = apiResult.Param.Count ?? 0;
                        totalLoadedData += data.Count;
                    }
                    else
                    {

                        MessageManager.Show(this, paramCommon, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillToUCControlRepply(List<ReportCommentGetPagingResultSDO> data)
        {
            try
            {
                int rowIndex = tableDetailComment.RowCount;
                //int rowIndexShowRepply = rowIndex;
                int columnIndex = 0;
                tableDetailComment.Controls.Clear();
                tableDetailComment.RowStyles.Clear();
                this.tableDetailComment.AutoScroll = true;
                this.tableDetailComment.AutoSize = false;
                this.tableDetailComment.HorizontalScroll.Visible = false;
                isShowRepply = false;
                foreach (var item in data)
                {
                    UCItemComment ItemComment = new UCItemComment(item, this.moduleData, resultData, isShowRepply, (DelegateSelectData)showReplly) { Dock = DockStyle.Fill };
                    tableDetailComment.Controls.Add(ItemComment, columnIndex, rowIndex);
                    columnIndex++;
                    //EventHandler cardItem.Click += new EventHandler(CardItem_Click);
                    if (columnIndex >= tableDetailComment.ColumnCount)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                var dataComment = apiResult.Data;
                int indexComment = dataComment.FindIndex(c => c.Id == IdParent);

                tableDetailComment.Visible = true;
                txtComment.Visible = true;
                //btnSendRepply.Visible = true;
                tableDetailComment.RowCount = rowIndex + 1;

                //LabelControl showFull = new LabelControl();
                //showFull.Text = "Xem thêm";
                //Font underlinedFont = new Font(showFull.Font.FontFamily, showFull.Font.Size, FontStyle.Underline);
                //showFull.Font = underlinedFont;
                //showFull.Padding = new Padding(50, 0, 0, 0);
                //showFull.Click += showFull_Clicked;

                tableDetailComment.Padding = new Padding(50, 0, 0, 0);
                tableDetailComment.Height = 160;
                tableDetailComment.Width = 1000;
                txtRepComment.Margin = new Padding(50, 0, 0, 0);
                txtRepComment.Enabled = true;
                txtRepComment.Width = 800;
                txtRepComment.Height = 50;
                txtRepComment.Dock = DockStyle.Fill;
                txtRepComment.KeyPress += txtRepComment_KeyPress;
                txtRepComment.Properties.NullText = "Nhập bình luận";
                txtRepComment.Properties.NullValuePrompt = "Nhập bình luận";

                tableAllComment.Controls.Add(txtRepComment, 0, indexComment + 2);
                if (numReply > 0)
                {
                    tableAllComment.Controls.Add(tableDetailComment, 0, indexComment + 2);
                }
                //tableAllComment.Controls.Add(showFull, 0, indexComment + 2);
                //tableAllComment.SetCellPosition (txtRepComment,);
            }
                
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void showFull_Clicked(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FillToUCControlPar(List<ReportCommentGetPagingResultSDO> data)
        {
            try
            {
                int rowIndex = 1;//tableAllComment.RowCount;
                int columnIndex = 0;
                tableAllComment.Controls.Clear();
                tableAllComment.RowStyles.Clear();
                this.tableAllComment.AutoScroll = true;
                this.tableAllComment.AutoSize = false;
                this.tableAllComment.HorizontalScroll.Visible = false;
                isShowRepply = true;
                foreach (var item in data)
                {
                    UCItemComment ItemComment = new UCItemComment(item, this.moduleData, resultData, isShowRepply, (DelegateSelectData)showReplly) { Dock = DockStyle.Fill };
                    tableAllComment.Controls.Add(ItemComment, columnIndex, rowIndex);
                    columnIndex++;
                    //cardItem.Click += new EventHandler(CardItem_Click);
                    if (columnIndex >= tableAllComment.ColumnCount)
                    {
                        columnIndex = 0;
                        rowIndex = rowIndex + 1;
                    }
                    if (IdParent == item.Id)
                    {
                        numReply = item.NumOfReply;
                    }
                }
                //tableAllComment.RowCount = rowIndex + 1;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void showReplly(string data)
        {
            IdParent = data;
            senRepply = true;
            FillDataToControl();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendCommentPar();
                FillDataToControl();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SendCommentPar()
        {
            try
            {
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                ReportCommentInputSDO filter = new ReportCommentInputSDO();
                filter.ServiceCode = resultData.ServiceCode;
                filter.ParentId = null;
                filter.Content = txtComment.Text;
                lstEvaluate = new BackendAdapter(paramCommon).Post<ReportCommentResultSDO>("api/HisStoreService/ReportComment", ApiConsumers.MosConsumer, filter, paramCommon);
                if (lstEvaluate != null)
                {
                    MessageManager.Show(this, paramCommon, true);
                }
                else
                {
                    MessageManager.Show(this, paramCommon, false);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SendRepComment()
        {
            try
            {

                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                ReportCommentInputSDO filter = new ReportCommentInputSDO();
                filter.ServiceCode = resultData.ServiceCode;
                filter.ParentId = IdParent;
                filter.Content = txtRepComment.Text;
                lstEvaluate = new BackendAdapter(paramCommon).Post<ReportCommentResultSDO>("api/HisStoreService/ReportComment", ApiConsumers.MosConsumer, filter, paramCommon);
                if (lstEvaluate != null)
                {
                    MessageManager.Show(this, paramCommon, true);
                }
                else
                {
                    MessageManager.Show(this, paramCommon, false);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtComment_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    SendCommentPar();
                    txtComment.Text = "";
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRepComment_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    SendRepComment();
                    txtRepComment.Text = "";
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
