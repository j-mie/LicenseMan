using CredentialManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseManLoader
{
    public partial class LoginForm : Form
    {
        public bool UserClosing = false;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(UsernameTextbox.Text) || String.IsNullOrEmpty(PasswordTextbox.Text))
            {
                MessageBox.Show("No username or password provided!!!");
            }
            else
            {
                var cm = new Credential { Target = "LicenseMan", PersistanceType = PersistanceType.Enterprise, Username = UsernameTextbox.Text, Password = PasswordTextbox.Text };
                cm.Save();

                UserClosing = true;
                this.Close();
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    if (!UserClosing)
                    {
                        Environment.Exit(-1);
                    }
                    break;
                default:
                    break;
            }

            UserClosing = false;
        }
    }
}
