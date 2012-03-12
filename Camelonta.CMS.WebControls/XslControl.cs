using System;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.Web.BaseControls;
using CMSAPI = Camelonta.CMS.Web.BaseControls.XslExtensions.CMSAPI;

namespace Camelonta.CMS.WebControls
{
    /// <summary>
    /// This control is a basic Xsl / Xml transform control. It passes some parameters about Camelonta CMS to the XSL stylesheet. 
    /// Most times you will use the XslPageControl or XslPageTreeControl which inherits from this.
    /// </summary>
    public class XslControl : BaseControl
    {

        protected XsltArgumentList _XslParams;
        protected XmlDocument _XmlDoc;

        public XmlDocument XmlDoc
        {
            get { return _XmlDoc; }
            set { _XmlDoc = value; }
        }

        private string _XmlFile = String.Empty;
        public string XmlFile
        {
            get { return _XmlFile; }
            set { _XmlFile = value; }
        }

        private string _XslFile = String.Empty;
        public string XslFile
        {
            get { return _XslFile; }
            set { _XslFile = value; }
        }

        private XslCompiledTransform _XslCompiledTransform;
        public XslCompiledTransform XslCompiledTransform
        {
            get
            {
                return _XslCompiledTransform;
            }
        }

        private BasePage _hostPage;
        public BasePage HostPage
        {
            get { return _hostPage; }
            set { _hostPage = value; }
        }


        protected override void OnInit(EventArgs e)
        {
            // Creating some objects
            _hostPage = (BasePage)this.Page;
            Publication pub = _hostPage.CMS.Context.Publication;

            // Get the XSL
            if (XslFile.Equals(string.Empty))
                throw new Exception("XslFile attribute is empty");
            else
                _XslCompiledTransform = Templates.GetXslCompiledTransform(_hostPage.CMS.Context.PublicationID, Page.ResolveUrl(XslFile));


            _XslParams = new XsltArgumentList();

            // Adding NodeID as parameter
            try
            {
                _XslParams.AddParam("NodeID", "", _hostPage.CMS.Context.NodeID);
            }
            catch { }

            // Adding some more parameters:
            _XslParams.AddParam("TemplatePath", "", pub.TemplateUrl);
            _XslParams.AddParam("ApplicationPath", "", HttpContext.Current.Request.ApplicationPath);
            _XslParams.AddParam("CurrentPageData", "", _hostPage.CMS.Context.PageData.ToXmlNode());
            _XslParams.AddParam("PubID", "", pub.PubID);
            _XslParams.AddParam("PublishedUrl", "", pub.PublishedPrimaryUrl);
            _XslParams.AddParam("PageType", "", _hostPage.CMS.Context.PageData.PageType.Name);

            // Adding Custom Publication Properties
            XmlDocument siteProperties = SitePropertyHelper.GetSitePropertyPage(pub.PubID).XmlContentNode();
            foreach (XmlNode property in siteProperties.ChildNodes)
            {
                string customPropValue = property.InnerXml;
                _XslParams.AddParam(property.Name, "", customPropValue);
            }

            // Adding CMS API
            CMSAPI cms = new CMSAPI();
            cms.LiveMode = Globals.IsLiveMode();
            cms.UrlProvider = _hostPage.CMS.Context.UrlProvider;
            _XslParams.AddExtensionObject("urn:CMS", cms);

            SetXml();

            base.OnInit(e);
        }

        protected virtual void SetXml()
        {
            _XmlDoc.Load(HttpContext.Current.Server.MapPath(XmlFile));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.XslCompiledTransform.Transform(_XmlDoc, _XslParams, writer);
            base.Render(writer);
        }
    }
}
