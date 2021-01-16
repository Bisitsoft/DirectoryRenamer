using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using OurOpenSource.TextUnit;

namespace DirectoryRenamer
{
    public partial class Program
    {
        private static ProgramOptions options = new ProgramOptions();

        private static void Main(string[] args)
        {
            InitHelp();
            bool hasArgs = ArgumentReader.Read(args, ref options);

            if (!hasArgs)
            {
                if (options.Path == null)
                {
                    Console.Write("Root dircetory path: ");
                    options.Path = Console.ReadLine();
                }
                if (!CheckDirectory(options.Path))
                {
                    throw new ArgumentException("The dircetory isn't exist.", "path");
                    //Exit(-1);
                }
            }
            
            if (options.DebugMode)
            {
                Console.WriteLine(
                    "Debug              [" + options.DebugMode.ToString() +         "]\n" +
                    "View               [" + options.ViewOption.ToString() +        "]\n" +
                    "ContinueOption     [" + options.ContinueOption.ToString() +    "]\n" +
                    "FindMode           [" + options.FindMode.ToString() +          "]\n" +
                    "ReverseMode        [" + options.ReverseMode.ToString() +       "]\n" +
                    "UpdateMode         [" + options.UpdateMode.ToString() +        "]\n" +
                    "UseLastWord        [" + options.UseLastWordOption.ToString() + "]\n" +
                    "Path = \"" + options.Path + "\"\n"
                    );
            }

            Excute(options.Path);

            Exit(0);
        }

        // //此处将Debug和普通模式分离仅为提高效率，请在维护时请注意非Debug部分代码相同。
        #region ===Excutions===
        private static int depth;

        private static void Excute(string path)
        {
            if (options.DebugMode)
            {
                depth = 0;
            }

            if (options.FindMode)
            {
                RecurrenceFind(path);
            }
            else if (options.ReverseMode)
            {
                RecurrenceReverse(path);

                Console.WriteLine("Done!");
            }
            else
            {
                RecurrenceDo(path);

                Console.WriteLine("Done!");
            }
        }

        private static void RecurrenceDo(string path)
        {
            DirectoryInfo pDir = new DirectoryInfo(path);
            DirectoryInfo[] children = pDir.GetDirectories();
            foreach (DirectoryInfo sub in children)
            {
                string newPath = null;

                if (options.UpdateMode)
                {
                    if (options.UseLastWordOption)
                    {
                        string word = sub.Name.Split('.').Last();
                        if ((sub.Name.Length - (1 + word.Length) < 0) ||
                            (sub.Name.Substring(0, sub.Name.Length - (1 + word.Length)) != pDir.Name))//ERROR: < 0
                        {
                            newPath = String.Format("{0}/{1}.{2}", pDir.FullName, pDir.Name, word);
                        }
                    }
                    else
                    {
                        if (!Regex.Match(sub.Name, String.Format(@"^{0}\..+$", pDir.Name)).Success)
                        {
                            newPath = String.Format("{0}/{1}.{2}", pDir.FullName, pDir.Name, sub.Name);
                        }
                    }
                }
                else
                {
                    newPath = String.Format("{0}/{1}.{2}", pDir.FullName, pDir.Name, sub.Name);
                }

                if (newPath != null)
                {
                    if (options.ViewOption)//VIEW
                    {
                        Console.WriteLine(String.Format("\"{0}\" -> \"{1}\"", sub.FullName, newPath));
                    }
                    RenameDirectory(sub.FullName, newPath);
                    RecurrenceDo(newPath);
                }
                else
                {
                    RecurrenceDo(sub.FullName);
                }
            }
        }

        private static void RecurrenceFind(string path)
        {
            DirectoryInfo sDir = new DirectoryInfo(path);
            DirectoryInfo parent = sDir.Parent;

            bool flag;
            if (options.UseLastWordOption)
            {
                string word = sDir.Name.Split('.').Last();
                flag = (sDir.Name.Length - (1 + word.Length) < 0) ||
                    (sDir.Name.Substring(0, sDir.Name.Length - (1 + word.Length)) == parent.Name);//ERROR: < 0
            }
            else
            {
                flag = Regex.Match(sDir.Name, String.Format(@"^{0}\..+$", parent.Name)).Success;
            }

            if (flag)
            {
                RecurrenceFind(parent.FullName);
            }
            else
            {
                Console.WriteLine(sDir.FullName);
            }
        }

