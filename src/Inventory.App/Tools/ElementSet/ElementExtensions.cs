﻿#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using Windows.UI.Xaml;

namespace Inventory
{
    public static class ElementExtensions
    {
        public static void Show(this FrameworkElement elem, bool visible = true)
        {
            elem.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static void Hide(this FrameworkElement elem)
        {
            elem.Visibility = Visibility.Collapsed;
        }

        public static bool IsCategory(this FrameworkElement elem, string category)
        {
            return elem.Tag is String tag ? tag.Split(' ').Any(s => s == category) : false;
        }

        public static bool IsCategory(this FrameworkElement elem, params string[] categories)
        {
            return elem.Tag is String tag ? tag.Split(' ').Any(s => categories.Any(c => s == c.Trim())) : false;
        }
    }
}
