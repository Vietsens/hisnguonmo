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
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using System.Drawing.Drawing2D;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;


namespace HIS.Desktop.Plugins.KioskInformation
{
    public partial class frmGreetingScreen : FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        string heinMediOrgCode;
        private Inventec.Desktop.Common.Modules.Module module;
        int countTimeHideError = 0;
        int countTimeChangeWallpaper = 0;
        KioskInformationSDO currentData = new KioskInformationSDO();
        Graphics graphics;

        List<Image> listImage;
        Image image;

        CommonParam paraCommon = new CommonParam(); // using Inventec.Core;
        public frmGreetingScreen(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                WaitingManager.Show();
                this.module = currentModule;
                this.Text = currentModule.text;// set tên hiển thị trên form
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmGreetingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                timerWallPaper.Interval = 1000;
                RegisterTimer(GetModuleLink(), "timerWallPaper", timerWallPaper.Interval, timerWallPaper_Tick);
                //WaitingManager.Show();
                timer.Stop();

                //Lấy ảnh từ thư mục trong HIS.Desktop
                getImageFromFile();

                //Load ảnh mặc định lên form 
                if (listImage != null && listImage.Count > 0)
                {
                    image = listImage.First();
                    this.layoutControlGroup2.BackgroundImage = image;
                }
                //Thời gian đổi hình nền


                StartTimer(GetModuleLink(), "timerWallPaper");

                this.SetControlValue(); // lấy tên chi nhánh
                labelError.Visible = false;
                this.lciError.Padding = new DevExpress.XtraLayout.Utils.Padding(this.Width * 470 / 1300, this.Width * 470 / 1300, 20, 20);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                StopTimer(GetModuleLink(), "timerWallPaper");
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


        private void getImageFromFile()
        {
            try
            {
                if (listImage != null && listImage.Count > 0) return;
                listImage = new List<Image>();
                //URL thư mục đã được cấu hình
                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_ICD_CM.xlsx");
                if (System.IO.Directory.Exists(Application.StartupPath + "\\Img\\Background_Image_Kiosk_Information\\"))
                //if (System.IO.Directory.Exists(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK))
                {
                    string[] fileEntries = System.IO.Directory.GetFiles(Application.StartupPath + "\\Img\\Background_Image_Kiosk_Information\\").OrderBy(f => f).ToArray();

                    if (fileEntries == null || fileEntries.Count() == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong ton tai file anh trong thu muc. Path = " + Application.StartupPath + "\\Img\\Background_Image_Kiosk_Information\\" + ". " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
                    }
                    else
                    {
                        foreach (var item in fileEntries)
                        {
                            listImage.Add(Image.FromFile(item));
                        }
                    }
                }

                else
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong ton tai thu muc: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK), Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetControlValue()
        {
            try
            {
                var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(o => o.ID == HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId()).SingleOrDefault();
                string branchName = branch.BRANCH_NAME ?? "";
                lblBranchName.Text += branchName.ToUpper();
                txtTreatmentCode.Text = "";
                txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void FillDataToInformScreen()
        {
            try
            {
                timer.Stop();
                labelError.Visible = false;
                lciError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                string idUser = txtTreatmentCode.Text;
                CommonParam paramCommon = new CommonParam();
                List<KioskInformationSDO> lstKioskInform = new BackendAdapter(paraCommon).Get<List<MOS.SDO.KioskInformationSDO>>("api/HisTreatment/GetKioskInformation", ApiConsumers.MosConsumer, idUser, paraCommon);
                if ((lstKioskInform == null) || (lstKioskInform.Count() == 0))
                {
                    StartTimer(GetModuleLink(), "timerWallPaper");
                    countTimeHideError = 0;
                    lciError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    labelError.Visible = true;
                    txtTreatmentCode.Focus();
                }
                else
                {
                    if (lstKioskInform.Count == 1)
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.Text = "";
                        frmGetInforationScreen formScreen = new frmGetInforationScreen(module, lstKioskInform.FirstOrDefault(), image);
                        formScreen.ShowDialog();
                        //this.Show();

                    }
                    else
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.Text = "";
                        //this.Hide();
                        //panelError_Select.Visible = true;
                        //gridControlSelectProFile.Visible = true;
                        //gridControlSelectProFile.DataSource = lstKioskInform;
                        frmSelectProfile formScreen = new frmSelectProfile(module, lstKioskInform, listImage, countTimeChangeWallpaper, OpenFormPatient);
                        formScreen.ShowDialog();
                        formScreen.Location = new Point(formScreen.Location.X, formScreen.Location.Y + 50);
                        //this.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenFormPatient(object[] obj)
        {
            try
            {
                frmGetInforationScreen frmGetInforationScreen = new frmGetInforationScreen(this.module, obj[0] as KioskInformationSDO, obj[1] as Image);
                frmGetInforationScreen.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnAgree_Click(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                if (txtTreatmentCode.Text != "")
                {
                    if (!btnAgree.Enabled) return;
                    FillDataToInformScreen();
                }
                else txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //WaitingManager.Hide();
            //nếu không nhập kí tự vào thì hiện thông báo yêu cầu nhập số
            //nếu nhập dãy kí tự vào thì kiểm tra
        }
        private void txtTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) //gọi sự kiện Click
                {
                    e.Handled = true;// bỏ qua xử lý khác
                    btnAgree_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            try
            {

                if (timer.Enabled)
                {
                    countTimeHideError++;
                    if (countTimeHideError == 60)
                    {
                        timer.Stop();
                        labelError.Visible = false;
                        txtTreatmentCode.Text = "";
                        txtTreatmentCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerWallPaper_Tick()
        {

            try
            {

                if (timerWallPaper.Enabled)
                {
                    countTimeChangeWallpaper++;
                    if (countTimeChangeWallpaper % 10 == 0)
                    {
                        if (listImage != null && listImage.Count > 0)
                        {
                            if (countTimeChangeWallpaper / 10 == listImage.Count)
                            {
                                countTimeChangeWallpaper = 0;
                            }

                            image = listImage[countTimeChangeWallpaper / 10];
                            this.layoutControlGroup2.BackgroundImage = image;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSelectProFile_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound) // đưa kiểu dữ liệu về kiểu khác hoặc Object
                {
                    var data = (KioskInformationSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "DobStr")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.Dob);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
