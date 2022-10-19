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
using Windows.UI.Xaml.Media;

namespace Inventory
{
    public partial class ElementSet<T>
    {
        public ElementSet<T> SetBackground(Brush value)
        {
            return ForEach<Control>(v => v.Background = value).ForEach<Panel>(v => v.Background = value);
        }

        public ElementSet<T> SetForeground(Brush value)
        {
            return ForEach<Control>(v => v.Foreground = value);
        }

        public ElementSet<T> SetOpacity(double value)
        {
            return ForEach(v => v.Opacity = value);
        }

        public ElementSet<T> SetVisibility(Visibility value)
        {
            return ForEach(v => v.Visibility = value);
        }
    }
}