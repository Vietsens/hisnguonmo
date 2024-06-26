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
using AutoMapper;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000326.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000326
{
    public class Mps000326Processor : AbstractProcessor
    {
        Mps000326PDO rdo;

        private List<Mps000326ADO> listAdo = new List<Mps000326ADO>();

        public Mps000326Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000326PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                SetSingleKey();
                SetListData();
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListData", listAdo);
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (this.rdo._HoreHandover != null)
                {
                    AddObjectKeyIntoListkey(this.rdo._HoreHandover, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListData()
        {
            try
            {
                if (this.rdo._HoreHohas != null && this.rdo._HoreHohas.Count > 0)
                {
                    Mapper.CreateMap<V_HIS_HORE_HOHA, Mps000326ADO>();
                    foreach (var item in this.rdo._HoreHohas)
                    {
                        Mps000326ADO ado = Mapper.Map<Mps000326ADO>(item);
                        List<string> types = new List<string>();
                        if (!String.IsNullOrWhiteSpace(item.DOC_HOLD_TYPE_NAMES))
                        {
                            string[] aggrs = item.DOC_HOLD_TYPE_NAMES.Split(',');
                            types = aggrs != null ? aggrs.Where(o => !String.IsNullOrWhiteSpace(o)).ToList() : types;
                        }

                        if (types != null && types.Count > 0)
                        {
                            ado.Description = types.Count + "\n- ";
                            ado.Description = ado.Description + String.Join("\n- ", types);
                        }
                        if (ado.DOB > 0)
                        {
                            ado.DOB_YEAR = ado.DOB.ToString().Substring(0, 4);
                        }
                        listAdo.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
