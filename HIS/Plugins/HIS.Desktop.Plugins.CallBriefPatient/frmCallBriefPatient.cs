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
using DevExpress.Utils;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.CallBriefPatient
{
    public partial class frmCallBriefPatient : HIS.Desktop.Utility.FormBase
    {

        MOS.SDO.HisTreatmentLogSDO TreatmentLogSDO;
        RefeshReference RefeshReference;
        internal Inventec.Desktop.Common.Modules.Module module;
        private WaitDialogForm waitLoad = null;
        int ActionType = 0;
        public const int ActionAdd = 1;//1 -> Add
        public const int ActionEdit = 2;//2 -> Edit
        public const int ActionView = 3;//3 -> View
        public const int ActionViewForEdit = 4;//4 -> View for edit
        public frmCallBriefPatient(Inventec.Desktop.Common.Modules.Module module, RefeshReference RefeshReference, MOS.SDO.HisTreatmentLogSDO treatmentSDO)
		:base(module)
        {
            try
            {
                InitializeComponent();
                this.module = module;
                this.RefeshReference = RefeshReference;
                this.TreatmentLogSDO = treatmentSDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmCallBriefPatient(Inventec.Desktop.Common.Modules.Module module, RefeshReference RefeshReference)
		:base(module)
        {
            try
            {
                InitializeComponent();
                this.module = module;
                this.RefeshReference = RefeshReference;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCallBriefPatient_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.TreatmentLogSDO.HisMediRecord != null)
                {
                    ActionType = ActionEdit;
                    dtLogTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.TreatmentLogSDO.LOG_TIME);
                    dtLogTime.Update();
                }
                else
                {
                    ActionType = ActionAdd;
                    dtLogTime.EditValue = DateTime.Now;
                    dtLogTime.Update();
                }
                SetCaptionByLanguageKey();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallBriefPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.CallBriefPatient.frmCallBriefPatient).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barSave.Caption = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.barSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCallBriefPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                long treatmentId = TreatmentLogSDO.TREATMENT_ID;
                bool success = false;
                CommonParam param = new CommonParam();
                try
                {
                    WaitingManager.Show();
                    dtLogTime.Update();
                    if (ActionType == ActionAdd)
                    {
                        this.TreatmentLogSDO = new MOS.SDO.HisTreatmentLogSDO();
                        this.TreatmentLogSDO.HisMediRecord = new MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD();
                        this.TreatmentLogSDO.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                        this.TreatmentLogSDO.HisMediRecord.TREATMENT_ID = treatmentId;
                        this.TreatmentLogSDO.TREATMENT_ID = treatmentId;
                    }
                    HisMediRecordSDO HisMediRecordSDO = new HisMediRecordSDO();
                    Mapper.CreateMap<MOS.SDO.HisTreatmentLogSDO, MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG>();
                    HisMediRecordSDO.HisTreatmentLog = Mapper.Map<MOS.SDO.HisTreatmentLogSDO, MOS.EFMODEL.DataModels.HIS_TREATMENT_LOG>(this.TreatmentLogSDO);
                    HisMediRecordSDO.HisTreatmentLog.TREATMENT_LOG_TYPE_ID = HisTreatmentLogTypeCFG.TREATMENT_LOG_TYPE_ID__MEDI_RECORD;
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, MOS.EFMODEL.DataModels.HIS_MEDI_RECORD>();
                    HisMediRecordSDO.HisMediRecord = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD, MOS.EFMODEL.DataModels.HIS_MEDI_RECORD>(this.TreatmentLogSDO.HisMediRecord);

                    if (ActionType == ActionAdd)
                    {
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisMediRecordSDO>(HisRequestUriStore.HIS_TREATMENT_LOG_CREATE_MEDI_RECORD, ApiConsumers.MosConsumer, HisMediRecordSDO, null);
                        //new LOGIC.HisTreatmentLog.HisTreatmentLogLogic(param).MediRecordCreate<HisMediRecordSDO>(HisMediRecordSDO);
                        if (rs != null)
                        {
                            success = true;
                        }
                    }
                    else if (ActionType == ActionEdit)
                    {
                        HisMediRecordSDO.HisTreatmentLog.LOG_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLogTime.DateTime) ?? 0;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisMediRecordSDO>(HisRequestUriStore.HIS_TREATMENT_LOG_UPDATE_MEDI_RECORD, ApiConsumers.MosConsumer, HisMediRecordSDO, null);
                        //new LOGIC.HisTreatmentLog.HisTreatmentLogLogic(param).MediRecordUpdate<HisMediRecordSDO>(HisMediRecordSDO);
                        if (rs != null)
                        {
                            success = true;
                        }
                    }

                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Fatal(ex);
                    MessageUtil.SetParam(param, LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
                }

                #region Show message
                // ResultManager.ShowMessage(param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
                if (success)
                {
                    //if (RefeshReference != null)
                    RefeshReference();
                    this.Close();
                }
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
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void dtLogTime_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.Focus();
                e.Handled = true;
            }
        }





    }
}
