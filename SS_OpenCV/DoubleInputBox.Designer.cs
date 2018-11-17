namespace SS_OpenCV
{
    partial class DoubleInputBox
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
            this.ValueTextBox1 = new System.Windows.Forms.TextBox();
            this.ValueTextBox2 = new System.Windows.Forms.TextBox();
            this.ValueLabel1 = new System.Windows.Forms.Label();
            this.ValueLabel2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ValueTextBox1
            // 
            this.ValueTextBox1.Location = new System.Drawing.Point(99, 43);
            this.ValueTextBox1.Name = "ValueTextBox1";
            this.ValueTextBox1.Size = new System.Drawing.Size(136, 22);
            this.ValueTextBox1.TabIndex = 0;
            // 
            // ValueTextBox2
            // 
            this.ValueTextBox2.Location = new System.Drawing.Point(99, 81);
            this.ValueTextBox2.Name = "ValueTextBox2";
            this.ValueTextBox2.Size = new System.Drawing.Size(136, 22);
            this.ValueTextBox2.TabIndex = 1;
            // 
            // ValueLabel1
            // 
            this.ValueLabel1.AutoSize = true;
            this.ValueLabel1.Location = new System.Drawing.Point(10, 46);
            this.ValueLabel1.Name = "ValueLabel1";
            this.ValueLabel1.Size = new System.Drawing.Size(52, 17);
            this.ValueLabel1.TabIndex = 2;
            this.ValueLabel1.Text = "Value1";
            // 
            // ValueLabel2
            // 
            this.ValueLabel2.AutoSize = true;
            this.ValueLabel2.Location = new System.Drawing.Point(10, 84);
            this.ValueLabel2.Name = "ValueLabel2";
            this.ValueLabel2.Size = new System.Drawing.Size(52, 17);
            this.ValueLabel2.TabIndex = 3;
            this.ValueLabel2.Text = "Value2";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(99, 125);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // DoubleInputBox
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 166);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ValueLabel2);
            this.Controls.Add(this.ValueLabel1);
            this.Controls.Add(this.ValueTextBox2);
            this.Controls.Add(this.ValueTextBox1);
            this.Name = "DoubleInputBox";
            this.Text = "Double Input Box";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox ValueTextBox1;
        public System.Windows.Forms.TextBox ValueTextBox2;
        public System.Windows.Forms.Label ValueLabel1;
        public System.Windows.Forms.Label ValueLabel2;
        private System.Windows.Forms.Button button1;
    }
}