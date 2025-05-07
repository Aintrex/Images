namespace Imagesss
{
    partial class Form1
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
            if (disposing && (components != null))
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
            pictureBox1 = new PictureBox();
            AddImage = new Button();
            proButton = new Button();
            saveButton = new Button();
            lyrBox2 = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            lyrBox2.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(776, 494);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // AddImage
            // 
            AddImage.Location = new Point(794, 12);
            AddImage.Name = "AddImage";
            AddImage.Size = new Size(233, 50);
            AddImage.TabIndex = 1;
            AddImage.Text = "Add Image";
            AddImage.UseVisualStyleBackColor = true;
            AddImage.Click += AddImage_Click;
            // 
            // proButton
            // 
            proButton.Location = new Point(3, 3);
            proButton.Name = "proButton";
            proButton.Size = new Size(222, 54);
            proButton.TabIndex = 3;
            proButton.Text = "Calculate";
            proButton.UseVisualStyleBackColor = true;
            proButton.Visible = false;
            proButton.Click += proButton_Click;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(794, 68);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(233, 47);
            saveButton.TabIndex = 4;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // lyrBox2
            // 
            lyrBox2.AutoScroll = true;
            lyrBox2.AutoSize = true;
            lyrBox2.Controls.Add(proButton);
            lyrBox2.Location = new Point(794, 118);
            lyrBox2.Name = "lyrBox2";
            lyrBox2.Size = new Size(233, 439);
            lyrBox2.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1039, 569);
            Controls.Add(lyrBox2);
            Controls.Add(saveButton);
            Controls.Add(AddImage);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Fotoshop";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            lyrBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button AddImage;
        private Button proButton;
        private Button saveButton;
        private FlowLayoutPanel lyrBox2;
    }
}
