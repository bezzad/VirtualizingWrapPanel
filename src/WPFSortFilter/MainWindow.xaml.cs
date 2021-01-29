using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
namespace WPFSortFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string SearchTerm { get; set; }
        public ICollectionView CollectionView { get; }
        private ObservableCollection<Model> Items { get; } = new ObservableCollection<Model>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            AddItems();
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            ItemsControl.ItemsSource = CollectionView;
        }

        private void AddItems()
        {
            for (int i = 0; i < 5000; i++)
            {
                Items.Add(Model.FakeData);
            }
        }

        private void OnSearchTermChanged()
        {
            CollectionView.Filter = new Predicate<object>((item) => {
                if (string.IsNullOrWhiteSpace(SearchTerm) == false && item is Model contact)
                {
                    return contact.FirstName.StartsWith(SearchTerm, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            });
        }
    }
}
