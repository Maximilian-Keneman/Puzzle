namespace Puzzle
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.StartToolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GameBox = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartToolMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // StartToolMenuItem
            // 
            this.StartToolMenuItem.Name = "StartToolMenuItem";
            this.StartToolMenuItem.Size = new System.Drawing.Size(86, 20);
            this.StartToolMenuItem.Text = "Начать игру";
            this.StartToolMenuItem.Click += new System.EventHandler(this.StartToolMenuItem_Click);
            // 
            // GameBox
            // 
            this.GameBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameBox.Location = new System.Drawing.Point(0, 24);
            this.GameBox.Name = "GameBox";
            this.GameBox.Size = new System.Drawing.Size(800, 426);
            this.GameBox.TabIndex = 1;
            this.GameBox.TabStop = false;
            this.GameBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameBox_MouseMove);
            this.GameBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GameBox_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GameBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem StartToolMenuItem;
        private System.Windows.Forms.PictureBox GameBox;
    }
}

