using System;
using System.Web.UI.WebControls;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{
    public class CommentsListing  : Repeater
    {
        private Int32 _Limit = -1;
        public Int32 Limit
        {
            get { return _Limit; }
            set { _Limit = value; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            this.EnableViewState = false;
            BasePage basePage = (BasePage)this.Page;
            this.DataSource = basePage.CMS.Utility.Comments.Get(basePage.CMS.Context.PageData.PageGuid, _Limit);

            base.OnDataBinding(e);
        }
    }
}
