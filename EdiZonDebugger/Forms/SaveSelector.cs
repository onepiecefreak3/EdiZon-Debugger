using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdiZonDebugger
{
    public partial class SaveSelector : Form
    {
        string[] _saveFiles = null;

        public string SelectedFile { get => _saveFiles[saveComboBox.SelectedIndex]; }
        public bool ProperExit { get; private set; }

        public SaveSelector(string[] files)
        {
            InitializeComponent();

            _saveFiles = files;
            ProperExit = false;

            UpdateUI();
            UpdateComboBox();
        }

        private void UpdateUI()
        {
            var enabled = _saveFiles != null;

            saveComboBox.Enabled = enabled;
            OKBtn.Enabled = enabled;
        }

        private void UpdateComboBox()
        {
            saveComboBox.DataSource = _saveFiles;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            ProperExit = true;
            Close();
        }
    }
}
