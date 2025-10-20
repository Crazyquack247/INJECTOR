using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace INJECTOR.Modules.X_TExport
{
    public partial class ExportForm : Form
    {
        public string FinalPath { get; private set; }
        public bool OpenFolderAfterSave => chkOpenFolder.Checked;
        public string SelectedFolderPath { get; private set; } = "";

        public ExportForm(string defaultName, string defaultPath)
        {
            InitializeComponent();
            txtFileName.Text = defaultName;
            lblFolderPath.Text = defaultPath;
            chkOpenFolder.Checked = true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Title = "Select folder to export .x_t file";
                dialog.InitialDirectory = Directory.Exists(SelectedFolderPath)
                    ? SelectedFolderPath
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SelectedFolderPath = dialog.FileName;
                    lblFolderPath.Text = SelectedFolderPath;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = txtFileName.Text.Trim();
                string folderPath = lblFolderPath.Text.Trim();

                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderPath))
                {
                    MessageBox.Show("File name and path cannot be empty.", "Validation Error");
                    return;
                }

                // Ensure directory exists
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                FinalPath = Path.Combine(folderPath, fileName + ".x_t");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Save Error");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkOpenFolder_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
