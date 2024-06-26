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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggrExam.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggrExam
{
    public partial class UCExpMestAggrExam : HIS.Desktop.Utility.UserControlBase
    {
        /// <summary>
        /// Chi Tiet Don Thuoc Vat Tu
        /// </summary>
        /// <param name="_expMest"></param>
        /// <returns></returns>
        internal static List<SereServADO> LoadDataDetailExpMest(V_HIS_EXP_MEST _expMest)
        {
            List<SereServADO> _SereServs = new List<SereServADO>();//luu tat ca thuoc va vat tu
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<V_HIS_EXP_MEST_MEDICINE> _expMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_ID = _expMest.ID;
                _expMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                if (_expMedicines != null && _expMedicines.Count > 0)
                {
                    var dataGroups = _expMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    var _meidicneTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in dataGroups)
                    {
                        var dataType = _meidicneTypes.FirstOrDefault(p => p.ID == item[0].MEDICINE_TYPE_ID);
                        if (dataType == null)
                        {
                            continue;
                        }

                        SereServADO ado = new SereServADO();
                        foreach (var itemNumber in item)
                        {
                            ado.PACKAGE_NUMBER += (!String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? "; " + itemNumber.PACKAGE_NUMBER : "");
                        }

                        ado.AMOUNT = item.Sum(p => p.AMOUNT);

                        ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = dataType.MEDICINE_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = dataType.MEDICINE_TYPE_NAME;
                        _SereServs.Add(ado);
                    }
                }



                List<V_HIS_EXP_MEST_MATERIAL> _expMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                MOS.Filter.HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                materialFilter.EXP_MEST_ID = _expMest.ID;
                _expMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, param);
                if (_expMaterials != null && _expMaterials.Count > 0)
                {
                    var dataGroups = _expMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in dataGroups)
                    {
                        var dataType = _materialTypes.FirstOrDefault(p => p.ID == item[0].MATERIAL_TYPE_ID);
                        if (dataType == null)
                        {
                            continue;
                        }
                        SereServADO ado = new SereServADO();
                        foreach (var itemNumber in item)
                        {
                            ado.PACKAGE_NUMBER += !String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? "; " + itemNumber.PACKAGE_NUMBER : "";
                        }
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.PACKAGE_NUMBER = "";
                        ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = dataType.MATERIAL_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = dataType.MATERIAL_TYPE_NAME;
                        _SereServs.Add(ado);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }

            return _SereServs;
        }
    }

    public class SereServADO
    {
        public string SERVICE_UNIT_NAME { get; set; }
        public string TDL_SERVICE_CODE { get; set; }
        public string TDL_SERVICE_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public string PACKAGE_NUMBER { get; set; }
    }

}
