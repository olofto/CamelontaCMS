using System;
using System.IO;
using System.Web;
using System.Web.UI;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{
    /// <summary>
    /// Executes a control choosen by Camelonta.CMS.AdminControls.EditControls.UserControlChooser
    /// </summary>
    public class UserControlChooser : BaseControl
    {
        /// <summary>
        /// Path to control (virtual) ~/Controls/Test.ascx
        /// Control should be in Publication.TemplateUrl and that part of the url should not be passed to this control
        /// </summary>
        public string ControlPath { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            ControlPath = CMS.Context.Publication.TemplateUrl + ControlPath;

            if (File.Exists(HttpContext.Current.Server.MapPath(ControlPath)))
            {
                Control control = Page.LoadControl(ControlPath);
                control.ID = "UserControlChooser_" + Globals.CleanupFilename(ControlPath);
                Controls.Add(control);
            }
            else
                Controls.Add(new LiteralControl("Usercontrol: " + ControlPath + " does not exist"));

            base.OnLoad(e);
        }
    }
}
