using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using CustomCommonDialog;

namespace CustomColorDialogClient
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button button1;
        private TextBox textBoxR;
        private TextBox textBoxG;
        private TextBox textBoxB;
        private TextBox textBoxA;
        private Label labelOutputColor;
        private Label label2;
        private TextBox textBoxColor0;
        private TextBox textBoxColor1;
        private TextBox textBoxColor2;
        private Label labelCustomColors;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxR = new System.Windows.Forms.TextBox();
            this.textBoxG = new System.Windows.Forms.TextBox();
            this.textBoxB = new System.Windows.Forms.TextBox();
            this.textBoxA = new System.Windows.Forms.TextBox();
            this.labelOutputColor = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxColor0 = new System.Windows.Forms.TextBox();
            this.textBoxColor1 = new System.Windows.Forms.TextBox();
            this.textBoxColor2 = new System.Windows.Forms.TextBox();
            this.labelCustomColors = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(100, 238);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(336, 163);
            this.button1.TabIndex = 0;
            this.button1.Text = "Show Custom Color Dialog";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxR
            // 
            this.textBoxR.Location = new System.Drawing.Point(172, 22);
            this.textBoxR.Name = "textBoxR";
            this.textBoxR.Size = new System.Drawing.Size(66, 31);
            this.textBoxR.TabIndex = 1;
            this.textBoxR.Text = "0";
            // 
            // textBoxG
            // 
            this.textBoxG.Location = new System.Drawing.Point(250, 22);
            this.textBoxG.Name = "textBoxG";
            this.textBoxG.Size = new System.Drawing.Size(66, 31);
            this.textBoxG.TabIndex = 2;
            this.textBoxG.Text = "0";
            // 
            // textBoxB
            // 
            this.textBoxB.Location = new System.Drawing.Point(328, 22);
            this.textBoxB.Name = "textBoxB";
            this.textBoxB.Size = new System.Drawing.Size(66, 31);
            this.textBoxB.TabIndex = 3;
            this.textBoxB.Text = "0";
            // 
            // textBoxA
            // 
            this.textBoxA.Location = new System.Drawing.Point(406, 22);
            this.textBoxA.Name = "textBoxA";
            this.textBoxA.Size = new System.Drawing.Size(66, 31);
            this.textBoxA.TabIndex = 4;
            this.textBoxA.Text = "0";
            // 
            // labelOutputColor
            // 
            this.labelOutputColor.AutoSize = true;
            this.labelOutputColor.Location = new System.Drawing.Point(20, 428);
            this.labelOutputColor.Name = "labelOutputColor";
            this.labelOutputColor.Size = new System.Drawing.Size(133, 25);
            this.labelOutputColor.TabIndex = 5;
            this.labelOutputColor.Text = "Output Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "R G B A";
            // 
            // textBoxColor0
            // 
            this.textBoxColor0.Location = new System.Drawing.Point(12, 107);
            this.textBoxColor0.Name = "textBoxColor0";
            this.textBoxColor0.Size = new System.Drawing.Size(174, 31);
            this.textBoxColor0.TabIndex = 7;
            this.textBoxColor0.Text = "0";
            // 
            // textBoxColor1
            // 
            this.textBoxColor1.Location = new System.Drawing.Point(198, 107);
            this.textBoxColor1.Name = "textBoxColor1";
            this.textBoxColor1.Size = new System.Drawing.Size(174, 31);
            this.textBoxColor1.TabIndex = 8;
            this.textBoxColor1.Text = "0";
            // 
            // textBoxColor2
            // 
            this.textBoxColor2.Location = new System.Drawing.Point(386, 107);
            this.textBoxColor2.Name = "textBoxColor2";
            this.textBoxColor2.Size = new System.Drawing.Size(174, 31);
            this.textBoxColor2.TabIndex = 9;
            this.textBoxColor2.Text = "0";
            // 
            // labelCustomColors
            // 
            this.labelCustomColors.AutoSize = true;
            this.labelCustomColors.Location = new System.Drawing.Point(24, 471);
            this.labelCustomColors.Name = "labelCustomColors";
            this.labelCustomColors.Size = new System.Drawing.Size(153, 25);
            this.labelCustomColors.TabIndex = 10;
            this.labelCustomColors.Text = "Custom Colors";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(10, 24);
            this.ClientSize = new System.Drawing.Size(638, 590);
            this.Controls.Add(this.labelCustomColors);
            this.Controls.Add(this.textBoxColor2);
            this.Controls.Add(this.textBoxColor1);
            this.Controls.Add(this.textBoxColor0);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelOutputColor);
            this.Controls.Add(this.textBoxA);
            this.Controls.Add(this.textBoxB);
            this.Controls.Add(this.textBoxG);
            this.Controls.Add(this.textBoxR);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            Application.Run(new Form1());
        }

        private void button1_Click(object sender, System.EventArgs e)
        {

            Color initColor = Color.FromArgb(int.Parse(textBoxA.Text) % 256, int.Parse(textBoxR.Text) % 256, int.Parse(textBoxG.Text) % 256, int.Parse(textBoxB.Text) % 256);

            int[] customColors = { int.Parse(textBoxColor0.Text), int.Parse(textBoxColor1.Text), int.Parse(textBoxColor2.Text) };
            CustomColorDialog colDlg = new CustomColorDialog(Handle, initColor, 128, customColors);
            colDlg.IntialColor = initColor;
            bool f = colDlg.ShowDialog();

            if (f == true)
            {
                labelOutputColor.Text = colDlg.SelectedColor.ToString();
                labelOutputColor.BackColor = colDlg.SelectedColor;
                customColors = colDlg.CustomColors;

                string strCustomColors = string.Empty;
                foreach (int cc in customColors)
                {
                    strCustomColors += cc.ToString() + "|";
                }
                strCustomColors.TrimEnd('|');

                labelCustomColors.Text = strCustomColors;
            }
        
        }
    }
}
