using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

// Ajoutons l'attribut pour spécifier que nous ciblons Windows
[SupportedOSPlatform("windows")]
class Program
{
    private const string APP_VERSION = "1.4.0";
    
    // Constantes et chemins
    private const string ES_SYSTEMS_DIR = ".es_systems";
    private const string CONFIG_FILE = ".config";
    private static readonly string WorkingDirectory = Path.GetFullPath(
        Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\plugins\\DemulShooter"));

    // Nouvelle méthode pour tuer les processus
    private static void KillProcesses(string[] processesToKill)
    {
        foreach (string processName in processesToKill)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName));
                foreach (Process process in processes)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la tentative de fermeture de {processName}: {ex.Message}");
            }
        }
    }

    // Nouvelle méthode pour lancer un processus caché
    private static void StartHiddenProcess(string fileName, string? workingDir = null, string? arguments = null)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            
            if (!string.IsNullOrEmpty(workingDir))
                startInfo.WorkingDirectory = workingDir;
                
            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            using var process = Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            LogMessage(Path.Combine(WorkingDirectory, $"{ES_SYSTEMS_DIR}\\game-start-ES.log"), 
                $"Erreur lors du lancement de {fileName}: {ex.Message}");
        }
    }

    private static void LogMessage(string logFilePath, string message, bool includeTimestamp = true)
    {
        try
        {
            string logMessage = includeTimestamp 
                ? $"{DateTime.Now}: {message}\n"
                : $"{message}\n";
            File.AppendAllText(logFilePath, logMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur d'écriture du log: {ex.Message}");
        }
    }

    // Ajouter en haut du fichier
    private static IniFile? configIni;
    private static IniFile? romsIni;

    private class RomConfig
    {
        public string? GameName { get; set; }
        public string? RomName { get; set; }
        public string? Version { get; set; }
        public string? Target { get; set; }
        public string? Delay { get; set; }
        public string? Arguments { get; set; }
        public string? JoyToKey { get; set; }
        public string? NoMousy { get; set; }
    }

    private static RomConfig? GetRomConfig(string section, string romKey)
    {
        if (romsIni == null) return null;
        
        string configLine = romsIni.ReadValue(section, romKey, "");
        if (string.IsNullOrEmpty(configLine)) return null;

        var values = configLine.Split('|');
        if (values.Length < 8) return null;

        return new RomConfig
        {
            GameName = values[0].Trim(),
            RomName = values[1].Trim(),
            Version = values[2].Trim(),
            Target = values[3].Trim(),
            Delay = values[4].Trim(),
            Arguments = values[5].Trim(),
            JoyToKey = values[6].Trim(),
            NoMousy = values[7].Trim()
        };
    }

    static void Main(string[] args)
    {
        try
        {
            // Utiliser WorkingDirectory au lieu de redéclarer workingDirectory
            string configPath = Path.Combine(WorkingDirectory, ".es_systems\\.config");
            string romsPath = Path.Combine(WorkingDirectory, ".es_systems\\roms.ini");
            
            configIni = new IniFile(configPath);
            romsIni = new IniFile(romsPath);

            // Définir les processus à rechercher et terminer
            string[] processesToKill = { 
                "DemulShooter.exe", "DemulShooterX64.exe", 
                "JoyToKey.exe", 
                "AutoHotkey.exe", "AutoHotkeyU64.exe", "AutoHotkeyUX.exe", 
                "kill.exe" 
            };

            KillProcesses(processesToKill);

            // Lire le fichier .config
            string configFilePathpush = Path.Combine(WorkingDirectory, ".es_systems\\.config");
            if (!File.Exists(configFilePathpush))
            {
                return;
            }

            var configLines = File.ReadAllLines(configFilePathpush);
            string DSUpdtlock = "";
            string currentSectionpush = "";

            // Parcourir chaque ligne du fichier .config
            foreach (var line in configLines)
            {
                // Vérifier si la ligne est une section
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // Extraire le nom de la section
                    currentSectionpush = line.Trim('[', ']');
                    continue;
                }

                // Si la ligne n'est pas une section et que la section actuelle est [config], extraire la valeur de DSUpdtlock
                if (currentSectionpush == "config" && line.StartsWith("DSUpdtlock="))
                {
                    DSUpdtlock = line.Split('=')[1].Trim().ToLower();
                    break;
                }
            }

            // Exécuter HIDgetpush.exe et getput.exe seulement si DSUpdtlock=true
            if (DSUpdtlock == "true")
            {
                // Exécuter HIDgetpush.exe
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"{WorkingDirectory}\\HIDgetpush.exe",
                    WorkingDirectory = WorkingDirectory,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                Thread.Sleep(1000); // Attendre 1 secondes

                // Exécuter getput.exe
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"{WorkingDirectory}\\getput.exe",
                    WorkingDirectory = WorkingDirectory,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                Thread.Sleep(2000); // Attendre 2 secondes
            }

            // Vérifier si le fichier .locked est présent
            string lockedFilePath = Path.Combine(WorkingDirectory, ".locked");
            if (!File.Exists(lockedFilePath))
            {
                // Si le fichier .locked n'est pas présent, exécuter GUIDemulshooter pour le paramétrage et créer le fichier .locked
                Process.Start(Path.Combine(WorkingDirectory, "DemulShooter_GUI.exe"));
                File.WriteAllText(lockedFilePath, "");
            }
            else
            {
                Console.WriteLine("Le fichier .locked est déjà présent. Ne pas exécuter GUIDemulshooter.");
            }

            // Exécuter Check.bat
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C start \"Check\" /B \"{WorkingDirectory}\\.es_systems\\Check.bat\"",
                UseShellExecute = false, // Modifier à false
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true // Ajouter cette ligne
            });

            // Définir le chemin du fichier de log
            string logFilePath = Path.Combine(WorkingDirectory, ".es_systems\\game-start-ES.log");

            // Supprimer le fichier s'il existe
              if (File.Exists(logFilePath))
              {
                 File.Delete(logFilePath);
            }

            // 2 - Charger l'argument
            string gameExecuted = string.Join(" ", args );

            // Extraire le nom du fichier sans l'extension
            string fileName = Path.GetFileNameWithoutExtension(gameExecuted);

            // Vérifier s'il y a une double extension dans le nom de fichier
            if (fileName.Contains('.'))
            {
                // Si oui, prendre la première partie comme nom de fichier
                fileName = fileName.Split('.')[0];
            }

            // Filtrer pour retirer toute extension restante
            int extensionIndex = fileName.LastIndexOf('.');
            if (extensionIndex != -1)
            {
                fileName = fileName.Split('.')[0];
            }



            // 3 - Établir une liste de prérequis de dossiers systems
            string configFilePath = Path.Combine(WorkingDirectory, ".es_systems\\.config");
            if (!File.Exists(configFilePath))
            {
                return;
            }

            var lines = File.ReadAllLines(configFilePath);
            bool debugMode = false;
            List<string> requiredDirectories = new();
            string currentSectionConfig = "";

            foreach (var line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSectionConfig = line.Trim('[', ']');
                    continue;
                }

                if (currentSectionConfig == "config" && line.StartsWith("debug="))
                {
                    debugMode = line.Split('=')[1].Trim().ToLower() == "true";
                }
                else if (currentSectionConfig == "systems")
                {
                    requiredDirectories.Add(line);
                }
            }

            foreach (var dir in requiredDirectories)
            {
                if (!Directory.Exists(Path.Combine(WorkingDirectory, ".es_systems", dir)))
                {
                    return;
                }
            }


            // Afficher le nom du jeu exécuté si le mode de débogage est activé
            if (debugMode)
            {
                    LogMessage(logFilePath, $"Nom de jeu exécuté : {fileName}", false);
            }

            // 4 - Lire le fichier roms.ini
            string romsFilePath = Path.Combine(WorkingDirectory, ".es_systems\\roms.ini");
            if (!File.Exists(romsFilePath))
            {
                return;
            }

            var romsLines = File.ReadAllLines(romsFilePath);

            // Variables pour gérer les sections et le nom du jeu
            string currentSection = "";
            string gameName;
            string rom_demulshooter = "";
            string version_demulshooter = "";
            string target_value = "";
            string delay_seconds = "";
            string arguments_extra = "";
            string joytokey_flag = "";
            string nomousy_flag = "";

            // Parcourir chaque ligne du fichier roms.ini
            foreach (var line in romsLines)
            {
                // Vérifier si la ligne est une section
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // Extraire le nom de la section
                    currentSection = line.Trim('[', ']');
                    continue;
                }

                // Si la ligne n'est pas une section, extraire le nom du jeu
                var parts = line.Split('=');
                if (parts.Length >= 2)
                {
                    var gameParts = parts[1].Split('|');
                    gameName = gameParts[0].Trim();
                    if (gameParts.Length >= 8)
                    {
                        rom_demulshooter = gameParts[1].Trim();
                        version_demulshooter = gameParts[2].Trim();
                        target_value = gameParts[3].Trim();
                        delay_seconds = gameParts[4].Trim();
                        arguments_extra = gameParts[5].Trim();
                        joytokey_flag = gameParts[6].Trim();
                        nomousy_flag = gameParts[7].Trim();
                    }

                    // Ajouter le nom de jeu extrait au fichier de log si le mode de débogage est activé
                    if (debugMode)
                    {
                            LogMessage(logFilePath, $"Nom de jeu extrait : {gameName}", false);
                    }

                    // Vérifiez si les noms de jeu correspondent
                    if (string.Equals(gameName.Trim(), fileName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        // Obtenir l'horodatage actuel en France
                        TimeZoneInfo franceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                        DateTime franceTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, franceTimeZone);

                        // Afficher les informations demandées avec l'horodatage
                            LogMessage(logFilePath, $"Le jeu {fileName} correspond à {gameName} dans la section [{currentSection}] du fichier roms.ini.", true);
                            LogMessage(logFilePath, $"{gameName}|{rom_demulshooter}|{version_demulshooter}|{target_value}|{delay_seconds}|{arguments_extra}|{joytokey_flag}|{nomousy_flag}", true);

                        // Définir les chemins des fichiers
                        string demulShooterPath = WorkingDirectory; // Remplacez par le chemin approprié si nécessaire
                        string systemName = currentSection; // Le nom du système est la section actuelle dans le fichier roms.ini

                        string scriptFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{gameName}.ahk");
                        string exeFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{gameName}_ahk.exe");
                        string globalScriptFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", "global.ahk");
                        string globalExeFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", "global.exe");
                        string systemScriptFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{systemName}.ahk");
                        string systemExeFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{systemName}_ahk.exe");

                        // Vérifier si un fichier .ahk ou .exe du même nom que la rom/jeu existe et l'exécuter
                        // Exécuter les processus AutoHotkey, JoyToKey et nomousy en cachant la fenêtre
                        if (File.Exists(scriptFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = scriptFile,
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                                LogMessage(logFilePath, $"[AutoHotKey_ahk] Exécution du fichier AHK : {scriptFile}", false);
                        }
                        // ...
                        else if (File.Exists(exeFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = exeFile,
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                                LogMessage(logFilePath, $"[AutoHotKey_exe] Exécution du fichier EXE : {exeFile}", false);
                        }
                        else if (File.Exists(globalScriptFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = globalScriptFile,
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                                LogMessage(logFilePath, $"[AutoHotKey_global_ahk] Exécution du fichier AHK global : {globalScriptFile}", false);
                        }
                        else if (File.Exists(globalExeFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = globalExeFile,
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                                LogMessage(logFilePath, $"[AutoHotKey_global_exe] Exécution du fichier EXE global : {globalExeFile}", false);
                        }
                        else if (File.Exists(systemScriptFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = systemScriptFile,
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                                LogMessage(logFilePath, $"[AutoHotKey_system_ahk] Exécution du fichier AHK spécifique au système : {systemScriptFile}", false);
                        }
                        else if (File.Exists(systemExeFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = systemExeFile,
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                                LogMessage(logFilePath, $"[AutoHotKey_system_exe] Exécution du fichier EXE spécifique au système : {systemExeFile}", false);
                        }

                        // Exécuter JoyToKey si nécessaire
                        if (string.Equals(joytokey_flag, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = $"{WorkingDirectory}\\.es_systems\\.JoyToKey\\JoyToKey.exe",
                            });
                                LogMessage(logFilePath, $"[JoyToKey] Exécution de JoyToKey {joytokey_flag}", false);
                        }

                        // Vérifier et exécuter nomousy si nécessaire
                        string globalNomousyAhkFile = $"{WorkingDirectory}\\.es_systems\\{currentSection}\\ahk\\global_nomousy.ahk";
                        string globalNomousyExeFile = $"{WorkingDirectory}\\.es_systems\\{currentSection}\\ahk\\global_nomousy.exe";
                        string runNomousyExeFile = $"{WorkingDirectory}\\.es_systems\\.nomousy\\run_nomousy.exe";

                        if (string.Equals(nomousy_flag, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            if (File.Exists(globalNomousyAhkFile))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = globalNomousyAhkFile,
                                    UseShellExecute = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true
                                });
                                    LogMessage(logFilePath, $"[Global_nomousy_ahk] Exécution du fichier AHK global nomousy : {globalNomousyAhkFile}", false);
                            }
                            else if (File.Exists(globalNomousyExeFile))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = globalNomousyExeFile,
                                    UseShellExecute = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true
                                });
                                    LogMessage(logFilePath, $"[Global_nomousy_exe] Exécution du fichier EXE global nomousy : {globalNomousyExeFile}", false);
                            }
                            else
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = runNomousyExeFile,
                                    UseShellExecute = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true
                                });
                                    LogMessage(logFilePath, $"[Nomousy_true] Exécution de nomousy {nomousy_flag}", false);
                                }
                        }

                            // Exécuter DemulShooter seulement si target_value n'est pas "false"
                            if (!string.Equals(target_value, "false", StringComparison.OrdinalIgnoreCase))
                            {
                        string command = $"{WorkingDirectory}\\DemulShooter.exe -target={target_value} -rom={rom_demulshooter}";
                        if (!string.Equals(arguments_extra, "false", StringComparison.OrdinalIgnoreCase))
                        {
                            command += $" {arguments_extra}";
                        }

                        // Gérer la version de DemulShooter
                        if (string.Equals(version_demulshooter, "x64", StringComparison.OrdinalIgnoreCase))
                        {
                            command = command.Replace("DemulShooter.exe", "DemulShooterX64.exe");
                        }

                        // Ajouter la commande au fichier de log
                                LogMessage(logFilePath, $"[DemulShooter] Commande : {command}", false);

                                // Gérer le délai
                                if (!string.Equals(delay_seconds, "false", StringComparison.OrdinalIgnoreCase) && 
                                    int.TryParse(delay_seconds, out int delay))
                                {
                                    Thread.Sleep(delay * 1000); // Attendre delay_seconds secondes
                                    LogMessage(logFilePath, $"[DemulShooter] Exécution de DemulShooter avec un délai de {delay} secondes", false);
                                }

                        // Exécuter la commande
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/C {command}",
                            UseShellExecute = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                            }
                            else
                            {
                                LogMessage(logFilePath, "[DemulShooter] Exécution ignorée car target_value est false", false);
                            }

                        return;
                    }
                }
            }

            // Pause pour garder la console ouverte
            // Console.ReadLine();
        }
        catch (Exception ex)
        {
            LogMessage(Path.Combine(WorkingDirectory, $"{ES_SYSTEMS_DIR}\\game-start-ES.log"), 
                $"Erreur lors de l'exécution du programme: {ex.Message}", false);
        }
    }
}
