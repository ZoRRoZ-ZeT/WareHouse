using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class ChangePass : Form
    {
        public ChangePass()
        {
            InitializeComponent();
        }
           
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(OldPass.Text + Username));
            if(Convert.ToBase64String(hash) == Password)
            {
                if((NewPass.Text == RepeatPass.Text) && !(NewPass.Text == ""))
                {
                    SqlConnection con = new SqlConnection(ConnectionString);
                    con.Open();

                    var newMd5 = MD5.Create();
                    var newHash = newMd5.ComputeHash(Encoding.UTF8.GetBytes(NewPass.Text + Username));

                    SqlCommand cmd = new SqlCommand("UPDATE users SET password = '" + Convert.ToBase64String(newHash) + "' WHERE username ='" + Username + "'", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Пароль успешно изменён!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пароли не совпадают или не содержат символов!");
                }
            }
            else
            {
                MessageBox.Show("Вы ввели не верный старый пароль");
            }
        }


        private void label5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            label5.ForeColor = Color.FromArgb(78, 184, 206);
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.ForeColor = Color.WhiteSmoke;
        }

        int MouseX = 0, MouseY = 0;
        int MouseinX = 0, MouseinY = 0;
        bool IsMouseDown;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
            MouseinX = MousePosition.X - this.Bounds.X;
            MouseinY = MousePosition.Y - this.Bounds.Y;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
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
