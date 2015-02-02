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

                this.Close();
            }
        }
    }
}
