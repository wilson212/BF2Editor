using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BF2Editor.UI
{
    public partial class SelectModForm : Form
    {
        public SelectModForm()
        {
            InitializeComponent();

            foreach (BF2Mod mod in BF2Client.Mods)
            {
                ModComboBox.Items.Add(mod);
            }
        }

        public BF2Mod GetSelectedMod()
        {
            if (ModComboBox.SelectedIndex == -1)
                return null;

            return (BF2Mod)ModComboBox.SelectedItem;
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
