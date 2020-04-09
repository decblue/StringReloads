﻿using StringReloads.Engine;
using System;
using System.Linq;

namespace StringReloads.StringModifier
{
    class MonoWordWrap : IStringModifier
    {
        int? Size = null;
        int Width => (Size ?? Config.Default.Width);
        public MonoWordWrap(Main Main) {
            Main.OnFlagTriggered += (Flag, Cancel) =>
            {
                string Name = Flag.Name.ToUpperInvariant();
                if (Name != "MAXWIDTH" && Name != "WIDTH")
                    return;
                
                Cancel.Cancel = true;
                if (int.TryParse(Flag.Value, out int Size))
                    this.Size = Size;
                else
                    this.Size = null;
            };
        }

        public string Name => "MonoWordWrap";

        public bool CanRestore => true;

        public string Apply(string String, string Original)
        {
            int CurrentLength = 0;
            string Result = string.Empty;
            string BreakLine = Config.Default.BreakLine;
            int Width = this.Width;

            if (Original != null)
            {
                string[] OriLines = Original.Replace(BreakLine, "\x0").Split('\x0');
                if (Width <= 0)
                {
                    if (OriLines.Length == 1 && Width == 0)
                        Width = OriLines.First().Length;
                    else if (OriLines.Length > 1)
                        Width = (from x in OriLines orderby x.Length descending select x.Length).First();
                    else
                        Width = Math.Abs(Width);
                }
            }
            else Width = Math.Abs(Width);

            if (!String.Contains(" "))
                return String;

            String = String.Replace(BreakLine, " ").Replace("  ", "");
            foreach (var Word in String.Split(' ')) {
                if (CurrentLength > 0 && CurrentLength + Word.Length > Width) {
                    Result = Result.TrimEnd();
                    Result += BreakLine;
                    CurrentLength = 0;
                }
                Result += Word + " ";
                CurrentLength += Word.Length + 1;
            }

            return Result;
        }

        public string Restore(string String)
        {
            return String.Replace(Config.Default.BreakLine, " ");
        }
    }
}
