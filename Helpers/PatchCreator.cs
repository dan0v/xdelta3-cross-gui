/*Copyright 2020-2022 dan0v

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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;

namespace xdelta3_cross_gui
{
    class PatchCreator
    {
        private MainWindow MainParent;
        private double _Progress = 0;
        private bool _ProcFailed = false;

        public PatchCreator(MainWindow MainParent)
        {
            this.MainParent = MainParent;
        }
        public void CreateReadme()
        {
            try
            {
                if (!File.Exists(Path.Combine(this.MainParent.Options.PatchFileDestination, "exec")))
                {
                    Directory.CreateDirectory(Path.Combine(this.MainParent.Options.PatchFileDestination, "exec"));
                }
                if (!File.Exists(Path.Combine(this.MainParent.Options.PatchFileDestination, "original")))
                {
                    Directory.CreateDirectory(Path.Combine(this.MainParent.Options.PatchFileDestination, "original"));
                }
                string readmeString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "doc", "1.Readme.txt"));
                readmeString = String.Format(readmeString, MainWindow.VERSION);
                File.WriteAllText(Path.Combine(MainParent.Options.PatchFileDestination, "1.Readme.txt"), readmeString);
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
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NOTICE.txt"), Path.Combine(MainParent.Options.PatchFileDestination, "NOTICE.txt"), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void CreatePatchingBatchFiles()
        {
            this.MainParent.PatchProgress = 0;
            this._Progress = 0;

            if (!File.Exists(Path.Combine(MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory)) && !this.MainParent.Options.CreateBatchFileOnly)
            {
                Directory.CreateDirectory(Path.Combine(MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory));
            }

            //Batch creation - Windows//
            StreamWriter patchWriterWindows = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "2.Apply Patch-Windows.bat"));
            patchWriterWindows.WriteLine("@echo off");
            patchWriterWindows.WriteLine("mkdir output");
            // Batch creation - Linux //
            StreamWriter patchWriterLinux = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "2.Apply Patch-Linux.sh"));
            patchWriterLinux.NewLine = "\n";
            patchWriterLinux.WriteLine("#!/bin/sh");
            patchWriterLinux.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterLinux.WriteLine("mkdir ./output");
            patchWriterLinux.WriteLine("chmod +x ./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX));
            // Batch creation - Mac //
            StreamWriter patchWriterMac = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "2.Apply Patch-Mac.command"));
            patchWriterMac.NewLine = "\n";
            patchWriterMac.WriteLine("#!/bin/sh");
            patchWriterMac.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterMac.WriteLine("mkdir ./output");
            patchWriterMac.WriteLine("chmod +x ./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS));

            StreamWriter currentPatchScript = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "doNotDelete-In-Progress.bat"));
            if (!this.MainParent.Options.CreateBatchFileOnly)
            {
                currentPatchScript.Close();
                try
                {
                    File.Delete(Path.Combine(this.MainParent.Options.PatchFileDestination, "doNotDelete-In-Progress.bat"));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                currentPatchScript = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat"));
            }
            List<string> oldFileNames = new List<string>();
            List<string> newFileNames = new List<string>();
            this.MainParent.OldFilesList.ForEach(c => oldFileNames.Add(c.ShortName));
            this.MainParent.NewFilesList.ForEach(c => newFileNames.Add(c.ShortName));

            patchWriterWindows.WriteLine("echo Place the files to be patched in the \"original\" directory with the following names:");
            patchWriterLinux.WriteLine("echo Place the files to be patched in the \\\"original\\\" directory with the following names:");
            patchWriterMac.WriteLine("echo Place the files to be patched in the \\\"original\\\" directory with the following names:");
            patchWriterWindows.WriteLine("echo --------------------");
            patchWriterLinux.WriteLine("echo --------------------");
            patchWriterMac.WriteLine("echo --------------------");

            for (int i = 0; i < this.MainParent.OldFilesList.Count; i++)
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

            for (int i = 0; i < this.MainParent.OldFilesList.Count; i++)
            {
                // Batch creation - Windows
                patchWriterWindows.WriteLine("exec\\" + Path.GetFileName(MainWindow.XDELTA3_BINARY_WINDOWS) + " -v -d -s \".\\original\\{0}\" " + "\".\\" + this.MainParent.Options.PatchSubdirectory + "\\" + "{0}." + this.MainParent.Options.PatchExtention + "\" \".\\output\\{2}\"", oldFileNames[i], this.MainParent.Options.PatchSubdirectory + "\\" + (i + 1).ToString(), newFileNames[i]);
                // Batch creation - Linux //
                patchWriterLinux.WriteLine("./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX) + " -v -d -s \"./original/{0}\" " + '"' + this.MainParent.Options.PatchSubdirectory + '/' + "{0}." + this.MainParent.Options.PatchExtention + "\" \"./output/{2}\"", oldFileNames[i], this.MainParent.Options.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);
                // Batch creation - Mac //
                patchWriterMac.WriteLine("./exec/" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS) + " -v -d -s \"./original/{0}\" " + '"' + this.MainParent.Options.PatchSubdirectory + '/' + "{0}." + this.MainParent.Options.PatchExtention + "\" \"./output/{2}\"", oldFileNames[i], this.MainParent.Options.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);

                // Script for patch creation
                if (!this.MainParent.Options.CreateBatchFileOnly)
                {
                    currentPatchScript.WriteLine("\"" + MainWindow.XDELTA3_PATH + "\"" + " " + this.MainParent.Options.XDeltaArguments + " " + "\"" + this.MainParent.OldFilesList[i].FullPath + "\" \"" + this.MainParent.NewFilesList[i].FullPath + "\" \"" + Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, oldFileNames[i]) + "." + this.MainParent.Options.PatchExtention + "\"");
                }

            }
            patchWriterWindows.WriteLine("echo Completed!");
            patchWriterWindows.WriteLine("@pause");
            patchWriterWindows.Close();
            patchWriterLinux.Close();
            patchWriterMac.Close();

            currentPatchScript.Close();

            if (this.MainParent.Options.CreateBatchFileOnly)
            {
                try
                {
                    File.Delete(Path.Combine(MainParent.Options.PatchFileDestination, "doNotDelete-In-Progress.bat"));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                this.MainParent.PatchProgress = 0;
                if (this.MainParent.Options.ZipFilesWhenDone)
                {
                    this.ZipFiles();
                }
                this.MainParent.AlreadyBusy = false;
                Dispatcher.UIThread.InvokeAsync(new Action(() =>
                {
                    SuccessDialog dialog = new SuccessDialog(this.MainParent);
                    dialog.Show();
                    dialog.Topmost = true;
                    dialog.Topmost = false;
                }));
            }
            else
            {
                CreateNewXDeltaThread().Start();
            }
        }

        private Thread CreateNewXDeltaThread()
        {
            return new Thread(() =>
            {
                _ProcFailed = false;
                using (Process activeCMD = new Process())
                {
                    activeCMD.OutputDataReceived += HandleCMDOutput;
                    activeCMD.ErrorDataReceived += HandleCMDError;

                    ProcessStartInfo info = new ProcessStartInfo();

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        info.FileName = Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        string args = Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat");
                        string escapedArgs = "/bin/bash " + args.Replace("\"", "\\\"").Replace(" ", "\\ ").Replace("(", "\\(").Replace(")", "\\)");
                        info.FileName = "/bin/bash";
                        info.Arguments = $"-c \"{escapedArgs}\"";
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        string args = Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat");
                        string escapedArgs = "/bin/bash " + args.Replace("\"", "\\\"").Replace(" ", "\\ ").Replace("(", "\\(").Replace(")", "\\)");
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

                    activeCMD.Start();
                    activeCMD.BeginOutputReadLine();
                    activeCMD.BeginErrorReadLine();
                    activeCMD.WaitForExit();

                    if (_ProcFailed)
                    {
                        Dispatcher.UIThread.InvokeAsync(new Action(() =>
                        {
                            this.MainParent.AlreadyBusy = false;
                            this.MainParent.PatchProgress = 0;
                            MainParent.ShowTerminal = true;
                            ErrorDialog dialog = new ErrorDialog(Localization.Localizer.Instance["xDeltaProcessError"]);
                            dialog.Show();
                            dialog.Topmost = true;
                            dialog.Topmost = false;
                        }));
                        return;
                    }

                    try
                    {
                        File.Delete(Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat"));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                    if (this.MainParent.Options.ZipFilesWhenDone)
                    {
                        this.ZipFiles();
                    }
                    this.MainParent.AlreadyBusy = false;
                    this.MainParent.PatchProgress = 0;
                    Dispatcher.UIThread.InvokeAsync(new Action(() =>
                    {
                        SuccessDialog dialog = new SuccessDialog(this.MainParent);
                        dialog.Show();
                        dialog.Topmost = true;
                        dialog.Topmost = false;
                    }));
                }
            })
            { IsBackground = true };
        }

        private void ZipFiles()
        {
            new Thread(() =>
            {
                this.MainParent.PatchProgressIsIndeterminate = true;
                if (File.Exists(Path.Combine(this.MainParent.Options.PatchFileDestination, "..", this.MainParent.Options.ZipName + ".zip")))
                {
                    File.Delete(Path.Combine(this.MainParent.Options.PatchFileDestination, "..", this.MainParent.Options.ZipName + ".zip"));
                }
                ZipFile.CreateFromDirectory(this.MainParent.Options.PatchFileDestination, Path.Combine(this.MainParent.Options.PatchFileDestination, "..", this.MainParent.Options.ZipName + ".zip"));
                this.MainParent.PatchProgressIsIndeterminate = false;
            })
            { IsBackground = true }.Start();
        }

        private void HandleCMDOutput(object sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);
                if (e.Data.ToLower().Contains("error") || e.Data.ToLower().Contains("fail"))
                {
                    _ProcFailed = true;
                }
                this._Progress++;

                double prog = (this._Progress / this.MainParent.OldFilesList.Count) * 100;
                this.MainParent.PatchProgress = prog > 100 ? 100 : prog;

                this.MainParent.Console.AddLine(e.Data);
            }
        }

        private void HandleCMDError(object sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);
                if (e.Data.ToLower().Contains("error") || e.Data.ToLower().Contains("fail"))
                {
                    _ProcFailed = true;
                }

                this.MainParent.Console.AddLine(e.Data);
            }
        }

        public void CopyExecutables()
        {
            if (!File.Exists(Path.Combine(this.MainParent.Options.PatchFileDestination, "exec")))
            {
                Directory.CreateDirectory(Path.Combine(this.MainParent.Options.PatchFileDestination, "exec"));
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_WINDOWS), Path.Combine(this.MainParent.Options.PatchFileDestination, "exec", MainWindow.XDELTA3_BINARY_WINDOWS), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_LINUX), Path.Combine(this.MainParent.Options.PatchFileDestination, "exec", MainWindow.XDELTA3_BINARY_LINUX), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_MACOS), Path.Combine(this.MainParent.Options.PatchFileDestination, "exec", MainWindow.XDELTA3_BINARY_MACOS), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
