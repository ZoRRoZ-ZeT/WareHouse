using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Console : Form
    {
        private string DatabaseText { get; set; }
        private string StartString { get; set; }
        private string helpString { get; set; }
        private string errorString { get; set; }
        private string isActiveString { get; set; }
        private string[] commands { get; set; }
        public Console()
        {
            InitializeComponent();
            StartString = "TeamCode [Version 1.0.1776.864]\n" +
                "(c) Корпорация TeamCode (TeamCode corporation), 2019. Все права защищены.\n" +
                "\n" +
                "Admin> ";

            helpString = "Это список всех существующих комманд в Админ-консоли:\n" +
                "1. !help - Показывает список всех существующих комманд.\n" +
                "2. !home - Возвращает в главное меню консоли.\n" +
                "3. show table [table] - Показывает содержимоей данной таблицы table.\n" +
                "4. users - Показывает список всех существующих пользователей.\n" +
                "5. commandSQL ( [SQL Query] ) - Позволяет ввести собственный запрос SQL, для чтение/изменения существующей БД.(обязательно пробелы)\n" +
                "6. exit - Позволяет выйти из приложения.\n" +
                "7. hide - Скрывает консоль." +
                "\n" +
                "Admin> ";

            errorString = "Вы ввели не верную комманду, проверьте правильность ввода и повторите попытку!\n" +
                "\n" +
                "Admin> ";

            isActiveString = StartString;
            richTextBox1.Text = StartString;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
        }
        public string ConnectionString { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            string[] getText = richTextBox1.Text.Split();
            string[] getFullString = isActiveString.Split();

            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataAdapter sda;
            DataTable dt = new DataTable();
            int i = getFullString.Count() -1;

            if (getText[i].ToLower() == "!help")
            {
                try
                {
                    if (getText[i + 1] != "")
                    {
                        isActiveString = errorString;
                        richTextBox1.Text = errorString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        return;
                    }
                }
                catch (Exception)
                {
                }

                isActiveString = helpString;
                richTextBox1.Text = isActiveString;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                return;
            }
            else if (getText[i].ToLower() == "!home")
            {
                try
                {
                    if (getText[i + 1] != "")
                    {
                        isActiveString = errorString;
                        richTextBox1.Text = errorString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        return;
                    }
                }
                catch (Exception)
                {
                }

                isActiveString = StartString;
                richTextBox1.Text = StartString;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                return;
            }
            else if (getText[i].ToLower() == "hide")
            {
                try
                {
                    if (getText[i + 1] != "")
                    {
                        isActiveString = errorString;
                        richTextBox1.Text = errorString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        return;
                    }
                }
                catch (Exception)
                {
                }

                this.Hide();
                isActiveString = StartString;
                richTextBox1.Text = StartString;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                return;
            }
            else if (getText[i].ToLower() == "exit")
            {
                try
                {
                    if (getText[i + 1] != "")
                    {
                        isActiveString = errorString;
                        richTextBox1.Text = errorString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        return;
                    }
                }
                catch (Exception)
                {
                }

                Application.Exit();
            }
            else if (getText[i].ToLower() == "users")
            {
                try
                {
                    if (getText[i + 1] != "")
                    {
                        isActiveString = errorString;
                        richTextBox1.Text = errorString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        return;
                    }
                }
                catch (Exception)
                {
                }

                sda = new SqlDataAdapter("SELECT username FROM Users", con);
                sda.Fill(dt);

                isActiveString = "ID\tusername\n";
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    isActiveString += (j + 1).ToString() + ".\t" + dt.Rows[j][0].ToString() + "\n";
                }
                isActiveString += "\nAdmin> ";
                richTextBox1.Text = isActiveString;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                return;
            }
            else
            {
                for (int k = i; k < getText.Count(); k++)
                {
                    if (getText[k - 2].ToLower() == "show" && getText[k - 1].ToLower() == "table")
                    {
                        try
                        {
                            if (getText[k + 1] != "" || getText[k] == "")
                            {
                                isActiveString = errorString;
                                richTextBox1.Text = errorString;
                                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                return;
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            sda = new SqlDataAdapter("SELECT * FROM " + getText[k], con);
                            sda.Fill(dt);
                        }
                        catch (Exception)
                        {
                            isActiveString = errorString;
                            richTextBox1.Text = errorString;
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            return;
                        }


                        isActiveString = "Данная таблица состоит из таких столбцов, как:\n";
                        int p = 0;
                        foreach (DataColumn column in dt.Columns)
                        {
                            isActiveString += (p + 1).ToString() + ". " + column.ColumnName + "\n";
                            p++;
                        }
                        isActiveString += "\nПоля этой таблицы:\n";

                        for (int m = 0; m < dt.Rows.Count; m++)
                        {
                            for (int n = 0; n < dt.Columns.Count; n++)
                            {
                                isActiveString += dt.Rows[m][n].ToString() + "\t";
                            }
                            isActiveString += "\n";
                        }
                        isActiveString += "\nAdmin> ";
                        richTextBox1.Text = isActiveString;
                        return;
                    }
                    else if (getText[k - 1].ToLower() == "commandsql" && getText[k] == "(")
                    {
                        string command = "";
                        for (int start = k + 1; start < getText.Length - 1; ++start)
                        {
                            command += getText[start] + " ";
                        }
                        try
                        {
                            if (getText[k+1].ToLower() == "select")
                            {
                                sda = new SqlDataAdapter(command, con);
                                sda.Fill(dt);
                                isActiveString = "";
                                for (int m = 0; m < dt.Rows.Count; m++)
                                {
                                    for (int n = 0; n < dt.Columns.Count; n++)
                                    {
                                        isActiveString += dt.Rows[m][n].ToString() + "\t";
                                    }
                                    isActiveString += "\n";
                                }
                                isActiveString += "\nAdmin> ";
                                richTextBox1.Text = isActiveString;
                                return;
                            }
                        }
                        catch (Exception)
                        {
                            isActiveString = errorString;
                            richTextBox1.Text = errorString;
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            return;
                        }
                        con.Open();
                        SqlCommand cmd = new SqlCommand(command, con);
                        int count;
                        try
                        {
                            count = cmd.ExecuteNonQuery();
                        }
                        catch (SqlException)
                        {
                            isActiveString = errorString;
                            richTextBox1.Text = errorString;
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            return;
                        }
                        isActiveString = "Было изменено " + count.ToString() + " строк.\n" +
                            "\n" +
                            "Admin> ";
                        richTextBox1.Text = isActiveString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        UpdateDataBases();
                        return;
                    }
                }
                isActiveString = errorString;
                richTextBox1.Text = errorString;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                return;
            }
            return;
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

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < isActiveString.Length; i++)
            {
                try
                {
                    if (isActiveString[i] != richTextBox1.Text[i])
                    {
                        richTextBox1.Text = isActiveString;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        return;
                    }
                }
                catch (Exception)
                {
                    richTextBox1.Text = isActiveString;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    return;
                }

            }
        }
        private void UpdateDataBases()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM sys.objects WHERE type in (N'U')", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            DatabaseText = " Databases: ";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DatabaseText += dt.Rows[i][0] + ", ";
            }
            richTextBox2.Text = DatabaseText;
        }
        private void Console_Load(object sender, EventArgs e)
        {
            UpdateDataBases();
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
