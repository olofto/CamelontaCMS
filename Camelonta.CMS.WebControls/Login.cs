using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Camelonta.CMS.FrameWork;
using Camelonta.CMS.Web.BaseControls;

namespace Camelonta.CMS.WebControls
{
    public class Login : BaseControl
    {
        // General settings
        public bool HideIfLoggedIn { get; set; }

        // General texts
        public string UserNameLabelText { get; set; }
        public string PasswordLabelText { get; set; }
        public string UserNotFound { get; set; }

        // Texts for login form
        public string LoginTitleText { get; set; }
        public bool LoginRememberMeSet { get; set; }
        public string LoginRecoverPasswordText { get; set; }

        // Texts for get password form
        public string GetPasswordTitleText { get; set; }
        public string GetPasswordButtonText { get; set; }
        public string GetPasswordMailTitle { get; set; }
        public string GetPasswordMailText { get; set; }
        public string GetPasswordMailSent { get; set; }

        // Texts for set password form
        public string SetPasswordTitleText { get; set; }
        public string SetPasswordOldLinkText { get; set; }
        public string SetPasswordButtonText { get; set; }
        public string SetPasswordChanged { get; set; }
        

        // Forms
        private System.Web.UI.WebControls.Login LoginForm;
        private PlaceHolder GetPasswordForm;
        private PlaceHolder SetPasswordForm;

        protected override void OnLoad(EventArgs e)
        {
            // Log out
            if (Page.Request.QueryString["logout"] != null)
            {
                FormsAuthentication.SignOut();
                Page.Response.Redirect(CMS.Context.PageData.Url, true);
            }


            if (!HideIfLoggedIn || !Page.User.Identity.IsAuthenticated)
            {
                // Clear variables so they don't get default values and set default values
                if (LoginTitleText == null)
                    LoginTitleText = "";
                if (UserNameLabelText == null)
                    UserNameLabelText = "Användarnamn";
                if (PasswordLabelText == null)
                    PasswordLabelText = "Lösenord";
                if (UserNotFound == null)
                    UserNotFound = "Användaren {0} finns inte";
                if (LoginRecoverPasswordText == null)
                    LoginRecoverPasswordText = "Återställ lösenord";
                if (GetPasswordTitleText == null)
                    GetPasswordTitleText = "Skriv in ditt användarnamn för att återställa ditt lösenord";
                if (GetPasswordButtonText == null)
                    GetPasswordButtonText = "Skicka";
                if (GetPasswordMailTitle == null)
                    GetPasswordMailTitle = "Återställning av lösenord";
                if (GetPasswordMailText == null)
                    GetPasswordMailText = "Klicka på länken för att kunna återställa ditt lösenord";
                if (GetPasswordMailSent == null)
                    GetPasswordMailSent = "Ett mail har skickats till din emailadress";
                if (SetPasswordTitleText == null)
                    SetPasswordTitleText = "Välj ditt nya lösenord";
                if (SetPasswordOldLinkText == null)
                    SetPasswordOldLinkText = "Du försöker återsälla ditt lösenord med en gammal länk. Använd \"Återställ lösenord\" för att be om en ny länk.";
                if (SetPasswordButtonText == null)
                    SetPasswordButtonText = "Spara lösenord";
                if (SetPasswordChanged == null)
                    SetPasswordChanged = "Ditt lösenord har nu ändrats";


                AddLoginForm();
                AddGetPasswordForm();
                AddSetPasswordForm();

                // Event that happens when user comes here from a passwordlink
                if (!Page.IsPostBack)
                {
                    string username = Page.Request.QueryString["username"];
                    string uniqueID = Page.Request.QueryString["uniqueID"];

                    if (username != null && uniqueID != null)
                    {
                        MembershipUser user = Membership.GetUser(username);
                        Guid key = new Guid(uniqueID);

                        // check uniqeid on user's profile and show form
                        if (Users.IsKeyExist(username, key))
                        {
                            // Show set password form
                            SetPasswordForm.Visible = true;
                        }
                        else
                        {
                            base.Controls.Add(new LiteralControl(SetPasswordOldLinkText));
                        }
                    }
                    else if (Page.Request.QueryString["recover"] != null)
                    {
                        // Show get password form
                        GetPasswordForm.Visible = true;
                    }
                    else
                    {
                        // Show login
                        LoginForm.Visible = true;
                        base.Controls.Add(new LiteralControl("<p><a href=\"?recover=1\">" + LoginRecoverPasswordText + "</a></p>"));
                    }
                }
            }

            base.OnLoad(e);
        }

        #region Login

