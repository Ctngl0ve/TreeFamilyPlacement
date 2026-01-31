using FamalyPlacment.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FamalyPlacment.ViewModels
{
    /// <summary>
    /// Конвертер для преобразования значений перечисления TreeType в строковое представление и обратно.
    /// </summary>
    internal class TreeTypeConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует значение типа TreeType в его строковое представление на русском языке.
        /// </summary>
        /// <param name="value">Значение типа TreeType для преобразования.</param>
        /// <param name="targetType">Тип, в который необходимо преобразовать значение.</param>
        /// <param name="parameter">Дополнительный параметр конвертации (не используется).</param>
        /// <param name="culture">Информация о культуре для преобразования (не используется).</param>
        /// <returns>Строковое представление типа дерева на русском языке или пустая строка, если значение не распознано.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeType type)
            {
                switch (type)
                {
                    case TreeType.Oak:
                        return "Дуб";
                    case TreeType.Pine:
                        return "Сосна";
                    case TreeType.Birch:
                        return "Береза";
                    default:
                        return value != null ? value.ToString() : string.Empty;
                }
            }

            return value != null ? value.ToString() : string.Empty;
        }

        /// <summary>
        /// Преобразует строковое представление типа дерева обратно в значение перечисления TreeType.
        /// </summary>
        /// <param name="value">Строковое значение для преобразования.</param>
        /// <param name="targetType">Тип, в который необходимо преобразовать значение.</param>
        /// <param name="parameter">Дополнительный параметр конвертации (не используется).</param>
        /// <param name="culture">Информация о культуре для преобразования (не используется).</param>
        /// <returns>Значение перечисления TreeType, соответствующее строковому представлению.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если строка не соответствует ни одному из известных типов деревьев или значение не является строкой.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                switch (str)
                {
                    case "Дуб":
                        return TreeType.Oak;
                    case "Сосна":
                        return TreeType.Pine;
                    case "Береза":
                        return TreeType.Birch;
                    default:
                        throw new ArgumentException(string.Format($"Неизвестный тип дерева: {str}"));
                }
            }

            throw new ArgumentException("Некорректное значение для конвертации");
        }
    }
}
