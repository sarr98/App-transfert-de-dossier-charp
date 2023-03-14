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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Configuration;

namespace App
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
            panel3.BackColor = Color.White;
            panel4.BackColor = SystemColors.Control;
            textBox2.BackColor = SystemColors.Control;
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.BackColor= Color.White;
            panel4.BackColor = Color.White;
            textBox1.BackColor = SystemColors.Control;  
            panel3.BackColor = SystemColors.Control;   
        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {
            textBox2.UseSystemPasswordChar = false; 
        }

        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            textBox2.UseSystemPasswordChar = true;
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            string constring = ConfigurationManager.ConnectionStrings["AdminConn"].ToString();

            SqlConnection connection = new SqlConnection(constring);
            connection.Open();

            string login = textBox1.Text;
            string password = textBox2.Text;

            // Valider l'adresse e-mail de l'utilisateur
            if (!IsValidEmail(login))
            {
                MessageBox.Show("Veuillez saisir une adresse login valide.");
                return;
            }

            // Vérifier les informations d'identification de l'utilisateur
            string query = "SELECT COUNT(*) FROM Users WHERE login=@login AND password=@password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@password", password);

            int count = (int)command.ExecuteScalar();

            if (count == 1)
            {
                // Rediriger l'utilisateur vers la page de traitement
                FrmTraitement traitement = new FrmTraitement();
                traitement.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Login ou Password incorrect.");
            }

            connection.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
