using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
namespace WindowsFormsApp2
{
    public partial class Login : Form
    {

        private string ConnectionString { get; set; }
        private bool isCustomer { get; set; }
        private bool isSeller { get; set; }
        private bool isAdmin { get; set; }
        private int myId { get; set; }

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

        

        public Login()
        {
            InitializeComponent();
            ConnectionString = @"Data Source=DESKTOP-D2J80CP\HELLO;Initial Catalog=Optoviy_Sklad;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 40, 40));
            isCustomer = false;
            isSeller = false;
            isAdmin = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            Registration registr = new Registration();
            registr.ConnectionString = this.ConnectionString;
            registr.Show();

            registr.FormClosing += new FormClosingEventHandler(AnotherForm_closing);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            

           
            SqlConnection con = new SqlConnection(ConnectionString);

            string permission = "";

            if (isCustomer)
            {
                SqlDataAdapter customer = new SqlDataAdapter("SELECT COUNT(*) FROM dbo.Customer WHERE username='" + textBox1.Text + "'", con);
                DataTable checkCustomer = new DataTable();
                customer.Fill(checkCustomer);
                permission = "0";

                if (checkCustomer.Rows[0][0].ToString() == "0")
                {
                    MessageBox.Show("Ошибка! Введены не верные данные! Данного покупателя нету!");
                    return;
                }
                else
                {
                    SqlDataAdapter customerID = new SqlDataAdapter("SELECT ID_Customer FROM dbo.Customer WHERE username='" + textBox1.Text + "'", con);
                    DataTable ID = new DataTable();
                    customerID.Fill(ID);
                    myId = Convert.ToInt16(ID.Rows[0][0]);
                    MessageBox.Show(myId.ToString());
                }

            }
            else if (isSeller)
            {
                SqlDataAdapter seller = new SqlDataAdapter("SELECT COUNT(*) FROM dbo.Seller WHERE username='" + textBox1.Text + "'", con);
                DataTable checkSeller = new DataTable();
                seller.Fill(checkSeller);
                permission = "1";

                if (checkSeller.Rows[0][0].ToString() == "0")
                {
                    MessageBox.Show("Ошибка! Введены не верные данные! Данного поставщика нету!");
                    return;
                }
                else
                {
                    SqlDataAdapter sellerID = new SqlDataAdapter("SELECT ID_Seller FROM dbo.Seller WHERE username='" + textBox1.Text + "'", con);
                    DataTable ID = new DataTable();
                    sellerID.Fill(ID);
                    myId = Convert.ToInt16(ID.Rows[0][0]);
                    MessageBox.Show(myId.ToString());
                }
            }
            else
            {
                SqlDataAdapter admin = new SqlDataAdapter("SELECT COUNT(*) FROM dbo.users WHERE username='" + textBox1.Text + "' AND IsAdmin = 1", con);
                DataTable checkAdmin = new DataTable();
                admin.Fill(checkAdmin);
                permission = "2";

                if (checkAdmin.Rows[0][0].ToString() == "0")
                {
                    MessageBox.Show("Ошибка! Вы ошиблись...!");
                    return;
                }
                else
                {
                    myId = 0;
                    MessageBox.Show(myId.ToString());
                }
            }

            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text + textBox1.Text));
            SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM dbo.users WHERE username='" + textBox1.Text + "' AND password ='1'", con);
            SqlDataAdapter users = new SqlDataAdapter("SELECT name,surname,username FROM dbo.users WHERE username='" + textBox1.Text + "' AND password ='1'", con);
            DataTable dt = new DataTable();
            //MessageBox.Show(Convert.ToBase64String(hash));
            DataTable MyUsers = new DataTable();
            sda.Fill(dt);
            users.Fill(MyUsers);
            if(dt.Rows[0][0].ToString() == "1")
            {
                this.Hide();

                Main main = new Main();


                switch(permission)
                {
                    case "0":
                        main.label3.Text = "Покупатель";
                        break;
                    case "1":
                        main.label3.Text = "Поставщик";
                        break;
                    case "2":
                        main.label3.Text = "Администратор";
                        main.label3.ForeColor = Color.OrangeRed;
                        main.label7.ForeColor = Color.OrangeRed;
                        main.label7.Font = new Font(main.label7.Font, FontStyle.Bold);
                        main.label8.ForeColor = Color.OrangeRed;
                        main.label8.Font = new Font(main.label8.Font, FontStyle.Bold);
                        break;
                    default:
                        main.label3.Text = "Пользователь";
                        break;
                }
                main.TextUsername = MyUsers.Rows[0][2].ToString();
                main.TextName = MyUsers.Rows[0][0].ToString();
                main.TextSurname = MyUsers.Rows[0][1].ToString();
                main.TextPassword = Convert.ToBase64String(hash);
                main.ConnectionString = this.ConnectionString;
                main.TextPermission = permission;
                main.MyId = this.myId;
                main.Show();

                main.FormClosing += new FormClosingEventHandler(AnotherForm_closing);
            }
            else
            {
                MessageBox.Show("Ошибка! Введены не верные данные!");
            }

            
        }

        public void AnotherForm_closing(object sender, FormClosingEventArgs e)
        {
            this.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
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


        int MouseX = 0, MouseY = 0;
        int MouseinX = 0, MouseinY = 0;
        bool IsMouseDown;

        private void panel3_MouseDown(object sender, MouseEventArgs e)
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
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if(radioButton.Checked)
            {
                if(radioButton.Text == "Покупатель")
                {
                    isCustomer = true;
                    isSeller = false;
                    isAdmin = false;
                }
                else
                {
                    isCustomer = false;
                    isSeller = true;
                    isAdmin = false;
                }
            }
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if(IsMouseDown)
            {
                MouseX = MousePosition.X - MouseinX;
                MouseY = MousePosition.Y - MouseinY;

                this.SetDesktopLocation(MouseX, MouseY);
            }
        }
    }
}
