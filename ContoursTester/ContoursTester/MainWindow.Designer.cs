namespace ContoursViewer
{
    partial class MainWindow
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showNumberOfPoints = new System.Windows.Forms.ToolStripMenuItem();
            this._showDelaunayTriangulation = new System.Windows.Forms.ToolStripMenuItem();
            this._showContours = new System.Windows.Forms.ToolStripMenuItem();
            this._showBezierSegments = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeContoursIntervalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(909, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open ...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save As ...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._showNumberOfPoints,
            this._showDelaunayTriangulation,
            this._showContours,
            this._showBezierSegments});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // _showNumberOfPoints
            // 
            this._showNumberOfPoints.Name = "_showNumberOfPoints";
            this._showNumberOfPoints.Size = new System.Drawing.Size(229, 22);
            this._showNumberOfPoints.Text = "Show Number  of Points";
            this._showNumberOfPoints.Click += new System.EventHandler(this.showNumberOfPointsToolStripMenuItem_Click);
            // 
            // _showDelaunayTriangulation
            // 
            this._showDelaunayTriangulation.Name = "_showDelaunayTriangulation";
            this._showDelaunayTriangulation.Size = new System.Drawing.Size(229, 22);
            this._showDelaunayTriangulation.Text = "Show Delaunay Triangulation";
            this._showDelaunayTriangulation.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // _showContours
            // 
            this._showContours.Name = "_showContours";
            this._showContours.Size = new System.Drawing.Size(229, 22);
            this._showContours.Text = "Show Contours";
            this._showContours.Click += new System.EventHandler(this.showToolStripMenuItem1_Click);
            // 
            // _showBezierSegments
            // 
            this._showBezierSegments.Name = "_showBezierSegments";
            this._showBezierSegments.Size = new System.Drawing.Size(229, 22);
            this._showBezierSegments.Text = "Show Bezier Segments";
            this._showBezierSegments.Click += new System.EventHandler(this.showBezierSegmentsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeContoursIntervalToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // changeContoursIntervalToolStripMenuItem
            // 
            this.changeContoursIntervalToolStripMenuItem.Name = "changeContoursIntervalToolStripMenuItem";
            this.changeContoursIntervalToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.changeContoursIntervalToolStripMenuItem.Text = "Change Contours Interval ...";
            this.changeContoursIntervalToolStripMenuItem.Click += new System.EventHandler(this.changeContoursIntervalToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 524);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _showNumberOfPoints;
        private System.Windows.Forms.ToolStripMenuItem _showDelaunayTriangulation;
        private System.Windows.Forms.ToolStripMenuItem _showContours;
        private System.Windows.Forms.ToolStripMenuItem _showBezierSegments;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeContoursIntervalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;

    }
}

