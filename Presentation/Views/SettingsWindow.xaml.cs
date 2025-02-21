using System.Windows;
using Infrastructure.Managers;
using Presentation.ViewsModels;

namespace Presentation.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly ConfigManager _configManager;
        public MainViewModel ViewModel { get; }

        public SettingsWindow(ConfigManager configManager)
        {
            InitializeComponent();

            _configManager = configManager;

            // Инициализация ViewModel
            ViewModel = new MainViewModel();
            DataContext = ViewModel;

            LoadTemplates();
        }
        
        private void LoadTemplates()
        {
            // Очищаем текущие цепи
            ViewModel.Chains.Clear();

            // Загружаем шаблоны из ConfigManager
            foreach (var chainType in new[] { "Type1", "Type2", "Type3", "RgbStrip", "132", "141324132", "lighting", "flor", "somechain" }) // TODO: получить реальные типы
            {
                var alicePath = _configManager.GetTemplatePath(chainType, "Alice");
                var applePath = _configManager.GetTemplatePath(chainType, "Apple");

                ViewModel.Chains.Add(new ChainViewModel
                {
                    Type = chainType,
                    AlicePath = string.IsNullOrEmpty(alicePath) ? "Путь не задан" : alicePath,
                    ApplePath = string.IsNullOrEmpty(applePath) ? "Путь не задан" : applePath
                });
            }
        }
    }
}