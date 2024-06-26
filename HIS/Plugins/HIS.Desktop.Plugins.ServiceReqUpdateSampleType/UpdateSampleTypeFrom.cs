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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqUpdateSampleType
{
    public partial class UpdateSampleTypeFrom : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private HIS_SERVICE_REQ req;
        private List<HIS_TEST_SAMPLE_TYPE> ListSampleType;
        private Common.RefeshReference RefreshClick;
        private List<V_HIS_SERVICE_REQ> ServiceReqPrintRaws;
        private HisTreatmentWithPatientTypeInfoSDO Treatment;
        private List<V_HIS_SERE_SERV> SereServs;
        private List<V_HIS_BED_LOG> listBedLogs;

        public UpdateSampleTypeFrom(Inventec.Desktop.Common.Modules.Module moduleData, HIS_SERVICE_REQ req, Common.RefeshReference refreshClick)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.req = req;
            this.RefreshClick = refreshClick;
            this.Text = moduleData.text;
        }

        private void UpdateSampleTypeFrom_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();
                CreateThreadLoadDataForPrint();

                InitCombo();
                LoadDataCbo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCbo()
        {
            try
            {
                if (req.TEST_SAMPLE_TYPE_ID.HasValue)
                {
                    CboTestSampleType.EditValue = req.TEST_SAMPLE_TYPE_ID.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                ListSampleType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>();
                if (ListSampleType != null && ListSampleType.Count > 0)
                {
                    ListSampleType = ListSampleType.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                CboTestSampleType.Properties.DataSource = ListSampleType;
                CboTestSampleType.Properties.DisplayMember = "TEST_SAMPLE_TYPE_NAME";
                CboTestSampleType.Properties.ValueMember = "ID";
                CboTestSampleType.Properties.TextEditStyle = TextEditStyles.Standard;
                CboTestSampleType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                CboTestSampleType.Properties.ImmediatePopup = true;
                CboTestSampleType.ForceInitialize();
                CboTestSampleType.Properties.View.Columns.Clear();
                CboTestSampleType.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = CboTestSampleType.Properties.View.Columns.AddField("TEST_SAMPLE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = CboTestSampleType.Properties.View.Columns.AddField("TEST_SAMPLE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceReqUpdateSampleType.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceReqUpdateSampleType.UpdateSampleTypeFrom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UpdateSampleTypeFrom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnPrint.Text = Inventec.Common.Resource.Get.Value("UpdateSampleTypeFrom.BtnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnSave.Text = Inventec.Common.Resource.Get.Value("UpdateSampleTypeFrom.BtnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboTestSampleType.Properties.NullText = Inventec.Common.Resource.Get.Value("UpdateSampleTypeFrom.CboTestSampleType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciTestSampleType.Text = Inventec.Common.Resource.Get.Value("UpdateSampleTypeFrom.LciTestSampleType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciSampleTypeCode.Text = Inventec.Common.Resource.Get.Value("UpdateSampleTypeFrom.LciTestSampleType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTestSampleType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (CboTestSampleType.EditValue != null)
                    {
                        var aService = ListSampleType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboTestSampleType.EditValue ?? 0).ToString()));
                        if (aService != null)
                        {
                            TxtTestSampleTypeCode.Text = aService.TEST_SAMPLE_TYPE_CODE;
                            BtnSave.Focus();
                        }
                        else
                            CboTestSampleType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTestSampleType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboTestSampleType.EditValue != null)
                    {
                        var aService = ListSampleType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((CboTestSampleType.EditValue ?? 0).ToString()));
                        if (aService != null)
                        {
                            TxtTestSampleTypeCode.Text = aService.TEST_SAMPLE_TYPE_CODE;
                            BtnSave.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            CboTestSampleType.Focus();
                            CboTestSampleType.ShowPopup();
                        }
                    }
                    else
                    {
                        CboTestSampleType.Focus();
                        CboTestSampleType.ShowPopup();
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    CboTestSampleType.Focus();
                    CboTestSampleType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (req != null)
                {
                    LoadReqView();

                    HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                    HisServiceReqSDO.SereServs = SereServs;
                    HisServiceReqSDO.ServiceReqs = ServiceReqPrintRaws;

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, Treatment, listBedLogs, moduleData != null ? moduleData.RoomId : 0);
                    PrintServiceReqProcessor.Print("Mps000026", false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                long? sampleId = null;
                if (CboTestSampleType.EditValue != null)
                {
                    sampleId = Inventec.Common.TypeConvert.Parse.ToInt64((CboTestSampleType.EditValue ?? 0).ToString());
                }

                CommonParam param = new CommonParam();
                bool success = false;

                req.TEST_SAMPLE_TYPE_ID = sampleId;

                var apiResult = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("/api/HisServiceReq/UpdateSampleType", ApiConsumer.ApiConsumers.MosConsumer, req, SessionManager.ActionLostToken, param);
                if (apiResult != null)
                {
                    success = true;
                    req = apiResult;
                    if (RefreshClick != null)
                    {
                        RefreshClick();
                    }

                    BtnPrint.Focus();
                }

                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTestSampleType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboTestSampleType.EditValue = null;
                    BtnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadDataForPrint()
        {
            Thread sereServ = new Thread(LoadSereServ);
            Thread treat = new Thread(LoadTreatment);
            //Thread reqview = new Thread(LoadReqView);
            Thread bed = new Thread(LoadBedLog);
            try
            {
                sereServ.Start();
                treat.Start();
                //reqview.Start();
                bed.Start();
            }
            catch (Exception ex)
            {
                sereServ.Abort();
                treat.Abort();
                //reqview.Abort();
                bed.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServ()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.SERVICE_REQ_ID = req.ID;
                SereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = req.TREATMENT_ID;
                filter.INTRUCTION_TIME = req.INTRUCTION_TIME;
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumer.ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                if (hisTreatments != null && hisTreatments.Count > 0)
                {
                    Treatment = hisTreatments.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadReqView()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = req.ID;
                ServiceReqPrintRaws = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBedLog()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = req.TREATMENT_ID;
                listBedLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTestSampleTypeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(TxtTestSampleTypeCode.Text))
                    {
                        string code = TxtTestSampleTypeCode.Text.Trim();
                        var listData = ListSampleType.Where(o => o.TEST_SAMPLE_TYPE_CODE.Equals(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.TEST_SAMPLE_TYPE_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count == 1)
                        {
                            showCbo = false;
                            CboTestSampleType.EditValue = result.First().ID;
                            CboTestSampleType.Properties.Buttons[1].Visible = true;
                            BtnSave.Focus();
                            e.Handled = true;
                        }
                    }
                    if (showCbo)
                    {
                        CboTestSampleType.Focus();
                        CboTestSampleType.ShowPopup();
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
