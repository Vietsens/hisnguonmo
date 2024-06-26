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
using DevExpress.XtraTreeList;
using System;
using System.ComponentModel;

namespace Inventec.UC.TreeSereServHein.ADO
{
    public class SereServADO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV
    {
        string serviceName;
        object price;
        object heinPrice;
        object amount;
        object vat;
        object totalPrice;
        object totalPatientPrice;
        object checkState;
        MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServDTO;
        SereServModelList owner;
        SereServModelList sereServModelList;
        public SereServADO()
        {
            this.owner = null;
            this.ServiceName = "";
            this.Price = null;
            this.HeinPrice = null;
            this.Amount = null;
            this.Vat = null;
            this.TotalPrice = null;
            this.TotalPatientPrice = null;
            this.CheckState = null;
            this.sereServModelList = new SereServModelList();
        }
        public SereServADO(string serverName, object price, object vat, object heinPrice, object amount, object totalPrice, object totalPatientPrice, bool? checkState, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServ)
        {
            this.SereServDTO = sereServ;
            this.ServiceName = serverName;
            this.Price = ((price != null && !String.IsNullOrEmpty(price.ToString()) && Convert.ToDecimal(price) >= 0) ? price.ToString() : "");
            this.Vat = ((vat != null && !String.IsNullOrEmpty(vat.ToString()) && Convert.ToDecimal(vat) >= 0) ? vat.ToString() : "");
            this.HeinPrice = ((heinPrice != null && !String.IsNullOrEmpty(heinPrice.ToString()) && Convert.ToDecimal(heinPrice) >= 0) ? heinPrice.ToString() : "");// ((Convert.ToDecimal(heinPrice) <= 0) ? "" : (heinPrice ?? "").ToString());
            this.Amount = ((amount != null && !String.IsNullOrEmpty(amount.ToString()) && Convert.ToDecimal(amount) >= 0) ? amount.ToString() : ""); //(Convert.ToDecimal(amount) <= 0 ? "" : amount.ToString());
            this.TotalPrice = ((totalPrice != null && !String.IsNullOrEmpty(totalPrice.ToString()) && Convert.ToDecimal(totalPrice) >= 0) ? totalPrice.ToString() : ""); //(Convert.ToDecimal(totalPrice) <= 0 ? "" : totalPrice.ToString());
            this.TotalPatientPrice = ((totalPatientPrice != null && !String.IsNullOrEmpty(totalPatientPrice.ToString()) && Convert.ToDecimal(totalPatientPrice) >= 0) ? totalPatientPrice.ToString() : ""); //(Convert.ToDecimal(totalPatientPrice) <= 0 ? "" : totalPatientPrice.ToString());
            this.CheckState = checkState;
            this.sereServModelList = new SereServModelList();
        }
        public SereServADO(SereServModelList sereServModelList, string serverName, decimal price, decimal vat, decimal? heinPrice, decimal amount, object totalPrice, object totalPatientPrice, bool? checkState, bool? checkStateParent, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServ)
            : this(serverName, price, vat, heinPrice, amount, totalPrice, totalPatientPrice, checkState, sereServ)
        {
            this.sereServModelList = sereServModelList;
        }
        [Browsable(false)]
        public SereServModelList Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        [Browsable(false)]
        public MOS.EFMODEL.DataModels.V_HIS_SERE_SERV SereServDTO
        {
            get { return sereServDTO; }
            set
            {
                if (SereServDTO == value) return;
                sereServDTO = value;
                OnChanged();
            }
        }
        public string ServiceName
        {
            get { return serviceName; }
            set
            {
                if (ServiceName == value) return;
                serviceName = value;
                OnChanged();
            }
        }
        public object Price
        {
            get { return price; }
            set
            {
                if (Price == value) return;
                price = value;
                OnChanged();
            }
        }
        public object Vat
        {
            get { return vat; }
            set
            {
                if (vat == value) return;
                vat = value;
                OnChanged();
            }
        }
        public object HeinPrice
        {
            get { return heinPrice; }
            set
            {
                if (HeinPrice == value) return;
                heinPrice = value;
                OnChanged();
            }
        }
        public object CheckState
        {
            get { return checkState; }
            set
            {
                if (CheckState == value) return;
                checkState = value;
                OnChanged();
            }
        }
        [Browsable(false)]
        public object Amount
        {
            get { return amount; }
            set
            {
                if (Amount == value) return;
                amount = value;
                OnChanged();
            }
        }
        [Browsable(false)]
        public object TotalPrice
        {
            get { return totalPrice; }
            set
            {
                if (TotalPrice == value) return;
                totalPrice = value;
                OnChanged();
            }
        }
        [Browsable(false)]
        public object TotalPatientPrice
        {
            get { return totalPatientPrice; }
            set
            {
                if (TotalPatientPrice == value) return;
                totalPatientPrice = value;
                OnChanged();
            }
        }
        [Browsable(false)]
        public SereServModelList SereServModelList { get { return sereServModelList; } }
        void OnChanged()
        {
            if (owner == null) return;
            int index = owner.IndexOf(this);
            owner.ResetItem(index);
        }
    }

