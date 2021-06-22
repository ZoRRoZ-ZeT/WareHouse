using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace WindowsFormsApp2
{
    public partial class Main : Form
    {

        Console console;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MYACTION_HOTKEY_ID = 1;

        public Main()
        {
            InitializeComponent();
            isUserPanel = false;
            UserPanel.Hide();
            FullAdminPanel.Hide();
            HideAdminBtn();
            FullAdminPanel.Top = 194;
            UserPanel.Top = 0;
            console = new Console();
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID, 6, (int)Keys.F12);
            SelectedTovars = new List<string>();
            this.label29.Text = "0";
            this.button8.Enabled = false;
            this.button8.BackColor = Color.FromArgb(58, 141, 158);
            FullTovarPanel.Hide();
            FullOwnerPanel.Hide();
            FullTovarPanel.Location = new Point(299, 104);
            FullSellerPanel.Location = new Point(299, 113);
            FullOwnerPanel.Location = new Point(299, 112);
            FullSellerPanel.Hide();
            UserPanel.BringToFront();
            panel11.BringToFront();
            label7.BringToFront();
            label8.BringToFront();
            UserPanelButton.BringToFront();
            textBox = new RichTextBox();
            this.Controls.Add(textBox);
        }

        public string TextUsername { get; set; }
        public string TextName { get; set; }
        public string TextSurname { get; set; }
        public string TextPermission { get; set; }
        private bool isUserPanel { get; set; }
        public string ConnectionString { get; set; }
        public string TextPassword { get; set; }
        public int MyId { get; set; }
        private List<string> SelectedTovars { get; set; }

        private RichTextBox textBox { get; set; }

        private void pictureBox7_Paint_1(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, pictureBox7.ClientRectangle,
                Color.FromArgb(78, 184, 206), 2, ButtonBorderStyle.Solid,
                Color.FromArgb(78, 184, 206), 0, ButtonBorderStyle.Solid,
                Color.FromArgb(78, 184, 206), 0, ButtonBorderStyle.Solid,
                Color.FromArgb(78, 184, 206), 2, ButtonBorderStyle.Solid);
        }



        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == MYACTION_HOTKEY_ID && this.TextPermission == "2")
            {
                if (console.Visible)
                    console.Hide();
                else
                    console.Show();
            }
            base.WndProc(ref m);
        }




        private void ShowAdminBtn() {
            AdminLeft.Show();
            AdminRight.Show();
            AdminBottom.Show();
            AdminBG.Show();
            AdminBtn.Show();
        }

        private void HideAdminBtn()
        {
            AdminLeft.Hide();
            AdminRight.Hide();
            AdminBottom.Hide();
            AdminBG.Hide();
            AdminBtn.Hide();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            if (TextPermission == "0" || TextPermission == "1")
                this.AdminPanel.Hide();
            else
            {
                ShowAdminBtn();
                this.SAdminPanel.Show();
            }

            this.label6.Text = TextUsername;
            this.label7.Text = TextName;
            this.label8.Text = TextSurname;

            console.ConnectionString = this.ConnectionString;

            FillCards("");

            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataAdapter sda = new SqlDataAdapter("SELECT NameOfTovar, ID_Tovar FROM Tovar", con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda2 = new SqlDataAdapter("SELECT TOP 1 'Все товары' AS NameOfTovar, 0 AS ID_Tovar FROM users UNION SELECT NameOfTovar, ID_Tovar FROM Tovar", con);
            DataTable dt2 = new DataTable();
            sda.Fill(dt);
            sda2.Fill(dt2);

            comboBox1.DataSource = dt.AsDataView();
            comboBox1.DisplayMember = "NameOfTovar";
            comboBox1.ValueMember = "ID_Tovar";

            comboBox2.DataSource = dt2.AsDataView();
            comboBox2.DisplayMember = "NameOfTovar";
            comboBox2.ValueMember = "ID_Tovar";

            FillRequestForOwner();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddUsers_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(AddPassword.Text + AddUsername.Text));
            SqlCommand cmd = new SqlCommand("INSERT INTO users(username,password,name,surname) VALUES (@username,@password,@name,@surname)", con);
            cmd.Parameters.AddWithValue("@username", AddUsername.Text);
            cmd.Parameters.AddWithValue("@password", Convert.ToBase64String(hash));
            cmd.Parameters.AddWithValue("@name", AddName.Text);
            cmd.Parameters.AddWithValue("@surname", AddSurname.Text);
            cmd.ExecuteNonQuery();

            AddUsername.Clear();
            AddPassword.Clear();
            AddName.Clear();
            AddSurname.Clear();

            MessageBox.Show("Успешно!");
        }

        private void DeleteUsers_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            SqlCommand cmd = new SqlCommand("UPDATE Seller SET username = 0 WHERE username ='" + RemoveUser.Text + "';" +
                "UPDATE Customer SET username = 0 WHERE username ='" + RemoveUser.Text + "';" +
                "DELETE FROM users WHERE username ='" + RemoveUser.Text + "'", con);
            cmd.ExecuteNonQuery();

            RemoveUser.Clear();
            MessageBox.Show("Успешно!");
        }

        private void EditUsers_Click(object sender, EventArgs e)
        {

            if (!(EditName.Text == "") && !(EditSurname.Text == "") && !(EditUser.Text == ""))
            {
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE users SET name='" + EditName.Text + "', surname='" + EditSurname.Text + "' WHERE username='" + EditUser.Text + "'", con);
                cmd.ExecuteNonQuery();

                if (EditUser.Text == TextUsername)
                {
                    this.label7.Text = EditName.Text;
                    this.label8.Text = EditSurname.Text;
                }

                EditName.Clear();
                EditSurname.Clear();
                EditUser.Clear();


                MessageBox.Show("Успешно!");
            }
            else
            {
                MessageBox.Show("Ошибка!Не все поля введены!");
            }
        }



        private void ChangePerm_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            SqlCommand cmd = new SqlCommand("UPDATE users SET isAdmin='" + PermValue.Text + "' WHERE username='" + PermLogin.Text + "'", con);
            cmd.ExecuteNonQuery();

            PermLogin.Clear();
            PermValue.Clear();

            MessageBox.Show("Успешно!");
        }

        private void ViewUsers_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            SqlDataAdapter users = new SqlDataAdapter("SELECT username,name,surname,isAdmin FROM users", con);
            DataTable dt = new DataTable();

            users.Fill(dt);


            List<TextBox> usersBox = new List<TextBox>();

            usersBox.Add(ListBoxU);
            usersBox.Add(ListBoxName);
            usersBox.Add(ListBoxSurname);
            usersBox.Add(ListBoxPerm);
            usersBox[0].Text = "Users\r\n" +
                       "------------------------\r\n";
            usersBox[1].Text = "Name\r\n" +
                       "------------------------\r\n";
            usersBox[2].Text = "Surname\r\n" +
                       "------------------------\r\n";
            usersBox[3].Text = "isAdmin\r\n" +
                       "------------------------\r\n";
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                for (int j = 0; j < dt.Columns.Count; ++j)
                {
                    usersBox[j].Text += dt.Rows[i][j].ToString() + "\r\n";
                }
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.quitMenuH1;
            this.label5.ForeColor = Color.FromArgb(78, 184, 206);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.quitMenu;
            this.label5.ForeColor = Color.WhiteSmoke;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {
            label1.Focus();
            ChangePass Change = new ChangePass();
            Change.Username = this.TextUsername;
            Change.Password = this.TextPassword;
            Change.ConnectionString = this.ConnectionString;
            Change.Show();
            Change.Left = this.Left + (this.Size.Width - Change.Size.Width) / 2;
            Change.Top = this.Top + (this.Size.Height - Change.Size.Height) / 2;
            this.Enabled = false;
            this.Focus();
            UserPanel.Hide();
            isUserPanel = false;

            Change.FormClosing += new FormClosingEventHandler(AnotherForm_closing);
        }

        public void AnotherForm_closing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = true;
        }

        private void UserPanelButton_Click(object sender, EventArgs e)
        {
            if (!isUserPanel)
            {
                UserPanel.Show();
                if (TextPermission == "2")
                {
                    this.label7.ForeColor = Color.Red;
                    this.label8.ForeColor = Color.Red;
                }
                isUserPanel = true;
            }
            else
            {
                UserPanel.Hide();
                isUserPanel = false;
            }
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.quitMenuH1;
            this.label5.ForeColor = Color.FromArgb(78, 184, 206);
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.quitMenu;
            this.label5.ForeColor = Color.WhiteSmoke;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FullAdminPanel.Show();
            FullTovarPanel.Hide();
            FullOwnerPanel.Hide();
            textBox.Hide();
        }

        private void FillCards(string command)
        {
            TovarPanel.Controls.Clear();

            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlDataAdapter countTovars = new SqlDataAdapter("SELECT COUNT(*) FROM Tovar WHERE CountOnSklad > 0 " + command, con);
            SqlDataAdapter infoTovars = new SqlDataAdapter("SELECT NameOfTovar, NameType, CountOnSklad, Avg_Price, ID_Tovar FROM Tovar WHERE CountOnSklad > 0 " + command, con);
            DataTable countDt = new DataTable();
            DataTable infoDt = new DataTable();

            countTovars.Fill(countDt);
            infoTovars.Fill(infoDt);

            PictureBox[] pictureBoxes = new PictureBox[Convert.ToInt16(countDt.Rows[0][0])];
            Label[,] labels = new Label[Convert.ToInt16(countDt.Rows[0][0]), 6];
            Button[] buttons = new Button[Convert.ToInt16(countDt.Rows[0][0])];



            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if ((i * 4 + j + 1) > Convert.ToInt16(countDt.Rows[0][0]))
                        break;

                    pictureBoxes[i * 4 + j] = new PictureBox()
                    {
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Location = new System.Drawing.Point(36 + (j * 150), 30 + (i * 180)),
                        Name = "picturebox" + ((i + 1) * 10 + (j + 1)).ToString(),
                        Size = new System.Drawing.Size(142, 162),
                        TabIndex = 64,
                        TabStop = false
                    };
                    this.Controls.Add(pictureBoxes[i * 4 + j]);
                    TovarPanel.Controls.Add(pictureBoxes[i * 4 + j]);

                    labels[(i * 4 + j), 0] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label" + ((i + 1) * 10 + (j + 1)).ToString() + "1",
                        Size = new System.Drawing.Size(67, 21),
                        TabIndex = 59,
                        Text = infoDt.Rows[i * 4 + j][0].ToString()
                    };
                    labels[(i * 4 + j), 0].Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + (pictureBoxes[i * 4 + j].Width - labels[(i * 4 + j), 0].Bounds.X) / 8, pictureBoxes[i * 4 + j].Top + 18);
                    this.Controls.Add(labels[(i * 4 + j), 0]);
                    TovarPanel.Controls.Add(labels[(i * 4 + j), 0]);
                    labels[(i * 4 + j), 0].BringToFront();

                    labels[(i * 4 + j), 1] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + 21, pictureBoxes[i * 4 + j].Top + 40),
                        Name = "label" + ((i + 1) * 10 + (j + 1)).ToString() + "2",
                        Size = new System.Drawing.Size(97, 21),
                        TabIndex = 65,
                        Text = "Тип: " + infoDt.Rows[i * 4 + j][1].ToString()
                    };
                    this.Controls.Add(labels[(i * 4 + j), 1]);
                    TovarPanel.Controls.Add(labels[(i * 4 + j), 1]);
                    labels[(i * 4 + j), 1].BringToFront();


                    labels[(i * 4 + j), 2] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + 5, pictureBoxes[i * 4 + j].Top + 72),
                        Name = "label" + ((i + 1) * 10 + (j + 1)).ToString() + "3",
                        Size = new System.Drawing.Size(82, 19),
                        TabIndex = 66,
                        Text = "На складе: "
                    };
                    this.Controls.Add(labels[(i * 4 + j), 2]);
                    TovarPanel.Controls.Add(labels[(i * 4 + j), 2]);
                    labels[(i * 4 + j), 2].BringToFront();

                    labels[(i * 4 + j), 3] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + 93, pictureBoxes[i * 4 + j].Top + 72),
                        Name = "label" + ((i + 1) * 10 + (j + 1)).ToString() + "4",
                        Size = new System.Drawing.Size(33, 19),
                        TabIndex = 67,
                        Text = infoDt.Rows[i * 4 + j][2].ToString()
                    };
                    this.Controls.Add(labels[(i * 4 + j), 3]);
                    TovarPanel.Controls.Add(labels[(i * 4 + j), 3]);
                    labels[(i * 4 + j), 3].BringToFront();

                    labels[(i * 4 + j), 4] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + 5, pictureBoxes[i * 4 + j].Top + 97),
                        Name = "label" + ((i + 1) * 10 + (j + 1)).ToString() + "5",
                        Size = new System.Drawing.Size(46, 19),
                        TabIndex = 68,
                        Text = "Цена:"
                    };
                    this.Controls.Add(labels[(i * 4 + j), 4]);
                    TovarPanel.Controls.Add(labels[(i * 4 + j), 4]);
                    labels[(i * 4 + j), 4].BringToFront();

                    labels[(i * 4 + j), 5] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(22)))), ((int)(((byte)(31))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + 53, pictureBoxes[i * 4 + j].Top + 97),
                        Name = "label" + ((i + 1) * 10 + (j)).ToString() + "6",
                        Size = new System.Drawing.Size(60, 19),
                        TabIndex = 69,
                        Text = infoDt.Rows[i * 4 + j][3].ToString() + " грн"
                    };
                    this.Controls.Add(labels[(i * 4 + j), 5]);
                    TovarPanel.Controls.Add(labels[(i * 4 + j), 5]);
                    labels[(i * 4 + j), 5].BringToFront();


                    buttons[i * 4 + j] = new Button()
                    {
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j].Left + 30, pictureBoxes[i * 4 + j].Top + 132),
                        Name = "button " + infoDt.Rows[i * 4 + j][0].ToString() + " " + infoDt.Rows[i * 4 + j][2].ToString() + " "
                        + infoDt.Rows[i * 4 + j][3].ToString() + " " + infoDt.Rows[i * 4 + j][4].ToString(),
                        Size = new System.Drawing.Size(82, 21),
                        TabIndex = 70,
                        Text = "Добавить",
                        UseVisualStyleBackColor = false
                    };
                    this.Controls.Add(buttons[i * 4 + j]);
                    TovarPanel.Controls.Add(buttons[i * 4 + j]);
                    buttons[i * 4 + j].BringToFront();
                    buttons[i * 4 + j].Click += new System.EventHandler(this.buttonTovar_Click);
                    string[] infos = buttons[i * 4 + j].Name.Split();
                    for (int k = 0; k < SelectedTovars.Count; k++)
                    {
                        if (infos[infos.Length - 1] == SelectedTovars[k])
                        {
                            buttons[i * 4 + j].Enabled = false;
                            buttons[i * 4 + j].BackColor = Color.FromArgb(58, 141, 158);
                        }
                    }

                }
            }
        }



        private void FillSelectedCards()
        {
            if (SelectedTovars.Count == 0)
            {
                SelectedTovarPanel.Controls.Clear();
                return;
            }
            string[] _infosCount = new string[SelectedTovars.Count + 1];
            string[] _infosId = new string[SelectedTovars.Count + 1];
            int count = 0;
            for (int i = 0; i < SelectedTovarPanel.Controls.Count; ++i)
            {
                if (SelectedTovarPanel.Controls[i].GetType() == typeof(TextBox))
                {
                    string[] _infos2 = SelectedTovarPanel.Controls[i].Name.Split();
                    _infosId[count] = _infos2[_infos2.Length - 3];
                    _infosCount[count] = SelectedTovarPanel.Controls[i].Text;
                    count++;
                }
            }
            SelectedTovarPanel.Controls.Clear();
            PictureBox[] pictureBoxes = new PictureBox[SelectedTovars.Count];
            Label[,] labels = new Label[SelectedTovars.Count, 4];
            Button[] buttons = new Button[SelectedTovars.Count];
            TextBox[] textBoxes = new TextBox[SelectedTovars.Count];

            SqlConnection con = new SqlConnection(this.ConnectionString);
            string command = "ID_tovar IN(";
            for (int i = 0; i < SelectedTovars.Count; ++i)
            {
                if (i != SelectedTovars.Count - 1)
                    command += SelectedTovars[i] + ", ";
                else
                    command += SelectedTovars[i] + ")";
            }
            SqlDataAdapter infoTovars = new SqlDataAdapter("SELECT NameOfTovar, NameType, CountOnSklad, Avg_Price, ID_Tovar FROM Tovar WHERE " + command, con);
            DataTable infoDt = new DataTable();

            infoTovars.Fill(infoDt);

            for (int i = 0; i < SelectedTovars.Count; ++i)
            {
                pictureBoxes[i] = new PictureBox()
                {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Location = new System.Drawing.Point(16, 13 + (i * 75)),
                    Name = "picturebox2__" + (i).ToString(),
                    Size = new System.Drawing.Size(438, 60),
                    TabIndex = 79,
                    TabStop = false
                };
                this.Controls.Add(pictureBoxes[i]);
                SelectedTovarPanel.Controls.Add(pictureBoxes[i]);
                pictureBoxes[i].SendToBack();

                labels[i, 0] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label2__" + (i).ToString() + "1",
                    Size = new System.Drawing.Size(67, 21),
                    TabIndex = 59,
                    Text = infoDt.Rows[i][0].ToString()
                };
                labels[i, 0].Location = new System.Drawing.Point(pictureBoxes[i].Left + 11, pictureBoxes[i].Top + 10);
                this.Controls.Add(labels[i, 0]);
                SelectedTovarPanel.Controls.Add(labels[i, 0]);
                labels[i, 0].BringToFront();

                labels[i, 1] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label2__" + (i).ToString() + "2",
                    Size = new System.Drawing.Size(67, 21),
                    TabIndex = 59,
                    Text = "На складе"
                };
                labels[i, 1].Location = new System.Drawing.Point(pictureBoxes[i].Left + 12, pictureBoxes[i].Top + 33);
                this.Controls.Add(labels[i, 1]);
                SelectedTovarPanel.Controls.Add(labels[i, 1]);
                labels[i, 1].BringToFront();


                labels[i, 2] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label2__" + (i).ToString() + "3",
                    Size = new System.Drawing.Size(67, 21),
                    TabIndex = 59,
                    Text = infoDt.Rows[i][2].ToString()
                };
                labels[i, 2].Location = new System.Drawing.Point(pictureBoxes[i].Left + 76, pictureBoxes[i].Top + 33);
                this.Controls.Add(labels[i, 2]);
                SelectedTovarPanel.Controls.Add(labels[i, 2]);
                labels[i, 2].BringToFront();

                labels[i, 3] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label2__" + (i).ToString() + "4",
                    Size = new System.Drawing.Size(67, 21),
                    TabIndex = 59,
                    Text = "Хочу купить:"
                };
                labels[i, 3].Location = new System.Drawing.Point(pictureBoxes[i].Left + 173, pictureBoxes[i].Top + 29);
                this.Controls.Add(labels[i, 3]);
                SelectedTovarPanel.Controls.Add(labels[i, 3]);
                labels[i, 3].BringToFront();

                textBoxes[i] = new TextBox()
                {
                    BackColor = System.Drawing.Color.Black,
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                    Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    HideSelection = false,
                    Name = "textBox2__" + " " + infoDt.Rows[i][4].ToString() + " " + infoDt.Rows[i][3].ToString() + " " + infoDt.Rows[i][2].ToString(),
                    Size = new System.Drawing.Size(74, 25),
                    TabIndex = 82,
                    Text = "0",
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Center
                };
                textBoxes[i].Location = new System.Drawing.Point(pictureBoxes[i].Left + 296, pictureBoxes[i].Top + 29);
                string _info = infoDt.Rows[i][4].ToString();
                for (int m = 0; m < SelectedTovars.Count + 1; m++)
                {
                    if (_info == _infosId[m])
                    {
                        textBoxes[i].Text = _infosCount[m];
                    }
                }

                textBoxes[i].TextChanged += new System.EventHandler(this.textBoxes_TextChanged);
                this.Controls.Add(textBoxes[i]);
                SelectedTovarPanel.Controls.Add(textBoxes[i]);
                textBoxes[i].BringToFront();

                buttons[i] = new Button()
                {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "buttonSelected " + infoDt.Rows[i][4].ToString(),
                    Size = new System.Drawing.Size(20, 22),
                    TabIndex = 84,
                    Text = "Х",
                    UseVisualStyleBackColor = false
                };
                buttons[i].Location = new System.Drawing.Point(pictureBoxes[i].Left + 412, pictureBoxes[i].Top + 5);
                buttons[i].Click += new System.EventHandler(this.buttonSelectedTovar_Click);
                this.Controls.Add(buttons[i]);
                SelectedTovarPanel.Controls.Add(buttons[i]);
                buttons[i].BringToFront();
            }

            SelectedTovarPanel.Show();

        }

        private void FillSellerRequests()
        {
            MyRequestsPanel.Controls.Clear();

            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlDataAdapter countRequests = new SqlDataAdapter("SELECT COUNT(*) FROM HistoryOfCosts WHERE ID_Seller = " + MyId.ToString(), con);
            SqlDataAdapter infoRequests = new SqlDataAdapter("SELECT NameOfTovar, Price, CountSuggestion, isActive, HistoryOfCosts.ID_Tovar FROM HistoryOfCosts " +
                "INNER JOIN Tovar ON Tovar.ID_tovar = HistoryOfCosts.ID_Tovar WHERE ID_Seller = " + MyId.ToString(), con);
            DataTable countDt = new DataTable();
            DataTable infoDt = new DataTable();

            countRequests.Fill(countDt);
            infoRequests.Fill(infoDt);

            PictureBox[,] pictureBoxes = new PictureBox[Convert.ToInt16(countDt.Rows[0][0]), 2];
            Label[,] labels = new Label[Convert.ToInt16(countDt.Rows[0][0]), 8];

            string ActiveStatus;

            for (int i = 0; i < Convert.ToInt16(countDt.Rows[0][0]); i++)
            {
                if (infoDt.Rows[i][3].ToString() == "True")
                    ActiveStatus = "Активна";
                else
                    ActiveStatus = "Не активна";

                pictureBoxes[i, 0] = new PictureBox()
                {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Location = new System.Drawing.Point(10, 12 + (i * 80)),
                    Name = "pictureBox3___" + i.ToString(),
                    Size = new System.Drawing.Size(616, 67),
                    TabIndex = 91,
                    TabStop = false
                };
                this.Controls.Add(pictureBoxes[i, 0]);
                MyRequestsPanel.Controls.Add(pictureBoxes[i, 0]);

                pictureBoxes[i, 1] = new PictureBox()
                {
                    BackColor = System.Drawing.Color.Black,
                    Location = new System.Drawing.Point(pictureBoxes[i, 0].Left - 2, pictureBoxes[i, 0].Top - 2),
                    Name = "pictureBox3__1" + i.ToString(),
                    Size = new System.Drawing.Size(620, 71),
                    TabIndex = 103,
                    TabStop = false
                };
                this.Controls.Add(pictureBoxes[i, 1]);
                MyRequestsPanel.Controls.Add(pictureBoxes[i, 1]);
                pictureBoxes[i, 1].SendToBack();

                labels[i, 0] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Location = new System.Drawing.Point(23, 21),
                    Name = "label31" + i.ToString(),
                    Size = new System.Drawing.Size(142, 23),
                    TabIndex = 95,
                    Text = "Статус заявки:"
                };
                labels[i, 0].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 13, pictureBoxes[i, 0].Top + 9);
                this.Controls.Add(labels[i, 0]);
                MyRequestsPanel.Controls.Add(labels[i, 0]);
                labels[i, 0].BringToFront();

                labels[i, 1] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label32" + i.ToString(),
                    Size = new System.Drawing.Size(68, 23),
                    TabIndex = 98,
                    Text = ActiveStatus
                };
                labels[i, 1].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 168, pictureBoxes[i, 0].Top + 9);
                this.Controls.Add(labels[i, 1]);
                MyRequestsPanel.Controls.Add(labels[i, 1]);
                labels[i, 1].BringToFront();

                labels[i, 2] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label33" + i.ToString(),
                    Size = new System.Drawing.Size(51, 19),
                    TabIndex = 96,
                    Text = "Товар:"
                };
                labels[i, 2].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 15, pictureBoxes[i, 0].Top + 36);
                this.Controls.Add(labels[i, 2]);
                MyRequestsPanel.Controls.Add(labels[i, 2]);
                labels[i, 2].BringToFront();

                labels[i, 3] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label34" + i.ToString(),
                    Size = new System.Drawing.Size(56, 19),
                    TabIndex = 97,
                    Text = infoDt.Rows[i][0].ToString()
                };
                labels[i, 3].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 72, pictureBoxes[i, 0].Top + 36);
                this.Controls.Add(labels[i, 3]);
                MyRequestsPanel.Controls.Add(labels[i, 3]);
                labels[i, 3].BringToFront();

                labels[i, 4] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label35" + i.ToString(),
                    Size = new System.Drawing.Size(43, 19),
                    TabIndex = 101,
                    Text = "Цена:"
                };
                labels[i, 4].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 459, pictureBoxes[i, 0].Top + 12);
                this.Controls.Add(labels[i, 4]);
                MyRequestsPanel.Controls.Add(labels[i, 4]);
                labels[i, 4].BringToFront();

                labels[i, 5] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label36" + i.ToString(),
                    Size = new System.Drawing.Size(60, 19),
                    TabIndex = 102,
                    Text = infoDt.Rows[i][1].ToString() + " грн"
                };
                labels[i, 5].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 516, pictureBoxes[i, 0].Top + 12);
                this.Controls.Add(labels[i, 5]);
                MyRequestsPanel.Controls.Add(labels[i, 5]);
                labels[i, 5].BringToFront();

                labels[i, 6] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label37" + i.ToString(),
                    Size = new System.Drawing.Size(72, 19),
                    TabIndex = 99,
                    Text = "Осталось"
                };
                labels[i, 6].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 459, pictureBoxes[i, 0].Top + 36);
                this.Controls.Add(labels[i, 6]);
                MyRequestsPanel.Controls.Add(labels[i, 6]);
                labels[i, 6].BringToFront();

                labels[i, 7] = new Label()
                {
                    AutoSize = true,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                    Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Name = "label38" + i.ToString(),
                    Size = new System.Drawing.Size(41, 19),
                    TabIndex = 100,
                    Text = infoDt.Rows[i][2].ToString()
                };
                labels[i, 7].Location = new System.Drawing.Point(pictureBoxes[i, 0].Left + 537, pictureBoxes[i, 0].Top + 36);
                this.Controls.Add(labels[i, 7]);
                MyRequestsPanel.Controls.Add(labels[i, 7]);
                labels[i, 7].BringToFront();

            }
        }

        private void FillRequestForOwner()
        {
            OwnerRequestPanel.Controls.Clear();

            string command, command2;
            if (comboBox2.SelectedValue.ToString() == "0")
            {
                command = "ID_Tovar > 0";
                command2 = "H1.ID_Tovar > 0";
            }
            else
            {
                command = "ID_Tovar = " + comboBox2.SelectedValue.ToString();
                command2 = "H1.ID_Tovar = " + comboBox2.SelectedValue.ToString();
            }

            SqlConnection con = new SqlConnection(this.ConnectionString);
            SqlDataAdapter countRequests = new SqlDataAdapter("SELECT COUNT(*) FROM HistoryOfCosts WHERE " + command + " AND " +
                "Price BETWEEN " + textBox4.Text + " AND " + textBox5.Text + " AND isActive = 1", con);
            SqlDataAdapter infoRequests = new SqlDataAdapter(
                "SELECT f1.NameOfFirma, H1.Price, H1.CountSuggestion, T1.NameOfTovar, H1.ID_Tovar, H1.ID_Seller, H1.Date " +
                    "FROM((HistoryOfCosts H1 INNER JOIN Tovar T1 ON H1.ID_Tovar = T1.ID_Tovar) INNER JOIN Seller S1 ON S1.ID_Seller = H1.ID_Seller) " +
                    "INNER JOIN Firma F1 ON F1.ID_Firma = S1.ID_Firma " +
                    "WHERE " + command2 + " AND " +
                    "H1.Price BETWEEN " + textBox4.Text + " AND " + textBox5.Text + " AND H1.isActive = 1", con);
            DataTable countDt = new DataTable();
            DataTable infoDt = new DataTable();

            countRequests.Fill(countDt);
            infoRequests.Fill(infoDt);

            PictureBox[,] pictureBoxes = new PictureBox[Convert.ToInt16(countDt.Rows[0][0]), 2];
            Label[,] labels = new Label[Convert.ToInt16(countDt.Rows[0][0]), 11];
            TextBox[] textBoxes = new TextBox[Convert.ToInt16(countDt.Rows[0][0])];
            Panel[] panels = new Panel[Convert.ToInt16(countDt.Rows[0][0])];
            CheckBox[] checkBoxes = new CheckBox[Convert.ToInt16(countDt.Rows[0][0])];
            Button[] buttons = new Button[Convert.ToInt16(countDt.Rows[0][0])];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if ((i * 4 + j + 1) > Convert.ToInt16(countDt.Rows[0][0]))
                        break;

                    pictureBoxes[i * 4 + j, 0] = new PictureBox()
                    {
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Location = new System.Drawing.Point(15 + (j * 305), 9 + (i * 300)),
                        Name = "pictureBox14___" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(294, 277),
                        TabIndex = 110,
                        TabStop = false
                    };
                    this.Controls.Add(pictureBoxes[i * 4 + j, 0]);
                    OwnerRequestPanel.Controls.Add(pictureBoxes[i * 4 + j, 0]);

                    pictureBoxes[i * 4 + j, 1] = new PictureBox()
                    {
                        BackColor = System.Drawing.Color.Black,
                        Location = new System.Drawing.Point(pictureBoxes[i * 4 + j, 0].Left - 2, pictureBoxes[i * 4 + j, 0].Top - 2),
                        Name = "pictureBox14__1" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(298, 281),
                        TabIndex = 121,
                        TabStop = false
                    };
                    this.Controls.Add(pictureBoxes[i * 4 + j, 1]);
                    OwnerRequestPanel.Controls.Add(pictureBoxes[i * 4 + j, 1]);
                    pictureBoxes[i * 4 + j, 1].SendToBack();

                    //--------------111---------------//15 9
                    labels[(i * 4 + j), 0] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label43" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(146, 21),
                        TabIndex = 106,
                        Text = "Предложение от:"
                    };
                    labels[(i * 4 + j), 0].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 14, pictureBoxes[(i * 4 + j), 0].Top + 20);
                    this.Controls.Add(labels[(i * 4 + j), 0]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 0]);
                    labels[(i * 4 + j), 0].BringToFront();

                    //--------------222---------------//15 9
                    labels[(i * 4 + j), 1] = new Label()
                    {
                        //AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label44" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(120, 21),
                        TabIndex = 107,
                        Text = infoDt.Rows[(i * 4 + j)][0].ToString()
                    };
                    labels[(i * 4 + j), 1].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 170, pictureBoxes[(i * 4 + j), 0].Top + 24);
                    this.Controls.Add(labels[(i * 4 + j), 1]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 1]);
                    labels[(i * 4 + j), 1].BringToFront();

                    //--------------333---------------//15 9
                    labels[(i * 4 + j), 2] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label45" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(51, 19),
                        TabIndex = 108,
                        Text = "Товар:"
                    };
                    labels[(i * 4 + j), 2].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 16, pictureBoxes[(i * 4 + j), 0].Top + 67);
                    this.Controls.Add(labels[(i * 4 + j), 2]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 2]);
                    labels[(i * 4 + j), 2].BringToFront();

                    //--------------444---------------// 15 9
                    labels[(i * 4 + j), 3] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label46" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(82, 19),
                        TabIndex = 109,
                        Text = infoDt.Rows[(i * 4 + j)][3].ToString()
                    };
                    labels[(i * 4 + j), 3].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 73, pictureBoxes[(i * 4 + j), 0].Top + 67);
                    this.Controls.Add(labels[(i * 4 + j), 3]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 3]);
                    labels[(i * 4 + j), 3].BringToFront();

                    //--------------555---------------// 15 9
                    labels[(i * 4 + j), 4] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label50" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(46, 19),
                        TabIndex = 113,
                        Text = "Цена:"
                    };
                    labels[(i * 4 + j), 4].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 169, pictureBoxes[(i * 4 + j), 0].Top + 67);
                    this.Controls.Add(labels[(i * 4 + j), 4]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 4]);
                    labels[(i * 4 + j), 4].BringToFront();

                    //--------------666---------------// 15 9
                    labels[(i * 4 + j), 5] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label49" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(52, 19),
                        TabIndex = 114,
                        Text = infoDt.Rows[(i * 4 + j)][1].ToString() + " грн"
                    };
                    labels[(i * 4 + j), 5].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 218, pictureBoxes[(i * 4 + j), 0].Top + 67);
                    this.Controls.Add(labels[(i * 4 + j), 5]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 5]);
                    labels[(i * 4 + j), 5].BringToFront();

                    //--------------777---------------//15 9
                    labels[(i * 4 + j), 6] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label48" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(92, 19),
                        TabIndex = 111,
                        Text = "Количество:"
                    };
                    labels[(i * 4 + j), 6].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 63, pictureBoxes[(i * 4 + j), 0].Top + 104);
                    this.Controls.Add(labels[(i * 4 + j), 6]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 6]);
                    labels[(i * 4 + j), 6].BringToFront();

                    //--------------888---------------//15 9
                    labels[(i * 4 + j), 7] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label47" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(33, 19),
                        TabIndex = 112,
                        Text = infoDt.Rows[(i * 4 + j)][2].ToString()
                    };
                    labels[(i * 4 + j), 7].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 161, pictureBoxes[(i * 4 + j), 0].Top + 104);
                    this.Controls.Add(labels[(i * 4 + j), 7]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 7]);
                    labels[(i * 4 + j), 7].BringToFront();

                    //--------------999---------------//15 9
                    labels[(i * 4 + j), 8] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label51" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(73, 19),
                        TabIndex = 115,
                        Text = "Закупить:"
                    };
                    labels[(i * 4 + j), 8].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 16, pictureBoxes[(i * 4 + j), 0].Top + 143);
                    this.Controls.Add(labels[(i * 4 + j), 8]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 8]);
                    labels[(i * 4 + j), 8].BringToFront();

                    //-------------10-10-10---------------//15 9
                    labels[(i * 4 + j), 9] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label52" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(119, 19),
                        TabIndex = 117,
                        Text = "Итоговая сумма:"
                    };
                    labels[(i * 4 + j), 9].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 165, pictureBoxes[(i * 4 + j), 0].Top + 185);
                    this.Controls.Add(labels[(i * 4 + j), 9]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 9]);
                    labels[(i * 4 + j), 9].BringToFront();

                    //-------------11-11-11---------------//
                    labels[(i * 4 + j), 10] = new Label()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "label53 " + infoDt.Rows[(i * 4 + j)][1].ToString() + " " + infoDt.Rows[(i * 4 + j)][2].ToString() + " " + infoDt.Rows[(i * 4 + j)][4].ToString() + " " + infoDt.Rows[(i * 4 + j)][5].ToString() + " " + infoDt.Rows[(i * 4 + j)][6].ToString(),
                        Size = new System.Drawing.Size(53, 15),
                        TabIndex = 118,
                        Text = "0 грн"
                    };
                    labels[(i * 4 + j), 10].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 201, pictureBoxes[(i * 4 + j), 0].Top + 207);
                    this.Controls.Add(labels[(i * 4 + j), 10]);
                    OwnerRequestPanel.Controls.Add(labels[(i * 4 + j), 10]);
                    labels[(i * 4 + j), 10].BringToFront();


                    textBoxes[i * 4 + j] = new TextBox()
                    {
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        BorderStyle = System.Windows.Forms.BorderStyle.None,
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "textBox6 " + infoDt.Rows[(i * 4 + j)][1].ToString() + " " + infoDt.Rows[(i * 4 + j)][2].ToString() + " " + infoDt.Rows[(i * 4 + j)][4].ToString() + " " + infoDt.Rows[(i * 4 + j)][5].ToString() + " " + infoDt.Rows[(i * 4 + j)][6].ToString(),
                        Size = new System.Drawing.Size(64, 13),
                        TabIndex = 116,
                        Text = "0",
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Center
                    };
                    textBoxes[i * 4 + j].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 108, pictureBoxes[(i * 4 + j), 0].Top + 147);
                    this.Controls.Add(textBoxes[i * 4 + j]);
                    OwnerRequestPanel.Controls.Add(textBoxes[i * 4 + j]);
                    textBoxes[i * 4 + j].BringToFront();

                    textBoxes[i * 4 + j].TextChanged += new System.EventHandler(TextBoxes_ChangedReq);

                    panels[i * 4 + j] = new Panel()
                    {
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "panel27" + (i * 4 + j).ToString(),
                        Size = new System.Drawing.Size(60, 1),
                        TabIndex = 102
                    };
                    panels[i * 4 + j].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 110, pictureBoxes[(i * 4 + j), 0].Top + 162);
                    this.Controls.Add(panels[i * 4 + j]);
                    OwnerRequestPanel.Controls.Add(panels[i * 4 + j]);
                    panels[i * 4 + j].BringToFront();

                    checkBoxes[i * 4 + j] = new CheckBox()
                    {
                        AutoSize = true,
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(36)))), ((int)(((byte)(49))))),
                        Checked = false,
                        CheckState = System.Windows.Forms.CheckState.Unchecked,
                        FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                        ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        Name = "checkBox1 " + infoDt.Rows[(i * 4 + j)][1].ToString() + " " + infoDt.Rows[(i * 4 + j)][2].ToString() + " " + infoDt.Rows[(i * 4 + j)][4].ToString() + " " + infoDt.Rows[(i * 4 + j)][5].ToString() + " " + infoDt.Rows[(i * 4 + j)][6].ToString(),
                        Size = new System.Drawing.Size(94, 17),
                        TabIndex = 119,
                        Text = "Подтверждаю",
                        UseVisualStyleBackColor = false
                    };
                    checkBoxes[i * 4 + j].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 181, pictureBoxes[(i * 4 + j), 0].Top + 146);
                    this.Controls.Add(checkBoxes[i * 4 + j]);
                    OwnerRequestPanel.Controls.Add(checkBoxes[i * 4 + j]);
                    checkBoxes[i * 4 + j].BringToFront();

                    buttons[i * 4 + j] = new Button()
                    {
                        BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                        FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                        Name = "button10 " + infoDt.Rows[(i * 4 + j)][1].ToString() + " " + infoDt.Rows[(i * 4 + j)][2].ToString() + " " + infoDt.Rows[(i * 4 + j)][4].ToString() + " " + infoDt.Rows[(i * 4 + j)][5].ToString() + " " + infoDt.Rows[(i * 4 + j)][6].ToString(),
                        Size = new System.Drawing.Size(139, 23),
                        TabIndex = 120,
                        Text = "Оплатить",
                        UseVisualStyleBackColor = false
                    };
                    buttons[i * 4 + j].Location = new System.Drawing.Point(pictureBoxes[(i * 4 + j), 0].Left + 68, pictureBoxes[(i * 4 + j), 0].Top + 237);
                    this.Controls.Add(buttons[i * 4 + j]);
                    OwnerRequestPanel.Controls.Add(buttons[i * 4 + j]);
                    buttons[i * 4 + j].BringToFront();

                    buttons[i * 4 + j].Click += new System.EventHandler(ButtonRequests_Click);
                }
            }

        }

        private void TextBoxes_ChangedReq(object sender, EventArgs e)
        {
            //MessageBox.Show((sender as TextBox).Text);
            string[] thisText = (sender as TextBox).Name.Split();
            string[] infos;
            int d;
            if (Int32.TryParse((sender as TextBox).Text, out d))
            {
                d = Convert.ToInt16((sender as TextBox).Text);
                if (d > Convert.ToInt16(thisText[2]))
                    d = Convert.ToInt16(thisText[2]);
                (sender as TextBox).Text = d.ToString();
                (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;

            }
            else
            {
                (sender as TextBox).Text = "0";
                (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
            }
            for (int i = 0; i < OwnerRequestPanel.Controls.Count; ++i)
            {
                if (OwnerRequestPanel.Controls[i].GetType() == typeof(Label))
                {
                    infos = OwnerRequestPanel.Controls[i].Name.Split();
                    try
                    {
                        if (infos[1] == thisText[1] && infos[2] == thisText[2] && infos[3] == thisText[3] && infos[4] == thisText[4] && infos[5] == thisText[5] && infos[6] == thisText[6])
                        {
                            OwnerRequestPanel.Controls[i].Text = (Convert.ToInt16(thisText[1]) * Convert.ToInt16((sender as TextBox).Text)).ToString() + " грн";
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    

                }
            }
        }

        private void ButtonRequests_Click(object sender, EventArgs e)
        {
            bool stop1 = false;
            bool stop2 = false; ;
            string[] infos = (sender as Button).Name.Split();
            string[] infos2;
            int idTovar = 0;
            int idSeller = 0;
            int getCount = 0;
            string date = "";
            for (int i = 0; i < OwnerRequestPanel.Controls.Count; ++i)
            {
                if (stop1 && stop2)
                    break;
                if(OwnerRequestPanel.Controls[i].GetType() == typeof(CheckBox))
                {
                    CheckBox check = (CheckBox)OwnerRequestPanel.Controls[i];
                    infos2 = OwnerRequestPanel.Controls[i].Name.Split();
                    if(infos[1] == infos2[1] && infos[2] == infos2[2] && infos[3] == infos2[3] && infos[4] == infos2[4] && infos[5] == infos2[5] && infos[6] == infos2[6])
                    {
                        if (!check.Checked)
                        {
                            MessageBox.Show("Вы не подтвердили сделку!");
                            return;
                        }
                        else
                        {
                                stop1 = true;
                        }
                    }
                }
                else if(OwnerRequestPanel.Controls[i].GetType() == typeof(TextBox))
                {
                    infos2 = OwnerRequestPanel.Controls[i].Name.Split();
                    if(infos[1] == infos2[1] && infos[2] == infos2[2] && infos[3] == infos2[3] && infos[4] == infos2[4] && infos[5] == infos2[5] && infos[6] == infos2[6])
                    {
                        if(Convert.ToInt16(OwnerRequestPanel.Controls[i].Text) == 0)
                        {
                            MessageBox.Show("Вы не ввели количество или оно равно 0");
                            return;
                        }
                        else
                        {
                            idTovar = Convert.ToInt16(infos2[3]);
                            idSeller = Convert.ToInt16(infos2[4]);
                            date = infos2[5] + " " + infos2[6];
                            getCount = Convert.ToInt16(OwnerRequestPanel.Controls[i].Text);

                            stop2 = true;
                        }
                    }
                }
            }
            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetTovarFromSeller", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter tovarParam = new SqlParameter
                {
                    ParameterName = "@tovar",
                    Value = idTovar
                };
                cmd.Parameters.Add(tovarParam);

                SqlParameter dateParam = new SqlParameter
                {
                    ParameterName = "@date",
                    Value = Convert.ToDateTime(date)
                };
                cmd.Parameters.Add(dateParam);

                SqlParameter sellerParam = new SqlParameter
                {
                    ParameterName = "@seller",
                    Value = idSeller
                };
                cmd.Parameters.Add(sellerParam);

                SqlParameter countParam = new SqlParameter
                {
                    ParameterName = "@getCount",
                    Value = getCount
                };
                cmd.Parameters.Add(countParam);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Вы успешно купили " + getCount + " единиц(у) товара");
            FillRequestForOwner();

        }

        private void buttonTovar_Click(object sender, EventArgs e)
        {

            string[] infos = (sender as Button).Name.Split();
            (sender as Button).Enabled = false;
            (sender as Button).BackColor = Color.FromArgb(58, 141, 158);
            SelectedTovars.Add(infos[infos.Length - 1]);
            FillSelectedCards();
        }

        private void textBoxes_TextChanged(object sender, EventArgs e)
        {
            string[] infos = (sender as TextBox).Name.Split();
            int res;
            if(Int32.TryParse((sender as TextBox).Text,out res))
            {
                if (res > Convert.ToInt16(infos[infos.Length - 1]))
                    res = Convert.ToInt16(infos[infos.Length - 1]);
            }
            else
            {
                (sender as TextBox).Text = "0";
                (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
                return;
            }

            (sender as TextBox).Text = res.ToString();
            (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;

            int summa = 0;
            for(int i = 0; i < SelectedTovarPanel.Controls.Count; ++i)
            {
                if(SelectedTovarPanel.Controls[i].GetType() == typeof(TextBox))
                {
                    string[] infos3 = SelectedTovarPanel.Controls[i].Name.Split();
                    summa += Convert.ToInt16(SelectedTovarPanel.Controls[i].Text) * Convert.ToInt16(infos3[infos3.Length - 2]);
                }
            }
            if (summa > 0)
            {
                this.button8.Enabled = true;
                this.button8.BackColor = Color.FromArgb(78, 184, 206);
            }
            else
            {
                this.button8.Enabled = false;
                this.button8.BackColor = Color.FromArgb(58, 141, 158);
            }

            this.label29.Text = summa.ToString() + " грн";
        }
        private void buttonSelectedTovar_Click(object sender, EventArgs e)
        {
            string[] infos = (sender as Button).Name.Split();
            for (int j = 0; j < TovarPanel.Controls.Count; j++)
            {
                if(TovarPanel.Controls[j].GetType() == typeof(Button))
                {
                    string[] infos2 = TovarPanel.Controls[j].Name.Split();
                    if (infos2[infos2.Length-1] == infos[infos.Length - 1])
                    {
                        TovarPanel.Controls[j].Enabled = true;
                        TovarPanel.Controls[j].BackColor = Color.FromArgb(78, 184, 206);
                        SelectedTovars.Remove(infos[infos.Length - 1]);
                    }
                }
                
            }
            SelectedTovars.Remove(infos[infos.Length - 1]);
            FillSelectedCards();

            int summa = 0;
            for (int i = 0; i < SelectedTovarPanel.Controls.Count; ++i)
            {
                if (SelectedTovarPanel.Controls[i].GetType() == typeof(TextBox))
                {
                    string[] infos3 = SelectedTovarPanel.Controls[i].Name.Split();
                    summa += Convert.ToInt16(SelectedTovarPanel.Controls[i].Text) * Convert.ToInt16(infos3[infos3.Length - 2]);
                }
            }
            if (summa > 0)
            {
                this.button8.Enabled = true;
                this.button8.BackColor = Color.FromArgb(78, 184, 206);
            }
            else
            {
                this.button8.Enabled = false;
                this.button8.BackColor = Color.FromArgb(58, 141, 158);
            }

            this.label29.Text = summa.ToString() + " грн";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.TextPermission == "0")
            {
                FullTovarPanel.Show();
                FullAdminPanel.Hide();
                FullSellerPanel.Hide();
                FullOwnerPanel.Hide();
                textBox.Hide();
            }
            else
            {
                this.Controls.Remove(textBox);
                FullAdminPanel.Hide();
                FullTovarPanel.Hide();
                FullSellerPanel.Hide();
                FullOwnerPanel.Hide();
                textBox = new RichTextBox()
                {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(53)))), ((int)(((byte)(71))))),
                    BorderStyle = System.Windows.Forms.BorderStyle.None,
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Location = new System.Drawing.Point(500, 210),
                    Name = "richTextBox1",
                    Size = new System.Drawing.Size(772, 674),
                    TabIndex = 0,
                    Text =  "У ВАС НЕТУ ПРАВ!",
                    Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                };
                textBox.BringToFront();
                this.Controls.Add(textBox);
            }

            UserPanel.BringToFront();
            panel11.BringToFront();
            label7.BringToFront();
            label8.BringToFront();
            UserPanelButton.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (this.TextPermission == "1")
            {
                FullAdminPanel.Hide();
                FullTovarPanel.Hide();
                FullOwnerPanel.Hide();
                FullSellerPanel.Show();
                FillSellerRequests();
                textBox.Hide();
            }
            else
            {
                this.Controls.Remove(textBox);
                FullAdminPanel.Hide();
                FullTovarPanel.Hide();
                FullOwnerPanel.Hide();
                textBox = new RichTextBox()
                {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(53)))), ((int)(((byte)(71))))),
                    BorderStyle = System.Windows.Forms.BorderStyle.None,
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Location = new System.Drawing.Point(500, 210),
                    Name = "richTextBox1",
                    Size = new System.Drawing.Size(772, 674),
                    TabIndex = 0,
                    Text = "У ВАС НЕТУ ПРАВ!",
                    Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                };
                textBox.BringToFront();
                this.Controls.Add(textBox);
            }
            UserPanel.BringToFront();
            panel11.BringToFront();
            label7.BringToFront();
            label8.BringToFront();
            UserPanelButton.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.TextPermission == "2")
            {
                FullAdminPanel.Hide();
                FullTovarPanel.Hide();
                FullSellerPanel.Hide();
                FullOwnerPanel.Show();
                textBox.Hide();
            }
            else
            {
                this.Controls.Remove(textBox);
                FullSellerPanel.Hide();
                FullAdminPanel.Hide();
                FullTovarPanel.Hide();
                textBox = new RichTextBox()
                {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(53)))), ((int)(((byte)(71))))),
                    BorderStyle = System.Windows.Forms.BorderStyle.None,
                    ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                    Location = new System.Drawing.Point(500, 210),
                    Name = "richTextBox1",
                    Size = new System.Drawing.Size(772, 674),
                    TabIndex = 0,
                    Text = "У ВАС НЕТУ ПРАВ!",
                    Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                };
                textBox.BringToFront();
                this.Controls.Add(textBox);
            }

            UserPanel.BringToFront();
            panel11.BringToFront();
            label7.BringToFront();
            label8.BringToFront();
            UserPanelButton.BringToFront();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FullTovarPanel.Hide();
            FullAdminPanel.Hide();
            FullSellerPanel.Hide();
            FullOwnerPanel.Hide();
            this.Controls.Remove(textBox);
            textBox = new RichTextBox()
            {
                BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(53)))), ((int)(((byte)(71))))),
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(184)))), ((int)(((byte)(206))))),
                Location = new System.Drawing.Point(500, 210),
                Name = "richTextBox1",
                Size = new System.Drawing.Size(772, 674),
                TabIndex = 0,
                Text = "Это приложение разработано\n" +
                        "   Эдуардом Перетокиным!  \n" +
                        "\n" +
                        "Поставьте маскимум)",
                Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
            };
            textBox.BringToFront();
            this.Controls.Add(textBox);

            UserPanel.BringToFront();
            panel11.BringToFront();
            label7.BringToFront();
            label8.BringToFront();
            UserPanelButton.BringToFront();
        }

        int MouseX = 0, MouseY = 0;
        int MouseinX = 0, MouseinY = 0;
        bool IsMouseDown;

        private void panel11_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
            MouseinX = MousePosition.X - this.Bounds.X;
            MouseinY = MousePosition.Y - this.Bounds.Y;
        }

        private void panel11_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void panel11_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                MouseX = MousePosition.X - MouseinX;
                MouseY = MousePosition.Y - MouseinY;

                this.SetDesktopLocation(MouseX, MouseY);
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label18_MouseEnter(object sender, EventArgs e)
        {
            this.label18.ForeColor = Color.FromArgb(78, 184, 206);
            this.pictureBox5.BackgroundImage = Properties.Resources.changePassH1;
        }

        private void label18_MouseLeave(object sender, EventArgs e)
        {
            this.label18.ForeColor = Color.WhiteSmoke;
            this.pictureBox5.BackgroundImage = Properties.Resources.img_234288;
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            this.label18.ForeColor = Color.FromArgb(78, 184, 206);
            this.pictureBox5.BackgroundImage = Properties.Resources.changePassH1;
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e)
        {
            this.label18.ForeColor = Color.WhiteSmoke;
            this.pictureBox5.BackgroundImage = Properties.Resources.img_234288;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "Я ищу...")
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Я ищу...";
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text == "Я ищу...")
            {
                FillCards("");
                return;
            }
            int number = 0;
            int countWords = 0;
            if(textBox1.Text.Length > 3)
            {
                if ((textBox1.Text.Length % 2) == 0)
                {
                    number = textBox1.Text.Length / 2;
                    countWords = number + 1;
                }
                else
                {
                    number = textBox1.Text.Length / 2 + 1;
                    countWords = number;
                }
                
            }
            else
            {
                number = textBox1.Text.Length;
                countWords = 1;
            }
            string[] search = new string[countWords];
            string command;
            for (int i = 0; i < search.Length; ++i)
            {
                for(int j = 0; j < number; ++j)
                {
                    search[i] += textBox1.Text[i + j].ToString();
                }

            }

            command = "AND (NameOfTovar LIKE('%";
            for (int i = 0; i < search.Length; ++i)
            {
                if (i != (search.Length - 1))
                    command += search[i] + "%') OR NameOfTovar LIKE('%";
                else
                    command += search[i] + "%'))";
            }

            FillCards(command);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<string> tovars = new List<string>();
            for (int i = 0; i < SelectedTovarPanel.Controls.Count; ++i)
            {
                if (SelectedTovarPanel.Controls[i].GetType() == typeof(TextBox))
                {
                    string[] infos3 = SelectedTovarPanel.Controls[i].Name.Split();
                    tovars.Add(infos3[infos3.Length - 3] + "," + SelectedTovarPanel.Controls[i].Text + ";");
                }
            }

            string _fullTovars = "";
            for(int i = 0; i < tovars.Count; ++i)
            {
                _fullTovars += tovars[i];
            }

            using (SqlConnection con = new SqlConnection(this.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("BuyFromSklad",con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter customerParam = new SqlParameter
                {
                    ParameterName = "@customer",
                    Value = this.MyId
                };
                cmd.Parameters.Add(customerParam);



                SqlParameter tovarsParam = new SqlParameter
                {
                    ParameterName = "@tovars",
                    Value = _fullTovars
                };
                cmd.Parameters.Add(tovarsParam);
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < TovarPanel.Controls.Count; i++)
            {
                if (TovarPanel.Controls[i].GetType() == typeof(Button))
                {
                    string[] infos = TovarPanel.Controls[i].Name.Split();
                    for (int j = 0; j < SelectedTovars.Count; j++)
                    {
                        if (infos[infos.Length - 1] == SelectedTovars[j])
                        {
                            TovarPanel.Controls[i].Enabled = true;
                            TovarPanel.Controls[i].BackColor = Color.FromArgb(78, 184, 206);
                            SelectedTovars.Remove(infos[infos.Length - 1]);
                        }
                    }

                }
            }
            

            MessageBox.Show("Вы успешно оплатили! Ваша сумма сделки: " + this.label29.Text);
            FillSelectedCards();
            FillCards("");
            this.label29.Text = "0";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.SelectedValue.ToString());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int result = 0;
            if (Int32.TryParse(textBox2.Text, out result))
            {
                result = Convert.ToInt32(textBox2.Text);
                textBox2.Text = result.ToString();
                textBox2.SelectionStart = textBox2.Text.Length;
                if (result < 1)
                {
                    textBox2.Text = "0";
                    textBox2.SelectionStart = textBox2.Text.Length;
                }
                return;
            }
            else
            {
                textBox2.Text = "0";
                textBox2.SelectionStart = textBox2.Text.Length;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int result = 0;
            if (Int32.TryParse(textBox3.Text, out result))
            {
                result = Convert.ToInt32(textBox3.Text);
                textBox3.Text = result.ToString();
                textBox3.SelectionStart = textBox3.Text.Length;
                if (result < 1)
                {
                    textBox3.Text = "0";
                    textBox3.SelectionStart = textBox3.Text.Length;
                }
                return;
            }
            else
            {
                textBox3.Text = "0";
                textBox3.SelectionStart = textBox3.Text.Length;
            }
        }

        private void panel18_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "0" && textBox3.Text != "0")
            {
                using (SqlConnection con = new SqlConnection(this.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("ProvideSuggest", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter sellerParam = new SqlParameter
                    {
                        ParameterName = "@seller",
                        Value = this.MyId
                    };
                    cmd.Parameters.Add(sellerParam);

                    SqlParameter tovarParam = new SqlParameter
                    {
                        ParameterName = "@tovar",
                        Value = comboBox1.SelectedValue.ToString()
                    };
                    cmd.Parameters.Add(tovarParam);

                    SqlParameter countParam = new SqlParameter
                    {
                        ParameterName = "@count",
                        Value = textBox3.Text
                    };
                    cmd.Parameters.Add(countParam);

                    SqlParameter priceParam = new SqlParameter
                    {
                        ParameterName = "@price",
                        Value = textBox2.Text
                    };
                    cmd.Parameters.Add(priceParam);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Ваша заявка успешно создана!");
                    FillSellerRequests();
                    textBox2.Text = "0";
                    textBox3.Text = "0";
                }
            }
            else
            {
                MessageBox.Show("Вы ввели неверные данные!");
                textBox2.Text = "0";
                textBox3.Text = "0";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox2.SelectedValue.ToString());
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            FillRequestForOwner();
        }

        private void label19_MouseEnter(object sender, EventArgs e)
        {
            label19.ForeColor = Color.FromArgb(78, 184, 206);
        }

        private void label19_MouseLeave(object sender, EventArgs e)
        {
            label19.ForeColor = Color.WhiteSmoke;
        }

    }
}
