using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using App.Model;

namespace App
{
    public partial class FrmTraitement : Form
    {
        public FrmTraitement()
        {
            InitializeComponent();
        }
        

        private void closebtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void label7_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                // Afficher un message d'avertissement pour informer l'utilisateur qu'il doit supprimer le répertoire existant avant de sélectionner un nouveau.
                DialogResult result = MessageBox.Show("Vous avez déjà sélectionné un répertoire. Voulez-vous supprimer le répertoire existant avant de sélectionner un nouveau ?", "Avertissement", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Effacer le contenu du champ de texte.
                    textBox1.Text = "";
                }
                else
                {
                    // Sortir de la méthode sans rien faire.
                    return;
                }
            }

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                // Afficher la boîte de dialogue pour la sélection du répertoire
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // Afficher le chemin du répertoire sélectionné dans le TextBox associé au Label
                    textBox1.Text = dialog.SelectedPath;
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                // Afficher un message d'avertissement pour informer l'utilisateur qu'il doit supprimer le répertoire existant avant de sélectionner un nouveau.
                DialogResult result = MessageBox.Show("Vous avez déjà sélectionné un répertoire. Voulez-vous supprimer le répertoire existant avant de sélectionner un nouveau ?", "Avertissement", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Effacer le contenu du champ de texte.
                    textBox2.Text = "";
                }
                else
                {
                    // Sortir de la méthode sans rien faire.
                    return;
                }
            }

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                // Afficher la boîte de dialogue pour la sélection du répertoire
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // Afficher le chemin du répertoire sélectionné dans le TextBox associé au Label
                    textBox2.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnCopie_Click(object sender, EventArgs e)
        {
            // Vérification si la requête est valide
            DAL dal = new DAL();
            bool success = true; // variable pour vérifier si aucune erreur n'a été détectée

            try
            {
                string sourceDirectory = textBox1.Text.Trim();
                string targetDirectory = textBox2.Text.Trim();
                string querySql = "";
                if (radioButton1.Checked)
                {
                    querySql = textBox3.Text.Trim();
                }
                else if (radioButton2.Checked)
                {
                    querySql = dal.GetNumeroDossierOrbus(textBox3.Text.Trim());
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un bouton radio!");
                    success = false; // mise à false si une erreur est détectée

                    return;
                }

                if (string.IsNullOrWhiteSpace(sourceDirectory) || string.IsNullOrWhiteSpace(targetDirectory) || string.IsNullOrWhiteSpace(querySql))
                {
                    MessageBox.Show("Veuillez saisir les répertoires source, destination et la requête!");
                    success = false; // mise à false si une erreur est détectée

                }

                // Vérification si les répertoires source et destination existent
                if (!Directory.Exists(sourceDirectory))
                {
                    MessageBox.Show("Le répertoire source n'existe pas!");
                    success = false; // mise à false si une erreur est détectée

                }

                if (!Directory.Exists(targetDirectory))
                {
                    success = false; // mise à false si une erreur est détectée

                }


                {
                    DataTable dt = dal.GetData(querySql);

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        MessageBox.Show("La requête est invalide ou ne retourne aucun résultat!");
                        success = false; // mise à false si une erreur est détectée
                    }

                    List<int> listeDossier = new List<int>();
                   
                    foreach (DataRow row in dt.Rows)
                    {
                        int numeroDossierOrbus;
                        if (!int.TryParse(row["numeroDossierOrbus"].ToString(), out numeroDossierOrbus))
                        {
                            MessageBox.Show("Le numéro de dossier est invalide ou n'est pas un nombre entier!");
                            success = false; // mise à false si une erreur est détectée
                        }
                        listeDossier.Add(numeroDossierOrbus);
                    }

                    if (success) // affichage du message si aucune erreur n'a été détectée
                    {
                        dal.CopyFiles(sourceDirectory, targetDirectory, listeDossier);
                        MessageBox.Show("Copie des repertoires terminés avec succès!");
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {

        }

         
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox3.Enabled = true;
                label9.Visible = true;
                label9.Text = "*alias NumeroDossierTPS numeroDossierOrbus";
                textBox3.Text = "";
            }
            else if (radioButton2.Checked)
            {
                textBox3.Enabled = true;
                label9.Visible = true;
                label9.Text = "*Séparer les numero par ',' ou ';'";
                textBox3.Text = "";
            }
        }

   
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox3.Enabled = true;
                label9.Visible = true;
                label9.Text = "*Séparer les numero par ',' ou ';'";
                textBox3.Text = "";
            }
            else if (radioButton1.Checked)
            {
                textBox3.Enabled = true;
                label9.Visible = true;
                label9.Text = "*alias NumeroDossierTPS numeroDossierOrbus";
                textBox3.Text = "";
            }
        }



        private void FrmTraitement_Load(object sender, EventArgs e)
        {
            // Vérifier l'état initial des boutons radio et désactiver le champ si aucun bouton n'est sélectionné
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                textBox3.Enabled = false;
                label9.Visible = false;
            }
        }
    }
}
