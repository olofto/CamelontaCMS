using System;
using System.Web;
using System.Web.UI.WebControls;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.FrameWork.Extensions;
using System.Web.Security;
using System.Xml;

namespace Camelonta.CMS.WebControls
{
    public class Property : Camelonta.CMS.Web.BaseControls.BaseControl
    {
        private string _PropertyName;
        public string PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        private bool _AllowOnPageEditing;
        public bool AllowOnPageEditing
        {
            get { return _AllowOnPageEditing; }
            set { _AllowOnPageEditing = value; }
        }

        private bool _Recursive = false;
        public bool Recursive
        {
            get { return _Recursive; }
            set { _Recursive = value; }
        }

        private bool _ContextSensitive = true;
        public bool ContextSensitive
        {
            get { return _ContextSensitive; }
            set { _ContextSensitive = value; }
        }

        private string _Default;
        public string Default
        {
            get { return _Default; }
            set { _Default = value; }
        }

        private PageData _pageData;

        public override void DataBind()
        {
            if (_ContextSensitive)
            {
                // Override content with current context-PageData from repeater
                System.Web.UI.Control controlItem = Globals.GetFirstParentControlOfType(this, "System.Web.UI.WebControls.RepeaterItem");
                if (controlItem != null)
                {
                    RepeaterItem repeaterItem = (RepeaterItem)controlItem;
                    string type = repeaterItem.DataItem.GetType().ToString();
                    if (type == "Camelonta.CMS.FrameWork.PageData")
                        _pageData = (PageData)repeaterItem.DataItem;
                    else if (type == "System.Data.DataRowView")
                    {
                        Foreach forEachControl = (Foreach)controlItem.Parent;
                        Int32 currentRowIndex = repeaterItem.ItemIndex + 1;
                        string elementName = forEachControl.Property;

                        // Simple elements
                        if (_PropertyName == "this")
                        {
                            _PropertyName = elementName;

                            // Set new PropertyName
                            if (elementName.Contains("|"))
                            {
                                // PropertyName contains serveral properties, split them up and fix the xpath
                                _PropertyName = "(";
                                foreach (string split in elementName.Split("|".ToCharArray()))
                                    _PropertyName += split + "|";
                                _PropertyName = _PropertyName.Substring(0, _PropertyName.Length - 1);
                                _PropertyName += ")[" + currentRowIndex + "]";

                            }
                            else
                            {
                                // PropertyName contains one property
                                _PropertyName = elementName + "[" + currentRowIndex + "]";
                            }
                        }
                        // Advanced (Panel) elements
                        else
                        {
                            // Set new PropertyName
                            _PropertyName = "(" + elementName + ")[" + currentRowIndex + "]/" + _PropertyName;
                        }
                    }
                }
            }

            if (_pageData == null)
                _pageData = CMS.Context.PageData;

            base.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (_AllowOnPageEditing && Users.UserCanEditOnPage(CMS.Context.Publication, CMS.Context.PageData))
            {
                // Only do this once per request
                if (HttpContext.Current.Items["onPageEditingAdded"] == null)
                {
                    string onPageScript = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Camelonta.CMS.WebControls.OnPage.js");
                    string prefix = Globals.SystemPath;
                    if (Globals.IsLiveMode())
                        prefix = Globals.SystemUrl;

                    string initjQuery = prefix + "js/initjQuery.js";

                    this.Page.ClientScript.RegisterClientScriptInclude("initjQuery", initjQuery);
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "initjQueryCall", "<script>initJQuery('" + prefix + "');</script>");

                    this.Page.ClientScript.RegisterClientScriptInclude("onPageScript", onPageScript);

                    // Add to requestcache
                    HttpContext.Current.Items["onPageEditingAdded"] = 1;
                }
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            string content = CMS.GetPageProperty(_pageData, _PropertyName, _Recursive, _Default);

            // OnPage-popup
            if (_AllowOnPageEditing && Users.UserCanEditOnPage(CMS.Context.Publication, CMS.Context.PageData) && ContentIsFromThisPage(content))
            {
                string propertyName = _PropertyName;
                if (propertyName.Contains("/"))
                    propertyName = propertyName.Substring(0, propertyName.LastIndexOf("/"));
                string propertyPrettyName = GetPropertyPrettyName(_PropertyName);
                string editElementUrl = "EditElement.aspx?";
                if (Globals.IsLiveMode())
                    editElementUrl = Globals.CombineUrls(Globals.SystemUrl, editElementUrl) + "PublishOnSave=1&";
                else
                    editElementUrl = Globals.CombineUrls(Globals.SystemPath, editElementUrl);
                string onClick = String.Format("showOnpage(this, '{0}', '{1}', '{2}', {3}, {4}, {5})",
                    editElementUrl, propertyName, propertyPrettyName, CMS.Context.NodeID, CMS.Context.PublicationID, _pageData.VersionID);
                if (String.IsNullOrEmpty(content))
                    content = "<p>&nbsp;</p>";
                writer.Write(String.Format("<div onMouseOver=\"onPageElementMouseOver(this)\" onMouseOut=\"onPageElementMouseOut(this)\" onDblClick=\"{0}\" id=\"{1}\">{2}</div>", onClick, Globals.CreateRandomString(4), content));
            }
            else
            {
                // Print content
                writer.Write(content);
            }
        }

        /// <summary>
        /// Check if the content of the property is acctually from this current PageData an not inherited or a default value
        /// </summary>
        private bool ContentIsFromThisPage(string content)
        {
            if (!_Recursive)
                return true;
            if (CMS.GetPageProperty(_pageData, _PropertyName, false, null) != content)
                return false;
            return true;
        }

        /// <summary>
        /// Get the pretty labelname from the pagetype
        /// </summary>
        private string GetPropertyPrettyName(string propertyName)
        {
            XmlNode property = CMS.Context.PageData.XmlContentNode().DocumentElement.SelectSingleNode(_PropertyName);
            if (property != null)
            {
                propertyName = property.Name;
                PageType pt = CMS.Context.Template.GetPageType(CMS.Context.PageData.PageType.Name);
                pt.ParseElements();
                if (pt.Elements.Contains(propertyName))
                    propertyName = ((Element)pt.Elements[propertyName]).Label;
            }
            return propertyName;
        }
    }
}
