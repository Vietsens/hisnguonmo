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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraBars;
using EMR_MAIN;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Process;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.ConfigKeys;

namespace HIS.Desktop.Plugins.Library.FormMedicalRecord
{
    public class MediRecordMenuPopupProcessor
    {
        private string BUTTON__CONFIG__CODE = "HIS000025";
        private List<HIS_EMR_COVER_TYPE> _LstEmrCoverType = new List<HIS_EMR_COVER_TYPE>();
        private List<SDA_HIDE_CONTROL> _HideControls;
        private EmrInputADO inputAdo;

        public MediRecordMenuPopupProcessor()
        {
            this._HideControls = HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.GetByModule("HIS.Desktop.Plugins.Library.FormMedicalRecord");
            HisConfigCFG.LoadConfig();
        }

        public void InitMenu(PopupMenu _Menu, BarManager barManager, EmrInputADO ado)
        {
            try
            {
                this.inputAdo = ado;

                List<HIS_EMR_COVER_TYPE> types = null;
                long UsingFormVersion = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(SdaConfigKeys.IS_HIDE_COVER_TYPE));
                if (UsingFormVersion != 1 || !ado.EmrCoverTypeId.HasValue)
                {
                    types = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                    if (this.inputAdo.lstEmrCoverTypeId != null && this.inputAdo.lstEmrCoverTypeId.Count > 0)
                    {
                        types = types.Where(o => this.inputAdo.lstEmrCoverTypeId.Contains(o.ID)).ToList();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o =>
                        o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.ROOM_ID == ado.roomId
                        && o.TREATMENT_TYPE_ID == (ado.Treatment != null ? ado.Treatment.TDL_TREATMENT_TYPE_ID : ado.TreatmentTypeId)
                        ).ToList();
                        if (data == null || data.Count <= 0)
                        {
                            data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o =>
                            o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.DEPARTMENT_ID == ado.DepartmentId
                            && o.TREATMENT_TYPE_ID == (ado.Treatment != null ? ado.Treatment.TDL_TREATMENT_TYPE_ID : ado.TreatmentTypeId)
                            ).ToList();
                        }

                        if (data != null && data.Count > 0)
                        {
                            types = types.Where(o => data.Exists(e => e.EMR_COVER_TYPE_ID == o.ID)).ToList();
                        }
                    }
                }

