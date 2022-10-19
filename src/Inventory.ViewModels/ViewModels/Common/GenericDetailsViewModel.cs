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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    public abstract partial class GenericDetailsViewModel<TModel> : ViewModelBase where TModel : ObservableObject, new()
    {
        private TModel _editableItem = null;

        private bool _isEditMode = false;

        private bool _isEnabled = true;

        private TModel _item = null;

        public GenericDetailsViewModel(ICommonServices commonServices) : base(commonServices)
        {
        }

        public ICommand BackCommand => new RelayCommand(OnBack);
        public ICommand CancelCommand => new RelayCommand(OnCancel);
        public bool CanGoBack => !IsMainView && NavigationService.CanGoBack;
        public ICommand DeleteCommand => new RelayCommand(OnDelete);

        public TModel EditableItem
        {
            get => _editableItem;
            set => Set(ref _editableItem, value);
        }

        public ICommand EditCommand => new RelayCommand(OnEdit);
        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;

        public bool IsEditMode
        {
            get => _isEditMode;
            set => Set(ref _isEditMode, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Set(ref _isEnabled, value);
        }

        public TModel Item
        {
            get => _item;
            set
            {
                if (Set(ref _item, value))
                {
                    EditableItem = _item;
                    IsEnabled = (!_item?.IsEmpty) ?? false;
                    NotifyPropertyChanged(nameof(IsDataAvailable));
                    NotifyPropertyChanged(nameof(IsDataUnavailable));
                    NotifyPropertyChanged(nameof(Title));
                }
            }
        }

        public abstract bool ItemIsNew { get; }
        public ILookupTables LookupTables => LookupTablesProxy.Instance;
        public ICommand SaveCommand => new RelayCommand(OnSave);

        public virtual void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                TModel editableItem = new TModel();
                editableItem.Merge(Item);
                EditableItem = editableItem;
            }
        }

        public virtual void CancelEdit()
        {
            if (ItemIsNew)
            {
                // We were creating a new item: cancel means exit
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService.CloseViewAsync();
                }
                return;
            }

            // We were editing an existing item: just cancel edition
            if (IsEditMode)
            {
                EditableItem = Item;
            }
            IsEditMode = false;
        }

        public virtual async Task DeleteAsync()
        {
            TModel model = Item;
            if (model != null)
            {
                IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    MessageService.Send(this, "ItemDeleted", model);
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        public virtual async Task SaveAsync()
        {
            IsEnabled = false;
            bool isNew = ItemIsNew;
            if (await SaveItemAsync(EditableItem))
            {
                Item.Merge(EditableItem);
                Item.NotifyChanges();
                NotifyPropertyChanged(nameof(Title));
                EditableItem = Item;

                if (isNew)
                {
                    MessageService.Send(this, "NewItemSaved", Item);
                }
                else
                {
                    MessageService.Send(this, "ItemChanged", Item);
                }
                IsEditMode = false;

                NotifyPropertyChanged(nameof(ItemIsNew));
            }
            IsEnabled = true;
        }

        public virtual Result Validate(TModel model)
        {
            foreach (IValidationConstraint<TModel> constraint in GetValidationConstraints(model))
            {
                if (!constraint.Validate(model))
                {
                    return Result.Error("Validation Error", constraint.Message);
                }
            }
            return Result.Ok();
        }

        protected abstract Task<bool> ConfirmDeleteAsync();

        protected abstract Task<bool> DeleteItemAsync(TModel model);

        protected virtual IEnumerable<IValidationConstraint<TModel>> GetValidationConstraints(TModel model)
        {
            return Enumerable.Empty<IValidationConstraint<TModel>>();
        }

        protected virtual void OnBack()
        {
            StatusReady();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        protected virtual void OnCancel()
        {
            StatusReady();
            CancelEdit();
            MessageService.Send(this, "CancelEdit", Item);
        }

        protected virtual async void OnDelete()
        {
            StatusReady();
            if (await ConfirmDeleteAsync())
            {
                await DeleteAsync();
            }
        }

        protected virtual void OnEdit()
        {
            StatusReady();
            BeginEdit();
            MessageService.Send(this, "BeginEdit", Item);
        }

        protected virtual async void OnSave()
        {
            StatusReady();
            Result result = Validate(EditableItem);
            if (result.IsOk)
            {
                await SaveAsync();
            }
            else
            {
                await DialogService.ShowAsync(result.Message, $"{result.Description} Please, correct the error and try again.");
            }
        }

        protected abstract Task<bool> SaveItemAsync(TModel model);
    }
}