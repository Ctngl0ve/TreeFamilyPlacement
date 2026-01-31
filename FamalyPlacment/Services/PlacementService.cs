using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using FamalyPlacment.Abstractions;
using FamalyPlacment.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FamalyPlacment.Services
{
    /// <summary>
    /// Сервис для размещения семейств деревьев в документе Revit.
    /// </summary>
    public class PlacementService : IPlacementService
    {
        private readonly Document _document;

        /// <summary>
        /// Инициализирует новый экземпляр класса PlacementService с указанным документом Revit.
        /// </summary>
        /// <param name="document">Документ Revit, в котором будет выполняться размещение семейств.</param>
        public PlacementService(Document document)
        {
            _document = document;
        }

        /// <summary>
        /// Размещает указанное количество экземпляров семейства выбранного типа дерева в документе Revit.
        /// Экземпляры размещаются в сетке с шагом 2 метра на первом доступном уровне.
        /// </summary>
        /// <param name="treeType">Тип дерева для размещения.</param>
        /// <param name="count">Количество экземпляров для размещения. Должно быть больше нуля.</param>
        /// <returns>Результат операции размещения. Успешный результат, если размещение выполнено успешно, иначе результат с описанием ошибки.</returns>
        public Result Place(TreeType treeType, int count)
        {
            return Validate(count)
                .Bind(() => FindFamily(treeType))
                .Bind(s => PlaceInstances(s, count));
        }

        private Result Validate(int count)
        {
            if (count < 0)
                return Result.Failure("Количество должно быть больше 0");
            return Result.Success();
        }

        private Result<FamilySymbol> FindFamily(TreeType treeType)
        {
            string treeName = string.Empty;
            switch (treeType)
            {
                case TreeType.Oak:
                    treeName = "Дуб";
                    break;
                case TreeType.Pine:
                    treeName = "Сосна";
                    break;
                case TreeType.Birch:
                    treeName = "Береза";
                    break;
            }

            FamilySymbol familySymbol = new FilteredElementCollector(_document)
                .OfCategory(BuiltInCategory.OST_Planting)
                .OfClass(typeof(FamilySymbol))
                .OfType<FamilySymbol>()
                .Where(x => x.FamilyName.Contains(treeName))
                .FirstOrDefault();

            if (familySymbol == null)
                return Result.Failure<FamilySymbol>("Не найден типоразмер для размещения");

            return familySymbol;
        }

        private Result PlaceInstances(FamilySymbol familySymbol, int count)
        {
            try
            {
                double step = UnitUtils.ConvertToInternalUnits(2, UnitTypeId.Meters);
                
                int columns = (int)Math.Ceiling(Math.Sqrt(count));
                int rows = (int)Math.Ceiling((double)count / columns);
                
                var points = new List<XYZ>();
                for (int i = 0; i < count; i++)
                {
                    int row = i / columns;
                    int column = i % columns;
                    points.Add(new XYZ(column * step, row * step, 0));
                }

                var level = new FilteredElementCollector(_document)
                    .OfClass(typeof(Level))
                    .OfType<Level>()
                    .OrderBy(l => l.Elevation)
                    .FirstOrDefault();

                if (level == null)
                    return Result.Failure("Не удалось определить уровень для размещения");

                using (Transaction transaction = new Transaction(_document, "Размещение деревьев"))
                {
                    transaction.Start();

                    if (!familySymbol.IsActive)
                    {
                        familySymbol.Activate();
                    }

                    foreach (var point in points)
                    {
                        _document.Create.NewFamilyInstance(
                            point,
                            familySymbol,
                            level,
                            Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    }
                    transaction.Commit();
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
