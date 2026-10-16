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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.ContentSubclinical;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ContentSubclinical.ContentSubclinical
{
    public sealed class ContentSubclinicalBehavior : Tool<IDesktopToolContext>, IContentSubclinical
    {
        /// <summary>
        /// Cờ truyền qua args (List&lt;string&gt;) từ plugin gọi: mở form ở chế độ CHỈ XEM kết quả CLS
        /// (ẩn tích chọn/chèn vào tờ điều trị), không yêu cầu DelegateSelectData.
        /// </summary>
        public const string ARG__VIEW_ONLY = "VIEW_ONLY";

        /// <summary>
        /// Cờ trong List&lt;string&gt; args: kết quả dạng CHUỖI trả về qua DelegateSelectData được bọc
        /// thành HIS.Desktop.ADO.ContentSubclinicalResultADO (RESULT + người thực hiện y lệnh) thay cho
        /// string thuần. Không truyền cờ → trả string như cũ (QD831 và các plugin khác không đổi).
        /// </summary>
        public const string ARG__WITH_EXECUTOR = "WITH_EXECUTOR";

        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId = 0;
        bool returnObject = false;
        bool isViewOnly = false;
        public ContentSubclinicalBehavior()
            : base()
        {
        }

        public ContentSubclinicalBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IContentSubclinical.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    HIS.Desktop.Common.DelegateSelectData _DelegateSelectData = null;
                    HIS.Desktop.Common.DelegateSelectTestIndexGroupData _DelegateTestIndexGroup = null;
                    string _serviceIds = null; // có = chế độ headless lấy kết quả theo serviceIds
                    bool withExecutor = false; // cờ WITH_EXECUTOR: kết quả chuỗi bọc kèm người thực hiện y lệnh
                    bool isClsTab = false; // Viec 57971: flag CLS_TAB - view-only mode shows the results in service group tabs
                    string clsTabInitCode = null; // Viec 57971: flag CLS_TAB_INIT:<group code> - tab opened first

                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                        else if (item is HIS.Desktop.Common.DelegateSelectTestIndexGroupData)
                        {
                            // Có delegate này = chế độ tự động lấy chỉ số xét nghiệm theo nhóm (headless).
                            _DelegateTestIndexGroup = (HIS.Desktop.Common.DelegateSelectTestIndexGroupData)item;
                        }
                        else if (item is HIS.Desktop.Common.DelegateSelectData)
                        {
                            _DelegateSelectData = (HIS.Desktop.Common.DelegateSelectData)item;
                        }
                        else if (item is string)
                        {
                            _serviceIds = (string)item;
                        }
                        else if (item is bool)
                        {
                            returnObject = (bool)item;
                        }
                        else if (item is List<string>)
                        {
                            // Cờ chế độ từ plugin gọi (không dùng string/bool đơn vì đã bị chiếm bởi
                            // serviceIds/returnObject — truyền nhầm sẽ kích chế độ headless).
                            List<string> flags = (List<string>)item;
                            if (flags.Contains(ARG__VIEW_ONLY))
                            {
                                isViewOnly = true;
                            }
                            if (flags.Contains(ARG__WITH_EXECUTOR))
                            {
                                withExecutor = true;
                            }
                            if (flags.Contains(Base.ClsTabGroupStore.ARG__CLS_TAB))
                            {
                                isClsTab = true;
                            }
                            string initFlag = flags.FirstOrDefault(o => o != null && o.StartsWith(Base.ClsTabGroupStore.ARG__CLS_TAB_INIT_PREFIX, StringComparison.Ordinal));
                            if (initFlag != null)
                            {
                                clsTabInitCode = initFlag.Substring(Base.ClsTabGroupStore.ARG__CLS_TAB_INIT_PREFIX.Length);
                            }
                        }

                    }

                    if (_DelegateTestIndexGroup != null)
                    {
                        // Bọc delegate nhóm chỉ số vào DelegateSelectData để form dùng chung cơ chế callback.
                        HIS.Desktop.Common.DelegateSelectData wrapper = (data) => _DelegateTestIndexGroup(data);
                        if (currentModule != null && treatmentId > 0)
                        {
                            // Headless: tạo form (KHÔNG Show) và load đồng bộ + bắn delegate ngay.
                            frmContentSubclinical frm = new frmContentSubclinical(currentModule, treatmentId, wrapper, true);
                            frm.RunHeadlessTestIndexGroup();
                            result = frm; // plugin gọi sẽ Dispose
                        }
                    }
                    else if (currentModule != null && treatmentId > 0 && _DelegateSelectData != null && _serviceIds != null)
                    {
                        // Headless: lấy kết quả các dịch vụ con theo serviceIds đã cấu hình, trả chuỗi (giống "+").
                        frmContentSubclinical frm = new frmContentSubclinical(currentModule, treatmentId, _DelegateSelectData, false);
                        frm.IsWithExecutor = withExecutor;
                        frm.RunHeadlessByServiceIds(_serviceIds);
                        result = frm; // plugin gọi sẽ Dispose
                    }
                    else if (currentModule != null && treatmentId > 0 && _DelegateSelectData != null)
                    {
                        frmContentSubclinical frm = new frmContentSubclinical(currentModule, treatmentId, _DelegateSelectData, returnObject);
                        frm.IsWithExecutor = withExecutor;
                        result = frm;
                    }
                    else if (isViewOnly && currentModule != null && treatmentId > 0)
                    {
                        // Chế độ CHỈ XEM (mở từ ngoài tờ điều trị, vd nút "Kết quả CLS" màn Buồng bệnh):
                        // không cần DelegateSelectData vì không trả dữ liệu về plugin gọi.
                        frmContentSubclinical frm = new frmContentSubclinical(currentModule, treatmentId, true);
                        frm.IsClsTabMode = isClsTab;
                        frm.InitialClsTabCode = clsTabInitCode;
                        result = frm;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
