using System;

namespace OmniTools
{
    /// <summary>
    /// Représente la configuration complète de l’application :
    /// onglets et boutons associés.
    /// </summary>
    public static class AppConfig
    {
        // Tableau d'onglets (4 dans cet exemple).
        public static TabConfig[] Tabs = new TabConfig[]
        {
            new TabConfig
            {
                Title = "Activators",
                Buttons = new ButtonConfig[]
                {
                    new ButtonConfig 
                    { 
                        Title = "Change\nWindows Edition", 
                        Url = "https://raw.githubusercontent.com/massgravel/Microsoft-Activation-Scripts/refs/heads/master/MAS/Separate-Files-Version/Change_Windows_Edition.cmd", 
                        Arguments = "" , 
                        CustomHeight = 60,
                        CustomWidth = 200
                    },
                    new ButtonConfig 
                    { 
                        Title = " ", 
                        Url = " ", 
                        Arguments = "", 
                        CustomHeight = 60,
                        CustomWidth = 200
                    },
                    new ButtonConfig 
                    { 
                        Title = "Change\nOffice Edition", 
                        Url = "https://raw.githubusercontent.com/massgravel/Microsoft-Activation-Scripts/refs/heads/master/MAS/Separate-Files-Version/Change_Office_Edition.cmd", 
                        Arguments = "", 
                        CustomHeight = 60,
                        CustomWidth = 200
                    },
                }
            },
        };
    }

    /// <summary>
    /// Représente un onglet (nom + liste de boutons).
    /// </summary>
    public class TabConfig
    {
        public string Title { get; set; }
        public ButtonConfig[] Buttons { get; set; }
    }

    /// <summary>
    /// Représente la configuration d’un bouton (texte, URL, arguments...).
    /// </summary>
    public class ButtonConfig
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Arguments { get; set; }

        // --- Nouveaux champs pour personnalisation ---
        public int? CustomWidth { get; set; }  // Largeur personnalisée
        public int? CustomHeight { get; set; } // Hauteur personnalisée
        public bool CenterHorizontally { get; set; } // Centrage horizontal
    }
}
