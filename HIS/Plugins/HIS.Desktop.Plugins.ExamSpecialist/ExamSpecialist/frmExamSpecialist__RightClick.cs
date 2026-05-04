/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.Library.FormMedicalRecord.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamSpecialist.ExamSpecialist
{
    public partial class frmExamSpecialist
    {
        #region Right-click Vỏ bệnh án

        private BarManager popupBarManager;
        private PopupMenu popupVoBenhAn;
        private MediRecordMenuPopupProcessor emrMenuPopupProcessor;
        private ImageCollection popupImageCollection;
        private RepositoryItemButtonEdit repositoryItemButtonEditMedicalRecord;
        private const string EMR_DOCUMENT_MODULE_LINK = "HIS.Desktop.Plugins.EmrDocument";

        /// <summary>
        /// Icon "con mắt" 16x16 (base64 PNG) — copy nguyên từ
        /// HIS.Desktop.Plugins.TreatmentList\UCTreatmentList.resx
        /// (key "repositoryItembtnViewMedicalRecord.Buttons") để icon hiển thị
        /// trên cột "Chi tiết bệnh án" giống y hệt màn Hồ sơ điều trị.
        /// </summary>
        private const string EYE_ICON_BASE64 =
            "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAA90RVh0VGl0" +
            "bGUAU2hvdztFeWU7SeMHJQAAArJJREFUOE+lk2lIlGEQx8fMPlhRkkenZFheiAdIpKvmyXrseqyru6uY" +
            "R25aiQcumVArnsV6rRaaqUmGd7JemCCikURGafilIO+jxAtd7cL4x/uiBNoX6YH/w8Mw85thZh4CQP+j" +
            "XYa96l9nHxFpEtF+ItLaEvNm7Bo7nemJxIYqRNZs4E2HszqFPLOEYj9TlZJnOlzCM9ks8D4/rOAad2S5" +
            "GcnCrAwMGHi+pxHleRj9BbBGvrm4KtRuqV+ZgM/djzH/ToXNr28x97oOI425aL8rQYmf+Woq54yIiA4w" +
            "1dj7KFiGZlGARWyXXILpgVo0Fd2GmOcOrqsTzptbgOvCQTDXCVV3ovCmMhXVkZzftxxORzMQFpDsYmxU" +
            "HWn/q1weh4jIKHDchXhQ34b+iRnYuYggTclCbtkzOHNFCAu7gpwYPpQBVoi01uewvVEKLO8lR4dBpWrD" +
            "0vIqXAOT0f1hFOHx6TCz9YKprRca+obgIUzB5PQCamqbIBX6Q+5s2EhE2hQvuT7Z3NoDtfob1Os/4B+e" +
            "hocNvYhKysY5c2cIIlJQWPMCImk6pmaXMT61iIqnrZB4SmaI6DAJgxJvpOd3Yn3jJ9bU3/G8vR/CmAxk" +
            "l7eitPklMktVCIiSo0HVh/HpRRZwLaUGly6KZGwFVqeO6HqLFRVlNQP4Mr+GpZUN1Lf0wDckCZb2YvBD" +
            "Zahr6cXY1AJGPs4iu6gLtpdlDXoHtU5u7Qt76bgLMnKCpZUbBY960TvwCYNDE5ieW8GrwVF09oxAruiA" +
            "W1DhhrVTYjERHd9aMA0Kjq3ehhw6YWhzwdEnLdNNoBh0FeSNuQcr4cS/P+bgnfHe2jE+V0ffxIKIjjLd" +
            "Z0bIjjEkrpqCpFUUeLWCATErq73ldIyI9IhIl6mQSbCd1d43jw1mATs/x161y7BX/QE10AHqvhLjjAAA" +
            "AABJRU5ErkJggg==";

        /// <summary>
        /// 12 DataBar icons (DevExpress Image Gallery) — match indices used by
        /// MediRecordMenuPopupProcessor / TreatmentList PopupMenuProcessor.
        /// </summary>
        private static readonly string[] PopupMenuIconPaths = new[]
        {
            "images/data%20bars/gradientbluedatabar_16x16.png",
            "images/data%20bars/gradientgreendatabar_16x16.png",
            "images/data%20bars/gradientlightbluedatabar_16x16.png",
            "images/data%20bars/gradientorangedatabar_16x16.png",
            "images/data%20bars/gradientpurpledatabar_16x16.png",
            "images/data%20bars/gradientreddatabar_16x16.png",
            "images/data%20bars/solidbluedatabar_16x16.png",
            "images/data%20bars/solidgreendatabar_16x16.png",
            "images/data%20bars/solidlightbluedatabar_16x16.png",
            "images/data%20bars/solidorangedatabar_16x16.png",
            "images/data%20bars/solidpurpledatabar_16x16.png",
            "images/data%20bars/solidreddatabar_16x16.png"
        };

        /// <summary>
        /// Đăng ký sự kiện right-click trên grid để hiển thị menu Vỏ bệnh án
        /// và cấu hình cột "Chi tiết bệnh án" (button + click → mở EmrDocument).
        /// Gọi 1 lần trong Form_Load.
        /// </summary>
        private void InitRightClickMenu()
        {
            try
            {
                this.gridView1.PopupMenuShowing -= gridView1_PopupMenuShowing;
                this.gridView1.PopupMenuShowing += gridView1_PopupMenuShowing;

                InitMedicalRecordColumn();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cấu hình cột "Chi tiết bệnh án" (gridColumn_MedicalRecorDetails):
        ///   - Gán RepositoryItemButtonEdit (icon button) làm ColumnEdit.
        ///   - Đăng ký Click → mở plugin HIS.Desktop.Plugins.EmrDocument.
        ///   - Sắp xếp VisibleIndex để cột nằm TRƯỚC cột Trạng thái.
        /// </summary>
        private void InitMedicalRecordColumn()
        {
            try
            {
                if (repositoryItemButtonEditMedicalRecord == null)
                {
                    repositoryItemButtonEditMedicalRecord = new RepositoryItemButtonEdit();
                    repositoryItemButtonEditMedicalRecord.AutoHeight = false;
                    repositoryItemButtonEditMedicalRecord.TextEditStyle = TextEditStyles.HideTextEditor;
                    repositoryItemButtonEditMedicalRecord.Name = "repositoryItemButtonEditMedicalRecord";
                    repositoryItemButtonEditMedicalRecord.Buttons.Clear();

                    System.Drawing.Image icon = LoadEyeIconFromBase64();

                    repositoryItemButtonEditMedicalRecord.Buttons.Add(
                        new EditorButton(
                            ButtonPredefines.Glyph,
                            "",
                            -1,
                            true,
                            true,
                            false,
                            DevExpress.XtraEditors.ImageLocation.MiddleCenter,
                            icon,
                            new KeyShortcut(Keys.None),
                            new SerializableAppearanceObject(),
                            new SerializableAppearanceObject(),
                            new SerializableAppearanceObject(),
                            new SerializableAppearanceObject(),
                            "Xem chi tiết bệnh án",
                            null,
                            null,
                            true));

                    repositoryItemButtonEditMedicalRecord.ButtonClick += repositoryItemButtonEditMedicalRecord_ButtonClick;

                    if (!gridControlExamSpecialist.RepositoryItems.Contains(repositoryItemButtonEditMedicalRecord))
                    {
                        gridControlExamSpecialist.RepositoryItems.Add(repositoryItemButtonEditMedicalRecord);
                    }
                }

                gridColumn_MedicalRecorDetails.ColumnEdit = repositoryItemButtonEditMedicalRecord;
                gridColumn_MedicalRecorDetails.OptionsColumn.AllowEdit = true;
                gridColumn_MedicalRecorDetails.ToolTip = "Chi tiết bệnh án";
                gridColumn_MedicalRecorDetails.Caption = "Chi tiết bệnh án";
                gridColumn_MedicalRecorDetails.OptionsColumn.ShowCaption = false;
                // VisibleIndex set trực tiếp trong Designer (8 — TRƯỚC Status=9)
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditMedicalRecord_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = gridView1.GetFocusedRow() as V_HIS_SPECIALIST_EXAM;
                if (row == null) return;
                OpenEmrDocument(row.TREATMENT_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mở plugin HIS.Desktop.Plugins.EmrDocument với TREATMENT_CODE — pattern giống
        /// TreatmentList (UCTreatmentList.gridViewtreatmentList_MouseDown / ChiTietBenhAnClick).
        /// </summary>
        private void OpenEmrDocument(string treatmentCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(treatmentCode) || this.currentModule == null) return;

                WaitingManager.Show();
                List<object> listObj = new List<object>();
                listObj.Add(treatmentCode);
                PluginInstanceBehavior.ShowModule(
                    EMR_DOCUMENT_MODULE_LINK,
                    this.currentModule.RoomId,
                    this.currentModule.RoomTypeId,
                    listObj);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thêm menu item "Chi tiết bệnh án" vào popup — giống pattern TreatmentList
        /// (PopupMenuProcessor.cs: itemPatientDetail). KHÔNG hiển thị nếu user
        /// không có quyền truy cập module HIS.Desktop.Plugins.EmrDocument (PTTK yêu cầu).
        /// </summary>
        private void AddChiTietBenhAnMenuItem(string treatmentCode)
        {
            try
            {
                var moduleData = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == EMR_DOCUMENT_MODULE_LINK);
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    return;
                }

                BarButtonItem item = new BarButtonItem(popupBarManager, "Chi tiết bệnh án", 9);
                item.Tag = treatmentCode;
                item.ItemClick += chiTietBenhAnMenuItem_Click;
                popupVoBenhAn.AddItem(item).BeginGroup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chiTietBenhAnMenuItem_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item == null || e.Item.Tag == null) return;
                OpenEmrDocument(e.Item.Tag.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.MenuType != GridMenuType.Row) return;

                GridHitInfo hi = e.HitInfo;
                if (!hi.InRowCell) return;

                var row = gridView1.GetRow(hi.RowHandle) as V_HIS_SPECIALIST_EXAM;
                if (row == null || (row.TREATMENT_ID ?? 0) == 0) return;

                ShowPopupMenu(row.TREATMENT_ID.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowPopupMenu(long treatmentId)
        {
            HIS_TREATMENT treatment = null;
            try
            {
                WaitingManager.Show();
                treatment = LoadTreatment(treatmentId);
                WaitingManager.Hide();

                if (treatment == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ExamSpecialist__RightClick.ShowPopupMenu: Khong load duoc HIS_TREATMENT, treatmentId="
                        + treatmentId);
                    return;
                }

                if (popupBarManager == null)
                {
                    popupBarManager = new BarManager();
                    popupBarManager.Form = this;
                    popupBarManager.Images = BuildPopupImageCollection();
                }
                if (emrMenuPopupProcessor == null)
                {
                    emrMenuPopupProcessor = new MediRecordMenuPopupProcessor();
                }
                if (popupVoBenhAn == null)
                {
                    popupVoBenhAn = new PopupMenu(popupBarManager);
                }
                popupVoBenhAn.ItemLinks.Clear();

                EmrInputADO emrInputAdo = BuildEmrInputAdo(treatment);
                emrMenuPopupProcessor.InitMenu(popupVoBenhAn, popupBarManager, emrInputAdo);

                AddChiTietBenhAnMenuItem(treatment.TREATMENT_CODE);

                popupVoBenhAn.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT LoadTreatment(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                var lst = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get",
                    ApiConsumers.MosConsumer,
                    filter,
                    param);
                return (lst != null) ? lst.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private EmrInputADO BuildEmrInputAdo(HIS_TREATMENT treatment)
        {
            EmrInputADO ado = new EmrInputADO();
            try
            {
                long roomId = (this.currentModule != null) ? this.currentModule.RoomId : 0;

                ado.TreatmentId = treatment.ID;
                ado.PatientId = treatment.PATIENT_ID;
                ado.Treatment = treatment;
                ado.TreatmentTypeId = treatment.TDL_TREATMENT_TYPE_ID;
                ado.roomId = roomId;

                var workplace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == roomId);
                if (workplace != null)
                {
                    ado.DepartmentId = workplace.DepartmentId;
                }

                if (treatment.EMR_COVER_TYPE_ID.HasValue)
                {
                    ado.EmrCoverTypeId = treatment.EMR_COVER_TYPE_ID.Value;
                    return ado;
                }

                var configByRoom = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.ROOM_ID == roomId
                        && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID)
                    .ToList();
                if (configByRoom != null && configByRoom.Count > 0)
                {
                    AssignCoverTypeIds(ado, configByRoom);
                    return ado;
                }

                if (ado.DepartmentId.HasValue)
                {
                    var configByDept = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.DEPARTMENT_ID == ado.DepartmentId.Value
                            && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID)
                        .ToList();
                    if (configByDept != null && configByDept.Count > 0)
                    {
                        AssignCoverTypeIds(ado, configByDept);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ado;
        }

        private void AssignCoverTypeIds(EmrInputADO ado, List<HIS_EMR_COVER_CONFIG> configs)
        {
            if (configs.Count == 1)
            {
                ado.EmrCoverTypeId = configs.First().EMR_COVER_TYPE_ID;
            }
            else
            {
                ado.lstEmrCoverTypeId = configs.Select(o => o.EMR_COVER_TYPE_ID).ToList();
            }
        }

        /// <summary>
        /// Load icon "con mắt" 16x16 từ base64 PNG (copy từ TreatmentList.resx).
        /// Trả về Bitmap clone (deep copy) để có thể dispose stream an toàn —
        /// Image.FromStream yêu cầu stream còn mở suốt vòng đời Image.
        /// </summary>
        private System.Drawing.Image LoadEyeIconFromBase64()
        {
            try
            {
                byte[] data = Convert.FromBase64String(EYE_ICON_BASE64);
                using (var ms = new System.IO.MemoryStream(data))
                using (var img = System.Drawing.Image.FromStream(ms))
                {
                    return new System.Drawing.Bitmap(img);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Build ImageCollection chứa 12 DataBar icons để gán cho BarManager.
        /// Cần thiết để các BarButtonItem trong MediRecordMenuPopupProcessor hiển thị icon
        /// (mỗi BarButtonItem khai báo imageIndex 0..11 trong constructor).
        /// </summary>
        private ImageCollection BuildPopupImageCollection()
        {
            try
            {
                if (popupImageCollection != null) return popupImageCollection;

                popupImageCollection = new ImageCollection();
                popupImageCollection.ImageSize = new System.Drawing.Size(16, 16);
                for (int i = 0; i < PopupMenuIconPaths.Length; i++)
                {
                    var img = DevExpress.Images.ImageResourceCache.Default.GetImage(PopupMenuIconPaths[i]);
                    if (img != null)
                    {
                        popupImageCollection.AddImage(img);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return popupImageCollection;
        }

        #endregion
    }
}