                List<HIS_EMR_FORM> forms = BackendDataWorker.Get<HIS_EMR_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.MENU_POSITION.HasValue).ToList();

                GenerateMenuVo(_Menu, barManager, types);

                GenerateMenuPhieu(_Menu, barManager, forms);

                if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                {
                    ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                    if (c != null)
                    {
                        BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                        bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                        _Menu.ItemLinks.Add(bbtnErm);
                    }
                }

                if (ado.EmrCoverTypeId.HasValue)
                {
                    HIS_EMR_COVER_TYPE coverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().FirstOrDefault(o => o.ID == ado.EmrCoverTypeId);
                    if (coverType != null)
                    {
                        BarButtonItem bbtnEmr2 = new BarButtonItem(barManager, coverType.EMR_COVER_TYPE_NAME, 4);
                        bbtnEmr2.Tag = coverType.EMR_COVER_TYPE_TAG;
                        bbtnEmr2.ItemClick += new ItemClickEventHandler(this.EmrMouseRightsingle_Click);
                        _Menu.AddItem(bbtnEmr2);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitMenuButton(PopupMenu _Menu, BarManager barManager, EmrInputADO ado)
        {
            try
            {
                this.inputAdo = ado;
                if (this.inputAdo.EmrCoverTypeId != null)
                {
                    Emrsingle_Click(this.inputAdo);
                }
                else
                {
                    List<HIS_EMR_COVER_TYPE> types = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                    if (this.inputAdo.lstEmrCoverTypeId != null && this.inputAdo.lstEmrCoverTypeId.Count > 0)
                    {
                        types = types.Where(o => this.inputAdo.lstEmrCoverTypeId.Contains(o.ID)).ToList();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o =>
                        o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.ROOM_ID == ado.roomId
                        && o.TREATMENT_TYPE_ID == (ado.Treatment != null ? ado.Treatment.TDL_TREATMENT_TYPE_ID : ado.TreatmentTypeId)
                        ).ToList();
                        if (data == null || data.Count <= 0)
                        {
                            data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o =>
                            o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.DEPARTMENT_ID == ado.DepartmentId
                            && o.TREATMENT_TYPE_ID == (ado.Treatment != null ? ado.Treatment.TDL_TREATMENT_TYPE_ID : ado.TreatmentTypeId)
                            ).ToList();
                        }

                        if (data != null && data.Count > 0)
                        {
                            types = types.Where(o => data.Exists(e => e.EMR_COVER_TYPE_ID == o.ID)).ToList();
                        }
                    }

                    List<HIS_EMR_FORM> forms = BackendDataWorker.Get<HIS_EMR_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.MENU_POSITION.HasValue).ToList();

                    GenerateMenuVo(_Menu, barManager, types);

                    if (GlobalVariables.AcsAuthorizeSDO != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null)
                    {
                        ACS_CONTROL c = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BUTTON__CONFIG__CODE);
                        if (c != null)
                        {
                            BarButtonItem bbtnErm = new BarButtonItem(barManager, "Thiết lập vỏ bệnh án", 4);
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.FromConfig__RightClick);
                            _Menu.ItemLinks.Add(bbtnErm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FormOpenEmr(long type, EmrInputADO ado, string _MaPhieu)
        {
            try
            {
                MediRecordProcessor processor = new MediRecordProcessor();
                HIS_EMR_COVER_TYPE coverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().FirstOrDefault(o => o.ID == type);
                if (coverType != null)
                {
                    processor.LoadDataEmr((LoaiBenhAnEMR)coverType.EMR_COVER_TYPE_TAG, ado, _MaPhieu);
                }
                else
                {
                    processor.LoadDataEmr(0, ado, _MaPhieu);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Emrsingle_Click(EmrInputADO ado)
        {
            try
            {
                this.inputAdo = ado;
                MediRecordProcessor processor = new MediRecordProcessor();
                HIS_EMR_COVER_TYPE coverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().FirstOrDefault(o => o.ID == this.inputAdo.EmrCoverTypeId);
                if (coverType != null)
                {
                    processor.LoadDataEmr((LoaiBenhAnEMR)coverType.EMR_COVER_TYPE_TAG, this.inputAdo);
                }
                else
                {
                    processor.LoadDataEmr(0, this.inputAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateMenuVo(PopupMenu menu, BarManager barManager, List<HIS_EMR_COVER_TYPE> types)
        {
            try
            {
                _LstEmrCoverType = types;
                if (types != null && types.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("InitMenu Auto Generate Vo Begin: " + types.Count);

                    BarSubItem bbtnEmrCover = new BarSubItem(barManager, "Vỏ bệnh án", 4);

                    types = types.OrderBy(o => String.IsNullOrEmpty(o.EMR_COVER_GROUP_NAME) ? o.EMR_COVER_TYPE_NAME : o.EMR_COVER_GROUP_NAME).ThenBy(o => o.EMR_COVER_TYPE_NAME).ToList();

                    BarSubItem bbtnGroup = null;
                    foreach (var item in types)
                    {
                        if (!String.IsNullOrEmpty(item.EMR_COVER_GROUP_NAME))
                        {
                            if (bbtnGroup == null || bbtnGroup.Caption != item.EMR_COVER_GROUP_NAME)
                            {
                                bbtnGroup = new BarSubItem(barManager, item.EMR_COVER_GROUP_NAME, 4);
                                bbtnEmrCover.AddItems(new BarItem[] { bbtnGroup });
                            }
                        }
                        else if (bbtnGroup != null)
                        {
                            bbtnGroup = null;
                        }

                        BarButtonItem bbtnErm = new BarButtonItem(barManager, item.EMR_COVER_TYPE_NAME, 4);
                        bbtnErm.Tag = item.EMR_COVER_TYPE_TAG;
                        bbtnErm.ItemClick += new ItemClickEventHandler(this.EmrMouseRightsingle_Click);
                        if (bbtnGroup != null)
                        {
                            bbtnGroup.ItemLinks.Add(bbtnErm);
                        }
                        else
                        {
                            bbtnEmrCover.ItemLinks.Add(bbtnErm);
                        }
                    }

                    menu.AddItems(new BarItem[] { bbtnEmrCover });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateMenuPhieu(PopupMenu menu, BarManager barManager, List<HIS_EMR_FORM> forms)
        {
            try
            {
                if (forms != null && forms.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("InitMenu Auto Generate Phieu Begin: " + forms.Count);

                    List<HIS_EMR_FORM> publicForm = forms.Where(o => o.MENU_POSITION == 1).ToList();
                    List<HIS_EMR_FORM> groupForm = forms.Where(o => o.MENU_POSITION == 2).ToList();

                    if (groupForm != null && groupForm.Count > 0)
                    {
                        BarSubItem bbtnEmrForm = new BarSubItem(barManager, "Phiếu vỏ bệnh án", 4);

                        BarSubItem bbtnGroup = null;
                        foreach (var item in groupForm)
                        {
                            if (!String.IsNullOrEmpty(item.EMR_FORM_GROUP_NAME))
                            {
                                if (bbtnGroup == null || bbtnGroup.Caption != item.EMR_FORM_NAME)
                                {
                                    bbtnGroup = new BarSubItem(barManager, item.EMR_FORM_NAME, 4);
                                    bbtnEmrForm.AddItems(new BarItem[] { bbtnGroup });
                                }
                            }
                            else if (bbtnGroup != null)
                            {
                                bbtnGroup = null;
                            }

                            BarButtonItem bbtnErm = new BarButtonItem(barManager, item.EMR_FORM_NAME, 4);
                            bbtnErm.Tag = item.EMR_FORM_CODE;
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.EmrFormMouseRight_Click);
                            if (bbtnGroup != null)
                            {
                                bbtnGroup.ItemLinks.Add(bbtnErm);
                            }
                            else
                            {
                                bbtnEmrForm.ItemLinks.Add(bbtnErm);
                            }
                        }

                        BarButtonItem bbtnOther = new BarButtonItem(barManager, "Khác", 4);
                        bbtnOther.Tag = "";
                        bbtnOther.ItemClick += new ItemClickEventHandler(this.FromPhieu__RightClick);
                        bbtnEmrForm.ItemLinks.Add(bbtnOther);

                        menu.AddItems(new BarItem[] { bbtnEmrForm });
                    }

                    if (publicForm != null && publicForm.Count > 0)
                    {
                        BarSubItem bbtnGroup = null;
                        foreach (var item in publicForm)
                        {
                            if (!String.IsNullOrEmpty(item.EMR_FORM_GROUP_NAME))
                            {
                                if (bbtnGroup == null || bbtnGroup.Caption != item.EMR_FORM_NAME)
                                {
                                    bbtnGroup = new BarSubItem(barManager, item.EMR_FORM_NAME, 4);
                                    menu.AddItems(new BarItem[] { bbtnGroup });
                                }
                            }
                            else if (bbtnGroup != null)
                            {
                                bbtnGroup = null;
                            }

                            BarButtonItem bbtnErm = new BarButtonItem(barManager, item.EMR_FORM_NAME, 4);
                            bbtnErm.Tag = item.EMR_FORM_CODE;
                            bbtnErm.ItemClick += new ItemClickEventHandler(this.EmrFormMouseRight_Click);
                            if (bbtnGroup != null)
                            {
                                bbtnGroup.ItemLinks.Add(bbtnErm);
                            }
                            else
                            {
                                menu.ItemLinks.Add(bbtnErm);
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

        private void FromConfig__RightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                FromConfig.frmConfigHide frm = new FromConfig.frmConfigHide(this._LstEmrCoverType);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FromPhieu__RightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                FromConfig.frmPhieu frm = new FromConfig.frmPhieu(this.inputAdo);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrMouseRightsingle_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                MediRecordProcessor processor = new MediRecordProcessor();
                if (e.Item is BarButtonItem && this.inputAdo != null)
                {
                    long tagValue = (long)(e.Item.Tag);
                    LoaiBenhAnEMR tag = (LoaiBenhAnEMR)tagValue;
                    processor.LoadDataEmr(tag, this.inputAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrFormMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.inputAdo != null)
                {
                    string tag = e.Item.Tag.ToString();
                    MediRecordProcessor processor = new MediRecordProcessor();
                    HIS_EMR_COVER_TYPE coverType = BackendDataWorker.Get<HIS_EMR_COVER_TYPE>().FirstOrDefault(o => o.ID == this.inputAdo.EmrCoverTypeId);

                    if (coverType != null)
                    {
                        processor.LoadDataEmr((LoaiBenhAnEMR)coverType.EMR_COVER_TYPE_TAG, this.inputAdo, tag);
                    }
                    else
                    {
                        processor.LoadDataEmr(0, this.inputAdo, tag);
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
