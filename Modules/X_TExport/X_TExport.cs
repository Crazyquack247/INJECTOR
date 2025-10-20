using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Text.RegularExpressions;
using System.IO;
using INJECTOR.Core.Services;

namespace INJECTOR.Modules.X_TExport
{
    /// <summary>
    /// Exports active .sldprt or .sldasm to:
    /// F:\Edgecam\Engineering Transfer\<Company>\<PartNumber>\<PartNumber>.x_t
    /// </summary>
    
    public class X_TExport : IModule
    {
        private ISldWorks _swApp;

        public X_TExport(ISldWorks swApp)
        {
            _swApp = swApp;
        }

        public void Initialize(ISldWorks swApp) { _swApp = swApp; }
        public void Terminate() { }

        public void RunExport()
        {
            try
            {
                ModelDoc2 doc = _swApp.IActiveDoc2;
                if (doc == null)
                {
                    MessageBox.Show("No active document to export.");
                    return;
                }

                int type = doc.GetType();
                if (type != (int)swDocumentTypes_e.swDocPART && type != (int)swDocumentTypes_e.swDocASSEMBLY)
                {
                    MessageBox.Show("Only Part or Assembly documents can be exported to .x_t.");
                    return;
                }

                string docPath = doc.GetPathName();
                if (string.IsNullOrWhiteSpace(docPath))
                {
                    MessageBox.Show("Please save the document before exporting.");
                    return;
                }

                string companyName = ExtractCompanyName(docPath);
                string partName = Path.GetFileNameWithoutExtension(docPath);

                // Build export path from settings

                var exportRoot = SettingsService.Instance.Current.ExportRoot;
                string targetDir = Path.Combine(exportRoot, companyName, partName);
                Directory.CreateDirectory(targetDir);

                // only pass directory path to the form, not the file path

                using (var form = new ExportForm(partName, targetDir))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string finalPath = form.FinalPath;
                        int errors = 0, warnings = 0;

                        bool result = doc.Extension.SaveAs(
                            finalPath,
                            (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                            (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                            null,
                            ref errors,
                            ref warnings
                        );

                        if (result)
                        {
                            _swApp.SendMsgToUser($"Exported successfully:\n{finalPath}");

                            // If user checked "Open Folder", open it
                            if (form.OpenFolderAfterSave)
                            {
                                string folder = Path.GetDirectoryName(finalPath);
                                if (Directory.Exists(folder))
                                    System.Diagnostics.Process.Start("explorer.exe", folder);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Export failed (Error {errors})", "X_T Export Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during export: {ex.Message}", "X_T Export Error");
            }
        }

        private string ExtractCompanyName(string fullPath)
        {
            try
            {
                var parts = fullPath.Split(Path.DirectorySeparatorChar);
                int idx = Array.IndexOf(parts, "DRAWING FILES");
                if (idx >= 0 && idx + 1 < parts.Length)
                    return parts[idx + 1];
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}