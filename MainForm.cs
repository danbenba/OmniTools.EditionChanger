using System; 
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniTools
{
    public partial class MainForm : Form
    {
        public RichTextBox richTextBoxLogs;
        private TabControl tabControl;

        // Nouveau : dossier personnalisé
        private string customDownloadFolder = string.Empty;

        // Nouveau : activer ou non les logs détaillés
        private bool detailedLogs = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.Text = "OmniTools Launcher";
            this.Size = new Size(532, 335);

            // Création du RichTextBox pour logs
            richTextBoxLogs = new RichTextBox
            {
                Dock = DockStyle.Bottom,
                Height = 150
            };
            this.Controls.Add(richTextBoxLogs);

            // Création du TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            // On le met au-dessus du RichTextBox
            this.Controls.Add(tabControl);
            this.Controls.SetChildIndex(tabControl, 0); // Assurez-vous que le TabControl est en premier

            // Ajout d’un label en bas pour la version
            Label versionLabel = new Label
            {
                Text = "Version 1.0.0",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(versionLabel);
            this.Controls.SetChildIndex(versionLabel, 0);
        }

        /// <summary>
        /// Initialise les onglets et boutons à partir de la config AppConfig + onglet Paramètres.
        /// </summary>
        private void InitializeCustomComponents()
        {
            // Parcourir tous les onglets définis dans la config
            foreach (var tabConfig in AppConfig.Tabs)
            {
                // Créer un TabPage pour chaque onglet
                var tabPage = new TabPage(tabConfig.Title);

                // Création d'un TableLayoutPanel pour gérer les boutons
                var tableLayoutPanel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    ColumnCount = 3, // Par exemple, 3 colonnes pour 3 boutons par ligne
                    RowCount = 0, // Dynamique
                    Padding = new Padding(20),
                };

                // Définir les styles des colonnes pour qu'elles prennent toutes la même largeur
                for (int i = 0; i < 3; i++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                }

                int currentColumn = 0;
                int currentRow = 0;

                foreach (var buttonConfig in tabConfig.Buttons)
                {
                    // Si c'est le début d'une nouvelle ligne
                    if (currentColumn == 0)
                    {
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        tableLayoutPanel.RowCount += 1;
                    }

                    // Créer le bouton
                    var button = new Button
                    {
                        Text = buttonConfig.Title,
                        Width = buttonConfig.CustomWidth ?? 120,
                        Height = buttonConfig.CustomHeight ?? 40,
                        Margin = new Padding(10),
                        Anchor = AnchorStyles.None // Pour centrer dans la cellule
                    };

                    // Ajouter un handler de clic qui va télécharger et exécuter
                    button.Click += async (sender, e) =>
                    {
                        await HandleButtonClickAsync(button, buttonConfig);
                    };

                    // Ajouter le bouton au TableLayoutPanel
                    tableLayoutPanel.Controls.Add(button, currentColumn, currentRow);

                    // Gérer le centrage si nécessaire
                    if (buttonConfig.CenterHorizontally)
                    {
                        // Fusionner les cellules de la colonne actuelle à la fin pour centrer
                        tableLayoutPanel.SetColumnSpan(button, 3); // Occupe toute la rangée
                        button.Anchor = AnchorStyles.None;
                        // Optionnel : ajuster la position ou ajouter un Panel pour un meilleur contrôle
                    }

                    currentColumn++;
                    if (currentColumn >= 3)
                    {
                        currentColumn = 0;
                        currentRow++;
                    }
                }

                // Ajouter le TableLayoutPanel au TabPage
                tabPage.Controls.Add(tableLayoutPanel);

                // Ajouter la page d'onglet au TabControl
                tabControl.TabPages.Add(tabPage);
            }

            // =========== ON AJOUTE ICI NOTRE ONGLET "Paramètres" ===========
            //CreateSettingsTab();
        }

        /// <summary>
        /// Crée et configure l’onglet Paramètres.
        /// </summary>
        private void CreateSettingsTab()
        {
            var settingsTabPage = new TabPage("Paramètres");
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(20), // Ajoute du padding pour l'esthétique
            };
            settingsTabPage.Controls.Add(panel);

            // Gestionnaire d'événements pour le redimensionnement afin de centrer les contrôles
            panel.Resize += (s, e) =>
            {
                CenterControls(panel);
            };

            // Label pour informer
            Label lblInfo = new Label
            {
                Text = "Choisissez votre dossier de téléchargement personnalisé :",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            panel.Controls.Add(lblInfo);

            // Bouton pour choisir un dossier
            Button btnChooseFolder = new Button
            {
                Text = "Choisir le dossier",
                Width = 150,
                Height = 30,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnChooseFolder.Click += BtnChooseFolder_Click;
            panel.Controls.Add(btnChooseFolder);

            // CheckBox pour activer/désactiver les logs détaillés
            CheckBox cbDetailedLogs = new CheckBox
            {
                Text = "Activer les logs détaillés",
                AutoSize = true,
                Checked = false, // Par défaut
                Margin = new Padding(0, 0, 0, 10)
            };
            cbDetailedLogs.CheckedChanged += (s, e) =>
            {
                detailedLogs = cbDetailedLogs.Checked;
                Logger.LogInfo("Logs détaillés = " + detailedLogs);
            };
            panel.Controls.Add(cbDetailedLogs);

            // Bouton pour nettoyer le dossier
            Button btnCleanFolder = new Button
            {
                Text = "Nettoyer le dossier",
                Width = 150,
                Height = 30,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnCleanFolder.Click += BtnCleanFolder_Click;
            panel.Controls.Add(btnCleanFolder);

            // Après l'ajout des contrôles, centrer initialement les contrôles
            CenterControls(panel);

            // Ajouter la page "Paramètres" au TabControl
            tabControl.TabPages.Add(settingsTabPage);
        }

        /// <summary>
        /// Centre les contrôles dans le FlowLayoutPanel (utilisé pour l'onglet Paramètres).
        /// </summary>
        /// <param name="panel">Le FlowLayoutPanel contenant les contrôles.</param>
        private void CenterControls(FlowLayoutPanel panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is Button || control is CheckBox || control is Label)
                {
                    // Créer un wrapper pour centrer chaque contrôle
                    Control wrapper = new Control
                    {
                        Width = panel.ClientSize.Width,
                        Height = control.Height,
                        Margin = new Padding(0, 5, 0, 5)
                    };
                    panel.Controls.SetChildIndex(control, 0); // Remettre le contrôle en haut pour insérer le wrapper

                    // Supprimer le contrôle du panel et l'ajouter au wrapper
                    panel.Controls.Remove(control);
                    wrapper.Controls.Add(control);
                    panel.Controls.Add(wrapper);
                }
            }
        }

        /// <summary>
        /// Handler pour le bouton "Choisir le dossier".
        /// </summary>
        private void BtnChooseFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    customDownloadFolder = fbd.SelectedPath;
                    Logger.LogSuccess("Dossier personnalisé sélectionné: " + customDownloadFolder);
                }
            }
        }

        /// <summary>
        /// Handler pour le bouton "Nettoyer le dossier".
        /// Supprime tous les .exe du dossier choisi ou de %TEMP%.
        /// </summary>
        private void BtnCleanFolder_Click(object sender, EventArgs e)
        {
            string folderToClean = string.IsNullOrEmpty(customDownloadFolder)
                ? Path.GetTempPath()
                : customDownloadFolder;

            try
            {
                string[] files = Directory.GetFiles(folderToClean, "*.exe");
                int deletedCount = 0;
                foreach (string file in files)
                {
                    File.Delete(file);
                    deletedCount++;
                }

                Logger.LogSuccess($"Nettoyage terminé. {deletedCount} fichier(s) .exe supprimé(s) dans {folderToClean}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Erreur lors du nettoyage: {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode appelée pour gérer le clic sur un bouton de téléchargement/exécution.
        /// Elle télécharge, exécute, puis supprime le fichier.
        /// 
        /// Modifié pour exécuter le fichier .cmd dans une nouvelle fenêtre cmd via cmd.exe /C.
        /// </summary>
        private async Task HandleButtonClickAsync(Button button, ButtonConfig config)
        {
            // Sauvegarder le texte d'origine du bouton
            string originalText = button.Text;

            try
            {
                // Désactiver le bouton et changer son texte
                button.Enabled = false;
                button.Text = "Téléchargement...";

                // Chemin du fichier temporaire ou personnalisé
                string folder = string.IsNullOrEmpty(customDownloadFolder)
                    ? Path.GetTempPath()
                    : customDownloadFolder;

                // Assurez-vous que le dossier existe
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Path.Combine(folder, Path.GetFileName(config.Url));

                // Logs
                if (detailedLogs)
                {
                    Logger.LogInfo($"Téléchargement de l'URL : {config.Url}");
                    Logger.LogInfo($"Vers le fichier : {fileName}");
                }
                else
                {
                    Logger.LogInfo($"Téléchargement de {config.Url}");
                }

                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(config.Url), fileName);
                }

                // Étape 2 : Lancer le fichier .cmd dans une nouvelle fenêtre cmd
                button.Text = "Lancement...";
                if (detailedLogs)
                {
                    Logger.LogInfo($"Exécution de {fileName} avec arguments: {config.Arguments}");
                }
                else
                {
                    Logger.LogInfo($"Exécution de {Path.GetFileName(fileName)}");
                }

                // Utilisation de cmd.exe pour exécuter le fichier .cmd via "/C"
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C \"{fileName}\" {config.Arguments}",
                    UseShellExecute = true,
                    CreateNoWindow = false, // Affiche la fenêtre de commande
                    WindowStyle = ProcessWindowStyle.Normal
                };

                Process process = Process.Start(psi);

                // Optionnel : attendre la fin du process
                if (process != null)
                {
                    await Task.Run(() => process.WaitForExit());
                }

                // Étape 3 : Supprimer le fichier (si on veut le supprimer même après exécution)
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    Logger.LogInfo($"Fichier supprimé : {fileName}");
                }

                // Log final
                Logger.LogSuccess($"Exécution terminée pour {config.Title}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Erreur lors du traitement du bouton {config.Title} : {ex.Message}");
            }
            finally
            {
                // Rétablir l'état du bouton
                button.Text = originalText;
                button.Enabled = true;
            }
        }

        /// <summary>
        /// Méthode utilisée par la classe Logger pour ajouter du texte dans la RichTextBox.
        /// </summary>
        public void AddLog(string message, Color color)
        {
            if (this.richTextBoxLogs.InvokeRequired)
            {
                this.richTextBoxLogs.Invoke(new Action(() => AddLog(message, color)));
            }
            else
            {
                // On insère un nouveau message avec la couleur indiquée
                int start = richTextBoxLogs.TextLength;
                richTextBoxLogs.AppendText(message + Environment.NewLine);
                int end = richTextBoxLogs.TextLength;

                // Sélectionne le texte qu’on vient d’ajouter pour changer la couleur
                richTextBoxLogs.Select(start, end - start);
                richTextBoxLogs.SelectionColor = color;

                // Remet la sélection à la fin
                richTextBoxLogs.Select(richTextBoxLogs.TextLength, 0);
                richTextBoxLogs.ScrollToCaret();
            }
        }
    }
}
