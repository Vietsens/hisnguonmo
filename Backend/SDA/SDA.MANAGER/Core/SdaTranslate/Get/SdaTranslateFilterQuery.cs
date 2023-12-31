using SDA.DAO.StagingObject;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using SDA.MANAGER.Base;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;

namespace SDA.MANAGER.Core.SdaTranslate.Get
{
    public class SdaTranslateFilterQuery : SdaTranslateFilter
    {
        public SdaTranslateFilterQuery()
            : base()
        {

        }

        internal List<System.Linq.Expressions.Expression<Func<SDA_TRANSLATE, bool>>> listExpression = new List<System.Linq.Expressions.Expression<Func<SDA_TRANSLATE, bool>>>();

        internal Inventec.Backend.MANAGER.OrderProcessorBase OrderProcess = new Inventec.Backend.MANAGER.OrderProcessorBase();

        internal SdaTranslateSO Query()
        {
            SdaTranslateSO search = new SdaTranslateSO();
            try
            {
                #region Abstract Base
                if (this.ID.HasValue)
                {
                    listExpression.Add(o => o.ID == this.ID.Value);
                }
                if (this.IS_ACTIVE.HasValue)
                {
                    listExpression.Add(o => o.IS_ACTIVE == this.IS_ACTIVE.Value);
                }
                if (this.CREATE_TIME_FROM.HasValue)
                {
                    listExpression.Add(o => o.CREATE_TIME.Value >= this.CREATE_TIME_FROM.Value);
                }
                if (this.CREATE_TIME_FROM__GREATER.HasValue)
                {
                    listExpression.Add(o => o.CREATE_TIME.Value > this.CREATE_TIME_FROM__GREATER.Value);
                }
                if (this.CREATE_TIME_TO.HasValue)
                {
                    listExpression.Add(o => o.CREATE_TIME.Value <= this.CREATE_TIME_TO.Value);
                }
                if (this.CREATE_TIME_TO__LESS.HasValue)
                {
                    listExpression.Add(o => o.CREATE_TIME.Value < this.CREATE_TIME_TO__LESS.Value);
                }
                if (this.MODIFY_TIME_FROM.HasValue)
                {
                    listExpression.Add(o => o.MODIFY_TIME.Value >= this.MODIFY_TIME_FROM.Value);
                }
                if (this.MODIFY_TIME_FROM__GREATER.HasValue)
                {
                    listExpression.Add(o => o.MODIFY_TIME.Value > this.MODIFY_TIME_FROM__GREATER.Value);
                }
                if (this.MODIFY_TIME_TO.HasValue)
                {
                    listExpression.Add(o => o.MODIFY_TIME.Value <= this.MODIFY_TIME_TO.Value);
                }
                if (this.MODIFY_TIME_TO__LESS.HasValue)
                {
                    listExpression.Add(o => o.MODIFY_TIME.Value < this.MODIFY_TIME_TO__LESS.Value);
                }
                if (!String.IsNullOrEmpty(this.CREATOR))
                {
                    listExpression.Add(o => o.CREATOR == this.CREATOR);
                }
                if (!String.IsNullOrEmpty(this.MODIFIER))
                {
                    listExpression.Add(o => o.MODIFIER == this.MODIFIER);
                }
                if (!String.IsNullOrEmpty(this.GROUP_CODE))
                {
                    listExpression.Add(o => o.GROUP_CODE == this.GROUP_CODE);
                }
                if (this.IDs != null && this.IDs.Count > 0)
                {
                    listExpression.Add(o => this.IDs.Contains(o.ID));
                }
                #endregion

                if (this.LANGUAGE_ID.HasValue)
                {
                    listExpression.Add(o => o.LANGUAGE_ID == this.LANGUAGE_ID.Value);
                }
                if (!String.IsNullOrEmpty(this.SCHEMA))
                {
                    listExpression.Add(o => o.SCHEMA == this.SCHEMA);
                }
                if (!String.IsNullOrEmpty(this.TABLE_NAME))
                {
                    listExpression.Add(o => o.TABLE_NAME == this.TABLE_NAME);
                }

                if (!String.IsNullOrEmpty(this.KEY_WORD))
                {
                    this.KEY_WORD = this.KEY_WORD.ToLower().Trim();
                    listExpression.Add(o =>
                        o.CREATOR.ToLower().Contains(this.KEY_WORD) ||
                        o.MODIFIER.ToLower().Contains(this.KEY_WORD) ||
                        o.FIND_COLUMN_NAME_ONE.ToLower().Contains(this.KEY_WORD) ||
                        o.FIND_COLUMN_NAME_TWO.ToLower().Contains(this.KEY_WORD) ||
                        o.FIND_DATA_CODE_ONE.ToLower().Contains(this.KEY_WORD) ||
                        o.FIND_DATA_CODE_TWO.ToLower().Contains(this.KEY_WORD) ||
                        o.SCHEMA.ToLower().Contains(this.KEY_WORD) ||
                        o.TABLE_NAME.ToLower().Contains(this.KEY_WORD) ||
                        o.UPDATE_COLUMN_NAME.ToLower().Contains(this.KEY_WORD) ||
                        o.UPDATE_DATA_VALUE.ToLower().Contains(this.KEY_WORD)
                        );
                }

                search.listSdaTranslateExpression.AddRange(listExpression);
                search.OrderField = OrderProcess.GetOrderField<SDA_TRANSLATE>(ORDER_FIELD);
                search.OrderDirection = OrderProcess.GetOrderDirection(ORDER_DIRECTION);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                search.listSdaTranslateExpression.Clear();
                search.listSdaTranslateExpression.Add(o => o.ID == NEGATIVE_ID);
            }
            return search;
        }
    }
}
