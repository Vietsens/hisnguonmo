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
using SAR.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using HIS.UC.FormType.Core.ServiceGroupCombo_F32__.Validation;

namespace HIS.UC.FormType.Core.ServiceGroupCombo_F32__
{
    public partial class UCServiceGroupCombo : UserControl
    {
        int positionHandleControl = -1;
        bool isValidData = false;
        V_SAR_RETY_FOFI _Config;

        List<HIS_SERVICE_TYPE> hisServiceTypes = null;
        List<V_HIS_SERVICE> listServiceGroup = null;
        SAR.EFMODEL.DataModels.V_SAR_REPORT report;


        public UCServiceGroupCombo(V_SAR_RETY_FOFI config, object paramRDO)
        {
            InitializeComponent();
            try
            {
                //FormTypeConfig.ReportHight += 25;
                if (paramRDO is GenerateRDO)
                {
                    this.report = (paramRDO as GenerateRDO).Report;
                }
                this._Config = config;
                if (config != null)
                {
                    this.isValidData = (config != null && config.IS_REQUIRE == IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE);
                }

                Init();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void Init()
        {
            try
            {
                if (this.isValidData)
                {
                    lciServiceType.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidControl();
                }
                //Set Title cho control
                if (this._Config != null && !String.IsNullOrEmpty(this._Config.DESCRIPTION))
                {
                    var listTitle = this._Config.DESCRIPTION.Split(new char[] { ';', ',' });
                    if (listTitle != null && listTitle.Length >= 2)
                    {
                        lciServiceType.Text = listTitle[0];
                        lciCboServiceGroup.Text = listTitle[1];
                    }
                }
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => report), report));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCServiceGroupCombo_Load(object sender, EventArgs e)
        {
            try
            {
                this.InitComboServiceType();
                this.InitCboServiceGroup();

                if (this.report != null)
                {
                    SetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitComboServiceType()
        {
            try
            {
                this.hisServiceTypes = Config.HisFormTypeConfig.HisServiceTypes.Where(o => (o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).ToList();

                cboServiceType.Properties.DataSource = this.hisServiceTypes;
                cboServiceType.Properties.DisplayMember = "SERVICE_TYPE_NAME";
                cboServiceType.Properties.ValueMember = "ID";
                cboServiceType.Properties.ForceInitialize();
                cboServiceType.Properties.Columns.Clear();
                cboServiceType.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_TYPE_CODE", "", 50));
                cboServiceType.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_TYPE_NAME", "", 150));
                cboServiceType.Properties.ShowHeader = false;
                cboServiceType.Properties.ImmediatePopup = true;
                cboServiceType.Properties.DropDownRows = 10;
                cboServiceType.Properties.PopupWidth = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitCboServiceGroup()
        {
            try
            {
                cboServiceGroup.Properties.DataSource = null;
                cboServiceGroup.Properties.DisplayMember = "SERVICE_NAME";
                cboServiceGroup.Properties.ValueMember = "ID";
                cboServiceGroup.Properties.ForceInitialize();
                cboServiceGroup.Properties.Columns.Clear();
                cboServiceGroup.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_CODE", "", 70));
                cboServiceGroup.Properties.Columns.Add(new LookUpColumnInfo("SERVICE_NAME", "", 200));
                cboServiceGroup.Properties.ShowHeader = false;
                cboServiceGroup.Properties.ImmediatePopup = true;
                cboServiceGroup.Properties.DropDownRows = 10;
                cboServiceGroup.Properties.PopupWidth = 270;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetDataSourceCboServiceGroup()
        {
            try
            {
                cboServiceGroup.Properties.DataSource = listServiceGroup;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtServiceTypeCode.Text))
                    {
                        string key = txtServiceTypeCode.Text.ToLower();
                        var listData = this.hisServiceTypes.Where(o => o.SERVICE_TYPE_CODE.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboServiceType.EditValue = listData.FirstOrDefault().ID;
                            cboServiceGroup.Focus();
                        }
                    }
                    if (!valid)
                    {
                        cboServiceType.Focus();
                        cboServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    cboServiceGroup.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtServiceTypeCode.Text = "";
                cboServiceType.Properties.Buttons[1].Visible = false;
                listServiceGroup = new List<V_HIS_SERVICE>();
                if (cboServiceType != null)
                {
                    var serviceType = this.hisServiceTypes.FirstOrDefault(o => o.ID == Convert.ToInt64(cboServiceType.EditValue));
                    if (serviceType != null)
                    {
                        cboServiceType.Properties.Buttons[1].Visible = true;
                        txtServiceTypeCode.Text = serviceType.SERVICE_TYPE_CODE;
                        LoadListServiceGroupByServiceType(serviceType);
                    }
                }
                SetDataSourceCboServiceGroup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboServiceGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceGroup.EditValue != null)
                {
                    cboServiceGroup.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboServiceGroup.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadListServiceGroupByServiceType(HIS_SERVICE_TYPE serviceType)
        {
            try
            {
                listServiceGroup = new List<V_HIS_SERVICE>();
                if (!Config.HisFormTypeConfig.DicSetyService.ContainsKey(serviceType.ID))
                {
                    return;
                }
                var listData = Config.HisFormTypeConfig.DicSetyService[serviceType.ID];
                var listLeaf = listData.Where(o => o.SERVICE_TYPE_ID == serviceType.ID && o.PARENT_ID.HasValue && o.IS_LEAF == (short)1).ToList();
                if (listLeaf != null && listLeaf.Count > 0)
                {
                    var listParentId = listLeaf.Select(g => g.PARENT_ID).ToList();
                    listServiceGroup = listData.Where(o => listParentId.Contains(o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidControl()
        {
            try
            {
                ValidControlServiceType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlServiceType()
        {
            try
            {
                ServiceTypeValidationRule serviceTypeRule = new ServiceTypeValidationRule();
                serviceTypeRule.txtServiceTypeCode = txtServiceTypeCode;
                serviceTypeRule.cboServiceType = cboServiceType;
                dxValidationProvider1.SetValidationRule(txtServiceTypeCode, serviceTypeRule);
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

        public string GetValue()
        {
            string result = "";
            try
            {
                long? serviceTypeId = null;
                if (cboServiceType.EditValue != null)
                {
                    serviceTypeId = (long)cboServiceType.EditValue;
                }
                long? serviceGroupId = null;
                if (cboServiceGroup.EditValue != null)
                {
                    serviceGroupId = (long)cboServiceGroup.EditValue;
                }
                result = String.Format(this._Config.JSON_OUTPUT, ConvertUtils.ConvertToObjectFilter(serviceTypeId), ConvertUtils.ConvertToObjectFilter(serviceGroupId));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }

            return result;
        }

        public void SetValue()
        {
            try
            {
                if (this._Config.JSON_OUTPUT != null && this.report.JSON_FILTER != null)
                {
                    var jsOutputSub = this._Config.JSON_OUTPUT.Split(new string[] { "," }, StringSplitOptions.None);
                    string value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this._Config, jsOutputSub[0], this.report.JSON_FILTER);
                    if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                    {
                        cboServiceType.Properties.DataSource = hisServiceTypes;
                        cboServiceType.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                    }
                    if (jsOutputSub.Count() > 1)
                    {
                        value = HIS.UC.FormType.CopyFilter.CopyFilter.CopyFilterProcess(this._Config, jsOutputSub[1], this.report.JSON_FILTER);
                        if (value != null && value != "null" && Inventec.Common.TypeConvert.Parse.ToInt64(value) > 0)
                        {
                            cboServiceGroup.Properties.DataSource = listServiceGroup;
                            cboServiceGroup.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(value);
                        }
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
                if (this.isValidData)
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
    }
}
