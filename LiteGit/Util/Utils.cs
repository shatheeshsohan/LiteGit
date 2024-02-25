using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LiteGit.Util
{
    class Utils
    {
        public static bool IsItemExists (ListBox listBox, string item) 
        {
            var match = listBox.Items.Cast<String>().ToList().FirstOrDefault(x => x == item);
            if(match != null)
            {
                return true;
            }
            return false;
        }
    }
}
