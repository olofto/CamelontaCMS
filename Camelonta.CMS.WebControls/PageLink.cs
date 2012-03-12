using System;
using System.Web.UI;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{
    public class PageLink : BaseControl
    {
        private string _TextProperty;
        public string TextProperty
        {
            get { return _TextProperty; }
            set { _TextProperty = value; }
        }

        private string _LinkProperty;
        public string LinkProperty
        {
            get { return _LinkProperty; }
            set { _LinkProperty = value; }
        }
        private string _TextPublicationProperty;
        public string TextPublicationProperty
        {
            get { return _TextPublicationProperty; }
            set { _TextPublicationProperty = value; }
        }

        private string _LinkPublicationProperty;
        public string LinkPublicationProperty
        {
            get { return _LinkPublicationProperty; }
            set { _LinkPublicationProperty = value; }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        private string _Link;
        public string Link
        {
            get { return _Link; }
            set { _Link = value; }
        }

        private string _CssClass;
        public string CssClass
        {
            get { return _CssClass; }
            set { _CssClass = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!String.IsNullOrEmpty(_LinkPublicationProperty))
                _Link = CMS.Context.GetPublicationProperty(_LinkPublicationProperty);
            if (!String.IsNullOrEmpty(_TextPublicationProperty))
                _Text = CMS.Context.GetPublicationProperty(_TextPublicationProperty);
            if (!String.IsNullOrEmpty(_LinkProperty))
                _Link = CMS.GetPageProperty(_LinkProperty);
            if (!String.IsNullOrEmpty(_TextProperty))
                _Text = CMS.GetPageProperty(_TextProperty);
            if (!String.IsNullOrEmpty(_Link) && !String.IsNullOrEmpty(_Text))
            {
                string cssClass = "";
                if (!String.IsNullOrEmpty(_CssClass))
                    cssClass = String.Format(" class=\"{0}\"", _CssClass);

                writer.Write(String.Format("<a href=\"{0}\"{1}>{2}</a>", _Link, cssClass, _Text));
            }
            base.Render(writer);
        }
    }
}
