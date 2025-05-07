namespace Imagesss
{
    partial class FuraForm
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
            pictureBoxFiltered = new PictureBox();
            pictureBoxFilter = new PictureBox();
            comboBox1 = new ComboBox();
            button1 = new Button();
            textBox1 = new TextBox();
            pictureBoxSpectrum = new PictureBox();
            comboBox2 = new ComboBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFiltered).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFilter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpectrum).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxFiltered
            // 
            pictureBoxFiltered.Location = new Point(12, 203);
            pictureBoxFiltered.Name = "pictureBoxFiltered";
            pictureBoxFiltered.Size = new Size(391, 235);
            pictureBoxFiltered.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxFiltered.TabIndex = 0;
            pictureBoxFiltered.TabStop = false;
            // 
            // pictureBoxFilter
            // 
            pictureBoxFilter.Location = new Point(409, 203);
            pictureBoxFilter.Name = "pictureBoxFilter";
            pictureBoxFilter.Size = new Size(388, 235);
            pictureBoxFilter.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxFilter.TabIndex = 1;
            pictureBoxFilter.TabStop = false;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Идеальный", "Гаусс ФНЧ", "Гаусс ФВЧ" });
            comboBox1.Location = new Point(15, 15);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(197, 23);
            comboBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(15, 44);
            button1.Name = "button1";
            button1.Size = new Size(197, 40);
            button1.TabIndex = 3;
            button1.Text = "Calculate";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(218, 15);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(297, 23);
            textBox1.TabIndex = 4;
            // 
            // pictureBoxSpectrum
            // 
            pictureBoxSpectrum.Location = new Point(409, 54);
            pictureBoxSpectrum.Name = "pictureBoxSpectrum";
            pictureBoxSpectrum.Size = new Size(388, 143);
            pictureBoxSpectrum.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSpectrum.TabIndex = 5;
            pictureBoxSpectrum.TabStop = false;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "Низких частот", "Высоких частот" });
            comboBox2.Location = new Point(521, 15);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(262, 23);
            comboBox2.TabIndex = 6;
            comboBox2.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 36F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(15, 105);
            label1.Name = "label1";
            label1.Size = new Size(138, 55);
            label1.TabIndex = 7;
            label1.Text = "label1";
            // 
            // FuraForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(comboBox2);
            Controls.Add(pictureBoxSpectrum);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(comboBox1);
            Controls.Add(pictureBoxFilter);
            Controls.Add(pictureBoxFiltered);
            Name = "FuraForm";
            Text = "Form3";
            ((System.ComponentModel.ISupportInitialize)pictureBoxFiltered).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFilter).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpectrum).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxFiltered;
        private PictureBox pictureBoxFilter;
        private ComboBox comboBox1;
        private Button button1;
        private TextBox textBox1;
        private PictureBox pictureBoxSpectrum;
        private ComboBox comboBox2;
        private Label label1;
    }
}