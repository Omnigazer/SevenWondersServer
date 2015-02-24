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
            this.pickcardButton = new System.Windows.Forms.Button();
            this.sellcardButton = new System.Windows.Forms.Button();
            this.moezolotoLabel = new System.Windows.Forms.Label();
            this.headlineLabel = new System.Windows.Forms.Label();
            this.playcardButton = new System.Windows.Forms.Button();
            this.boardContainer = new System.Windows.Forms.GroupBox();
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
            this.wonderBox.Location = new System.Drawing.Point(283, 304);
            this.wonderBox.Name = "wonderBox";
            this.wonderBox.Size = new System.Drawing.Size(783, 270);
            this.wonderBox.TabIndex = 3;
            this.wonderBox.TabStop = false;
            // 
            // pickcardButton
            // 
            this.pickcardButton.Location = new System.Drawing.Point(444, 51);
            this.pickcardButton.Name = "pickcardButton";
            this.pickcardButton.Size = new System.Drawing.Size(85, 23);
            this.pickcardButton.TabIndex = 4;
            this.pickcardButton.Text = "Pick a Card";
            this.pickcardButton.UseVisualStyleBackColor = true;
            this.pickcardButton.Click += new System.EventHandler(this.pickcardButton_Click);
            // 
            // sellcardButton
            // 
            this.sellcardButton.Location = new System.Drawing.Point(444, 109);
            this.sellcardButton.Name = "sellcardButton";
            this.sellcardButton.Size = new System.Drawing.Size(85, 23);
            this.sellcardButton.TabIndex = 5;
            this.sellcardButton.Text = "Sell the card";
            this.sellcardButton.UseVisualStyleBackColor = true;
            this.sellcardButton.Click += new System.EventHandler(this.sellcardButton_Click);
            // 
            // moezolotoLabel
            // 
            this.moezolotoLabel.AutoSize = true;
            this.moezolotoLabel.Location = new System.Drawing.Point(572, 199);
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
            // playcardButton
            // 
            this.playcardButton.Location = new System.Drawing.Point(444, 80);
            this.playcardButton.Name = "playcardButton";
            this.playcardButton.Size = new System.Drawing.Size(85, 23);
            this.playcardButton.TabIndex = 9;
            this.playcardButton.Text = "Play the Card";
            this.playcardButton.UseVisualStyleBackColor = true;
            this.playcardButton.Click += new System.EventHandler(this.playcardButton_Click);
            // 
            // boardContainer
            // 
            this.boardContainer.Controls.Add(this.wonderBox);
            this.boardContainer.Location = new System.Drawing.Point(12, 344);
            this.boardContainer.Name = "boardContainer";
            this.boardContainer.Size = new System.Drawing.Size(1072, 580);
            this.boardContainer.TabIndex = 10;
            this.boardContainer.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 936);
            this.Controls.Add(this.boardContainer);
            this.Controls.Add(this.playcardButton);
            this.Controls.Add(this.headlineLabel);
            this.Controls.Add(this.moezolotoLabel);
            this.Controls.Add(this.sellcardButton);
            this.Controls.Add(this.pickcardButton);
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
        private System.Windows.Forms.Button pickcardButton;
        private System.Windows.Forms.Button sellcardButton;
        private System.Windows.Forms.Label moezolotoLabel;
        private System.Windows.Forms.Label headlineLabel;
        private System.Windows.Forms.Button playcardButton;
        private System.Windows.Forms.GroupBox boardContainer;
    }
}

