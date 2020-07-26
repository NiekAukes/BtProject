using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeHandUI
{
    public abstract class LeHandAction
    {
        public abstract string Parse();
    }
    public class Kpress : LeHandAction
    {
        char character = 'a';

        public Kpress(char Character)
        {
            character = Character;
        }
        public override string Parse()
        {
            string s = "Kpress(\"" + character + "\")\n";
            return s;
        }
    }
    public class Mpress : LeHandAction
    {
        short Button;
        public Mpress(short button)
        {
            Button = button;
        }
        public override string Parse()
        {
            string s = "Kpress(" + Button + ")\n";
            return s;
        }
    }
    public class MMove : LeHandAction
    {
        double dX;
        double dY;
        public MMove(double deltaX, double deltaY)
        {
            dX = deltaX;
            dY = deltaY;
        }
        public override string Parse()
        {
            string s = "Kpress(" + dX + ", " + dY + ")\n";
            return s;
        }
    }
    public class end : LeHandAction
    {
        public override string Parse()
        {
            return "return false\n";
        }
    }

    public struct Logic
    {
        public Logic(int var, double begin, double end, LeHandAction Action)
        {
            variable = var;
            beginrange = begin;
            endrange = end;
            action = Action;
        }
        public int variable;
        public double beginrange, endrange;
        public LeHandAction action;
    }
    public class LuaParser
    {
        public static readonly string[] varnames =
        {
            "LF", "RF", "MF", "IF", "Th", "AX", "AY", "AZ", "RY"
        };
        //parses Logic into Lua
        public static string Parse(Logic[] logics)
        {
            int nestvalue = 0;
            string s = "LeHand = require \"LeHand\"\nUpdate = function ()\n";
            nestvalue++;
            for (int i = 0; i < logics.Length; i++)
            {
                Logic l = logics[i];
                for (int j = 0; j < nestvalue; j++) { s += "\t"; }

                s += "if " + varnames[l.variable] + " > " + l.beginrange.ToString("G", System.Globalization.CultureInfo.InvariantCulture) +
                    " and " + varnames[l.variable] + " < " + l.endrange.ToString("G", System.Globalization.CultureInfo.InvariantCulture) + " then\n";
                nestvalue++;

                for (int j = 0; j < nestvalue; j++) { s += "\t"; }
                s += l.action.Parse();
                nestvalue--;

                for (int j = 0; j < nestvalue; j++) { s += "\t"; }
                s += "end\n\n";
            }
            nestvalue--;
            s += "end\n";
            return s;
        }
    }
}
