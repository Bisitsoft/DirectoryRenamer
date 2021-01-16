using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//从DirectoryRenamer那里复制过来的。

namespace OurOpenSource.TextUnit
{
    public static class HelpViewer
    {
        public static List<string> Header = new List<string>();
        public static Dictionary<string, string> Help = new Dictionary<string, string>();
        public static List<string> Footer=new List<string>();

        //option为保留格式设定参数
        public static void View()
        {
            int w = Console.BufferWidth - 1;//貌似回车占一个位置，所以这里只好用一个笨方法，空出一个位置

            if (w < 6)//"*Help*".Length == 6
            {
                SimpleView();
            }
            else
            {
                ProTitle("Help", w);
                ProList(Header, w);
                ProBar(w);
                ProHelp(w);
                ProBar(w);
                ProList(Footer, w);
                ProBar(w);
            }
        }
        private static void ProBar(int bufferWidth)
        {
            ProTitle("", bufferWidth);
        }
        private static void ProTitle(string title, int bufferWidth)
        {
            ////Needn't safe check. !!!But should make w >= title.Length + 2!!!
            if (bufferWidth < title.Length + 2)
            {
                throw new InvalidOperationException("Buffer width is too small.");
            }

            Console.Write("*");
            Console.Write(new string('-', (bufferWidth - (title.Length + 2)) / 2));//Floor
            Console.Write(title);
            Console.Write(new string('-', (bufferWidth - (title.Length + 2) + 1) / 2));//Cell
            Console.WriteLine("*");
        }
        private static string[] AutoSplitLine(string source, int bufferWidth, char fillChar = '\0')
        {
            List<string> r = new List<string>();

            string[] splited = source.Split('\n');

            int i;
            foreach(string temp in splited)
            {
                i = 0;
                while (true)
                {
                    if (temp.Length - i > bufferWidth)
                    {
                        r.Add(temp.Substring(i, bufferWidth));
                        i += bufferWidth;
                    }
                    else
                    {
                        if(fillChar != '\0' && temp.Length - i < bufferWidth)
                        {
                            r.Add(temp.Substring(i) + new string(fillChar, bufferWidth - (temp.Length - i)));
                        }
                        else
                        {
                            r.Add(temp.Substring(i));
                        }

                        break;
                    }
                }
            }

            return r.ToArray();
        }
        private static void ProList(List<string> list, int bufferWidth)
        {
            int w = Console.BufferWidth;
            int w_2 = w - 2;

            ////Needn't safe check. !!!But should make w > 2!!!
            if (w <= 2)
            {
                throw new InvalidOperationException("Buffer width <= 2");
            }

            foreach (string temp in list)
            {
                string[] lines = AutoSplitLine(temp, bufferWidth - 2 , ' ');
                foreach(string line in lines)
                {
                    Console.Write("|");
                    Console.Write(line);
                    Console.WriteLine("|");
                }
            }
        }
        private static string[] GetStandradArgumentLines(string argLine)
        {
            int i;
            List<string> r = new List<string>();
            StringBuilder temp;

            temp = new StringBuilder("");
            for (i = 0; i < argLine.Length; i++)
            {
                if (Char.IsWhiteSpace(argLine[i]) || argLine[i] == '\n')
                {
                    if (temp.ToString() != "")
                    {
                        r.Add(temp.ToString());
                        temp = new StringBuilder("");
                    }
                }
                else
                {
                    temp.Append(argLine[i]);
                }
            }
            if (temp.ToString() != "")
            {
                r.Add(temp.ToString());
            }

            return r.ToArray();
        }
        private static void ProHelp(int bufferWidth)
        {
            int w_3 = bufferWidth - 3;
            int i, iend, cnt;
            int lgstArg, w1, w2;

            ////Needn't safe check. !!!But should make w >= 5!!!
            if (bufferWidth < 5)
            {
                throw new InvalidOperationException("Buffer width <= 2");
            }

            //找出最长的参数
            //对于"  -a  \n --abs"这样的最长值为"--abs"的长度，也就是5
            lgstArg = 0;
            foreach(string temp in Help.Keys)
            {
                string[] argLines = GetStandradArgumentLines(temp);
                foreach(string line in argLines)
                {
                    if (line.Length > lgstArg)
                    {
                        lgstArg = line.Length;
                    }
                }
            }
            //分配屏幕宽度
            if ((float)lgstArg / w_3 < 0.25)
            {
                w1 = lgstArg;
            }
            else
            {
                w1 = (int)(w_3 * 0.25);
            }
            w2 = w_3 - w1;
            
            foreach (KeyValuePair<string, string> temp in Help)
            {
                StringBuilder argInOneLine = new StringBuilder("");
                string[] argsNotInOneLine = GetStandradArgumentLines(temp.Key);
                foreach (string line in argsNotInOneLine)
                {
                    argInOneLine.Append(line);
                    argInOneLine.Append("\n");
                }argInOneLine.Remove(argInOneLine.Length - 1, 1);
                string[] argLines = AutoSplitLine(argInOneLine.ToString(), w1, ' ');

                string[] dscLines = AutoSplitLine(temp.Value, w2, ' ');

                iend = argLines.Length > dscLines.Length ? argLines.Length : dscLines.Length;
                for (i = 0; i < iend; i++)
                {
                    Console.Write("|");
                    if(i < argLines.Length)
                    {
                        Console.Write(argLines[i]);
                        if (argLines[i].Length < w1)
                        {
                            Console.Write(new string(' ', w1 - argLines[i].Length));
                        }
                    }
                    else
                    {
                        Console.Write(new string(' ', w1));
                    }
                    Console.Write("|");
                    if(i< dscLines.Length)
                    {
                        Console.Write(dscLines[i]);
                    }
                    else
                    {
                        Console.Write(new string(' ', w2));
                    }
                    Console.WriteLine("|");
                }
            }
        }

        private static void SimpleView()
        {
            Console.WriteLine("Help:");

            SimpleList(Header);

            Console.WriteLine();

            SimpleHelp();

            Console.WriteLine();

            SimpleList(Footer);
        }
        private static void SimpleHelp()
        {
            Console.WriteLine("Argument|Description");
            foreach (KeyValuePair<string, string> help in Help)
            {
                Console.WriteLine(String.Format("{0}|{1}", help.Key, help.Value));
            }
        }
        private static void SimpleList(List<string> list)
        {
            foreach(string temp in list)
            {
                Console.WriteLine(temp);
            }
        }
    }
}
