using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{ 
    internal class StringDrawer
    {
        private char[,] canvas;

        public int length => canvas.GetLength(0);
        public int height => canvas.GetLength(1);

        public StringDrawer(int length, int height)
        {
            canvas = new char[length, height];
        }

        public StringDrawer draw(int x, int y, string value)
        {
            if (x < 0 || y < 0 || x >= length || y >= height)
                return this;

            for (int i = 0; i < value.Length; i++) {
                canvas[x + i, y] = value[i];
            }

            return this;
        }

        public StringDrawer erase(int x, int y, int length) => draw(x, y, "".PadLeft(length));

        public string render()
        {
            var r = "";
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < length; x++) {
                    if (canvas[x, y] == '\0') r += ' ';
                    else r += canvas[x, y];
                }
                r += '\n';
            }
            return r;
        }
    }
}
