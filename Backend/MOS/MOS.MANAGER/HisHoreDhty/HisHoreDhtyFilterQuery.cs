using Inventec.Common.Logging;
using MOS.DAO.StagingObject;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.MANAGER.Base;
using System;
using System.Collections.Generic;

namespace MOS.MANAGER.HisHoreDhty
{
    public class HisHoreDhtyFilterQuery : HisHoreDhtyFilter
    {
        public HisHoreDhtyFilterQuery()
            : base()
        {

        }

        internal List<System.Linq.Expressions.Expression<Func<HIS_HORE_DHTY, bool>>> listHisHoreDhtyExpression = new List<System.Linq.Expressions.Expression<Func<HIS_HORE_DHTY, bool>>>();

        

        internal HisHoreDhtySO Query()
        {
            HisHoreDhtySO search = new HisHoreDhtySO();
            try
            {
                #region Abstract Base
                if (this.ID.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.ID == this.ID.Value);
                }
				if (this.IDs != null)
                {
                    listHisHoreDhtyExpression.Add(o => this.IDs.Contains(o.ID));
                }
                if (this.IS_ACTIVE.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.IS_ACTIVE == this.IS_ACTIVE.Value);
                }
                if (this.CREATE_TIME_FROM.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.CREATE_TIME.Value >= this.CREATE_TIME_FROM.Value);
                }
                if (this.CREATE_TIME_TO.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.CREATE_TIME.Value <= this.CREATE_TIME_TO.Value);
                }
                if (this.MODIFY_TIME_FROM.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.MODIFY_TIME.Value >= this.MODIFY_TIME_FROM.Value);
                }
                if (this.MODIFY_TIME_TO.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.MODIFY_TIME.Value <= this.MODIFY_TIME_TO.Value);
                }
                if (!String.IsNullOrEmpty(this.CREATOR))
                {
                    listHisHoreDhtyExpression.Add(o => o.CREATOR == this.CREATOR);
                }
                if (!String.IsNullOrEmpty(this.MODIFIER))
                {
                    listHisHoreDhtyExpression.Add(o => o.MODIFIER == this.MODIFIER);
                }
                if (!String.IsNullOrEmpty(this.GROUP_CODE))
                {
                    listHisHoreDhtyExpression.Add(o => o.GROUP_CODE == this.GROUP_CODE);
                }
                if (this.IDs != null)
                {
                    listHisHoreDhtyExpression.Add(o => this.IDs.Contains(o.ID));
                }
                #endregion

                if (this.DOC_HOLD_TYPE_ID.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.DOC_HOLD_TYPE_ID == this.DOC_HOLD_TYPE_ID.Value);
                }
                if (this.DOC_HOLD_TYPE_IDs != null)
                {
                    listHisHoreDhtyExpression.Add(o => this.DOC_HOLD_TYPE_IDs.Contains(o.DOC_HOLD_TYPE_ID));
                }
                if (this.HOLD_RETURN_ID.HasValue)
                {
                    listHisHoreDhtyExpression.Add(o => o.HOLD_RETURN_ID == this.HOLD_RETURN_ID.Value);
                }
                if (this.HOLD_RETURN_IDs != null)
                {
                    listHisHoreDhtyExpression.Add(o => this.HOLD_RETURN_IDs.Contains(o.HOLD_RETURN_ID));
                }

                search.listHisHoreDhtyExpression.AddRange(listHisHoreDhtyExpression);
                search.OrderField = ORDER_FIELD;
                search.OrderDirection = ORDER_DIRECTION;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                search.listHisHoreDhtyExpression.Clear();
                search.listHisHoreDhtyExpression.Add(o => o.ID == NEGATIVE_ID);
            }
            return search;
        }
    }
}
