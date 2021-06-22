using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WindowsFormsApp2
{
    public partial class Registration : Form
    {
        public string ConnectionString { get; set; }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
       (
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
       );


        public Registration()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 40, 40));
            ConfirmNext.Hide();
            GoNextRegistr.Hide();
            RadioButtonsPanel.Hide();
            FirmaRegistrPanel.Hide();
            EmailRegistrPanel.Location = new Point(24, 178);
            isSeller = false;
        }
        private bool isSeller { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox7.Text == "" || textBox8.Text == "" || textBox9.Text == "")
            {
                MessageBox.Show("Проверьте правильность введённых данных!");
                return;
            }

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO Firma(ID_Firma,NameOfFirma,Adress,Phone) VALUES(@id,@name,@adress,@phone)", con);
            SqlDataAdapter sda = new SqlDataAdapter("SELECT MAX(ID_Firma) FROM Firma", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            int idFirma = Convert.ToInt16(dt.Rows[0][0]) + 1;
            cmd.Parameters.AddWithValue("@id", Convert.ToInt16(dt.Rows[0][0]) + 1);
            cmd.Parameters.AddWithValue("@name", textBox7.Text);
            cmd.Parameters.AddWithValue("@adress", textBox8.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox9.Text);
            cmd.ExecuteNonQuery();

            if(isSeller)
            {
                cmd = new SqlCommand("INSERT INTO Seller(ID_Seller,ID_Firma,username) VALUES(@idS,@idF,@username)",con);
                sda = new SqlDataAdapter("SELECT MAX(ID_Seller) FROM Seller", con);
                dt = new DataTable();
                sda.Fill(dt);
                cmd.Parameters.AddWithValue("@username", textBox1.Text);
                cmd.Parameters.AddWithValue("@idS", Convert.ToInt16(dt.Rows[0][0]) + 1);
                cmd.Parameters.AddWithValue("@idF", idFirma);
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd = new SqlCommand("INSERT INTO Customer(ID_Customer,ID_Firma,username) VALUES(@idC,@idF,@username)", con);
                sda = new SqlDataAdapter("SELECT MAX(ID_Customer) FROM Customer", con);
                dt = new DataTable();
                sda.Fill(dt);
                cmd.Parameters.AddWithValue("@username", textBox1.Text);
                cmd.Parameters.AddWithValue("@idC", Convert.ToInt16(dt.Rows[0][0]) + 1);
                cmd.Parameters.AddWithValue("@idF", idFirma);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Вы зарегистрированы!");
            this.Close();
        }


        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.ForeColor = Color.FromArgb(78, 184, 206);
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.ForeColor = Color.WhiteSmoke;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        int MouseX = 0, MouseY = 0;
        int MouseinX = 0, MouseinY = 0;
        bool IsMouseDown;

        private void panel5_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
            MouseinX = MousePosition.X - this.Bounds.X;
            MouseinY = MousePosition.Y - this.Bounds.Y;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Username")
                textBox1.Clear();

            panel1.BackColor = Color.FromArgb(78, 184, 206);
            textBox1.ForeColor = Color.FromArgb(78, 184, 206);
            pictureBox2.BackgroundImage = Properties.Resources.icon1C;

            panel2.BackColor = Color.WhiteSmoke;
            textBox2.ForeColor = Color.WhiteSmoke;
            pictureBox3.BackgroundImage = Properties.Resources.img_234288;

            panel3.BackColor = Color.WhiteSmoke;
            textBox3.ForeColor = Color.WhiteSmoke;
            pictureBox4.BackgroundImage = Properties.Resources.name;

            panel4.BackColor = Color.WhiteSmoke;
            textBox4.ForeColor = Color.WhiteSmoke;
            pictureBox5.BackgroundImage = Properties.Resources.name;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Password")
                textBox2.Clear();

            textBox2.PasswordChar = '•';

            panel2.BackColor = Color.FromArgb(78, 184, 206);
            textBox2.ForeColor = Color.FromArgb(78, 184, 206);
            pictureBox3.BackgroundImage = Properties.Resources.icon2C;


            panel1.BackColor = Color.WhiteSmoke;
            textBox1.ForeColor = Color.WhiteSmoke;
            pictureBox2.BackgroundImage = Properties.Resources.username;

            panel3.BackColor = Color.WhiteSmoke;
            textBox3.ForeColor = Color.WhiteSmoke;
            pictureBox4.BackgroundImage = Properties.Resources.name;

            panel4.BackColor = Color.WhiteSmoke;
            textBox4.ForeColor = Color.WhiteSmoke;
            pictureBox5.BackgroundImage = Properties.Resources.name;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "Name")
                textBox3.Clear();

            panel3.BackColor = Color.FromArgb(78, 184, 206);
            textBox3.ForeColor = Color.FromArgb(78, 184, 206);
            pictureBox4.BackgroundImage = Properties.Resources.nameC;


            panel1.BackColor = Color.WhiteSmoke;
            textBox1.ForeColor = Color.WhiteSmoke;
            pictureBox2.BackgroundImage = Properties.Resources.username;

            panel2.BackColor = Color.WhiteSmoke;
            textBox2.ForeColor = Color.WhiteSmoke;
            pictureBox3.BackgroundImage = Properties.Resources.img_234288;

            panel4.BackColor = Color.WhiteSmoke;
            textBox4.ForeColor = Color.WhiteSmoke;
            pictureBox5.BackgroundImage = Properties.Resources.name;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "Surname")
                textBox4.Clear();

            panel4.BackColor = Color.FromArgb(78, 184, 206);
            textBox4.ForeColor = Color.FromArgb(78, 184, 206);
            pictureBox5.BackgroundImage = Properties.Resources.nameC;


            panel1.BackColor = Color.WhiteSmoke;
            textBox1.ForeColor = Color.WhiteSmoke;
            pictureBox2.BackgroundImage = Properties.Resources.username;

            panel2.BackColor = Color.WhiteSmoke;
            textBox2.ForeColor = Color.WhiteSmoke;
            pictureBox3.BackgroundImage = Properties.Resources.img_234288;

            panel3.BackColor = Color.WhiteSmoke;
            textBox3.ForeColor = Color.WhiteSmoke;
            pictureBox4.BackgroundImage = Properties.Resources.name;
        }

        private void Registration_Load(object sender, EventArgs e)
        {
        }

        private int code { get; set; }

        private void ConfirmEmail_Click(object sender, EventArgs e)
        {
            string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            if(Regex.IsMatch(textBox5.Text,pattern, RegexOptions.IgnoreCase))
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM users where email = '" + textBox5.Text + "'",connection);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if(dt.Rows[0][0].ToString() == "1")
                {
                    MessageBox.Show("Данная почта уже существует!");
                    return;
                }
                Random x = new Random();
                code = x.Next(111111, 999999);
                MessageBox.Show("Ваш код: " + code.ToString());
                ConfirmEmail.Hide();
                textBox6.Enabled = true;
                textBox6.ForeColor = Color.WhiteSmoke;
                panel7.BackColor = Color.WhiteSmoke;
                ConfirmNext.Show();
            }
            else
            {
                MessageBox.Show("Вы не правильно ввели почту!");
            }
            
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            if (textBox5.Text == "Email")
                textBox5.Clear();

            panel6.BackColor = Color.FromArgb(78, 184, 206);
            textBox5.ForeColor = Color.FromArgb(78, 184, 206);
            if (textBox6.Enabled)
            {
                panel7.BackColor = Color.WhiteSmoke;
                textBox6.ForeColor = Color.WhiteSmoke;
            }
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            if (textBox6.Text == "Code")
                textBox6.Clear();

            panel7.BackColor = Color.FromArgb(78, 184, 206);
            textBox6.ForeColor = Color.FromArgb(78, 184, 206);

            panel6.BackColor = Color.WhiteSmoke;
            textBox5.ForeColor = Color.WhiteSmoke;
        }

        private void ConfirmNext_Click(object sender, EventArgs e)
        {
            if(textBox6.Text == code.ToString())
            {
                EmailRegistrPanel.Hide();
                UserTablePanel.Show();
                UserTablePanel.Location = new Point(25, 136);
                GoNextRegistr.Show();
                RadioButtonsPanel.Show();
            }
        }

        private void GoNextRegistr_Click(object sender, EventArgs e)
        {
            bool Check = true;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            SqlDataAdapter users = new SqlDataAdapter("SELECT username FROM users", con);
            SqlDataAdapter usersCount = new SqlDataAdapter("SELECT COUNT(*) FROM users", con);

            DataTable usernamesTable = new DataTable();
            users.Fill(usernamesTable);

            DataTable userCountTable = new DataTable();
            usersCount.Fill(userCountTable);

            for (int i = 0; i < Convert.ToInt16(userCountTable.Rows[0][0].ToString()); ++i)
                if (textBox1.Text == usernamesTable.Rows[i][0].ToString())
                {
                    this.panel1.BackColor = Color.Red;
                    MessageBox.Show("Данный пользователь уже существует!");
                    Check = false;
                }


            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                MessageBox.Show("Данные введены некорректно!");
            else if (Check)
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text + textBox1.Text));
                SqlCommand cmd = new SqlCommand("INSERT INTO users(username,password,email,name,surname,isAdmin) VALUES (@username,@password,@email,@name,@surname, 0)", con);
                cmd.Parameters.AddWithValue("@username", textBox1.Text);
                cmd.Parameters.AddWithValue("@password", Convert.ToBase64String(hash));
                cmd.Parameters.AddWithValue("@email", textBox5.Text);
                cmd.Parameters.AddWithValue("@name", textBox3.Text);
                cmd.Parameters.AddWithValue("@surname", textBox4.Text);
                cmd.ExecuteNonQuery();
            }

            ConfirmNext.Hide();
            RadioButtonsPanel.Hide();
            GoNextRegistr.Hide();
            UserTablePanel.Hide();
            FirmaRegistrPanel.Show();
            CompleteRegistr.Location = new Point(40, 405);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Text == "Покупатель")
                isSeller = false;
            else
                isSeller = true;
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            if (textBox7.Text == "Название Фирмы")
                textBox7.Clear();

            panel8.BackColor = Color.FromArgb(78, 184, 206);
            textBox7.ForeColor = Color.FromArgb(78, 184, 206);

            panel9.BackColor = Color.WhiteSmoke;
            textBox8.ForeColor = Color.WhiteSmoke;

            panel10.BackColor = Color.WhiteSmoke;
            textBox9.ForeColor = Color.WhiteSmoke;
        }

        private void textBox8_Enter(object sender, EventArgs e)
        {
            if (textBox8.Text == "Адресс")
                textBox8.Clear();

            panel9.BackColor = Color.FromArgb(78, 184, 206);
            textBox8.ForeColor = Color.FromArgb(78, 184, 206);

            panel8.BackColor = Color.WhiteSmoke;
            textBox7.ForeColor = Color.WhiteSmoke;

            panel10.BackColor = Color.WhiteSmoke;
            textBox9.ForeColor = Color.WhiteSmoke;
        }

        private void textBox9_Enter(object sender, EventArgs e)
        {
            if (textBox9.Text == "Телефон")
                textBox9.Clear();

            panel10.BackColor = Color.FromArgb(78, 184, 206);
            textBox9.ForeColor = Color.FromArgb(78, 184, 206);

            panel8.BackColor = Color.WhiteSmoke;
            textBox7.ForeColor = Color.WhiteSmoke;

            panel9.BackColor = Color.WhiteSmoke;
            textBox8.ForeColor = Color.WhiteSmoke;
        }

        private void panel5_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void panel5_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                MouseX = MousePosition.X - MouseinX;
                MouseY = MousePosition.Y - MouseinY;

                this.SetDesktopLocation(MouseX, MouseY);
            }
        }
    }
}
