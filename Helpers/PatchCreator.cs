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

using Avalonia.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using xdelta3_cross_gui.Models;

namespace xdelta3_cross_gui
{
    class PatchCreator()
    {
        private static PatchCreator? _instance;
        public static PatchCreator Instance => _instance ??= new PatchCreator();

        private readonly MainWindow MainParent = App.MainWindow ?? new();
        private ConcurrentDictionary<string, Process?> _activeProcesses = [];
        private volatile bool _patchingDone = false;
        private volatile object _patchingDoneLock = new();
        private volatile int _remainingTasks = 0;

        private static object _lock_progress = new();
        private double _progress = 0;
        private static object _lock_procFailed = new();
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
            _procFailed = false;
            _patchingDone = false;

            if (!File.Exists(Path.Combine(MainParent.Config.PatchFileDestination, MainParent.Config.PatchSubdirectory)))
            {
                Directory.CreateDirectory(Path.Combine(MainParent.Config.PatchFileDestination, MainParent.Config.PatchSubdirectory));
            }

            StreamWriter patchWriterWindows = new(Path.Combine(MainParent.Config.PatchFileDestination, "2.Apply Patch-Windows.bat"));
            StreamWriter patchWriterLinux = new(Path.Combine(MainParent.Config.PatchFileDestination, "2.Apply Patch-Linux.sh"))
            {
                NewLine = "\n"
            };
            StreamWriter patchWriterMac = new(Path.Combine(MainParent.Config.PatchFileDestination, "2.Apply Patch-Mac.command"))
            {
                NewLine = "\n"
            };

            string[] oldFileNames = MainParent.OldFilesList.Select(c => c.ShortName).ToArray();
            string[] newFileNames = MainParent.NewFilesList.Select(c => c.ShortName).ToArray();

            patchWriterWindows.WriteLine("@echo off");
            patchWriterWindows.WriteLine("CHCP 65001");
            patchWriterWindows.WriteLine("mkdir output");
            patchWriterWindows.WriteLine("echo Place the files to be patched in the \"original\" directory with the following names:");
            patchWriterWindows.WriteLine("echo --------------------");

