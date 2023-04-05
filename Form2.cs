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
using System.Reflection.Emit;

namespace registration
{
    public partial class Form2 : Form
    {

        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\innotics\STDdb.mdf;Integrated Security=True;Connect Timeout=30");

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            GetData();
            displayData();
        }

        private void displayData()
        {

            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from stdDetails";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        public DataTable GetData()
        {
            DataTable dt = new DataTable();
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from stdDetails";

            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            label1.Text = dt.Rows.Count.ToString();

            int eggCount = GetEggCount();
            label3.Text = eggCount.ToString();

            int vegiCount = GetVegiCount();
            label2.Text = vegiCount.ToString();


            int FishCount = GetFishCount();
            label4.Text = FishCount.ToString();


            int ChikenCount = GetChikenCount();
            label5.Text = ChikenCount.ToString();

            con.Close();
            return dt;
        }


        public int GetEggCount()
        {
            int count = 0;

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select count(*) from stdDetails where Food ='Egg'";
            count = (int)cmd.ExecuteScalar();
            return count;
        }

        public int GetVegiCount()
        {
            int count = 0;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select count(*) from stdDetails where Food ='Vegitable'";
            count = (int)cmd.ExecuteScalar();
            return count;
        }

        public int GetFishCount()
        {
            int count = 0;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select count(*) from stdDetails where Food ='Fish'";
            count = (int)cmd.ExecuteScalar();
            return count;
        }

        public int GetChikenCount()
        {
            int count = 0;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select count(*) from stdDetails where Food ='Chiken'";
            count = (int)cmd.ExecuteScalar();
            return count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int studentId = int.Parse(textBox1.Text);
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\innotics\\STDdb.mdf;Integrated Security=True;Connect Timeout=30";
            string query = "DELETE FROM stdDetails WHERE stdId = @StudentId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", studentId);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Row deleted successfully.");
                }
                else
                {
                    MessageBox.Show("No rows deleted.");
                }

            }
            textBox1.Text = "";
            GetData();
            displayData();




        }
    }
}
