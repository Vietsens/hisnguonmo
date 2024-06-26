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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ExpMestMedicineGrid;
using MOS.SDO;
using HIS.UC.ExpMestMedicineGrid.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using DevExpress.Utils.Menu;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ExpMestOtherExport.Resources;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestOtherExport.Config;

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class UCExpMestOtherExport : HIS.Desktop.Utility.UserControlBase
    {
        private void InitResultSdoByExpMestId()
        {
            try
            {
                this.resultSdo = null;
                if (expMestId > 0)
                {
                    HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.ID = this.expMestId;
                    var listExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, null);
                    if (listExpMest == null || listExpMest.Count != 1)
                    {
                        throw new Exception("Khong lay duoc expMest theo id: " + expMestId);
                    }
                    var expMest = listExpMest.FirstOrDefault();

                    if (expMest != null)
                    {
                        if (expMest.EXP_MEST_REASON_ID == HisConfigCFG.HisExpMestReasonId__ThanhLy)
                        {
                            EnableControlPriceAndAVT(true);
                        }
                        else
                        {
                            EnableControlPriceAndAVT(false);
                        }
                    }

                    List<HIS_EXP_MEST_MEDICINE> listMedicine = new List<HIS_EXP_MEST_MEDICINE>();
                    List<HIS_EXP_MEST_MATERIAL> listMaterial = new List<HIS_EXP_MEST_MATERIAL>();
                    List<HIS_EXP_MEST_BLOOD> listBlood = new List<HIS_EXP_MEST_BLOOD>();

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_ID = expMest.ID;
                    var listExpMestMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);

                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_ID = expMest.ID;
                    var listExpMestMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, null);

                    HisExpMestBloodViewFilter expMestBloodFilter = new HisExpMestBloodViewFilter();
                    expMestBloodFilter.EXP_MEST_ID = expMest.ID;
                    var listExpMestBlood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLOOD>>("api/HisExpMestBlood/GetView", ApiConsumers.MosConsumer, expMestBloodFilter, null);

                    if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                    {
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                        listMedicine = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(listExpMestMedicine);
                        foreach (var item in listExpMestMedicine)
                        {
                            if (!dicTypeAdo.ContainsKey(TYPE_MEDICINE))
                                dicTypeAdo[TYPE_MEDICINE] = new Dictionary<long, MediMateTypeADO>();
                            var dic = dicTypeAdo[TYPE_MEDICINE];
                            MediMateTypeADO ado = new MediMateTypeADO(item);
                            dic[ado.MEDI_MATE_TYPE_ID] = ado;
                        }
                    }
                    if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                    {
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                        listMaterial = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(listExpMestMaterial);
                        foreach (var item in listExpMestMaterial)
                        {
                            if (!dicTypeAdo.ContainsKey(TYPE_MATERIAL))
                                dicTypeAdo[TYPE_MATERIAL] = new Dictionary<long, MediMateTypeADO>();
                            var dic = dicTypeAdo[TYPE_MATERIAL];

                            MediMateTypeADO ado = new MediMateTypeADO(item);
                            dic[ado.MEDI_MATE_TYPE_ID] = ado;
                        }
                    }

                    if (listExpMestBlood != null && listExpMestBlood.Count > 0)
                    {
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLOOD, HIS_EXP_MEST_BLOOD>();
                        listBlood = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_BLOOD>>(listExpMestBlood);
                        foreach (var item in listExpMestBlood)
                        {
                            if (!dicTypeAdo.ContainsKey(TYPE_BLOOD))
                                dicTypeAdo[TYPE_BLOOD] = new Dictionary<long, MediMateTypeADO>();
                            var dic = dicTypeAdo[TYPE_BLOOD];
                            MediMateTypeADO ado = new MediMateTypeADO(item);
                            dic[ado.BLOOD_ID] = ado;
                        }
                    }

                    if (listExpMestMedicine != null && listExpMestMedicine.Count > 0)
                    {
                        xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageExpMediMate;
                        xtraTabControlExpMest.SelectedTabPage = xtraTabPageExpMestMedi;
                    }
                    else if (listExpMestMaterial != null && listExpMestMaterial.Count > 0)
                    {
                        xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageExpMediMate;
                        xtraTabControlExpMest.SelectedTabPage = xtraTabPageExpMestMate;
                    }
                    else if (listExpMestBlood != null && listExpMestBlood.Count > 0)
                    {
                        xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageExpBloodSend;
                        xtraTabControlExpMest.SelectedTabPage = xtraTabPageBloodExp;
                    }

                    resultSdo = new HisExpMestResultSDO();
                    resultSdo.ExpMaterials = listMaterial;
                    resultSdo.ExpMedicines = listMedicine;
                    resultSdo.ExpBloods = listBlood;
                    resultSdo.ExpMest = expMest;
                    txtDescription.EditValue = expMest.DESCRIPTION;
                    if (expMest.TDL_TOTAL_PRICE != null)
                        spinEditDiscountPercent.EditValue = String.Format("{0:#,##0}", 100*(expMest.DISCOUNT ?? 0) / (expMest.TDL_TOTAL_PRICE));
                    else
                        spinEditDiscountPercent.EditValue = null;
                    cboReason.EditValue = expMest.EXP_MEST_REASON_ID;
                    //
                    txtRecipient.Text = expMest.RECIPIENT;
                    txtRecevingPlace.Text = expMest.RECEIVING_PLACE;
                    //ProcessFillDataBySuccess();
                    FillDataToGridExpMest();
                    FillDataGridExpMestDetail();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
