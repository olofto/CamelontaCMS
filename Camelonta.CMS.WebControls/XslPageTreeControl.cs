namespace Camelonta.CMS.WebControls
{
    public class XslPageTreeControl : XslControl
    {
        protected override void SetXml()
        {
            _XmlDoc = HostPage.CMS.Context.PageTree;
        }
    }
}
