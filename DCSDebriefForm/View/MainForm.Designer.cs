using DCSDebriefForm.View;

namespace DCSDebriefForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if( settings != null&& settings.Data != null )
                UpdateDB();

            if( watcher != null )
            {
                watcher.StopWatching();
                watcher.Dispose();
                watcher = null;
            }

            if( readReader != null )
                readReader.Dispose();

            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lsoGradeListBox = new FlowLayoutPanelEx();
            SuspendLayout();
            // 
            // lsoGradeListBox
            // 
            lsoGradeListBox.AutoScroll = true;
            lsoGradeListBox.Dock = DockStyle.Fill;
            lsoGradeListBox.FlowDirection = FlowDirection.TopDown;
            lsoGradeListBox.Location = new Point(0, 0);
            lsoGradeListBox.Name = "lsoGradeListBox";
            lsoGradeListBox.Size = new Size(1687, 804);
            lsoGradeListBox.TabIndex = 0;
            lsoGradeListBox.WrapContents = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1687, 804);
            Controls.Add(lsoGradeListBox);
            DoubleBuffered = true;
            Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "DCS World LSO Grades";
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanelEx lsoGradeListBox;

    }


}
