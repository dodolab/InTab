using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractiveTable.Core.Data.Capture;

namespace InteractiveTable.GUI.CaptureSet
{
    public partial class TemplateEditor : Form
    {
        Templates templates;

        public TemplateEditor(Templates templates)
        {
            InitializeComponent();

            this.templates = templates;
            templates.Sort((t1, t2) => t1.name.CompareTo(t2.name));
            UpdateInterface();
        }

        void UpdateInterface()
        {
            dgvTemplates.RowCount = templates.Count;
        }

        private void dgvTemplates_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < templates.Count)
            {
                switch (e.ColumnIndex)
                {
                    case 0:
                        e.Value = e.RowIndex;
                        break;
                    case 1:
                        e.Value = templates[e.RowIndex].name;
                        break;
                    case 2:
                        e.Value = templates[e.RowIndex].preferredAngleNoMore90;
                        break;
                }
            }
        }

        private void dgvTemplates_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < templates.Count && e.ColumnIndex == 1)
                templates[e.RowIndex].name = e.Value.ToString();
        }

        private void dgvTemplates_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedCells.Count > 0)
            {
                int iRow = dgvTemplates.SelectedCells[0].RowIndex;
                if (iRow >= 0 && iRow < templates.Count)
                {
                    Refresh();
                    templates[iRow].Draw(CreateGraphics(), new Rectangle(dgvTemplates.Bounds.Right + 10, dgvTemplates.Bounds.Top, 300, 200));
                }
            }
        }

        private void cbPreferredAngle_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedCells.Count > 0)
            {
                int iRow = dgvTemplates.SelectedCells[0].RowIndex;
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedCells.Count > 0)
            {
                int iRow = dgvTemplates.SelectedCells[0].RowIndex;
                if (iRow >= 0 && iRow < templates.Count)
                {
                    try
                    {
                        templates.rockSettings.RemoveWhere(rck => rck.contour_name.Equals(templates.ElementAt(iRow).name));
                        templates.RemoveAt(iRow);
                        UpdateInterface();
                        Refresh();
                    }
                    catch { }
                }
            }
        }



    }
}
