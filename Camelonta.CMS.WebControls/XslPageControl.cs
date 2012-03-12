using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camelonta.CMS.WebControls
{
    public class XslPageControl : Camelonta.CMS.WebControls.XslControl
    {

        public XslPageControl()
        {
        }

        protected override void SetXml()
        {
            _XmlDoc = HostPage.CMS.Context.PageData.ToXmlNode().OwnerDocument;
        }
    }
}