            patchWriterLinux.WriteLine("#!/bin/sh");
            patchWriterLinux.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterLinux.WriteLine("mkdir ./output");
            patchWriterLinux.WriteLine("chmod +x ./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX));
            patchWriterLinux.WriteLine("echo Place the files to be patched in the \\\"original\\\" directory with the following names:");
            patchWriterLinux.WriteLine("echo --------------------");

            patchWriterMac.WriteLine("#!/bin/sh");
            patchWriterMac.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterMac.WriteLine("mkdir ./output");
            patchWriterMac.WriteLine("chmod +x ./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS));
            patchWriterMac.WriteLine("echo Place the files to be patched in the \\\"original\\\" directory with the following names:");
            patchWriterMac.WriteLine("echo --------------------");

            for (int i = 0; i < MainParent.OldFilesList.Count; i++)
            {
                patchWriterWindows.WriteLine("echo " + oldFileNames[i]);
                patchWriterLinux.WriteLine("echo \"" + oldFileNames[i] + "\"");
                patchWriterMac.WriteLine("echo \"" + oldFileNames[i] + "\"");
            }

            patchWriterWindows.WriteLine("echo --------------------");
            patchWriterWindows.WriteLine("echo Patched files will be in the \"output\" directory");
            patchWriterWindows.WriteLine("pause");

            patchWriterLinux.WriteLine("echo --------------------");
            patchWriterLinux.WriteLine("echo Patched files will be in the \\\"output\\\" directory");
            patchWriterLinux.WriteLine("read -p \"Press enter to continue...\" inp");

            patchWriterMac.WriteLine("echo --------------------");
            patchWriterMac.WriteLine("echo Patched files will be in the \\\"output\\\" directory");
            patchWriterMac.WriteLine("read -p \"Press enter to continue...\" inp");

            ConcurrentBag<PatchCreationJob> patchJobs = [];
            for (int i = 0; i < MainParent.OldFilesList.Count; i++)
            {
                patchWriterWindows.WriteLine("exec\\" + Path.GetFileName(MainWindow.XDELTA3_BINARY_WINDOWS) + " -v -d -s \".\\original\\{0}\" " + "\".\\" + MainParent.Config.PatchSubdirectory + "\\" + "{0}." + MainParent.Config.PatchExtention + "\" \".\\output\\{2}\"", oldFileNames[i], MainParent.Config.PatchSubdirectory + "\\" + (i + 1).ToString(), newFileNames[i]);
                patchWriterLinux.WriteLine("./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX) + " -v -d -s \"./original/{0}\" " + '"' + MainParent.Config.PatchSubdirectory + '/' + "{0}." + MainParent.Config.PatchExtention + "\" \"./output/{2}\"", oldFileNames[i], MainParent.Config.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);
                patchWriterMac.WriteLine("./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS) + " -v -d -s \"./original/{0}\" " + '"' + MainParent.Config.PatchSubdirectory + '/' + "{0}." + MainParent.Config.PatchExtention + "\" \"./output/{2}\"", oldFileNames[i], MainParent.Config.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);

                // Actual script to generate patch files
                patchJobs.Add(new(Guid.NewGuid().ToString(), MainParent.Config.XDeltaArguments, MainParent.OldFilesList[i].FullPath, MainParent.NewFilesList[i].FullPath, Path.Combine(MainParent.Config.PatchFileDestination, MainParent.Config.PatchSubdirectory, oldFileNames[i]) + "." + MainParent.Config.PatchExtention));
            }

            patchWriterWindows.WriteLine("echo Completed!");
            patchWriterWindows.WriteLine("@pause");
            patchWriterWindows.Close();
            patchWriterLinux.Close();
            patchWriterMac.Close();

            List<Task> tasks = [];

            _remainingTasks = patchJobs.Count;

            for (int i = 0; i < (patchJobs.Count < MainParent.Config.MaximumThreads ? patchJobs.Count : MainParent.Config.MaximumThreads); i++)
            {
                new Thread(() =>
                {
                    while (patchJobs.TryTake(out var nextJob))
                    {
                        CreateNewXDeltaTask(i, nextJob);
                    }

                    lock (_patchingDoneLock)
                    {
                        if (!_patchingDone && _remainingTasks == 0)
                        {
                            _patchingDone = true;
                            Debug.WriteLine("Patching done, wrapping up...");
                            FinishPatchCreationJob();
                        }
                    }  
                })
                {
                    IsBackground = true
                }.Start();
            }
        }

        private void CreateNewXDeltaTask(int threadNumber, PatchCreationJob patchCreationJob)
        {
            Debug.WriteLine($"Running job on thread number: {threadNumber}");

            if (_procFailed)
            {
                return;
            }

            using Process activeCMD = new();
            activeCMD.OutputDataReceived += HandleCMDOutput;
            activeCMD.ErrorDataReceived += HandleCMDError;

            ProcessStartInfo info = new()
            {
                FileName = MainWindow.XDELTA3_PATH,
                Arguments = $"""{patchCreationJob.Options} "{patchCreationJob.Source}" "{patchCreationJob.Goal}" "{patchCreationJob.PatchDestination}" """,

                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            activeCMD.StartInfo = info;
            activeCMD.EnableRaisingEvents = true;

            _activeProcesses.TryAdd(patchCreationJob.id, activeCMD);
            activeCMD.Start();
            activeCMD.BeginOutputReadLine();
            activeCMD.BeginErrorReadLine();
            activeCMD.WaitForExit();
            activeCMD.OutputDataReceived -= HandleCMDOutput;
            activeCMD.ErrorDataReceived -= HandleCMDError;
            _activeProcesses.TryRemove(patchCreationJob.id, out var id);
            lock(_patchingDoneLock)
            {
                _remainingTasks--;
            }
        }


        private void FinishPatchCreationJob()
        {
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

            if (MainParent.Config.ZipFilesWhenDone)
            {
                ZipFiles();
            }
            else
            {
                DisplaySuccess();
            }
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
                DisplaySuccess();
            })
            { IsBackground = true }.Start();
        }

        private void DisplaySuccess()
        {
            Dispatcher.UIThread.InvokeAsync(new Action(() =>
            {
                MainParent.AlreadyBusy = false;
                MainParent.PatchProgress = 0;
                SuccessDialog dialog = new(MainParent);
                dialog.Show();
                dialog.Topmost = true;
                dialog.Topmost = false;
            }));
        }

        private void HandleCMDOutput(object? sender, DataReceivedEventArgs e)
        {
            if (sender is Process proc)
            {
                if (proc.HasExited && proc.ExitCode != 0)
                {
                    Debug.WriteLine("Process exited with code: " + proc.ExitCode);
                    lock (_lock_procFailed)
                    {
                        _procFailed = true;
                    }
                }
                else if (proc.HasExited && proc.ExitCode == 0)
                {
                    lock (_lock_progress)
                    {
                        _progress++;
                        double prog = (_progress / MainParent.OldFilesList.Count) * 100;
                        MainParent.PatchProgress = prog > 100 ? 100 : prog;
                    }
                }
            }
        }

        private void HandleCMDError(object? sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);
                if (e.Data.ToLower().Contains("error") || e.Data.ToLower().Contains("fail"))
                {
                    lock (_lock_procFailed)
                    {
                        _procFailed = true;
                    }
                }

                MainParent.Console.AddLine(e.Data);
            }
        }

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
            _patchingDone = true;
            _procFailed = true;
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
