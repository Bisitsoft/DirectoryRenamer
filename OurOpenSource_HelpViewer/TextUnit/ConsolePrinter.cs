using System;

namespace OurOpenSource.TextUnit
{
    public class ConsolePrinter
    {
        public int ScreenWidth;
        /// <summary>
        /// Please set a half-angle character.
        /// </summary>
        public char OuterCorner = '@';
        /// <summary>
        /// Please set a half-angle character.
        /// </summary>
        public char TransverseBorder = '=';
        /// <summary>
        /// Please set a half-angle character.
        /// </summary>
        public char VerticalBorder = 'I';
        /// <summary>
        /// Please set a half-angle character.
        /// </summary>
        public char InteriorCorner = '*';
        /// <summary>
        /// Please set a half-angle character.
        /// </summary>
        public char TransverseLine = '-';
        /// <summary>
        /// Please set a half-angle character.
        /// </summary>
        public char VerticalLine = '|';

        //return real use seats.(Withour '\n'
        )
        public int PrintTitle(string title)
        {
            ;
        }

        /// <summary>
        /// Initializing TextPrinter.
        /// </summary>
        /// <param name="screenWidth">The screen width.(If it <=0, it will become Console.BufferWidth)</param>
        public ConsolePrinter(int screenWidth = 0)
        {
            if (screenWidth > 0)
            {
                this.ScreenWidth = screenWidth;
            }
            else
            {
                this.ScreenWidth = Console.BufferWidth;
            }
        }
    }
}
