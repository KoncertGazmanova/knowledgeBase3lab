using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace ExpertnayaBZ
{
    public partial class mainform : Form
    {
        // ������ �������� ����� (��������, ����������� �� JSON)
        private Frame rootFrame;

        public mainform()
        {
            InitializeComponent();

            // ������������� �� ������� Load �����, ����� �������� ����� mainform_Load
            this.Load += mainform_Load;

            // ������������� �� ������� ������ ���� � TreeView
            treeViewKnowledge.AfterSelect += treeViewKnowledge_AfterSelect;

            // ���������� ������ "����� ��������"
            buttonQuery.Click += buttonQuery_Click;
        }

        /// <summary>
        /// ������� �������� �����: ��������� JSON-����, ������ �������� �������, ���������� � TreeView.
        /// </summary>
        private void mainform_Load(object sender, EventArgs e)
        {
            // ���� � JSON-����� (���� ����� � ����������� .exe)
            string filePath = Path.Combine(Application.StartupPath, "knowledgeBase.json");
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"���� {filePath} �� ������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // ��������� ��������� ��������� �� JSON
                rootFrame = FrameLoader.LoadFrames(filePath);

                // ������� ������ � ��������� ������� �� �������
                treeViewKnowledge.Nodes.Clear();
                PopulateTreeViewFromFrame(rootFrame, treeViewKnowledge.Nodes);

                // ���������� ��� ����, ����� ����� ������� ��� ��������
                treeViewKnowledge.ExpandAll();

                // ������: ���� ����� ������������� ��������� comboBoxSeason
                // (�������� �� ������ "�����" ��� ��������)
                var vacationFrame = FindFrameByName(rootFrame, "�����");
                if (vacationFrame != null)
                {
                    var seasonFrame = FindFrameByName(vacationFrame, "�����");
                    if (seasonFrame != null)
                    {
                        comboBoxSeason.Items.Clear();
                        foreach (var child in seasonFrame.Children)
                        {
                            comboBoxSeason.Items.Add(child.Name); // ��������� "����", "�����", "����", "�����"
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ���� ���-�� ����� �� ��� (��������, ������ ��������)
                MessageBox.Show($"������ �������� ���� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ����������� ���������� TreeView �� ������� (������� � ���������).
        /// </summary>
        private void PopulateTreeViewFromFrame(Frame frame, TreeNodeCollection nodes)
        {
            // "Root" � ����������� �����, ����� �������� ��� ������� �������� ������.
            // ������ �� ���������� "Root" � ������, � ����� ��������� ��� �����.
            if (frame.Name == "Root")
            {
                foreach (var child in frame.Children)
                {
                    var newNode = nodes.Add(child.Name);
                    PopulateChildrenRecursive(child, newNode);
                }
            }
            else
            {
                // ���� � ��� �� "Root", �� ��� ����������� �����,
                // ������� ��� ��� ���� ���� � ����� ���������� ������� �����
                var node = nodes.Add(frame.Name);
                PopulateChildrenRecursive(frame, node);
            }
        }

        /// <summary>
        /// ���������� ��������� ��� ����� � �������� ������ � TreeView-����.
        /// </summary>
        private void PopulateChildrenRecursive(Frame frame, TreeNode node)
        {
            // ������� ������� � �������� "��������" ����� ��� �����
            // (��������, [���] = "��������", [������] = "�����")
            foreach (var slot in frame.Slots)
            {
                // ���� ���� �������� ������ �����, ����������� � ������
                if (slot.Value is string[] arr)
                {
                    node.Nodes.Add($"[{slot.Key}] = {string.Join(", ", arr)}");
                }
                else
                {
                    node.Nodes.Add($"[{slot.Key}] = {slot.Value}");
                }
            }

            // ����� ���������� ������� �������� ������
            foreach (var child in frame.Children)
            {
                var childNode = node.Nodes.Add(child.Name);
                PopulateChildrenRecursive(child, childNode);
            }
        }

        /// <summary>
        /// �������: ������������ ������ ���� � TreeView.
        /// �� ������ ���������� ��� �������� � �������� �������� � textBoxDetails.
        /// </summary>
        private void treeViewKnowledge_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBoxDetails.Text = $"�� ������� ����: {e.Node.Text}";

            if (e.Node.Nodes.Count > 0)
            {
                textBoxDetails.Text += "\r\n�������� �������� ����:";
                foreach (TreeNode child in e.Node.Nodes)
                {
                    textBoxDetails.Text += $"\r\n - {child.Text}";
                }
            }
        }

        /// <summary>
        /// ������ "����� ��������": ������ �������� "������", ������� ��������� ����� � ������,
        /// � ���������� ���������� ����� ������.
        /// </summary>
        private void buttonQuery_Click(object sender, EventArgs e)
        {
            // ������� ���� ������
            textBoxQueryResult.Clear();

            // ��������� ��������� ����� � ������ �� ComboBox
            string season = comboBoxSeason.SelectedItem?.ToString();
            string budgetStr = comboBoxBudget.SelectedItem?.ToString();

            // ���������, ��� ��� ���� ���������
            if (string.IsNullOrEmpty(season) || string.IsNullOrEmpty(budgetStr))
            {
                textBoxQueryResult.Text = "�������� ����� � ������!";
                return;
            }

            // ��������� ����� (�������, ��������, ��������) � �����,
            // ����� �������� �� ���������� ����� ������
            int budgetValue = 0;
            switch (budgetStr)
            {
                case "������": budgetValue = 300; break;
                case "�������": budgetValue = 700; break;
                case "�������": budgetValue = 1500; break;
            }

            // ������� ����� "�����" (������ rootFrame)
            var vacationFrame = FindFrameByName(rootFrame, "�����");
            if (vacationFrame == null)
            {
                textBoxQueryResult.Text = "� ���� ������ ��� ������ '�����'.";
                return;
            }

            // ���� ������ "����� ������", "��������" � "�����", �.�. ��� ����� ��� ������
            var placeFrame = FindFrameByName(vacationFrame, "����� ������");
            var budgetFrame = FindFrameByName(vacationFrame, "��������");
            var seasonFrame = FindFrameByName(vacationFrame, "�����");

            if (placeFrame == null || budgetFrame == null || seasonFrame == null)
            {
                textBoxQueryResult.Text = "� '�����' �� ������� ������ ��������� (����� ������ / �������� / �����).";
                return;
            }

            // ������ �������� ����� "����", "�����", "����" ��� "�����" (� ����������� �� ������ ������������)
            var seasonChild = FindFrameByName(seasonFrame, season);
            if (seasonChild == null)
            {
                textBoxQueryResult.Text = $"�� ������� ���������� �� ������ '{season}'.";
                return;
            }

            // ������ seasonChild.Slots["Value"] ��������� ������ ����� (�������� ����, ��������� � ���� �����).
            string[] possiblePlacesBySeason = seasonChild.Slots.TryGetValue("Value", out var valArr)
                ? valArr as string[]
                : null;

            if (possiblePlacesBySeason == null || possiblePlacesBySeason.Length == 0)
            {
                textBoxQueryResult.Text = $"� ������ '{season}' �� ������� ������� �����!";
                return;
            }

            // ����� ���� �������� "�������", ����� ������ ��������� ������� ����� � �������� � ��������
            var costFrame = FindFrameByName(budgetFrame, "�������");
            if (costFrame == null)
            {
                textBoxQueryResult.Text = "�� ������� ���������� � �������� � ����� '��������'.";
                return;
            }

            var suitablePlaces = new List<string>();

            // ���������� ��� �����, ������� ������ �������� � ��������� ������
            foreach (string placeName in possiblePlacesBySeason)
            {
                // ���� ���� placeName � costFrame (����., "����" -> 500, "����" -> 1000 � �.�.)
                if (!costFrame.Slots.ContainsKey(placeName))
                {
                    // ��� ������ � ��������� ��� ������ �����
                    continue;
                }

                // ������� ���������� ��������� � int
                if (int.TryParse(costFrame.Slots[placeName].ToString(), out int placeCost))
                {
                    // ���������� c ��������� "��������" ��������
                    if (placeCost <= budgetValue)
                    {
                        suitablePlaces.Add(placeName);
                    }
                }
            }

            // ������� ���������
            if (suitablePlaces.Count == 0)
            {
                textBoxQueryResult.Text = "��� ����, ���������� �� ������� � ������.";
            }
            else
            {
                textBoxQueryResult.Text = "���������� �����:\r\n"
                    + string.Join("\r\n", suitablePlaces);
            }
        }

        /// <summary>
        /// ����������� ����� ������ ������ � ��������� ������ ����� ������� ������ � ���� ��� ��������.
        /// </summary>
        /// <param name="parent">�����, � ������� ����</param>
        /// <param name="name">������� ���</param>
        /// <returns>����� � ������ ������, ���� null, ���� �� �������</returns>
        private Frame FindFrameByName(Frame parent, string name)
        {
            // ���� ��� �������� ������ ��������� � �������, ���������� ���
            if (parent.Name == name)
                return parent;

            // ����� ���������� ���� �� ���� �������� �������
            foreach (var child in parent.Children)
            {
                var found = FindFrameByName(child, name);
                if (found != null)
                    return found;
            }

            // �� �������
            return null;
        }
    }
}
