using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Settings;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Managers;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Virtualni herni stul
    /// </summary>
     [Serializable]
    public class Table : A_TableObject
    {
        protected new FRectangle shape;
        protected new TableSettings settings;

        public Table()
        {
            settings = new TableSettings();
            this.shape = new FRectangle();
            shape.Height = CommonAttribService.DEFAULT_TABLE_HEIGHT;
            shape.Width = CommonAttribService.DEFAULT_TABLE_WIDTH;
        }

        public new TableSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value is TableSettings) this.settings = (TableSettings)value;
                else throw new Exception("Spatny argument");
            }
        }

         /// <summary>
         /// Vrati tvar stolu
         /// </summary>
        public new FRectangle Shape
        {
            get
            {
                return shape;
            }
            set
            {
                if(value is FRectangle)
                this.shape = (FRectangle)value;
            }
        }
    }
}
