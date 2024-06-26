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
using MPS.Processor.Mps000099.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000099
{
    public class Mps000099Processor : AbstractProcessor
    {
        Mps000099PDO rdo;
        List<Mps000099ADO> listAdo = new List<Mps000099ADO>();
        public Mps000099Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000099PDO)rdoBase;
        }



        public override bool ProcessData()
        {
            bool result = false;
            try
            {

                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                //đọc template trong MPS
                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                ProcessListData();
                ProcessSingleKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMedi", this.listAdo);
                //objectTag.AddObjectData(store, "ListSaleExpMest", rdo._LstSaleExpMest);
                //objectTag.AddObjectData(store, "ListSaleExpMestMater", rdo._LstSaleExpMestMater);
                //objectTag.AddObjectData(store, "ListMaterial", rdo._Materials);
                //if (rdo._Medicines != null && rdo._Medicines.Count > 0 && (rdo._Materials == null || rdo._Materials.Count == 0))
                //{
                //    objectTag.AddRelationship(store, "ListMedi", "ListSaleExpMest", "EXP_MEST_ID", "ID");
                //}
                //if (rdo._Materials != null && rdo._Materials.Count > 0 && (rdo._Medicines == null || rdo._Medicines.Count == 0))
                //{
                //    objectTag.AddRelationship(store, "ListMaterial", "ListSaleExpMestMater", "EXP_MEST_ID", "ID");
                //}
                //if (rdo._Medicines != null && rdo._Materials != null)
                //{
                //    objectTag.AddRelationship(store, "ListMedi", "ListSaleExpMest", "EXP_MEST_ID", "ID");
                //    objectTag.AddRelationship(store, "ListMaterial", "ListSaleExpMestMater", "EXP_MEST_ID", "ID");
                //}

                result = true;
            }
            catch (Exception ex)
            {
                return result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListData()
        {
            try
            {
                if (rdo != null)
                {
                    if (rdo._Medicines != null && rdo._Medicines.Count > 0)
                    {
                        foreach (var item in rdo._Medicines)
                        {
                            if (item.EXP_MEST_ID != null)
                            {
                                V_HIS_EXP_MEST exp = new V_HIS_EXP_MEST();
                                if (rdo._LstSaleExpMest != null && rdo._LstSaleExpMest.Count > 0)
                                {
                                    exp = rdo._LstSaleExpMest.FirstOrDefault(o => o.ID == item.EXP_MEST_ID);
                                }
                                else if (rdo._SaleExpMest != null)
                                {
                                    exp = rdo._SaleExpMest;
                                }
                                this.listAdo.Add(new Mps000099ADO(exp, item));
                            }
                        }
                    }

                    if (rdo._Materials != null && rdo._Materials.Count > 0)
                    {
                        foreach (var item in rdo._Materials)
                        {
                            if (item.EXP_MEST_ID != null)
                            {
                                V_HIS_EXP_MEST exp = new V_HIS_EXP_MEST();
                                if (rdo._LstSaleExpMest != null && rdo._LstSaleExpMest.Count > 0)
                                {
                                    exp = rdo._LstSaleExpMest.FirstOrDefault(o => o.ID == item.EXP_MEST_ID);
                                }
                                else if (rdo._SaleExpMest != null)
                                {
                                    exp = rdo._SaleExpMest;
                                }

                                //V_HIS_EXP_MEST exp = rdo._LstSaleExpMest.FirstOrDefault(o => o.ID == item.EXP_MEST_ID);
                                this.listAdo.Add(new Mps000099ADO(exp, item));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                if (this.rdo._SaleExpMest != null)
                {
                    if (!String.IsNullOrEmpty(this.rdo._SaleExpMest.TDL_PATIENT_GENDER_NAME))
                        SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.GENDER_NAME, this.rdo._SaleExpMest.TDL_PATIENT_GENDER_NAME));
                    if (this.rdo._SaleExpMest != null)
                    {
                        SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.PATIENT_CODE, this.rdo._SaleExpMest.TDL_PATIENT_CODE));
                        SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.PATIENT_NAME, this.rdo._SaleExpMest.TDL_PATIENT_NAME));
                    }
                    SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.REQ_USERNAME, this.rdo._SaleExpMest.REQ_USERNAME));
                    SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.AGE, Inventec.Common.DateTime.Calculation.Age(this.rdo._SaleExpMest.TDL_PATIENT_DOB ?? 0)));
                }

                if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                {
                    SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.MEDICINE_TYPE_NAME, this.rdo._Medicines.First().MEDICINE_TYPE_NAME));
                    SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.AMOUNT, this.rdo._Medicines.Sum(o => o.AMOUNT)));
                    SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.SERVICE_UNIT_NAME, this.rdo._Medicines.First().SERVICE_UNIT_NAME));
                    SetSingleKey(new KeyValue(Mps000099ExtendSingleKey.TUTORIAL, this.rdo._Medicines.First().TUTORIAL));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
