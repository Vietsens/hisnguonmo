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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Patient
{
    delegate void PatientMouseRight_Click(object sender, ItemClickEventArgs e);

    class PatientPopupMenuProcessor
    {
        V_HIS_PATIENT currentPatient;
        PatientMouseRight_Click patientMouseRightClick;
        BarManager barManager;
        PopupMenu menu;
        internal enum ModuleType
        {
            ScnPersonalHealth,
            ScnAccidentHurt,
            PatientUpdateExt,
            PatientProgram,
            EvenLog,
            ScnNutrition,
            ScnDeath,
            PatientInfo,
            FamilyInformation,
            BenhNhanTamThan,
            KhamThai,
            KeHoachHoa,
            PhaThai,
            BenhNhanHIV,
            TYTTuVong,
            TYTSinhDe,
            BenhNhanLao, 
            TheDiUng,
            TYTSotRet
        }
        //internal ModuleType moduleType { get; set; }

        internal PatientPopupMenuProcessor(MOS.EFMODEL.DataModels.V_HIS_PATIENT _currentPatient, PatientMouseRight_Click _patientMouseRightClick, BarManager _barManager)
        {
            this.currentPatient = _currentPatient;
            this.patientMouseRightClick = _patientMouseRightClick;
            this.barManager = _barManager;
        }

        internal void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);
                // Add item and show
                menu.ItemLinks.Clear();

                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(HIS.Desktop.Plugins.Patient.UCListPatient).Assembly);

                if (this.currentPatient != null && !string.IsNullOrEmpty(this.currentPatient.PERSON_CODE))
                {
                    #region HSSK
                    BarSubItem subSucKhoeCaNhan = new BarSubItem(this.barManager, "Hồ sơ sức khỏe cá nhân", 1);

                    BarButtonItem itemCapNhat = new BarButtonItem(barManager, "Cập nhật bố mẹ đẻ", 1);
                    itemCapNhat.Tag = ModuleType.FamilyInformation;
                    itemCapNhat.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subSucKhoeCaNhan.AddItem(itemCapNhat);

                    BarButtonItem itemTienSuSucKhoe = new BarButtonItem(barManager, "Thông tin tiền sử sức khỏe", 1);
                    itemTienSuSucKhoe.Tag = ModuleType.ScnPersonalHealth;
                    itemTienSuSucKhoe.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subSucKhoeCaNhan.AddItem(itemTienSuSucKhoe);

                    BarButtonItem itemTaiNanThuongTich = new BarButtonItem(barManager, "Thông tin tai nạn thương tích", 1);
                    itemTaiNanThuongTich.Tag = ModuleType.ScnAccidentHurt;
                    itemTaiNanThuongTich.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subSucKhoeCaNhan.AddItem(itemTaiNanThuongTich);

                    BarButtonItem itemDinhDuong = new BarButtonItem(barManager, "Thông tin dinh dưỡng", 1);
                    itemDinhDuong.Tag = ModuleType.ScnNutrition;
                    itemDinhDuong.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subSucKhoeCaNhan.AddItem(itemDinhDuong);

                    BarButtonItem itemTuVong = new BarButtonItem(barManager, "Thông tin tử vong", 1);
                    itemTuVong.Tag = ModuleType.ScnDeath;
                    itemTuVong.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subSucKhoeCaNhan.AddItem(itemTuVong);

                    menu.AddItems(new BarItem[] { subSucKhoeCaNhan });
                    #endregion
                }

                #region Trạm y tế
                BarSubItem subTramYTe = new BarSubItem(this.barManager, "Trạm y tế", 8);

                BarButtonItem itemTamThan = new BarButtonItem(barManager, "Tâm thần", 8);
                itemTamThan.Tag = ModuleType.BenhNhanTamThan;
                itemTamThan.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                subTramYTe.AddItem(itemTamThan);

                BarButtonItem itemHIV = new BarButtonItem(barManager, "HIV", 8);
                itemHIV.Tag = ModuleType.BenhNhanHIV;
                itemHIV.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                subTramYTe.AddItem(itemHIV);

                BarButtonItem itemTytTuVong = new BarButtonItem(barManager, "Tử vong", 8);
                itemTytTuVong.Tag = ModuleType.TYTTuVong;
                itemTytTuVong.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                subTramYTe.AddItem(itemTytTuVong);

                BarButtonItem itemTytLao = new BarButtonItem(barManager, "Lao", 8);
                itemTytLao.Tag = ModuleType.BenhNhanLao;
                itemTytLao.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                subTramYTe.AddItem(itemTytLao);

                BarButtonItem itemTytSotRet = new BarButtonItem(barManager, "Sốt rét", 8);
                itemTytSotRet.Tag = ModuleType.TYTSotRet;
                itemTytSotRet.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                subTramYTe.AddItem(itemTytSotRet);

                if (this.currentPatient != null && this.currentPatient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                {
                    BarButtonItem itemKhamThai = new BarButtonItem(barManager, "Khám thai", 8);
                    itemKhamThai.Tag = ModuleType.KhamThai;
                    itemKhamThai.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subTramYTe.AddItem(itemKhamThai);

                    BarButtonItem itemPhaThai = new BarButtonItem(barManager, "Phá thai", 8);
                    itemPhaThai.Tag = ModuleType.PhaThai;
                    itemPhaThai.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subTramYTe.AddItem(itemPhaThai);

                    BarButtonItem itemKeHoachHoa = new BarButtonItem(barManager, "Kế hoạch hóa gia đình", 8);
                    itemKeHoachHoa.Tag = ModuleType.KeHoachHoa;
                    itemKeHoachHoa.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subTramYTe.AddItem(itemKeHoachHoa);

                    BarButtonItem itemSinhDe = new BarButtonItem(barManager, "Sinh đẻ", 8);
                    itemSinhDe.Tag = ModuleType.TYTSinhDe;
                    itemSinhDe.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                    subTramYTe.AddItem(itemSinhDe);
                }
                menu.AddItems(new BarItem[] { subTramYTe });

                #endregion

                BarButtonItem itemPatientProgram = new BarButtonItem(barManager, "Bệnh nhân chương trình", 6);
                itemPatientProgram.Tag = ModuleType.PatientProgram;
                itemPatientProgram.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                menu.AddItems(new BarItem[] { itemPatientProgram });

                BarButtonItem itemEvenLog = new BarButtonItem(barManager, "Lịch sử tác động", 5);
                itemEvenLog.Tag = ModuleType.EvenLog;
                itemEvenLog.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                menu.AddItems(new BarItem[] { itemEvenLog });

                BarButtonItem itemPatientInfo = new BarButtonItem(barManager, "Thông tin bệnh nhân", 7);
                itemPatientInfo.Tag = ModuleType.PatientInfo;
                itemPatientInfo.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                menu.AddItems(new BarItem[] { itemPatientInfo });

                BarButtonItem itemTheDiUng = new BarButtonItem(barManager, "Thẻ dị ứng", 6);
                itemTheDiUng.Tag = ModuleType.TheDiUng;
                itemTheDiUng.ItemClick += new ItemClickEventHandler(this.patientMouseRightClick);
                menu.AddItems(new BarItem[] { itemTheDiUng });

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
