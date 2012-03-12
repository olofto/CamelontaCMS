using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Xml;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{
    /// <summary>
    /// Iterates over a series of Custom Page Properties on a page
    /// </summary>
    public class Foreach : Repeater
    {
        private Int32 _PageNodeID;
        public Int32 PageNodeID
        {
            get { return _PageNodeID; }
            set { _PageNodeID = value; }
        }

        /// <summary>
        /// Name of the Property on the PageData. Valid values are the name of the Properties you have defined in the Schema-file.
        /// </summary>
        private string _Property;
        public string Property
        {
            get { return _Property; }
            set { _Property = value; }
        }

        /// <summary>
        /// Randomize the order of the items. Currently only works for Properties and not pages...
        /// </summary>
        private bool _Randomize;
        public bool Randomize
        {
            get { return _Randomize; }
            set { _Randomize = value; }
        }

        private bool _Recursive;
        public bool Recursive
        {
            get { return _Recursive; }
            set { _Recursive = value; }
        }

        protected override void CreateChildControls()
        {
            this.EnableViewState = false;
            BasePage basePage = (BasePage)this.Page;
            XmlNode xmlContentNode = null;

            if (_PageNodeID > 0)
            {
                // Override xmlContentNode with the PageData defined with PageNodeID
                xmlContentNode = basePage.CMS.GetPage(_PageNodeID).XmlContentNode().DocumentElement;
            }
            else
            {
                if (this.Parent.GetType().ToString() == "System.Web.UI.WebControls.RepeaterItem")
                {
                    try
                    {
                        // Override xmlContentNode with current context-page in repeater
                        RepeaterItem parentItem = (RepeaterItem)this.Parent;
                        PageNode pageNode = (PageNode)parentItem.DataItem;
                        xmlContentNode = basePage.CMS.GetPage(pageNode.NodeID).XmlContentNode().DocumentElement;
                    }
                    catch { }
                }

                if (xmlContentNode == null)
                {
                    // Default
                    xmlContentNode = basePage.CMS.Context.PageData.XmlContentNode().DocumentElement;
                }
            }

            // Make it recursive
            if (_Recursive)
            {
                if (xmlContentNode.SelectNodes(_Property).Count == 0)
                {
                    List<PageData> ancestors = Pages.GetAncestorsAndSelf(basePage.CMS.Context.NodeID);
                    foreach (PageData page in ancestors)
                    {
                        if (xmlContentNode.SelectNodes(_Property).Count == 0)
                            if (page.XmlContentNode().FirstChild.SelectNodes(_Property).Count > 0)
                            {
                                basePage.CMS.RewriteUrl(page);
                                xmlContentNode = page.XmlContentNode().DocumentElement;
                            }
                    }
                }
            }

            XmlNodeList properties = xmlContentNode.SelectNodes(_Property);

            if (Randomize)
            {
                XmlNodeList randomProps = properties;
                List<String> nodes = new List<String>();
                foreach (XmlNode xn in properties)
                {
                    nodes.Add(xn.OuterXml);
                }

                nodes.Shuffle();

                string randomizedXml = "<Randomized>";
                foreach (String pxml in nodes)
                {
                    Debug.WriteLine(pxml);
                    randomizedXml += pxml;
                }
                randomizedXml += "</Randomized>";
                XmlDocument randomizedProps = new XmlDocument();
                randomizedProps.LoadXml(randomizedXml);
                properties = randomizedProps.DocumentElement.SelectNodes("*");
            }

            if (properties.Count > 0)
                this.DataSource = ConvertXmlNodeListToDataTable(properties);

            base.CreateChildControls();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            // Hide errors when trying to get properties that don't exist
            try
            {
                base.OnDataBinding(e);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Foreach: " + this.ID + "; PageNodeID: " + _PageNodeID + "; Property: " + _Property + " Message: " + ex.Message, ex);
            }
        }

        private DataTable ConvertXmlNodeListToDataTable(XmlNodeList xnl)
        {
            DataTable dt = new DataTable();
            int TempColumn = 0;

            // Add dynamic columns
            foreach (XmlNode parentNode in xnl)
            {
                foreach (XmlNode node in parentNode.ChildNodes)
                {
                    TempColumn++;
                    DataColumn dc = new DataColumn(node.Name, Type.GetType("System.String"));
                    if (!dt.Columns.Contains(node.Name))
                    {
                        dt.Columns.Add(dc);
                    }
                }
            }

            // Add static columns
            DataColumn dcSelf = new DataColumn("this", Type.GetType("System.String"));
            dt.Columns.Add(dcSelf);

            DataColumn dcElementName = new DataColumn("ElementName", Type.GetType("System.String"));
            dt.Columns.Add(dcElementName);

            DataColumn dcPageNodeID = new DataColumn("PageNodeID", Type.GetType("System.String"));
            dt.Columns.Add(dcPageNodeID);

            // Add content
            int ColumnsCount = dt.Columns.Count;
            for (int i = 0; i < xnl.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < ColumnsCount; j++)
                {
                    if (xnl.Item(i).ChildNodes[j] != null)
                        dr[xnl.Item(i).ChildNodes[j].Name] = Globals.RemoveCDATA(xnl.Item(i).ChildNodes[j].InnerXml);
                }
                // Add static content
                dr["this"] = Globals.RemoveCDATA(xnl.Item(i).InnerXml);
                dr["ElementName"] = xnl.Item(i).Name;
                dr["PageNodeID"] = this.PageNodeID;

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}