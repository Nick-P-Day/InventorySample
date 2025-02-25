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
using Windows.UI.Xaml.Controls;

namespace Inventory.Controls
{
    public class FluidGrid : Panel
    {
        #region Columns
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(Columns), typeof(int), typeof(FluidGrid), new PropertyMetadata(2));

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        #endregion

        #region ColumnWidth
        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(nameof(ColumnWidth), typeof(double), typeof(FluidGrid), new PropertyMetadata(0.0));

        public double ColumnWidth
        {
            get => (double)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        #endregion

        #region MinColumnWidth
        public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(FluidGrid), new PropertyMetadata(180.0));

        public double MinColumnWidth
        {
            get => (double)GetValue(MinColumnWidthProperty);
            set => SetValue(MinColumnWidthProperty, value);
        }

        #endregion

        #region MaxColumnWidth
        public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.Register(nameof(MaxColumnWidth), typeof(double), typeof(FluidGrid), new PropertyMetadata(360.0));

        public double MaxColumnWidth
        {
            get => (double)GetValue(MaxColumnWidthProperty);
            set => SetValue(MaxColumnWidthProperty, value);
        }

        #endregion

        #region ColumnSpan
        public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(FluidGrid), new PropertyMetadata(1));

        public static int GetColumnSpan(UIElement element)
        {
            return (int)element.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(UIElement element, int value)
        {
            element.SetValue(ColumnSpanProperty, value);
        }

        #endregion

        #region ColumnSpacing
        public static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register(nameof(ColumnSpacing), typeof(double), typeof(FluidGrid), new PropertyMetadata(0.0));

        public double ColumnSpacing
        {
            get => (double)GetValue(ColumnSpacingProperty);
            set => SetValue(ColumnSpacingProperty, value);
        }

        #endregion

        #region RowSpacing
        public static readonly DependencyProperty RowSpacingProperty = DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(FluidGrid), new PropertyMetadata(0.0));

        public double RowSpacing
        {
            get => (double)GetValue(RowSpacingProperty);
            set => SetValue(RowSpacingProperty, value);
        }

        #endregion

        protected override Size ArrangeOverride(Size finalSize)
        {
            var countWidth = InferColumns(finalSize.Width);
            int count = countWidth.Item1;
            double width = countWidth.Item2;
            double widthspacing = width + ColumnSpacing;

            int x = 0;
            int y = 0;
            double[] rowHeights = new double[Children.Count];
            FrameworkElement[,] cells = new FrameworkElement[count, Children.Count];

            foreach (FrameworkElement item in Children)
            {
                int span = GetActualColumnSpan(item, count);
                if (x > 0 && x + span > count)
                {
                    x = 0;
                    y++;
                }
                rowHeights[y] = Math.Max(rowHeights[y], item.DesiredSize.Height);
                cells[x, y] = item;
                x += span;
            }

            double height = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                double h = 0;
                for (int j = 0; j < count; j++)
                {
                    FrameworkElement cell = cells[j, i];
                    if (cell != null)
                    {
                        int span = GetActualColumnSpan(cell, count);
                        cell.Arrange(new Rect(j * widthspacing, height, (width * span) + (ColumnSpacing * (span - 1)), rowHeights[i]));
                        h = Math.Max(h, cell.DesiredSize.Height);
                    }
                }
                height += h + RowSpacing;
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var countWidth = InferColumns(availableSize.Width);
            int count = countWidth.Item1;
            double width = countWidth.Item2;

            foreach (FrameworkElement item in Children)
            {
                int span = GetActualColumnSpan(item, count);
                item.Measure(new Size((width * span) + (ColumnSpacing * (span - 1)), availableSize.Height));
            }

            int x = 0;
            int y = 0;
            double[] rowHeights = new double[Children.Count];

            foreach (FrameworkElement item in Children)
            {
                int span = GetActualColumnSpan(item, count);
                if (x > 0 && x + span > count)
                {
                    x = 0;
                    y++;
                }
                rowHeights[y] = Math.Max(rowHeights[y], item.DesiredSize.Height);
                x += span;
            }

            double height = 0;
            for (int n = 0; n < rowHeights.Length; n++)
            {
                height += rowHeights[n];
            }
            height += RowSpacing * y;

            return new Size((count * width) + (ColumnSpacing * (count - 1)), height);
        }

        private int GetActualColumnSpan(FrameworkElement element, int count)
        {
            int span = Math.Max(1, GetColumnSpan(element));
            return Math.Min(count, span);
        }

        private Tuple<int, double> InferColumns(double availableWidth)
        {
            if (ColumnWidth > 0)
            {
                return Tuple.Create(Columns, Math.Min(ColumnWidth, MaxColumnWidth));
            }

            if (Double.IsInfinity(availableWidth))
            {
                return Tuple.Create(Columns, MaxColumnWidth);
            }

            double maxDesiredWidth = (Columns * MaxColumnWidth) + (ColumnSpacing * (Columns - 1));
            if (maxDesiredWidth <= availableWidth)
            {
                return Tuple.Create(Columns, MaxColumnWidth);
            }

            double minDesiredWidth = (Columns * MinColumnWidth) + (ColumnSpacing * (Columns - 1));
            if (minDesiredWidth <= availableWidth)
            {
                return Tuple.Create(Columns, (availableWidth - (ColumnSpacing * (Columns - 1))) / Columns);
            }

            if (MinColumnWidth >= availableWidth)
            {
                return Tuple.Create(1, availableWidth);
            }

            double width = MinColumnWidth + ColumnSpacing;
            int n = 1;
            for (; n < Columns; n++)
            {
                width += MinColumnWidth;
                if (width >= availableWidth)
                {
                    break;
                }
            }

            return Tuple.Create(n, (availableWidth - (ColumnSpacing * (n - 1))) / n);
        }
    }
}