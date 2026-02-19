using System;
using System.Collections.Generic;
using Shared.Platform;

namespace Werewolf_Control.Models
{
    public class Menu
    {
        /// <summary>
        /// The actions you want in your menu.
        /// </summary>
        public List<PlatformAction> Actions { get; set; }

        /// <summary>
        /// How many columns.  Defaults to 1.
        /// </summary>
        public int Columns { get; set; }

        public Menu(int col = 1, List<PlatformAction> actions = null)
        {
            Actions = actions ?? new List<PlatformAction>();
            Columns = Math.Max(col, 1);
        }
    }
}
