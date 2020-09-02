using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace xdelta3_cross_gui
{
    class PatchCreator
    {
        private MainWindow MainParent;
        private double _Progress = 0;
        public PatchCreator(MainWindow MainParent)
        {
            this.MainParent = MainParent;
        }
        public void CreateReadme()
        {
            if (!File.Exists(MainParent.Options.PatchFileDestination))
            {
                Directory.CreateDirectory(MainParent.Options.PatchFileDestination);
            }
            StreamWriter readmeWriter = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "1.Readme.txt"));
            readmeWriter.WriteLine("Created using xDelta3 Cross GUI " + MainWindow.VERSION + " by dan0v, https://github.com/dan0v/xdelta3-cross-gui");
            readmeWriter.WriteLine("");
            readmeWriter.WriteLine("Windows:");
            readmeWriter.WriteLine("1. Copy your original files into this folder with their original file names");
            readmeWriter.WriteLine("2. Double click the 2.Apply Patch-Windows.bat file and patching will begin");
            readmeWriter.WriteLine("3. Once patching is complete you will find your newly patched files in the main folder and the originals in a folder called 'old'");
            readmeWriter.WriteLine("4. Enjoy");
            readmeWriter.WriteLine("");
            readmeWriter.WriteLine("Linux:");
            readmeWriter.WriteLine("1. Copy your original files into this folder with their original file names");
            readmeWriter.WriteLine("2. In terminal, type: sh " + '"' + "2.Apply Patch-Linux.sh" + '"' + ". Patching should start automatically");
            readmeWriter.WriteLine("2. Alternatively, if you're using a GUI, double click 2.Apply Patch-Linux.sh and patching should start automatically");
            readmeWriter.WriteLine("3. Once patching is complete you will find your newly patched files in the main folder and the originals in a folder called 'old'");
            readmeWriter.WriteLine("4. Enjoy");
            readmeWriter.WriteLine("");
            readmeWriter.WriteLine("MacOS:");
            readmeWriter.WriteLine("1. Copy your original files into this folder with their original file names");
            readmeWriter.WriteLine("2. Double click 2.Apply Patch-Mac.command and a terminal window should appear");
            readmeWriter.WriteLine("3. Once patching is complete you will find your newly patched files in the main folder and the originals in a folder called 'old'");
            readmeWriter.WriteLine("4. Enjoy");
            readmeWriter.Close();
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
            patchWriterWindows.WriteLine("mkdir old");
            // Batch creation - Linux //
            StreamWriter patchWriterLinux = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "2.Apply Patch-Linux.sh"));
            patchWriterLinux.NewLine = "\n";
            patchWriterLinux.WriteLine("#!/bin/sh");
            patchWriterLinux.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterLinux.WriteLine("mkdir old");
            patchWriterLinux.WriteLine("chmod +x ./" + Path.GetFileName(MainWindow.XDELTA3_BINARY_LINUX));
            // Batch creation - Mac //
            StreamWriter patchWriterMac = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "2.Apply Patch-Mac.command"));
            patchWriterMac.NewLine = "\n";
            patchWriterMac.WriteLine("#!/bin/sh");
            patchWriterMac.WriteLine("cd \"$(cd \"$(dirname \"$0\")\" && pwd)\"");
            patchWriterMac.WriteLine("mkdir ./old");
            patchWriterMac.WriteLine("chmod +x ./" + Path.GetFileName(MainWindow.XDELTA3_BINARY_MACOS));

            StreamWriter currentPatchScript = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, "doNotDelete-In-Progress.bat"));
            if (!this.MainParent.Options.CreateBatchFileOnly)
            {
                currentPatchScript.Close();
                try
                {
                    File.Delete(Path.Combine(this.MainParent.Options.PatchFileDestination, "doNotDelete-In-Progress.bat"));
                } catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                
                currentPatchScript = new StreamWriter(Path.Combine(MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat"));
            }
            List<string> oldFileNames = new List<string>();
            List<string> newFileNames = new List<string>();
            this.MainParent.OldFilesList.ForEach(c => oldFileNames.Add(c.ShortName));
            this.MainParent.NewFilesList.ForEach(c => newFileNames.Add(c.ShortName));

            for (int i = 0; i < this.MainParent.OldFilesList.Count; i++)
            {
                patchWriterWindows.WriteLine(MainWindow.XDELTA3_BINARY_WINDOWS + " -v -d -s \"{0}\" " + "\".\\" + this.MainParent.Options.PatchSubdirectory + "\\" + "{0}." + this.MainParent.Options.PatchExtention + "\" \"{2}\"", oldFileNames[i], this.MainParent.Options.PatchSubdirectory + "\\" + (i + 1).ToString(), newFileNames[i]);
                patchWriterWindows.WriteLine("move \"{0}\" old", oldFileNames[i]);
                // Batch creation - Linux //
                patchWriterLinux.WriteLine(MainWindow.XDELTA3_BINARY_LINUX + " -v -d -s \"{0}\" " + '"' + this.MainParent.Options.PatchSubdirectory + '/' + "{0}." + this.MainParent.Options.PatchExtention + "\" \"{2}\"", oldFileNames[i], this.MainParent.Options.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);
                patchWriterLinux.WriteLine("mv \"{0}\" old", oldFileNames[i]);
                // Batch creation - Mac //
                patchWriterMac.WriteLine(MainWindow.XDELTA3_BINARY_MACOS + " -v -d -s \"{0}\" " + '"' + this.MainParent.Options.PatchSubdirectory + '/' + "{0}." + this.MainParent.Options.PatchExtention + "\" \"{2}\"", oldFileNames[i], this.MainParent.Options.PatchSubdirectory + (i + 1).ToString(), newFileNames[i]);
                patchWriterMac.WriteLine("mv \"{0}\" old", oldFileNames[i]);
                
                // Script for patch creation
                if (!this.MainParent.Options.CreateBatchFileOnly)
                {
                    currentPatchScript.WriteLine("\"" + MainWindow.XDELTA3_PATH + "\""+ " " + this.MainParent.Options.XDeltaArguments + " " + "\"" + this.MainParent.OldFilesList[i].FullPath + "\" \"" + this.MainParent.NewFilesList[i].FullPath + "\" \"" + Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, oldFileNames[i]) + "." + this.MainParent.Options.PatchExtention + "\"");
                }

            }
            patchWriterWindows.WriteLine("echo Completed!");
            patchWriterWindows.WriteLine("@pause");
            patchWriterWindows.Close();
            patchWriterLinux.Close();
            patchWriterMac.Close();

            currentPatchScript.Close();

            if (!this.MainParent.Options.CreateBatchFileOnly)
            {
                new Thread(() =>
                {
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
                            string escapedArgs = "/bin/bash " + args.Replace("\"", "\\\"");
                            info.FileName = "/bin/bash";
                            info.Arguments = $"-c \"{escapedArgs}\"";
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            // Todo test
                            string args = Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat");
                            string escapedArgs = "/bin/bash " + args.Replace("\"", "\\\"");
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
                        try
                        {
                            File.Delete(Path.Combine(this.MainParent.Options.PatchFileDestination, this.MainParent.Options.PatchSubdirectory, "doNotDelete-In-Progress.bat"));
                        } catch (Exception e)
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
                { IsBackground = true }.Start();
            } else
            {
                try
                {
                    File.Delete(Path.Combine(MainParent.Options.PatchFileDestination, "doNotDelete-In-Progress.bat"));
                } catch (Exception e)
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
                ZipFile.CreateFromDirectory(this.MainParent.Options.PatchFileDestination, Path.Combine(this.MainParent.Options.PatchFileDestination,"..", this.MainParent.Options.ZipName + ".zip"));
                this.MainParent.PatchProgressIsIndeterminate = false;
            })
            { IsBackground = true}.Start();
        }

        private void HandleCMDOutput(object sender, DataReceivedEventArgs e)
        {
            double prog = 0;
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);
                this._Progress++;
                
                prog = (this._Progress / this.MainParent.OldFilesList.Count) * 100;
                this.MainParent.PatchProgress = prog > 100 ? 100 : prog;

                this.MainParent.Console.AddLine(e.Data);
            }
        }

        private void HandleCMDError(object sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null && (e.Data + "").Trim() != "")
            {
                Debug.WriteLine(e.Data);

                this.MainParent.Console.AddLine(e.Data);
            }
        }

        public void CopyExecutables()
        {
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_WINDOWS), Path.Combine(this.MainParent.Options.PatchFileDestination, MainWindow.XDELTA3_BINARY_WINDOWS), true);
            } catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_LINUX), Path.Combine(this.MainParent.Options.PatchFileDestination, MainWindow.XDELTA3_BINARY_LINUX), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            try
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "exec", MainWindow.XDELTA3_BINARY_MACOS), Path.Combine(this.MainParent.Options.PatchFileDestination, MainWindow.XDELTA3_BINARY_MACOS), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
