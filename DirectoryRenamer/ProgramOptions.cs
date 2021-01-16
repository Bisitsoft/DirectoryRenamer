using System;

namespace DirectoryRenamer
{
    public class ProgramOptions
    {
        public bool DebugMode = false;
        public bool ViewOption = true;

        public bool ContinueOption = false;
        public bool FindMode = false;
        public bool ReverseMode = false;
        public bool UpdateMode = false;
        public bool UseLastWordOption = false;

        public string Path = null;

        public void CheckOption()
        {
            if (FindMode)
            {
                if (ReverseMode)
                {
                    throw new ArgumentException("-f and -r can still together.", "args");
                }
                if (!UpdateMode)
                {
                    throw new ArgumentException("When using -f, the -u switch must be on.", "args");
                }
            }
            if(ReverseMode && !UpdateMode)
            {
                throw new ArgumentException("When using -r, the -u switch must be on.", "args");
            }
            if (UseLastWordOption && !UpdateMode)
            {
                throw new ArgumentException("When using -U, the -u switch must be on.", "args");
            }
            if (Path == null)
            {
                throw new ArgumentException("The path can't be null.", "args");
                //Program.Exit(-1);
            }
        }

        public ProgramOptions()
        {
            ;
        }
    }
}
