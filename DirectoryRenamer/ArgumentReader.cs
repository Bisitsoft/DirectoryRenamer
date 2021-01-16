using System;
using System.Linq;
using System.Text.RegularExpressions;

using OurOpenSource.TextUnit;

namespace DirectoryRenamer
{
    public static class ArgumentReader
    {
        public static bool Read(string[] args,ref ProgramOptions options)
        {
            if (args.Length > 0)
            {
                var _debugOption=from val in args
                                 where val == "-D" || val == "--debug"
                                 select val;
                if (_debugOption.Count() > 0)
                {
                    options.DebugMode = true;
                }
                if (options.DebugMode)//DEBUG
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine(String.Format("args[{0}] = \"{1}\".", i, args[i]));
                    }
                }

                var _continueOption = from val in args
                                      where val == "-c" || val == "--continue"
                                      select val;
                if (_continueOption.Count() > 0)
                {
                    options.ContinueOption = true;
                }

                var _helpOption = from val in args
                                  where val == "-h" || val == "--help"
                                  select val;
                if (_helpOption.Count() > 0)
                {
                    HelpViewer.View();

                    Program.Exit(0);
                }

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-c":
                        case "--continue":
                            break;
                        case "-D":
                        case "--debug":
                            break;
                        case "-f":
                        case "--find":
                            options.UpdateMode = true;//auto
                            options.FindMode = true;
                            break;
                        case "-h":
                        case "--help":
                            break;
                        case "-r":
                        case "--reverse":
                            options.UpdateMode = true;//auto
                            options.ReverseMode = true;
                            break;
                        case "-u":
                        case "--update":
                            options.UpdateMode = true;
                            break;
                        case "-U":
                            options.UpdateMode = true;//auto
                            options.UseLastWordOption = true;
                            break;
                        case "-V":
                        case "--view":
                            options.ViewOption = true;
                            break;
                        default:
                            if (Regex.Match(args[i], @"^-!.*").Success)
                            {
                                switch (args[i].Substring(2))
                                {
                                    case "--233":
                                    case "--2333":
                                        options.ViewOption = false;
                                        break;
                                    case "-V":
                                    case "--view":
                                        options.ViewOption = false;
                                        break;
                                    default:
                                        throw new ArgumentException(String.Format("-! not support \"{0}\".", args[i].Substring(2)), String.Format("args[{0}]", i));
                                        //Program.Exit(-1);
                                }
                                if(Regex.Match(args[i].Substring(2), "^--23+$").Success)
                                {
                                    Console.WriteLine(args[i].Substring(4));
                                }
                            }
                            else if (options.Path == null)
                            {
                                options.Path = args[i];
                                if (!Program.CheckDirectory(options.Path))
                                {
                                    throw new ArgumentException("The dircetory isn't exist.", "path");
                                    //Exit(-1);
                                }
                            }
                            else
                            {
                                throw new ArgumentException(String.Format("Unknown argument \"{0}\".", args[i]), String.Format("args[{0}]", i));
                                //Program.Exit(-1);
                            }
                            break;
                    }
                }

                options.CheckOption();
                return true;
            }
            return false;
        }
    }
}
