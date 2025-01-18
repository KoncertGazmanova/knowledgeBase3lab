using System.Collections.Generic;

namespace ExpertnayaBZ
{
    /// <summary>
    /// Класс-фрейм. 
    /// Name – название фрейма, 
    /// Slots – словарь "имя_слота -> значение" (строки, числа, др. объекты),
    /// Children – вложенные фреймы, если объект внутри ещё содержит объекты/массивы.
    /// Parent – ссылка на родительский фрейм (опционально).
    /// </summary>
    public class Frame
    {
        public string Name { get; set; }
        public Dictionary<string, object> Slots { get; set; } = new Dictionary<string, object>();
        public List<Frame> Children { get; set; } = new List<Frame>();
        public Frame Parent { get; set; }
    }
}
