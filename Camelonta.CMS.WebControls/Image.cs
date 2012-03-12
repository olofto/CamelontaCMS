using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Camelonta.CMS.FrameWork;

namespace Camelonta.CMS.WebControls
{
    public class Image : Camelonta.CMS.Web.BaseControls.BaseControl
    {
        public Image()
        {
        }

        public Image(string filename)
        {
            _Filename = filename;
        }

        private string _Filename = String.Empty;
        public string Filename
        {
            get { return _Filename; }
            set { _Filename = value; }
        }

        private bool _Crop = false;
        public bool Crop
        {
            get { return _Crop; }
            set { _Crop = value; }
        }

        private string _Scale = string.Empty;
        /// <summary>
        /// Use this to set the 'scale' property for ImageResize (both|upscaleonly|downscaleonly|upscalecanvas)
        /// </summary>
        public string Scale 
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        [Obsolete("Use FilenameProperty instead.")]
        public string PropertyName
        {
            get { return _FilenameProperty; }
            set { _FilenameProperty = value; }
        }

        [Obsolete("Use FilenameProperty instead.")]
        public string PropertyFilename
        {
            get { return _FilenameProperty; }
            set { _FilenameProperty = value; }
        }

        private string _FilenameProperty;
        public string FilenameProperty
        {
            get { return _FilenameProperty; }
            set { _FilenameProperty = value; }
        }

        private bool _RecursiveFilenameProperty = false;
        public bool RecursiveFilenameProperty
        {
            get { return _RecursiveFilenameProperty; }
            set { _RecursiveFilenameProperty = value; }
        }

        private string _DefaultFilenameProperty;
        public string DefaultFilenameProperty
        {
            get { return _DefaultFilenameProperty; }
            set { _DefaultFilenameProperty = value; }
        }

        private string _Width = String.Empty;
        public string Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        private string _Height = String.Empty;
        public string Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        private string _MaxWidth = String.Empty;
        public string MaxWidth
        {
            get { return _MaxWidth; }
            set { _MaxWidth = value; }
        }

        private string _MaxHeight = String.Empty;
        public string MaxHeight
        {
            get { return _MaxHeight; }
            set { _MaxHeight = value; }
        }

        private string _BgColor = String.Empty;
        public string BgColor
        {
            get { return _BgColor; }
            set { _BgColor = value; }
        }

        private string _CssClass = String.Empty;
        public string CssClass
        {
            get { return _CssClass; }
            set { _CssClass = value; }
        }

        private string _UseMap = String.Empty;
        public string UseMap
        {
            get { return _UseMap; }
            set { _UseMap = value; }
        }

        private string _Alt = String.Empty;
        public string Alt
        {
            get { return _Alt; }
            set { _Alt = value; }
        }

        private string _AltProperty = String.Empty;
        public string AltProperty
        {
            get { return _AltProperty; }
            set { _AltProperty = value; }
        }

        private bool _RecursiveAltProperty = false;
        public bool RecursiveAltProperty
        {
            get { return _RecursiveAltProperty; }
            set { _RecursiveAltProperty = value; }
        }

        private string _Title = String.Empty;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _TitleProperty = String.Empty;
        public string TitleProperty
        {
            get { return _TitleProperty; }
            set { _TitleProperty = value; }
        }

        private bool _RecursiveTitleProperty = false;
        public bool RecursiveTitleProperty
        {
            get { return _RecursiveTitleProperty; }
            set { _RecursiveTitleProperty = value; }
        }

        private string _PageLink = String.Empty;
        public string PageLink
        {
            get { return _PageLink; }
            set { _PageLink = value; }
        }

        private string _PageLinkProperty = String.Empty;
        public string PageLinkProperty
        {
            get { return _PageLinkProperty; }
            set { _PageLinkProperty = value; }
        }

        private bool _RecursivePageLinkProperty = false;
        public bool RecursivePageLinkProperty
        {
            get { return _RecursivePageLinkProperty; }
            set { _RecursivePageLinkProperty = value; }
        }

        private string _PageLinkCssClass = String.Empty;
        public string PageLinkCssClass
        {
            get { return _PageLinkCssClass; }
            set { _PageLinkCssClass = value; }
        }

        private bool _ContextSensitive = true;
        public bool ContextSensitive
        {
            get { return _ContextSensitive; }
            set { _ContextSensitive = value; }
        }

        private PageData _pageData;

