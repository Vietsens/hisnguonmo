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
using Inventec.Core;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using HIS.Desktop.Plugins.ExecuteBedRoomSummary;
using Inventec.Common.Adapter;
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.LocalData;
using MPS.Processor.Mps000209.PDO;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.ExecuteBedRoomSummary
{
    public partial class UCBedRoom : HIS.Desktop.Utility.UserControlBase
    {

        List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER> listRoomCounter = null;
        List<HisBedRoomCouterSDO> listBedRoomCounter = null;
        List<HIS_DEPARTMENT> _DepartmentSelecteds;
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        public UCBedRoom(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
            InitComboDepartment(BackendDataWorker.Get<HIS_DEPARTMENT>());
            InitDepartmentCheck();
            MeShow();
        }

        public void MeShow()
        {
            try
            {
                List<long> listDepartmentId = new List<long>();
                listRoomCounter = new List<L_HIS_ROOM_COUNTER>();
                listBedRoomCounter = new List<HisBedRoomCouterSDO>();
              
                HisTreatmentBedRoomViewFilter treatmentBedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                treatmentBedRoomViewFilter.ORDER_DIRECTION = "ASC";
                treatmentBedRoomViewFilter.ORDER_FIELD = "DEPARTMENT_NAME";
                if (chkTong.Checked == true)
                {
                   // treatmentBedRoomViewFilter.
                }
                HisRoomCounterLViewFilter filter = new HisRoomCounterLViewFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "DEPARTMENT_NAME";
                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    listDepartmentId = _DepartmentSelecteds.Select(o => o.ID).ToList();
                    filter.DEPARTMENT_IDs = listDepartmentId;
                }
                if (chkTong.Checked == true)
                {
                   // filter.T
                }
                gridControlExecuteRoomCouter.DataSource = null;
                gridControlBedRoomCouter.DataSource = null;

                listRoomCounter = FillDataIntoGridControlRoomCouter(filter);
                listBedRoomCounter = FillDataIntoGridControlBedRoomCouter(treatmentBedRoomViewFilter, listDepartmentId);

                gridControlExecuteRoomCouter.DataSource = FillDataIntoGridControlRoomCouter(filter);
                gridControlBedRoomCouter.DataSource = FillDataIntoGridControlBedRoomCouter(treatmentBedRoomViewFilter, listDepartmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExecuteRoomCouter_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER dataRow = (MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TOTAL_CLOSE_SERVICE_REQ")
                    {
                        if (chkTong.Checked == true)
                        {
                            if (true)
                            {

                            }
                        }

                        e.Value = (dataRow.TOTAL_TODAY_SERVICE_REQ ?? 0) - (dataRow.TOTAL_NEW_SERVICE_REQ ?? 0);
                    }
                    else if(e.Column.FieldName == "RESPONSIBLE_USERNAME")
                    {
                        if (dataRow.RESPONSIBLE_USERNAME != null)
                        {
                            e.Value = dataRow.RESPONSIBLE_USERNAME;
                        }
                    }
                 
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBedRoomCouter_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisBedRoomCouterSDO dataRow = (HisBedRoomCouterSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static List<HisBedRoomCouterSDO> FillDataIntoGridControlBedRoomCouter(HisTreatmentBedRoomViewFilter hisBedRoomFilter, List<long> listDepartmentId)
        {
            List<HisBedRoomCouterSDO> lstHisBedRoomCouterSDO = new List<HisBedRoomCouterSDO>();
            try
            {
                CommonParam param = new CommonParam();
                hisBedRoomFilter.IS_ACTIVE = 1;
                hisBedRoomFilter.IS_IN_ROOM = true;

                List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, hisBedRoomFilter, param);

                if (lstTreatmentBedRoom != null && lstTreatmentBedRoom.Count > 0)
                {
                    HisBedRoomViewFilter filter = new HisBedRoomViewFilter();

                    if (listDepartmentId != null && listDepartmentId.Count > 0)
                    {
                        filter.DEPARTMENT_IDs = listDepartmentId;
                    }
                    filter.IS_ACTIVE = 1;

                    List<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM> VHisBedRooms = new BackendAdapter(param).Get<List<V_HIS_BED_ROOM>>("api/HisBedRoom/GetView", ApiConsumers.MosConsumer, filter, param);

                    if (VHisBedRooms != null && VHisBedRooms.Count > 0)
                    {
                        foreach (var item_bedRoom in VHisBedRooms)
                        {
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM, HisBedRoomCouterSDO>();
                            HisBedRoomCouterSDO hisBedRoomCouterSDO = new HisBedRoomCouterSDO();
                            hisBedRoomCouterSDO = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM, HisBedRoomCouterSDO>(item_bedRoom);

                            if (lstTreatmentBedRoom != null && lstTreatmentBedRoom.Count > 0)
                            {
                                var treatmentBedRoom = lstTreatmentBedRoom.Where(o => o.BED_ROOM_ID == item_bedRoom.ID).ToList();
                                if (treatmentBedRoom != null && treatmentBedRoom.Count > 0)
                                {
                                    hisBedRoomCouterSDO.TOTAL_TREATMENT_BED_ROOM = treatmentBedRoom.Count;
                                }
                            }
                            lstHisBedRoomCouterSDO.Add(hisBedRoomCouterSDO);
                        }

                        lstHisBedRoomCouterSDO = lstHisBedRoomCouterSDO.OrderBy(o => o.DEPARTMENT_NAME).ThenBy(p => p.DEPARTMENT_NAME).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return lstHisBedRoomCouterSDO;
        }

        internal  List<MOS.EFMODEL.DataModels.L_HIS_ROOM_COUNTER> FillDataIntoGridControlRoomCouter(HisRoomCounterLViewFilter filter) //static
        {
            try
            {
                filter.IS_ACTIVE = 1;

                CommonParam param = new CommonParam();
                var GetCounter = new BackendAdapter(param).Get<List<L_HIS_ROOM_COUNTER>>("api/HisRoom/GetCounterLView", ApiConsumers.MosConsumer, filter, param);

                if (GetCounter != null && GetCounter.Count > 0)
                {
                    if (chkTong.Checked == true)
                    {
                        GetCounter = GetCounter.Where( o =>o.TOTAL_TODAY_SERVICE_REQ>0).OrderBy(p => p.DEPARTMENT_NAME).ThenBy(q => q.EXECUTE_ROOM_NAME).ToList();
                    }
                    else
                    {
                        GetCounter = GetCounter.OrderBy(p => p.DEPARTMENT_NAME).ThenBy(q => q.EXECUTE_ROOM_NAME).ToList();
                    }
                   
                }
                return GetCounter;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private void UCBedRoom_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteBedRoomSummary.Resources.Lang", typeof(HIS.Desktop.Plugins.ExecuteBedRoomSummary.UCBedRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCBedRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UCBedRoom.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCBedRoom.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCBedRoom.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCBedRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCBedRoom.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCBedRoom.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCBedRoom.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetDataSourceCombo(string keyWord, ref List<HIS_DEPARTMENT> department)
        {
            try
            {
                var departments = BackendDataWorker.Get<HIS_DEPARTMENT>();
                if (!string.IsNullOrEmpty(keyWord))
                {
                    if (departments != null && departments.Count > 0)
                    {
                        departments = departments.Where(o =>
                            o.DEPARTMENT_NAME.ToUpper().Contains(keyWord.ToUpper())
                            || o.DEPARTMENT_CODE.ToUpper().Contains(keyWord.ToUpper())
                            ).ToList();
                    }
                }
                //cboDepartment.Properties.DataSource = departments;
                department = departments;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000209", delegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                //{
                this.cboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                btnSearch.Focus();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        #region Print

        bool delegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                InThongKePhongBuong(printTypeCode, fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InThongKePhongBuong(string printTypeCode, string fileName)
        {
            try
            {
                bool result = false;

                WaitingManager.Show();

                Mps000209ADO mps000209Ado = new Mps000209ADO();
                mps000209Ado.CREATE_TIME = Inventec.Common.DateTime.Get.Now();

                string listDepartmentNames = "";
                foreach (var item in _DepartmentSelecteds)
                {
                    listDepartmentNames += item.DEPARTMENT_NAME + " ,";
                }

                mps000209Ado.LIST_DEPARTMENT_NAME = listDepartmentNames;

                if (currentModule != null && currentModule.RoomId > 0)
                {
                    mps000209Ado.ROOM_NAME_PRINT = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).ROOM_NAME;
                    mps000209Ado.DEPARTMENT_NAME_PRINT = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).DEPARTMENT_NAME;
                }

                mps000209Ado.LOGIN_NAME_PRINT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                mps000209Ado.USERNAME_PRINT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                MPS.Processor.Mps000209.PDO.Mps000209PDO mps000209RDO = new MPS.Processor.Mps000209.PDO.Mps000209PDO(mps000209Ado, listBedRoomCounter, listRoomCounter, _DepartmentSelecteds);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000209RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000209RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string departmentName = "";
                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _DepartmentSelecteds)
                    {
                        departmentName += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = departmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Print()
        {
            try
            {
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Search()
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboDepartment(List<HIS_DEPARTMENT> datas)
        {
            try
            {
                if (datas != null)
                {
                    cboDepartment.Properties.DataSource = datas;
                    cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                    cboDepartment.Properties.ValueMember = "ID";
                    cboDepartment.Properties.PopupFormWidth = 370;
                    GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null && datas.Count == 1)
                    {
                        gridCheckMark.SelectAll(cboDepartment.Properties.DataSource);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDepartmentCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDepartment.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__Department);
                cboDepartment.Properties.Tag = gridCheck;
                cboDepartment.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDepartment.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Department(object sender, EventArgs e)
        {
            try
            {
                _DepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DepartmentSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<HIS_DEPARTMENT> departments = new List<HIS_DEPARTMENT>();
                    SetDataSourceCombo(cboDepartment.Text, ref departments);
                    InitComboDepartment(departments);
                    if (departments.Count == 1)
                    {
                        btnSearch.Focus();
                    }
                    else
                    {
                        this.cboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDepartment_Click(object sender, EventArgs e)
        {
            try
            {
                cboDepartment.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

    }
}
