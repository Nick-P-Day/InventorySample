﻿#region copyright
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
using Windows.UI.Xaml.Media;

namespace Inventory
{
    public partial class ElementSet : ElementSet<FrameworkElement>
    {
    }

    public partial class ElementSet<T> : IEnumerable<T> where T : UIElement
    {
        public ElementSet()
        {
        }

        public ElementSet(IEnumerable<T> enumerable)
        {
            Enumerable = enumerable;
        }

        public IEnumerable<T> Enumerable { get; private set; }

        public static ElementSet<S> Children<S>(DependencyObject source, string category) where S : FrameworkElement
        {
            return new ElementSet<S>(GetChildren<S>(source, v => v.IsCategory(category)));
        }

        public static ElementSet<S> Children<S>(object source, Func<S, bool> predicate = null) where S : UIElement
        {
            return new ElementSet<S>(GetChildren<S>(source, predicate));
        }

        public static ElementSet<S> Children<S>(DependencyObject source, Func<S, bool> predicate = null) where S : UIElement
        {
            return new ElementSet<S>(GetChildren<S>(source, predicate));
        }

        public T FirstOrDefault()
        {
            return Enumerable.FirstOrDefault();
        }

        public S FirstOrDefault<S>() where S : FrameworkElement
        {
            foreach (var item in Enumerable)
            {
                if (item is S)
                {
                    return item as S;
                }
            }
            return default(S);
        }

        public ElementSet<T> ForEach(Action<T> action)
        {
            foreach (var item in Enumerable)
            {
                action(item);
            }
            return this;
        }

        public ElementSet<T> ForEach<S>(Action<S> action) where S : UIElement
        {
            foreach (var item in Enumerable)
            {
                S target = item as S;
                if (target != null)
                {
                    action(target);
                }
            }
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        public ElementSet<T> Reverse()
        {
            return new ElementSet<T>(Enumerable.Reverse());
        }

        private static IEnumerable<S> GetChildren<S>(object source, Func<S, bool> predicate = null) where S : UIElement
        {
            predicate = predicate ?? new Func<S, bool>((e) => true);

            if (source is UIElement element)
            {
                foreach (var item in GetChildren<S>(element, predicate))
                {
                    yield return item;
                }
                yield break;
            }

            if (source is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is S match)
                    {
                        if (predicate(match))
                        {
                            yield return match;
                        }
                    }
                    if (item is DependencyObject depObject)
                    {
                        foreach (var elem in GetChildren<S>(depObject, predicate))
                        {
                            yield return elem;
                        }
                    }
                }
            }
        }

        private static IEnumerable<S> GetChildren<S>(DependencyObject source, Func<S, bool> predicate = null) where S : UIElement
        {
            if (source != null)
            {
                predicate = predicate ?? new Func<S, bool>((e) => true);

                var count = VisualTreeHelper.GetChildrenCount(source);
                for (int n = 0; n < count; n++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(source, n);
                    if (child is S match)
                    {
                        if (predicate(match))
                        {
                            yield return match;
                        }
                    }
                    foreach (var item in GetChildren<S>(child, predicate))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}