using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_cp
{
    internal class signupview : UserControl
    {
        private Button button1;
        internal TextBox textBox1;
        private Button button2;
        internal TextBox textBox2;
        //tworzymy event na kliknięcie przycisku back
        public event EventHandler back_clicked;
        private www www;
        public signupview(www sharedwww)
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
            button1.Location = new Point(170, 239);
            button1.Name = "button1";
            button1.Size = new Size(194, 66);
            button1.TabIndex = 0;
            button1.Text = "Register";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(170, 76);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(194, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "Nickname";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(170, 143);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(194, 23);
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
            button2.MouseClick += button2_MouseClick;
            // 
            // signupview
            // 
            Controls.Add(button2);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "signupview";
            Size = new Size(524, 364);
            ResumeLayout(false);
            PerformLayout();

        }
        //przywolywujemy publiczny event przy naciskaniu na klawisz back
        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            back_clicked?.Invoke(this, EventArgs.Empty);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string nickname = textBox1.Text;
            string pass = textBox2.Text;
            
            try
            {
                var cred = www.client.CreateUserWithEmailAndPasswordAsync($"{nickname}@gmail.com", pass);
                var user  = await(cred);
            }
            catch (FirebaseAuthException ex)
            {

                MessageBox.Show(ex.Message);

            }
            return;
        }
    }
}
