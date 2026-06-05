/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using HIS.UC.TransactionPayformGrid.ADO;
using HIS.UC.TransactionPayformGrid.GetData;
using HIS.UC.TransactionPayformGrid.Reload;
using HIS.UC.TransactionPayformGrid.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.UC.TransactionPayformGrid
{
    /// <summary>
    /// Entry point cua UC luoi hinh thuc thanh toan. Form cha goi:
    ///   var proc = new UCTransactionPayformGridProcessor(param);
    ///   UserControl uc = (UserControl)proc.Run(initADO);
    ///   panelHost.Controls.Add(uc); uc.Dock = DockStyle.Fill;
    ///   ... khi bam nut tao giao dich:
    ///   var rows = proc.GetData(uc) as List&lt;PayformRowADO&gt;;
    /// </summary>
    public class UCTransactionPayformGridProcessor : BussinessBase
    {
        object uc;

        public UCTransactionPayformGridProcessor()
            : base()
        {
        }

        public UCTransactionPayformGridProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        /// <summary>Khoi tao UC voi du lieu dau vao</summary>
        public object Run(TransactionPayformGridInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIRun(param, arg);
                uc = behavior != null ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        /// <summary>Nap lai danh sach dong cho luoi</summary>
        public void Reload(UserControl control, List<PayformRowADO> data)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lay danh sach dong hien tai (List&lt;PayformRowADO&gt;) - dung khi bam tao giao dich</summary>
        public object GetData(UserControl control)
        {
            object result = null;
            try
            {
                IGetData behavior = GetDataFactory.MakeIGetData(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Cap nhat so tien phai thu (Can thu) - tinh lai cot Con lai / Con thieu</summary>
        public void SetRequiredAmount(UserControl control, decimal requiredAmount)
        {
            try
            {
                UserControl target = (control == null ? (UserControl)uc : control);
                if (target is UCTransactionPayformGrid)
                {
                    ((UCTransactionPayformGrid)target).SetRequiredAmount(requiredAmount);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Lay tong thanh tien (VND) cua tat ca cac dong</summary>
        public decimal GetTotalAmount(UserControl control)
        {
            decimal result = 0;
            try
            {
                UserControl target = (control == null ? (UserControl)uc : control);
                if (target is UCTransactionPayformGrid)
                {
                    result = ((UCTransactionPayformGrid)target).GetTotalAmount();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Kiem tra tinh hop le cua luoi truoc khi tao giao dich</summary>
        public bool ValidateData(UserControl control)
        {
            bool result = false;
            try
            {
                UserControl target = (control == null ? (UserControl)uc : control);
                if (target is UCTransactionPayformGrid)
                {
                    result = ((UCTransactionPayformGrid)target).ValidateData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
