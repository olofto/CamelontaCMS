using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{
    /// <summary>
    /// Shows or hides content depending on the values of the properties.
    /// </summary>
    public class If : BaseControl
    {
        private string _ValueIsNotEmpty;
        public string ValueIsNotEmpty
        {
            get { return _ValueIsNotEmpty; }
            set { _ValueIsNotEmpty = value; }
        }
        private string _ValueIsEmpty;
        public string ValueIsEmpty
        {
            get { return _ValueIsEmpty; }
            set { _ValueIsEmpty = value; }
        }
        private string _PageType;
        public string PageType
        {
            get { return _PageType; }
            set { _PageType = value; }
        }
        private string _NotPageType;
        public string NotPageType
        {
            get { return _NotPageType; }
            set { _NotPageType = value; }
        }
        public string PropertyExists
        {
            get { return _PropertyIsNotEmpty; }
            set { _PropertyIsNotEmpty = value; }
        }
        private string _PropertyIsNotEmpty;
        public string PropertyIsNotEmpty
        {
            get { return _PropertyIsNotEmpty; }
            set { _PropertyIsNotEmpty = value; }
        }
        private string _Browser;
        public string Browser
        {
            get { return _Browser; }
            set { _Browser = value; }
        }

        /// <summary>
        /// For CheckBoxes.
        /// </summary>
        private string _PropertyIsTrue;
        public string PropertyIsTrue
        {
            get { return _PropertyIsTrue; }
            set { _PropertyIsTrue = value; }
        }
        private string _CurrentElementName;
        public string CurrentElementName
        {
            get { return _CurrentElementName; }
            set { _CurrentElementName = value; }
        }

        private bool? _LiveMode;
        public bool? LiveMode
        {
            get { return _LiveMode; }
            set { _LiveMode = value; }
        }
        private bool? _IsTrue;
        public bool? IsTrue
        {
            get { return _IsTrue; }
            set { _IsTrue = value; }
        }
        private bool? _IsCurrentPage;
        public bool? IsCurrentPage
        {
            get { return _IsCurrentPage; }
            set { _IsCurrentPage = value; }
        }
        private bool? _IsActivePage;
        public bool? IsActivePage
        {
            get { return _IsActivePage; }
            set { _IsActivePage = value; }
        }



        private bool _ContextSensitive = true;
        public bool ContextSensitive
        {
            get { return _ContextSensitive; }
            set { _ContextSensitive = value; }
        }

        protected bool _hideControls;
        private PageData _pageData;

        protected override void OnInit(EventArgs e)
        {
            // Current ElementName in a Foreach is this value
            if (_CurrentElementName != null)
            {
                if (this.Parent.GetType().ToString() == "System.Web.UI.WebControls.RepeaterItem")
                {
                    RepeaterItem parentItem = (RepeaterItem)this.Parent;
                    DataRowView row = (DataRowView)parentItem.DataItem;
                    if (row != null)
                        if (_CurrentElementName != row["ElementName"].ToString())
                            _hideControls = true;
                }
            }

            base.OnInit(e);
        }

        public override void DataBind()
        {
            if (_ContextSensitive)
            {
                // Override content with current context-page from repeater
                Control controlItem = Globals.GetFirstParentControlOfType(this, "System.Web.UI.WebControls.RepeaterItem");
                if (controlItem != null)
                {
                    RepeaterItem repeaterItem = (RepeaterItem)controlItem;
                    if (repeaterItem.DataItem.GetType().ToString() == "Camelonta.CMS.FrameWork.PageData")
                        _pageData = (PageData)repeaterItem.DataItem;
                }
            }

            // Set PageData if is has not yet been set
            if (_pageData == null)
                _pageData = CMS.Context.PageData;

            base.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Set PageData if is has not yet been set
            if (_pageData == null)
                _pageData = CMS.Context.PageData;

            Int32 countProperties = 0;

            if (_ValueIsNotEmpty != null)
                countProperties++;
            if (_ValueIsEmpty != null)
                countProperties++;
            if (_PageType != null)
                countProperties++;
            if (_NotPageType != null)
                countProperties++;
            if (_PropertyIsNotEmpty != null)
                countProperties++;
            if (_PropertyIsTrue != null)
                countProperties++;
            if (_CurrentElementName != null)
                countProperties++;
            if (_LiveMode != null)
                countProperties++;
            if (_IsTrue != null)
                countProperties++;
            if (_IsCurrentPage != null)
                countProperties++;
            if (_IsActivePage != null)
                countProperties++;
            if (_Browser != null)
                countProperties++;

            if (countProperties > 1)
                throw new Exception("If-control " + this.ID + " in \"" + this.Parent.ID + "\" cannot have more than one statement");


            // Any value is not empty
            if (_ValueIsNotEmpty != null)
                if (String.IsNullOrEmpty(_ValueIsNotEmpty))
                    _hideControls = true;

            // Any value is empty
            if (_ValueIsEmpty != null)
                if (!String.IsNullOrEmpty(_ValueIsEmpty))
                    _hideControls = true;

            // LiveMode
            if (_LiveMode != null)
            {
                if (_LiveMode == true && !Globals.IsLiveMode())
                    _hideControls = true;

                if (_LiveMode == false && Globals.IsLiveMode())
                    _hideControls = true;
            }

            // Is current PageData
            if (_IsCurrentPage != null)
            {
                _hideControls = (bool)_IsCurrentPage;

                // Is this PageData active?
                if (_pageData.NodeID == CMS.Context.PageData.NodeID)
                    _hideControls = (bool)!_IsCurrentPage;
            }

            // Is active PageData
            if (_IsActivePage != null)
            {
                _hideControls = (bool)_IsActivePage;

                // Is this PageData active?
                if (_pageData.NodeID == CMS.Context.PageData.NodeID)
                {
                    _hideControls = (bool)!_IsActivePage;
                }
                else
                {
                    // Is a parent active?
                    if (_pageData.ParentID > 0)
                    {
                        if (_pageData.ParentID == CMS.Context.PageData.NodeID)
                            _hideControls = (bool)!_IsActivePage;
                        if (Pages.GetPageByNodeID(_pageData.ParentID).ParentID == CMS.Context.PageData.NodeID)
                            _hideControls = (bool)!_IsActivePage;
                    }
                }
            }

            // If value is true
            if (_IsTrue != null)
            {
                _hideControls = (bool)!_IsTrue;
            }

            // Is this PageType
            if (!String.IsNullOrEmpty(_PageType))
            {
                if (_PageType.Contains(","))
                {
                    string[] pagetypes = SplitParameters(_PageType);
                    _hideControls = pagetypes.Where(pt => pt == _pageData.PageType.Name.ToLower()).Count() == 0; // Hide if we don't find pagetype in _PageType string
                }
                else
                {
                    // Old way for backwards compability
                    if (_PageType.StartsWith("!"))
                    {
                        if (_pageData.PageType.Name == _PageType.Substring(1))
                            _hideControls = true;
                    }
                    else
                    {
                        if (_pageData.PageType.Name != _PageType)
                            _hideControls = true;
                    }
                }
            }

            // Is NOT this PageType
            if (!String.IsNullOrEmpty(_NotPageType))
            {
                string[] pagetypes = SplitParameters(_NotPageType);
                _hideControls = pagetypes.Where(pt => pt == _pageData.PageType.Name.ToLower()).Count() > 0; // Hide if we find pagetype in _NotPageType string
            }

            // Property is not null or empty
            if (!String.IsNullOrEmpty(_PropertyIsNotEmpty))
                if (_PropertyIsNotEmpty.StartsWith("!"))
                {
                    if (!String.IsNullOrEmpty(_pageData.GetProperty(_PropertyIsNotEmpty.Substring(1))))
                        _hideControls = true;
                }
                else
                {
                    if (String.IsNullOrEmpty(_pageData.GetProperty(_PropertyIsNotEmpty)))
                        _hideControls = true;
                }

            // Property is "True" (checkboxes)
            if (!String.IsNullOrEmpty(_PropertyIsTrue))
                if (_PropertyIsTrue.StartsWith("!"))
                {
                    if (_pageData.GetProperty(_PropertyIsTrue.Substring(1)) != "True")
                        _hideControls = true;
                }
                else
                {
                    if (_pageData.GetProperty(_PropertyIsTrue) == "True")
                        _hideControls = true;
                }

            // Browser
            if (!String.IsNullOrEmpty(_Browser))
            {
                var showControls = CheckBroswerProperty();
                _hideControls = !showControls;
            }

            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Hide controls
            HideControls();

            base.Render(writer);
        }

        /// <summary>
        /// Method to split parameters + ToLower() + Trim(), parameter will be split at '||', '&&' and ','
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <returns>string[]</returns>
        private string[] SplitParameters(string parameter)
        {
            string[] parameters;

            if (parameter.Contains(","))
                parameters = parameter.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            else if (parameter.Contains("||"))
                parameters = parameter.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            else if (parameter.Contains("&&"))
                parameters = parameter.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
            else
            {
                parameters = new[] { parameter };
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = parameters[i].ToLower().Trim();
            }

            return parameters;
        }


        private void HideControls()
        {
            // Hide controls
            if (_hideControls)
            {
                this.Visible = false;
                try
                {
                    this.Controls.Clear();
                }
                catch { }
            }
        }

        private bool CheckBroswerProperty()
        {
            string[] browsers = SplitParameters(_Browser);
            bool and = false;
            if (_Browser.Contains("&&"))
                and = true;

            bool hide = false;

            string client = CMS.Browser.BrowserName;


            bool showControls = false;
            foreach (string browser in browsers)
            {
                if (browser == "ie") //Check for IE
                    if (client == "ie" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!ie") //Check for NOT IE
                    if (client != "ie" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "ie6") //Check for IE6
                    if (client == "ie" && CMS.Browser.MajorVersion == 6 && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!ie6") //Check for NOT IE6
                    if ((client != "ie" || CMS.Browser.MajorVersion != 6) && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "ie7") //Check for IE7
                    if (client == "ie" && CMS.Browser.MajorVersion == 7 && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!ie7") //Check for NOT IE7
                    if ((client != "ie" || CMS.Browser.MajorVersion != 7) && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "firefox") //Check for Firefox
                    if (client == "firefox" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!firefox") //Check for NOT Firefox
                    if (client != "firefox" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "opera") //Check for Opera
                    if (client == "opera" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!opera") //Check for NOT Opera
                    if (client != "opera" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "chrome") //Check for Chrome
                    if (client == "chrome" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!chrome") //Check for NOT Chrome
                    if (client != "chrome" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "safari") //Check for Safari
                    if (client == "safari" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!safari") //Check for NOT Safari
                    if (client != "safari" && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "mobileview") //Check for mobile device (respects the 'IsNotMobile' cookie)
                    if (CMS.Browser.IsMobile && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!mobileview") //Check for NOT mobile device (respects the 'IsNotMobile' cookie)
                    if (!CMS.Browser.IsMobile && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }

                if (browser == "mobile") //Check for mobile device
                    if (CMS.Browser.IsMobileNoCookie && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
                if (browser == "!mobile") //Check for NOT mobile device
                    if (!CMS.Browser.IsMobileNoCookie && !hide)
                        showControls = true;
                    else if (and)
                    {
                        showControls = false;
                        hide = true;
                    }
            }
            return showControls;
        }
    }
}