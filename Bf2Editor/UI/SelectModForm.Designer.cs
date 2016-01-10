namespace BF2Editor.UI
{
    partial class SelectModForm
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
            this.ModComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ModComboBox
            // 
            this.ModComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModComboBox.FormattingEnabled = true;
            this.ModComboBox.Location = new System.Drawing.Point(25, 88);
            this.ModComboBox.Name = "ModComboBox";
            this.ModComboBox.Size = new System.Drawing.Size(235, 21);
            this.ModComboBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(22, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 46);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please select one of the following mods found within the Battlefield 2 directory";
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(105, 125);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(75, 23);
            this.SelectButton.TabIndex = 2;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // SelectModForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 160);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ModComboBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectModForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select A Mod";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox ModComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SelectButton;
    }
}