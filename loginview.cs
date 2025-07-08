using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_cp
{
    internal class loginview : UserControl
    {
        public event EventHandler back_clicked;
        private www www;

        public loginview(www sharedwww)
        {
            InitializeComponent();

            this.www = sharedwww;
        }

        private void InitializeComponent()
        {
            button1 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button2 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(157, 251);
            button1.Name = "button1";
            button1.Size = new Size(194, 66);
            button1.TabIndex = 0;
            button1.Text = "Log in";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(142, 93);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(222, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "Nickname";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(142, 131);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(222, 23);
            textBox2.TabIndex = 2;
            textBox2.Text = "Password";
            // 
            // button2
            // 
            button2.Location = new Point(3, 3);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 3;
            button2.Text = "back";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // loginview
            // 
            Controls.Add(button2);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "loginview";
            Size = new Size(524, 364);
            ResumeLayout(false);
            PerformLayout();

        }
        private Button button1;
        private TextBox textBox1;
        private Button button2;
        private TextBox textBox2;


        private void button2_Click(object sender, EventArgs e)
        {
            back_clicked?.Invoke(this, EventArgs.Empty);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string nickname = textBox1.Text;
            string password = textBox2.Text;
            try
            {
                var cred = www.client.SignInWithEmailAndPasswordAsync($"{nickname}@gmail.com", password);
                var rspns = await (cred);
                
                MessageBox.Show($"succesfully logged in, user uid: {rspns.User.Uid}");
                
                MessageBox.Show($"user is null?  -->{rspns.User == null}");
            }
            catch(FirebaseAuthException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
