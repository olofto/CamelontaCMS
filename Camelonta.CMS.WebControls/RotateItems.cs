using System;
using System.Web;
using System.Xml;
using Camelonta.CMS.FrameWork;

namespace Camelonta.CMS.WebControls
{
	/// <summary>
	/// This class will rotate elements so that the viewer sees one after another. You use it by setting the NodeSet property to an Xpath expression
    /// pointing out the elements that you want to rotate (for example "//images"). 
	/// </summary>
	public class RotateItems : XslPageControl
	{

		protected string _NodeSet="//*";
		public string NodeSet
		{
			get
			{
				return _NodeSet;
			}

			set
			{
				_NodeSet = value;
			}
		}


	    protected override void OnPreRender(EventArgs e)
		{

			XmlNodeList nl = XmlDoc.DocumentElement.SelectNodes(_NodeSet);
            _XslParams.AddParam("NodeSet", "", NodeSet);
			HttpCookie indexCookie;
			if(nl!=null)
			{
				int maxCount = nl.Count;
				int currentUserIndex = 1;
                _XslParams.AddParam("MaxCount", "", maxCount.ToString());
                Logger.Debug("RotateItems: max count av " + _NodeSet + ": " + maxCount.ToString());
				// finding out users previous viewed number
				try
				{
					if (HttpContext.Current.Request.Cookies[this.UniqueID] != null)
					{
						indexCookie = HttpContext.Current.Request.Cookies[this.UniqueID];
						currentUserIndex = Convert.ToInt32(indexCookie.Value);
						Logger.Debug("RotateItems: Read CurrentUserIndex OK " + currentUserIndex);
					}
				}
				catch(Exception ex)
				{
					Logger.Error("RotateItems: Error: " + ex.Message);
				}
                _XslParams.AddParam("Index", "", currentUserIndex.ToString());
				if(currentUserIndex < maxCount)
					currentUserIndex++;
				else
					currentUserIndex = 1;

				Logger.Debug("RotateItems: Index: " + currentUserIndex);
				indexCookie = new HttpCookie(this.UniqueID);
				indexCookie.Value = currentUserIndex.ToString();
				HttpContext.Current.Response.AppendCookie(indexCookie);
			}
			base.OnPreRender (e);
		}
	}
}
