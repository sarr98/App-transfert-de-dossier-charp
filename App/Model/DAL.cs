using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data;


namespace App.Model
{
    public class DAL
    {
        public string GetNomFormulaire(string codeFormulaire)
        {
            string constring = ConfigurationManager.ConnectionStrings["AdminConn"].ToString();
            string nomFormulaire = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(constring))
                {
                    connection.Open();
                    string query = "SELECT NOMFORMULAIRE FROM FORMULAIRE WHERE CODEFORMULAIRE =@CodeFormulaire";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CodeFormulaire", codeFormulaire);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nomFormulaire = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Une exception s'est produite : {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
            }

            return nomFormulaire;
        }


        public void RenommerFichier(string sourceDirectory, string targetDirectory)
        {
            try
            {
                // Vérifier si le répertoire source existe
                if (!Directory.Exists(sourceDirectory))
                {
                    MessageBox.Show("Le répertoire source n'existe pas : " + sourceDirectory);
                }

                // Vérifier si le répertoire destination existe, sinon le créer
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // Copier tous les fichiers du répertoire source vers le répertoire destination
                string[] files = Directory.GetFiles(sourceDirectory, "*.*");
                Dictionary<string, int> codeFormulaireCounts = new Dictionary<string, int>(); // Utilisé pour stocker le nombre de fichiers avec le même code de formulaire
                foreach (string file in files)
                {
                    // Vérifier l'extension du fichier
                    string extension = Path.GetExtension(file).ToLowerInvariant();
                    if (extension == ".xml")
                    {
                        // Si l'extension est .xml, passer au fichier suivant
                        continue;
                    }

                    // Obtenir le nom du fichier sans le chemin d'accès
                    string fileName = Path.GetFileName(file);

                    // Extraire le code de formulaire à partir du nom de fichier
                    string codeFormulaire = GetCodeFormulaire(fileName);

                    // Extraire le nom de formulaire à partir du code de formulaire
                    string nomFormulaire = GetNomFormulaire(codeFormulaire);

                    // Remplacer les espaces vides par des underscores dans le nom de formulaire
                    nomFormulaire = nomFormulaire.Replace(" ", "_");

                    // Initialiser le compteur pour ce code de formulaire à 1
                    int codeFormulaireCount = 1;

                    // Vérifier si nous avons déjà rencontré un fichier avec le même code de formulaire
                    if (codeFormulaireCounts.ContainsKey(codeFormulaire))
                    {
                        // Si oui, utiliser le compteur existant pour incrémenter le nom de fichier
                        codeFormulaireCount = codeFormulaireCounts[codeFormulaire] + 1;
                        codeFormulaireCounts[codeFormulaire] = codeFormulaireCount;
                    }
                    else
                    {
                        // Si non, enregistrer le compteur pour ce code de formulaire
                        codeFormulaireCounts[codeFormulaire] = codeFormulaireCount;
                    }

                    // Diviser le nom de fichier en parties séparées par des underscores
                    string[] fileNameParts = fileName.Split('_');

                    // Obtenir le nom de pôle à partir du troisième élément du nom de fichier, s'il est présent
                    string nomPole = null;
                    if ((fileNameParts.Length > 2) && (!fileNameParts[1].ToString().Contains("G")))
                    {
                        string idPoleString = fileNameParts[2].ToString().Split('.')[0];
                        int idPole = int.Parse(idPoleString);
                        nomPole = GetNomPole(idPole);
                    }

                    // Obtenir le dossier TPS à partir du premier élément du nom de fichier
                    string dossierTPS = fileNameParts[0];

                    // Construire le nouveau nom de fichier
                    string newFileName = nomFormulaire;
                    if (nomPole != null)
                    {
                        newFileName = newFileName + "_" + nomPole;
                    }
                    if (codeFormulaireCounts[codeFormulaire] > 1)
                    {
                        newFileName += "_" + codeFormulaireCounts[codeFormulaire];
                    }
                    newFileName = dossierTPS + "_" + newFileName + ".pdf";

                    // Obtenir le chemin complet de la destination du fichier
                    string targetPath = Path.Combine(targetDirectory, newFileName);

                    // Copier le fichier source vers la destination en renommant
                    File.Copy(file, targetPath);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
            }
        }


        //////public string GetCodeFormulaire(string filename)
        //////{
        //////    try
        //////    {
        //////        string codeFormulaire = "";
        //////        if (!filename.Contains("G")) 
        //////        {
        //////            string[] parts = Path.GetFileNameWithoutExtension(filename).Split('_');
        //////            codeFormulaire = parts[1];      
        //////        }
        //////        else 
        //////        {
        //////            string[] parts = Path.GetFileNameWithoutExtension(filename).Split('_');
        //////            string codeG = parts[1];
        //////            string[] parts2 = codeG.Split('G');
        //////            codeFormulaire = parts2[0];
        //////        }
        //////        return codeFormulaire;
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        MessageBox.Show($"Une exception s'est produite : {ex.Message}");
        //////        return "";
        //////    }
        //////}

        public string GetCodeFormulaire(string filename)
        {
            try
            {
                string[] parts = Path.GetFileNameWithoutExtension(filename).Split('_');
                string codeG = parts[1];
                string codeFormulaire = codeG.Split('G')[0];
                if (string.IsNullOrEmpty(codeFormulaire))
                {
                    codeFormulaire = parts[1];
                }
                return codeFormulaire;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une exception s'est produite : {ex.Message}");
                return "";
            }
        }

        public string GetNomPole(int idPole)
        {
            string constring = ConfigurationManager.ConnectionStrings["AdminConn"].ToString();
            string query = "SELECT NOMPOLE FROM POLES WHERE IDPOLE = @idPole";

            using (SqlConnection connection = new SqlConnection(constring))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idPole", idPole);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return result.ToString();
                }
                else
                {
                    return idPole.ToString();
                }
            }
        }


        public void CopyFiles(string sourceDirectory, string targetDirectory, List<int> listeDossier, bool messageBoxOnEmptyList = false)
        {
            try
            {
                if (listeDossier == null || listeDossier.Count == 0)
                {
                    if (messageBoxOnEmptyList)
                    {
                        MessageBox.Show("La liste des dossiers est vide.");
                    }
                    return;
                }

                foreach (int numeroDossier in listeDossier)
                {
                    string fullNameDossierSource = Path.Combine(sourceDirectory, numeroDossier.ToString());
                    if (!Directory.Exists(fullNameDossierSource))
                    {
                        continue;
                    }

                    string fullNameDossierDest = Path.Combine(targetDirectory, numeroDossier.ToString());
                    RenommerFichier(fullNameDossierSource, fullNameDossierDest);
                }
            }
            catch (Exception ex)
            {
                // Gestion de l'exception
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public DataTable GetData(string query)
        {
            string constring = ConfigurationManager.ConnectionStrings["AdminConn"].ToString();
            try
            {
                SqlConnection sqlCon = new SqlConnection(constring);
                if (sqlCon.State == ConnectionState.Closed) { sqlCon.Open(); }
                DataSet tempDS = new DataSet();
                SqlDataAdapter tempDA = new SqlDataAdapter(string.Format(query), sqlCon);
                tempDA.Fill(tempDS, "ResultSet");
                DataTable resultSet = tempDS.Tables["ResultSet"];

                sqlCon.Close();

                return resultSet;

            }
            catch (Exception ex)
            {
                throw new Exception("GetData: La requete suivante a ramené NULL: \n query=" +query + "\n Erreur =>" + ex.Message);
            }

        }


        public string GetNumeroDossierOrbus(string numerodossiers)
        {
            try
            {
                // Remplacer les caractères spéciaux et les espaces par des virgules
                numerodossiers = Regex.Replace(numerodossiers, @"[^\w]+", ",");

                // Diviser la chaîne numerodossiers en un tableau de chaînes en utilisant la virgule comme séparateur
                string[] numerodossiersArray = numerodossiers.Split(',');

                // les joindre ensemble en utilisant une virgule comme séparateur
                string parameters = string.Join(", ", numerodossiersArray.Select(n => n.ToString()));

                // Créer la requête SQL en utilisant les paramètres
                string query = $"SELECT NUMERODOSSIERTPS numeroDossierOrbus FROM DOSSIERTPS WHERE NUMERODOSSIERTPS IN ({parameters})";

                // Retourner la requête SQL
                return query;
            }
            catch (Exception ex)
            {
                // Si une exception est levée, la capturer et afficher le message d'erreur
                MessageBox.Show("Une exception s'est produite: " + ex.Message);
                return null;
            }
        }


    }


}
