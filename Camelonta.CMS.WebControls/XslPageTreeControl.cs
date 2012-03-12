using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camelonta.CMS.WebControls
{
    public class XslPageTreeControl : Camelonta.CMS.WebControls.XslControl
    {

        public XslPageTreeControl()
        {
        }

        protected override void SetXml()
        {
            _XmlDoc = HostPage.CMS.Context.PageTree;
        }
    }
}
