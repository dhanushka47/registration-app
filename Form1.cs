using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AForge.Video.DirectShow;
using AForge.Video;
using ZXing;

using ClosedXML.Excel;


using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;
using System.IO;

namespace registration
{
    public partial class Form1 : Form
    {

        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\innotics\STDdb.mdf;Integrated Security=True;Connect Timeout=30");
      
         FilterInfoCollection filter;
        VideoCaptureDevice captureDevice;
        
        public Form1()
        {

            InitializeComponent();
            LoadComboBoxItems();
            dbWorks();
            hideitems();


        }

        public void dbWorks () {
            string folderPath = "C:\\innotics";
            string filePath = Path.Combine(folderPath, "STDdb.mdf");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


        }

        public void hideitems() {
        
            textBox2.Enabled=false;
            textBox3.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;

            button2.Enabled = false;
            button3.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;

        }

        public void showitems() {

            textBox2.Enabled = true;
            textBox3.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;

            button2.Enabled = true;
            button3.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;




        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filter)
                comboBox1.Items.Add(filterInfo.Name);
                comboBox1.SelectedIndex = 0;

          
            button2.Enabled= false;
            textBox1.Enabled= false;

            


        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            button1.Enabled= false;
            comboBox1.Enabled= false;
            button2.Enabled= true;

            button4.Enabled = false;
            button5.Enabled = false;


            captureDevice = new VideoCaptureDevice(filter[comboBox1.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
            }
        }





        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                BarcodeReader barcode = new BarcodeReader();
                Result result = barcode.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    textBox1.Text = result.ToString();
                    timer1.Stop();

                    button1.Enabled = true;
                    button2.Enabled = false;
                    button4.Enabled = true;
                    button5.Enabled = true;

                    if (captureDevice.IsRunning)
                        captureDevice.Stop(); 
                    
                         showitems();

                   
                   

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled= false;
            button1.Enabled = true;
            comboBox1.Enabled = true;


            button4.Enabled = true;
            button5.Enabled = true;


            if (captureDevice.IsRunning)
                captureDevice.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(comboBox4.Text))
            {
                MessageBox.Show("ID / Name / Gender Food Type cannot be  empty.");
                return;
            }

            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "insert into stdDetails values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + comboBox2.Text + "','" + comboBox3.Text + "','" + comboBox4.Text + "')";
            cmd.ExecuteNonQuery();
            con.Close();
            //disp_data();

            clearData();

            MessageBox.Show("Student Details saved succussfully..");
            hideitems();

        }

       

        

        private void clearData()
        {

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";


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
            con.Close();
            return dt;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }


        private void LoadComboBoxItems()
        {
            string filePath = "C:\\innotics\\school.txt";
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        comboBox3.Items.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items from file: " + ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
          

           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook |.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (IXLWorkbook workbook = new XLWorkbook())
                        {
                            DataTable dt = GetData();
                            workbook.Worksheets.Add(dt, "std details");
                            workbook.SaveAs(sfd.FileName);
                        }
                        MessageBox.Show("Save successful", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           

        }

        private void button7_Click(object sender, EventArgs e)
        {
            clearData();
            
        }

        private void button6_Click(object sender, EventArgs e)
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



                clearData();
                hideitems();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
