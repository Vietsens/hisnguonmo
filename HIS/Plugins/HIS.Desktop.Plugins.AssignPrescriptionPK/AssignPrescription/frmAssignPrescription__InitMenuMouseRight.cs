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
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MixedInfusion;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal PopupMenu menu;

        public enum MOUSE_RIGHT_TYPE
        {
            EDIT_DAY_NUM,
            EDIT_EXPEND_TYPE,
            MIXED_INFUSION,
            CANCEL_MIXED_INFUSION
        }

        private void InitMenu()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("InitMenu.1");
                if (menu == null)
                    menu = new PopupMenu(barManager1);
                // Add item and show
                menu.ItemLinks.Clear();
                if (CheckEditDayNum() && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                {
                    Inventec.Common.Logging.LogSystem.Info("InitMenu.2");
                    BarButtonItem itemEditDayNum = new BarButtonItem(barManager1, ResourceMessage.PopupMenu_SuaSoNgay, 1);
                    itemEditDayNum.Tag = MOUSE_RIGHT_TYPE.EDIT_DAY_NUM;
                    itemEditDayNum.ItemClick += new ItemClickEventHandler(setProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemEditDayNum });
                }

                if (CheckEditExpendType() && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                {
                    Inventec.Common.Logging.LogSystem.Info("InitMenu.3");
                    BarButtonItem itemEditExpendType = new BarButtonItem(barManager1, ResourceMessage.PopupMenu_LoaiHaoPhi, 1);
                    itemEditExpendType.Tag = MOUSE_RIGHT_TYPE.EDIT_EXPEND_TYPE;
                    itemEditExpendType.ItemClick += new ItemClickEventHandler(setProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemEditExpendType });
                }

                if (checkMixedInfusion(true))
                {
                    Inventec.Common.Logging.LogSystem.Info("InitMenu.2");
                    BarButtonItem itemMixedInfusion = new BarButtonItem(barManager1, ResourceMessage.PopupMenu_PhaTruyenCungNhau, 1);
                    itemMixedInfusion.Tag = MOUSE_RIGHT_TYPE.MIXED_INFUSION;
                    itemMixedInfusion.ItemClick += new ItemClickEventHandler(setProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemMixedInfusion });
                }

                if (checkMixedInfusion(false))
                {
                    Inventec.Common.Logging.LogSystem.Info("InitMenu.2");
                    BarButtonItem itemCancelMixedInfusion = new BarButtonItem(barManager1, ResourceMessage.PopupMenu_HuyPhaTruyen, 1);
                    itemCancelMixedInfusion.Tag = MOUSE_RIGHT_TYPE.CANCEL_MIXED_INFUSION;
                    itemCancelMixedInfusion.ItemClick += new ItemClickEventHandler(setProcessMenu);
                    menu.AddItems(new BarButtonItem[] { itemCancelMixedInfusion });
                }


                if (menu.ItemLinks.Count > 0)
                    menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setProcessMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                var btn = e.Item as BarButtonItem;
                MOUSE_RIGHT_TYPE processType = (MOUSE_RIGHT_TYPE)btn.Tag;
                switch (processType)
                {
                    case MOUSE_RIGHT_TYPE.EDIT_DAY_NUM:
                        frmEditDayNum frm = new frmEditDayNum(ReloadDataEditDayNum);
                        frm.ShowDialog();
                        break;
                    case MOUSE_RIGHT_TYPE.EDIT_EXPEND_TYPE:
                        frmIsExpendType frmExpendType = new frmIsExpendType(ReloadDataEditExpendType);
                        frmExpendType.ShowDialog();
                        break;
                    case MOUSE_RIGHT_TYPE.MIXED_INFUSION:
                        List<MediMatyTypeADO> mediMatyTypes = this.GetMediMatySelected();
                        if (mediMatyTypes != null && mediMatyTypes.Count > 0)
                        {
                            var checkThuoc = mediMatyTypes.Where(o => o.DataType != HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC).ToList();

                            if (checkThuoc.Count > 0)
                            {
                                var Name = checkThuoc.Select(o => o.MEDICINE_TYPE_NAME).Distinct().ToList();
                                MessageBox.Show(String.Format(ResourceMessage.KhongThuocNhomThuoc, String.Join(",", Name)));
                            }
                            else 
                            {
                                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();

                                frmMixedInfusion frmMixed = new frmMixedInfusion(ReloadDateMixedInfusion, mediMatyTypeADO);
                                frmMixed.ShowDialog();
                            }
                        }
                        break;
                    case MOUSE_RIGHT_TYPE.CANCEL_MIXED_INFUSION:
                        CancelMixedInfusion();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CancelMixedInfusion()
        {
            try
            {
                List<MediMatyTypeADO> mediMatyTypes = this.GetMediMatySelected();

                List<long?> MixedInfusion = mediMatyTypes.Select(o=>o.MIXED_INFUSION).Distinct().ToList();
 
                foreach (var item in mediMatyTypes)
                {
                    item.MIXED_INFUSION = null;
                    item.IS_MIXED_MAIN = null;
                    item.TUTORIAL_INFUSION = "";
                }

                foreach (var mediMaty in mediMatyTypeADOs)
                {
                    if (MixedInfusion.Contains(mediMaty.MIXED_INFUSION))
                    {
                        mediMaty.MIXED_INFUSION = null;
                        mediMaty.IS_MIXED_MAIN = null;
                        mediMaty.TUTORIAL_INFUSION = "";
                    }
                }
                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ReloadDateMixedInfusion(object data) 
        {
            try
            {
                string TutorialInfusion = (string)data;
                List<MediMatyTypeADO> mediMatyTypes = this.GetMediMatySelected();

                long? MixedMax = mediMatyTypeADOs.Where(p => p.MIXED_INFUSION != null).Max(o => o.MIXED_INFUSION);
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                mediMatyTypeADO.IS_MIXED_MAIN = 1;
                mediMatyTypeADO.TUTORIAL_INFUSION = TutorialInfusion;

                foreach (var item in mediMatyTypes)
                {
                    item.MIXED_INFUSION = (MixedMax != null ? MixedMax : 0) + 1;
                }
                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadDataEditExpendType(object data)
        {
            try
            {
                if (data != null)
                {
                    bool expTypeId = (bool)data;
                    List<MediMatyTypeADO> mediMatyTypes = this.GetMediMatySelected();
                    foreach (var item in mediMatyTypes)
                    {
                        if (item.IsExpend && ((item.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0))
                        {
                            item.IsExpendType = expTypeId;
                            Inventec.Common.Logging.LogSystem.Debug("ReloadDataEditExpendType. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expTypeId), expTypeId));
                        }
                    }
                    this.gridControlServiceProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDataEditDayNum(object data)
        {
            try
            {
                if (data != null)
                {
                    decimal dayNum = (decimal)data;
                    List<MediMatyTypeADO> mediMatyTypes = this.GetMediMatySelected();
                    string serviceNameError = "";
                    foreach (var item in mediMatyTypes)
                    {
                        decimal oldAmount = item.AMOUNT ?? 0;
                        bool hasRoundUpAmount = false;
                        bool isChoPhepKeLe = (((item.IsAllowOdd.HasValue && item.IsAllowOdd.Value == true)) 
                            //&& (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            );
                        int phanthapphanle = (isChoPhepKeLe ? 6 : 0);

                        if (oldAmount != (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)oldAmount, phanthapphanle))
                        {
                            oldAmount = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)oldAmount, phanthapphanle);
                            hasRoundUpAmount = true;
                        }

                        decimal? amount = ((oldAmount / (item.UseDays.HasValue ? (item.UseDays.Value) : 1)) * dayNum);
                        if (amount != (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)amount, phanthapphanle))
                        {
                            amount = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)amount, phanthapphanle);
                            hasRoundUpAmount = true;
                        }

                        if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ReloadDataEditDayNum. 1");
                            if (!WarningOddConvertWorker.CheckWarningOddConvertAmount(item, amount))
                                continue;
                            Inventec.Common.Logging.LogSystem.Debug("ReloadDataEditDayNum. 2");
                            lstOutPatientPres = lstOutPatientPres.Where(o => o.PrimaryKey != item.PrimaryKey).ToList();
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                            if (!TakeOrReleaseBeanWorker.TakeForUpdateBean(this.intructionTimeSelecteds, this.oldExpMestId, item, amount.Value, true, new CommonParam(), this.UseTimeSelecteds, this.lstOutPatientPres))
                            {
                                serviceNameError += item.MEDICINE_TYPE_NAME + "; ";
                                continue;
                            }
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                            Inventec.Common.Logging.LogSystem.Debug("ReloadDataEditDayNum. 3");
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => amount), amount)
                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.oldExpMestId), this.oldExpMestId)
                                + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.oldExpMestId), this.oldExpMestId)
                                );
                        }
                        if (hasRoundUpAmount)
                        {
                            item.ErrorTypeAmountHasRound = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                            item.ErrorMessageAmountHasRound = ResourceMessage.ThuocVatTuDaBiLamTronSoLuongLenDoSoLuongCuBiKeLe;
                        }
                        else
                        {
                            item.ErrorTypeAmountHasRound = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                            item.ErrorMessageAmountHasRound = "";
                        }

                        item.AMOUNT = amount;
                        double checkLech = (double)(dayNum - (item.UseDays.HasValue ? (item.UseDays.Value) : 1));
                        item.UseDays = dayNum;
                        DateTime dtUseTimeTo = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.UseTimeTo ?? 0) ?? DateTime.Now);
                        item.UseTimeTo = item.UseTimeTo.HasValue ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTimeTo.AddDays((double)(checkLech))) : (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(licUseTime.Visible && UseTime > 0 ? UseTime : this.intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.AddDays((double)(dayNum - 1))));
                    }

                    gridControlServiceProcess.RefreshDataSource();
                    if (!String.IsNullOrEmpty(serviceNameError))
                    {
                        MessageBox.Show(String.Format(ResourceMessage.CapNhatThatBai_KhongDuSoLuongKhaDung, serviceNameError), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckEditExpendType()
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = GetMediMatySelected();
                result = result && !(mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0);
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    result = mediMatyTypeADOs != null ? mediMatyTypeADOs.Any(o => o.IsExpend && (o.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0) : false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckEditDayNum()
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = GetMediMatySelected();
                result = result && !(mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0);
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    MediMatyTypeADO mediMatyTypeADO = mediMatyTypeADOs.FirstOrDefault(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM);
                    if (mediMatyTypeADO != null)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<MediMatyTypeADO> GetMediMatySelected()
        {
            List<MediMatyTypeADO> result = new List<MediMatyTypeADO>();
            try
            {
                int[] selectRows = gridViewServiceProcess.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var mediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetRow(selectRows[i]);
                        result.Add(mediMatyTypeADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool checkMixedInfusion(bool check)
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = GetMediMatySelected();
                result = result && !(mediMatyTypeADOs == null || mediMatyTypeADOs.Count <= 1);

                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 1)
                {
                    var MediNumber = mediMatyTypeADOs.Where(o => o.MIXED_INFUSION != null).Count();

                    if (check && (MediNumber > 0))
                    {
                        result = false;
                    }

                    if (!check && (MediNumber <= 0))
                    {
                        result = false;
                    }

                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            return result;
        }

    }
}
