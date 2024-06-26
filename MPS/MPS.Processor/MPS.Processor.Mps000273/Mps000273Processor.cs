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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000273.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000273
{
    public class Mps000273Processor : AbstractProcessor
    {
        Mps000273PDO rdo;
        List<SereServADO> Output;
        List<SereServADO> OutputRationTimeGroup;
        List<HIS_RATION_TIME> Ration;
        public Mps000273Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000273PDO)rdoBase;
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

                ProcessSereServ();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "SereServs", Output);
                objectTag.AddObjectData(store, "SereServsTimeGr", OutputRationTimeGroup);
                objectTag.AddObjectData(store, "RationTime", Ration.OrderBy(o=>o.ID).ToList());
                objectTag.AddRelationship(store, "RationTime", "SereServsTimeGr", "ID", "RATION_TIME_ID");
                barCodeTag.ProcessData(store, dicImage);

                store.SetCommonFunctions();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessSereServ()
        {
            try
            {
                Output = new List<SereServADO>();
                OutputRationTimeGroup = new List<SereServADO>();
                Ration = new List<HIS_RATION_TIME>();
                //lay danh sach Ration time
                var RationName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_RATION_TIME>().ToList();
                var rationGroup = rdo.ListSereServ.GroupBy(o => o.RATION_TIME_ID);
                foreach (var rationItem in rationGroup)
                {
                    var data = rationItem.FirstOrDefault();
                    HIS_RATION_TIME rationtime = new HIS_RATION_TIME();
                    rationtime = RationName.FirstOrDefault(o => o.ID == data.RATION_TIME_ID);
                    Ration.Add(rationtime);
                }
                //
               
                var groupSereServ = rdo.ListSereServ.GroupBy(o => new { o.TDL_SERVICE_CODE, o.PRICE }).ToList();

                SetSingleKey(new KeyValue(Mps000273ExtendSingleKey.REQUEST_DEPARTMENT_NAME, rdo.ListSereServ.FirstOrDefault().REQUEST_DEPARTMENT_NAME));

                foreach (var group in groupSereServ)
                {
                    var fistGroup = group.FirstOrDefault();
                    SereServADO Parent = new SereServADO(fistGroup);
                    Parent.TITLE_NAME = fistGroup.EXECUTE_DEPARTMENT_NAME;
                    Parent.AMOUNT_SUM = group.Sum(o => o.AMOUNT);
                    Output.Add(Parent);

                    //Nhóm theo buổi
                    var sereServTemps = group.GroupBy(o => o.RATION_TIME_ID).ToList();
                    foreach (var itemdt in sereServTemps)
                    {
                        var fistGr = itemdt.FirstOrDefault();
                        SereServADO SereServ = new SereServADO(fistGr);
                        SereServ.TITLE_NAME = RationName.FirstOrDefault(o => o.ID == fistGroup.RATION_TIME_ID).RATION_TIME_NAME ?? "";

                        SereServ.AMOUNT_SUM = itemdt.Sum(o => o.AMOUNT);
                        OutputRationTimeGroup.Add(SereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey(rdo.rationSum, false);
                AddObjectKeyIntoListkey(rdo.ListSereServ, false);
                AddObjectKeyIntoListkey(rdo.ado, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class CalculateMergerData : TFlexCelUserFunction
        {
            long typeId = 0;
            long mediMateTypeId = 0;

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length <= 0)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    long servicetypeId = Convert.ToInt64(parameters[0]);
                    long mediMateId = Convert.ToInt64(parameters[1]);

                    if (servicetypeId > 0 && mediMateId > 0)
                    {
                        if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                        {
                            return true;
                        }
                        else
                        {
                            this.typeId = servicetypeId;
                            this.mediMateTypeId = mediMateId;
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
