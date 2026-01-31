using CSharpFunctionalExtensions;
using FamalyPlacment.Models;

namespace FamalyPlacment.Abstractions
{
    /// <summary>
    /// Интерфейс сервиса для размещения семейств в документе Revit.
    /// </summary>
    public interface IPlacementService
    {
        /// <summary>
        /// Размещает указанное количество экземпляров семейства выбранного типа дерева в документе Revit.
        /// </summary>
        /// <param name="treeType">Тип дерева для размещения.</param>
        /// <param name="count">Количество экземпляров для размещения.</param>
        /// <returns>Результат операции размещения. Успешный результат, если размещение выполнено успешно, иначе результат с описанием ошибки.</returns>
        Result Place(TreeType treeType, int count);
    }
}
