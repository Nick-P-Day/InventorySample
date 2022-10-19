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

using System.Threading.Tasks;

namespace Inventory.Services
{
    public enum DataProviderType
    {
        SQLite,
        SQLServer,
        WebAPI
    }

    public interface ISettingsService
    {
        DataProviderType DataProvider { get; set; }
        string DbVersion { get; }
        bool IsRandomErrorsEnabled { get; set; }
        string PatternConnectionString { get; }
        string SQLServerConnectionString { get; set; }
        string UserName { get; set; }
        string Version { get; }

        Task<Result> CreateDabaseAsync(string connectionString);

        Task<Result> ResetLocalDataProviderAsync();

        Task<Result> ValidateConnectionAsync(string connectionString);
    }
}