        public override void DataBind()
        {
            if (_ContextSensitive)
            {
                // Override content with current context-PageData from repeater
                Control controlItem = Globals.GetFirstParentControlOfType(this, "System.Web.UI.WebControls.RepeaterItem");
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
                        if (_FilenameProperty == "this")
                        {
                            _FilenameProperty = elementName;

                            // Set new FilenameProperty
                            _FilenameProperty = elementName + "[" + currentRowIndex + "]";
                        }
                        // Advanced (Panel) elements
                        else
                        {
                            // Set new FilenameProperty
                            _FilenameProperty = "(" + elementName + ")[" + currentRowIndex + "]/" + _FilenameProperty;

                            // Set new AltPropertyName
                            if (!String.IsNullOrEmpty(_AltProperty))
                                _AltProperty = "(" + elementName + ")[" + currentRowIndex + "]/" + _AltProperty;

                            // Set new TitlePropertyName
                            if (!String.IsNullOrEmpty(_TitleProperty))
                                _TitleProperty = "(" + elementName + ")[" + currentRowIndex + "]/" + _TitleProperty;

                            // Set new PageLinkProperty
                            if (!String.IsNullOrEmpty(_PageLinkProperty))
                                _PageLinkProperty = "(" + elementName + ")[" + currentRowIndex + "]/" + _PageLinkProperty;
                        }
                    }
                }
            }

            if (_pageData == null)
                _pageData = CMS.Context.PageData;

            base.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!String.IsNullOrEmpty(_FilenameProperty))
                _Filename = CMS.GetPageProperty(_pageData, _FilenameProperty, _RecursiveFilenameProperty, _DefaultFilenameProperty);
            if (!String.IsNullOrEmpty(_AltProperty))
                _Alt = CMS.GetPageProperty(_pageData, _AltProperty, _RecursiveAltProperty, "");
            if (!String.IsNullOrEmpty(_TitleProperty))
                _Title = CMS.GetPageProperty(_pageData, _TitleProperty, _RecursiveTitleProperty, "");
            if (!String.IsNullOrEmpty(_PageLinkProperty))
                _PageLink = CMS.GetPageProperty(_pageData, _PageLinkProperty, _RecursivePageLinkProperty, "");

            if (!String.IsNullOrEmpty(_Filename))
            {
                string imgSrc = "{0}?";
                
                imgSrc = String.Format(imgSrc, _Filename);

                if (!String.IsNullOrEmpty(_Width))
                    imgSrc += "&amp;width=" + _Width;
                if (!String.IsNullOrEmpty(_Height))
                    imgSrc += "&amp;height=" + _Height;
                if (!String.IsNullOrEmpty(_MaxWidth))
                    imgSrc += "&amp;maxwidth=" + _MaxWidth;
                if (!String.IsNullOrEmpty(_MaxHeight))
                    imgSrc += "&amp;maxheight=" + _MaxHeight;
                if (!String.IsNullOrEmpty(_BgColor))
                    imgSrc += "&amp;bgcolor=" + _BgColor;
                if (!String.IsNullOrEmpty(_Scale))
                    imgSrc += "&amp;scale=" + _Scale;
                if (_Crop)
                    imgSrc += "&amp;crop=auto";

                imgSrc = imgSrc.Replace("?&amp;", "?");

                string cssClass = "";
                if (!String.IsNullOrEmpty(_CssClass))
                    cssClass = String.Format(" class=\"{0}\"", _CssClass);

                string altAttr = "";
                if (!String.IsNullOrEmpty(_Alt))
                    altAttr = String.Format(" alt=\"{0}\"", _Alt);

                string titleAttr = "";
                if (!String.IsNullOrEmpty(_Title))
                    titleAttr = String.Format(" title=\"{0}\"", _Title);

                string useMapAttr = "";
                if (!String.IsNullOrEmpty(_UseMap))
                    useMapAttr = String.Format(" usemap=\"{0}\"", _UseMap);

                // Write link
                if (!String.IsNullOrEmpty(_PageLink))
                {
                    string pageLinkCssClass = "";
                    if (!String.IsNullOrEmpty(_PageLinkCssClass))
                        pageLinkCssClass = String.Format(" class=\"{0}\"", _PageLinkCssClass);
                    writer.Write(String.Format("<a href=\"{0}\"{1}>", _PageLink, pageLinkCssClass));
                }

                // Write image
                writer.Write(String.Format("<img src=\"{0}\"{1}{2}{3}{4}/>", imgSrc, cssClass, altAttr, titleAttr, useMapAttr));

                // Write end of link
                if (!String.IsNullOrEmpty(_PageLink))
                    writer.Write("</a>");

                base.Render(writer);
            }
        }

    }
}
