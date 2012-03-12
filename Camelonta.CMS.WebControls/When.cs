using System;
using System.Web.UI;

namespace Camelonta.CMS.WebControls
{
    /// <summary>
    /// Shows and hides content depending on the values of the properties. True shows "Then", false shows "Otherwise". Same input parameters as the If-controler
    /// </summary>
    [ParseChildren(true)]
    public class When : If
    {
        /// <summary>
        /// When expressions are true this template will be shown
        /// </summary>
        [TemplateContainer(typeof(TemplateControl))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate Then { get; set; }

        /// <summary>
        /// When expressions are false this template will be shown
        /// </summary>
        [TemplateContainer(typeof(TemplateControl))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate Otherwise { get; set; }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e); // Load state from If-control

            if (!_hideControls && Then != null)
                Then.InstantiateIn(this);
            if (_hideControls && Otherwise != null)
                Otherwise.InstantiateIn(this);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // Do not hide any controls, we want to see them all since we only have instantiated the ones we want
            _hideControls = false;

            base.Render(writer);
        }
    }
}