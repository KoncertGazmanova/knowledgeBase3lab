namespace ExpertnayaBZ
{
    partial class mainform
    {
        private System.ComponentModel.IContainer components = null;

        // Элементы формы
        private System.Windows.Forms.TreeView treeViewKnowledge;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.TextBox textBoxDetails;
        private System.Windows.Forms.Label labelQueryResult;
        private System.Windows.Forms.TextBox textBoxQueryResult;

        // Новые элементы:
        private System.Windows.Forms.ComboBox comboBoxSeason;
        private System.Windows.Forms.ComboBox comboBoxBudget;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.Label labelSeason;
        private System.Windows.Forms.Label labelBudget;

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            treeViewKnowledge = new TreeView();
            labelInfo = new Label();
            textBoxDetails = new TextBox();
            labelQueryResult = new Label();
            textBoxQueryResult = new TextBox();
            comboBoxSeason = new ComboBox();
            comboBoxBudget = new ComboBox();
            buttonQuery = new Button();
            labelSeason = new Label();
            labelBudget = new Label();
            SuspendLayout();
            // 
            // treeViewKnowledge
            // 
            treeViewKnowledge.Location = new Point(12, 12);
            treeViewKnowledge.Name = "treeViewKnowledge";
            treeViewKnowledge.Size = new Size(545, 586);
            treeViewKnowledge.TabIndex = 0;
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Location = new Point(563, 12);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(217, 15);
            labelInfo.TabIndex = 1;
            labelInfo.Text = "Информация о выбранном элементе:";
            // 
            // textBoxDetails
            // 
            textBoxDetails.Location = new Point(563, 30);
            textBoxDetails.Multiline = true;
            textBoxDetails.Name = "textBoxDetails";
            textBoxDetails.ReadOnly = true;
            textBoxDetails.Size = new Size(350, 100);
            textBoxDetails.TabIndex = 2;
            // 
            // labelQueryResult
            // 
            labelQueryResult.AutoSize = true;
            labelQueryResult.Location = new Point(563, 250);
            labelQueryResult.Name = "labelQueryResult";
            labelQueryResult.Size = new Size(110, 15);
            labelQueryResult.TabIndex = 3;
            labelQueryResult.Text = "Результат запроса:";
            // 
            // textBoxQueryResult
            // 
            textBoxQueryResult.Location = new Point(563, 270);
            textBoxQueryResult.Multiline = true;
            textBoxQueryResult.Name = "textBoxQueryResult";
            textBoxQueryResult.ReadOnly = true;
            textBoxQueryResult.ScrollBars = ScrollBars.Vertical;
            textBoxQueryResult.Size = new Size(350, 100);
            textBoxQueryResult.TabIndex = 4;
            // 
            // comboBoxSeason
            // 
            comboBoxSeason.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSeason.FormattingEnabled = true;
            comboBoxSeason.Items.AddRange(new object[] { "Лето", "Зима","Весна","Осень" });
            comboBoxSeason.Location = new Point(563, 170);
            comboBoxSeason.Name = "comboBoxSeason";
            comboBoxSeason.Size = new Size(121, 23);
            comboBoxSeason.TabIndex = 5;
            // 
            // comboBoxBudget
            // 
            comboBoxBudget.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxBudget.FormattingEnabled = true;
            comboBoxBudget.Items.AddRange(new object[] { "низкий", "средний", "высокий" });
            comboBoxBudget.Location = new Point(693, 170);
            comboBoxBudget.Name = "comboBoxBudget";
            comboBoxBudget.Size = new Size(121, 23);
            comboBoxBudget.TabIndex = 6;
            // 
            // buttonQuery
            // 
            buttonQuery.Location = new Point(823, 170);
            buttonQuery.Name = "buttonQuery";
            buttonQuery.Size = new Size(90, 23);
            buttonQuery.TabIndex = 7;
            buttonQuery.Text = "Найти варианты";
            buttonQuery.UseVisualStyleBackColor = true;
            // 
            // labelSeason
            // 
            labelSeason.AutoSize = true;
            labelSeason.Location = new Point(563, 150);
            labelSeason.Name = "labelSeason";
            labelSeason.Size = new Size(43, 15);
            labelSeason.TabIndex = 8;
            labelSeason.Text = "Сезон:";
            // 
            // labelBudget
            // 
            labelBudget.AutoSize = true;
            labelBudget.Location = new Point(693, 150);
            labelBudget.Name = "labelBudget";
            labelBudget.Size = new Size(53, 15);
            labelBudget.TabIndex = 9;
            labelBudget.Text = "Бюджет:";
            // 
            // mainform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(985, 687);
            Controls.Add(labelBudget);
            Controls.Add(labelSeason);
            Controls.Add(buttonQuery);
            Controls.Add(comboBoxBudget);
            Controls.Add(comboBoxSeason);
            Controls.Add(textBoxQueryResult);
            Controls.Add(labelQueryResult);
            Controls.Add(textBoxDetails);
            Controls.Add(labelInfo);
            Controls.Add(treeViewKnowledge);
            Name = "mainform";
            Text = "Фреймовая экспертная система: Выбор отдыха";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
