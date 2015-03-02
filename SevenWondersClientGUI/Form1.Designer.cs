namespace SevenWondersClientGUI
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.startclientButton = new System.Windows.Forms.Button();
            this.wonderBox = new System.Windows.Forms.PictureBox();
            this.moezolotoLabel = new System.Windows.Forms.Label();
            this.headlineLabel = new System.Windows.Forms.Label();
            this.boardContainer = new System.Windows.Forms.GroupBox();
            this.boosterContainer = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.wonderBox)).BeginInit();
            this.boardContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(50, 22);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(367, 303);
            this.listBox1.TabIndex = 1;
            // 
            // startclientButton
            // 
            this.startclientButton.Location = new System.Drawing.Point(444, 22);
            this.startclientButton.Name = "startclientButton";
            this.startclientButton.Size = new System.Drawing.Size(85, 23);
            this.startclientButton.TabIndex = 2;
            this.startclientButton.Text = "Start Client";
            this.startclientButton.UseVisualStyleBackColor = true;
            this.startclientButton.Click += new System.EventHandler(this.startclientButton_Click);
            // 
            // wonderBox
            // 
            this.wonderBox.Location = new System.Drawing.Point(65, 492);
            this.wonderBox.Name = "wonderBox";
            this.wonderBox.Size = new System.Drawing.Size(1480, 270);
            this.wonderBox.TabIndex = 3;
            this.wonderBox.TabStop = false;
            // 
            // moezolotoLabel
            // 
            this.moezolotoLabel.AutoSize = true;
            this.moezolotoLabel.Location = new System.Drawing.Point(589, 51);
            this.moezolotoLabel.Name = "moezolotoLabel";
            this.moezolotoLabel.Size = new System.Drawing.Size(78, 13);
            this.moezolotoLabel.TabIndex = 6;
            this.moezolotoLabel.Text = "MOE ZOLOTO";
            // 
            // headlineLabel
            // 
            this.headlineLabel.AutoSize = true;
            this.headlineLabel.Location = new System.Drawing.Point(589, 22);
            this.headlineLabel.Name = "headlineLabel";
            this.headlineLabel.Size = new System.Drawing.Size(61, 13);
            this.headlineLabel.TabIndex = 8;
            this.headlineLabel.Text = "HEADLINE";
            // 
            // boardContainer
            // 
            this.boardContainer.Controls.Add(this.boosterContainer);
            this.boardContainer.Controls.Add(this.wonderBox);
            this.boardContainer.Location = new System.Drawing.Point(12, 344);
            this.boardContainer.Name = "boardContainer";
            this.boardContainer.Size = new System.Drawing.Size(1566, 785);
            this.boardContainer.TabIndex = 10;
            this.boardContainer.TabStop = false;
            // 
            // boosterContainer
            // 
            this.boosterContainer.Location = new System.Drawing.Point(6, 19);
            this.boosterContainer.Name = "boosterContainer";
            this.boosterContainer.Size = new System.Drawing.Size(566, 163);
            this.boosterContainer.TabIndex = 4;
            this.boosterContainer.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1604, 1157);
            this.Controls.Add(this.boardContainer);
            this.Controls.Add(this.headlineLabel);
            this.Controls.Add(this.moezolotoLabel);
            this.Controls.Add(this.startclientButton);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.wonderBox)).EndInit();
            this.boardContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button startclientButton;
        private System.Windows.Forms.PictureBox wonderBox;
        private System.Windows.Forms.Label moezolotoLabel;
        private System.Windows.Forms.Label headlineLabel;
        private System.Windows.Forms.GroupBox boardContainer;
        private System.Windows.Forms.GroupBox boosterContainer;
    }
}

