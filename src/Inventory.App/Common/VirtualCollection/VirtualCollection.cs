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

using Inventory.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Inventory.Services
{
    public abstract partial class VirtualCollection<T> : IItemsRangeInfo, INotifyCollectionChanged
    {
        public readonly int RangeSize;

        private readonly DispatcherTimer _timer = null;

        private bool _cancel = false;

        private bool _isBusy = false;

        private object _sync = new Object();

        private IReadOnlyList<ItemIndexRange> _trackedItems = null;

        public VirtualCollection(ILogService logService, int rangeSize = 16, bool mustExploreDeepExceptions = false)
        {
            MustExploreDeepExceptions = mustExploreDeepExceptions;
            LogService = logService;

            RangeSize = rangeSize;
            Ranges = new Dictionary<int, IList<T>>();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _timer.Tick += OnTimerTick;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ILogService LogService { get; }
        public Dictionary<int, IList<T>> Ranges { get; }
        protected abstract T DefaultItem { get; }
        private bool MustExploreDeepExceptions { get; set; }

        public virtual void Dispose()
        { }

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            FetchRanges(trackedItems.Normalize().ToArray());
        }

        protected abstract Task<IList<T>> FetchDataAsync(int pageIndex, int pageSize);

        protected async void LogException(string source, string action, Exception exception)
        {
            if (MustExploreDeepExceptions == false)
            {
                await LogService.WriteAsync(LogType.Error, source, action, exception.Message, exception.ToString());
            }
            else
            {
                await LogService.WriteAsync(LogType.Error, source, action, exception);
            }
        }

        private void ClearUntrackedItems(IReadOnlyList<ItemIndexRange> trackedItems)
        {
            foreach (var rangeIndex in Ranges.Keys.ToArray())
            {
                bool isTracked = false;
                int index = rangeIndex * RangeSize;
                foreach (var trackedRange in trackedItems)
                {
                    if (trackedRange.Intersects(index, (uint)RangeSize))
                    {
                        isTracked = true;
                        break;
                    }
                }
                if (!isTracked)
                {
                    Ranges.Remove(rangeIndex);
                }
            }
        }

        private async Task FetchRange(ItemIndexRange trackedRange)
        {
            int firstIndex = trackedRange.FirstIndex / RangeSize;
            int lastIndex = trackedRange.LastIndex / RangeSize;
            for (int index = firstIndex; index <= lastIndex; index++)
            {
                if (!Ranges.ContainsKey(index))
                {
                    var items = await FetchDataAsync(index, RangeSize);
                    if (items != null)
                    {
                        Ranges[index] = items;
                        for (int n = 0; n < items.Count; n++)
                        {
                            int replaceIndex = Math.Min((index * RangeSize) + n, Count - 1);
                            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, items[n], null, replaceIndex));
                        }
                    }
                }
            }
        }

        private async void FetchRanges(IReadOnlyList<ItemIndexRange> trackedItems)
        {
            _trackedItems = trackedItems;

            _timer.Stop();
            lock (_sync)
            {
                if (_isBusy)
                {
                    _cancel = true;
                    _timer.Start();
                    return;
                }
                _cancel = false;
                _isBusy = true;
            }

            ClearUntrackedItems(trackedItems);
            await FetchRangesAsync(trackedItems);

            lock (_sync)
            {
                _isBusy = false;
            }
        }

        private async Task FetchRangesAsync(IReadOnlyList<ItemIndexRange> trackedItems)
        {
            foreach (var trackedRange in trackedItems)
            {
                await FetchRange(trackedRange);
                if (_cancel)
                {
                    return;
                }
            }
        }

        private void OnTimerTick(object sender, object e)
        {
            FetchRanges(_trackedItems);
        }
    }
}