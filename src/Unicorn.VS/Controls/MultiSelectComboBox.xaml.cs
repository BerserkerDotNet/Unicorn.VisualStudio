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
            ((MultiSelectComboBox)d).DisplayInControl();
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof (IList<string>),
                typeof (MultiSelectComboBox), new FrameworkPropertyMetadata(null,
                    OnSelectedItemsChanged));

        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register("SelectedText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

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
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            control.SelectNodes();
            control.SetText();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;

            if (clickedBox.Content == "All")
            {
                if (clickedBox.IsChecked.Value)
                {
                    foreach (Node node in _nodeList)
                    {
                        node.IsSelected = true;
                    }
                }
                else
                {
                    foreach (Node node in _nodeList)
                    {
                        node.IsSelected = false;
                    }
                }

            }
            else
            {
                int _selectedCount = 0;
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected && s.Title != "All")
                        _selectedCount++;
                }
                if (_selectedCount == _nodeList.Count - 1)
                    _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = true;
                else
                    _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = false;
            }
            SetSelectedItems();
            SetText();

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
                if (node.IsSelected && node.Title != "All")
                {
                    if (DataSource.Count > 0)
                        SelectedItems.Add(node.Title);
                }
            }
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();
            if (this.DataSource.Any())
                _nodeList.Add(new Node("All"));
            foreach (var title in this.DataSource)
            {
                Node node = new Node(title);
                _nodeList.Add(node);
            }
            ItemsSource = _nodeList;
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected && s.Title == "All")
                    {
                        displayText = new StringBuilder();
                        displayText.Append("All");
                        break;
                    }
                    else if (s.IsSelected && s.Title != "All")
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }
                SelectedText = displayText.ToString().TrimEnd(',');
            }
            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(SelectedText))
            {
                SelectedText = "None";
            }
        }
    }
}
