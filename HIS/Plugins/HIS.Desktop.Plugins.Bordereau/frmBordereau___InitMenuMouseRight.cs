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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Bordereau.Base;
using HIS.Desktop.Plugins.Bordereau.ChooseCondition;
using HIS.Desktop.Plugins.Bordereau.ChooseEquipmentSet;
using HIS.Desktop.Plugins.Bordereau.ChooseOtherPaySource;
using HIS.Desktop.Plugins.Bordereau.ChoosePatientType;
using HIS.Desktop.Plugins.Bordereau.ChooseService;
using HIS.Desktop.Plugins.Bordereau.ChooseShareCount;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {
        SereServADO sereServChooseService { get; set; }
        long? patientTypeChoose { get; set; }
        long? conditionChoose { get; set; }
        Dictionary<long, long?> dicCondition { get; set; }
        Dictionary<long, long?> primariPatientType { get; set; }
        Dictionary<long, long?> dicOtherPaySource { get; set; }
        long? otherPaySourceChoose { get; set; }
        long? shareCount { get; set; }

        public enum ProcessType
        {
            EXECUTE_IS_EXPEND,
            EXECUTE_IS_NOT_EXPEND,
            EXECUTE_IS_EXECUTE,
            EXECUTE_IS_NOT_EXECUTE,
            EXECUTE_IS_OUT_PARENT_FEE,
            EXECUTE_IS_NOT_OUT_PARENT_FEE,
            EXECUTE_SET_PARENT_ID,
            EXECUTE_SET_NOT_PARENT_ID,
            EXECUTE_SET_PATIENT_TYPE,
            EXECUTE_SET_PRIMARY_PATIENT_TYPE,
            EXECUTE_SET_EQUIPMENT_SET,
            EXECUTE_SET_SHARE_COUNT,
            EXECUTE_IS_EXPEND_TYPE,
            EXECUTE_IS_NOT_EXPEND_TYPE,
            EXECUTE_IS_FUN_ACCEPT,
            EXECUTE_IS_NOT_FUN_ACCEPT,
            EXECUTE_SET_OTHER_PAY_SOURCE,
            EXECUTE_IS_NOT_USE_BHYT,
            EXECUTE_SET_SERVICE_CONDITION
        }

        private void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager1);
                // Add item and show
                menu.ItemLinks.Clear();
                if (CheckExistServiceKTCOrPTTT())
                {
                    BarButtonItem itemSetParent = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemSetParent", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemSetParent.Tag = ProcessType.EXECUTE_SET_PARENT_ID;
                    itemSetParent.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemSetParent });
                }

                if (CheckExistParent())
                {
                    BarButtonItem itemHasParent = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemHasParent", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemHasParent.Tag = ProcessType.EXECUTE_SET_NOT_PARENT_ID;
                    itemHasParent.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemHasParent });
                }

                if (currentTreatment != null && currentTreatment.FUND_ID > 0)
                {
                    if (this.CheckExistIsFunAccept())
                    {
                        BarButtonItem itemFunAccept = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemFunAccept", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                        itemFunAccept.Tag = ProcessType.EXECUTE_IS_NOT_FUN_ACCEPT;
                        itemFunAccept.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                        menu.AddItems(new BarButtonItem[] { itemFunAccept });
                    }

                    if (this.CheckExistIsNotFunAccept())
                    {
                        BarButtonItem itemNotFunAccept = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemNotFunAccept", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                        itemNotFunAccept.Tag = ProcessType.EXECUTE_IS_FUN_ACCEPT;
                        itemNotFunAccept.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                        menu.AddItems(new BarButtonItem[] { itemNotFunAccept });
                    }
                }

                if (this.currentTreatment.IS_ACTIVE != 0 &&
                    this.CheckExistIsExpend() && this.CheckPremissionEditAll(ProcessType.EXECUTE_IS_NOT_EXPEND, false))
                {
                    BarButtonItem itemExpend = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemExpend", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemExpend.Tag = ProcessType.EXECUTE_IS_NOT_EXPEND;
                    itemExpend.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemExpend });
                }

                if (this.currentTreatment.IS_ACTIVE != 0 && this.CheckExistIsNotExpend() && this.CheckPremissionEditAll(ProcessType.EXECUTE_IS_EXPEND, false))
                {
                    BarButtonItem itemNotExpend = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemNotExpend", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemNotExpend.Tag = ProcessType.EXECUTE_IS_EXPEND;
                    itemNotExpend.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemNotExpend });
                }

                if (this.CheckExistExpendAndNoParent())
                {
                    BarButtonItem itemExpendType = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemExpendType", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemExpendType.Tag = ProcessType.EXECUTE_IS_NOT_EXPEND_TYPE;
                    itemExpendType.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemExpendType });
                }

                if (this.CheckNotExistExpendAndNoParent())
                {
                    BarButtonItem itemExpendType = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemNotExpendType", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemExpendType.Tag = ProcessType.EXECUTE_IS_EXPEND_TYPE;
                    itemExpendType.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemExpendType });
                }

                if (this.currentTreatment.IS_ACTIVE != 0 && this.CheckExistIsNoExecute() && this.CheckPremissionEditAll(ProcessType.EXECUTE_IS_EXECUTE, false))
                {
                    if (!CheckIsThuocVatTuMau())
                    {
                        BarButtonItem itemNoExecute = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemNoExecute", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                        itemNoExecute.Tag = ProcessType.EXECUTE_IS_EXECUTE;
                        itemNoExecute.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                        menu.AddItems(new BarButtonItem[] { itemNoExecute });
                    }
                }

                if (this.currentTreatment.IS_ACTIVE != 0 && this.CheckExistIsNotNoExecute() && this.CheckPremissionEditAll(ProcessType.EXECUTE_IS_NOT_EXECUTE, false))
                {
                    if (!CheckIsThuocVatTuMau())
                    {
                        BarButtonItem itemExecute = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemExecute", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                        itemExecute.Tag = ProcessType.EXECUTE_IS_NOT_EXECUTE;
                        itemExecute.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                        menu.AddItems(new BarButtonItem[] { itemExecute });
                    }
                }

                if (this.currentTreatment.IS_ACTIVE != 0 && this.CheckExistIsNotOutParentFee() && this.CheckPremissionEditAll(ProcessType.EXECUTE_IS_OUT_PARENT_FEE, false))
                {
                    BarButtonItem itemIsNotOutParentFee = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemIsNotOutParentFee", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemIsNotOutParentFee.Tag = ProcessType.EXECUTE_IS_OUT_PARENT_FEE;
                    itemIsNotOutParentFee.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemIsNotOutParentFee });
                }

                if (this.currentTreatment.IS_ACTIVE != 0 && this.CheckExistIsOutParentFee() && this.CheckPremissionEditAll(ProcessType.EXECUTE_IS_NOT_OUT_PARENT_FEE, false))
                {
                    BarButtonItem itemIsOutParentFee = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemIsOutParentFee", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemIsOutParentFee.Tag = ProcessType.EXECUTE_IS_NOT_OUT_PARENT_FEE;
                    itemIsOutParentFee.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemIsOutParentFee });
                }

                BarButtonItem itemDTTT = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemDTTT", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                itemDTTT.Tag = ProcessType.EXECUTE_SET_PATIENT_TYPE;
                itemDTTT.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                menu.AddItems(new BarButtonItem[] { itemDTTT });

                if (CheckIsThuocVatTuMau())
                {
                    BarButtonItem itemServiceCondition = new BarButtonItem(barManager1, "Điều kiện thanh toán", 1);
                    itemServiceCondition.Tag = ProcessType.EXECUTE_SET_SERVICE_CONDITION;
                    itemServiceCondition.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemServiceCondition });
                }


                BarButtonItem itemDTTTPhuTthu = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemDTTTPhuTthu", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                itemDTTTPhuTthu.Tag = ProcessType.EXECUTE_SET_PRIMARY_PATIENT_TYPE;
                itemDTTTPhuTthu.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                menu.AddItems(new BarButtonItem[] { itemDTTTPhuTthu });

                if (!CheckExistIsNotGiuong())
                {
                    BarButtonItem itemShareCount = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemShareCount", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemShareCount.Tag = ProcessType.EXECUTE_SET_SHARE_COUNT;
                    itemShareCount.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemShareCount });
                }

                if (!CheckExistIsNotMetarial())
                {
                    BarButtonItem itemEquipment = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.ItemEquipment", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemEquipment.Tag = ProcessType.EXECUTE_SET_EQUIPMENT_SET;
                    itemEquipment.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemEquipment });
                }

                if (this.currentTreatment.IS_ACTIVE != 0)
                {
                    BarButtonItem itemOtherPaySource = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.itemOtherPaySource", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemOtherPaySource.Tag = ProcessType.EXECUTE_SET_OTHER_PAY_SOURCE;
                    itemOtherPaySource.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemOtherPaySource });
                }

                if (this.currentTreatment.IS_ACTIVE != 0 && !CheckExistIsNotUseBHYT())
                {
                    BarButtonItem itemIsNotUseBHYT = new BarButtonItem(barManager1, Inventec.Common.Resource.Get.Value("frmBordereau.PopupMenu.itemIsNotUseBHYT", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), 1);
                    itemIsNotUseBHYT.Tag = ProcessType.EXECUTE_IS_NOT_USE_BHYT;
                    itemIsNotUseBHYT.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemIsNotUseBHYT });
                }

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setExecuteProcessMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.sereServChooseService = null;
                this.patientTypeChoose = null;
                this.primariPatientType = null;
                this.dicOtherPaySource = null;
                this.shareCount = null;
                this.otherPaySourceChoose = null;

                List<SereServADO> sereServADOSelecteds = GetSereServSelected();
                if (sereServADOSelecteds == null || sereServADOSelecteds.Count == 0)
                    throw new Exception("Khong co dich vu nao duoc chon!");

                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                var btn = e.Item as BarButtonItem;
                ProcessType processType = (ProcessType)btn.Tag;
                ProcessInitDataAndExecuteMenu(sereServADOSelecteds, processType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessInitDataAndExecuteMenu(List<SereServADO> sereServADOSelecteds, ProcessType processType)
        {
            try
            {
                long? equipmentId = null;
                long? numOrder = null;
                List<long> sereServIds = sereServADOSelecteds.Select(o => o.ID).ToList();
                List<long> serviceIds = sereServADOSelecteds.Select(o => o.SERVICE_ID).ToList();
                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                UpdateField updateField = new UpdateField();

                if (!this.CheckPremissionEditAll(processType, true))
                    return;

                if (processType == ProcessType.EXECUTE_SET_PARENT_ID)
                {
                    //Dich vu dang dang chon de dinh kem. Neu dang duoc dinh kiem thi hien thi mac dinh
                    //Neu co 2 dich vu dang duoc dinh kem thi hien thi cai dau tien

                    frmChooseService frmChooseService = new frmChooseService(this.SereServADOs, sereServADOSelecteds, currentDepartmentId, RefeshDataChooseService);
                    frmChooseService.ShowDialog();
                    if (this.sereServChooseService == null)
                        throw new Exception("Chua chon duoc dich vu pttt");
                    if (sereServIds.Contains(this.sereServChooseService.ID))
                    {
                        MessageBox.Show(string.Format(Inventec.Common.Resource.Get.Value("CacDichVuBanChonCoDVPTKhongChoPhepThucHien", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), sereServChooseService.TDL_SERVICE_CODE), "Thông báo", MessageBoxButtons.OK);
                        return;
                    }

                    //Kiem tra ton tai dich vu pt
                    SereServADO sereServADO = sereServADOSelecteds.FirstOrDefault(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                    if (sereServADO != null)
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(Inventec.Common.Resource.Get.Value("CacDichVuBanChonCoDVPTBanCoChacMuonGanVaoGoiKhong", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != DialogResult.OK)
                        {
                            return;
                        }
                    }
                }
                else if (processType == ProcessType.EXECUTE_IS_NOT_EXPEND)
                {
                    int[] selectRows = gridViewBordereau.GetSelectedRows();
                    List<SereServADO> sereServSelecteds = new List<SereServADO>();
                    if (selectRows != null && selectRows.Count() > 0)
                    {
                        for (int i = 0; i < selectRows.Count(); i++)
                        {
                            var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                            sereServSelecteds.Add(sereServADO);
                        }
                    }

                    if (SereServADOs != null && SereServADOs.Count > 0)
                    {
                        var ss = SereServADOs.Where(o => o.IS_EXPEND == 1 && o.EXPEND_TYPE_ID == 1 && o.PARENT_ID == null && sereServSelecteds.Contains(o)).ToList();
                        if (ss != null && ss.Count > 0)
                        {
                            string sereServNotAllowStr = "";
                            foreach (var item in ss)
                            {
                                sereServNotAllowStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                            }

                            MessageBox.Show(string.Format(Inventec.Common.Resource.Get.Value("CacDichVuDaDuocTichHPVaHPTGKhongTheBoTichHPTruocKhiBoTichHPTG", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), sereServNotAllowStr), "Thông báo");
                            return;
                        }
                    }
                }
                else if (processType == ProcessType.EXECUTE_IS_EXECUTE)
                {
                    List<HIS_SERE_SERV> sereServWithMinDurations = this.GetSereServWithMinDuration(currentTreatment.PATIENT_ID, serviceIds);
                    if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
                    {
                        string sereServMinDurationStr = "";
                        foreach (var item in sereServWithMinDurations)
                        {
                            sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                        }

                        if (MessageBox.Show(string.Format(Inventec.Common.Resource.Get.Value("CacDichVuSauCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture()), sereServMinDurationStr), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                }
                else if (processType == ProcessType.EXECUTE_SET_PATIENT_TYPE)
                {
                    patientTypeChoose = null;
                    primariPatientType = null;
                    dicOtherPaySource = null;
                    frmChoosePatientType frmChoosePatientType = new frmChoosePatientType(currentTreatment, sereServADOSelecteds, currentHisPatientTypeAlters, ReLoadChoosePatientType);
                    frmChoosePatientType.ShowDialog();

                    if (!patientTypeChoose.HasValue || patientTypeChoose == 0)
                    {
                        return;
                    }
                }
                else if (processType == ProcessType.EXECUTE_SET_OTHER_PAY_SOURCE)
                {
                    otherPaySourceChoose = null;
                    frmChooseOtherPaySource frmChooseOtherPaySource = new frmChooseOtherPaySource(sereServADOSelecteds, ReLoadOtherPaySource);
                    frmChooseOtherPaySource.ShowDialog();
                    if (!otherPaySourceChoose.HasValue)
                    {
                        return;
                    }

                }
                else if (processType == ProcessType.EXECUTE_SET_PRIMARY_PATIENT_TYPE)
                {
                    patientTypeChoose = null;
                    primariPatientType = null;
                    dicOtherPaySource = null;
                    frmChoosePatientType frmChoosePatientType = new frmChoosePatientType(currentTreatment, sereServADOSelecteds, currentHisPatientTypeAlters, ReLoadChoosePatientType);
                    frmChoosePatientType.ShowDialog();

                    if (!patientTypeChoose.HasValue || patientTypeChoose == 0)
                    {
                        return;
                    }
                }
                else if (processType == ProcessType.EXECUTE_SET_SHARE_COUNT)
                {
                    shareCount = null;
                    if (this.CheckExistServiceNotGiuong(sereServADOSelecteds))
                        return;
                    frmChooseShareCount frmShareCount = new frmChooseShareCount(ReLoadChooseShareCount);
                    frmShareCount.ShowDialog();

                    if (!shareCount.HasValue || shareCount == 0)
                    {
                        return;
                    }
                }
                else if (processType == ProcessType.EXECUTE_SET_EQUIPMENT_SET)
                {
                    frmEquipmentSet frm = new frmEquipmentSet(null, null, HIS.Desktop.Plugins.Bordereau.ChooseEquipmentSet.frmEquipmentSet.CONTROL_TYPE.HIDE_NUMORDER);
                    frm.ShowDialog();
                    if (!frm.success)
                    {
                        return;
                    }

                    equipmentId = frm.equipmentId;
                    numOrder = frm.numOrder;
                }
                else if (processType == ProcessType.EXECUTE_SET_SERVICE_CONDITION)
                {
                    if (sereServADOSelecteds.GroupBy(o => o.SERVICE_ID).Count() > 1)
                    {
                        XtraMessageBox.Show("Không cho phép chọn dịch vụ khác nhau", "Thông báo");
                        return;
                    }
                    frmChooseCondition frmChooseCondition = new frmChooseCondition(sereServADOSelecteds, RefeshDataChooseCondition, currentTreatment.ID);
                    frmChooseCondition.ShowDialog();
                    if (!this.conditionChoose.HasValue || this.conditionChoose == 0)
                        return;
                }

                switch (processType)
                {
                    case ProcessType.EXECUTE_SET_PATIENT_TYPE:

                        string serviceNamesString = "";
                        bool isValid = true;
                        long? ptBillPatientTYpe = null;
                        List<string> lstOtherPaySource = new List<string>();
                        primariPatientType = new Dictionary<long, long?>();
                        dicOtherPaySource = new Dictionary<long, long?>();
                        dicCondition = new Dictionary<long, long?>();
                        foreach (var sereServSelected in sereServADOSelecteds)
                        {
                            primariPatientType[sereServSelected.ID] = sereServSelected.PRIMARY_PATIENT_TYPE_ID;
                            dicCondition[sereServSelected.ID] = sereServSelected.SERVICE_CONDITION_ID;
                            var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == sereServSelected.SERVICE_ID);
                            if ((this.patientTypeChoose ?? 0) > 0)
                            {
                                var pt = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                                                        .FirstOrDefault(o => o.ID == this.patientTypeChoose);
                                var oldPt = BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                                                        .FirstOrDefault(o => o.ID == sereServSelected.PATIENT_TYPE_ID);
                                if (pt != null)
                                {
                                    if (pt.PATIENT_TYPE_CODE == Config.HisPatientTypeCFG.PATIENT_TYPE_CODE__BHYT && sereServSelected.IS_NOT_USE_BHYT == 1)
                                    {
                                        string message = String.Format("Không cho phép đổi sang ĐTTT BHYT do dịch vụ {0} - {1}(Mã y lệnh: {2}) đã được người chỉ định tích 'Không hưởng BHYT'", service.SERVICE_CODE, service.SERVICE_NAME, sereServSelected.TDL_SERVICE_REQ_CODE);
                                        DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                                        return;
                                    }
                                    if (service != null && service.BILL_PATIENT_TYPE_ID != null && service.IS_NOT_CHANGE_BILL_PATY == 1
                                        && (string.IsNullOrEmpty(service.APPLIED_PATIENT_TYPE_IDS) || IsContainString(service.APPLIED_PATIENT_TYPE_IDS, pt.ID.ToString()))
                                        && ((oldPt != null && oldPt.ID == service.BILL_PATIENT_TYPE_ID && pt.ID != oldPt.ID)
                                || IsContainString(service.APPLIED_PATIENT_TYPE_IDS, pt.ID.ToString()))
                                && (string.IsNullOrEmpty(service.APPLIED_PATIENT_CLASSIFY_IDS) || IsContainString(service.APPLIED_PATIENT_CLASSIFY_IDS, this.currentTreatment.TDL_PATIENT_CLASSIFY_ID != null ? this.currentTreatment.TDL_PATIENT_CLASSIFY_ID.ToString() : "-1"))
                                && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.IS_SET_PRIMARY_PATIENT_TYPE) == "1"
                                        )
                                    {
                                        serviceNamesString += service.SERVICE_NAME;
                                        serviceNamesString += ", ";
                                        isValid = false;
                                        primariPatientType[sereServSelected.ID] = service.BILL_PATIENT_TYPE_ID;
                                    }
                                    if ((!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) || !string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS)))
                                    {
                                        List<string> lstPt = new List<string>();
                                        List<string> lstOldPt = new List<string>();
                                        if (!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS))
                                            lstPt = pt.OTHER_PAY_SOURCE_IDS.Split(',').ToList();
                                        if (!string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS))
                                            lstOldPt = oldPt.OTHER_PAY_SOURCE_IDS.Split(',').ToList();
                                        if (((!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS))
                                            || (string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && !string.IsNullOrEmpty(oldPt.OTHER_PAY_SOURCE_IDS))
                                            || lstPt.Where(o => lstOldPt.Exists(p => p == o)).ToList() == null
                                                || lstPt.Where(o => lstOldPt.Exists(p => p == o)).ToList().Count == 0)
                                             && ((!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && sereServSelected.OTHER_PAY_SOURCE_ID != null && lstPt.FirstOrDefault(o => Int64.Parse(o) == sereServSelected.OTHER_PAY_SOURCE_ID) == null)
                                                || (string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && sereServSelected.OTHER_PAY_SOURCE_ID != null)
                                                || (!string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS) && sereServSelected.OTHER_PAY_SOURCE_ID == null)))
                                        {
                                            if (string.IsNullOrEmpty(pt.OTHER_PAY_SOURCE_IDS))
                                                dicOtherPaySource[sereServSelected.ID] = null;
                                            else
                                                dicOtherPaySource[sereServSelected.ID] = Int64.Parse(lstPt[0]);
                                        }
                                    }
                                }
                            }
                        }

                        if (!isValid)
                        {
                            serviceNamesString = serviceNamesString.Substring(0, serviceNamesString.Length - 2);

                            string message = String.Format("Dịch vụ {0} có thông tin đối tượng bắt buộc. Phần mềm sẽ tự động cập nhật đối tượng thanh toán và đối tượng phụ thu tương ứng.\nBạn có muốn tiếp tục?", serviceNamesString);
                            if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                            {
                                btnFind_Click(null, null);
                                return;
                            }
                        }
                        break;
                    default:
                        break;
                }


                foreach (var id in sereServIds)
                {
                    HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                    sereServ.ID = id;
                    var ssItem = sereServADOSelecteds.FirstOrDefault(o => o.ID == id);
                    if (ssItem != null)
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServ, ssItem);


                    switch (processType)
                    {
                        case ProcessType.EXECUTE_IS_FUN_ACCEPT:
                            sereServ.IS_FUND_ACCEPTED = 1;
                            updateField = UpdateField.IS_FUND_ACCEPTED;//TODO
                            break;
                        case ProcessType.EXECUTE_IS_NOT_FUN_ACCEPT:
                            sereServ.IS_FUND_ACCEPTED = null;
                            updateField = UpdateField.IS_FUND_ACCEPTED;//TODO
                            break;
                        case ProcessType.EXECUTE_IS_EXPEND:
                            sereServ.IS_EXPEND = 1;
                            updateField = UpdateField.IS_EXPEND;
                            break;
                        case ProcessType.EXECUTE_IS_NOT_EXPEND:
                            sereServ.IS_EXPEND = null;
                            updateField = UpdateField.IS_EXPEND;
                            break;
                        case ProcessType.EXECUTE_IS_EXPEND_TYPE:
                            sereServ.EXPEND_TYPE_ID = 1;
                            updateField = UpdateField.EXPEND_TYPE_ID;
                            break;
                        case ProcessType.EXECUTE_IS_NOT_EXPEND_TYPE:
                            sereServ.EXPEND_TYPE_ID = null;
                            updateField = UpdateField.EXPEND_TYPE_ID;
                            break;
                        case ProcessType.EXECUTE_IS_EXECUTE:
                            sereServ.IS_NO_EXECUTE = null;
                            updateField = UpdateField.IS_NO_EXECUTE;
                            break;
                        case ProcessType.EXECUTE_IS_NOT_EXECUTE:
                            sereServ.IS_NO_EXECUTE = 1;
                            updateField = UpdateField.IS_NO_EXECUTE;
                            break;
                        case ProcessType.EXECUTE_IS_NOT_USE_BHYT:
                            sereServ.IS_NOT_USE_BHYT = 1;
                            updateField = UpdateField.IS_NOT_USE_BHYT;
                            break;
                        case ProcessType.EXECUTE_IS_OUT_PARENT_FEE:
                            sereServ.IS_OUT_PARENT_FEE = 1;
                            updateField = UpdateField.IS_OUT_PARENT_FEE;
                            break;
                        case ProcessType.EXECUTE_IS_NOT_OUT_PARENT_FEE:
                            sereServ.IS_OUT_PARENT_FEE = null;
                            updateField = UpdateField.IS_OUT_PARENT_FEE;
                            break;
                        case ProcessType.EXECUTE_SET_PARENT_ID:
                            sereServ.PARENT_ID = this.sereServChooseService.ID;
                            updateField = UpdateField.PARENT_ID;
                            break;
                        case ProcessType.EXECUTE_SET_NOT_PARENT_ID:
                            sereServ.PARENT_ID = null;
                            updateField = UpdateField.PARENT_ID;
                            break;
                        case ProcessType.EXECUTE_SET_PATIENT_TYPE:
                            sereServ.PRIMARY_PATIENT_TYPE_ID = this.primariPatientType.ContainsKey(id) ? primariPatientType[id] : null;
                            sereServ.SERVICE_CONDITION_ID = this.dicCondition.ContainsKey(id) ? dicCondition[id] : null;
                            sereServ.PATIENT_TYPE_ID = this.patientTypeChoose ?? 0;
                            updateField = UpdateField.PATIENT_TYPE_ID;
                            break;
                        case ProcessType.EXECUTE_SET_OTHER_PAY_SOURCE:
                            sereServ.OTHER_PAY_SOURCE_ID = this.otherPaySourceChoose != 0 ? this.otherPaySourceChoose : null;
                            updateField = UpdateField.OTHER_PAY_SOURCE_ID;
                            break;
                        case ProcessType.EXECUTE_SET_PRIMARY_PATIENT_TYPE:
                            sereServ.PRIMARY_PATIENT_TYPE_ID = this.patientTypeChoose ?? 0;
                            updateField = UpdateField.PRIMARY_PATIENT_TYPE_ID;
                            break;
                        case ProcessType.EXECUTE_SET_SHARE_COUNT:
                            sereServ.SHARE_COUNT = this.shareCount;
                            updateField = UpdateField.SHARE_COUNT;
                            break;
                        case ProcessType.EXECUTE_SET_EQUIPMENT_SET:
                            sereServ.EQUIPMENT_SET_ID = equipmentId;
                            sereServ.EQUIPMENT_SET_ORDER = numOrder;
                            updateField = UpdateField.EQUIPMENT_SET_ORDER__AND__EQUIPMENT_SET_ID;
                            break;
                        case ProcessType.EXECUTE_SET_SERVICE_CONDITION:
                            sereServ.SERVICE_CONDITION_ID = this.conditionChoose != 0 ? this.conditionChoose : null;
                            updateField = UpdateField.SERVICE_CONDITION_ID;
                            break;
                        default:
                            break;
                    }
                    sereServs.Add(sereServ);
                }
                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                hisSereServPayslipSDO.Field = updateField;
                hisSereServPayslipSDO.SereServs = sereServs;
                hisSereServPayslipSDO.TreatmentId = currentTreatment.ID;

                bool result = UpdatePayslipInfoProcess(hisSereServPayslipSDO);
                if (result && processType == ProcessType.EXECUTE_SET_PATIENT_TYPE && dicOtherPaySource != null && dicOtherPaySource.Count > 0)
                {
                    List<string> lstSereName = new List<string>();
                    foreach (var item in sereServs)
                    {
                        if (dicOtherPaySource.ContainsKey(item.ID))
                        {
                            item.OTHER_PAY_SOURCE_ID = dicOtherPaySource[item.ID];
                            lstSereName.Add(sereServADOSelecteds.FirstOrDefault(o => o.ID == item.ID).TDL_SERVICE_NAME);
                        }
                    }
                    string message = String.Format("Bạn có muốn thay đổi thông tin nguồn khác chi trả của dịch vụ {0} hay không?", String.Join(",", lstSereName));
                    if (MessageBox.Show(message, "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        return;
                    }
                    HisSereServPayslipSDO hisSereServOtherPayslipSDO = new HisSereServPayslipSDO();
                    hisSereServOtherPayslipSDO.Field = UpdateField.OTHER_PAY_SOURCE_ID;
                    hisSereServOtherPayslipSDO.SereServs = sereServs;
                    hisSereServOtherPayslipSDO.TreatmentId = currentTreatment.ID;

                    UpdatePayslipInfoProcess(hisSereServOtherPayslipSDO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadChooseShareCount(object data)
        {
            try
            {
                if (data != null)
                {
                    this.shareCount = (long?)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadChoosePatientType(object data)
        {
            try
            {
                if (data != null)
                {
                    patientTypeChoose = (long?)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadOtherPaySource(object data)
        {
            try
            {
                if (data != null)
                {
                    otherPaySourceChoose = (long?)data;
                }
                else
                {
                    otherPaySourceChoose = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RefeshDataChooseCondition(object data)
        {
            try
            {
                if (data != null)
                {
                    this.conditionChoose = (long?)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataChooseService(object data)
        {
            try
            {
                if (data != null && data is SereServADO)
                {
                    this.sereServChooseService = data as SereServADO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckIsThuocVatTuMau()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                foreach (var item in sereServSelecteds)
                {
                    var serviceType = Services.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                    if (serviceType == null) return false;
                    if (serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || serviceType.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                        if (serviceType != null)
                        {
                            result = true;
                        }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistServiceKTCOrPTTT()
        {
            bool result = false;
            try
            {
                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA) && o.IS_NO_EXECUTE != 1).ToList();

                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistParent()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (sereServSelecteds != null && sereServSelecteds.Count > 0)
                {
                    var ss = sereServSelecteds.Where(o => o.PARENT_ID.HasValue).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsExpend()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_EXPEND == 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckAllowPopupMenuShow()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_EXPEND == 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistExpendAndNoParent()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_EXPEND == 1 && o.EXPEND_TYPE_ID == 1 && o.PARENT_ID == null && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckNotExistExpendAndNoParent()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_EXPEND == 1 && o.EXPEND_TYPE_ID != 1 && o.PARENT_ID == null && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotFunAccept()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_FUND_ACCEPTED != 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotExpend()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_EXPEND != 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNoExecute()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_NO_EXECUTE == 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotNoExecute()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_NO_EXECUTE != 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotUseBHYT()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.PATIENT_TYPE_ID == Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsOutParentFee()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.PARENT_ID.HasValue && o.IS_OUT_PARENT_FEE == 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0 && sereServSelecteds.Count == ss.Count)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotGiuong()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                SereServADO sereServADONotG = sereServSelecteds.FirstOrDefault(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G);
                if (sereServADONotG != null)
                    result = true;
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsFunAccept()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.IS_FUND_ACCEPTED == 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotMetarial()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                SereServADO sereServADONotVT = sereServSelecteds.FirstOrDefault(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                if (sereServADONotVT != null)
                    result = true;
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistServiceNotGiuong(List<SereServADO> sereServADOs)
        {
            bool result = false;
            try
            {
                SereServADO sereServADO = sereServADOs.FirstOrDefault(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G);
                if (sereServADO != null)
                {
                    MessageBox.Show("Tồn tại dịch vụ không phải giường");
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistIsNotOutParentFee()
        {
            bool result = false;
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServADO);
                    }
                }

                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    var ss = SereServADOs.Where(o => o.PARENT_ID.HasValue && o.IS_OUT_PARENT_FEE != 1 && sereServSelecteds.Contains(o)).ToList();
                    if (ss != null && ss.Count > 0 && sereServSelecteds.Count == ss.Count)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckPremissionEditAll(ProcessType processType, bool isShowErr)
        {
            bool result = true;
            try
            {
                //Set columnType
                ComlumnType columnType;
                if (processType == ProcessType.EXECUTE_IS_EXECUTE || processType == ProcessType.EXECUTE_IS_NOT_EXECUTE)
                    columnType = ComlumnType.NO_EXECUTE;
                else if (processType == ProcessType.EXECUTE_IS_EXPEND || processType == ProcessType.EXECUTE_IS_NOT_EXPEND)
                    columnType = ComlumnType.EXPEND;
                else if (processType == ProcessType.EXECUTE_IS_OUT_PARENT_FEE || processType == ProcessType.EXECUTE_IS_OUT_PARENT_FEE)
                    columnType = ComlumnType.IS_OUT_PARENT_FEE;
                else if (processType == ProcessType.EXECUTE_IS_EXPEND_TYPE || processType == ProcessType.EXECUTE_IS_NOT_EXPEND_TYPE)
                    columnType = ComlumnType.EXPEND_TYPE_ID;
                else
                    columnType = ComlumnType.NO_CHECK_COLUMN;

                int[] selectRows = gridViewBordereau.GetSelectedRows();
                List<SereServADO> sereServSelecteds = new List<SereServADO>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServ = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        sereServSelecteds.Add(sereServ);
                    }
                }

                string serviceNameCustom = "";
                foreach (var item in sereServSelecteds)
                {
                    string mess = "";
                    result = this.CheckPremissionEdit(item, columnType, ref mess) && result;
                    if (!String.IsNullOrEmpty(mess))
                    {
                        serviceNameCustom += (mess + ",\n");
                    }
                }

                if (!result)
                {
                    if (isShowErr)
                        MessageBox.Show(String.Format("Dịch vụ không được quyền sửa\n\r {0}", serviceNameCustom), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<SereServADO> GetSereServSelected()
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                int[] selectRows = gridViewBordereau.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sereServADO = (SereServADO)gridViewBordereau.GetRow(selectRows[i]);
                        result.Add(sereServADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool UpdatePayslipInfoProcess(HisSereServPayslipSDO hisSereServPayslipSDO)
        {
            bool result = false;
            try
            {
                if (hisSereServPayslipSDO == null)
                    return result;

                if (serviceReqs != null && serviceReqs.Count > 0 && hisSereServPayslipSDO.Field == UpdateField.PATIENT_TYPE_ID
                    && hisSereServPayslipSDO.SereServs != null && hisSereServPayslipSDO.SereServs.Count > 0
                    && hisSereServPayslipSDO.SereServs.Exists(o => o.PATIENT_TYPE_ID == HIS.Desktop.Plugins.Bordereau.Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT))
                {
                    List<SereServADO> lstSs = SereServADOs.Where(o => hisSereServPayslipSDO.SereServs.Select(s => s.ID).Contains(o.ID)).ToList();
                    List<HIS_SERVICE_REQ> lstServiceReq = serviceReqs.Where(o => lstSs.Select(s => s.SERVICE_REQ_ID ?? 0).Contains(o.ID)).ToList();
                    if (lstServiceReq != null && lstServiceReq.Exists(o => o.IS_NOT_USE_BHYT == 1))
                    {
                        string messError = "Không cho phép đổi sang đối tượng thanh toán BHYT với các dịch vụ \"Không được hưởng BHYT\": ";
                        foreach (var item in lstServiceReq)
                        {
                            messError += string.Format("{0}({1})", string.Join(",", lstSs.Where(o => o.SERVICE_REQ_ID == item.ID).Select(s => s.TDL_SERVICE_CODE)), item.SERVICE_REQ_CODE) + "; ";
                        }

                        messError = messError.Substring(0, messError.Length - 2);
                        XtraMessageBox.Show(messError);
                        return result;
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisSereServPayslipSDO), hisSereServPayslipSDO));
                List<HIS_SERE_SERV> sereServResults = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, hisSereServPayslipSDO, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServResults), sereServResults));
                WaitingManager.Hide();
                if (sereServResults != null && sereServResults.Count > 0)
                {
                    result = true;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServResults), sereServResults));
                    ReloadDataToGridAndPrint(sereServResults);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("UpdatePayslipInfoProcess fail => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServResults), sereServResults));
                }

                MessageManager.Show(this, param, result);
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ReloadDataToGridAndPrint(List<HIS_SERE_SERV> sereServResults)
        {
            try
            {
                if (this.gridViewBordereau.IsEditing)
                    this.gridViewBordereau.CloseEditor();

                if (this.gridViewBordereau.FocusedRowModified)
                    this.gridViewBordereau.UpdateCurrentRow();

                if (sereServResults != null && sereServResults.Count > 0)
                {
                    List<SereServADO> sereServADODisplay = new List<SereServADO>();
                    //Inventec.Common.Logging.LogSystem.Debug("ReloadDataToGridAndPrint. sereServResults.Count > 0");
                    var equipmentSets = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true);
                    foreach (var item in this.SereServADOs)
                    {
                        HIS_SERE_SERV sereServ = sereServResults.FirstOrDefault(o => o.ID == item.ID);
                        if (sereServ != null)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(item, sereServ);

                            if (item.EQUIPMENT_SET_ID > 0)
                            {
                                HIS_EQUIPMENT_SET equipmentSet = equipmentSets != null ? equipmentSets.FirstOrDefault(o => o.ID == (item.EQUIPMENT_SET_ID ?? 0)) : null;
                                if (equipmentSet != null)
                                {
                                    string numOrder = item.EQUIPMENT_SET_ORDER.HasValue ? String.Format("({0})", item.EQUIPMENT_SET_ORDER.Value) : "";
                                    item.EQUIPMENT_SET_NAME__NUM_ORDER = String.Format("{0} {1}", equipmentSet.EQUIPMENT_SET_NAME, numOrder);
                                }
                            }
                            else
                            {
                                item.EQUIPMENT_SET_NAME__NUM_ORDER = null;
                            }
                        }
                    }
                    sereServADODisplay.AddRange(this.SereServADOs);
                    if (chkAssignBlood.Checked)
                    {
                        HisExpMestBltyReqView2Filter ft = new HisExpMestBltyReqView2Filter();
                        ft.TDL_TREATMENT_ID = this.currentTreatment.ID;
                        ft.EXP_MEST_STT_IDs = new List<long> {
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE };
                        var dt = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, ft, null);

                        foreach (var item in dt)
                        {
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            SereServADO ado = new SereServADO(item, service);
                            sereServADODisplay.Add(ado);
                        }

                    }

                    gridControlBordereau.DataSource = null;
                    gridControlBordereau.DataSource = sereServADODisplay;
                    ThreadCustomManager.ThreadResultCallBack(LoadTotalPriceDataToTestServiceReq, CallBackLoadTreatmentFee);
                    FillDataToButtonPrint();
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
