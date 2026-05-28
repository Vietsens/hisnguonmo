/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HIS.UC.PatientPackagePicker.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.PatientPackagePicker
{
    /// <summary>
    /// API public cho cac plugin goi vao de mo popup "Chon dich vu trong goi"
    /// cua mot benh nhan.
    ///
    /// Giao dien giua cac caller giong het nhau; chi KHAC nhau o du lieu can lay:
    ///   - AssignService (Yeu cau 1): bo thuoc/VT/mau/suat an
    ///   - AssignPrescriptionPK (Yeu cau 2): chi thuoc/VT
    ///   - TransactionBillOther / Bordereau: lay tat ca
    ///
    /// Tach lam 2 dau vao:
    ///   1) loadDetailFunc — load TOAN BO chi tiet cua mot goi (dung chung,
    ///      moi caller chi viec goi 1 API HisPatientPackageDt/Get).
    ///   2) detailFilter — predicate loc theo loai dich vu, TUNG caller cung cap.
    ///      Truyen null khi muon lay tat ca.
    /// </summary>
    public static class PatientPackagePickerProcessor
    {
        /// <summary>
        /// Mo popup o che do modal. Tra ve danh sach dich vu da chon kem so luong
        /// su dung lan nay; tra ve null neu user huy bo.
        /// </summary>
        /// <param name="activePackages">
        /// Danh sach goi (V_HIS_PATIENT_PACKAGE) cua benh nhan, da loc IS_ACTIVE=1.
        /// Caller load truoc qua API HisPatientPackage hoac VHisPatientPackage.
        /// </param>
        /// <param name="loadDetailFunc">
        /// Delegate load TOAN BO chi tiet cua mot goi theo PATIENT_PACKAGE_ID.
        /// Khong loc trong loader — de viec loc cho detailFilter.
        /// </param>
        /// <param name="detailFilter">
        /// Predicate loc theo loai dich vu (vi du theo SERVICE_TYPE_CODE).
        /// Truyen null neu khong can loc.
        /// </param>
        public static List<SelectedPatientPackageServiceADO> Pick(
            List<V_HIS_PATIENT_PACKAGE> activePackages,
            frmPatientPackagePicker.LoadDetailDelegate loadDetailFunc,
            frmPatientPackagePicker.DetailFilterDelegate detailFilter = null)
        {
            List<SelectedPatientPackageServiceADO> result = null;
            try
            {
                using (var frm = new frmPatientPackagePicker(
                    activePackages, loadDetailFunc, detailFilter))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        result = frm.SelectedItems;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
