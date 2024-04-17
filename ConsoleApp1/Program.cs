using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Définir les processus à rechercher et terminer
        string[] processesToKill = { "DemulShooter.exe", "DemulShooterX64.exe", "JoyToKey.exe", "AutoHotkey.exe", "AutoHotkeyU64.exe", "AutoHotkeyUX.exe", "kill.exe" };

        foreach (string process in processesToKill)
        {
            var runningProcess = Process.GetProcessesByName(process.Replace(".exe", ""));
            if (runningProcess.Length > 0)
            {
                foreach (var proc in runningProcess)
                {
                    proc.Kill();
                    Console.WriteLine($"Processus {process} terminé.");
                }
            }
            else
            {
                Console.WriteLine($"Le processus {process} n'est pas en cours d'exécution.");
            }
        }

        // 1 - Définir le dossier de travail
        string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\.DemulShooter"));

        // Lire le fichier .config
        string configFilePathpush = Path.Combine(workingDirectory, ".es_systems\\.config");
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
                FileName = $"{workingDirectory}\\HIDgetpush.exe",
                WorkingDirectory = workingDirectory,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            Thread.Sleep(1000); // Attendre 1 secondes

            // Exécuter getput.exe
            Process.Start(new ProcessStartInfo
            {
                FileName = $"{workingDirectory}\\getput.exe",
                WorkingDirectory = workingDirectory,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            Thread.Sleep(2000); // Attendre 2 secondes
        }

        // Vérifier si le fichier .locked est présent
        string lockedFilePath = Path.Combine(workingDirectory, ".locked");
        if (!File.Exists(lockedFilePath))
        {
            // Si le fichier .locked n'est pas présent, exécuter GUIDemulshooter pour le paramétrage et créer le fichier .locked
            Process.Start(Path.Combine(workingDirectory, "DemulShooter_GUI.exe"));
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
            Arguments = $"/C start \"Check\" /B \"{workingDirectory}\\.es_systems\\Check.bat\"",
            UseShellExecute = false, // Modifier à false
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true // Ajouter cette ligne
        });

        // Définir le chemin du fichier de log
        string logFilePath = Path.Combine(workingDirectory, ".es_systems\\game-start-ES.log");

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
        if (fileName.Contains("."))
        {
            // Si oui, prendre la première partie comme nom de fichier
            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
        }

        // Filtrer pour retirer toute extension restante
        int extensionIndex = fileName.LastIndexOf(".");
        if (extensionIndex != -1)
        {
            fileName = fileName.Substring(0, extensionIndex);
        }



        // 3 - Établir une liste de prérequis de dossiers systems
        string configFilePath = Path.Combine(workingDirectory, ".es_systems\\.config");
        if (!File.Exists(configFilePath))
        {
            return;
        }

        var lines = File.ReadAllLines(configFilePath);
        bool debugMode = false;
        List<string> requiredDirectories = new List<string>();
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
            if (!Directory.Exists(Path.Combine(workingDirectory, ".es_systems", dir)))
            {
                return;
            }
        }


        // Afficher le nom du jeu exécuté si le mode de débogage est activé
        if (debugMode)
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: Nom de jeu exécuté : {fileName}\n");
        }

        // 4 - Lire le fichier roms.ini
        string romsFilePath = Path.Combine(workingDirectory, ".es_systems\\roms.ini");
        if (!File.Exists(romsFilePath))
        {
            return;
        }

        var romsLines = File.ReadAllLines(romsFilePath);

        // Variables pour gérer les sections et le nom du jeu
        string currentSection = "";
        string gameName = "";
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
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: Nom de jeu extrait : {gameName}\n");
                }

                // Vérifiez si les noms de jeu correspondent
                if (string.Equals(gameName.Trim(), fileName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    // Obtenir l'horodatage actuel en France
                    TimeZoneInfo franceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                    DateTime franceTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, franceTimeZone);

                    // Afficher les informations demandées avec l'horodatage
                    File.AppendAllText(logFilePath, $"{franceTime}: Le jeu {fileName} correspond à {gameName} dans la section [{currentSection}] du fichier roms.ini.\n");
                    File.AppendAllText(logFilePath, $"{franceTime}: {gameName}|{rom_demulshooter}|{version_demulshooter}|{target_value}|{delay_seconds}|{arguments_extra}|{joytokey_flag}|{nomousy_flag}\n");

                    // Définir les chemins des fichiers
                    string demulShooterPath = workingDirectory; // Remplacez par le chemin approprié si nécessaire
                    string systemName = currentSection; // Le nom du système est la section actuelle dans le fichier roms.ini

                    string scriptFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{gameName}.ahk");
                    string exeFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{gameName}.exe");
                    string globalScriptFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", "global.ahk");
                    string globalExeFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", "global.exe");
                    string systemScriptFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{systemName}.ahk");
                    string systemExeFile = Path.Combine(demulShooterPath, ".es_systems", systemName, "ahk", $"{systemName}.exe");

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
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [AutoHotKey_ahk] Exécution du fichier AHK : {scriptFile}\n");
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
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [AutoHotKey_exe] Exécution du fichier EXE : {exeFile}\n");
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
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [AutoHotKey_global_ahk] Exécution du fichier AHK global : {globalScriptFile}\n");
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
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [AutoHotKey_global_exe] Exécution du fichier EXE global : {globalExeFile}\n");
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
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [AutoHotKey_system_ahk] Exécution du fichier AHK spécifique au système : {systemScriptFile}\n");
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
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [AutoHotKey_system_exe] Exécution du fichier EXE spécifique au système : {systemExeFile}\n");
                    }

                    // Exécuter JoyToKey si nécessaire
                    if (string.Equals(joytokey_flag, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = $"{workingDirectory}\\.es_systems\\.JoyToKey\\JoyToKey.exe",
                        });
                        File.AppendAllText(logFilePath, $"{DateTime.Now}: [JoyToKey] Exécution de JoyToKey {joytokey_flag}\n");
                    }

                    // Vérifier et exécuter nomousy si nécessaire
                    string globalNomousyAhkFile = $"{workingDirectory}\\.es_systems\\{currentSection}\\ahk\\global_nomousy.ahk";
                    string globalNomousyExeFile = $"{workingDirectory}\\.es_systems\\{currentSection}\\ahk\\global_nomousy.exe";
                    string runNomousyExeFile = $"{workingDirectory}\\.es_systems\\.nomousy\\run_nomousy.exe";

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
                            File.AppendAllText(logFilePath, $"{DateTime.Now}: [Global_nomousy_ahk] Exécution du fichier AHK global nomousy : {globalNomousyAhkFile}\n");
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
                            File.AppendAllText(logFilePath, $"{DateTime.Now}: [Global_nomousy_exe] Exécution du fichier EXE global nomousy : {globalNomousyExeFile}\n");
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
                            File.AppendAllText(logFilePath, $"{DateTime.Now}: [Nomousy_true] Exécution de nomousy {nomousy_flag}\n");
                        }
                    }

                    // Exécuter DemulShooter
                    string command = $"{workingDirectory}\\DemulShooter.exe -target={target_value} -rom={rom_demulshooter}";
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
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: [DemulShooter] Commande : {command}\n");

                    Thread.Sleep(int.Parse(delay_seconds) * 1000); // Attendre delay_seconds secondes
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: [DemulShooter] Exécution de DemulShooter avec un délai de {delay_seconds} secondes\n");

                    // Exécuter la commande
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });

                    return;
                }
            }
        }

        // Pause pour garder la console ouverte
        // Console.ReadLine();
    }
}
