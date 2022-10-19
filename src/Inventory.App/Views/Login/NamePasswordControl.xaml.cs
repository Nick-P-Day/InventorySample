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

namespace Inventory.Views
{
    public sealed partial class NamePasswordControl : UserControl
    {
        public NamePasswordControl()
        {
            InitializeComponent();
        }

        #region UserName
        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(NamePasswordControl), new PropertyMetadata(null));

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        #endregion

        #region Password
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(NamePasswordControl), new PropertyMetadata(null));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        #endregion

        #region LoginWithPasswordCommand
        public static readonly DependencyProperty LoginWithPasswordCommandProperty = DependencyProperty.Register(nameof(LoginWithPasswordCommand), typeof(ICommand), typeof(NamePasswordControl), new PropertyMetadata(null));

        public ICommand LoginWithPasswordCommand
        {
            get => (ICommand)GetValue(LoginWithPasswordCommandProperty);
            set => SetValue(LoginWithPasswordCommandProperty, value);
        }

        #endregion

        public void Focus()
        {
            userName.Focus(FocusState.Programmatic);
        }
    }
}