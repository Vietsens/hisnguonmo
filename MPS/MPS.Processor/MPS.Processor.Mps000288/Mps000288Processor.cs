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
using MPS.Processor.Mps000288.PDO;
using Inventec.Core;
using MPS.ProcessorBase;
using System.Text;
using System.Linq;

namespace MPS.Processor.Mps000288
{
    class Mps000288Processor : AbstractProcessor
    {
        Mps000288PDO rdo;
        List<RoleUserAdo> _RoleUserADOs = new List<RoleUserAdo>();
        public Mps000288Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000288PDO)rdoBase;
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
                            case "dtInTime":
                                _SetSingleKeyADO.In_Time = item.VALUE;
                                break;
                            case "dtOutTime":
                                _SetSingleKeyADO.Out_Time = item.VALUE;
                                break;
                            case "dtCheckTime":
                                _SetSingleKeyADO.Check_Time = item.VALUE;
                                break;
                            case "cboDepartment":
                                _SetSingleKeyADO.Department_Name = item.VALUE.Trim();
                                break;
                            case "txtDienBienBenh":
                                _SetSingleKeyADO.Dien_Bien_Benh = item.VALUE.Trim();
                                break;
                            case "txtKetLuan":
                                _SetSingleKeyADO.Ket_Luan = item.VALUE.Trim();
                                break;
                            case "LOGINNAME":
                                var base64EncodedBytes = System.Convert.FromBase64String(item.VALUE);
                                string jsonInput = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                                _RoleUserADOs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RoleUserAdo>>(jsonInput);
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
            public string In_Time { get; set; }
            public string Out_Time { get; set; }
            public string Check_Time { get; set; }
            public string Department_Name { get; set; }
            public string Dien_Bien_Benh { get; set; }
            public string Ket_Luan { get; set; }
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
