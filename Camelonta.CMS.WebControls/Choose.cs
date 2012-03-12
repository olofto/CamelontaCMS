using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Camelonta.CMS.WebControls
{
    public class Choose : WebControl, INamingContainer 
    {
        private MyITemplate _When;
        [PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(SimpleItem))]
        public MyITemplate When
        {
            get { return _When; }
            set { _When = value; }
        }

        private MyITemplate _Otherwise;
        [PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(SimpleItem))]
        public MyITemplate Otherwise
        {
            get { return _Otherwise; }
            set { _Otherwise = value; }
        }

        protected override void CreateChildControls()
        {
            SimpleItem item = new SimpleItem();
            if (System.Web.HttpContext.Current.Request.QueryString["when"] != null)
            {
                if (_When != null)
                {
                    //System.Web.HttpContext.Current.Response.Write("When: " +_When.Test);
                    _When.InstantiateIn(item);
                    this.Controls.Add(item);
                }
            }
            else
            {
                if (_Otherwise != null)
                {
                    System.Web.HttpContext.Current.Response.Write("Otherwise");
                    _Otherwise.InstantiateIn(item);
                    this.Controls.Add(item);
                }
            }

            base.CreateChildControls();
        }

        /*
        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            int count = 0;
            if (dataBinding)
            {
                foreach (object dataItem in dataSource)
                {
                    if (_When != null)
                    {
                        // create instance of SimpleItem control
                        SimpleItem item = new SimpleItem(dataItem, count++);

                        // instantiate in new item object
                        _When.InstantiateIn(item);

                        // add item to Controls collection
                        this.Controls.Add(item);

                        // need to support <%# %> expressions
                        item.DataBind();
                    }
                }
            }
            return count;
        }*/
    }

    public class SimpleItem : Control, IDataItemContainer
    {
        public SimpleItem()
        {
        }
        public SimpleItem(object dataItem, int index)
        {
            _DataItem = dataItem;
            _DataItemIndex = _DisplayIndex = index;
        }
        private object _DataItem;
        public object DataItem
        {
            get { return _DataItem; }
        }

        private int _DataItemIndex;
        public int DataItemIndex
        {
            get { return 0; }
        }

        private int _DisplayIndex;
        public int DisplayIndex
        {
            get { return 0; }
        }
    }


    public class MyITemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
        }

        private string _Test;
        public string Test
        {
            get { return _Test; }
            set { _Test = value; }
        }
    }
}
