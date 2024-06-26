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
using HIS.Desktop.DelegateRegister;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCExamHistory : UserControl
    {
        DelegateNextControl dlgFocusNextUserControl;
        internal List<TreatmentExamADO> TreatmentHistorys { get; set; }
        long patientId;
        public UCExamHistory()
        {
            InitializeComponent();
        }
        public UCExamHistory(long _patientId)
        {
            InitializeComponent();
            this.patientId = _patientId;
        }

        private void UCExamHistory_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKeyNew();
            //LoadTreatmentHistoryTogrid();
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCExamHistory
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCExamHistory).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExamHistory.gridColumn1.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExamHistory.gridColumn2.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExamHistory.gridColumn3.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExamHistory.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamHistory.Text = Inventec.Common.Resource.Get.Value("UCExamHistory.lciExamHistory.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(long _patientId)
        {
            try
            {
                this.patientId = _patientId;
                LoadTreatmentHistoryTogrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadTreatmentHistoryTogrid()
        {
            try
            {
                gridControl1.DataSource = null;
                if (this.patientId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Du lieu dau vao khog hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientId), patientId));
                    return;
                }

                CommonParam param = new CommonParam();
                HisTreatmentLView2Filter treatmentFilter = new HisTreatmentLView2Filter();
                treatmentFilter.PATIENT_ID = this.patientId;
                var treatmentByPatients = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_2>>("api/HisTreatment/GetLView2", ApiConsumers.MosConsumer, treatmentFilter, param);

                this.TreatmentHistorys = new List<TreatmentExamADO>();
                if (treatmentByPatients != null && treatmentByPatients.Count > 0)
                {
                    List<L_HIS_TREATMENT_2> treatmentHTs = treatmentByPatients.Where(o => o.TDL_FIRST_EXAM_ROOM_ID.HasValue).OrderByDescending(o => o.IN_TIME).ToList();
                    if (treatmentHTs != null && treatmentHTs.Count > 0)
                    {
                        List<HIS_EXECUTE_ROOM> executeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROOM>();
                        foreach (var item in treatmentHTs)
                        {
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_2, TreatmentExamADO>();
                            TreatmentExamADO treatmentHistoryADO = AutoMapper.Mapper.Map<L_HIS_TREATMENT_2, TreatmentExamADO>(item);
                            HIS_EXECUTE_ROOM executeRoom = executeRooms.FirstOrDefault(o => o.ROOM_ID == item.TDL_FIRST_EXAM_ROOM_ID);
                            treatmentHistoryADO.FIRST_EXAM_ROOM_NAME = (executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : "");
                            this.TreatmentHistorys.Add(treatmentHistoryADO);
                        }
                    }

                    gridControl1.DataSource = this.TreatmentHistorys;
                }
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
                this.lciExamHistory.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCExamHistory", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RefreshUserControl()
        {
            try
            {
                this.patientId = 0;
                this.TreatmentHistorys = null;
                gridControl1.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentExamADO data = (TreatmentExamADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "HISTORY_TIME_DISPLAY")
                        {
                            e.Value = HistoryTimeFormat(data.IN_TIME, data.OUT_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string HistoryTimeFormat(long intime, long? outtime)
        {
            string strTime = "";
            try
            {
                if (!outtime.HasValue)
                    strTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(intime);
                else
                {
                    string dateIn = intime.ToString().Substring(0, 8);
                    string dateOut = outtime.ToString().Substring(0, 8);
                    if (dateIn == dateOut)
                        strTime = intime.ToString().Substring(6, 2) + "/" + intime.ToString().Substring(4, 2) + "/" + intime.ToString().Substring(0, 4)
                            + " " + intime.ToString().Substring(8, 2) + ":" + intime.ToString().Substring(10, 2) + " - " + outtime.Value.ToString().Substring(8, 2) + ":" + outtime.Value.ToString().Substring(10, 2);
                    else
                        strTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(intime) + " " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(outtime.Value);
                }
            }
            catch (Exception ex)
            {
                strTime = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return strTime;
        }

		private void repView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			try
			{
                Popup.frmTreatmentDetail frm = new Popup.frmTreatmentDetail(((TreatmentExamADO)gridView1.GetFocusedRow()).ID);
                frm.ShowDialog();
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
        internal void DisposeControl()
        {
            try
            {
                patientId = 0;
                TreatmentHistorys = null;
                dlgFocusNextUserControl = null;
                this.gridView1.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
                this.repView.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repView_ButtonClick);
                this.Load -= new System.EventHandler(this.UCExamHistory_Load);
                gridView1.GridControl.DataSource = null;
                gridControl1.DataSource = null;
                repView = null;
                gridColumn3 = null;
                lciExamHistory = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
                gridColumn2 = null;
                gridColumn1 = null;
                gridView1 = null;
                gridControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
