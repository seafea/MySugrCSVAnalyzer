using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySugrCSVAnalyzer.Controllers;

namespace MySugrCSVAnalyzer
{
    public partial class frmMain : Form
    {
        private readonly Controllers.ReadInputController _inputFileController;
        public frmMain()
        {
            InitializeComponent();
            _inputFileController = new ReadInputController();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            setLabelMessageAndVisibility(
                label: lblErrorMessage,
                message: "",
                visibility: false);
            setLabelMessageAndVisibility(
                label: lblStatusMessage,
                message: "",
                visibility: false);
            OpenFileDialog picker = new OpenFileDialog();
            DialogResult pickerResult = picker.ShowDialog();
            if (pickerResult == DialogResult.OK)
            {
                _inputFileController.fileName = picker.FileName;
                setLabelMessageAndVisibility(
                    label: lblStatusMessage,
                    message: picker.FileName + " currently selected.",
                    visibility: true);
                _inputFileController.readInputFile();
            }
        }

        private static void setLabelMessageAndVisibility(Label label, string message, bool visibility)
        {
            label.Text = message;
            label.Visible = visibility;
        }
    }
}
