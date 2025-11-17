namespace RimLangKit.Processors
{
    /// <summary>
    /// Состояние для TagCollector - замена статических коллекций
    /// </summary>
    public class TagCollectorData
    {
        /// <summary>
        /// Уникальные Tags
        /// </summary>
        public List<string> Tags { get; } = new();

        /// <summary>
        /// Уникальные Defs
        /// </summary>
        public List<string> Defs { get; } = new();

        /// <summary>
        /// Список Tags для каждого Defs
        /// </summary>
        public Dictionary<string, List<string>> TagList { get; } = new();

        /// <summary>
        /// Распространенность Tags для текущего перевода
        /// </summary>
        public Dictionary<string, int> TagSpread { get; } = new();

        /// <summary>
        /// Распространенность Defs для текущего перевода
        /// </summary>
        public Dictionary<string, int> DefsSpread { get; } = new();

        /// <summary>
        /// Очистка всех данных
        /// </summary>
        public void Clear()
        {
            Tags.Clear();
            Defs.Clear();
            TagList.Clear();
            TagSpread.Clear();
            DefsSpread.Clear();
        }
    }
}
