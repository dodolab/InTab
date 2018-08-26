using System;
using InteractiveTable.Core.Data.TableObjects.Shapes;
using InteractiveTable.Core.Data.TableObjects.SettingsObjects;
using InteractiveTable.Managers;

namespace InteractiveTable.Core.Data.TableObjects.FunctionObjects
{
    /// <summary>
    /// Entity for the table
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
                else throw new Exception("Bad argument");
            }
        }

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
