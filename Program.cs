using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StartElevated
{
  class Program
  {
    private static readonly string _exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

    static void Main(string[] args)
    {
      Console.WriteLine("StartElevated 2015 - jorgie@missouri.edu - ({0})", _exeName);
      if (args.Length < 1)
      {
        ShowUse();
        return;
      }
      else
      {
        string cmd = args[0];

        if (cmd.Equals("?") || cmd.Equals("/?") || cmd.Equals("/h"))
        {
          ShowUse();
          return;
        }

        string options = "";

        if (args.Length > 1)
        {
          int index = Environment.GetCommandLineArgs()[0].Length + Environment.GetCommandLineArgs()[1].Length + 3;
          options = Environment.CommandLine.Substring(index);
        }

        Console.WriteLine("Command: " + cmd);
        Console.WriteLine("Options: " + options);
        Console.WriteLine("Current: " + Environment.CurrentDirectory);
        StartElevated(cmd, options);
      }
    }

    private static void ShowUse()
    {
      string msg = string.Format(
        "\r\n  This program simply launches a program while asking for elevation.\r\n\r\n" + 
        "  Example: {0} c:\\path\\program.exe opt1 opt2\r\n\r\n" +
        "  Remember that during the elevation proces, the working directory becomes: %windir%\\system32",
        _exeName
      );
      Console.Error.WriteLine(msg);
    }

    private static void StartElevated(string EXEPath, string Options)
    {
      ProcessStartInfo psi = new ProcessStartInfo(EXEPath)
      {
        UseShellExecute = true,
        Verb = "runas",
        Arguments = Options,
      };

      var p = new Process
      {
        EnableRaisingEvents = true, // enable WaitForExit()
        StartInfo = psi
      };

      try { p.Start(); }
      catch (SystemException se) {ShowError(se); }
    }

    private static void ShowError(SystemException se)
    {
      Console.Error.WriteLine("* " + se.Message);
      if (se.InnerException != null) Console.Error.WriteLine("* " + se.InnerException.Message);
      if (se.Message.StartsWith("No application is associated"))
        Console.Error.WriteLine("* {0} only works with executables", _exeName);
    }
  }
}
