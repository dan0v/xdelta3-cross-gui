/*Copyright 2020-2024 dan0v

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Environment;

namespace xdelta3_cross_gui
{
    class PatchCreator()
    {
        private static PatchCreator? _instance;
        public static PatchCreator Instance => _instance ??= new PatchCreator();

        private readonly MainWindow MainParent = App.MainWindow ?? new();
        private ConcurrentDictionary<string, Process?> _activeProcesses = [];

        private double _progress = 0;
        private bool _procFailed = false;

        public void CreateReadme()
        {
            try
            {
                if (!File.Exists(Path.Combine(MainParent.Config.PatchFileDestination, "exec")))
                {
                    Directory.CreateDirectory(Path.Combine(MainParent.Config.PatchFileDestination, "exec"));
                }
                if (!File.Exists(Path.Combine(MainParent.Config.PatchFileDestination, "original")))
                {
                    Directory.CreateDirectory(Path.Combine(MainParent.Config.PatchFileDestination, "original"));
                }
                string readmeString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "doc", "1.Readme.txt"));
                readmeString = string.Format(readmeString, MainWindow.VERSION);
                File.WriteAllText(Path.Combine(MainParent.Config.PatchFileDestination, "1.Readme.txt"), readmeString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
		}

        public void CopyNotice()
        {
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NOTICE.txt"), Path.Combine(MainParent.Config.PatchFileDestination, "NOTICE.txt"), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void CreatePatchingBatchFiles()
        {
            MainParent.PatchProgress = 0;
            _progress = 0;

            if (!File.Exists(Path.Combine(MainParent.Config.PatchFileDestination, MainParent.Config.PatchSubdirectory)))
            {
                Directory.CreateDirectory(Path.Combine(MainParent.Config.PatchFileDestination, MainParent.Config.PatchSubdirectory));
            }

            //Batch creation - Windows//
            StreamWriter patchWriterWindows = new(Path.Combine(MainParent.Config.PatchFileDestination, "2.Apply Patch-Windows.bat"));
            patchWriterWindows.WriteLine("@echo off");
            patchWriterWindows.WriteLine("CHCP 65001");
            patchWriterWindows.WriteLine("mkdir output");
            // Batch creation - Linux //
            StreamWriter patchWriterLinux = new(Path.Combine(MainParent.Config.PatchFileDestination, "2.Apply Patch-Linux.sh"))
            {
                NewLine = "\n"
            };
            patchWriterLinux.WriteLine("#!/bin/sh");
            patchWriterLinux.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterLinux.WriteLine("mkdir ./output");
            patchWriterLinux.WriteLine("chmod +x ./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX));
            // Batch creation - Mac //
            StreamWriter patchWriterMac = new(Path.Combine(MainParent.Config.PatchFileDestination, "2.Apply Patch-Mac.command"))
            {
                NewLine = "\n"
            };
            patchWriterMac.WriteLine("#!/bin/sh");
            patchWriterMac.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterMac.WriteLine("mkdir ./output");
            patchWriterMac.WriteLine("chmod +x ./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS));

            string currentPatchScriptPath = GetTempBatPath();

            StreamWriter currentPatchScript = new(currentPatchScriptPath);

            // Enable UTF in windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                currentPatchScript.WriteLine("CHCP 65001\n");
            }

            List<string> oldFileNames = [];
            List<string> newFileNames = [];
            MainParent.OldFilesList.ForEach(c => oldFileNames.Add(c.ShortName));
            MainParent.NewFilesList.ForEach(c => newFileNames.Add(c.ShortName));

            patchWriterWindows.WriteLine("echo Place the files to be patched in the \"original\" directory with the following names:");
            patchWriterLinux.WriteLine("echo Place the files to be patched in the \\\"original\\\" directory with the following names:");
            patchWriterMac.WriteLine("echo Place the files to be patched in the \\\"original\\\" directory with the following names:");
            patchWriterWindows.WriteLine("echo --------------------");
            patchWriterLinux.WriteLine("echo --------------------");
            patchWriterMac.WriteLine("echo --------------------");

            for (int i = 0; i < MainParent.OldFilesList.Count; i++)
            {
                patchWriterWindows.WriteLine("echo " + oldFileNames[i]);
                patchWriterLinux.WriteLine("echo \"" + oldFileNames[i] + "\"");
                patchWriterMac.WriteLine("echo \"" + oldFileNames[i] + "\"");
            }
            patchWriterWindows.WriteLine("echo --------------------");
            patchWriterLinux.WriteLine("echo --------------------");
            patchWriterMac.WriteLine("echo --------------------");

            patchWriterWindows.WriteLine("echo Patched files will be in the \"output\" directory");
            patchWriterLinux.WriteLine("echo Patched files will be in the \\\"output\\\" directory");
            patchWriterMac.WriteLine("echo Patched files will be in the \\\"output\\\" directory");

            patchWriterWindows.WriteLine("pause");
            patchWriterLinux.WriteLine("read -p \"Press enter to continue...\" inp");
            patchWriterMac.WriteLine("read -p \"Press enter to continue...\" inp");


            for (int i = 0; i < MainParent.OldFilesList.Count; i++)
            {
                // Batch creation - Windows
                patchWriterWindows.WriteLine("exec\\" + Path.GetFileName(MainWindow.XDELTA3_BINARY_WINDOWS) + " -v -d -s \".\\original\\{0}\" " + "\".\\" + MainParent.Config.PatchSubdirectory + "\\" + "{0}." + MainParent.Config.PatchExtention + "\" \".\\output\\{2}\"", oldFileNames[i], MainParent.Config.PatchSubdirectory + "\\" + (i + 1).ToString(), newFileNames[i]);
                // Batch creation - Linux //
                patchWriterLinux.WriteLine("./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX) + " -v -d -s \"./original/{0}\" " + '"' + MainParent.Config.PatchSubdirectory + '/' + "{0}." + MainParent.Config.PatchExtention + "\" \"./output/{2}\"", oldFileNames[i], MainParent.Config.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);
                // Batch creation - Mac //
                patchWriterMac.WriteLine("./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS) + " -v -d -s \"./original/{0}\" " + '"' + MainParent.Config.PatchSubdirectory + '/' + "{0}." + MainParent.Config.PatchExtention + "\" \"./output/{2}\"", oldFileNames[i], MainParent.Config.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);

                // Actual script to generate patch files
                currentPatchScript.WriteLine("\"" + MainWindow.XDELTA3_PATH + "\"" + " " + MainParent.Config.XDeltaArguments + " " + "\"" + MainParent.OldFilesList[i].FullPath + "\" \"" + MainParent.NewFilesList[i].FullPath + "\" \"" + Path.Combine(MainParent.Config.PatchFileDestination, MainParent.Config.PatchSubdirectory, oldFileNames[i]) + "." + MainParent.Config.PatchExtention + "\"");
            }
            patchWriterWindows.WriteLine("echo Completed!");
            patchWriterWindows.WriteLine("@pause");
            patchWriterWindows.Close();
            patchWriterLinux.Close();
            patchWriterMac.Close();
            currentPatchScript.Close();

            var threadId = Guid.NewGuid();
            var thread = CreateNewXDeltaThread(threadId.ToString(), currentPatchScriptPath);
            thread.Start();
        }

        private Thread CreateNewXDeltaThread(string threadId, string tempFilePath)
        {
            return new Thread(() =>
            {
                _procFailed = false;
                using Process activeCMD = new();
                activeCMD.OutputDataReceived += HandleCMDOutput;
                activeCMD.ErrorDataReceived += HandleCMDError;

                ProcessStartInfo info = new();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    info.FileName = tempFilePath;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string escapedArgs = "/bin/bash " + tempFilePath.Replace("\"", "\\\"").Replace(" ", "\\ ").Replace("(", "\\(").Replace(")", "\\)");
                    info.FileName = "/bin/bash";
                    info.Arguments = $"-c \"{escapedArgs}\"";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    string escapedArgs = "/bin/bash " + tempFilePath.Replace("\"", "\\\"").Replace(" ", "\\ ").Replace("(", "\\(").Replace(")", "\\)");
                    info.FileName = "/bin/bash";
                    info.Arguments = $"-c \"{escapedArgs}\"";
                }

                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;

                activeCMD.StartInfo = info;
                activeCMD.EnableRaisingEvents = true;

                _activeProcesses.TryAdd(threadId, activeCMD);
                activeCMD.Start();
                activeCMD.BeginOutputReadLine();
                activeCMD.BeginErrorReadLine();
                activeCMD.WaitForExit();
                _activeProcesses.TryRemove(threadId, out var id);

                if (_procFailed)
                {
                    Dispatcher.UIThread.InvokeAsync(new Action(() =>
                    {
                        MainParent.AlreadyBusy = false;
                        MainParent.PatchProgress = 0;
                        MainParent.ShowTerminal = true;
                        ErrorDialog dialog = new(Localization.Localizer.Instance["xDeltaProcessError"]);
                        dialog.Show();
                        dialog.Topmost = true;
                        dialog.Topmost = false;
                    }));
                    return;
                }

                try
                {
                    File.Delete(tempFilePath);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                if (MainParent.Config.ZipFilesWhenDone)
                {
                    ZipFiles();
                }
                Dispatcher.UIThread.InvokeAsync(new Action(() =>
                {
                    MainParent.AlreadyBusy = false;
                    MainParent.PatchProgress = 0;
                    SuccessDialog dialog = new(MainParent);
                    dialog.Show();
                    dialog.Topmost = true;
                    dialog.Topmost = false;
                }));
            })
            { IsBackground = true };
        }

        private void ZipFiles()
        {
            new Thread(() =>
            {
                MainParent.PatchProgressIsIndeterminate = true;
                if (File.Exists(Path.Combine(MainParent.Config.PatchFileDestination, "..", MainParent.Config.ZipName + ".zip")))
                {
                    File.Delete(Path.Combine(MainParent.Config.PatchFileDestination, "..", MainParent.Config.ZipName + ".zip"));
                }
                ZipFile.CreateFromDirectory(MainParent.Config.PatchFileDestination, Path.Combine(MainParent.Config.PatchFileDestination, "..", MainParent.Config.ZipName + ".zip"));
                MainParent.PatchProgressIsIndeterminate = false;
            })
            { IsBackground = true }.Start();
        }

        private void HandleCMDOutput(object? sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);
                if (e.Data.ToLower().Contains("error") || e.Data.ToLower().Contains("fail"))
                {
                    _procFailed = true;
                }
                _progress++;

                double prog = (_progress / MainParent.OldFilesList.Count) * 100;
                MainParent.PatchProgress = prog > 100 ? 100 : prog;

                MainParent.Console.AddLine(e.Data);
            }
        }

        private void HandleCMDError(object? sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);
                if (e.Data.ToLower().Contains("error") || e.Data.ToLower().Contains("fail"))
                {
                    _procFailed = true;
                }

                MainParent.Console.AddLine(e.Data);
            }
        }

        private string GetTempBatPath() =>  Path.Join(MainWindow.TEMPORARY_FILE_STORAGE, $"xdelta3-cross-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.bat");

        public void CopyExecutables()
        {
            if (!File.Exists(Path.Combine(MainParent.Config.PatchFileDestination, "exec")))
            {
                Directory.CreateDirectory(Path.Combine(MainParent.Config.PatchFileDestination, "exec"));
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_WINDOWS), Path.Combine(MainParent.Config.PatchFileDestination, "exec", MainWindow.XDELTA3_BINARY_WINDOWS), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_LINUX), Path.Combine(MainParent.Config.PatchFileDestination, "exec", MainWindow.XDELTA3_BINARY_LINUX), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_MACOS), Path.Combine(MainParent.Config.PatchFileDestination, "exec", MainWindow.XDELTA3_BINARY_MACOS), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void OnClosing()
        {
            var threads = _activeProcesses;
            foreach (var item in threads)
            {
                try
                {
                    item.Value?.Kill();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }
    }
}
