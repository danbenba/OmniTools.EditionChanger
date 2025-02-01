using System;
using System.Drawing;

namespace OmniTools
{
    /// <summary>
    /// Classe utilitaire pour la gestion des logs.
    /// </summary>
    public static class Logger
    {
        public static MainForm MainFormInstance { get; set; }

        /// <summary>
        /// Génère un timestamp au format [HH:mm:ss].
        /// </summary>
        private static string GetTimestamp()
        {
            return $"[{DateTime.Now:HH:mm:ss}]";
        }

        /// <summary>
        /// Log un message d'information.
        /// </summary>
        public static void LogInfo(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [INFO] {message}", Color.Black);
        }

        /// <summary>
        /// Log un message de succès.
        /// </summary>
        public static void LogSuccess(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [SUCCESS] {message}", Color.Green);
        }

        /// <summary>
        /// Log un message d'avertissement.
        /// </summary>
        public static void LogWarning(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [WARNING] {message}", Color.Orange);
        }

        /// <summary>
        /// Log un message d'erreur.
        /// </summary>
        public static void LogError(string message)
        {
            MainFormInstance?.AddLog($"{GetTimestamp()} [ERROR] {message}", Color.Red);
        }

        /// <summary>
        /// Efface tous les logs du RichTextBox.
        /// </summary>
        public static void Clear()
        {
            if (MainFormInstance?.richTextBoxLogs.InvokeRequired == true)
            {
                MainFormInstance.richTextBoxLogs.Invoke(new Action(() => MainFormInstance.richTextBoxLogs.Clear()));
            }
            else
            {
                MainFormInstance?.richTextBoxLogs.Clear();
            }
        }
    }
}
