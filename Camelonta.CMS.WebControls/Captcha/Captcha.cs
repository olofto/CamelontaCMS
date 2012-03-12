/*
Captcha asp.net control.
Developed by: Aref Karimi
Email: Arefkr@gmail.com
Last update: 20 October 2009

This code is free to use. 
*/

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{

    public class Captcha : BaseUserControl
    {
        private string _TryDifferentText;
        public string TryDifferentText
        {
            get { return _TryDifferentText; }
            set { _TryDifferentText = value; }
        }
        private bool _LinebreakAfterTextBox;
        public bool LinebreakAfterTextBox
        {
            get { return _LinebreakAfterTextBox; }
            set { _LinebreakAfterTextBox = value; }
        }
        private bool _LinebreakAfterImage;
        public bool LinebreakAfterImage
        {
            get { return _LinebreakAfterImage; }
            set { _LinebreakAfterImage = value; }
        }
        public int MaxLetterCount { get; set; }

        // Constructor
        public Captcha()
        {
            // BaseUserControl defaults ViewState to false
            this.EnableViewState = true; // this control required ViewState.
        }

        readonly System.Web.UI.WebControls.Image imgCaptcha = new System.Web.UI.WebControls.Image();
        readonly LinkButton btnTryNewWords = new LinkButton();
        readonly TextBox txtCpatcha = new TextBox();

        protected override void CreateChildControls()
        {
            this.EnableViewState = true;
            btnTryNewWords.Click += btnTryNewWords_Click;
            btnTryNewWords.Text = TryDifferentText;

            this.Controls.Add(txtCpatcha);
            if (_LinebreakAfterTextBox)
                this.Controls.Add(new LiteralControl("<br/>"));

            this.Controls.Add(imgCaptcha);
            if (_LinebreakAfterImage)
                this.Controls.Add(new LiteralControl("<br/>"));

            this.Controls.Add(btnTryNewWords);

            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (IsPostBack)
                txtCpatcha.Text = "";

            base.Render(writer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (GeneratedText == null)
                    TryNew();
            }
        }

        public void TryNew()
        {
            char[] Valichars = {'1','2','3','4','5','6','7','8','9','0','a','b','c','d','e','f','g','h','i',
                           'j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' };
            string Captcha = "";
            int LetterCount = MaxLetterCount > 5 ? MaxLetterCount : 5;
            for (int i = 0; i < LetterCount; i++)
            {
                char newChar = (char)0;
                do
                {
                    newChar = Char.ToUpper(Valichars[new Random(DateTime.Now.Millisecond).Next(Valichars.Count() - 1)]);
                }
                while (Captcha.Contains(newChar));
                Captcha += newChar;
            }
            GeneratedText = Captcha;

            imgCaptcha.ImageUrl = "CaptchaImage.axd?CaptchaText=" +
                HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(SecurityHelper.EncryptString(Captcha)));
        }

        
        private string GeneratedText
        {
            get
            {
                return ViewState[this.ClientID + "text"] != null ? ViewState[this.ClientID + "text"].ToString() : null;
            }
            set
            {
                // Encrypt the value before storing it in viewstate.
                ViewState[this.ClientID + "text"] = value;
            }
        }


        public bool IsValid
        {
            get
            {
                if (GeneratedText == null)
                    throw new Exception("Captcha control requires ViewState. Please make shure that all controls above have EnableViewState set to true. Ex <CMS:If EnableViewState=\"true\">");
                else
                {
                    bool result = GeneratedText.ToUpper() == txtCpatcha.Text.Trim().ToUpper();
                    if (!result)
                        TryNew();
                    return result;
                }
            }
        }
        protected void btnTryNewWords_Click(object sender, EventArgs e)
        {
            TryNew();
        }
    }
}