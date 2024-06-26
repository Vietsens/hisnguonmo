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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.Resources;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using MOS.SDO;
using HIS.Desktop.Controls.Session;
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2 : HIS.Desktop.Utility.FormBase
    {
        #region ----ObjCurrent-----
        private V_HIS_SERVICE_REQ currentServiceReq { get; set; }
        private HIS_KSK_GENERAL currentKskGeneral { get; set; }
        private HIS_KSK_OVER_EIGHTEEN currentKskOverEight { get; set; }
        private HIS_KSK_UNDER_EIGHTEEN currentKskUnderEight { get; set; }
        private HIS_KSK_PERIOD_DRIVER currentKskPeriodDriver { get; set; }
        private HIS_KSK_DRIVER_CAR currentKskDriverCar { get; set; }
        private HIS_KSK_OTHER currentKskOther { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_PERIOD_DRIVER_DITY> lstDataDriverDity { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_KSK_UNEI_VATY> lstDataUneiVaty { get; set; }
        #endregion

        Inventec.Desktop.Common.Modules.Module currentModule;
        public frmEnterKskInfomantionVer2(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ hisServiceReq)
        {
            InitializeComponent();
            try
            {
                this.currentServiceReq = hisServiceReq;
                this.currentModule = moduleData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmEnterKskInfomantionVer2_Load(object sender, System.EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ShowInformationPatient();
                FillDataToPages();
                SetTabDefault();
                WaitingManager.Hide();
            }
            catch (System.Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTabDefault()
        {
            try
            {
                bool isActive = false;
                if (currentKskGeneral != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 0;
                    isActive = true;
                }
                else if (currentKskOverEight != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 1;
                    isActive = true;
                }
                else if (currentKskUnderEight != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 2;
                    isActive = true;
                }
                else if (currentKskPeriodDriver != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 3;
                    isActive = true;
                }
                else if (currentKskDriverCar != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 4;
                    isActive = true;
                }
                else if (currentKskOther != null)
                {
                    xtraTabControl1.SelectedTabPageIndex = 5;
                    isActive = true;
                }
                btnPrint.Enabled = isActive;
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowInformationPatient()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    txtServiceCode.Text = currentServiceReq.SERVICE_REQ_CODE;
                    txtTreatmentCode.Text = currentServiceReq.TDL_TREATMENT_CODE;
                    txtPatientCode.Text = currentServiceReq.TDL_PATIENT_CODE;
                    txtPatientName.Text = currentServiceReq.TDL_PATIENT_NAME;
                    txtGender.Text = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                    txtPatientDob.Text = currentServiceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB != (short?)1 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentServiceReq.TDL_PATIENT_DOB) : currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    txtInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentServiceReq.INTRUCTION_TIME);
                    if (currentServiceReq.TDL_KSK_CONTRACT_ID != null && currentServiceReq.TDL_KSK_CONTRACT_ID > 0)
                    {
                        CommonParam param = new CommonParam();
                        HisKskContractFilter filter = new HisKskContractFilter();
                        filter.ID = currentServiceReq.TDL_KSK_CONTRACT_ID;
                        var dataKskContract = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>>("api/HisKskContract/Get", ApiConsumers.MosConsumer, filter, param).SingleOrDefault();
                        txtKskContract.Text = dataKskContract.KSK_CONTRACT_CODE;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToPages()
        {
            try
            {
                FillDataPageGenaral();
                FillDataPageOverEighteen();
                FillDataPageUnderEighteen();
                FillDataPageDriverCar();
                FillDataPagePeriodDriver();
                FillDataPageKSKOther();
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataCboRank(DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("HEALTH_EXAM_RANK_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEALTH_EXAM_RANK_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillNoteBMI(DevExpress.XtraEditors.SpinEdit spinHeight, DevExpress.XtraEditors.SpinEdit spinWeight, System.Windows.Forms.Label txtBMI)
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value != null && spinHeight.Value != 0)
                {
                    bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));
                }
                string displayBMI = Math.Round(bmi, 2) + "";
                if (bmi < 16)
                {
                    displayBMI += " (Gầy độ III)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (16 <= bmi && bmi < 17)
                {
                    displayBMI += " (Gầy độ II)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.SKINNY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (17 <= bmi && bmi < (decimal)18.5)
                {
                    displayBMI += " (Gầy độ I)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.UCDHST.SKINNY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if ((decimal)18.5 <= bmi && bmi < 25)
                {
                    displayBMI += " (Bình thường)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.NORMAL", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (25 <= bmi && bmi < 30)
                {
                    displayBMI += " (Thừa cân)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OVERWEIGHT", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (30 <= bmi && bmi < 35)
                {
                    displayBMI += " (Béo phì độ I)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.I", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (35 <= bmi && bmi < 40)
                {
                    displayBMI += " (Béo phì độ II)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.II", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else if (40 < bmi)
                {
                    displayBMI += " (Béo phì độ III)";
                    //Inventec.Common.Resource.Get.Value("frmEnterKskInfomantion.BMIDISPLAY.OBESITY.III", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                txtBMI.Text = displayBMI;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                HisServiceReqKskExecuteV2SDO sdo = new HisServiceReqKskExecuteV2SDO();
                sdo.ServiceReqId = currentServiceReq.ID;
                sdo.RequestRoomId = currentModule.RoomId;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    sdo.KskGeneral = new KskGeneralV2SDO();
                    sdo.KskGeneral.HisKskGeneral = new HIS_KSK_GENERAL();
                    sdo.KskGeneral.HisKskGeneral = GetValueGeneral();
                    sdo.KskGeneral.HisDhst = new HIS_DHST();
                    sdo.KskGeneral.HisDhst = GetValueDhstGeneral();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    sdo.KskOverEighteen = new KskOverEighteenV2SDO();
                    sdo.KskOverEighteen.HisKskOverEighteen = new HIS_KSK_OVER_EIGHTEEN();
                    sdo.KskOverEighteen.HisKskOverEighteen = GetValueOverEighteen();
                    sdo.KskOverEighteen.HisDhst = new HIS_DHST();
                    sdo.KskOverEighteen.HisDhst = GetDhstOverighteen();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    sdo.KskUnderEighteen = new KskUnderEighteenV2SDO();
                    sdo.KskUnderEighteen.HisKskUnderEighteen = new HIS_KSK_UNDER_EIGHTEEN();
                    sdo.KskUnderEighteen.HisKskUnderEighteen = GetValueUnderEighteen();
                    sdo.KskUnderEighteen.HisDhst = new HIS_DHST();
                    sdo.KskUnderEighteen.HisDhst = GetDhstUnderEighteen();
                    sdo.KskUnderEighteen.HisKskUneiVatys = new System.Collections.Generic.List<HIS_KSK_UNEI_VATY>();
                    sdo.KskUnderEighteen.HisKskUneiVatys = GetUneiVaty();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    sdo.KskPeriodDriver = new KskPeriodDriverV2SDO();
                    sdo.KskPeriodDriver.HisKskPeriodDriver = new HIS_KSK_PERIOD_DRIVER();
                    sdo.KskPeriodDriver.HisKskPeriodDriver = GetValuePeriodDriver();
                    sdo.KskPeriodDriver.HisPeriodDriverDitys = new System.Collections.Generic.List<HIS_PERIOD_DRIVER_DITY>();
                    sdo.KskPeriodDriver.HisPeriodDriverDitys = GetDriverDity();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    sdo.HisKskDriverCar = new HIS_KSK_DRIVER_CAR();
                    sdo.HisKskDriverCar = GetValueDriverCar();
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 5)
                {
                    sdo.HisKskOther = GetValueKSKOther();
                }
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug("INPUT DATA:__api/HisServiceReq/KskExecuteV2 " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                KskExecuteResultV2SDO result = new BackendAdapter(param).Post<KskExecuteResultV2SDO>("api/HisServiceReq/KskExecuteV2", ApiConsumers.MosConsumer, sdo, param);
                if (result != null)
                {
                    success = true;
                    currentKskDriverCar = result.HisKskDriverCar;
                    currentKskGeneral = result.HisKskGeneral;
                    currentKskOverEight = result.HisKskOverEighteen;
                    currentKskPeriodDriver = result.HisKskPeriodDriver;
                    currentKskUnderEight = result.HisKskUnderEighteen;
                    currentKskOther = result.HisKskOther;
                    currentServiceReq = result.HisServiceReq;
                    btnPrint.Enabled = true;
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (System.Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                bool IsEnable = false;
                btnSave.Enabled = true;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    if (currentKskGeneral != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    if (currentKskOverEight != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    if (currentKskUnderEight != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    if (currentKskPeriodDriver != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    if (currentKskDriverCar != null)
                        IsEnable = true;
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 5)
                {
                    if (currentKskOther != null)
                    {
                        IsEnable = true;
                    }
                    if (chkKSKType1.Checked || chkKSKType2.Checked)
                    {
                        btnSave.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                    }
                }
                btnPrint.Enabled = IsEnable;
            }
            catch (System.Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled)
                    return;
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    if (currentKskGeneral != null)
                        PrintProcess(PRINT_TYPE.MPS000315);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    if (currentKskOverEight != null)
                        PrintProcess(PRINT_TYPE.MPS000452);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 2)
                {
                    if (currentKskUnderEight != null)
                        PrintProcess(PRINT_TYPE.MPS000453);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 3)
                {
                    if (currentKskPeriodDriver != null)
                        PrintProcess(PRINT_TYPE.MPS000454);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 4)
                {
                    if (currentKskDriverCar != null)
                        PrintProcess(PRINT_TYPE.MPS000455);
                }
                else if (xtraTabControl1.SelectedTabPageIndex == 5)
                {
                    if (currentKskOther != null)
                        PrintProcess(PRINT_TYPE.MPS000464);
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKSKType1_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (chkKSKType1.Checked || chkKSKType2.Checked)
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKSKType2_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (chkKSKType1.Checked || chkKSKType2.Checked)
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }






    }
}
