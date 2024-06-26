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
using AutoMapper;
using DevExpress.XtraTab;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.DHST;
using HIS.UC.DHST.ADO;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.ExamServiceReqResult.Run
{
    public partial class frmExamServiceReqResult : HIS.Desktop.Utility.FormBase
    {
        UserControl ucControlDHST;
        internal DHSTProcessor dhstProcessor;

        IcdProcessor icdProcessor;
        UserControl ucIcd;

        long sereServId;

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentHisVExamServiceReq = null;
        internal List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServs = null;

        public frmExamServiceReqResult()
        {
            InitializeComponent();
        }

        public frmExamServiceReqResult(Inventec.Desktop.Common.Modules.Module currentModule, long sereServId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                SetIconFrm();
                this.currentModule = currentModule;
                this.sereServId = sereServId;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                InitUCDHST();
                InitUcIcd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmExamServiceReqResult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                LoadServiceReqBySereServId();

                FillDataToControlExamServiceReq(this);

                if (ucIcd != null)
                {
                    icdProcessor.ReadOnly(ucIcd, true);
                }

                if (ucControlDHST != null)
                {
                    dhstProcessor.IsReadOnly(ucControlDHST, true);
                }

                SetTabPageVisible(xtraTabControl1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTabPageVisible(XtraTabControl tab)
        {
            long key = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__EXAM_SERVICE_REQ_EXCUTE_HIDE_TABS_INFOMATION__APPLICATION); ;
            if (key == 1)
            {
                for (int i = 0; i < tab.TabPages.Count; i++)
                    if (tab.TabPages[i].Name == "xtraTabPageChung")
                    {
                        tab.TabPages[i].Show();
                        tab.TabPages[i].PageVisible = true;
                        tab.SelectedTabPage = tab.TabPages[i];
                    }
                    else
                    {
                        tab.TabPages[i].PageVisible = false;
                    }
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExamServiceReqResult.Resources.Lang", typeof(HIS.Desktop.Plugins.ExamServiceReqResult.Run.frmExamServiceReqResult).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultNoteưewd.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlICD.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControlICD.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxDHST.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.groupBoxDHST.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlDHST.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControlDHST.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNextTreatmentInstruction.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciResultNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabControl1.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageChung.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage7.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage8.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage9.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage10.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage11.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage12.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage13.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage14.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage15.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage16.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamInfomation.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciExamInfomation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKhamToanThan.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciKhamToanThan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclude.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciConclude.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdExtraName.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciIcdExtraName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultNote.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHospitalizationReason.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciHospitalizationReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNgayThuCuaBenh.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciNgayThuCuaBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathologicalProcess.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciPathologicalProcess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathologicalHistory.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciPathologicalHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPathologicalHistoryFamily.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciPathologicalHistoryFamily.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOfBenh.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.lciOfBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKhamToanThan.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmExamServiceReqResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCDHST()
        {
            try
            {
                dhstProcessor = new DHSTProcessor();
                DHSTInitADO ado = new DHSTInitADO();
                this.ucControlDHST = (UserControl)dhstProcessor.Run(ado);
                if (this.ucControlDHST != null)
                {
                    this.layoutControlDHST.Controls.Add(this.ucControlDHST);
                    this.ucControlDHST.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                //ado.txtNextFocus = txtIcdText;
                ado.Width = 550;
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.layoutControlICD.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceReqBySereServId()
        {
            try
            {
                if (this.sereServId > 0)
                {
                    CommonParam param = new CommonParam();
                    sereServs = new List<HIS_SERE_SERV>();
                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.ID = this.sereServId;
                    sereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                    if (sereServs != null && sereServs.Count > 0)
                    {
                        currentHisVExamServiceReq = new HIS_SERVICE_REQ();
                        MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                        filter.ID = sereServs[0].SERVICE_REQ_ID;
                        currentHisVExamServiceReq = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlExamServiceReq(frmExamServiceReqResult control)
        {
            try
            {
                if (control.currentHisVExamServiceReq != null)
                {
                    control.txtHospitalizationReason.Text = control.currentHisVExamServiceReq.HOSPITALIZATION_REASON;
                    control.spinNgayThuCuaBenh.EditValue = control.currentHisVExamServiceReq.SICK_DAY;//TODO
                    control.txtPathologicalProcess.Text = control.currentHisVExamServiceReq.PATHOLOGICAL_PROCESS;
                    control.txtPathologicalHistory.Text = control.currentHisVExamServiceReq.PATHOLOGICAL_HISTORY;
                    control.txtPathologicalHistoryFamily.Text = control.currentHisVExamServiceReq.PATHOLOGICAL_HISTORY_FAMILY;
                    control.txtKhamToanThan.Text = control.currentHisVExamServiceReq.FULL_EXAM;
                    control.txtKhamBoPhan.Text = control.currentHisVExamServiceReq.PART_EXAM;
                    control.txtTuanHoan.Text = control.currentHisVExamServiceReq.PART_EXAM_CIRCULATION;
                    control.txtHoHap.Text = control.currentHisVExamServiceReq.PART_EXAM_RESPIRATORY;
                    control.txtTieuHoa.Text = control.currentHisVExamServiceReq.PART_EXAM_DIGESTION;
                    control.txtThanTietNieu.Text = control.currentHisVExamServiceReq.PART_EXAM_KIDNEY_UROLOGY;
                    control.txtThanKinh.Text = control.currentHisVExamServiceReq.PART_EXAM_NEUROLOGICAL;
                    control.txtCoXuongKhop.Text = control.currentHisVExamServiceReq.PART_EXAM_MUSCLE_BONE;
                    control.txtTMH.Text = control.currentHisVExamServiceReq.PART_EXAM_ENT;
                    control.txtRHM.Text = control.currentHisVExamServiceReq.PART_EXAM_STOMATOLOGY;
                    control.txtMat.Text = control.currentHisVExamServiceReq.PART_EXAM_EYE;
                    control.txtNoiTiet.Text = control.currentHisVExamServiceReq.PART_EXAM_OEND;
                    control.txtTamThan.Text = control.currentHisVExamServiceReq.PART_EXAM_MENTAL;
                    control.txtDinhDuong.Text = control.currentHisVExamServiceReq.PART_EXAM_NUTRITION;
                    control.txtVanDong.Text = control.currentHisVExamServiceReq.PART_EXAM_MOTION;
                    control.txtSanPhuKhoa.Text = control.currentHisVExamServiceReq.PART_EXAM_OBSTETRIC;
                    control.txtNextTreatmentInstruction.Text = control.currentHisVExamServiceReq.NEXT_TREATMENT_INSTRUCTION;
                    control.txtResultNote.Text = control.currentHisVExamServiceReq.NOTE;
                    control.txtDescription.Text = control.currentHisVExamServiceReq.SUBCLINICAL;
                    control.txtConclude.Text = control.currentHisVExamServiceReq.TREATMENT_INSTRUCTION;


                    IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = control.currentHisVExamServiceReq.ICD_CODE;
                    icd.ICD_NAME = control.currentHisVExamServiceReq.ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    control.txtIcdExtraName.Text = control.currentHisVExamServiceReq.ICD_TEXT;
                    control.txtIcdExtraCode.Text = control.currentHisVExamServiceReq.ICD_SUB_CODE;

                    if (control.currentHisVExamServiceReq.DHST_ID.HasValue)
                    {
                        MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                        dhstFilter.ID = control.currentHisVExamServiceReq.DHST_ID;
                        HIS_DHST currentDhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, new CommonParam()).FirstOrDefault();

                        control.dhstProcessor.SetValue(control.ucControlDHST, currentDhst);
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
