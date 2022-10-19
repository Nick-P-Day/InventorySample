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

using System.Threading.Tasks;

namespace Inventory.Services
{
    public interface ILoginService
    {
        bool IsAuthenticated { get; set; }

        bool IsWindowsHelloEnabled(string userName);

        void Logoff();

        Task<bool> SignInWithPasswordAsync(string userName, string password);

        Task<Result> SignInWithWindowsHelloAsync();

        Task TrySetupWindowsHelloAsync(string userName);
    }
}