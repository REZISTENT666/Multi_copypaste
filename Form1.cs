using Firebase.Auth;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.WinApi;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic.Logging;
using Multi_cp;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace pizdec
{
    //definiuje customapplication kontekst ktory nasladuje class applicationcontecst
    public partial class Form1 : Form
    {
        //private login loginform;
        private signupview signup;
        private www www;
        private loginview login;
        private main_page main;
        FirestoreDb db;
        public int count;
        public string indoc;
        public bool islogged;
        internal ContextMenuStrip menu;
        public Form1(www sharedwww)
        {
            this.KeyPreview = true;
            this.www = sharedwww;
            InitializeComponent();
            //dodajemy login
            login = new loginview(www);
            login.Dock = DockStyle.Fill;
            login.Visible = false;

            Controls.Add(login);

            //dodajemy signup jako user control
            signup = new signupview(www);
            signup.Dock = DockStyle.Fill;
            signup.Visible = false;

            Controls.Add(signup);
            //subskrybujemy na publiczny event wywolany w signup.cs
            signup.back_clicked+=Signup_back_clicked;

            login.back_clicked += Login_back_clicked;

            //имею доступ к бек кликед ибо это паблик ивент

            //tworzymy nowy objekt na wzor classy www

            www.client.AuthStateChanged+= on_auth;

            main = new main_page();
            main.Visible = false;
            main.Dock = DockStyle.Fill;
            Controls.Add(main);
            count = 2;
            islogged = false;
            this.KeyDown+=send;
            
            menu = new ContextMenuStrip();
            menu.Items.Add("Delete", null, delete_data);
            menu.Items.Add("Copy", null,copy_data);
        }
        private void copy_data(object sender,EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;
            if(item.Owner is ContextMenuStrip context)
            {
                Control ctrl = context.SourceControl;
                if(ctrl is RichTextBox rtb)
                {
                    try
                    {
                        Clipboard.SetData(DataFormats.Text, rtb.Text);
                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }
        private void delete_data(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;
            if ( item.Owner is  ContextMenuStrip context )
            {
                Control ctrl = context.SourceControl;
                if(ctrl is RichTextBox rtb)
                {
                    try
                    {
                        var doc = this.db.Document($"user_data/{www.client.User.Uid}");
                        var data_to_remove = new Dictionary<string, object>();
                        data_to_remove.Add(rtb.Tag.ToString(), FieldValue.Delete);
                        doc.UpdateAsync(data_to_remove);
                        rtb.Parent.Controls.Remove(rtb);
                        rtb.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }
        private async void on_auth(object sender, UserEventArgs e)
        {
            if (e.User!= null) {
                MessageBox.Show($"Currently authenticated user uid : {e.User.Uid}");
                //invoke bo on_auth nie jest Ui funkcja wiec nie moze zmieniac ui, dlatego uzywam invoke aby przywolac showmain
                try
                {
                    this.db = www.CreateDbWithEmailAndPassword();
                    MessageBox.Show($"Succesfully created db instance with method, db id = {this.db.DatabaseId}");

                    var snapshot = this.db.Document($"user_data/{www.client.User.Uid}").GetSnapshotAsync();
                    await snapshot;
                    if (snapshot.Result.Exists == false )
                    {
                        try
                        {
                            var data = new Dictionary<string, string>() {
                                {"1","Hello there! you can store your pastas here!" }
                            };
                            var creation = this.db.Document($"user_data/{www.client.User.Uid}").CreateAsync(data);
                            await creation;
                        }
                        catch (Exception ex) {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => showmain(e.User.Uid, this.db)));
                }
                else
                {
                    showmain(e.User.Uid, this.db);
                }

            }
        }

        private void RefreshUdata(FirestoreDb db, int count)
        {
            var userdoc = db.Document($"user_data/{www.client.User.Uid}");
            var snap = userdoc.GetSnapshotAsync().Result;
            var userdata = snap.ToDictionary();
            foreach (var key in userdata)
            {
                bool needs_to_be_created = true;
                foreach (Control control in main.flowLayoutPanel1.Controls)
                {
                    if (control is System.Windows.Forms.RichTextBox txt && txt.Tag != null && txt.Tag.ToString() == key.Key) {
                        needs_to_be_created = false;
                    }
                    
                }
                foreach (Control control in main.flowLayoutPanel2.Controls)
                {
                    if (control is System.Windows.Forms.RichTextBox txt && txt.Tag != null && txt.Tag.ToString() == key.Key)
                    {
                        needs_to_be_created = false;
                    }

                }
                if (needs_to_be_created) {
                    var textBox = new System.Windows.Forms.RichTextBox()
                    {
                        Text = key.Value.ToString(),
                        Tag = key.Key,
                        Multiline = true,
                        ReadOnly = true,
                        BorderStyle = BorderStyle.FixedSingle,
                        Width = 175,
                        WordWrap = true,
                        Margin = new Padding(0, 0, 0, 5),
                        Font = new Font("Segoe UI", 9),
                        BackColor = Color.Gray,
                        ForeColor = Color.Black,
                        Cursor = Cursors.Hand,

                    };

                    int textHeight = TextRenderer.MeasureText(
                    textBox.Text,
                    textBox.Font,
                    new Size(textBox.Width, int.MaxValue),
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;

                    textBox.Height = textBox.Height + textHeight;

                    textBox.SelectionChanged+=noselect;
                    textBox.ContextMenuStrip = this.menu;
                    //textBox.Controls.Add

                    if (this.count%2 == 0)
                    {
                        main.flowLayoutPanel1.Controls.Add(textBox);
                    }
                    else if (this.count%2 == 1) { main.flowLayoutPanel2.Controls.Add(textBox); }
                    this.count +=1;

                }
                
            }

        }

        private void noselect(object sender, EventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            richTextBox.SelectionLength = 0;

            richTextBox.MouseDown+=(s, args) =>
            {
                ((RichTextBox)s).SelectionLength = 0;
            };

        }

        private async void showmain(string uid,FirestoreDb db)
        {
            
            main.Visible = true;
            main.BringToFront();
            //pierwszy call aby odrazu zaladowac dane dla uzytkownika, refresh nastepuje kazde 5 sekund
            RefreshUdata(this.db,count);
            try
            {
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 5000;
                timer.Tick += Refreshtick;
                timer.Start();
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            
        }
        private void SubscribeChild(Control control)
        {
            control.KeyDown += send;
            foreach (Control child in control.Controls)
            {
                SubscribeChild(child);
            }
        }
        private async void send(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Scroll && Clipboard.ContainsText() == true)
            {
                MessageBox.Show("ctrl");
                try
                {
                    var userdoc = this.db.Document($"user_data/{www.client.User.Uid}");
                    var snap = userdoc.GetSnapshotAsync().Result;
                    var dict = snap.ToDictionary();
                    if (dict != null && dict.Count != 0)
                    {
                        foreach (var item in dict)
                        {
                            this.indoc = item.Key;
                        }
                    }
                    else { this.indoc = "1"; }
                        int num_of_desired_doc = int.Parse(this.indoc);
                    num_of_desired_doc +=1;
                    string new_key = num_of_desired_doc.ToString();

                    MessageBox.Show(new_key);

                    var data = new Dictionary<string, object>();
                    data.Add(new_key, Clipboard.GetText());

                    var upd = userdoc.UpdateAsync(data);
                    var rsl = await upd;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else {
                 }
        }
        private void Refreshtick(object  sender, EventArgs e)
        {
            RefreshUdata(this.db, count);
        }

        private void Login_back_clicked (object?sender, EventArgs e)
        {
            login.Visible=false;
        }

        private void Signup_back_clicked(object? sender, EventArgs e)
        {
            signup.Visible = false;
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            signup.Visible = true;
            signup.BringToFront();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            login.Visible = true;
            login.BringToFront();
        }
    }


    public class CustomApplicationContext : ApplicationContext
    {
        //przydzielamy type icontainer dla variable components i pozniej przydzielamy components container componentow
        private IContainer components;
        private NotifyIcon icon;
        internal Form1 form1;
        private  www www;
        private void InitializeContext()
        {
            www =  new www();
            //inicjalizujemy form1 aby pozniej pokazac
            form1 = new Form1(www);
            //tworzymy notify icon i inicjalizujemy 
           components = new System.ComponentModel.Container();
            icon = new NotifyIcon(components) {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon("chat-vector-icon_676179-133.ico"),
                Text = "Click <HOME> to call menu",
                Visible = true
                
            };
            
            //dodajemy exit button do toolstripu
            ToolStripMenuItem menuItem1 = new ToolStripMenuItem();
            menuItem1.Text = "Exit";
            icon.ContextMenuStrip.Items.Add(menuItem1);

            //inicjalizujemy funkcje na jakies dzialania usera
            icon.MouseDown += on_ico_click;
            menuItem1.Click += on_menuitem_click;

            //ustawiamy keylogger
            IKeyboardEvents hook = Hook.GlobalEvents();

            hook.KeyDown += onkeydown;
            form1.FormClosing += on_closing;

        }
        private void on_closing(object sender,FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                form1.Hide();
            }
            if (e.CloseReason == CloseReason.FormOwnerClosing)
            {

                form1.Close();
            }
        }
        private void onkeydown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Home)
            {
                if(form1.Visible!= true)
                {
                    form1.Show();
                    form1.BringToFront();
                }
                if (form1.Visible == true) {
                    form1.Activate();
                    form1.BringToFront();
                }
            }
        }

        private void on_menuitem_click(object sender,EventArgs e)
        {
            if (www.client != null && www.client.User != null)
            {
                try {
                    www.client.SignOut();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                if (www.client == null){
                    MessageBox.Show($"idk why the client is null");
                }
                if(www.client.User == null){
                    MessageBox.Show($"idk why the  user is null");
                }
            }
            Application.Exit();
        }
        private void on_ico_click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                icon.ContextMenuStrip.Enabled = true;
                icon.ContextMenuStrip.Show(System.Windows.Forms.Cursor.Position);
            }
            else
            {
                return;
            }
        }
        public CustomApplicationContext() { 

            InitializeContext();
        
        }
        //przy wywolaniu customappcontext wywoluje sie jego konstruktor ktory przywoluje initializecontext 
        // a initializecontext przywiazuje opisane funkcje ktore sa wywolywane na jakiekolwiek czyny z elementami 
    }

}
