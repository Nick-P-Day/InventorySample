#region copyright
// ****************************************************************** Copyright
// (c) Microsoft. All rights reserved. This code is licensed under the MIT
// License (MIT). THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO
// EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
// OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE CODE OR THE USE OR OTHER
// DEALINGS IN THE CODE. ******************************************************************
#endregion

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Inventory.Controls
{
    public sealed partial class DataList : UserControl, INotifyExpressionChanged
    {
        private static readonly DependencyExpression DataUnavailableMessageExpression = DependencyExpressions.Register(nameof(DataUnavailableMessage), nameof(ItemsSource));

        private static readonly DependencyExpressions DependencyExpressions = new DependencyExpressions();

        private static readonly DependencyExpression IsDataAvailableExpression = DependencyExpressions.Register(nameof(IsDataAvailable), nameof(ItemsSource));

        private static readonly DependencyExpression IsDataUnavailableExpression = DependencyExpressions.Register(nameof(IsDataUnavailable), nameof(IsDataAvailable));

        private static readonly DependencyExpression IsSingleSelectionExpression = DependencyExpressions.Register(nameof(IsSingleSelection), nameof(IsMultipleSelection));

        private static readonly DependencyExpression SelectionModeExpression = DependencyExpressions.Register(nameof(SelectionMode), nameof(IsMultipleSelection));

        private static readonly DependencyExpression ToolbarModeExpression = DependencyExpressions.Register(nameof(ToolbarMode), nameof(IsMultipleSelection), nameof(SelectedItemsCount));

        public DataList()
        {
            InitializeComponent();
            DependencyExpressions.Initialize(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region NewLabel
        public static readonly DependencyProperty NewLabelProperty = DependencyProperty.Register(nameof(NewLabel), typeof(string), typeof(DataList), new PropertyMetadata("New"));

        public string NewLabel
        {
            get => (string)GetValue(NewLabelProperty);
            set => SetValue(NewLabelProperty, value);
        }

        #endregion

        #region ItemsSource*
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataList), new PropertyMetadata(null, ItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataList control = d as DataList;
            control.UpdateItemsSource(e.NewValue, e.OldValue);
            DependencyExpressions.UpdateDependencies(control, nameof(ItemsSource));
        }

        private void UpdateItemsSource(object newValue, object oldValue)
        {
            if (oldValue is INotifyCollectionChanged oldSource)
            {
                oldSource.CollectionChanged -= OnCollectionChanged;
            }
            if (newValue is INotifyCollectionChanged newSource)
            {
                newSource.CollectionChanged += OnCollectionChanged;
            }
        }

        #endregion

        #region HeaderTemplate
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(DataList), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        #region ItemTemplate
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DataList), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        #endregion

        #region ItemSecondaryActionInvokedCommand
        public static readonly DependencyProperty ItemSecondaryActionInvokedCommandProperty = DependencyProperty.Register(nameof(ItemSecondaryActionInvokedCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand ItemSecondaryActionInvokedCommand
        {
            get => (ICommand)GetValue(ItemSecondaryActionInvokedCommandProperty);
            set => SetValue(ItemSecondaryActionInvokedCommandProperty, value);
        }

        #endregion

        #region DefaultCommands
        public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(DataList), new PropertyMetadata("new,select,refresh,search"));

        public string DefaultCommands
        {
            get => (string)GetValue(DefaultCommandsProperty);
            set => SetValue(DefaultCommandsProperty, value);
        }

        #endregion

        #region SelectedItem
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DataList), new PropertyMetadata(null));

        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion

        #region IsMultipleSelection*
        public static readonly DependencyProperty IsMultipleSelectionProperty = DependencyProperty.Register(nameof(IsMultipleSelection), typeof(bool), typeof(DataList), new PropertyMetadata(null, IsMultipleSelectionChanged));

        public bool IsMultipleSelection
        {
            get => (bool)GetValue(IsMultipleSelectionProperty);
            set => SetValue(IsMultipleSelectionProperty, value);
        }

        private static void IsMultipleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataList control = d as DataList;
            DependencyExpressions.UpdateDependencies(control, nameof(IsMultipleSelection));
        }

        #endregion

        #region SelectedItemsCount*
        public static readonly DependencyProperty SelectedItemsCountProperty = DependencyProperty.Register(nameof(SelectedItemsCount), typeof(int), typeof(DataList), new PropertyMetadata(null, SelectedItemsCountChanged));

        public int SelectedItemsCount
        {
            get => (int)GetValue(SelectedItemsCountProperty);
            set => SetValue(SelectedItemsCountProperty, value);
        }

        private static void SelectedItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataList control = d as DataList;
            DependencyExpressions.UpdateDependencies(control, nameof(SelectedItemsCount));
        }

        #endregion

        #region Query
        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register(nameof(Query), typeof(string), typeof(DataList), new PropertyMetadata(null));

        public string Query
        {
            get => (string)GetValue(QueryProperty);
            set => SetValue(QueryProperty, value);
        }

        #endregion

        #region ItemsCount
        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register(nameof(ItemsCount), typeof(int), typeof(DataList), new PropertyMetadata(0));

        public int ItemsCount
        {
            get => (int)GetValue(ItemsCountProperty);
            set => SetValue(ItemsCountProperty, value);
        }

        #endregion

        #region RefreshCommand
        public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand RefreshCommand
        {
            get => (ICommand)GetValue(RefreshCommandProperty);
            set => SetValue(RefreshCommandProperty, value);
        }

        #endregion

        #region QuerySubmittedCommand
        public static readonly DependencyProperty QuerySubmittedCommandProperty = DependencyProperty.Register(nameof(QuerySubmittedCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand QuerySubmittedCommand
        {
            get => (ICommand)GetValue(QuerySubmittedCommandProperty);
            set => SetValue(QuerySubmittedCommandProperty, value);
        }

        #endregion

        #region NewCommand
        public static readonly DependencyProperty NewCommandProperty = DependencyProperty.Register(nameof(NewCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand NewCommand
        {
            get => (ICommand)GetValue(NewCommandProperty);
            set => SetValue(NewCommandProperty, value);
        }

        #endregion

        #region DeleteCommand
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        #endregion

        #region StartSelectionCommand
        public static readonly DependencyProperty StartSelectionCommandProperty = DependencyProperty.Register(nameof(StartSelectionCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand StartSelectionCommand
        {
            get => (ICommand)GetValue(StartSelectionCommandProperty);
            set => SetValue(StartSelectionCommandProperty, value);
        }

        #endregion

        #region CancelSelectionCommand
        public static readonly DependencyProperty CancelSelectionCommandProperty = DependencyProperty.Register(nameof(CancelSelectionCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand CancelSelectionCommand
        {
            get => (ICommand)GetValue(CancelSelectionCommandProperty);
            set => SetValue(CancelSelectionCommandProperty, value);
        }

        #endregion

        #region SelectItemsCommand
        public static readonly DependencyProperty SelectItemsCommandProperty = DependencyProperty.Register(nameof(SelectItemsCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand SelectItemsCommand
        {
            get => (ICommand)GetValue(SelectItemsCommandProperty);
            set => SetValue(SelectItemsCommandProperty, value);
        }

        #endregion

        #region DeselectItemsCommand
        public static readonly DependencyProperty DeselectItemsCommandProperty = DependencyProperty.Register(nameof(DeselectItemsCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand DeselectItemsCommand
        {
            get => (ICommand)GetValue(DeselectItemsCommandProperty);
            set => SetValue(DeselectItemsCommandProperty, value);
        }

        #endregion

        #region SelectRangesCommand
        public static readonly DependencyProperty SelectRangesCommandProperty = DependencyProperty.Register(nameof(SelectRangesCommand), typeof(ICommand), typeof(DataList), new PropertyMetadata(null));

        public ICommand SelectRangesCommand
        {
            get => (ICommand)GetValue(SelectRangesCommandProperty);
            set => SetValue(SelectRangesCommandProperty, value);
        }

        #endregion

        public string DataUnavailableMessage => ItemsSource == null ? "Loading..." : "No items found.";
        public bool IsDataAvailable => ItemsSource?.Cast<object>().Any() ?? false;
        public bool IsDataUnavailable => !IsDataAvailable;
        public bool IsSingleSelection => !IsMultipleSelection;
        public ListViewSelectionMode SelectionMode => IsMultipleSelection ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single;
        public ListToolbarMode ToolbarMode => IsMultipleSelection ? (SelectedItemsCount > 0 ? ListToolbarMode.CancelDelete : ListToolbarMode.Cancel) : ListToolbarMode.Default;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsMultipleSelection)
            {
                if (ItemsSource is IList list)
                {
                    if (e.Action == NotifyCollectionChangedAction.Replace)
                    {
                        if (ItemsSource is ISelectionInfo selectionInfo)
                        {
                            if (selectionInfo.IsSelected(e.NewStartingIndex))
                            {
                                SelectedItem = list[e.NewStartingIndex];
                                System.Diagnostics.Debug.WriteLine("SelectedItem {0}", SelectedItem);
                            }
                        }
                    }
                }
            }
        }

        private void OnDoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (!IsMultipleSelection)
            {
                ItemSecondaryActionInvokedCommand?.TryExecute(listview.SelectedItem);
            }
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmittedCommand?.TryExecute(args.QueryText);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsMultipleSelection)
            {
                if (listview.SelectedItems != null)
                {
                    SelectedItemsCount = listview.SelectedItems.Count;
                }
                else if (listview.SelectedRanges != null)
                {
                    var ranges = listview.SelectedRanges;
                    SelectedItemsCount = ranges.IndexCount();
                    SelectRangesCommand?.TryExecute(ranges.GetIndexRanges().ToArray());
                }

                if (e.AddedItems != null)
                {
                    SelectItemsCommand?.TryExecute(e.AddedItems);
                }
                if (e.RemovedItems != null)
                {
                    DeselectItemsCommand?.TryExecute(e.RemovedItems);
                }
            }
        }

        private void OnToolbarClick(object sender, ToolbarButtonClickEventArgs e)
        {
            switch (e.ClickedButton)
            {
                case ToolbarButton.New:
                    NewCommand?.TryExecute();
                    break;

                case ToolbarButton.Delete:
                    DeleteCommand?.TryExecute();
                    break;

                case ToolbarButton.Select:
                    StartSelectionCommand?.TryExecute();
                    break;

                case ToolbarButton.Refresh:
                    RefreshCommand?.TryExecute();
                    break;

                case ToolbarButton.Cancel:
                    CancelSelectionCommand?.TryExecute();
                    break;
            }
        }

        #region NotifyPropertyChanged

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}