        private void AddLoginForm()
        {
            LoginForm = new System.Web.UI.WebControls.Login
                {
                    ID = "Login1",
                    Visible = false,
                    TitleText = LoginTitleText,
                    RememberMeSet = LoginRememberMeSet,
                    UserNameLabelText = UserNameLabelText,
                    PasswordLabelText = PasswordLabelText
                };
            LoginForm.Authenticate += Login1_Authenticate;
            base.Controls.Add(LoginForm);
        }


        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            // Login
            if (Membership.ValidateUser(LoginForm.UserName, LoginForm.Password))
            {
                SyncUser();
                FormsAuthentication.RedirectFromLoginPage(LoginForm.UserName, LoginForm.RememberMeSet);
            }
            else
            {
                // Not valid, clear
                e.Authenticated = false;
                LoginForm.Visible = true;
            }
        }

        /// <summary>
        /// Sync user with membership database
        /// </summary>
        private void SyncUser()
        {
            // Check that this user is in at least 1 Camelonta Role.
            if (Users.IsInAnyCamelontaRole(LoginForm.UserName))
            {
                // OK. Synch details from Membership Database.
                Users.SyncUserFromMembershipDatabase(LoginForm.UserName, new List<string>());
            }
        }
        #endregion



        #region Get Password

        private void AddSetPasswordForm()
        {
            string username = Page.Request.QueryString["username"];

            SetPasswordForm = new PlaceHolder();
            SetPasswordForm.Controls.Add(new LiteralControl("<p>" + SetPasswordTitleText + "</p>"));
            SetPasswordForm.Controls.Add(new LiteralControl(UserNameLabelText + ": " + username));
            SetPasswordForm.Controls.Add(new LiteralControl("<br/>" + PasswordLabelText + "<br/>"));
            SetPasswordForm.Controls.Add(new TextBox { ID = "tbSetPassword", TextMode = TextBoxMode.Password });
            SetPasswordForm.Controls.Add(new LiteralControl("<br/>"));

            Button button = new Button { ID = "btnSetSubmit", Text = SetPasswordButtonText };
            button.Click += SetPasswordForm_Click;
            SetPasswordForm.Controls.Add(button);


            SetPasswordForm.Visible = false;
            base.Controls.Add(SetPasswordForm);
        }



        ///// <summary>
        /////  Send passwordlink
        ///// </summary>
        protected void GetPasswordForm_Click(object sender, EventArgs e)
        {
            TextBox tbUserName = (TextBox)GetPasswordForm.FindControl("tbGetUsername");

            MembershipUser user = Membership.GetUser(tbUserName.Text);

            if (user != null)
            {
                // set uniqeid on user's profile 
                Guid uniqueID = Guid.NewGuid();
                Users.UpdatePasswordResetKey(user.UserName, uniqueID);

                string url = Globals.CombineUrls(CMS.Context.Publication.PublishedPrimaryUrl, CMS.Context.PageData.Path) + "?username=" + user.UserName + "&uniqueID=" + uniqueID;

                try
                {
                    Messaging.SendMail(user.Email, GetPasswordMailTitle, GetPasswordMailText + ".\n\n" + url);

                    base.Controls.Add(new LiteralControl(GetPasswordMailSent));
                }
                catch (Exception ex)
                {
                    base.Controls.Add(new LiteralControl(ex.Message));
                }
            }
            else
            {
                base.Controls.Add(new LiteralControl(String.Format(UserNotFound, tbUserName.Text)));
            }
        }

        #endregion

        #region Set Password

        private void AddGetPasswordForm()
        {
            GetPasswordForm = new PlaceHolder();
            GetPasswordForm.Controls.Add(new LiteralControl("<p>" + GetPasswordTitleText + "</p>"));
            GetPasswordForm.Controls.Add(new LiteralControl(UserNameLabelText + "<br/>"));
            GetPasswordForm.Controls.Add(new TextBox { ID = "tbGetUsername" });
            GetPasswordForm.Controls.Add(new LiteralControl("<br/>"));

            Button button = new Button { ID = "btnGetSubmit", Text = GetPasswordButtonText };
            button.Click += GetPasswordForm_Click;
            GetPasswordForm.Controls.Add(button);


            GetPasswordForm.Visible = false;
            base.Controls.Add(GetPasswordForm);
        }

        ///// <summary>
        ///// Form for setting new password
        ///// </summary>
        protected void SetPasswordForm_Click(object sender, EventArgs e)
        {
            string username = Page.Request.QueryString["username"];
            MembershipUser user = Membership.GetUser(username);

            TextBox tbPassword = (TextBox)SetPasswordForm.FindControl("tbSetPassword");

            if (user != null)
            {
                string uniqueID = Page.Request.QueryString["uniqueID"];
                Guid key = new Guid(uniqueID);
                if (Users.UpdatePassword(user.UserName, key, tbPassword.Text))
                {
                    base.Controls.Add(new LiteralControl(SetPasswordChanged));
                }
                else
                {
                    base.Controls.Add(new LiteralControl("Ett fel uppstod, försök igen"));
                }
            }
            else
            {
                base.Controls.Add(new LiteralControl(String.Format(UserNotFound, username)));
            }
        }

        #endregion


    }
}
