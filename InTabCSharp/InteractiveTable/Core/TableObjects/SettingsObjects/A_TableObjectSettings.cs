using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractiveTable.Core.Data.TableObjects.SettingsObjects
{
    /// <summary>
    /// Spolecne nastaveni pro vsechny herni objekty vcetne stolu
    /// </summary>
     [Serializable]
    public abstract class A_TableObjectSettings
    {
         /// <summary>
         /// Pokud true, je objekt povolen
         /// </summary>
        public Boolean enabled = true; 
    }
}
