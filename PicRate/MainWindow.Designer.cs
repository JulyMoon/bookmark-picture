namespace PicRate
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
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.imageList = new System.Windows.Forms.ListView();
            this.backButton = new System.Windows.Forms.Button();
            this.forwardButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Location = new System.Drawing.Point(12, 12);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(608, 342);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox.TabIndex = 0;
            this.imageBox.TabStop = false;
            // 
            // imageList
            // 
            this.imageList.Location = new System.Drawing.Point(626, 12);
            this.imageList.Name = "imageList";
            this.imageList.Size = new System.Drawing.Size(170, 303);
            this.imageList.TabIndex = 1;
            this.imageList.UseCompatibleStateImageBehavior = false;
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(626, 321);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(69, 33);
            this.backButton.TabIndex = 2;
            this.backButton.Text = "<---";
            this.backButton.UseVisualStyleBackColor = true;
            // 
            // forwardButton
            // 
            this.forwardButton.Location = new System.Drawing.Point(701, 321);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(95, 33);
            this.forwardButton.TabIndex = 3;
            this.forwardButton.Text = "--->";
            this.forwardButton.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 366);
            this.Controls.Add(this.forwardButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.imageList);
            this.Controls.Add(this.imageBox);
            this.Name = "MainWindow";
            this.Text = "PicRate";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imageBox;
        private System.Windows.Forms.ListView imageList;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button forwardButton;
    }
}

