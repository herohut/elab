namespace EmbeddedOffice
{
    partial class Form1
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
            this.btnUnload = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.msWordControl1 = new EmbeddedOffice.MsWordControl();
            this.SuspendLayout();
            // 
            // btnUnload
            // 
            this.btnUnload.Location = new System.Drawing.Point(464, 260);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(75, 23);
            this.btnUnload.TabIndex = 3;
            this.btnUnload.Text = "Unload";
            this.btnUnload.UseVisualStyleBackColor = true;
            this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(373, 261);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "NEW";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(373, 304);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(464, 303);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 5;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // msWordControl1
            // 
            this.msWordControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msWordControl1.Location = new System.Drawing.Point(0, 0);
            this.msWordControl1.Name = "msWordControl1";
            this.msWordControl1.Size = new System.Drawing.Size(912, 545);
            this.msWordControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 545);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUnload);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.msWordControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private MsWordControl msWordControl1;
        private System.Windows.Forms.Button btnUnload;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpenFile;

    }
}

