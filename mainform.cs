using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace ExpertnayaBZ
{
    public partial class mainform : Form
    {
        // Храним корневой фрейм (иерархия, загруженную из JSON)
        private Frame rootFrame;

        public mainform()
        {
            InitializeComponent();

            // Подписываемся на событие Load формы, чтобы сработал метод mainform_Load
            this.Load += mainform_Load;

            // Подписываемся на событие выбора узла в TreeView
            treeViewKnowledge.AfterSelect += treeViewKnowledge_AfterSelect;

            // Обработчик кнопки "Найти варианты"
            buttonQuery.Click += buttonQuery_Click;
        }

        /// <summary>
        /// Событие загрузки формы: загружаем JSON-файл, строим иерархию фреймов, отображаем в TreeView.
        /// </summary>
        private void mainform_Load(object sender, EventArgs e)
        {
            // Путь к JSON-файлу (ищем рядом с исполняемым .exe)
            string filePath = Path.Combine(Application.StartupPath, "knowledgeBase.json");
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Файл {filePath} не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Загружаем фреймовую структуру из JSON
                rootFrame = FrameLoader.LoadFrames(filePath);

                // Очищаем дерево и наполняем данными из фреймов
                treeViewKnowledge.Nodes.Clear();
                PopulateTreeViewFromFrame(rootFrame, treeViewKnowledge.Nodes);

                // Раскрываем все узлы, чтобы сразу увидеть всю иерархию
                treeViewKnowledge.ExpandAll();

                // Пример: если хотим автоматически заполнить comboBoxSeason
                // (считывая из фрейма "Сезон" все подключи)
                var vacationFrame = FindFrameByName(rootFrame, "Отдых");
                if (vacationFrame != null)
                {
                    var seasonFrame = FindFrameByName(vacationFrame, "Сезон");
                    if (seasonFrame != null)
                    {
                        comboBoxSeason.Items.Clear();
                        foreach (var child in seasonFrame.Children)
                        {
                            comboBoxSeason.Items.Add(child.Name); // добавляем "Зима", "Весна", "Лето", "Осень"
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Если что-то пошло не так (например, ошибка парсинга)
                MessageBox.Show($"Ошибка загрузки базы знаний: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Рекурсивное построение TreeView из фреймов (начиная с корневого).
        /// </summary>
        private void PopulateTreeViewFromFrame(Frame frame, TreeNodeCollection nodes)
        {
            // "Root" — технический фрейм, чтобы вместить все объекты верхнего уровня.
            // Обычно не показываем "Root" в дереве, а сразу добавляем его детей.
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
                // Если у нас не "Root", то это полноценный фрейм,
                // добавим его как один узел и потом рекурсивно добавим детей
                var node = nodes.Add(frame.Name);
                PopulateChildrenRecursive(frame, node);
            }
        }

        /// <summary>
        /// Рекурсивно добавляем все слоты и дочерние фреймы в TreeView-узлы.
        /// </summary>
        private void PopulateChildrenRecursive(Frame frame, TreeNode node)
        {
            // Сначала добавим в качестве "листовых" узлов все слоты
            // (например, [Тип] = "песчаный", [Погода] = "тёплая")
            foreach (var slot in frame.Slots)
            {
                // Если слот содержит массив строк, преобразуем в строку
                if (slot.Value is string[] arr)
                {
                    node.Nodes.Add($"[{slot.Key}] = {string.Join(", ", arr)}");
                }
                else
                {
                    node.Nodes.Add($"[{slot.Key}] = {slot.Value}");
                }
            }

            // Затем рекурсивно добавим дочерние фреймы
            foreach (var child in frame.Children)
            {
                var childNode = node.Nodes.Add(child.Name);
                PopulateChildrenRecursive(child, childNode);
            }
        }

        /// <summary>
        /// Событие: пользователь выбрал узел в TreeView.
        /// Мы просто показываем его название и дочерние элементы в textBoxDetails.
        /// </summary>
        private void treeViewKnowledge_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBoxDetails.Text = $"Вы выбрали узел: {e.Node.Text}";

            if (e.Node.Nodes.Count > 0)
            {
                textBoxDetails.Text += "\r\nСодержит дочерние узлы:";
                foreach (TreeNode child in e.Node.Nodes)
                {
                    textBoxDetails.Text += $"\r\n - {child.Text}";
                }
            }
        }

        /// <summary>
        /// Кнопка "Найти варианты": пример простого "вывода", который учитывает сезон и бюджет,
        /// и предлагает подходящие места отдыха.
        /// </summary>
        private void buttonQuery_Click(object sender, EventArgs e)
        {
            // Очищаем поле вывода
            textBoxQueryResult.Clear();

            // Считываем выбранный сезон и бюджет из ComboBox
            string season = comboBoxSeason.SelectedItem?.ToString();
            string budgetStr = comboBoxBudget.SelectedItem?.ToString();

            // Проверяем, что оба поля заполнены
            if (string.IsNullOrEmpty(season) || string.IsNullOrEmpty(budgetStr))
            {
                textBoxQueryResult.Text = "Выберите сезон и бюджет!";
                return;
            }

            // Переводим слово («низкий», «средний», «высокий») в число,
            // чтобы сравнить со стоимостью места отдыха
            int budgetValue = 0;
            switch (budgetStr)
            {
                case "низкий": budgetValue = 300; break;
                case "средний": budgetValue = 700; break;
                case "высокий": budgetValue = 1500; break;
            }

            // Находим фрейм "Отдых" (внутри rootFrame)
            var vacationFrame = FindFrameByName(rootFrame, "Отдых");
            if (vacationFrame == null)
            {
                textBoxQueryResult.Text = "В базе знаний нет фрейма 'Отдых'.";
                return;
            }

            // Ищем фреймы "Место отдыха", "Средства" и "Сезон", т.к. они нужны для логики
            var placeFrame = FindFrameByName(vacationFrame, "Место отдыха");
            var budgetFrame = FindFrameByName(vacationFrame, "Средства");
            var seasonFrame = FindFrameByName(vacationFrame, "Сезон");

            if (placeFrame == null || budgetFrame == null || seasonFrame == null)
            {
                textBoxQueryResult.Text = "В 'Отдых' не найдены нужные подфреймы (Место отдыха / Средства / Сезон).";
                return;
            }

            // Достаём дочерний фрейм "Зима", "Весна", "Лето" или "Осень" (в зависимости от выбора пользователя)
            var seasonChild = FindFrameByName(seasonFrame, season);
            if (seasonChild == null)
            {
                textBoxQueryResult.Text = $"Не найдена информация по сезону '{season}'.";
                return;
            }

            // Внутри seasonChild.Slots["Value"] ожидается массив строк (названия мест, доступных в этот сезон).
            string[] possiblePlacesBySeason = seasonChild.Slots.TryGetValue("Value", out var valArr)
                ? valArr as string[]
                : null;

            if (possiblePlacesBySeason == null || possiblePlacesBySeason.Length == 0)
            {
                textBoxQueryResult.Text = $"В сезоне '{season}' не указаны никакие места!";
                return;
            }

            // Далее ищем подфрейм "Расходы", чтобы узнать стоимость каждого места и сравнить с бюджетом
            var costFrame = FindFrameByName(budgetFrame, "Расходы");
            if (costFrame == null)
            {
                textBoxQueryResult.Text = "Не найдена информация о расходах в блоке 'Средства'.";
                return;
            }

            var suitablePlaces = new List<string>();

            // Перебираем все места, которые вообще доступны в выбранном сезоне
            foreach (string placeName in possiblePlacesBySeason)
            {
                // Ищем слот placeName в costFrame (напр., "Пляж" -> 500, "Горы" -> 1000 и т.д.)
                if (!costFrame.Slots.ContainsKey(placeName))
                {
                    // нет данных о стоимости для такого места
                    continue;
                }

                // Пробуем распарсить стоимость в int
                if (int.TryParse(costFrame.Slots[placeName].ToString(), out int placeCost))
                {
                    // Сравниваем c выбранным "числовым" бюджетом
                    if (placeCost <= budgetValue)
                    {
                        suitablePlaces.Add(placeName);
                    }
                }
            }

            // Выводим результат
            if (suitablePlaces.Count == 0)
            {
                textBoxQueryResult.Text = "Нет мест, подходящих по бюджету и сезону.";
            }
            else
            {
                textBoxQueryResult.Text = "Подходящие места:\r\n"
                    + string.Join("\r\n", suitablePlaces);
            }
        }

        /// <summary>
        /// Рекурсивный метод поиска фрейма с указанным именем среди данного фрейма и всех его потомков.
        /// </summary>
        /// <param name="parent">Фрейм, в котором ищем</param>
        /// <param name="name">Искомое имя</param>
        /// <returns>Фрейм с нужным именем, либо null, если не найдено</returns>
        private Frame FindFrameByName(Frame parent, string name)
        {
            // Если имя текущего фрейма совпадает с искомым, возвращаем его
            if (parent.Name == name)
                return parent;

            // Иначе рекурсивно ищем во всех дочерних фреймах
            foreach (var child in parent.Children)
            {
                var found = FindFrameByName(child, name);
                if (found != null)
                    return found;
            }

            // Не найдено
            return null;
        }
    }
}
