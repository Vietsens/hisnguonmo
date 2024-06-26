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
using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MPS.ProcessorBase.Core;
using MPS.Processor.Mps000291.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000291
{
    class Mps000291Processor : AbstractProcessor
    {
        Mps000291PDO rdo;
        List<RoleUserAdo> _RoleUserADOs = new List<RoleUserAdo>();
        public Mps000291Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000291PDO)rdoBase;
        }

        public void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                SetBarcodeKey();
                SetSingleKey();
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                objectTag.AddObjectData(store, "Datas", this._RoleUserADOs);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                _RoleUserADOs = new List<RoleUserAdo>();
                SetSingleKeyADO _SetSingleKeyADO = new SetSingleKeyADO();
                if (rdo._SarFormDatas != null && rdo._SarFormDatas.Count > 0)
                {
                    foreach (var item in rdo._SarFormDatas)
                    {
                        switch (item.KEY)
                        {
                            case "lblPatientName":
                                _SetSingleKeyADO.Patient_Name = item.VALUE;
                                break;
                            case "lblAge":
                                _SetSingleKeyADO.Age = item.VALUE;
                                break;
                            case "lblGenderName":
                                _SetSingleKeyADO.Gender_Name = item.VALUE;
                                break;
                            case "lblInCode":
                                _SetSingleKeyADO.In_Code = item.VALUE;
                                break;
                            case "lblAddress":
                                _SetSingleKeyADO.ADDRESS = item.VALUE;
                                break;
                            case "cboDepartment":
                                _SetSingleKeyADO.RESULT_DEPARTMENT_NAME = item.VALUE.Trim();
                                break;
                            case "cboBedRoom":
                                _SetSingleKeyADO.RESULT_BED_ROOM_NAME = item.VALUE.Trim();
                                break;
                            case "cboBed":
                                _SetSingleKeyADO.RESULT_BED_NAME = item.VALUE.Trim();
                                break;
                            case "lcIcd":
                                _SetSingleKeyADO.RESULT_ICD_NAME = item.VALUE.Trim();
                                break;
                            case "txtDienBienBenh":
                                _SetSingleKeyADO.DIEN_BIEN = item.VALUE.Trim();
                                break;
                            case "txtXetnghiem":
                                _SetSingleKeyADO.XET_NGHIEM = item.VALUE.Trim();
                                break;
                            case "txtQuatrinh":
                                _SetSingleKeyADO.QUA_TRINH = item.VALUE.Trim();
                                break;
                            case "txtKetQua":
                                _SetSingleKeyADO.KET_QUA = item.VALUE.Trim();
                                break;
                            case "txtHuongDieuTri":
                                _SetSingleKeyADO.HUONG_DIEU_TRI = item.VALUE.Trim();
                                break;
                            default:
                                break;
                        }
                    }
                }
                AddObjectKeyIntoListkey<SetSingleKeyADO>(_SetSingleKeyADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public class SetSingleKeyADO
        {
            public string Patient_Name { get; set; }
            public string Age { get; set; }
            public string Gender_Name { get; set; }
            public string In_Code { get; set; }
            public string ADDRESS { get; set; }
            public string RESULT_DEPARTMENT_NAME { get; set; }
            public string RESULT_BED_ROOM_NAME { get; set; }
            public string RESULT_BED_NAME { get; set; }
            public string RESULT_ICD_NAME { get; set; }
            public string DIEN_BIEN { get; set; }
            public string XET_NGHIEM { get; set; }
            public string QUA_TRINH { get; set; }
            public string KET_QUA { get; set; }
            public string HUONG_DIEU_TRI { get; set; }
        }

        public class RoleUserAdo
        {
            public int Action { get; set; }

            public long EXECUTE_ROLE_ID { get; set; }
            public string EXECUTE_ROLE_NAME { get; set; }
            public string LOGINNAME { get; set; }
            public string USERNAME { get; set; }
        }

    }
}
