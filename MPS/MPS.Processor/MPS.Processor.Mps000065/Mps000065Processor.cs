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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000065.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000065
{
    public class Mps000065Processor : AbstractProcessor
    {
        Mps000065PDO rdo;

        public Mps000065Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000065PDO)rdoBase;
        }

        internal void SetBarcodeKey()
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
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                SetBarcodeKey();
                SetSingleKey();


                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Users", rdo.lstHisDebateUserTGia);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER> listPresidentSecretary = rdo.lstHisDebateUser.Where(o => o.IS_PRESIDENT == 1 || o.IS_SECRETARY == 1).ToList();
                if (listPresidentSecretary != null)
                {
                    foreach (var item_User in listPresidentSecretary)
                    {
                        if (item_User.IS_PRESIDENT == 1)
                        {
                            SetSingleKey(new KeyValue(Mps000065ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                        }
                        if (item_User.IS_SECRETARY == 1)
                        {
                            SetSingleKey(new KeyValue(Mps000065ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                        }
                    }
                }
                if (rdo.HisDebateRow != null)
                {
                    SetSingleKey(new KeyValue(Mps000065ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.HisDebateRow.DEBATE_TIME ?? 0)));
                }
                else
                {
                    SetSingleKey(new KeyValue(Mps000065ExtendSingleKey.DEBATE_TIME_STR, ""));
                }
                if (rdo.Patient != null)
                {
                    SetSingleKey(new KeyValue(Mps000065ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB)));
                }

                AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                SetSingleKey(new KeyValue(Mps000065ExtendSingleKey.BED_ROOM_NAME, rdo.bedRoomName));
                AddObjectKeyIntoListkey<HIS_DEBATE>(rdo.HisDebateRow, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
