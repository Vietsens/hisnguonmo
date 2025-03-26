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
using DevExpress.XtraLayout;
using HIS.UC.WorkPlace;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.UC.PlusInfo.Config;
using HIS.Desktop.DelegateRegister;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using SDA.EFMODEL.DataModels;
using HIS.UC.PlusInfo.Design;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.PlusInfo
{
    public partial class UCPlusInfo : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        Design.UCPatientExtend ucExtend1;


        PatientInformationADO patientInfoADO { get; set; }
        UCPatientExtendADO patientExtendADO = new ADO.UCPatientExtendADO();
        WorkPlaceADO dataWorkPlaceADO = new ADO.WorkPlaceADO();
        DelegateFocusNextUserControl dlgFocusNextUserControl;

        int indexOfControlEnd = 0;
        int tagIndex = 0;
        int indexUCMaHoNgheo = 0;
        int indexUCCMNDNumber = 0;
        List<UserControl> listControl = null;
        List<UserControl> listControlForFormExtend = null;
        List<SDA_MODULE_FIELD> moduleField = null;
        List<string> userControlNameAdded = null;
        int totalModule = 0;
        int totalRowLimit;

        #endregion

        #region Khoi tao control

        internal Design.UCAddressNow ucAddress1;
        internal Design.UCWorkPlace ucWorkPlace1;
        internal Design.UCBlood ucBlood1;
        internal Design.UCBloodRh ucBloodRh1;
        internal Design.UCCMNDDate ucCMNDDate1;
        internal Design.UCCMND ucCmndNumber1;
        internal Design.UCCMNDPlace ucCMNDPlace1;
        internal Design.UCCommuneNow ucCommuneNow1;
        internal Design.UCDistrictNow ucDistrictNow1;
        internal Design.UCEmail ucEmail1;
        internal Design.UCEthnic ucEthnic1;
        internal Design.UCFatherName ucFather1;
        internal Design.UCHouseHold ucHouseHoldNumber1;
        internal Design.UCHohName ucHohName1;
        internal Design.UCHouseHoldRelative ucHouseHoldRelative1;
        internal Design.UCMilitaryRank ucMilitaryRank1;
        internal Design.UCMotherName ucMother1;
        internal Design.UCNational ucNational1;
        internal Design.UCPhoneNumber ucPhone1;
        internal Design.UCProgram ucProgram1;
        internal Design.UCProvinceNow ucProvinceNow1;
        internal Design.UCProvinceOfBirth ucProvinceOfBirth1;
        internal Design.UCMaHoNgheo ucMaHoNgheo1;
        internal Design.UCPatientStoreCode ucPatientStoreCode;
        internal Design.UCCommuneOfBirth ucCommuneOfBirth1;
        internal Design.UCDistrictOfBirth ucDistrictOfBirth1;
        internal Design.UCAddressOfBirth ucAddressOfBirth1;
        internal Design.UCHrmKskCode ucHrmKskCode;
        internal Design.UCTaxCode ucTaxCode;
        internal Design.UCExamHistory ucExamHistory;
        internal Design.UCCheckBoxCCCD ucCheckBoxCCCD;
        #endregion

        #region Constructor - Load

        public UCPlusInfo()
            : this(9, false)
        {
        }

        public UCPlusInfo(int _totalRowLimit)
            : this(_totalRowLimit, false)
        {
        }

        public UCPlusInfo(int _totalRowLimit, bool _isShow)
            : base("HIS.Desktop.Plugins.RegisterV2", "UCPlusInfo")
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo----1----InitializeComponent-------");
                InitializeComponent();
                HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(HIS.UC.PlusInfo.UCPlusInfo).Assembly);
                this.totalRowLimit = (_totalRowLimit == 0 ? 9 : _totalRowLimit);
                //this.InitDataStorage();
                //this.ConfigLayout();
                UCPlusInfo_Config.LoadConfig();
                this._isShowControlHrmKskCode = _isShow;

                //this.InitFieldFromAsync();
                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo----2----InitializeComponent-------");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Khoi tao constructor UCPlusInfo that bai: \n" + ex);
            }
        }

        private async void UCPlusInfo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo_Load 1");
                //SetCaptionByLanguageKey();
                timer1.Enabled = true;
                timer1.Interval = 2000;
                timer1.Start();
                this.InitFieldFromAsync();
                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo_Load 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Load UCPlusInfo that bai: \n" + ex);
            }
        }

        public async Task InitFieldFromAsync()
        {
            try
            {
                await this.InitDataStorage();
                if (this.moduleField == null)
                    this.moduleField = new List<SDA_MODULE_FIELD>();
                this.moduleField = this.moduleField
                    .Where(o => o.IS_VISIBLE == 1 && o.IS_ACTIVE == 1 && o.MODULE_LINK == UCPlusInfo_Config.ModuleLink //&&(o.FIELD_CODE != ChoiceControl.ucHrmKskCode || (o.FIELD_CODE == ChoiceControl.ucHrmKskCode && this._isShowControlHrmKskCode))
                        )
                    .OrderBy(o => o.NUM_ORDER ?? 999999).ThenBy(p => p.FIELD_NAME).ToList();

                this.totalModule = this.moduleField.Count;
                if (this.moduleField != null && this.moduleField.Count > 0)
                {
                    this.listControl = new List<UserControl>();
                    for (int i = 0; i < moduleField.Count; i++)
                    {
                        this.GetTempControl(moduleField[i]);
                    }
                }
                this.PositionControl(listControl);

                if (this.ucExtend1 != null)
                {
                    this.LoadControlForFormExtend(this.listControl, indexOfControlEnd);
                    this.ucExtend1.SetControlForFormExtend(this.listControlForFormExtend);
                }
                this.SetDelegateToSetValueAddress();
                this.SetDelegateToSetValueAddressKS();
                if (this.ucProvinceNow1 != null)
                    this.ucProvinceNow1.ReloadDataDistrictAndCommune(this.RefreshDataDistrictAndCommune);
                if (this.ucProvinceOfBirth1 != null)
                    this.ucProvinceOfBirth1.ReloadDataDistrictAndCommune(this.RefreshDataDistrictAndCommuneOfBirth);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCPlusInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCPlusInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.layoutControlGroup1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo----1----timer1_Tick-------");
                if (ucBlood1 != null)
                {
                    ucBlood1.DataDefault();
                }
                if (ucBloodRh1 != null)
                {
                    ucBloodRh1.DataDefault();
                }
                if (ucEthnic1 != null)
                {
                    ucEthnic1.InitEthnic();
                }
                if (ucHouseHoldRelative1 != null)
                {
                    ucHouseHoldRelative1.DataDefault();
                }
                if (ucMilitaryRank1 != null)
                {
                    ucMilitaryRank1.InitMilitaryRank();
                }
                if (ucNational1 != null)
                {
                    ucNational1.InitNational();
                }
                if (ucProgram1 != null)
                {
                    ucProgram1.DataDefault();
                }
                if (ucProvinceNow1 != null)
                {
                    ucProvinceNow1.UCProvinceInit();
                }
                if (ucProvinceOfBirth1 != null)
                {
                    ucProvinceOfBirth1.UCProvinceOfBirthInit();
                }


                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo----2----timer1_Tick-------");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public async Task UCPlusInfoOnLoadAsync()
        {
            try
            {
                //timer1.Stop();
                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo----1----UCPlusInfoOnLoadAsync-------");
                if (ucBlood1 != null)
                {
                    ucBlood1.DataDefault();
                }
                if (ucBloodRh1 != null)
                {
                    ucBloodRh1.DataDefault();
                }
                if (ucEthnic1 != null)
                {
                    ucEthnic1.InitEthnic();
                }
                if (ucHouseHoldRelative1 != null)
                {
                    ucHouseHoldRelative1.DataDefault();
                }
                if (ucMilitaryRank1 != null)
                {
                    ucMilitaryRank1.InitMilitaryRank();
                }
                if (ucNational1 != null)
                {
                    ucNational1.InitNational();
                }
                if (ucProgram1 != null)
                {
                    ucProgram1.DataDefault();
                }
                if (ucProvinceNow1 != null)
                {
                    ucProvinceNow1.UCProvinceInit();
                }
                if (ucProvinceOfBirth1 != null)
                {
                    ucProvinceOfBirth1.UCProvinceOfBirthInit();
                }
                if (ucHrmKskCode != null)
                {
                    ucHrmKskCode.SetVisible(false);
                }

                Inventec.Common.Logging.LogSystem.Debug("UCPlusInfo----2----UCPlusInfoOnLoadAsync-------");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConfigLayout()
        {
            try
            {
                //if (this.totalRowLimit > 0 && this.totalRowLimit != 9)
                //{
                //    layoutControl1.BeginUpdate();
                //    //for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i++)
                //    //{
                //    //    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].SizeType = SizeType.Percent;
                //    //    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].Width = (100 / 2);
                //    //}

                //    Inventec.Common.Logging.LogSystem.Info("ConfigLayout - 1");
                //    for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i++)
                //    {
                //        layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].SizeType = SizeType.Percent;
                //        layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].Height = (100 / this.totalRowLimit);
                //    }
                //    Inventec.Common.Logging.LogSystem.Info("ConfigLayout - 2");
                //    for (int i = layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i < this.totalRowLimit; i++)
                //    {
                //        layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Add(
                //            new RowDefinition()
                //            {
                //                SizeType = SizeType.Percent,
                //                Height = (100 / this.totalRowLimit)
                //            });
                //    }
                //    layoutControl1.EndUpdate();
                //    Inventec.Common.Logging.LogSystem.Info("ConfigLayout - 3");
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitDataStorage()
        {
            try
            {
                //this.moduleField = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA_MODULE_FIELD>();
                CommonParam paramCommon = new CommonParam();
                dynamic dfilter = new System.Dynamic.ExpandoObject();
                dfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.moduleField = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<SDA_MODULE_FIELD>>("api/SdaModuleField/Get", ApiConsumers.SdaConsumer, dfilter, paramCommon);
                if (this.moduleField != null) BackendDataWorker.UpdateToRam(typeof(SDA_MODULE_FIELD), this.moduleField, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitFieldFrom()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<SDA_MODULE_FIELD>())
                {
                    this.moduleField = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA_MODULE_FIELD>();
                }
                else
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    this.moduleField = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<SDA_MODULE_FIELD>>("api/SdaModuleField/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, filter, paramCommon);

                    if (this.moduleField != null) BackendDataWorker.UpdateToRam(typeof(SDA_MODULE_FIELD), this.moduleField, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (this.moduleField == null)
                    this.moduleField = new List<SDA_MODULE_FIELD>();
                this.moduleField = this.moduleField.Where(o => o.IS_VISIBLE == 1 && o.IS_ACTIVE == 1 && o.MODULE_LINK == UCPlusInfo_Config.ModuleLink).OrderBy(o => o.NUM_ORDER ?? 999999).ThenBy(p => p.FIELD_NAME).ToList();
                this.totalModule = this.moduleField.Count;
                if (this.moduleField != null && this.moduleField.Count > 0)
                {
                    this.listControl = new List<UserControl>();
                    for (int i = 0; i < moduleField.Count; i++)
                    {
                        this.GetTempControl(moduleField[i]);
                    }
                }
                this.PositionControl(listControl);

                if (this.ucExtend1 != null)
                {
                    this.LoadControlForFormExtend(this.listControl, indexOfControlEnd);
                    this.ucExtend1.SetControlForFormExtend(this.listControlForFormExtend);
                }
                this.SetDelegateToSetValueAddress();
                this.SetDelegateToSetValueAddressKS();
                if (this.ucProvinceNow1 != null)
                    this.ucProvinceNow1.ReloadDataDistrictAndCommune(this.RefreshDataDistrictAndCommune);
                if (this.ucProvinceOfBirth1 != null)
                    this.ucProvinceOfBirth1.ReloadDataDistrictAndCommune(this.RefreshDataDistrictAndCommuneOfBirth);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadControlForFormExtend(List<UserControl> listControl, int indexStartLoad)
        {
            try
            {
                //List<UserControl> _UserControls = new List<UserControl>();
                //if (this._isShowControlHrmKskCode)
                //{
                //    _UserControls = listControl;
                //}
                //else
                //{
                //    _UserControls = listControl.Where(p => p.Name != "UCHrmKskCode").ToList();
                //}
                this.listControlForFormExtend = null;
                if (userControlNameAdded != null && userControlNameAdded.Count > 0)
                {
                    this.listControlForFormExtend = listControl.Where(o => !userControlNameAdded.Contains(o.Name)).ToList();
                    var namePlus = this.listControlForFormExtend.Select(o => o.Name).ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => namePlus), namePlus));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetTempControl(SDA_MODULE_FIELD moduleField)
        {
            try
            {
                tagIndex += 1;
                switch (moduleField.FIELD_CODE)
                {
                    #region --------- Khoi tao uc control -------------

                    case ChoiceControl.ucAddress:
                        ucAddress1 = new Design.UCAddressNow();
                        ucAddress1.TabIndex = tagIndex;
                        ucAddress1.FocusNextControl(this.FocusNextUserControl);
                        this.listControl.Add(ucAddress1);
                        break;
                    case ChoiceControl.ucWorkPlace:
                        ucWorkPlace1 = new Design.UCWorkPlace(HIS.UC.PlusInfo.ClassGlobal.GlobalStore.CurrentModule, FocusNext);
                        ucWorkPlace1.TabIndex = tagIndex;
                        ucWorkPlace1.PreviewKeyDown += FocusNextUserControl;
                        listControl.Add(ucWorkPlace1);
                        break;
                    case ChoiceControl.ucBlood:
                        ucBlood1 = new Design.UCBlood();
                        ucBlood1.TabIndex = tagIndex;
                        ucBlood1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucBlood1);
                        break;
                    case ChoiceControl.ucBloodRh:
                        ucBloodRh1 = new Design.UCBloodRh();
                        ucBloodRh1.TabIndex = tagIndex;
                        ucBloodRh1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucBloodRh1);
                        break;
                    case ChoiceControl.ucCmndDate:
                        ucCMNDDate1 = new Design.UCCMNDDate();
                        ucCMNDDate1.TabIndex = tagIndex;
                        ucCMNDDate1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucCMNDDate1);
                        break;
                    case ChoiceControl.ucCmndNumber:
                        ucCmndNumber1 = new Design.UCCMND();
                        ucCmndNumber1.TabIndex = tagIndex;
                        ucCmndNumber1.FocusNextControl(this.FocusNextUserControl);
                        this.indexUCCMNDNumber = tagIndex;
                        listControl.Add(ucCmndNumber1);
                        break;
                    case ChoiceControl.ucCmndPlace:
                        ucCMNDPlace1 = new Design.UCCMNDPlace();
                        ucCMNDPlace1.TabIndex = tagIndex;
                        ucCMNDPlace1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucCMNDPlace1);
                        break;
                    case ChoiceControl.ucCommuneNow:
                        ucCommuneNow1 = new Design.UCCommuneNow();
                        ucCommuneNow1.TabIndex = tagIndex;
                        ucCommuneNow1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucCommuneNow1);
                        break;
                    case ChoiceControl.ucDistrictNow:
                        ucDistrictNow1 = new Design.UCDistrictNow();
                        ucDistrictNow1.TabIndex = tagIndex;
                        ucDistrictNow1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucDistrictNow1);
                        break;
                    case ChoiceControl.ucEmai:
                        ucEmail1 = new Design.UCEmail();
                        ucEmail1.TabIndex = tagIndex;
                        ucEmail1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucEmail1);
                        break;
                    case ChoiceControl.ucEthnic:
                        if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.ChangeEthnic == 0)
                        {
                            ucEthnic1 = new Design.UCEthnic();
                            ucEthnic1.TabIndex = tagIndex;
                            ucEthnic1.FocusNextControl(this.FocusNextUserControl);
                            listControl.Add(ucEthnic1);
                        }
                        break;
                    //case ChoiceControl.ucFatherName:
                    //    ucFather1 = new Design.UCFatherName();
                    //    ucFather1.TabIndex = tagIndex;
                    //    ucFather1.FocusNextControl(this.FocusNextUserControl);
                    //    listControl.Add(ucFather1);
                    //    break;
                    case ChoiceControl.ucHohName:
                        ucHohName1 = new Design.UCHohName();
                        ucHohName1.TabIndex = tagIndex;
                        ucHohName1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucHohName1);
                        break;
                    case ChoiceControl.ucHouseHold:
                        ucHouseHoldNumber1 = new Design.UCHouseHold();
                        ucHouseHoldNumber1.TabIndex = tagIndex;
                        ucHouseHoldNumber1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucHouseHoldNumber1);
                        break;
                    case ChoiceControl.ucHouseHoldRelative:
                        ucHouseHoldRelative1 = new Design.UCHouseHoldRelative();
                        ucHouseHoldRelative1.TabIndex = tagIndex;
                        ucHouseHoldRelative1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucHouseHoldRelative1);
                        break;
                    case ChoiceControl.ucMilitaryRank:
                        ucMilitaryRank1 = new Design.UCMilitaryRank();
                        ucMilitaryRank1.TabIndex = tagIndex;
                        ucMilitaryRank1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucMilitaryRank1);
                        break;
                    //case ChoiceControl.ucMotherName:
                    //    ucMother1 = new Design.UCMotherName();
                    //    ucMother1.TabIndex = tagIndex;
                    //    ucMother1.FocusNextControl(this.FocusNextUserControl);
                    //    listControl.Add(ucMother1);
                    //    break;
                    case ChoiceControl.ucNational:
                        ucNational1 = new Design.UCNational();
                        ucNational1.TabIndex = tagIndex;
                        ucNational1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucNational1);
                        break;
                    case ChoiceControl.ucPhoneNumber:
                        ucPhone1 = new Design.UCPhoneNumber();
                        ucPhone1.TabIndex = tagIndex;
                        ucPhone1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucPhone1);
                        break;
                    case ChoiceControl.ucProgram:
                        ucProgram1 = new Design.UCProgram();
                        ucProgram1.TabIndex = tagIndex;
                        ucProgram1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucProgram1);
                        break;
                    case ChoiceControl.ucProvinceNow:
                        ucProvinceNow1 = new Design.UCProvinceNow();
                        ucProvinceNow1.TabIndex = tagIndex;
                        ucProvinceNow1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucProvinceNow1);
                        break;
                    case ChoiceControl.ucProvinceOfBirth:
                        ucProvinceOfBirth1 = new Design.UCProvinceOfBirth();
                        ucProvinceOfBirth1.TabIndex = tagIndex;
                        ucProvinceOfBirth1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucProvinceOfBirth1);
                        break;
                    case ChoiceControl.ucMaHoNgheo:
                        ucMaHoNgheo1 = new Design.UCMaHoNgheo();
                        ucMaHoNgheo1.TabIndex = tagIndex;
                        ucMaHoNgheo1.FocusNextControl(this.FocusNextUserControl);
                        this.indexUCMaHoNgheo = tagIndex;
                        listControl.Add(ucMaHoNgheo1);
                        break;
                    case ChoiceControl.ucPatientStoreCode:
                        ucPatientStoreCode = new Design.UCPatientStoreCode();
                        ucPatientStoreCode.TabIndex = tagIndex;
                        ucPatientStoreCode.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucPatientStoreCode);
                        break;
                    case ChoiceControl.ucCommuneOfBirth:
                        ucCommuneOfBirth1 = new Design.UCCommuneOfBirth();
                        ucCommuneOfBirth1.TabIndex = tagIndex;
                        ucCommuneOfBirth1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucCommuneOfBirth1);
                        break;
                    case ChoiceControl.ucDistrictOfBirth:
                        ucDistrictOfBirth1 = new Design.UCDistrictOfBirth();
                        ucDistrictOfBirth1.TabIndex = tagIndex;
                        ucDistrictOfBirth1.FocusNextControl(this.FocusNextUserControl);
                        listControl.Add(ucDistrictOfBirth1);
                        break;
                    case ChoiceControl.ucAddressOfBirth:
                        ucAddressOfBirth1 = new Design.UCAddressOfBirth();
                        ucAddressOfBirth1.TabIndex = tagIndex;
                        ucAddressOfBirth1.FocusNextControl(this.FocusNextUserControl);
                        this.listControl.Add(ucAddressOfBirth1);
                        break;
                    case ChoiceControl.ucHrmKskCode:
                        //if (this._isShowControlHrmKskCode)
                        //{
                        ucHrmKskCode = new Design.UCHrmKskCode();
                        ucHrmKskCode.TabIndex = tagIndex;
                        ucHrmKskCode.FocusNextControl(this.FocusNextUserControl);
                        this.listControl.Add(ucHrmKskCode);
                        //}
                        break;
                    case ChoiceControl.ucTaxCode:
                        ucTaxCode = new Design.UCTaxCode();
                        ucTaxCode.TabIndex = tagIndex;
                        ucTaxCode.FocusNextControl(this.FocusNextUserControl);
                        this.listControl.Add(ucTaxCode);
                        break;
                    case ChoiceControl.ucExamHistory:
                        ucExamHistory = new Design.UCExamHistory();
                        ucExamHistory.TabIndex = tagIndex;
                        ucExamHistory.FocusNextControl(this.FocusNextUserControl);
                        this.listControl.Add(ucExamHistory);
                        break;
                    case ChoiceControl.UcCheckBoxCCCD:
                        ucCheckBoxCCCD = new Design.UCCheckBoxCCCD(ReloadFeildNameValid);
                        ucCheckBoxCCCD.TabIndex = tagIndex;
                        ucCheckBoxCCCD.FocusNextControl(this.FocusNextUserControl);
                        this.listControl.Add(ucCheckBoxCCCD);
                        break;
                    default:
                        break;

                        #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadFeildNameValid(bool isReload)
        {
            try
            {
                if (ucCmndNumber1 != null) {
                    ucCmndNumber1.ReloadValidFeild(!isReload);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusNext()
        {
            try
            {
                foreach (LayoutControlItem col in layoutControlGroup1.Items)
                {
                    if (col != null && col.Control.TabIndex == ucWorkPlace1.TabIndex + 1)
                    {
                        UserControl uc = null;
                        foreach (UserControl item in listControl)
                        {
                            if (item == col.Control)
                            {
                                uc = item;
                                break;
                            }
                        }
                        if (uc != null)
                        {
                            FocusControl(uc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PositionControl(List<UserControl> listControl)
        {
            try
            {
                layoutControl1.BeginUpdate();

                int numberRow = 0;
                if ((listControl.Count % 2) == 0)
                    numberRow = listControl.Count / 2;
                else numberRow = (listControl.Count / 2) + 1;

                layoutControl1.Items = new DevExpress.XtraLayout.Utils.ReadOnlyItemCollection();
                int dem = 1;
                int currentnumberRow = 0;
                bool isFirstPosition = false;
                List<BaseLayoutItem> layoutControlItemAdds = new List<BaseLayoutItem>();
                foreach (var itemControl in listControl)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("itemControl.Name", itemControl.Name));
                    if (currentnumberRow > totalRowLimit)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Truong hop currentnumberRow(" + currentnumberRow + ") > totalRowLimit(" + totalRowLimit + ")" + ", bo qua khong them vao do khong du cho hien thi, se hien thi o muc mo rong");
                        continue;
                    }
                    LayoutControlItem item1 = layoutControlGroup1.AddItem();
                    layoutControlItemAdds.Add(item1);
                    // Bind a control to the layout item.                 
                    item1.Control = itemControl;
                    item1.TextVisible = false;
                    item1.Name = String.Format("lciForItemControlTemp{0}", dem);
                    item1.SizeConstraintsType = SizeConstraintsType.Custom;
                    item1.MaxSize = new System.Drawing.Size(0, 26);
                    item1.MinSize = new System.Drawing.Size(0, 26);

                    if (userControlNameAdded == null)
                        userControlNameAdded = new List<string>();
                    userControlNameAdded.Add(itemControl.Name);

                    if (ucExamHistory != null)
                    {
                        if (itemControl.Name == ucExamHistory.Name)
                        {
                            item1.Height = ucExamHistory.Height;
                            item1.MaxSize = new System.Drawing.Size(0, ucExamHistory.Height);
                            item1.MinSize = new System.Drawing.Size(0, ucExamHistory.Height);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("layoutControl1.Width", layoutControl1.Width)
                                + Inventec.Common.Logging.LogUtil.TraceData("ucExamHistory.Height", ucExamHistory.Height));

                            if (isFirstPosition)
                            {
                                EmptySpaceItem emptySpaceItem = new EmptySpaceItem();
                                layoutControlGroup1.AddItem(emptySpaceItem);
                                emptySpaceItem.Move(layoutControlItemAdds[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                //layoutControlItemAdds.Add(emptySpaceItem);
                            }
                            currentnumberRow += 6;
                            isFirstPosition = false;
                        }
                        else if (isFirstPosition)
                        {
                            item1.Move(layoutControlItemAdds[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                            Inventec.Common.Logging.LogSystem.Debug("item1.Control.Name=" + item1.Control.Name + ",layoutControlItemAdds[dem - 2].Control.Name=" + ((LayoutControlItem)(layoutControlItemAdds[dem - 2])).Control.Name);
                            isFirstPosition = false;
                        }
                        else
                        {
                            isFirstPosition = true;
                            currentnumberRow += 1;
                        }
                        dem++;
                    }

                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => userControlNameAdded), userControlNameAdded)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentnumberRow), currentnumberRow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => totalRowLimit), totalRowLimit)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentnumberRow), currentnumberRow));

                if (currentnumberRow > totalRowLimit) // hien thi toi da 24 control trong vung UCPlusInfo
                {
                    ucExtend1 = new Design.UCPatientExtend();
                    LayoutControlItem item11 = layoutControlGroup1.AddItem("Mở rộng", ucExtend1);
                    item11.TextVisible = false;
                    if (isFirstPosition)
                    {
                        item11.Move(layoutControlItemAdds[layoutControlItemAdds.Count - 1], DevExpress.XtraLayout.Utils.InsertType.Right);
                    }
                    else
                    {
                        EmptySpaceItem emptySpaceItemBtn = new EmptySpaceItem();
                        layoutControlGroup1.AddItem(emptySpaceItemBtn);
                        emptySpaceItemBtn.Move(item11, DevExpress.XtraLayout.Utils.InsertType.Left);
                    }
                }
                EmptySpaceItem emptySpaceItem2 = new EmptySpaceItem();
                layoutControlGroup1.AddItem(emptySpaceItem2);

                layoutControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

    }
}
