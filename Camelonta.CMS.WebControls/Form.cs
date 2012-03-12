using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Camelonta.CMS.FrameWork;
using System.Net.Mail;

namespace Camelonta.CMS.Plus.WebControls.Form
{
    /// <summary>
    /// Form - Renders elements from Form.xml to create a form
    /// </summary>
    public class Form : Camelonta.CMS.Web.BaseControls.BaseControl
    {
        static ListItemCollection _RegisterForEventValidation = new ListItemCollection();


        /// <summary>
        /// Create form
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            CreateForm();
            base.OnInit(e);
        }

        /// <summary>
        /// Handle postback
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            if (Page.IsPostBack)
                HandlePostBack();
        }

        /// <summary>
        /// Register controls for event validation (ex DropDownList)
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            foreach (ListItem listItem in _RegisterForEventValidation)
                Page.ClientScript.RegisterForEventValidation(listItem.Text, listItem.Value);
            base.Render(writer);
        }

        /// <summary>
        /// Handle postback
        /// </summary>
        private void HandlePostBack()
        {
            //PostToScreen();
            PostToXML();
            FileUpload();
            PostToMail();
        }

        /// <summary>
        /// Create the form
        /// </summary>
        private void CreateForm()
        {
            Button submit = new Button();

            Int32 i = 0;
            foreach (XmlNode xmlNode in CMS.Context.Page.XmlContentNode.ChildNodes)
            {
                string uniqueID = (i++).ToString();

                // Add controls
                FormItem formItem = new FormItem(xmlNode, uniqueID);
                if (xmlNode.Name == "SubmitButton")
                {
                    submit = (Button)formItem.GetControl();
                }
                else
                {
                    // Add controls
                    Controls.Add(formItem);
                }
            }
            if (!String.IsNullOrEmpty(submit.Text))
                Controls.Add(submit);
        }

        /// <summary>
        /// Get correctyly typed control
        /// </summary>
        public static Control GetControl(string name, string label, string uniqueID, ListItemCollection listItems)
        {
            Control ctrl = new Control();

            if (name == "TextBox")
            {
                ctrl = new TextBox();
            }
            else if (name == "TextArea")
            {
                ctrl = new TextBox() { TextMode = TextBoxMode.MultiLine };
            }
            else if (name == "TextLabel")
            {
                ctrl = new Label() { Text = label };
            }
            else if (name == "DropDownList")
            {
                ctrl = new DropDownList();
                foreach (ListItem listItem in listItems)
                {
                    ((DropDownList)ctrl).Items.Add(listItem);
                    _RegisterForEventValidation.Add(new ListItem("Control__" + uniqueID, listItem.Value));
                }
            }
            else if (name == "FileUpload")
            {
                ctrl = new FileUpload();
            }
            else if (name == "CheckBoxList")
            {
                ctrl = new CheckBoxList();
                foreach (ListItem listItem in listItems)
                    ((CheckBoxList)ctrl).Items.Add(listItem);
            }
            else if (name == "RadioButtonList")
            {
                ctrl = new RadioButtonList();
                foreach (ListItem listItem in listItems)
                    ((RadioButtonList)ctrl).Items.Add(listItem);
            }
            else if (name == "TermsAndConditionsCheckBox")
            {
                throw new NotImplementedException();
            }
            else if (name == "CustomStyleSheet")
            {
                //throw new NotImplementedException();
            }
            else if (name == "SubmitButton")
            {
                ctrl = new Button() { Text = label };
            }

            if (String.IsNullOrEmpty(ctrl.ID))
                ctrl.ID = "Control__" + uniqueID;
            return ctrl;
        }

        /// <summary>
        /// Get validator for control
        /// </summary>
        public static Control GetValidator(string validationType, string uniqueID, string validationMessage)
        {
            if (validationType == "isEmpty")
            {
                RequiredFieldValidator val = new RequiredFieldValidator();
                val.ControlToValidate = "Control__" + uniqueID;
                val.Text = validationMessage;
                return val;
            }
            else if (validationType != "")
            {
                throw new NotImplementedException();
            }
            return null;
        }

        /// <summary>
        /// Mail the posted data
        /// </summary>
        private void PostToMail()
        {
            bool boolSendMail = false;
            string subject = "";
            string toAdress = "";
            string replyTo = "";

            Int32 i = 0;
            foreach (XmlNode xmlNode in CMS.Context.Page.XmlContentNode.ChildNodes)
            {
                string uniqueID = (i++).ToString();
                FormItem formItem = new FormItem(xmlNode, uniqueID);

                if (formItem.SendMail.isSendMail)
                {
                    boolSendMail = formItem.SendMail.isSendMail;
                    subject = formItem.SendMail.MailSubject;
                    toAdress = formItem.SendMail.MailToAdress;
                    replyTo = formItem.SendMail.MailReplyTo;
                }
            }


            if (boolSendMail)
            {
                string content = "";
                foreach (Control ctrl in Controls)
                {
                    string type = GetNiceTypeName(ctrl.GetType());

                    if (type == "FormItem")
                    {
                        FormItem formItem = ((FormItem)ctrl);
                        if (formItem.Visible)
                            content += formItem.ToRowInMail();

                    }
                }

                content = String.Format("<table>{0}</table>", content);

                // Create a message and set up the recipients.
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("olof@camelonta.se", toAdress, subject, content);
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Body = content;

                // Reading SmtpServer from web.config
                string smtpServer = Settings.Configuration().MailSettings.SmtpServer;
                SmtpClient client = new SmtpClient(smtpServer);
                // Add credentials if the SMTP server requires them.
                if (Settings.Configuration().MailSettings.SmtpUsername != string.Empty && Settings.Configuration().MailSettings.SmtpPassword != string.Empty)
                {
                    client.Credentials = new System.Net.NetworkCredential(Settings.Configuration().MailSettings.SmtpUsername, Settings.Configuration().MailSettings.SmtpPassword);
                    //HttpContext.Current.Trace.Write("Using credentials: " + Settings.Configuration().MailSettings.SmtpUsername  + " - " + Settings.Configuration().MailSettings.SmtpPassword);
                }
                else
                {
                    HttpContext.Current.Trace.Write("Using default credentials");
                    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }

                client.Send(message);
                message.Dispose();


                //Messaging.SendMail(toAdress, replyTo, subject, content);
            }
        }

        /// <summary>
        /// Upload file(s)
        /// </summary>
        private void FileUpload()
        {
            foreach (Control ctrl in Controls)
            {
                string type = GetNiceTypeName(ctrl.GetType());
                if (type == "FormItem")
                {
                    FormItem formItem = ((FormItem)ctrl);
                    if (formItem.Type == "FileUpload")
                    {
                        if (formItem.UploadFile.isUploadFile)
                        {
                            FileUpload fileUpload = (FileUpload)formItem.GetControl();
                            if (fileUpload.HasFile)
                            {
                                string saveTo = MapPathSecure("~/" + Path.Combine(formItem.UploadFile.PhysicalDirectory, fileUpload.FileName));
                                fileUpload.SaveAs(saveTo);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Post to screen for debugging
        /// </summary>
        private void PostToScreen()
        {
            System.Web.HttpContext.Current.Response.Write("PostBack - " + Controls.Count + " controls<br/>");

            foreach (Control ctrl in Controls)
            {
                string type = GetNiceTypeName(ctrl.GetType());

                if (type == "FormItem")
                {
                    FormItem formItem = ((FormItem)ctrl);
                    System.Web.HttpContext.Current.Response.Write(formItem.Name + ": " + formItem.GetValue() + "<br/>");
                }
            }
        }

        /// <summary>
        /// Post to XML-file
        /// </summary>
        private void PostToXML()
        {
            XmlDocument xmlDoc = GetXml();
            XmlElement postElement = xmlDoc.CreateElement("Post");

            // Lägger till datum för svar:
            XmlElement fieldElementDate = xmlDoc.CreateElement("Field");
            XmlElement typeElementDate = xmlDoc.CreateElement("Type");
            XmlElement labelElementDate = xmlDoc.CreateElement("Label");
            XmlElement valueElementDate = xmlDoc.CreateElement("Value");
            typeElementDate.InnerText = "DateTime";
            labelElementDate.InnerText = "Date";
            valueElementDate.InnerText = DateTime.Now.ToString("s");

            fieldElementDate.AppendChild(typeElementDate);
            fieldElementDate.AppendChild(labelElementDate);
            fieldElementDate.AppendChild(valueElementDate);
            postElement.AppendChild(fieldElementDate);


            foreach (Control ctrl in Controls)
            {
                string type = GetNiceTypeName(ctrl.GetType());

                if (type == "FormItem")
                {
                    FormItem formItem = ((FormItem)ctrl);

                    XmlElement fieldElement = xmlDoc.CreateElement("Field");
                    XmlElement typeElement = xmlDoc.CreateElement("Type");
                    XmlElement labelElement = xmlDoc.CreateElement("Label");
                    XmlElement valueElement = xmlDoc.CreateElement("Value");

                    valueElement.InnerXml = formItem.GetValue();

                    if (formItem.Visible)
                    {
                        typeElement.InnerText = formItem.Type;
                        labelElement.InnerText = formItem.LabelName;

                        fieldElement.AppendChild(typeElement);
                        fieldElement.AppendChild(labelElement);
                        fieldElement.AppendChild(valueElement);
                        postElement.AppendChild(fieldElement);
                    }
                }
            }

            xmlDoc.DocumentElement.AppendChild(postElement);
            xmlDoc.Save(GetXmlFilename());
            System.Web.HttpContext.Current.Response.Write("GetXmlFilename: " + GetXmlFilename() + "<br/>");
        }

        /// <summary>
        /// Get XML-file from disk
        /// </summary>
        protected XmlDocument GetXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Försöka ladda ifrån hårddisken
            string xmlFilename = GetXmlFilename();
            if (File.Exists(xmlFilename))
            {
                xmlDoc.Load(xmlFilename);
            }
            else
            {
                // Om inte finns: Skapa nytt:
                xmlDoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Posts/>");
            }
            return xmlDoc;
        }

        /// <summary>
        /// Get filename of XML-file
        /// </summary>
        private string GetXmlFilename()
        {
            return (System.Web.HttpContext.Current.Server.MapPath(
                String.Format("~/PostData_{0}_{1}.xml", CMS.Context.PublicationID, CMS.Context.Page.NodeID))
                );
        }

        /// <summary>
        /// Simplify the name of the type
        /// </summary>
        public static string GetNiceTypeName(Type type)
        {
            return type.ToString().Replace("System.Web.UI.WebControls.", "");
        }
    }
}

/// <summary>
/// Item for each control in the form
/// </summary>
class FormItem : Control
{
    public string Name { get { return _Name; } }
    public string Type { get { return _Type; } }
    public string LabelName { get { return _LabelName; } }
    public XmlNode XmlNode { get { return _XmlNode; } }
    public override bool Visible { get { return _Visible; } }

    public SendMail SendMail;
    public UploadFile UploadFile;

    string _Name;
    string _Type;
    string _LabelName;
    Label _Label;
    Control _Control;
    Control _Validator;
    XmlNode _XmlNode;
    bool _Visible;

    public FormItem(XmlNode xmlNode, string uniqueID)
    {
        SendMail = new SendMail();
        UploadFile = new UploadFile();

        // Create control-containers
        _Label = new Label();
        _Label.ID = "Label__" + uniqueID;
        Control validator = new Control();

        // Set texts
        string validationType = "";
        string validationMessage = "";
        ListItemCollection listItems = new ListItemCollection();
        foreach (XmlNode xmlSubNode in xmlNode)
        {
            // Label
            if (xmlSubNode.Name == "Label")
                _Label.Text = xmlSubNode.InnerXml;

            // Validation
            if (xmlSubNode.Name == "Validation")
                validationType = xmlSubNode.InnerXml;

            // Message for validation
            if (xmlSubNode.Name == "ValidationMessage")
                validationMessage = xmlSubNode.InnerXml;

            // Items
            if (xmlSubNode.Name == "ListItems")
                foreach (string listItem in xmlSubNode.InnerXml.Split("\n".ToCharArray()))
                    listItems.Add(listItem.Trim());
        }

        // Mail
        if (xmlNode.Name == "Mail")
            SendMail = new SendMail(xmlNode);

        // Fileupload
        if (xmlNode.Name == "FileUpload")
            UploadFile = new UploadFile(xmlNode);

        // Get control with correct type
        Control ctrl = Camelonta.CMS.WebControls.Form.GetControl(xmlNode.Name, _Label.Text, uniqueID, listItems);

        _Name = xmlNode.Name;
        _Control = ctrl;
        _Validator = Camelonta.CMS.WebControls.Form.GetValidator(validationType, uniqueID, validationMessage);
        _Type = Camelonta.CMS.WebControls.Form.GetNiceTypeName(_Control.GetType());
        _LabelName = _Label.Text;
        _XmlNode = xmlNode;

        _Visible = true;
        if (Type == "HiddenField" || Name == "FileUpload" || Name == "Mail")
            _Visible = false;
    }

    protected override void OnInit(EventArgs e)
    {
        // Create layout-controls (Label-control has only a Label and no Control)
        if (Name == "TextLabel")
        {
            Panel labelDiv = new Panel();
            labelDiv.CssClass = "Form_Label " + _Name;
            labelDiv.Controls.Add(_Label);
            Controls.Add(labelDiv);
        }
        else if (Type == "HiddenField")
        {
            Controls.Add(_Control);
        }
        else
        {
            Panel labelDiv = new Panel();
            Panel controlDiv = new Panel();
            labelDiv.CssClass = "Form_Label " + _Name;
            controlDiv.CssClass = "Form_Control " + _Name;
            labelDiv.Controls.Add(_Label);
            controlDiv.Controls.Add(_Control);
            if (_Validator != null)
                controlDiv.Controls.Add(_Validator);
            Controls.Add(labelDiv);
            Controls.Add(controlDiv);
        }

        base.OnInit(e);
    }

    public Control GetControl()
    {
        return _Control;
    }

    public string GetValue()
    {
        string ret = "";
        foreach (Control panels in Controls)
        {
            foreach (Control ctrl in panels.Controls)
            {
                if (ctrl.ID != null)
                    if (ctrl.ID.StartsWith("Control__"))
                    {
                        string type = Camelonta.CMS.WebControls.Form.GetNiceTypeName(ctrl.GetType());

                        if (type == "TextBox" || type == "TextArea")
                            ret = ((TextBox)ctrl).Text;
                        else if (type == "CheckBoxList")
                        {
                            foreach (ListItem item in ((CheckBoxList)ctrl).Items)
                                if (item.Selected)
                                    ret += item.Value;
                        }
                        else if (type == "RadioButtonList")
                        {
                            foreach (ListItem item in ((RadioButtonList)ctrl).Items)
                                if (item.Selected)
                                    ret += item.Value;
                        }
                        else if (type == "DropDownList")
                            ret = ((DropDownList)ctrl).SelectedValue;
                        else if (type == "FileUpload")
                            ret = this.UploadFile.UrlToDirectory + "/" + ((FileUpload)ctrl).FileName;
                        else if (type == "HiddenField")
                            ret = ((HiddenField)ctrl).Value;

                    }
            }
        }

        return ret;
    }

    public string ToRowInMail()
    {
        return String.Format("<tr><td>{0}</td><td>{1}</td></tr>", _Label.Text, GetValue());
    }
}

/// <summary>
/// Used if FormItem is of type Mail
/// </summary>
class SendMail
{
    public bool isSendMail;
    public string MailToAdress;
    public string MailReplyTo;
    public string MailSubject;

    public SendMail()
    {
        isSendMail = false;
    }

    public SendMail(XmlNode xmlNode)
    {
        foreach (XmlNode xmlSubNode in xmlNode)
        {
            if (xmlSubNode.Name == "SendMail")
                isSendMail = xmlSubNode.InnerXml == "True" ? true : false;
            if (xmlSubNode.Name == "MailToAdress")
                MailToAdress = xmlSubNode.InnerXml;
            if (xmlSubNode.Name == "MailReplyTo")
                MailReplyTo = xmlSubNode.InnerXml;
            if (xmlSubNode.Name == "MailSubject")
                MailSubject = xmlSubNode.InnerXml;
        }
    }
}

/// <summary>
/// Used if FormItem is of type FileUpload
/// </summary>
class UploadFile
{
    public bool isUploadFile;
    public bool SaveToDisk;
    public string PhysicalDirectory;
    public string UrlToDirectory;


    public UploadFile()
    {
        isUploadFile = false;
    }

    public UploadFile(XmlNode xmlNode)
    {
        foreach (XmlNode xmlSubNode in xmlNode)
        {
            if (xmlSubNode.Name == "SaveToDisk")
            {
                SaveToDisk = xmlSubNode.InnerXml == "True" ? true : false;
                isUploadFile = true;
            }
            if (xmlSubNode.Name == "PhysicalDirectory")
                PhysicalDirectory = xmlSubNode.InnerXml;
            if (xmlSubNode.Name == "UrlToDirectory")
                UrlToDirectory = xmlSubNode.InnerXml;
        }
    }
}
