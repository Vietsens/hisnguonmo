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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggrExam.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
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

namespace HIS.Desktop.Plugins.ExpMestAggrExam
{
    public partial class UCExpMestAggrExam : HIS.Desktop.Utility.UserControlBase
    {
        #region Variable
        List<long> list_exp_mest_id;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        int rowCount = 0;
        int dataTotal = 0;
        long aggrExpMestId;
        int rowCountExpM = 0;
        int dataTotalExpM = 0;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_EXP_MEST currentAggrExpMest { get; set; }
        V_HIS_TREATMENT_4 treatment;

        bool isCheckAll = true;
        #endregion

        public UCExpMestAggrExam()
        {
            InitializeComponent();
        }

        public UCExpMestAggrExam(Inventec.Desktop.Common.Modules.Module currentModule, V_HIS_TREATMENT_4 _treatment)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatment = _treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestAggregate_Load(object sender, EventArgs e)
        {
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();

                SetCaptionByLanguageKey();

                if (this.treatment != null)
                {
                    LoadDataToTreatmentInfo(this.treatment);
                }
                else
                {
                    LoadDataToTreatmentInfo(null);
                }


                this.gridControlAggrExpMest.ToolTipController = this.toolTipController1;
                this.gridControlAggregateRequest.ToolTipController = this.toolTipController1;
                this.gridControlExpMestReq.ToolTipController = this.toolTipController1;
                chkNotSynthetic.Checked = true;
                chkSynthesized.Checked = false;
                SetDataDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetTreatment()
        {
            try
            {
                MOS.Filter.HisTreatmentView4Filter treatmentFilter = new HisTreatmentView4Filter();

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    treatmentFilter.TREATMENT_CODE__EXACT = code;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        /// Du lieu mac dinh
        /// </summary>
        private void SetDataDefault()
        {
            try
            {

                SetDefaultValue();

                LoadDataExpMestThuocNT();
                LoadDataAggrExpMest();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreatmentInfo(V_HIS_TREATMENT_4 treatment4)
        {
            try
            {
                if (treatment4 != null)
                {
                    txtPatientCode.Text = treatment4.TDL_PATIENT_CODE;
                    txtPatientName.Text = treatment4.TDL_PATIENT_NAME;
                    txtTreatmentCode.Text = treatment4.TREATMENT_CODE;
                }
                else
                {
                    txtPatientCode.Text = "";
                    txtPatientName.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData()
        {
            Thread threadMetyReq = new System.Threading.Thread(LoadDataExpMestThuocNT);
            Thread threadMatyReq = new System.Threading.Thread(LoadDataAggrExpMest);

            threadMetyReq.Priority = ThreadPriority.Normal;
            threadMatyReq.Priority = ThreadPriority.Normal;
            try
            {
                threadMetyReq.Start();
                threadMatyReq.Start();

                threadMetyReq.Join();
                threadMatyReq.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                threadMatyReq.Abort();
            }
        }

        private void LoadDataExpMestThuocNTNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataExpMestThuocNT(); }));
                }
                else
                {
                    LoadDataExpMestThuocNT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataAggrExpMestNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataAggrExpMest(); }));
                }
                else
                {
                    LoadDataAggrExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                // Load thoi gian mac dinh len control datetime

                //chkNotSynthetic.Checked = true;
                //chkSynthesized.Checked = false;
                gridControlAggrExpMest.DataSource = null;
                gridControlDetail.DataSource = null;
                gridControlExpMestReq.DataSource = null;
                gridControlAggregateRequest.DataSource = null;

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpMestAggrExam.Resources.Lang", typeof(HIS.Desktop.Plugins.ExpMestAggrExam.UCExpMestAggrExam).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqRoomName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColExpReqPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqUserTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqUserTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqUserName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColExpReqTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColExpReqPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpDelete.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpDelete.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPrint.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPrint.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpPrint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpStockName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpCreator.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpTimeDisplay.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpTimeDisplay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpUserName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediSTT.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediTypeCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediUnitName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediAmount.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediBidNumber.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExecuteRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColExpTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReqUserName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAggrExpMest.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.btnAggrExpMest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarFilterProcess.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navBarFilterProcess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotSynthetic.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.chkNotSynthetic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSynthesized.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.chkSynthesized.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckSynthesized.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciCheckSynthesized.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciTreatmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckNotSynthetic.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciCheckNotSynthetic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_EXP_MEST data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();

                        //Status
                        long expMestSttId = data.EXP_MEST_STT_ID;
                        //trang: dang gui YC : màu vàng
                        if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu xanh
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet mau den
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestReq_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                gridControlDetail.DataSource = null;
                if (e.Column.FieldName == "DX$CheckboxSelectorColumn" || e.Column.FieldName == "EDIT")
                {
                    return;
                }
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridViewExpMestReq.GetFocusedRow();
                if (row != null)
                {
                    gridControlDetail.DataSource = LoadDataDetailExpMest(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tổng hợp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAggrExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!btnAggrExpMest.Enabled)
                //{
                //    return;
                //}
                //btnAggrExpMest.Enabled = false;
                SelectCheckExpMestAggr();
                ExecuteAggrByListPres();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        List<V_HIS_EXP_MEST> _ExpMestChecks { get; set; }
        /// <summary>
        /// Select Check 
        /// </summary>
        private void SelectCheckExpMestAggr()
        {
            try
            {
                //_ExpMestChecks = new List<V_HIS_EXP_MEST>();
                //if (gridViewExpMestReq.RowCount > 0)
                //{
                //    for (int i = 0; i < gridViewExpMestReq.SelectedRowsCount; i++)
                //    {
                //        if (gridViewExpMestReq.GetSelectedRows()[i] >= 0)
                //        {
                //            _ExpMestChecks.Add((V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(gridViewExpMestReq.GetSelectedRows()[i]));
                //        }
                //    }
                //}
                _ExpMestChecks = new List<V_HIS_EXP_MEST>();
                if (this._ExpMestADOs != null && this._ExpMestADOs.Count > 0)
                {
                    _ExpMestChecks.AddRange(this._ExpMestADOs.Where(p => p.IsCheck == true && p.AGGR_EXP_MEST_ID == null && p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tong hop
        /// </summary>
        private void ExecuteAggrByListPres()
        {
            try
            {
                //WaitingManager.Show();
                this.list_exp_mest_id = new List<long>();
                //Review
                if (this._ExpMestChecks != null && this._ExpMestChecks.Count > 0)
                {
                    this.list_exp_mest_id = this._ExpMestChecks.Select(p => p.ID).ToList();
                }
                else
                {
                    // WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn dịch vụ", "Thông báo");
                    return;
                }
                DelegateReturnSuccess(true);
                // WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Start Tong Hop
        /// </summary>
        /// <param name="data"></param>
        public void DelegateReturnSuccess(object data)
        {
            try
            {
                //Review
                bool success = false;
                CommonParam param = new CommonParam();
                if (data is long)
                    this.list_exp_mest_id.Add((long)data);
                MOS.SDO.HisExpMestAggrSDO expMestSdo = new MOS.SDO.HisExpMestAggrSDO();
                expMestSdo.ExpMestIds = this.list_exp_mest_id;
                expMestSdo.ReqRoomId = this.currentModule.RoomId;
                var hisAggrExpMestCreate = new BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/AggrExamCreate", ApiConsumers.MosConsumer, expMestSdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (hisAggrExpMestCreate != null)
                {
                    success = true;
                    LoadDataExpMestThuocNT();
                    LoadDataAggrExpMest();
                }
                else
                    success = false;
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlAggrExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                //Review
                gridControlAggregateRequest.DataSource = null;
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridViewAggrExpMest.GetFocusedRow();
                this.aggrExpMestId = row.ID;
                if (this.aggrExpMestId > 0)
                {
                    LoadDetailAggrExpMestByAggrExpMestId();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tim Kiem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataExpMestThuocNT();
                LoadDataAggrExpMest();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lam Lai
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                chkNotSynthetic.Checked = true;
                chkSynthesized.Checked = false;
                SetDataDefault(); // Du lieu Grid yeu cau
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewDetail_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == grdColMediSTT)
                {
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestReq)
                {
                    ToolTipDetail(gridControlExpMestReq, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlAggrExpMest)
                {
                    ToolTipDetail(gridControlAggrExpMest, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlAggregateRequest)
                {
                    ToolTipDetail(gridControlAggregateRequest, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ToolTipDetail(DevExpress.XtraGrid.GridControl gridControl, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandle != info.RowHandle)
                    {
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        if (info.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                        {
                            text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                        }
                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordProcess_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataExpMestThuocNT(); // Du lieu Grid yeu cau
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewAggrExpMest_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                //Review
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                    {
                        //Status
                        //trang: dang gui YC : màu vàng
                        if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu đỏ
                        else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet : mau den
                        else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                    }

                    //else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
                    //{
                    //    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXP_TIME ?? 0);
                    //}
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrExpMest_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(e.RowHandle);
                    var departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                    long expMestSttId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewAggrExpMest.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    string creator = (gridViewAggrExpMest.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "DELETE_DISPLAY")
                    {
                        if (data.REQ_DEPARTMENT_ID == departmentId
                            && (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                            || expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST))
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_Delete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void buttonEditDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //CommonParam param = new CommonParam();
            //bool success = false;
            //try
            //{
            //    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST _ExpMestRow = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridViewAggrExpMest.GetFocusedRow();
            //    if (_ExpMestRow != null && DevExpress.XtraEditors.XtraMessageBox.Show(
            //        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
            //        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
            //        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        var result = new BackendAdapter(param).Post<bool>("/api/HisExpMest/AggrDelete", ApiConsumers.MosConsumer, _ExpMestRow.ID, param);
            //        if (result)
            //        {
            //            success = true;
            //            gridControlDetail.DataSource = null;
            //            gridControlAggregateRequest.DataSource = null;
            //            LoadDataExpMestThuocNT();
            //            LoadDataAggrExpMest();
            //        }
            //        WaitingManager.Hide();
            //        MessageManager.Show(this.ParentForm, param, success);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void repositoryItemButtonEdit_Print_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //this.currentAggrExpMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridViewAggrExpMest.GetFocusedRow();
                //if (this.currentAggrExpMest != null)
                //{
                //    PrintAggregateExpMest(this.currentAggrExpMest);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch12345()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh123456()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnAggrExpMest()
        {
            try
            {
                btnAggrExpMest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void f2KeyWordFocused()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestReq_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(e.RowHandle);
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EDIT")
                        {
                            if (data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = repositoryItemButtonEdit__Pres;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit__Disable;
                            }
                        }
                        else if (e.Column.FieldName == "DELETE")
                        {
                            if (data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = repositoryItemButton__Delete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton__Delete_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (data.AGGR_EXP_MEST_ID == null || data.AGGR_EXP_MEST_ID <= 0))
                            {
                                e.RepositoryItem = repositoryItemCheck_E;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck_D;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestReq_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                var dataRow = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(hi.RowHandle);
                                if (dataRow != null && dataRow.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (dataRow.AGGR_EXP_MEST_ID == null || dataRow.AGGR_EXP_MEST_ID <= 0))
                                {
                                    checkEdit.Checked = !checkEdit.Checked;
                                    view.CloseEditor();
                                    if (this._ExpMestADOs != null && this._ExpMestADOs.Count > 0)
                                    {
                                        var dataChecks = this._ExpMestADOs.Where(p => p.IsCheck == true && p.AGGR_EXP_MEST_ID == null && p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList();
                                        if (dataChecks != null && dataChecks.Count > 0)
                                        {
                                            gridColumnCheck.Image = imageListIcon.Images[5];
                                        }
                                        else
                                        {
                                            gridColumnCheck.Image = imageListIcon.Images[6];
                                        }
                                    }
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                        else if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit))
                        {
                            var data = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(hi.RowHandle);
                            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                            if (data != null && hi.HitTest == GridHitTest.RowCell && data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                if (hi.Column.FieldName == "DELETE")
                                {
                                    DeleteServiceReq(data);
                                }
                                else if (hi.Column.FieldName == "EDIT")
                                {
                                    ShowAssignPrescription(data);
                                }
                            }
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewExpMestReq.BeginUpdate();
                            if (this._ExpMestADOs == null)
                                this._ExpMestADOs = new List<ExpMestADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._ExpMestADOs)
                                {
                                    item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._ExpMestADOs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewExpMestReq.EndUpdate();
                        }
                    }
                }
                //if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                //{
                //    GridView view = sender as GridView;
                //    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                //    GridHitInfo hi = view.CalcHitInfo(e.Location);
                //    var data = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(hi.RowHandle);
                //    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                //    if (data != null && hi.HitTest == GridHitTest.RowCell && data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                //    {
                //        if (hi.Column.FieldName == "DELETE")
                //        {
                //            DeleteServiceReq(data);
                //        }
                //        else if (hi.Column.FieldName == "EDIT")
                //        {
                //            ShowAssignPrescription(data);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sua Yeu Cau Don Thuoc
        /// </summary>
        /// <param name="data"></param>
        private void ShowAssignPrescription(V_HIS_EXP_MEST data)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(data.TDL_TREATMENT_ID ?? 0, 0, 0);
                    assignServiceADO.PatientDob = data.TDL_PATIENT_DOB ?? 0;
                    assignServiceADO.PatientName = data.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = data.TDL_TREATMENT_CODE;
                    assignServiceADO.TreatmentId = data.TDL_TREATMENT_ID ?? 0;

                    MOS.Filter.HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.ID = data.SERVICE_REQ_ID;
                    var resultServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, reqFilter, null);
                    HIS_SERVICE_REQ _serviceReq = new HIS_SERVICE_REQ();
                    if (resultServiceReqs != null && resultServiceReqs.Count > 0)
                    {
                        _serviceReq = resultServiceReqs.FirstOrDefault();
                    }
                    MOS.EFMODEL.DataModels.HIS_EXP_MEST _expMes = new HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(_expMes, data);
                    AssignPrescriptionEditADO assignEditADO = assignEditADO = new AssignPrescriptionEditADO(_serviceReq, _expMes, FillDataApterSave);

                    assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                    listArgs.Add(assignServiceADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xoa Yeu Cau Don Thuoc
        /// </summary>
        private void DeleteServiceReq(V_HIS_EXP_MEST data)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                //Review
                if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                    sdo.Id = data.SERVICE_REQ_ID;
                    sdo.RequestRoomId = this.currentModule.RoomId;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (success)
                    {
                        btnSearch_Click(null, null);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestReq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewExpMestReq.RowCount > 0 && gridViewExpMestReq.SelectedRowsCount > 0)
                {
                    btnAggrExpMest.Enabled = true;
                }
                else
                {
                    btnAggrExpMest.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrExpMest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInTraDoiTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_EXP_MEST> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST>();
                if (gridViewAggrExpMest.RowCount > 0)
                {
                    for (int i = 0; i < gridViewAggrExpMest.SelectedRowsCount; i++)
                    {
                        if (gridViewAggrExpMest.GetSelectedRows()[i] >= 0)
                        {
                            _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(gridViewAggrExpMest.GetSelectedRows()[i]));
                        }
                    }
                }
                if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_ExpMestTraDoiChecks);
                        listArgs.Add((long)5);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        if (extenceInstance.GetType() == typeof(bool))
                        {
                            return;
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrExpMest_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        //if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit) || hi.Column.FieldName == "DELETE_DISPLAY")
                        //{
                        var data = (V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(hi.RowHandle);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        if (hi.Column.FieldName == "DELETE_DISPLAY")
                        {
                            #region ----- DELETE_DISPLAY -----
                            if (data != null
                        && hi.HitTest == GridHitTest.RowCell
                        && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                try
                                {
                                    CommonParam param = new CommonParam();
                                    bool success = false;

                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        var result = new BackendAdapter(param).Post<bool>("/api/HisExpMest/AggrExamDelete", ApiConsumers.MosConsumer, data.ID, param);
                                        if (result)
                                        {
                                            success = true;
                                            gridControlDetail.DataSource = null;
                                            gridControlAggregateRequest.DataSource = null;
                                            LoadDataExpMestThuocNT();
                                            LoadDataAggrExpMest();
                                        }
                                        WaitingManager.Hide();
                                        MessageManager.Show(this.ParentForm, param, success);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WaitingManager.Hide();
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "PRINT_DISPLAY")
                        {
                            PrintAggregateExpMest(data);
                        }

                        // }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSynthesized_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNotSynthetic_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