    //<treeList1>
    public class SereServModelList : BindingList<SereServADO>, TreeList.IVirtualTreeListData
    {
        void TreeList.IVirtualTreeListData.VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            SereServADO obj = info.Node as SereServADO;
            info.Children = obj.SereServModelList;
        }
        protected override void InsertItem(int index, SereServADO item)
        {
            item.Owner = this;
            base.InsertItem(index, item);
        }
        void TreeList.IVirtualTreeListData.VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            SereServADO obj = info.Node as SereServADO;
            switch (info.Column.FieldName)
            {
                case "ServiceName":
                    info.CellData = obj.ServiceName;
                    break;
                case "Price":
                    info.CellData = (obj.Price == null ? "" : Inventec.Common.Number.Convert.NumberToStringMoney((double)obj.Price));
                    break;
                case "Vat":
                    info.CellData = obj.Vat;
                    break;
                case "HeinPrice":
                    info.CellData = (obj.HeinPrice == null ? "" : Inventec.Common.Number.Convert.NumberToStringMoney((double)obj.HeinPrice));
                    break;
                case "Amount":
                    info.CellData = obj.Amount;
                    break;
                case "TotalPrice":
                    info.CellData = (obj.TotalPrice == null ? "" : Inventec.Common.Number.Convert.NumberToStringMoney((double)obj.TotalPrice));
                    break;
                case "TotalPatientPrice":
                    info.CellData = (obj.TotalPatientPrice == null ? "" : Inventec.Common.Number.Convert.NumberToStringMoney((double)obj.TotalPatientPrice));
                    break;
                case "CheckState":
                    info.CellData = obj.CheckState;
                    break;
                case "SereServDTO":
                    info.CellData = obj.SereServDTO;
                    break;
            }
        }
        void TreeList.IVirtualTreeListData.VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        {
            SereServADO obj = info.Node as SereServADO;
            switch (info.Column.FieldName)
            {
                case "ServiceName":
                    obj.ServiceName = (string)info.NewCellData;
                    break;
                case "Price":
                    obj.Price = (string)info.NewCellData;
                    break;
                case "Vat":
                    obj.Vat = (string)info.NewCellData;
                    break;
                case "HeinPrice":
                    obj.HeinPrice = (string)info.NewCellData;
                    break;
                case "Amount":
                    obj.Amount = (decimal)info.NewCellData;
                    break;
                case "TotalPrice":
                    obj.TotalPrice = (string)info.NewCellData;
                    break;
                case "TotalPatientPrice":
                    obj.TotalPatientPrice = (string)info.NewCellData;
                    break;
                case "CheckState":
                    obj.CheckState = (bool?)info.NewCellData;
                    break;
                case "SereServDTO":
                    obj.SereServDTO = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)info.NewCellData;
                    break;
            }
        }
    }
    //</treeList1>
}
