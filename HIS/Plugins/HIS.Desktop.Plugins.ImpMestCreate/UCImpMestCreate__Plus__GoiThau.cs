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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using HIS.Desktop.Plugins.ImpMestCreate.Base;
using HIS.Desktop.Plugins.ImpMestCreate.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate
{
    public partial class UCImpMestCreate : UserControlBase
    {
        private void InitComboGoiThau()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboGoiThau, listBids, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboGoiThau(List<V_HIS_BID_1> datas, long bidId)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboGoiThau, datas, controlEditorADO);

                cboGoiThau.EditValue = bidId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_BID_1> _HisBidBySuppliers { get; set; }//lay ds goi thau theo nha cc, bat buoc chon nha cung cap truoc
        private bool isEnableGoiThau;//Neu  chon thau truoc thuoc thi k xu li gi them

        //List<V_HIS_BID_MEDICINE_TYPE> _BidMedicineTypes { get; set; }
        // List<V_HIS_BID_MATERIAL_TYPE> _BidMaterialTypes { get; set; }

        Dictionary<long, List<V_HIS_BID_MEDICINE_TYPE>> _dicMedicineTypes { get; set; }
        Dictionary<long, List<V_HIS_BID_MATERIAL_TYPE>> _dicMaterialTypes { get; set; }

        Dictionary<long, List<V_HIS_BID_MEDICINE_TYPE>> _dicBidMedicineTypes = new Dictionary<long, List<V_HIS_BID_MEDICINE_TYPE>>();
        Dictionary<long, List<V_HIS_BID_MATERIAL_TYPE>> _dicBidMaterialTypes = new Dictionary<long, List<V_HIS_BID_MATERIAL_TYPE>>();

        private void ProcessBidByType()
        {
            try
            {
                List<long> _bidIds = new List<long>();
                this.cboGoiThau.Enabled = false;
                if (this.currrentServiceAdo == null)
                {
                    return;
                }

                if (this.currrentServiceAdo.IsMedicine)
                {
                    HisBidMedicineTypeViewFilter filter = new HisBidMedicineTypeViewFilter();
                    filter.MEDICINE_TYPE_ID = this.currrentServiceAdo.MEDI_MATE_ID;
                    var datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MEDICINE_TYPE>>("api/HisBidMedicineType/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (datas != null && datas.Count > 0)
                    {
                        if (this.currentSupplierForEdit != null && this.currentSupplierForEdit.ID > 0)
                            datas = datas.Where(p => p.SUPPLIER_ID == this.currentSupplierForEdit.ID).ToList();//ktra lai xem thau co thuoc nha CC hay k?
                        _dicMedicineTypes.Clear();
                        foreach (var item in datas)
                        {
                            _bidIds.Add(item.BID_ID);
                            if (!_dicMedicineTypes.ContainsKey(item.BID_ID))
                            {
                                _dicMedicineTypes[item.BID_ID] = new List<V_HIS_BID_MEDICINE_TYPE>();
                                _dicMedicineTypes[item.BID_ID].Add(item);
                            }
                            else
                                _dicMedicineTypes[item.BID_ID].Add(item);

                            if (!_dicBidMedicineTypes.ContainsKey(item.MEDICINE_TYPE_ID))
                            {
                                _dicBidMedicineTypes[item.MEDICINE_TYPE_ID] = new List<V_HIS_BID_MEDICINE_TYPE>();
                            }

                            if (!_dicBidMedicineTypes[item.MEDICINE_TYPE_ID].Exists(o => o.ID == item.ID))
                            {
                                _dicBidMedicineTypes[item.MEDICINE_TYPE_ID].Add(item);
                            }
                        }
                    }
                }
                else
                {
                    HisBidMaterialTypeViewFilter filter = new HisBidMaterialTypeViewFilter();
                    filter.MATERIAL_TYPE_ID = this.currrentServiceAdo.MEDI_MATE_ID;
                    var datas = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BID_MATERIAL_TYPE>>("api/HisBidMaterialType/GetView", ApiConsumers.MosConsumer, filter, null);

                    if (datas != null && datas.Count > 0)
                    {
                        if (this.currentSupplierForEdit != null && this.currentSupplierForEdit.ID > 0)
                            datas = datas.Where(p => p.SUPPLIER_ID == this.currentSupplierForEdit.ID).ToList();//ktra lai xem thau co thuoc nha CC hay k?
                        _dicMaterialTypes.Clear();
                        foreach (var item in datas)
                        {
                            _bidIds.Add(item.BID_ID);
                            if (!_dicMaterialTypes.ContainsKey(item.BID_ID))
                            {
                                _dicMaterialTypes[item.BID_ID] = new List<V_HIS_BID_MATERIAL_TYPE>();
                                _dicMaterialTypes[item.BID_ID].Add(item);
                            }
                            else
                                _dicMaterialTypes[item.BID_ID].Add(item);

                            if (item.MATERIAL_TYPE_ID.HasValue)
                            {
                                if (!_dicBidMaterialTypes.ContainsKey(item.MATERIAL_TYPE_ID ?? 0))
                                {
                                    _dicBidMaterialTypes[item.MATERIAL_TYPE_ID ?? 0] = new List<V_HIS_BID_MATERIAL_TYPE>();
                                }

                                if (!_dicBidMaterialTypes[item.MATERIAL_TYPE_ID ?? 0].Exists(o => o.ID == item.ID))
                                {
                                    _dicBidMaterialTypes[item.MATERIAL_TYPE_ID ?? 0].Add(item);
                                }
                            }
                            else if (item.MATERIAL_TYPE_MAP_ID.HasValue)
                            {
                                var materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.MATERIAL_TYPE_MAP_ID == item.MATERIAL_TYPE_MAP_ID).ToList();
                                if (materialType != null && materialType.Count > 0)
                                {
                                    foreach (var maty in materialType)
                                    {
                                        if (!_dicBidMaterialTypes.ContainsKey(maty.ID))
                                        {
                                            _dicBidMaterialTypes[maty.ID] = new List<V_HIS_BID_MATERIAL_TYPE>();
                                        }

                                        if (!_dicBidMaterialTypes[maty.ID].Exists(o => o.ID == item.ID))
                                        {
                                            _dicBidMaterialTypes[maty.ID].Add(item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (_HisBidBySuppliers != null
                    && _HisBidBySuppliers.Count > 0
                    && this.currrentServiceAdo != null
                    && !isEnableGoiThau
                    && (!checkOutBid.Checked || checkInOutBid.Checked)
                    )
                {
                    // if (!isEnableGoiThau)
                    cboGoiThau.Properties.DataSource = null;
                    if (cboImpMestType.EditValue != null && Convert.ToInt64(cboImpMestType.EditValue) == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                    {
                        this.lciGoiThau.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    }
                    else
                    {
                        this.lciGoiThau.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    }

                    var _dataSources = (_bidIds != null) ? _HisBidBySuppliers.Where(p => _bidIds.Contains(p.ID)).ToList() : _HisBidBySuppliers;
                    long bidId = 0;
                    if (_dataSources != null && _dataSources.Count > 0)
                    {
                        _dataSources = _dataSources.OrderByDescending(p => p.MODIFY_TIME).ToList();
                        if (!checkOutBid.Checked)
                            bidId = _dataSources.FirstOrDefault().ID;
                        if (checkInOutBid.Checked)
                            bidId = _dataSources.OrderByDescending(p => p.ID).Last().ID;
                    }

                    InitComboGoiThau(_dataSources, bidId);// cboGoiThau.Properties.DataSource = _dataSources;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGoiThau_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboGoiThau.EditValue = null;
                    this.cboGoiThau.Properties.Buttons[1].Visible = false;

                    //TODO -- 
                    txtBidYear.Enabled = true;
                    txtBidNumber.Enabled = true;
                    txtBidNumOrder.Enabled = true;
                    txtBidGroupCode.Enabled = true;
                    txtBid.Enabled = true;
                    this.currentBid = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
