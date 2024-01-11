using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.MdbgEngine;

namespace DotNetDebuggerWV
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                return;
            var engine = new MDbgEngine();
            engine.Options.ShowFullPaths = true;
            engine.Options.StopOnException = true;
            var process = engine.CreateProcess(args[0], "", DebugModeFlag.Default, null);
            string logFile = "DebuggerLog.txt";
            File.WriteAllText(logFile, "");
            while (process.IsAlive)
            {
                process.Go().WaitOne();
                object o = process.StopReason;        
                ExceptionThrownStopReason m = o as ExceptionThrownStopReason;
                if (m != null)
                {
                    try
                    {
                        MDbgThread t = process.Threads.Active;
                        MDbgValue ex = t.CurrentException;
                        MDbgFrame f = t.CurrentFrame;
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Exception is thrown:" + ex.TypeName + "(");
                        sb.Append(m.EventType + ") at function " + f.Function.FullName);
                        if (t.CurrentSourcePosition != null)
                            sb.AppendLine(" in source file:" + t.CurrentSourcePosition.Path + ":" + t.CurrentSourcePosition.Line);
                        else
                            sb.AppendLine(" (cant find pdb for symbols to get source position)");
                        Console.Write(sb.ToString());
                        File.AppendAllText(logFile, sb.ToString());
                    }
                    catch (Exception e)
                    {
                        string msg = "Exception is thrown, but can't inspect it.";
                        Console.WriteLine(msg);
                        File.AppendAllText(logFile, msg);
                    }
                }
            }
            Console.WriteLine("Done!");
            File.AppendAllText(logFile, "Done!");
        }        
    }
}
