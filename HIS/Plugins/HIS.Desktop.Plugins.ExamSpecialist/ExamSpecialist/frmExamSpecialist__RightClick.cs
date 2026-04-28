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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
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
        /// Đăng ký sự kiện right-click trên grid để hiển thị menu Vỏ bệnh án.
        /// Gọi 1 lần trong Form_Load.
        /// </summary>
        private void InitRightClickMenu()
        {
            try
            {
                this.gridView1.PopupMenuShowing -= gridView1_PopupMenuShowing;
                this.gridView1.PopupMenuShowing += gridView1_PopupMenuShowing;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
