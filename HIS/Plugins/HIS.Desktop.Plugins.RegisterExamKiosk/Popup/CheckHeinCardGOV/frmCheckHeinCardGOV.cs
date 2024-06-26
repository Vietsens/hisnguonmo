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
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.InformationObject;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.CheckHeinCardGOV
{
    public partial class frmCheckHeinCardGOV : Form
    {
        ResultHistoryLDO resultHistoryLDO = null;
        HIS.Desktop.Common.DelegateSelectData registerData;
        HIS.Desktop.Common.DelegateSelectData registerDataInformation;
        HIS.Desktop.Common.DelegateRefreshData setNull;
        List<HIS_MEDI_ORG> listMediOrgs;

        HisPatientForKioskSDO PatientForKiosk;

        DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        List<HIS_PATIENT_TYPE> hisPatientType;

        private bool stopThread;

        public frmCheckHeinCardGOV(ResultHistoryLDO resultHistoryLDO, HIS.Desktop.Common.DelegateSelectData _registerDataInformation, HIS.Desktop.Common.DelegateSelectData _registerData, HIS.Desktop.Common.DelegateRefreshData _setNull, DelegateCloseForm_Uc closingForm, HisPatientForKioskSDO _PatientForKiosk)
        {
            InitializeComponent();
            try
            {
                this.resultHistoryLDO = resultHistoryLDO;
                this.registerDataInformation = _registerDataInformation;
                this.registerData = _registerData;
                this.setNull = _setNull;
                this.PatientForKiosk = _PatientForKiosk;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.DelegateClose = closingForm;

                CloseThread = new System.Threading.Thread(ClosingForm);
                CloseThread.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckHeinCardGOV_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                listMediOrgs = new List<HIS_MEDI_ORG>();
                listMediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>();
                //AddControlToLayoutControlGroup3();
                InitButton();
                stopThread = true;
                LoadInfo();
                LoadDataGridControl();
                //this.TopMost = true;
                stopThread = false;
                ResetLoopCount();
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                SetCaptionByLanguageKey();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterExamKiosk.Resources.Lang", typeof(HIS.Desktop.Plugins.RegisterExamKiosk.Popup.CheckHeinCardGOV.frmCheckHeinCardGOV).Assembly);
                this.Text = Inventec.Common.Resource.Get.Value("frmCheckHeinCardGOV.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitButton()
        {
            try
            {
                hisPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.IS_NOT_FOR_KIOSK != 1 && o.IS_NOT_USE_FOR_PATIENT != 1 && o.IS_NOT_USE_FOR_PAYMENT != 1).OrderByDescending(o => o.ID).ToList();
                this.layoutControlGroup2.BeginUpdate();
                var groupItems = new List<BaseLayoutItem>();
                int dem = 0;
                for (int i = 0; i < hisPatientType.Count; i++)
                {
                    dem += 1;
                    Button btnSinglePrint = new Button();
                    if (hisPatientType[i].PATIENT_TYPE_NAME.ToUpper().Trim().StartsWith("KHÁM"))
                    {
                        btnSinglePrint.Text = hisPatientType[i].PATIENT_TYPE_NAME.ToUpper();
                    }
                    else
                    {
                        btnSinglePrint.Text = "KHÁM " + hisPatientType[i].PATIENT_TYPE_NAME.ToUpper();
                    }

                    btnSinglePrint.BackColor = Color.Teal;
                    btnSinglePrint.Font = new Font("Microsoft Sans Serif", 17, FontStyle.Regular);
                    btnSinglePrint.Click += new System.EventHandler(this.btnCustomBurtton_Click);
                    btnSinglePrint.Tag = hisPatientType[i].ID;
                    btnSinglePrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnSinglePrint.ForeColor = Color.White;

                    //btnSinglePrint.Size = btnSinglePrint.CalcBestFit(btnSinglePrint.CreateGraphics());
                    btnSinglePrint.Dock = System.Windows.Forms.DockStyle.Fill;
                    btnSinglePrint.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
                    //btnSinglePrint.AutoWidthInLayoutControl = true;

                    var itembtnSinglePrintTemp = new LayoutControlItem
                    {
                        Control = btnSinglePrint,
                        Name = String.Format("lcibtnSinglePrintTemp{0}", dem),
                        TextVisible = false,
                        //SizeConstraintsType = SizeConstraintsType.Custom,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0),
                    };

                    groupItems.Add(itembtnSinglePrintTemp);


                    layoutControlGroup2.Add(itembtnSinglePrintTemp);
                    //mainLayoutControlGroup.Add(splitter);
                    if (groupItems != null && groupItems.Count > 0)
                    {
                        itembtnSinglePrintTemp.Move(layoutControlItem15, DevExpress.XtraLayout.Utils.InsertType.Left);
                    }
                }
                //int y = 0;
                foreach (LayoutControlItem item in groupItems)
                {
                    //item.Move(layoutControlItem15, DevExpress.XtraLayout.Utils.InsertType.Left);
                    if (item.Control != null && !String.IsNullOrWhiteSpace(item.Control.Text))
                    {
                        item.SizeConstraintsType = SizeConstraintsType.Custom;
                        item.MinSize = new System.Drawing.Size(180, 100);
                        item.MaxSize = new System.Drawing.Size(200, 100);
                    }
                    else
                    {
                        item.SizeConstraintsType = SizeConstraintsType.Default;
                    }
                }

                foreach (LayoutControlItem item in layoutControlGroup2.Items)
                {
                    Inventec.Common.Logging.LogSystem.Debug(string.Format("layoutControlGroup2 {2} {0} - {1}", item.Location.X, item.Location.Y, item.Text));
                }

                this.layoutControlGroup2.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCustomBurtton_Click(object sender, EventArgs e)
        {
            try
            {
                ResetLoopCount();
                var data = sender as Button;
                if ((long)data.Tag != 1)
                {
                    this.Close();
                    if (this.registerData != null)
                        this.registerData(data.Tag);
                }
                else
                {
                    var currentBranch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());

                    if (!String.IsNullOrWhiteSpace(resultHistoryLDO.maDKBD)
                        && (resultHistoryLDO.maDKBD == currentBranch.HEIN_MEDI_ORG_CODE || ValidSysMediOrgCode(currentBranch.SYS_MEDI_ORG_CODE, resultHistoryLDO.maDKBD) || ValidSysMediOrgCode(currentBranch.ACCEPT_HEIN_MEDI_ORG_CODE, resultHistoryLDO.maDKBD)))
                    {
                        HisExamRegisterKioskSDO sdoData = new HisExamRegisterKioskSDO();
                        sdoData.RightRouteCode = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                        this.Close();
                        if (this.registerDataInformation != null)
                            this.registerDataInformation(sdoData);
                        if (this.registerData != null)
                            this.registerData(data.Tag);
                    }
                    else
                    {
                        frmInformationObject frm = new frmInformationObject(this.PatientForKiosk, resultHistoryLDO, SelectedPatientType, SelectedInformationData);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidSysMediOrgCode(string sysMediOrgCode, string maDKBD)
        {
            bool isValidSysMediOrgCode = false;
            try
            {
                if (String.IsNullOrWhiteSpace(sysMediOrgCode) || String.IsNullOrWhiteSpace(maDKBD))
                    return false;
                string[] listSysMediOrgCode = sysMediOrgCode.Split(',') ?? new string[] { };
                foreach (var item in listSysMediOrgCode)
                {
                    if (resultHistoryLDO.maDKBD.Trim() == item.Trim())
                    {
                        isValidSysMediOrgCode = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValidSysMediOrgCode;
        }

        private void SelectedPatientType(object patienType)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patienType), patienType));
                if (patienType != null && patienType.GetType() == typeof(long))
                {
                    this.Close();
                    if (this.registerData != null)
                        this.registerData((long)patienType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectedInformationData(object data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                if (data != null && data.GetType() == typeof(HisExamRegisterKioskSDO))
                {
                    if (this.registerDataInformation != null)
                        this.registerDataInformation((HisExamRegisterKioskSDO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInfo()
        {
            try
            {
                lblCoQuanBHXH.Text = resultHistoryLDO.cqBHXH;
                lblDiaChi.Text = resultHistoryLDO.diaChi;
                lblGiaTriTheDen.Text = resultHistoryLDO.gtTheDen;
                lblGiaTriTheTu.Text = resultHistoryLDO.gtTheTu;
                lblGioiTinh.Text = resultHistoryLDO.gioiTinh;
                lblHoTen.Text = resultHistoryLDO.hoTen;
                lblMaDKBD.Text = resultHistoryLDO.maDKBD;
                lblMaKhuVuc.Text = resultHistoryLDO.maKV;
                lblNgayDu5Nam.Text = resultHistoryLDO.ngayDu5Nam;
                lblMaKetQua.Text = resultHistoryLDO.maKetQua == "000" ? "Hợp lệ" : "Không hợp lệ";

                var medi = listMediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE == resultHistoryLDO.maDKBD);
                if (medi != null)
                {
                    lblMediOrgName.Text = medi.MEDI_ORG_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataGridControl()
        {
            try
            {
                gridControlHistory.BeginUpdate();
                gridControlHistory.DataSource = resultHistoryLDO.dsLichSuKCB2018;
                gridControlHistory.EndUpdate();
            }
            catch (Exception ex)
            {
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

        private void gridViewHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ExamHistoryLDO data = (ExamHistoryLDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "tinhTrang_str")
                        {
                            if (data.tinhTrang == "1")
                                e.Value = "Ra viện";
                            else if (data.tinhTrang == "2")
                                e.Value = "Chuyển viện";
                            else if (data.tinhTrang == "3")
                                e.Value = "Trốn viện";
                            else if (data.tinhTrang == "4")
                                e.Value = "Xin ra viện";
                        }
                        else if (e.Column.FieldName == "kqDieuTri_str")
                        {
                            if (data.kqDieuTri == "1")
                                e.Value = "Khỏi";
                            else if (data.kqDieuTri == "2")
                                e.Value = "Đỡ";
                            else if (data.kqDieuTri == "3")
                                e.Value = "Không thay đổi";
                            else if (data.kqDieuTri == "4")
                                e.Value = "Nặng hơn";
                            else if (data.kqDieuTri == "5")
                                e.Value = "Tử vong";
                        }
                        else if (e.Column.FieldName == "MEDI_ORG_NAME")
                        {
                            var medi = listMediOrgs.FirstOrDefault(o => o.MEDI_ORG_CODE == data.maCSKCB);
                            if (medi != null)
                            {
                                e.Value = medi.MEDI_ORG_NAME;
                            }
                        }
                        else if (e.Column.FieldName == "tuNgay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Int64.Parse(data.ngayVao + "00"));
                        }
                        else if (e.Column.FieldName == "denNgay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Int64.Parse(data.ngayRa + "00"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCheckHeinCardGOV_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.setNull();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCheckHeinCardGOV_KeyDown(object sender, KeyEventArgs e)
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

                    this.Invoke(new MethodInvoker(delegate() { this.Close(); }));
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
