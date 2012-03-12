using System;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.Web.BaseControls.HttpHandlers;

namespace Camelonta.CMS.WebControls
{
    public class Combiner : Camelonta.CMS.Web.BaseControls.BaseControl
    {
        public Combiner()
        {
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _Media = "all";
        public string Media
        {
            get { return _Media; }
            set { _Media = value; }
        }

        private string _Path;
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }

        private string _Files;
        public string Files
        {
            get { return _Files; }
            set { _Files = value; }
        }

        private int _Version = 1;
        public int Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private bool _Enabled = true;
        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (_Type != "text/css" && _Type != "text/javascript")
                throw new System.Exception("Combiner: Type have to be text/javascript or text/css");

            if (String.IsNullOrEmpty(_Files))
                throw new System.Exception("Combiner: Files must be set to a list of files");

            string cssHtml = "<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" media=\"{2}\" />";
            string jsHtml = "<script src=\"{0}\" type=\"{1}\"></script>";

            if (Enabled)
            {
                /* Use CombinerHandler to combine and compress files */
                string prefix = Globals.CombineUrls(CMS.Context.Publication.TemplateUrl, _Path);
                string url = String.Format(prefix + "/Combiner.axd?f={0}&p={1}&t={2}&v={3}&pubid={4}", _Files, _Path, _Type, _Version, CMS.Context.PublicationID);

                if (_Type == "text/css")
                    writer.Write(String.Format(cssHtml, url, _Type, _Media));
                else
                    writer.Write(String.Format(jsHtml, url, _Type));
            }
            else
            {
                /* Write files as seperate tags to simulate "normal" use for debug */
                string url = "";
                string[] fileNames = _Files.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string fileName in fileNames)
                {
                    if (fileName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || fileName.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                        url = fileName;
                    else
                        url = CombinerHelper.GetUrlFromFilename(_Path, fileName, CMS.Context.Publication);

                    if (_Type == "text/css")
                        writer.Write(String.Format(cssHtml, url, _Type, _Media));
                    else
                        writer.Write(String.Format(jsHtml, url, _Type));
                }
            }
        }
    }

}
