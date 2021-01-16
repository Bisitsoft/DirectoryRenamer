using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

ABORT

namespace DirectoryRenamer
{
    public partial class Program
    {
        static void DebugExcute(string path)
        {
            if (options.FindMode)
            {
                Console.WriteLine("In find mode:");//DEBUG
                DebugRecurrenceFind(path, 0);
            }
            else if (options.ReverseMode)
            {
                Console.WriteLine("In reverse mode:");//DEBUG
                DebugRecurrenceReverse(path, 0);

                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("In normal mode:");//DEBUG
                DebugRecurrenceDo(path, 0);

                Console.WriteLine("Done!");
            }
        }

        private static void DebugRecurrenceDo(string path, int depth)
        {
            DirectoryInfo pDir = new DirectoryInfo(path);
            DirectoryInfo[] children = pDir.GetDirectories();
            Console.WriteLine(String.Format("{0}[{1}]{2}{{", new string(' ', depth * 2), depth, path));//DEBUG
            foreach (DirectoryInfo sub in children)
            {
                Console.WriteLine(String.Format("{0}Checking \"{1}\"{{", new string(' ', (depth + 1) * 2), sub.Name));//DEBUG
                string newPath = null;

                if (options.UpdateMode)
                {
                    if (options.UseLastWordOption)
                    {
                        string word = sub.Name.Split('.').Last();
                        Console.WriteLine(String.Format("{0}Last word is {1}", new string(' ', (depth + 2) * 2), word));//DEBUG
                        #region >>>>>>>>DEBUG<<<<<<<<
                        if ((sub.Name.Length - (1 + word.Length) < 0) ||
                            (sub.Name.Substring(0, sub.Name.Length - (1 + word.Length)) != pDir.Name))//ERROR: < 0
                        {
                            Console.WriteLine(String.Format("{0}Unmatched with {1}", new string(' ', (depth + 2) * 2), pDir.Name));//DEBUG
                        }
                        else
                        {
                            Console.WriteLine(String.Format("{0}Matched with {1}", new string(' ', (depth + 2) * 2), pDir.Name));//DEBUG
                        }
						#endregion

						if ((sub.Name.Length - (1 + word.Length) < 0) ||
                            (sub.Name.Substring(0, sub.Name.Length - (1 + word.Length)) != pDir.Name))//ERROR: < 0
                        {
                            newPath = String.Format("{0}/{1}.{2}", pDir.FullName, pDir.Name, word);
                        }
                    }
                    else
                    {
						#region >>>>>>>>DEBUG<<<<<<<<
						if (Regex.Match(Regex.Escape(sub.Name), String.Format(@"^{0}\..+$", pDir.Name)).Success)
                        {
                            Console.WriteLine(String.Format("{0}Matched with {1}", new string(' ', (depth + 2) * 2), pDir.Name));//DEBUG
                        }
                        else
                        {
                            Console.WriteLine(String.Format("{0}Unmatched with {1}", new string(' ', (depth + 2) * 2), pDir.Name));//DEBUG
                        }
						#endregion
						if (!Regex.Match(Regex.Escape(sub.Name), String.Format(@"^{0}\..+$", pDir.Name)).Success)
                        {
                            newPath = String.Format("{0}/{1}.{2}", pDir.FullName, pDir.Name, sub.Name);
                        }
                    }
                    Console.WriteLine(String.Format("{0}}}", new string(' ', (depth + 1) * 2)));//DEBUG
                }
                else
                {
                    newPath = String.Format("{0}/{1}.{2}", pDir.FullName, pDir.Name, sub.Name);
                }

                Console.WriteLine(String.Format("{0}New path is {1}", new string(' ', (depth + 2) * 2), newPath == null ? "<null>" : newPath));//DEBUG
                if (newPath != null)
                {
                    Console.WriteLine(String.Format("\"{0}\" -> \"{1}\"", sub.FullName, newPath));//DEBUG
                    RenameDirectory(sub.FullName, newPath);
                    DebugRecurrenceDo(newPath, depth + 1);
                }
                else
                {
                    DebugRecurrenceDo(sub.FullName, depth + 1);
                }

                Console.WriteLine(String.Format("{0}}}", new string(' ', (depth + 1) * 2)));//DEBUG
            }
        }

