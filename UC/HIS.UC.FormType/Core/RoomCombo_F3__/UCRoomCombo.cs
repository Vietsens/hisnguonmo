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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.UC.FormType.Loader;
using His.UC.LibraryMessage;

namespace HIS.UC.FormType.RoomCombo
{
    public partial class UCRoomCombo : DevExpress.XtraEditors.XtraUserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config;
        HeadCard headCard = new HeadCard();
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;

        public UCRoomCombo(SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI config, object paramRDO)
        {
            try
            {
                InitializeComponent();
                //FormTypeConfig.ReportHight += 25;
             
                this.config = config;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this.isValidData = (this.config != null && this.config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void Init()
        {
            try
            {
                if (this.config.JSON_OUTPUT == "\"ROOM_ID\":{0}") RoomLoader.LoadDataToCombo(cboRoom);
                if (this.config.JSON_OUTPUT == "\"KEY_HEIN_CARD\":{0}"||this.config.JSON_OUTPUT == "\"HEIN_NUMBER_CODE\":{0}") RoomLoader.LoadDataToCombo1(cboRoom);
                if (this.isValidData)
                {
                    Validation();
                    layoutControlItem10.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                SetTitle();//Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetTitle()
        {
            try
            {
                if (this.config != null && !String.IsNullOrEmpty(this.config.DESCRIPTION))
                {
                    layoutControlItem10.Text = this.config.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRoomCode_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            try
            {
                var mediStocks = Config.HisFormTypeConfig.VHisRooms;//.Where(f => f.ROOM_CODE.Contains(txtRoomCode.Text.Trim())).ToList();
                if (mediStocks != null)
                {
                    if (mediStocks.Count == 1)
                    {
                        cboRoom.EditValue = mediStocks[0].ID;
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                    else
                    {
                        cboRoom.ShowPopup();
                        cboRoom.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        if (this.config.JSON_OUTPUT == "\"ROOM_ID\":{0}")
                        {
                            var department = Config.HisFormTypeConfig.VHisRooms.FirstOrDefault(f => f.ID == long.Parse(cboRoom.EditValue.ToString()));
                            if (department != null)
                            {
                                //txtRoomCode.Text = department.ROOM_CODE;
                            }
                        }

                        if (this.config.JSON_OUTPUT == "\"KEY_HEIN_CARD\":{0}"||this.config.JSON_OUTPUT == "\"HEIN_NUMBER_CODE\":{0}")
                        {
                            List<string> a = MOS.LibraryHein.Bhyt.HeinObject.HeinObjectBenefitStore.GetObjectCodeWithBenefitCodes();
                            List<HeadCard> listData = new List<HeadCard>();
                            HeadCard data;
                            for (int i = 0; i < a.Count(); i++)
                            {
                                data = new HeadCard();
                                data.INDEX = i; data.HEAD_HEINCARD = a[i];
                                listData.Add(data);
                            }
                            var department = listData.Where(f => f.INDEX == long.Parse(cboRoom.EditValue.ToString())).First();
                            if (department != null)
                            {
                                this.headCard.INDEX = department.INDEX;
                                this.headCard.HEAD_HEINCARD = department.HEAD_HEINCARD;
                            }
                        }
                    }
                    System.Windows.Forms.SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var department = Config.HisFormTypeConfig.VHisRooms.FirstOrDefault(f => f.ID == long.Parse(cboRoom.EditValue.ToString()));
                        if (department != null)
                        {
                            //txtRoomCode.Text = department.ROOM_CODE;
                        }
                        System.Windows.Forms.SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValue()
        {
            string value = "";
            try
            {
                long? departmentId = (long?)headCard.INDEX;
                
                value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(departmentId));
                string departmentId1;
                if (this.config.JSON_OUTPUT == "\"HEIN_NUMBER_CODE\":{0}")
                {
                    departmentId1 = headCard.HEAD_HEINCARD;
                    value = String.Format(this.config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(departmentId));
                }
            }
            catch (Exception ex)
            {
                value = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }
        public void SetValue()
        {
            try
            {
                if (this.config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this.config,this.config.JSON_OUTPUT, this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        cboRoom.Properties.DataSource = Config.HisFormTypeConfig.VHisRooms;
                        cboRoom.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                    if (this.config.JSON_OUTPUT == "\"HEIN_NUMBER_CODE\":{0}")
                    {
                        List<string> a = MOS.LibraryHein.Bhyt.HeinObject.HeinObjectBenefitStore.GetObjectCodeWithBenefitCodes();
                        List<HeadCard> listData = new List<HeadCard>();
                        HeadCard data;
                        for (int i = 0; i < a.Count(); i++)
                        {
                            data = new HeadCard();
                            data.INDEX = i; data.HEAD_HEINCARD = a[i];
                            listData.Add(data);
                        }
                        cboRoom.Properties.DataSource = listData;
                        cboRoom.EditValue = (listData.FirstOrDefault(f => f.HEAD_HEINCARD == value)??new HeadCard()).INDEX;
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool Valid()
        {
            bool result = true;
            try
            {
                if (this.isValidData != null && this.isValidData)
                {
                    this.positionHandleControl = -1;
                    result = dxValidationProvider1.Validate();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region Validation
        private void ValidateRoom()
        {
            try
            {
                HIS.UC.FormType.RoomCombo.Validation.RoomValidationRule validRule = new HIS.UC.FormType.RoomCombo.Validation.RoomValidationRule();
                //validRule.txtRoomCode = txtRoomCode;
                validRule.cboRoom = cboRoom;
                validRule.ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboRoom, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Validation()
        {
            try
            {
                ValidateRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void UCRoomCombo_Load(object sender, EventArgs e)
        {
            try
            {
                //layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UC_ROOM_COMBO_LCI_TITLE_NAME", Resources.ResourceLanguageManager.LanguageUCRoomCombo, Base.LanguageManager.GetCulture());
                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
