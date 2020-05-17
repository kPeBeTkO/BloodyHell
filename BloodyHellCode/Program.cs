﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BloodyHell
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TestForm());
            Application.Run(new GameForm(new List<string>() { "Castle", "Dungeon", "Dimentions", "Level1", "Maze" }));
            //Application.Run(new GameForm(new List<string>() {   "Maze" }));
        }
    }
}