        private static void DebugRecurrenceFind(string path, int depth)
        {
            DirectoryInfo sDir = new DirectoryInfo(path);
            DirectoryInfo parent = sDir.Parent;

            Console.WriteLine(String.Format("{0}[{1}]{2}{{", new string(' ', depth * 2), depth, path));//DEBUG

            bool flag;
            if (options.UseLastWordOption)
            {
                string word = sDir.Name.Split('.').Last();
                Console.WriteLine(String.Format("{0}Last word is {1}", new string(' ', (depth + 2) * 2), word));//DEBUG
                #region >>>>>>>>DEBUG<<<<<<<<
                if ((sDir.Name.Length - (1 + word.Length) < 0) ||
                    (sDir.Name.Substring(0, sDir.Name.Length - (1 + word.Length)) != parent.Name))//ERROR: < 0
                {
                    Console.WriteLine(String.Format("{0}Unmatched with {1}", new string(' ', (depth + 2) * 2), parent.Name));//DEBUG
                }
                else
                {
                    Console.WriteLine(String.Format("{0}Matched with {1}", new string(' ', (depth + 2) * 2), parent.Name));//DEBUG
                }
                #endregion
                flag = (sDir.Name.Length - (1 + word.Length) < 0) ||
                    (sDir.Name.Substring(0, sDir.Name.Length - (1 + word.Length)) == parent.Name);//ERROR: < 0
            }
            else
            {
                #region >>>>>>>>DEBUG<<<<<<<<
                if (Regex.Match(Regex.Escape(sDir.Name), String.Format(@"^{0}\..+$", parent.Name)).Success)//ERROR: < 0
                {
                    Console.WriteLine(String.Format("{0}Matched with {1}", new string(' ', (depth + 2) * 2), parent.Name));//DEBUG
                }
                else
                {
                    Console.WriteLine(String.Format("{0}Unmatched with {1}", new string(' ', (depth + 2) * 2), parent.Name));//DEBUG
                }
                #endregion
                flag = Regex.Match(Regex.Escape(sDir.Name), String.Format(@"^{0}\..+$", parent.Name)).Success;
            }

            if (flag)
            {
                DebugRecurrenceFind(parent.FullName, depth + 1);
            }
            else
            {
                Console.WriteLine(sDir.FullName);
            }

            Console.WriteLine(String.Format("{0}}}", new string(' ', depth * 2), depth, path));//DEBUG
        }

        private static void DebugRecurrenceReverse(string path, int depth)
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
                    if ((sub.Name.Length - (1 + word.Length) < 0) ||
                        (sub.Name.Substring(0, sub.Name.Length - (1 + word.Length)) == pDir.Name))//ERROR: < 0
                    {
                        newPath = String.Format("{0}/{1}", pDir.FullName, word);
                    }
                }
                else
                {
                    //检查该子目录是否符合命名规则
                    if (Regex.Match(Regex.Escape(sub.Name), String.Format("^{0}\\..+$", pDir.Name)).Success)
                    {
                        newPath = String.Format("{0}/{1}", pDir.FullName, sub.Name.Substring(pDir.Name.Length + 1));
                    }
                }

                Console.WriteLine(String.Format("{0}New path is {1}", new string(' ', (depth + 2) * 2), newPath == null ? "<null>" : newPath));//DEBUG
                if (newPath != null)
                {
                    DebugRecurrenceReverse(sub.FullName, depth + 1);
                    Console.WriteLine(String.Format("\"{0}\" -> \"{1}\"", sub.FullName, newPath));//DEBUG

                    RenameDirectory(sub.FullName, newPath);
                }
            }
        }
    }
}
