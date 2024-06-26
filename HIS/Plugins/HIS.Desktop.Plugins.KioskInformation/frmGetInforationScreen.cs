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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
using DevExpress.XtraLayout;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Drawing.Drawing2D;

namespace HIS.Desktop.Plugins.KioskInformation
{
    public partial class frmGetInforationScreen : FormBase
    {
        KioskInformationSDO kioskInform = new KioskInformationSDO();
        int subclinicalsStatus = 0;
        int examinationHeight = 0;
        Image _image;
        Image examinationsBackgr;
        Image subclinicalsBackgr;
        Graphics graphics;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public frmGetInforationScreen(Inventec.Desktop.Common.Modules.Module currentModule, Image image)
            : this(null, null, null)
        {
        }
        public frmGetInforationScreen(Inventec.Desktop.Common.Modules.Module currentModule, MOS.SDO.KioskInformationSDO _kioskInform, Image image)
            : base(currentModule)
        {
            WaitingManager.Show();
            this._image = image;
            InitializeComponent();

            try
            {
                this.kioskInform = _kioskInform;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadInformationScreen()
        {
            try
            {
                WaitingManager.Show();
                SetSizeOfLayouts();
                SetBackGroundImage();
                if (this.kioskInform != null)
                {

                    // Hiển thị kết quả ra màn hình
                    lblPatientAddress.Text = kioskInform.PatientAddress;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(kioskInform.Dob);
                    lblGenderName.Text = kioskInform.GenderName;
                    if (kioskInform.HeinCardNumber != null)
                    {
                        string s1 = kioskInform.HeinCardNumber.Substring(0, 2) + "-";
                        string s2 = kioskInform.HeinCardNumber.Substring(2, 1) + "-";
                        string s3 = kioskInform.HeinCardNumber.Substring(3, 2) + "-";
                        string s4 = kioskInform.HeinCardNumber.Substring(5, 2) + "-";
                        string s5 = kioskInform.HeinCardNumber.Substring(7, 3) + "-";
                        string s6 = kioskInform.HeinCardNumber.Substring(10, 5);
                        lblHeinCardNumber.Text = s1 + s2 + s3 + s4 + s5 + s6;
                    }
                    lblHeinMediOrgName.Text = kioskInform.HeinMediOrgName;
                    lblPatientName.Text = kioskInform.PatientName;
                    if (this.kioskInform.Examinations != null && this.kioskInform.Examinations.Count() > 0)
                    {
                        //foreach (var item in this.kioskInform.Examinations)
                        //{
                        //    Inventec.Common.Logging.LogSystem.Info("STT chờ KL Examinations= ");// + item.ResultingNumOrder != null ? item.ResultingNumOrder.ToString() : "ko có data");
                        //}
                        gridControlExaminations.BeginUpdate();
                        gridControlExaminations.DataSource = null;
                        gridControlExaminations.DataSource = kioskInform.Examinations;
                        gridControlExaminations.EndUpdate();
                    }
                    if (this.kioskInform.Subclinicals != null && this.kioskInform.Subclinicals.Count() > 0)
                    {
                        gridControlSubclinicals.BeginUpdate();
                        gridControlSubclinicals.DataSource = null;
                        gridControlSubclinicals.DataSource = kioskInform.Subclinicals;
                        gridControlSubclinicals.EndUpdate();
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private Image ResizeImage(Image imgToResize, int _destWidth, int _destHeight)
        {
            int destWidth = _destWidth;
            int destHeight = _destHeight;
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            imgToResize = b;
            g.Dispose();
            return (Image)imgToResize;
        }
        private void SetBackGroundImage()
        {
            try
            {
                layoutControlGroup2.BackgroundImage = _image;
                var imgarray = new Image[3];
                // Thiếu hàm Resize
                Image _imageFixedSize = ResizeImage(_image, this.layoutControlGroup2.Width, this.layoutControlGroup2.Height);
                var img = _imageFixedSize;

                for (int j = 0; j < 3; j++)
                {
                    var index = j;
                    switch (j)
                    {

                        case 0:
                            {
                                imgarray[index] = new Bitmap(layoutControlGroup2.Width, layoutControlGroup2.Height * 150 / 668);
                                break;
                            }
                        case 1:
                            {
                                imgarray[index] = new Bitmap(layoutControlGroup2.Width, layoutControlGroup2.Height * 438 / 668);
                                break;
                            }
                        case 2:
                            {
                                imgarray[index] = new Bitmap(layoutControlGroup2.Width, layoutControlGroup2.Height * 80 / 668);
                                break;
                            }
                    }
                    if (imgarray[index] != null)
                    {
                        graphics = Graphics.FromImage(imgarray[index]);
                    }
                    if (graphics != null)
                    {
                        switch (j)
                        {
                            case 0:
                                {
                                    graphics.DrawImage(img, new Rectangle(0, 0, layoutControlGroup2.Width, layoutControlGroup2.Height * 150 / 668), new Rectangle(0, 0, layoutControlGroup2.Width, layoutControlGroup2.Height * 150 / 668), GraphicsUnit.Pixel);
                                    break;
                                }
                            case 1:
                                {
                                    graphics.DrawImage(img, new Rectangle(0, 0, layoutControlGroup2.Width, layoutControlGroup2.Height * 438 / 668), new Rectangle(0, layoutControlGroup2.Height * 150 / 668, layoutControlGroup2.Width, layoutControlGroup2.Height * 438 / 668), GraphicsUnit.Pixel);
                                    break;
                                }
                            case 2:
                                {
                                    graphics.DrawImage(img, new Rectangle(0, layoutControlGroup2.Height * 438 / 668, layoutControlGroup2.Width, layoutControlGroup2.Height * 150 / 668), new Rectangle(0, 0, layoutControlGroup2.Width, layoutControlGroup2.Height * 80 / 668), GraphicsUnit.Pixel);
                                    break;
                                }
                        }
                        graphics.Dispose();
                    }

                }

                if (imgarray[0] != null)
                    examinationsBackgr = imgarray[0];
                gridControlExaminations.BackgroundImage = examinationsBackgr;
                if (imgarray[1] != null)
                    subclinicalsBackgr = imgarray[1];
                gridControlSubclinicals.BackgroundImage = subclinicalsBackgr;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSizeOfLayouts()
        {
            try
            {
                lciGeneralInfor.MinSize = new System.Drawing.Size(this.Width, this.Height * 100 / 768);
                lciGeneralInfor.MaxSize = new System.Drawing.Size(this.Width, this.Height * 100 / 768);
                layoutControlItem2.MaxSize = new System.Drawing.Size(this.Width, this.Height * 668 / 768);
                layoutControlItem2.MinSize = new System.Drawing.Size(this.Width, this.Height * 668 / 768);

                lcigridControlExaminations.MinSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 150 / 668);
                lcigridControlExaminations.MaxSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 150 / 668);
                lcigridControlSubclinicals.MinSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 438 / 668);
                lcigridControlSubclinicals.MaxSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 438 / 668);

                layoutControlItem18.MinSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 80 / 668);
                layoutControlItem18.MaxSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 80 / 668);

                this.layoutControlItem18.MaxSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 80 / 668);
                this.layoutControlItem18.MinSize = new System.Drawing.Size(layoutControlGroup2.Width, layoutControlGroup2.Height * 80 / 668);
                this.layoutControlItem18.Padding = new DevExpress.XtraLayout.Utils.Padding(this.Width * 555 / 1300, this.Width * 555 / 1300, 15, 15);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string GetExamStatus(int status)
        {
            string result = null;
            switch (status)
            {
                case 1:
                    result = "Chưa thực hiện";
                    break;
                case 2:
                    result = "Đang thực hiện";
                    break;
                case 3:
                    result = "Đã hoàn thành";
                    break;
                case 4:
                    result = "Chờ kết quả cận lâm sàng";
                    break;
                case 5:
                    result = "Chờ kết luận";
                    break;
                default:
                    break;

            }
            return result;
        }
        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmGetInforationScreen_Load(object sender, EventArgs e)
        {
            InitializeComponent();
            LoadInformationScreen();
        }

        private void gridViewExaminations_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound) // đưa kiểu dữ liệu về kiểu khác hoặc Object
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (KioskServiceSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "Statuss")
                        {
                            e.Value = GetExamStatus((int)data.Status);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSubclinicals_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (KioskServiceSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "ServiceNamee")
                        {
                            e.Value = data.ServiceName;
                        }
                        if (e.Column.FieldName == "RoomNamee")
                        {
                            e.Value = data.RoomName;
                        }
                        if (e.Column.FieldName == "Statuss")
                        {
                            e.Value = "";
                            e.Value = GetSubclinicalsStatus((int)data.Status);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetSubclinicalsStatus(int status)
        {
            string result = null;
            switch (status)
            {
                case 1:
                    result = "Chưa thực hiện";
                    break;
                case 2:
                    result = "Đang thực hiện";
                    break;
                case 3:
                    result = "Đã có kết quả";
                    break;
                default:
                    break;

            }
            return result;
        }
    }
}