        private static void RecurrenceReverse(string path)
        {
            DirectoryInfo pDir = new DirectoryInfo(path);
            DirectoryInfo[] children = pDir.GetDirectories();

            foreach (DirectoryInfo sub in children)
            {
                string newPath = null;

                if (options.UseLastWordOption)
                {
                    //取出最后一个词
                    string word = sub.Name.Split('.').Last();

                    //检查该子目录是否符合命名规则
                    if ((sub.Name.Length - (1 + word.Length) > 0) &&
                        (sub.Name.Substring(0, sub.Name.Length - (1 + word.Length)) == pDir.Name))//ERROR: < 0
                    {
                        newPath = String.Format("{0}/{1}", pDir.FullName, word);
                    }
                }
                else
                {
                    //检查该子目录是否符合命名规则
                    if (Regex.Match(sub.Name, String.Format("^{0}\\..+$", pDir.Name)).Success)
                    {
                        newPath = String.Format("{0}/{1}", pDir.FullName, sub.Name.Substring(pDir.Name.Length + 1));
                    }
                }

                if (newPath != null)
                {
                    RecurrenceReverse(sub.FullName);
                    if (options.ViewOption)//VIEW
                    {
                        Console.WriteLine(String.Format("\"{0}\" -> \"{1}\"", sub.FullName, newPath));
                    }
                    RenameDirectory(sub.FullName, newPath);
                }
            }
        }
        #endregion

        private static void RenameDirectory(string oldPath, string newPath)
        {
            DirectoryInfo oldDir = new DirectoryInfo(oldPath);
            DirectoryInfo newDir = new DirectoryInfo(newPath);
            if (newDir.Exists)
            {
                throw new ArgumentException(String.Format("The dircetory \"{0}\" has been existed. Please solve this problem and use argument -u retry later.", newDir.FullName), "path");
                //Exit(-1);
            }

            Directory.Move(oldDir.FullName, newDir.FullName);
        }

        /// <summary>
        /// Check directory path format is right or wrong.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>Is directory exist.</returns>
        public static bool CheckDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(options.Path);
            return di.Exists;
        }

        private static void EndContinue()
        {
            if (options.ContinueOption)
            {
                Console.Write("(Press return(enter) key to continue) ");
                Console.ReadLine();
            }
        }
        public static void Exit(int exitCode)
        {
            EndContinue();
            Environment.Exit(exitCode);
        }

        public static void InitHelp()
        {
            HelpViewer.Header.Add("DirectoryRenamer   Help:");
            HelpViewer.Header.Add("Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            HelpViewer.Header.Add("Description: A tool to make the directories to be easy to search for some requirements.");
            HelpViewer.Header.Add("");
            HelpViewer.Header.Add("Command Format:");
            HelpViewer.Header.Add("  DirectoryRenamer (<arguments>) <path>");

            HelpViewer.Help.Add("-c --continue", "Keep console not to die after program finished.");
            HelpViewer.Help.Add("-D --debug"   , "Get more information for debug.");
            HelpViewer.Help.Add("-f --find"    , "Try find the root of the directory.(This option will auto set -u)");
            HelpViewer.Help.Add("-h --help"    , "Get help.");
            HelpViewer.Help.Add("-r --reverse" , "Let the program remove the struct of name.(This option will auto set -u)");
            HelpViewer.Help.Add("-u --update"  , "If the name of the directory is meeting the requirements, it will not be changed.\n" +
                                                 "Example: The name \"A.B.C\", if the \"A.B\" or \"A\" is it's parent directory name, is will not be changed.");
            HelpViewer.Help.Add("-U"           , "Read the last word of the directory name and open the update mode.(This option will auto set and over -u)\n" +
                                                 "Example: The name \"A.B.C\", if the \"A\" is it's parent directory name, it didn't pass -u rule, and it will become \"A.E\" not \"A.B.C\".");
            HelpViewer.Help.Add("-V --view"    , "Watch what did the program do.(Defalut set)");
            HelpViewer.Help.Add("-!<arugument>", "Turn off the switch.\n" +
                                                 "Support: -V.");

            HelpViewer.Footer.Add("Auth: Orange233");
            HelpViewer.Footer.Add("Copyright: Copyright(c) 2020 Bisitsoft");
            HelpViewer.Footer.Add("Website: https://www.ourorangenet.com/");
            HelpViewer.Footer.Add("Feedback here: https://github.com/Orange23333/DirectoryRenamer/issues");
        }
    }
}
