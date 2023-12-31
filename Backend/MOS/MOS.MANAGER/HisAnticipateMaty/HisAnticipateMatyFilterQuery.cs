using Inventec.Common.Logging;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisAnticipateMaty
{
    public class HisAnticipateMatyFilterQuery : HisAnticipateMatyFilter
    {
        public HisAnticipateMatyFilterQuery()
            : base()
        {

        }

        internal List<System.Linq.Expressions.Expression<Func<HIS_ANTICIPATE_MATY, bool>>> listHisAnticipateMatyExpression = new List<System.Linq.Expressions.Expression<Func<HIS_ANTICIPATE_MATY, bool>>>();



        internal HisAnticipateMatySO Query()
        {
            HisAnticipateMatySO search = new HisAnticipateMatySO();
            try
            {
                #region Abstract Base
                if (this.ID.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.ID == this.ID.Value);
                }
                if (this.IDs != null)
                {
                    listHisAnticipateMatyExpression.Add(o => this.IDs.Contains(o.ID));
                }
                if (this.IS_ACTIVE.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.IS_ACTIVE == this.IS_ACTIVE.Value);
                }
                if (this.CREATE_TIME_FROM.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.CREATE_TIME.Value >= this.CREATE_TIME_FROM.Value);
                }
                if (this.CREATE_TIME_TO.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.CREATE_TIME.Value <= this.CREATE_TIME_TO.Value);
                }
                if (this.MODIFY_TIME_FROM.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.MODIFY_TIME.Value >= this.MODIFY_TIME_FROM.Value);
                }
                if (this.MODIFY_TIME_TO.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.MODIFY_TIME.Value <= this.MODIFY_TIME_TO.Value);
                }
                if (!String.IsNullOrEmpty(this.CREATOR))
                {
                    listHisAnticipateMatyExpression.Add(o => o.CREATOR == this.CREATOR);
                }
                if (!String.IsNullOrEmpty(this.MODIFIER))
                {
                    listHisAnticipateMatyExpression.Add(o => o.MODIFIER == this.MODIFIER);
                }
                if (!String.IsNullOrEmpty(this.GROUP_CODE))
                {
                    listHisAnticipateMatyExpression.Add(o => o.GROUP_CODE == this.GROUP_CODE);
                }
                #endregion

                if (this.ANTICIPATE_ID.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.ANTICIPATE_ID == this.ANTICIPATE_ID.Value);
                }
                if (this.MATERIAL_TYPE_ID.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.MATERIAL_TYPE_ID == this.MATERIAL_TYPE_ID.Value);
                }
                if (this.ANTICIPATE_IDs != null)
                {
                    listHisAnticipateMatyExpression.Add(o => this.ANTICIPATE_IDs.Contains(o.ANTICIPATE_ID));
                }
                if (this.MATERIAL_TYPE_IDs != null)
                {
                    listHisAnticipateMatyExpression.Add(o => this.MATERIAL_TYPE_IDs.Contains(o.MATERIAL_TYPE_ID));
                }
                if (this.SUPPLIER_ID.HasValue)
                {
                    listHisAnticipateMatyExpression.Add(o => o.SUPPLIER_ID.HasValue && o.SUPPLIER_ID.Value == this.SUPPLIER_ID.Value);
                }
                if (this.SUPPLIER_IDs != null)
                {
                    listHisAnticipateMatyExpression.Add(o => o.SUPPLIER_ID.HasValue && this.SUPPLIER_IDs.Contains(o.SUPPLIER_ID.Value));
                }

                search.listHisAnticipateMatyExpression.AddRange(listHisAnticipateMatyExpression);
                search.OrderField = ORDER_FIELD;
                search.OrderDirection = ORDER_DIRECTION;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                search.listHisAnticipateMatyExpression.Clear();
                search.listHisAnticipateMatyExpression.Add(o => o.ID == NEGATIVE_ID);
            }
            return search;
        }
    }
}
