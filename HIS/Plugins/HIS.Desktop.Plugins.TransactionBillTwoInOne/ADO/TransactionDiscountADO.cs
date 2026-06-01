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
using System.ComponentModel;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO
{
    public class TransactionDiscountADO : INotifyPropertyChanged
    {
        private long? _id;
        private decimal? _discount;
        private decimal? _discountRatio;
        private string _reason;

        public long? ID
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("ID"); }
        }

        // Nullable: null -> ô grid hiển thị RỖNG (không hiện "0" lúc đầu, user gõ thẳng không phải xóa 0 thừa).
        public decimal? DISCOUNT
        {
            get { return _discount; }
            set { _discount = value; OnPropertyChanged("DISCOUNT"); }
        }

        public decimal? DISCOUNT_RATIO
        {
            get { return _discountRatio; }
            set { _discountRatio = value; OnPropertyChanged("DISCOUNT_RATIO"); }
        }

        public string REASON
        {
            get { return _reason; }
            set { _reason = value; OnPropertyChanged("REASON"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
