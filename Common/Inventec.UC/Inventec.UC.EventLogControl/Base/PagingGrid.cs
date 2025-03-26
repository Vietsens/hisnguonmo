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
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Base
{
    internal class PagingGrid
    {
        private ComboBoxEdit _comboPageSize;
        private LabelControl _lblDisplayPageNo;
        private LabelControl _labelStartRec;
        private TextEdit _txtCurrentPage;
        private LabelControl _lblTotalPage;
        private SimpleButton _btnLast;
        private SimpleButton _btnNext;
        private SimpleButton _btnPrevious;
        private SimpleButton _btnFirst;
        private int _recNo;
        private int _currentPage;
        private int _pageSize;
        private int _maxRec;
        private int _dataCount;
        private int _pageCount;

        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; }
        }

        public int MaxRec
        {
            get { return _maxRec; }
            set { _maxRec = value; }
        }

        public int DataCount
        {
            get { return _dataCount; }
            set { _dataCount = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        public int RecNo
        {
            get { return _recNo; }
            set { _recNo = value; }
        }

        public LabelControl LabelDisplayPageNo
        {
            get { return _lblDisplayPageNo; }
            set { _lblDisplayPageNo = value; }
        }

        public ComboBoxEdit ComboPageSize
        {
            get { return _comboPageSize; }
            set { _comboPageSize = value; }
        }

        public LabelControl LabelTotalPage
        {
            get { return _lblTotalPage; }
            set { _lblTotalPage = value; }
        }
        public TextEdit TxtCurrentPage
        {
            get { return _txtCurrentPage; }
            set { _txtCurrentPage = value; }
        }

        public SimpleButton BtnLast
        {
            get { return _btnLast; }
            set { _btnLast = value; }
        }

        public SimpleButton BtnNext
        {
            get { return _btnNext; }
            set { _btnNext = value; }
        }

        public SimpleButton BtnPrevious
        {
            get { return _btnPrevious; }
            set { _btnPrevious = value; }
        }

        public SimpleButton BtnFirst
        {
            get { return _btnFirst; }
            set { _btnFirst = value; }
        }

        public void LoadPage()
        {
            int i = 0;
            int startRec = RecNo;
            int endRec = 0;
            endRec = PageSize * (CurrentPage - 1);
            //if (CurrentPage == PageCount)
            //{
            //    endRec = MaxRec;
            //}
            //else
            //{
            //    endRec = PageSize * (CurrentPage - 1);
            //}       
            for (i = startRec; i <= endRec - 1; i++)
            {
                if (i < MaxRec)
                {
                    RecNo = RecNo + 1;
                }
            }
            DisplayPageInfo();
        }

        private void DisplayPageInfo()
        {
            int endRec = 0;
            if (CurrentPage == PageCount)
            {
                endRec = MaxRec;
            }
            else
            {
                endRec = PageSize * (CurrentPage);
            }
            LabelDisplayPageNo.Text = (RecNo + 1).ToString() + " - " + endRec.ToString() + "/" + MaxRec.ToString();
            TxtCurrentPage.Text = CurrentPage.ToString();
            LabelTotalPage.Text = "/" + PageCount.ToString();
            if (this.DataCount == 0)
            {
                LabelDisplayPageNo.Text = "0 - 0/0";
                TxtCurrentPage.Text = "0";
                LabelTotalPage.Text = "/0";
            }
        }

        private void SetEnableButton(bool fBtnLast, bool fBtnNext, bool fBtnFirst, bool fBtnPrevious)
        {
            BtnLast.Enabled = fBtnLast;
            BtnNext.Enabled = fBtnNext;
            BtnFirst.Enabled = fBtnFirst;
            BtnPrevious.Enabled = fBtnPrevious;
        }

        public void LastPage()
        {
            CurrentPage = PageCount;
            RecNo = PageSize * (CurrentPage - 1);
            LoadPage();
            SetEnableButton(false, false, true, true);
        }

        public void FirstPage()
        {
            if (CurrentPage == 1)
            {
                return;
            }
            CurrentPage = 1;
            RecNo = 0;
            LoadPage();
            SetEnableButton(true, true, false, false);
        }

        public void PreviousPage()
        {
            if (CurrentPage == PageCount + 1)
            {
                RecNo = PageSize * (CurrentPage - 2);
                return;
            }
            CurrentPage = CurrentPage - 1;
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            else
            {
                if (CurrentPage == 1)
                {
                    SetEnableButton(true, true, false, false);
                }
                else
                {
                    SetEnableButton(true, true, true, true);
                }
                RecNo = PageSize * (CurrentPage - 1);
            }
            LoadPage();
        }

        public void NextPage()
        {
            if (CurrentPage == PageCount)
            {
                return;
            }
            CurrentPage = CurrentPage + 1;
            if (CurrentPage == PageCount)
            {
                SetEnableButton(false, false, true, true);
            }
            else
            {
                SetEnableButton(true, true, true, true);
            }
            if (CurrentPage > PageCount)
            {
                CurrentPage = PageCount;
                if (RecNo == MaxRec)
                {
                }
            }
            LoadPage();
        }

        public void Innitial(LabelControl lblDisplayPageNo, ComboBoxEdit cboPageSize, TextEdit txtCurrentPage, LabelControl lblTotalPage, SimpleButton btnLast, SimpleButton btnPrevious, SimpleButton btnFirst, SimpleButton btnNext, int maxRec, int dataCount)
        {
            this.LabelDisplayPageNo = lblDisplayPageNo;
            this.ComboPageSize = cboPageSize;
            this.TxtCurrentPage = txtCurrentPage;
            this.LabelTotalPage = lblTotalPage;
            this.BtnLast = btnLast;
            this.BtnNext = btnNext;
            this.BtnFirst = btnFirst;
            this.BtnPrevious = btnPrevious;
            this.PageSize = Convert.ToInt32(this.ComboPageSize.Text);
            this.MaxRec = maxRec;
            this.PageCount = this.MaxRec / this.PageSize;
            if ((this.MaxRec % this.PageSize) > 0)
            {
                this.PageCount = this.PageCount + 1;
            }
            this.CurrentPage = 1;
            this.RecNo = 0;
            this.DataCount = dataCount;
            LoadPage();
            this.BtnLast.Enabled = true;
            this.BtnNext.Enabled = true;
            this.BtnFirst.Enabled = false;
            this.BtnPrevious.Enabled = false;
            if (this.DataCount == 0 || this.PageCount == 1)
            {
                this.BtnLast.Enabled = false;
                this.BtnNext.Enabled = false;
                this.BtnFirst.Enabled = false;
                this.BtnPrevious.Enabled = false;
            }
        }
    }
}
