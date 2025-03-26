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
            pictureBox1.Location = new Point(14, 16);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(899, 708);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // AddImage
            // 
            AddImage.Location = new Point(920, 16);
            AddImage.Margin = new Padding(3, 4, 3, 4);
            AddImage.Name = "AddImage";
            AddImage.Size = new Size(254, 66);
            AddImage.TabIndex = 1;
            AddImage.Text = "Add Image";
            AddImage.UseVisualStyleBackColor = true;
            AddImage.Click += AddImage_Click;
            // 
            // proButton
            // 
            proButton.Location = new Point(3, 4);
            proButton.Margin = new Padding(3, 4, 3, 4);
            proButton.Name = "proButton";
            proButton.Size = new Size(254, 72);
            proButton.TabIndex = 3;
            proButton.Text = "Calculate";
            proButton.UseVisualStyleBackColor = true;
            proButton.Visible = false;
            proButton.Click += proButton_Click;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(923, 90);
            saveButton.Margin = new Padding(3, 4, 3, 4);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(251, 63);
            saveButton.TabIndex = 4;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // lyrBox2
            // 
            lyrBox2.AutoScroll = true;
            lyrBox2.Controls.Add(proButton);
            lyrBox2.Location = new Point(920, 157);
            lyrBox2.Margin = new Padding(3, 4, 3, 4);
            lyrBox2.Name = "lyrBox2";
            lyrBox2.Size = new Size(254, 547);
            lyrBox2.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1187, 759);
            Controls.Add(lyrBox2);
            Controls.Add(saveButton);
            Controls.Add(AddImage);
            Controls.Add(pictureBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Fotoshop";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            lyrBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Button AddImage;
        private Button proButton;
        private Button saveButton;
        private FlowLayoutPanel lyrBox2;
    }
}
