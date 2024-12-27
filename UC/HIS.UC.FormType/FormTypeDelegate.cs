using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType
{
    public class FormTypeDelegate
    {
        private static ProcessFormType processFormType;
        public static ProcessFormType ProcessFormType
        {
            get
            {
                return processFormType;
            }
            set
            {
                processFormType = value;
            }
        }

        private static GetCashierUser getCashierUser;
        public static GetCashierUser GetCashierUser
        {
            get
            {
                return getCashierUser;
            }
            set
            {
                getCashierUser = value;
            }
        }

        private static GetSaleUser getSaleUser;
        public static GetSaleUser GetSaleUser
        {
            get
            {
                return getSaleUser;
            }
            set
            {
                getSaleUser = value;
            }
        }

        private static PagingGetDelegate pagingGet;
        public static PagingGetDelegate PagingGet
        {
            get
            {
                return pagingGet;
            }
            set
            {
                pagingGet = value;
            }
        }

    }
}
