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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.DispenseMedicine.ADO;
using HIS.Desktop.Plugins.DispenseMedicine.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.DispenseMedicine
{
    public partial class frmDispenseMedicine : FormBase
    {
        List<DispenseMedyMatyADO> dispenseMedyMatyADOTemps { get; set; }

        private void CauHinhDinhMuc()
        {
            try
            {
                if (currentMetyThanhPham == null)
                {
                    MessageBox.Show("Vui lòng chọn thành phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<HIS_METY_MATY> metyMatys = new List<HIS_METY_MATY>();
                List<HIS_METY_METY> metyMetys = new List<HIS_METY_METY>();
                if (dispenseMetyMatyADOs != null && dispenseMetyMatyADOs.Count > 0)
                {
                    foreach (var item in dispenseMetyMatyADOs)
                    {
                        if (item.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            HIS_METY_MATY metyMaty = new HIS_METY_MATY();
                            metyMaty.METY_PRODUCT_ID = currentMetyThanhPham.ID;
                            metyMaty.MATERIAL_TYPE_ID = item.PreparationMediMatyTypeId;
                            metyMaty.MATERIAL_TYPE_AMOUNT = spinMetyAmount.EditValue == null || spinMetyAmount.Value == 0 ? item.Amount : item.Amount / spinMetyAmount.Value;
                            metyMatys.Add(metyMaty);
                        }
                        else if (item.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            HIS_METY_METY metyMety = new HIS_METY_METY();
                            metyMety.METY_PRODUCT_ID = currentMetyThanhPham.ID;
                            metyMety.PREPARATION_MEDICINE_TYPE_ID = item.PreparationMediMatyTypeId;
                            metyMety.PREPARATION_AMOUNT = spinMetyAmount.EditValue == null || spinMetyAmount.Value == 0 ? item.Amount : item.Amount / spinMetyAmount.Value;
                            metyMetys.Add(metyMety);
                        }
                    }
                }
                else
                {
                    HIS_METY_MATY metyMaty = new HIS_METY_MATY();
                    metyMaty.METY_PRODUCT_ID = currentMetyThanhPham.ID;
                    metyMatys.Add(metyMaty);
                }


                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MetyMaty").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.MetyMaty");

                V_HIS_METY_PRODUCT MetyProduct =new  V_HIS_METY_PRODUCT();
                  Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_METY_PRODUCT>(MetyProduct, currentMetyThanhPham);
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(metyMatys);
                    listArgs.Add(metyMetys);
                    listArgs.Add(MetyProduct);
                    //listArgs.Add((DelegateReturnMutilObject)ReloadMetyMaty);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.module.RoomId, this.module.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadMetyMaty(object[] args)
        {
            try
            {
                dispenseMedyMatyADOTemps = (from r in dispenseMetyMatyADOs select new DispenseMedyMatyADO(r)).ToList();
                dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();
                if (args != null && args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] is List<HIS_METY_METY>)
                        {
                            this.MakeMetyMety((List<HIS_METY_METY>)args[i]);
                        }
                        else if (args[i] is List<HIS_METY_MATY>)
                        {
                            this.MakeMetyMaty((List<HIS_METY_MATY>)args[i]);
                        }
                    }
                }
                gridControlDSChePham.DataSource = dispenseMetyMatyADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MakeMetyMety(List<HIS_METY_METY> data)
        {
            try
            {
                if (data != null)
                {
                    List<HIS_METY_METY> metyMetys = data as List<HIS_METY_METY>;
                    List<V_HIS_MEDICINE_TYPE> medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    if (metyMetys != null && metyMetys.Count > 0)
                    {
                        foreach (var item in metyMetys)
                        {
                            V_HIS_MEDICINE_TYPE medicineType = medicineTypes.FirstOrDefault(o => o.ID == item.PREPARATION_MEDICINE_TYPE_ID);
                            DispenseMedyMatyADO dispenseMatyADO = new DispenseMedyMatyADO();
                            dispenseMatyADO.PreparationMediMatyTypeId = item.PREPARATION_MEDICINE_TYPE_ID;

                            dispenseMatyADO.Amount = spinMetyAmount.EditValue == null || spinMetyAmount.Value == 0
                                ? item.PREPARATION_AMOUNT : spinMetyAmount.Value * item.PREPARATION_AMOUNT;
                            dispenseMatyADO.CFGAmount = item.PREPARATION_AMOUNT;


                            if (action == ACTION.UPDATE)
                            {
                                if (dispenseMedyMatyADOTemps != null && dispenseMedyMatyADOTemps.Count > 0)
                                {
                                    DispenseMedyMatyADO dispenseMedyMatyADO = dispenseMedyMatyADOTemps.FirstOrDefault(o=>o.PreparationMediMatyTypeId == item.PREPARATION_MEDICINE_TYPE_ID);
                                    if (dispenseMedyMatyADO == null || (dispenseMedyMatyADO.Amount < item.PREPARATION_AMOUNT && dispenseMedyMatyADO.IsNotAvaliable == false))
                                    {
                                        dispenseMatyADO.IsNotAvaliable = true;
                                    }
                                }
                            }
                            else
                            {

                                D_HIS_MEDI_STOCK_1 mediStock = mediStockD1SDOs.FirstOrDefault(o => o.SERVICE_TYPE_ID == medicineType.SERVICE_TYPE_ID && o.ID == item.PREPARATION_MEDICINE_TYPE_ID);
                                if (mediStock == null || mediStock.AMOUNT < item.PREPARATION_AMOUNT)
                                {
                                    dispenseMatyADO.IsNotAvaliable = true;
                                }
                            }

                            if (medicineType != null)
                            {
                                dispenseMatyADO.ServiceUnitName = medicineType.SERVICE_UNIT_NAME;
                                dispenseMatyADO.ServiceTypeId = medicineType.SERVICE_TYPE_ID;
                                dispenseMatyADO.PreparationMediMatyTypeName = medicineType.MEDICINE_TYPE_NAME;
                            }
                            if (dispenseMetyMatyADOs == null)
                                dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();
                            dispenseMetyMatyADOs.Add(dispenseMatyADO);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MakeMetyMaty(List<HIS_METY_MATY> data)
        {
            try
            {
                if (data != null)
                {
                    List<HIS_METY_MATY> metyMatys = data as List<HIS_METY_MATY>;
                    List<V_HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    if (metyMatys != null && metyMatys.Count > 0)
                    {
                        foreach (var item in metyMatys)
                        {

                            V_HIS_MATERIAL_TYPE materialType = materialTypes.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                            DispenseMedyMatyADO dispenseMatyADO = new DispenseMedyMatyADO();
                            dispenseMatyADO.PreparationMediMatyTypeId = item.MATERIAL_TYPE_ID;
                            dispenseMatyADO.Amount = spinMetyAmount.EditValue == null || spinMetyAmount.Value == 0 ? item.MATERIAL_TYPE_AMOUNT : spinMetyAmount.Value * item.MATERIAL_TYPE_AMOUNT;
                            dispenseMatyADO.CFGAmount = item.MATERIAL_TYPE_AMOUNT;

                            if (action == ACTION.UPDATE)
                            {
                                if (dispenseMedyMatyADOTemps != null && dispenseMedyMatyADOTemps.Count > 0)
                                {
                                    DispenseMedyMatyADO dispenseMedyMatyADO = dispenseMedyMatyADOTemps.FirstOrDefault(o => o.PreparationMediMatyTypeId == item.MATERIAL_TYPE_ID);
                                    if (dispenseMedyMatyADO == null || (dispenseMedyMatyADO.Amount < item.MATERIAL_TYPE_AMOUNT && dispenseMedyMatyADO.IsNotAvaliable == false))
                                    {
                                        dispenseMatyADO.IsNotAvaliable = true;
                                    }
                                }
                            }
                            else
                            {
                                D_HIS_MEDI_STOCK_1 mediStock = mediStockD1SDOs.FirstOrDefault(o => o.SERVICE_TYPE_ID == materialType.SERVICE_TYPE_ID && o.ID == item.MATERIAL_TYPE_ID);
                                if (mediStock == null || mediStock.AMOUNT < item.MATERIAL_TYPE_AMOUNT)
                                {
                                    dispenseMatyADO.IsNotAvaliable = true;
                                }
                            }

                            if (materialType != null)
                            {
                                dispenseMatyADO.ServiceUnitName = materialType.SERVICE_UNIT_NAME;
                                dispenseMatyADO.ServiceTypeId = materialType.SERVICE_TYPE_ID;
                                dispenseMatyADO.PreparationMediMatyTypeName = materialType.MATERIAL_TYPE_NAME;
                            }
                            if (dispenseMetyMatyADOs == null)
                                dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();
                            dispenseMetyMatyADOs.Add(dispenseMatyADO);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
