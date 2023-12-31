using Inventec.Common.Logging;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.MANAGER.Base;
using MOS.MANAGER.Token;
using MOS.SDO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MOS.MANAGER.HisImpMest
{
    public class HisImpMestManuViewFilterQuery : HisImpMestManuViewFilter
    {
        public HisImpMestManuViewFilterQuery()
            : base()
        {

        }

        internal List<System.Linq.Expressions.Expression<Func<V_HIS_IMP_MEST_MANU, bool>>> listVHisImpMestManuExpression = new List<System.Linq.Expressions.Expression<Func<V_HIS_IMP_MEST_MANU, bool>>>();

        

        internal HisImpMestSO Query()
        {
            HisImpMestSO search = new HisImpMestSO();
            try
            {
                #region Abstract Base
                if (this.ID.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.ID == this.ID.Value);
                }
                if (this.IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.IDs.Contains(o.ID));
                }
                if (this.IS_ACTIVE.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.IS_ACTIVE == this.IS_ACTIVE.Value);
                }
                if (this.CREATE_TIME_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.CREATE_TIME.Value >= this.CREATE_TIME_FROM.Value);
                }
                if (this.CREATE_TIME_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.CREATE_TIME.Value <= this.CREATE_TIME_TO.Value);
                }
                if (this.MODIFY_TIME_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.MODIFY_TIME.Value >= this.MODIFY_TIME_FROM.Value);
                }
                if (this.MODIFY_TIME_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.MODIFY_TIME.Value <= this.MODIFY_TIME_TO.Value);
                }
                if (!String.IsNullOrEmpty(this.CREATOR))
                {
                    listVHisImpMestManuExpression.Add(o => o.CREATOR == this.CREATOR);
                }
                if (!String.IsNullOrEmpty(this.MODIFIER))
                {
                    listVHisImpMestManuExpression.Add(o => o.MODIFIER == this.MODIFIER);
                }
                if (!String.IsNullOrEmpty(this.GROUP_CODE))
                {
                    listVHisImpMestManuExpression.Add(o => o.GROUP_CODE == this.GROUP_CODE);
                }
                #endregion

                if (this.IMP_MEST_TYPE_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.IMP_MEST_TYPE_ID.Value == o.IMP_MEST_TYPE_ID);
                }
                if (this.IMP_MEST_STT_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.IMP_MEST_STT_ID.Value == o.IMP_MEST_STT_ID);
                }
                if (this.MEDI_STOCK_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.MEDI_STOCK_ID.Value == o.MEDI_STOCK_ID);
                }
                if (this.MEDI_STOCK_PERIOD_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.MEDI_STOCK_PERIOD_ID.HasValue && this.MEDI_STOCK_PERIOD_ID.Value == o.MEDI_STOCK_PERIOD_ID.Value);
                }
                if (this.REQ_ROOM_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.REQ_ROOM_ID.HasValue && this.REQ_ROOM_ID.Value == o.REQ_ROOM_ID.Value);
                }
                if (this.REQ_DEPARTMENT_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.REQ_DEPARTMENT_ID.HasValue && this.REQ_DEPARTMENT_ID.Value == o.REQ_DEPARTMENT_ID.Value);
                }
                if (this.CHMS_EXP_MEST_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.CHMS_EXP_MEST_ID.HasValue && this.CHMS_EXP_MEST_ID.Value == o.CHMS_EXP_MEST_ID.Value);
                }
                if (this.AGGR_IMP_MEST_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.AGGR_IMP_MEST_ID.HasValue && this.AGGR_IMP_MEST_ID.Value == o.AGGR_IMP_MEST_ID.Value);
                }
                if (this.MOBA_EXP_MEST_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.MOBA_EXP_MEST_ID.HasValue && this.MOBA_EXP_MEST_ID.Value == o.MOBA_EXP_MEST_ID.Value);
                }
                if (this.TDL_TREATMENT_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.TDL_TREATMENT_ID.HasValue && this.TDL_TREATMENT_ID.Value == o.TDL_TREATMENT_ID.Value);
                }
                if (this.SUPPLIER_ID != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.SUPPLIER_ID.HasValue && this.SUPPLIER_ID.Value == o.SUPPLIER_ID.Value);
                }

                if (this.IMP_MEST_TYPE_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.IMP_MEST_TYPE_IDs.Contains(o.IMP_MEST_TYPE_ID));
                }
                if (this.IMP_MEST_STT_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.IMP_MEST_STT_IDs.Contains(o.IMP_MEST_STT_ID));
                }
                if (this.MEDI_STOCK_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => this.MEDI_STOCK_IDs.Contains(o.MEDI_STOCK_ID));
                }
                if (this.MEDI_STOCK_PERIOD_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.MEDI_STOCK_PERIOD_ID.HasValue && this.MEDI_STOCK_PERIOD_IDs.Contains(o.MEDI_STOCK_PERIOD_ID.Value));
                }
                if (this.REQ_ROOM_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.REQ_ROOM_ID.HasValue && this.REQ_ROOM_IDs.Contains(o.REQ_ROOM_ID.Value));
                }
                if (this.REQ_DEPARTMENT_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.REQ_DEPARTMENT_ID.HasValue && this.REQ_DEPARTMENT_IDs.Contains(o.REQ_DEPARTMENT_ID.Value));
                }
                if (this.CHMS_EXP_MEST_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.CHMS_EXP_MEST_ID.HasValue && this.CHMS_EXP_MEST_IDs.Contains(o.CHMS_EXP_MEST_ID.Value));
                }
                if (this.AGGR_IMP_MEST_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.AGGR_IMP_MEST_ID.HasValue && this.AGGR_IMP_MEST_IDs.Contains(o.AGGR_IMP_MEST_ID.Value));
                }
                if (this.MOBA_EXP_MEST_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.MOBA_EXP_MEST_ID.HasValue && this.MOBA_EXP_MEST_IDs.Contains(o.MOBA_EXP_MEST_ID.Value));
                }
                if (this.TDL_TREATMENT_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.TDL_TREATMENT_ID.HasValue && this.TDL_TREATMENT_IDs.Contains(o.TDL_TREATMENT_ID.Value));
                }
                if (this.SUPPLIER_IDs != null)
                {
                    listVHisImpMestManuExpression.Add(o => o.SUPPLIER_ID.HasValue && this.SUPPLIER_IDs.Contains(o.SUPPLIER_ID.Value));
                }

                if (this.IMP_TIME_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.IMP_TIME.Value >= this.IMP_TIME_FROM.Value);
                }
                if (this.IMP_DATE_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.IMP_DATE.Value >= this.IMP_DATE_FROM.Value);
                }
                if (this.APPROVAL_TIME_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.APPROVAL_TIME.Value >= this.APPROVAL_TIME_FROM.Value);
                }
                if (this.CREATE_DATE_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.CREATE_DATE >= this.CREATE_DATE_FROM.Value);
                }
                if (this.DOCUMENT_DATE_FROM.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.DOCUMENT_DATE.Value >= this.DOCUMENT_DATE_FROM.Value);
                }

                if (this.IMP_TIME_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.IMP_TIME.Value <= this.IMP_TIME_TO.Value);
                }
                if (this.IMP_DATE_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.IMP_DATE.Value <= this.IMP_DATE_TO.Value);
                }
                if (this.APPROVAL_TIME_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.APPROVAL_TIME.Value <= this.APPROVAL_TIME_TO.Value);
                }
                if (this.CREATE_DATE_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.CREATE_DATE <= this.CREATE_DATE_TO.Value);
                }
                if (this.DOCUMENT_DATE_TO.HasValue)
                {
                    listVHisImpMestManuExpression.Add(o => o.DOCUMENT_DATE.Value <= this.DOCUMENT_DATE_TO.Value);
                }

                if (this.HAS_MEDI_STOCK_PERIOD.HasValue && this.HAS_MEDI_STOCK_PERIOD.Value)
                {
                    listVHisImpMestManuExpression.Add(o => o.MEDI_STOCK_PERIOD_ID.HasValue);
                }
                if (this.HAS_MEDI_STOCK_PERIOD.HasValue && !this.HAS_MEDI_STOCK_PERIOD.Value)
                {
                    listVHisImpMestManuExpression.Add(o => !o.MEDI_STOCK_PERIOD_ID.HasValue);
                }
                if (this.HAS_DOCUMENT_NUMBER.HasValue && this.HAS_DOCUMENT_NUMBER.Value)
                {
                    listVHisImpMestManuExpression.Add(o => o.DOCUMENT_NUMBER != null);
                }
                if (this.HAS_DOCUMENT_NUMBER.HasValue && !this.HAS_DOCUMENT_NUMBER.Value)
                {
                    listVHisImpMestManuExpression.Add(o => o.DOCUMENT_NUMBER == null);
                }
                if (this.HAS_AGGR.HasValue && this.HAS_AGGR.Value)
                {
                    listVHisImpMestManuExpression.Add(o => o.AGGR_IMP_MEST_ID.HasValue);
                }
                if (this.HAS_AGGR.HasValue && !this.HAS_AGGR.Value)
                {
                    listVHisImpMestManuExpression.Add(o => !o.AGGR_IMP_MEST_ID.HasValue);
                }
                if (!String.IsNullOrEmpty(this.IMP_MEST_CODE__EXACT))
                {
                    listVHisImpMestManuExpression.Add(o => o.IMP_MEST_CODE == this.IMP_MEST_CODE__EXACT);
                }
                if (!String.IsNullOrEmpty(this.DOCUMENT_NUMBER__EXACT))
                {
                    listVHisImpMestManuExpression.Add(o => o.DOCUMENT_NUMBER != null && o.DOCUMENT_NUMBER == this.DOCUMENT_NUMBER__EXACT);
                }

                if (this.DATA_DOMAIN_FILTER)
                {
                    string loginName = Inventec.Token.ResourceSystem.ResourceTokenManager.GetLoginName();
                    if (!this.WORKING_ROOM_ID.HasValue)
                    {
                        listVHisImpMestManuExpression.Add(o => loginName.Equals(o.CREATOR));
                    }
                    else
                    {
                        List<WorkPlaceSDO> workPlaces = TokenManager.GetWorkPlaceList();
                        WorkPlaceSDO workPlace = workPlaces != null ? workPlaces.Where(o => o.RoomId == this.WORKING_ROOM_ID).FirstOrDefault() : null;
                        if (workPlace != null)
                        {
                            listVHisImpMestManuExpression.Add(o => o.MEDI_STOCK_ID == workPlace.MediStockId || o.REQ_ROOM_ID == workPlace.RoomId || loginName.Equals(o.CREATOR) || o.REQ_DEPARTMENT_ID == workPlace.DepartmentId);
                        }
                        else
                        {
                            listVHisImpMestManuExpression.Add(o => loginName.Equals(o.CREATOR));
                        }
                    }
                }

                if (!String.IsNullOrEmpty(this.KEY_WORD))
                {
                    this.KEY_WORD = this.KEY_WORD.ToLower().Trim();
                    listVHisImpMestManuExpression.Add(o => o.APPROVAL_LOGINNAME.ToLower().Contains(this.KEY_WORD)
                        || o.APPROVAL_USERNAME.ToLower().Contains(this.KEY_WORD)
                        || o.CREATOR.ToLower().Contains(this.KEY_WORD)
                        || o.DELIVERER.ToLower().Contains(this.KEY_WORD)
                        || o.DESCRIPTION.ToLower().Contains(this.KEY_WORD)
                        || o.DOCUMENT_NUMBER.ToLower().Contains(this.KEY_WORD)
                        || o.IMP_LOGINNAME.ToLower().Contains(this.KEY_WORD)
                        || o.IMP_MEST_CODE.ToLower().Contains(this.KEY_WORD)
                        || o.IMP_MEST_STT_CODE.ToLower().Contains(this.KEY_WORD)
                        || o.IMP_MEST_STT_NAME.ToLower().Contains(this.KEY_WORD)
                        || o.IMP_USERNAME.ToLower().Contains(this.KEY_WORD)
                        || o.MEDI_STOCK_CODE.ToLower().Contains(this.KEY_WORD)
                        || o.MEDI_STOCK_NAME.ToLower().Contains(this.KEY_WORD)
                        || o.MODIFIER.ToLower().Contains(this.KEY_WORD)
                        || o.REQ_LOGINNAME.ToLower().Contains(this.KEY_WORD)
                        || o.REQ_USERNAME.ToLower().Contains(this.KEY_WORD));
                }
                

                search.listVHisImpMestManuExpression.AddRange(listVHisImpMestManuExpression);
                search.OrderField = ORDER_FIELD;
                search.OrderDirection = ORDER_DIRECTION;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                search.listVHisImpMestManuExpression.Clear();
                search.listVHisImpMestManuExpression.Add(o => o.ID == NEGATIVE_ID);
            }
            return search;
        }
    }
}
