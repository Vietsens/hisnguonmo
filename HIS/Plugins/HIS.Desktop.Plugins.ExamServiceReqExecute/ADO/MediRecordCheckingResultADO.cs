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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.ADO
{
    /// <summary>
    /// Ket qua tra ve cua api "api/EmrDocument/MediRecordChecking" (EMR.SDO.MediRecordCheckingResultSDO).
    ///
    /// Khai bao lai o day thay vi tham chieu truc tiep EMR.SDO vi kieu phan tu cua
    /// SignatureMissingDocuments trong EMR.SDO la EMR.EFMODEL.DataModels.V_EMR_DOCUMENT,
    /// ma ban EMR.EFMODEL.dll hien tai trong lib\EMR khong con chua cac kieu view V_*
    /// (xem ghi chu trong ExamServiceReqExecuteControl__CheckDocumentHospitalize.cs).
    /// Ban tin di tren day la JSON nen chi can trung ten thuoc tinh la Newtonsoft map duoc.
    /// </summary>
    public class MediRecordCheckingResultADO
    {
        /// <summary>Cac van ban chua hoan thanh chu ky.</summary>
        public List<MediRecordCheckingDocumentADO> SignatureMissingDocuments { get; set; }

        /// <summary>Ten cac van ban bat buoc nhung chua duoc tao (chi co ten, khong co thong tin loai van ban).</summary>
        public List<string> MandatoryMissingDocuments { get; set; }
    }

    /// <summary>
    /// Mot van ban chua hoan thanh trong ket qua kiem tra ho so benh an.
    /// Chi khai bao cac truong duoc su dung; cac truong con lai cua V_EMR_DOCUMENT bi bo qua khi deserialize.
    /// </summary>
    public class MediRecordCheckingDocumentADO
    {
        public long ID { get; set; }

        public string DOCUMENT_CODE { get; set; }

        public string DOCUMENT_NAME { get; set; }

        /// <summary>Khoa ngoai toi EMR_DOCUMENT_TYPE, dung de tra cuu IS_HOSPITALIZATION.</summary>
        public long? DOCUMENT_TYPE_ID { get; set; }

        public string DOCUMENT_TYPE_CODE { get; set; }

        public string DOCUMENT_TYPE_NAME { get; set; }

        /// <summary>Danh sach tai khoan da ky.</summary>
        public string SIGNERS { get; set; }

        /// <summary>Danh sach tai khoan chua ky.</summary>
        public string UN_SIGNERS { get; set; }
    }
}
