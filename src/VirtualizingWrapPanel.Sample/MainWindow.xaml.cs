using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using VirtualizingWrapPanel.Sample;
using VirtualizingWrapPanel.Sample.Models;

namespace WPFSortFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ICollectionView CollectionView { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        public string SearchTerm { get; set; }
        public ObservableCollection<ViewModel> Items { get; } = new ObservableCollection<ViewModel>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            AddItems();
            CollectionView = CollectionViewSource.GetDefaultView(Items);
        }

        private void AddItems()
        {
            for (int i = 0; i < 50000; i++)
            {
                Items.Add(new ViewModel() { Model = BookModel.FakeData.Generate() });
            }
        }

        private void OnSearchTermChanged()
        {
            CollectionView.Filter = new Predicate<object>((item) =>
            {
                if (string.IsNullOrWhiteSpace(SearchTerm) == false && item is ViewModel vm)
                {
                    return vm.Model.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                           vm.Model.Author.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            });
        }
    }
}
