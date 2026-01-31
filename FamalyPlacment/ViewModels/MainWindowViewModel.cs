
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using FamalyPlacment.Abstractions;
using FamalyPlacment.Models;
using System.Collections.ObjectModel;

namespace FamalyPlacment.ViewModels
{
    /// <summary>
    /// Модель представления для главного окна приложения размещения семейств в Revit.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IPlacementService _placementService;
        private TreeType _selectedTreeType;
        private int _count;
        private string _statusMessage;

        /// <summary>
        /// Инициализирует новый экземпляр класса MainWindowViewModel.
        /// </summary>
        /// <param name="placementService">Сервис для размещения семейств в документе Revit.</param>
        public MainWindowViewModel(IPlacementService placementService)
        {
            PlaceCommand = new RelayCommand(PlaceTree);

            TreeTypes = new ObservableCollection<TreeType>
            {
                TreeType.Oak,
                TreeType.Pine,
                TreeType.Birch
            };

            _placementService = placementService;
        }

        /// <summary>
        /// Получает коллекцию доступных типов деревьев для размещения.
        /// </summary>
        public ObservableCollection<TreeType> TreeTypes { get; }

        /// <summary>
        /// Получает или задает выбранный тип дерева для размещения.
        /// </summary>
        public TreeType SelectedTreeType
        {
            get => _selectedTreeType;
            set => SetProperty(ref _selectedTreeType, value);
        }

        /// <summary>
        /// Получает или задает количество экземпляров для размещения.
        /// </summary>
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        /// <summary>
        /// Получает или задает сообщение о статусе операции размещения.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// Получает команду для выполнения размещения деревьев.
        /// </summary>
        public RelayCommand PlaceCommand { get; }

        /// <summary>
        /// Выполняет размещение выбранного количества экземпляров выбранного типа дерева в документе Revit.
        /// </summary>
        private void PlaceTree()
        {
            CSharpFunctionalExtensions.Result result = _placementService.Place(SelectedTreeType, Count);
            if (result.IsSuccess)
            {
                StatusMessage = $"Размещено {Count} экземпляров деревьев";
                TaskDialog.Show("Размещение деревьев", StatusMessage);
            }
            else
            {
                StatusMessage = $"Ошибка {result.Error}";
                TaskDialog.Show("Размещение деревьев", result.Error);
            }
        }
    }
}
