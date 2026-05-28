/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace HIS.UC.PatientPackagePicker.ADO
{
    /// <summary>
    /// Wrapper de bind 1 dong V_HIS_PATIENT_PACKAGE_DT vao grid chi tiet —
    /// bo sung 2 cot UI khong co tren view: IS_CHECKED (checkbox dau dong)
    /// va AMOUNT_THIS_TIME (so luong su dung lan nay, mac dinh 1).
    /// Cac cot con lai forward thang sang Detail de FieldName cua grid
    /// tro vao luon ma khong can duplicate du lieu.
    /// </summary>
    public class PackageDetailRowADO
    {
        public V_HIS_PATIENT_PACKAGE_DT Detail { get; set; }

        public bool IS_CHECKED { get; set; }
        public decimal AMOUNT_THIS_TIME { get; set; }

        public long? ID
        {
            get { return Detail != null ? Detail.ID : (long?)null; }
        }

        public long? SERVICE_ID
        {
            get { return Detail != null ? Detail.SERVICE_ID : (long?)null; }
        }

        public string SERVICE_CODE
        {
            get { return Detail != null ? Detail.SERVICE_CODE : null; }
        }

        public string SERVICE_NAME
        {
            get { return Detail != null ? Detail.SERVICE_NAME : null; }
        }

        public string SERVICE_TYPE_NAME
        {
            get { return Detail != null ? Detail.SERVICE_TYPE_NAME : null; }
        }

        public string SERVICE_TYPE_CODE
        {
            get { return Detail != null ? Detail.SERVICE_TYPE_CODE : null; }
        }

        public decimal? AMOUNT
        {
            get { return Detail != null ? Detail.AMOUNT : (decimal?)null; }
        }

        public decimal? AMOUNT_USED
        {
            get { return Detail != null ? Detail.AMOUNT_USED : (decimal?)null; }
        }

        public decimal? UNIT_PRICE
        {
            get { return Detail != null ? Detail.UNIT_PRICE : (decimal?)null; }
        }
    }
}
