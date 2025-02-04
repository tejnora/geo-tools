namespace ContoursViewer
{
    partial class ContoursIntervalDialog
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

        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this._intervalInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._minHeight = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._maxHeight = new System.Windows.Forms.Label();
            this._ok = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._elevationDifference = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Contours interval:";
            // 
            // _intervalInput
            // 
            this._intervalInput.Location = new System.Drawing.Point(121, 80);
            this._intervalInput.Name = "_intervalInput";
            this._intervalInput.Size = new System.Drawing.Size(133, 20);
            this._intervalInput.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minimum height:";
            // 
            // _minHeight
            // 
            this._minHeight.AutoSize = true;
            this._minHeight.Location = new System.Drawing.Point(118, 13);
            this._minHeight.Name = "_minHeight";
            this._minHeight.Size = new System.Drawing.Size(35, 13);
            this._minHeight.TabIndex = 3;
            this._minHeight.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Maximum height:";
            // 
            // _maxHeight
            // 
            this._maxHeight.AutoSize = true;
            this._maxHeight.Location = new System.Drawing.Point(118, 38);
            this._maxHeight.Name = "_maxHeight";
            this._maxHeight.Size = new System.Drawing.Size(35, 13);
            this._maxHeight.TabIndex = 5;
            this._maxHeight.Text = "label5";
            // 
            // _ok
            // 
            this._ok.Location = new System.Drawing.Point(93, 113);
            this._ok.Name = "_ok";
            this._ok.Size = new System.Drawing.Size(75, 23);
            this._ok.TabIndex = 6;
            this._ok.Text = "OK";
            this._ok.UseVisualStyleBackColor = true;
            this._ok.Click += new System.EventHandler(this._ok_Click);
            // 
            // _cancel
            // 
            this._cancel.Location = new System.Drawing.Point(174, 113);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 7;
            this._cancel.Text = "Cancel";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Elevation difference:";
            // 
            // _elevationDifference
            // 
            this._elevationDifference.AutoSize = true;
            this._elevationDifference.Location = new System.Drawing.Point(118, 62);
            this._elevationDifference.Name = "_elevationDifference";
            this._elevationDifference.Size = new System.Drawing.Size(35, 13);
            this._elevationDifference.TabIndex = 9;
            this._elevationDifference.Text = "label5";
            // 
            // ContoursIntervalDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 153);
            this.Controls.Add(this._elevationDifference);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._ok);
            this.Controls.Add(this._maxHeight);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._minHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._intervalInput);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ContoursIntervalDialog";
            this.Text = "ContoursIntervalDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _intervalInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label _minHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label _maxHeight;
        private System.Windows.Forms.Button _ok;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label _elevationDifference;
    }
}