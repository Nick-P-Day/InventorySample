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

using Inventory.Services;
using System;
using System.Threading.Tasks;

namespace Inventory.ViewModels
{
    public class ShellArgs
    {
        public object Parameter { get; set; }
        public UserInfo UserInfo { get; set; }
        public Type ViewModel { get; set; }
    }

    public class ShellViewModel : ViewModelBase
    {
        private bool _isEnabled = true;

        private bool _isError = false;

        private bool _isLocked = false;

        private string _message = "Ready";

        public ShellViewModel(ILoginService loginService, ICommonServices commonServices) : base(commonServices)
        {
            IsLocked = !loginService.IsAuthenticated;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Set(ref _isEnabled, value);
        }

        public bool IsError
        {
            get => _isError;
            set => Set(ref _isError, value);
        }

        public bool IsLocked
        {
            get => _isLocked;
            set => Set(ref _isLocked, value);
        }

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public UserInfo UserInfo { get; protected set; }

        public ShellArgs ViewModelArgs { get; protected set; }

        public virtual Task LoadAsync(ShellArgs args)
        {
            ViewModelArgs = args;
            if (ViewModelArgs != null)
            {
                UserInfo = ViewModelArgs.UserInfo;
                NavigationService.Navigate(ViewModelArgs.ViewModel, ViewModelArgs.Parameter);
            }
            return Task.CompletedTask;
        }

        public virtual void Subscribe()
        {
            MessageService.Subscribe<ILoginService, bool>(this, OnLoginMessage);
            MessageService.Subscribe<ViewModelBase, string>(this, OnMessage);
        }

        public virtual void Unload()
        {
        }

        public virtual void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        private async void OnLoginMessage(ILoginService loginService, string message, bool isAuthenticated)
        {
            if (message == "AuthenticationChanged")
            {
                await ContextService.RunAsync(() =>
                {
                    IsLocked = !isAuthenticated;
                });
            }
        }

        private async void OnMessage(ViewModelBase viewModel, string message, string status)
        {
            switch (message)
            {
                case "StatusMessage":
                case "StatusError":
                    if (viewModel.ContextService.ContextID == ContextService.ContextID)
                    {
                        IsError = message == "StatusError";
                        SetStatus(status);
                    }
                    break;

                case "EnableThisView":
                case "DisableThisView":
                    if (viewModel.ContextService.ContextID == ContextService.ContextID)
                    {
                        IsEnabled = message == "EnableThisView";
                        SetStatus(status);
                    }
                    break;

                case "EnableOtherViews":
                case "DisableOtherViews":
                    if (viewModel.ContextService.ContextID != ContextService.ContextID)
                    {
                        await ContextService.RunAsync(() =>
                        {
                            IsEnabled = message == "EnableOtherViews";
                            SetStatus(status);
                        });
                    }
                    break;

                case "EnableAllViews":
                case "DisableAllViews":
                    await ContextService.RunAsync(() =>
                    {
                        IsEnabled = message == "EnableAllViews";
                        SetStatus(status);
                    });
                    break;
            }
        }

        private void SetStatus(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            Message = message;
        }
    }
}