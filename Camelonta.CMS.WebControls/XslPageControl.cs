namespace Camelonta.CMS.WebControls
{
    public class XslPageControl : XslControl
    {
        protected override void SetXml()
        {
            _XmlDoc = HostPage.CMS.Context.PageData.ToXmlNode().OwnerDocument;
        }
    }
}
