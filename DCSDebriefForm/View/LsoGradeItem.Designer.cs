using DCSDebriefForm.View;

namespace DCSDebriefForm
{
    partial class LsoGradeItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelGradeIndicator = new Panel();
            labelGrade = new Label();
            labelLsoGrade = new Label();
            tableLayoutPanel = new TableLayoutPanel();
            labelWire = new Label();
            labelUnitType = new Label();
            labelCarrier = new Label();
            labelPilot = new Label();
            labelDateTime = new Label();
            listView = new ResizableListView();
            columnHeaderError = new ColumnHeader();
            columnHeaderTranslation = new ColumnHeader();
            labelGradeTranslation = new Label();
            tableLayoutPanelMain = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanelMain.SuspendLayout();
            SuspendLayout();
            // 
            // panelGradeIndicator
            // 
            panelGradeIndicator.BackColor = Color.Red;
            panelGradeIndicator.Location = new Point(850, 21);
            panelGradeIndicator.Margin = new Padding(20);
            panelGradeIndicator.Name = "panelGradeIndicator";
            panelGradeIndicator.Size = new Size(16, 16);
            panelGradeIndicator.TabIndex = 0;
            // 
            // labelGrade
            // 
            labelGrade.Dock = DockStyle.Fill;
            labelGrade.ForeColor = Color.Black;
            labelGrade.Location = new Point(894, 1);
            labelGrade.Name = "labelGrade";
            labelGrade.Size = new Size(194, 61);
            labelGrade.TabIndex = 0;
            labelGrade.Text = "No Grade";
            labelGrade.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelLsoGrade
            // 
            labelLsoGrade.Dock = DockStyle.Fill;
            labelLsoGrade.ForeColor = Color.Black;
            labelLsoGrade.Location = new Point(1176, 1);
            labelLsoGrade.Name = "labelLsoGrade";
            labelLsoGrade.Size = new Size(785, 61);
            labelLsoGrade.TabIndex = 0;
            labelLsoGrade.Text = "Error codes";
            labelLsoGrade.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.ColumnCount = 8;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.Controls.Add(labelWire, 6, 0);
            tableLayoutPanel.Controls.Add(labelUnitType, 2, 0);
            tableLayoutPanel.Controls.Add(labelCarrier, 3, 0);
            tableLayoutPanel.Controls.Add(labelPilot, 1, 0);
            tableLayoutPanel.Controls.Add(panelGradeIndicator, 4, 0);
            tableLayoutPanel.Controls.Add(labelDateTime, 0, 0);
            tableLayoutPanel.Controls.Add(labelLsoGrade, 7, 0);
            tableLayoutPanel.Controls.Add(labelGrade, 5, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(3, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(1176, 63);
            tableLayoutPanel.TabIndex = 1;
            // 
            // labelWire
            // 
            labelWire.Dock = DockStyle.Fill;
            labelWire.ForeColor = Color.Black;
            labelWire.Location = new Point(1095, 1);
            labelWire.Name = "labelWire";
            labelWire.Size = new Size(74, 61);
            labelWire.TabIndex = 5;
            labelWire.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelUnitType
            // 
            labelUnitType.Dock = DockStyle.Fill;
            labelUnitType.ForeColor = Color.Black;
            labelUnitType.Location = new Point(381, 1);
            labelUnitType.Name = "labelUnitType";
            labelUnitType.Size = new Size(194, 61);
            labelUnitType.TabIndex = 4;
            labelUnitType.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCarrier
            // 
            labelCarrier.Dock = DockStyle.Fill;
            labelCarrier.ForeColor = Color.Black;
            labelCarrier.Location = new Point(582, 1);
            labelCarrier.Name = "labelCarrier";
            labelCarrier.Size = new Size(244, 61);
            labelCarrier.TabIndex = 3;
            labelCarrier.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelPilot
            // 
            labelPilot.Dock = DockStyle.Fill;
            labelPilot.ForeColor = Color.Black;
            labelPilot.Location = new Point(180, 1);
            labelPilot.Name = "labelPilot";
            labelPilot.Size = new Size(194, 61);
            labelPilot.TabIndex = 2;
            labelPilot.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelDateTime
            // 
            labelDateTime.Dock = DockStyle.Fill;
            labelDateTime.ForeColor = Color.Black;
            labelDateTime.Location = new Point(4, 1);
            labelDateTime.Name = "labelDateTime";
            labelDateTime.Size = new Size(169, 61);
            labelDateTime.TabIndex = 1;
            labelDateTime.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // listView
            // 
            listView.Anchor =    AnchorStyles.Top  |  AnchorStyles.Bottom   |  AnchorStyles.Left   |  AnchorStyles.Right ;
            listView.Columns.AddRange(new ColumnHeader[] { columnHeaderError, columnHeaderTranslation });
            listView.ForeColor = Color.Black;
            listView.Location = new Point(3, 72);
            listView.Name = "listView";
            listView.Size = new Size(1176, 171);
            listView.TabIndex = 2;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderError
            // 
            columnHeaderError.Text = "Error";
            // 
            // columnHeaderTranslation
            // 
            columnHeaderTranslation.Text = "Translation";
            columnHeaderTranslation.Width = 360;
            // 
            // labelGradeTranslation
            // 
            labelGradeTranslation.Dock = DockStyle.Fill;
            labelGradeTranslation.ForeColor = Color.Black;
            labelGradeTranslation.Location = new Point(583, 0);
            labelGradeTranslation.Name = "labelGradeTranslation";
            labelGradeTranslation.Size = new Size(596, 61);
            labelGradeTranslation.TabIndex = 2;
            labelGradeTranslation.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.Controls.Add(listView, 0, 1);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanel, 0, 0);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 2;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 28.04878F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 71.95122F));
            tableLayoutPanelMain.Size = new Size(1182, 246);
            tableLayoutPanelMain.TabIndex = 3;
            // 
            // LsoGradeItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanelMain);
            Name = "LsoGradeItem";
            Size = new Size(1182, 246);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanelMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelGradeIndicator;
        private Label labelGrade;
        private Label labelLsoGrade;
        private TableLayoutPanel tableLayoutPanel;
        private Label labelDateTime;
        private Label labelGradeTranslation;
        private ResizableListView listView;
        private TableLayoutPanel tableLayoutPanelMain;
        private ColumnHeader columnHeaderError;
        private ColumnHeader columnHeaderTranslation;
        private Label labelCarrier;
        private Label labelPilot;
        private Label labelWire;
        private Label labelUnitType;
    }
}
