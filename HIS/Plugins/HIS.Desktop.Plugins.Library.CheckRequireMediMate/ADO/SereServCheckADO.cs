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
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.Library.CheckRequireMediMate.ADO
{
    /// <summary>
    /// Dich vu dang duoc ket thuc thuc hien, rut gon ve dung cac cot can kiem tra.
    /// Cac man goi thu vien co kieu du lieu khac nhau (HIS_SERE_SERV, V_HIS_SERE_SERV_5, V_HIS_SERE_SERV)
    /// nen quy ve 1 kieu chung o day.
    /// </summary>
    public class SereServCheckADO
    {
        /// <summary>HIS_SERE_SERV.ID cua dich vu (dong thuoc/vat tu di kem co PARENT_ID = gia tri nay)</summary>
        public long ID { get; set; }

        public long SERVICE_ID { get; set; }

        public long? SERVICE_REQ_ID { get; set; }

        public string TDL_SERVICE_CODE { get; set; }

        public string TDL_SERVICE_NAME { get; set; }

        public short? IS_NO_EXECUTE { get; set; }

        public short? IS_DELETE { get; set; }

        public static SereServCheckADO From(HIS_SERE_SERV data)
        {
            if (data == null) return null;
            SereServCheckADO ado = new SereServCheckADO();
            ado.ID = data.ID;
            ado.SERVICE_ID = data.SERVICE_ID;
            ado.SERVICE_REQ_ID = data.SERVICE_REQ_ID;
            ado.TDL_SERVICE_CODE = data.TDL_SERVICE_CODE;
            ado.TDL_SERVICE_NAME = data.TDL_SERVICE_NAME;
            ado.IS_NO_EXECUTE = data.IS_NO_EXECUTE;
            ado.IS_DELETE = data.IS_DELETE;
            return ado;
        }

        public static SereServCheckADO From(V_HIS_SERE_SERV data)
        {
            if (data == null) return null;
            SereServCheckADO ado = new SereServCheckADO();
            ado.ID = data.ID;
            ado.SERVICE_ID = data.SERVICE_ID;
            ado.SERVICE_REQ_ID = data.SERVICE_REQ_ID;
            ado.TDL_SERVICE_CODE = data.TDL_SERVICE_CODE;
            ado.TDL_SERVICE_NAME = data.TDL_SERVICE_NAME;
            ado.IS_NO_EXECUTE = data.IS_NO_EXECUTE;
            ado.IS_DELETE = data.IS_DELETE;
            return ado;
        }

        public static SereServCheckADO From(V_HIS_SERE_SERV_5 data)
        {
            if (data == null) return null;
            SereServCheckADO ado = new SereServCheckADO();
            ado.ID = data.ID;
            ado.SERVICE_ID = data.SERVICE_ID;
            ado.SERVICE_REQ_ID = data.SERVICE_REQ_ID;
            ado.TDL_SERVICE_CODE = data.TDL_SERVICE_CODE;
            ado.TDL_SERVICE_NAME = data.TDL_SERVICE_NAME;
            ado.IS_NO_EXECUTE = data.IS_NO_EXECUTE;
            ado.IS_DELETE = data.IS_DELETE;
            return ado;
        }
    }
}
