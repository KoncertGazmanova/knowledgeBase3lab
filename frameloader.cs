using Newtonsoft.Json.Linq;
using System.IO;

namespace ExpertnayaBZ
{
    /// <summary>
    /// Класс для загрузки JSON-файла в иерархию фреймов.
    /// Предполагается, что в JSON могут быть объекты, массивы и простые значения.
    /// Каждый ключ (property) в JSON становится "именем" фрейма или слота,
    /// а структура вкладывается рекурсивно.
    /// </summary>
    public static class FrameLoader
    {
        /// <summary>
        /// Загрузить иерархию фреймов из указанного JSON-файла.
        /// </summary>
        /// <param name="filePath">Путь к JSON-файлу</param>
        /// <returns>Корневой фрейм (Name = "Root"), содержащий все загруженные данные как детей</returns>
        public static Frame LoadFrames(string filePath)
        {
            // Считываем JSON-файл в строку
            var json = File.ReadAllText(filePath);

            // Парсим через Newtonsoft.Json.Linq, получаем JObject (корневой объект)
            var jObj = JObject.Parse(json);

            // Создаем "корневой" фрейм, куда поместим все объекты верхнего уровня
            // (например, у вас в JSON сверху есть "Отдых": {...}, значит "Отдых" станет ребенком Root)
            var rootFrame = new Frame { Name = "Root" };

            // Перебираем все свойства верхнего уровня (например, property.Name = "Отдых")
            foreach (var property in jObj.Properties())
            {
                // Рекурсивно создаём фрейм из этого JToken (value может быть объектом, массивом или строкой)
                var childFrame = CreateFrameFromJToken(property.Name, property.Value, rootFrame);

                // Добавляем полученный фрейм к списку детей корневого фрейма
                rootFrame.Children.Add(childFrame);
            }

            return rootFrame;
        }

        /// <summary>
        /// Рекурсивно превращаем JToken (объект, массив или значение) в фрейм со слотами и дочерними фреймами.
        /// </summary>
        /// <param name="name">Имя текущего узла (ключ JSON-свойства)</param>
        /// <param name="token">JToken (JObject, JArray или простое значение)</param>
        /// <param name="parent">Родительский фрейм (в который вкладывается этот)</param>
        /// <returns>Новый фрейм</returns>
        private static Frame CreateFrameFromJToken(string name, JToken token, Frame parent)
        {
            // Создаем фрейм с заданным именем
            var frame = new Frame
            {
                Name = name,
                Parent = parent
            };

            // Проверяем тип JToken — объект, массив, либо простое значение
            if (token.Type == JTokenType.Object)
            {
                // Если это JObject, значит внутри есть различные свойства
                var jObject = (JObject)token;

                // Перебираем все вложенные свойства
                foreach (var prop in jObject.Properties())
                {
                    // Если prop.Value тоже является объектом или массивом, нужно создать дочерний фрейм
                    if (prop.Value.Type == JTokenType.Object || prop.Value.Type == JTokenType.Array)
                    {
                        // Рекурсивно строим дочерний фрейм
                        var child = CreateFrameFromJToken(prop.Name, prop.Value, frame);
                        frame.Children.Add(child);
                    }
                    else
                    {
                        // Иначе это простое значение (строка, число и т.п.), считаем его "слотом"
                        frame.Slots[prop.Name] = prop.Value.ToString();
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                // Если это массив (JArray), сохраним его как один слот.
                // Здесь упрощённо предполагаем, что массив состоит из строк (string[]).
                var arr = (JArray)token;
                frame.Slots["Value"] = arr.ToObject<string[]>();
            }
            else
            {
                // Любой другой простой тип (число, строка, boolean и т.д.) — тоже слот (с именем "Value")
                frame.Slots["Value"] = token.ToString();
            }

            return frame;
        }
    }
}
