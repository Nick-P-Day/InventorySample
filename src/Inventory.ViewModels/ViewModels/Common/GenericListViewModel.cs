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

using Inventory.Models;
using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    public abstract partial class GenericListViewModel<TModel> : ViewModelBase where TModel : ObservableObject
    {
        private bool _isMultipleSelection = false;

        private IList<TModel> _items = null;

        private int _itemsCount = 0;

        private string _query = null;

        private TModel _selectedItem = default(TModel);

        private ListToolbarMode _toolbarMode = ListToolbarMode.Default;

        public GenericListViewModel(ICommonServices commonServices) : base(commonServices)
        {
        }

        public ICommand CancelSelectionCommand => new RelayCommand(OnCancelSelection);
        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);
        public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);

        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => Set(ref _isMultipleSelection, value);
        }

        public IList<TModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public int ItemsCount
        {
            get => _itemsCount;
            set => Set(ref _itemsCount, value);
        }

        public ILookupTables LookupTables => LookupTablesProxy.Instance;

        public ICommand NewCommand => new RelayCommand(OnNew);

        public string Query
        {
            get => _query;
            set => Set(ref _query, value);
        }

        public ICommand RefreshCommand => new RelayCommand(OnRefresh);
        public IndexRange[] SelectedIndexRanges { get; protected set; }

        public TModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Set(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        MessageService.Send(this, "ItemSelected", _selectedItem);
                    }
                }
            }
        }

        public List<TModel> SelectedItems { get; protected set; }
        public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
        public ICommand SelectRangesCommand => new RelayCommand<IndexRange[]>(OnSelectRanges);
        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        public override string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        public ListToolbarMode ToolbarMode
        {
            get => _toolbarMode;
            set => Set(ref _toolbarMode, value);
        }

        protected virtual void OnCancelSelection()
        {
            StatusReady();
            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }

        protected abstract void OnDeleteSelection();

        protected virtual void OnDeselectItems(IList<object> items)
        {
            if (items?.Count > 0)
            {
                StatusReady();
            }
            if (IsMultipleSelection)
            {
                foreach (TModel item in items)
                {
                    SelectedItems.Remove(item);
                }
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        protected abstract void OnNew();

        protected abstract void OnRefresh();

        protected virtual void OnSelectItems(IList<object> items)
        {
            StatusReady();
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<TModel>());
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        protected virtual void OnSelectRanges(IndexRange[] indexRanges)
        {
            SelectedIndexRanges = indexRanges;
            int count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
            StatusMessage($"{count} items selected");
        }

        protected virtual void OnStartSelection()
        {
            StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<TModel>();
            SelectedIndexRanges = null;
            IsMultipleSelection = true;
        }
    }
}