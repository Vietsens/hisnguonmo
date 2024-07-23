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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using System.Globalization;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    public delegate void DelegateSelectData(string data);
    public partial class UCItemComment : UserControl
    {
        ReportCommentGetPagingResultSDO currentComment = new ReportCommentGetPagingResultSDO();
        Inventec.Desktop.Common.Modules.Module moduleData;
        HisStoreServiceReportSDO resultData;
        bool isShowReply ;
        bool statusLike;
        bool? isYouLike;
        DelegateSelectData dlgRefreshData;
        public UCItemComment(ReportCommentGetPagingResultSDO commentDetail, Inventec.Desktop.Common.Modules.Module moduleData, HisStoreServiceReportSDO result, bool isShow, DelegateSelectData dlgRefreshData)
        {
            InitializeComponent();
            this.resultData = result;
            this.moduleData = moduleData;
            this.currentComment = commentDetail;
            this.isShowReply = isShow;
            isYouLike = currentComment.IsYouLiked;
            this.dlgRefreshData = dlgRefreshData;
            SetCaptionByLanguageKey();
            FillDataToControl();
        }

        private void UCItemComment_Load(object sender, EventArgs e)
        {
            try
            {
                //FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.Resources.Lang", typeof(UCItemComment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCItemComment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblNumRepply.Text = Inventec.Common.Resource.Get.Value("UCItemComment.lblNumRepply.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblNumLike.Text = Inventec.Common.Resource.Get.Value("UCItemComment.lblNumLike.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblContent.Text = Inventec.Common.Resource.Get.Value("UCItemComment.lblContent.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblTimePost.Text = Inventec.Common.Resource.Get.Value("UCItemComment.lblTimePost.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblUserName.Text = Inventec.Common.Resource.Get.Value("UCItemComment.lblUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblStatusLike.Text = Inventec.Common.Resource.Get.Value("UCItemComment.lblStatusLike.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCItemComment.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
               
                lblUserName.Text = currentComment.CustomerName;
                lblContent.AutoSizeInLayoutControl = true;
                lblContent.Text = !string.IsNullOrEmpty(currentComment.Content) ? (currentComment.Content.Length > 200 ? currentComment.Content.Substring(0, 200).ToString() + "..." : currentComment.Content) : "Content";
                //lblContent.Text = currentComment.Content;
                lblNumLike.Text = currentComment.NumOfLiked.ToString();
                lblNumRepply.Text = currentComment.NumOfReply.ToString();

                string dateString = currentComment.PostTime.ToString();
                string format = "yyyyMMddHHmmss";
                DateTime dateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
                string formattedDate = dateTime.ToString("dd/MM/yyyy HH:mm:ss");
                lblTimePost.Text = string.Format(" | {0}", formattedDate);
                if (isShowReply == false)
                {
                    picShowRepply.Visible = false;
                    panelControl1.Controls.Clear();
                }
                else
                {
                    panelControl1.Visible = true;
                    picShowRepply.Visible = true;
                }
                if (isYouLike == true)
                {
                    lblStatusLike.AppearanceItemCaption.ForeColor = Color.Green;
                    lblNumLike.ForeColor = Color.Green;
                    statusLike = true;
                    isYouLike = false;
                }
                else
                {
                    lblStatusLike.AppearanceItemCaption.ForeColor = Color.Black;
                    lblNumLike.ForeColor = Color.Black;
                    statusLike = false;
                    isYouLike = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void picStatusLike_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                ReportCommentLikeInputSDO filter = new ReportCommentLikeInputSDO();
                filter.CommentId = currentComment.Id;
                bool resuilt = new BackendAdapter(paramCommon).Post<bool>("api/HisStoreService/ReportCommentLike", ApiConsumers.MosConsumer, filter, paramCommon);
                if (resuilt == true)
                {
                    //MessageManager.Show( paramCommon, true);
                    FillDataToControl();
                    if (statusLike == false)
                    {
                        lblStatusLike.AppearanceItemCaption.ForeColor = Color.Black;
                        currentComment.NumOfLiked = currentComment.NumOfLiked - 1;
                        lblNumLike.Text = currentComment.NumOfLiked.ToString();
                        lblNumLike.ForeColor = Color.Black;
                    }
                    else if (statusLike == true)
                    {
                        lblStatusLike.AppearanceItemCaption.ForeColor = Color.Green;
                        currentComment.NumOfLiked = currentComment.NumOfLiked + 1;
                        lblNumLike.Text = currentComment.NumOfLiked.ToString();
                        lblNumLike.ForeColor = Color.Green;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("api/HisStoreService/ReportCommentLike khong hoat dong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resuilt), resuilt));
                    MessageManager.Show(paramCommon, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool isShow = false;
        private void picRepply_Click(object sender, EventArgs e)
        {
            try
            {
                if (isShow == false)
                {
                    picShowRepply.Image = Properties.Resources.arrowUp;
                    isShow = true;
                }
                else
                    {
                        picShowRepply.Image = Properties.Resources.arrowDown;
                        isShow = false;
                    }

                if (dlgRefreshData != null)
                    dlgRefreshData(currentComment.Id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

           
        }
        
    }
}
