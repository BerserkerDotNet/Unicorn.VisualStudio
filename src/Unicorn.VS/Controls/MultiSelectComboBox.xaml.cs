using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Unicorn.VS.Controls
{
    public partial class MultiSelectComboBox
    {
        private const string AllItems = "All";
        private readonly ObservableCollection<Node> _nodeList;
        public MultiSelectComboBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof (IList<string>), typeof (MultiSelectComboBox),
                new FrameworkPropertyMetadata(null,
                    MultiSelectComboBox.OnDataSourceChanged));

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiSelectComboBox = ((MultiSelectComboBox)d);
            multiSelectComboBox.DisplayInControl();
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof (IList<string>),
                typeof (MultiSelectComboBox), new FrameworkPropertyMetadata(null,
                    OnSelectedItemsChanged));

        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register("SelectedText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(AllItems));

        public IList<string> DataSource
        {
            get { return (IList<string>)GetValue(DataSourceProperty); }
            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

        public IList<string> SelectedItems
        {
            get { return (IList<string>)GetValue(SelectedItemsProperty); }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiSelectComboBox)d;
            control.SelectNodes();
            control.SetText();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (SelectedIndex >= 0 && _nodeList.Count >= SelectedIndex + 1)
            {
                _nodeList[SelectedIndex].IsSelected = !_nodeList[SelectedIndex].IsSelected;
                if (SelectedIndex == 0)
                    ChangeItemsState(true);
                else //Skip, if selecting first item, First should be "All"
                    IndicateThatAllSelected();
                SetSelectedItems();
                SetText();
            }
            
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var clickedBox = (CheckBox)sender;
            if (clickedBox.Content != null && clickedBox.Content.Equals(AllItems))
            {
                var state = clickedBox.IsChecked != null && clickedBox.IsChecked.Value;
                ChangeItemsState(state);
            }
            else
            {
                IndicateThatAllSelected();
            }
            SetSelectedItems();
            SetText();

        }

        private void ChangeItemsState(bool state)
        {
            foreach (Node node in _nodeList)
            {
                node.IsSelected = state;
            }
        }

        private void IndicateThatAllSelected()
        {
            var selectedCount = _nodeList.Count(s => s.IsSelected && s.Title != AllItems);
            var node = _nodeList.FirstOrDefault(i => i.Title == AllItems);
            if (node != null)
                node.IsSelected = selectedCount == _nodeList.Count - 1;
        }

        private void SelectNodes()
        {
            if (SelectedItems == null)
                return;

            foreach (var item in SelectedItems)
            {
                var node = _nodeList.FirstOrDefault(i => i.Title == item);
                if (node != null)
                    node.IsSelected = true;
            }
        }

        private void SetSelectedItems()
        {
            if (SelectedItems == null)
                SelectedItems = new List<string>();
            SelectedItems.Clear();
            foreach (Node node in _nodeList)
            {
                if (node.IsSelected && node.Title != AllItems)
                {
                    if (DataSource.Count > 0)
                        SelectedItems.Add(node.Title);
                }
            }
        }

        private void DisplayInControl()
        {
            if(DataSource is ObservableCollection<string>)
                ((ObservableCollection<string>)DataSource).CollectionChanged += MultiSelectComboBox_CollectionChanged;

            SyncItems();
            ItemsSource = _nodeList;
        }

        private void SyncItems()
        {
            _nodeList.Clear();
            foreach (var title in this.DataSource)
            {
                var node = new Node(title);
                _nodeList.Add(node);
            }
        }

        private void MultiSelectComboBox_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SyncItems();
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                var displayText = new StringBuilder();
                foreach (var s in _nodeList)
                {
                    if (s.IsSelected && s.Title == AllItems)
                    {
                        displayText = new StringBuilder();
                        displayText.Append(AllItems);
                        break;
                    }
                    if (s.IsSelected && s.Title != AllItems)
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }
                SelectedText = displayText.ToString().TrimEnd(',');
            }
            if (string.IsNullOrEmpty(SelectedText))
                SelectedText = AllItems;
        }
    }
}
