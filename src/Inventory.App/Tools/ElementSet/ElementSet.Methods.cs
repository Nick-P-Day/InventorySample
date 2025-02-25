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

using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Inventory
{
    public partial class ElementSet<T>
    {
        public void Focus(FocusState value)
        {
            FirstOrDefault<Control>()?.Focus(value);
        }

        public ElementSet<T> Hide()
        {
            return ForEach(e => e.Visibility = Visibility.Collapsed);
        }

        public ElementSet<T> SetImplicitHideAnimation(ICompositionAnimationBase animation)
        {
            return ForEach(e => ElementCompositionPreview.SetImplicitHideAnimation(e, animation));
        }

        public ElementSet<T> SetImplicitShowAnimation(ICompositionAnimationBase animation)
        {
            return ForEach(e => ElementCompositionPreview.SetImplicitShowAnimation(e, animation));
        }

        public ElementSet<T> SetIsTranslationEnabled(bool value)
        {
            return ForEach(e => ElementCompositionPreview.SetIsTranslationEnabled(e, value));
        }

        public ElementSet<T> Show()
        {
            return ForEach(e => e.Visibility = Visibility.Visible);
        }
    }
}