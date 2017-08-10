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
        private readonly ReadInputController inputFileController;
        public frmMain()
        {
            InitializeComponent();
            inputFileController = new ReadInputController();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            setLabelMessageAndVisibility(
                label: this.lblErrorMessage,
                message: string.Empty,
                visibility: false);
            setLabelMessageAndVisibility(
                label: this.lblStatusMessage,
                message: string.Empty,
                visibility: false);
            OpenFileDialog picker = new OpenFileDialog();
            DialogResult pickerResult = picker.ShowDialog();
            if (pickerResult == DialogResult.OK)
            {
                inputFileController.fileName = picker.FileName;
                setLabelMessageAndVisibility(
                    label: this.lblStatusMessage,
                    message: picker.FileName + " currently selected.",
                    visibility: true);
                inputFileController.readInputFile();
                inputFileController.LoadLogbookByDay();
                int? averageHappyReadings = inputFileController.GetAverageOfReadingsWithSpecifiedTag("Happy");
                if (averageHappyReadings != null)
                {
                    MessageBox.Show("Average of all readings tagged happy: " + averageHappyReadings.ToString());
                }
            }
        }

        private static void setLabelMessageAndVisibility(Label label, string message, bool visibility)
        {
            label.Text = message;
            label.Visible = visibility;
        }
    }
}
