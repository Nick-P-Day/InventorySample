#region copyright
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

namespace Inventory
{
    public class DependencyExpressions
    {
        private ConcurrentDictionary<string, DependencyExpression> _dependencyMap = new ConcurrentDictionary<string, DependencyExpression>();

        public void Initialize(INotifyExpressionChanged source)
        {
            source.PropertyChanged += OnPropertyChanged;
        }

        public void Uninitialize(INotifyExpressionChanged source)
        {
            source.PropertyChanged -= OnPropertyChanged;
        }

        public DependencyExpression Register(string name, params string[] dependencies)
        {
            DependencyExpression dexp = new DependencyExpression(name, dependencies);
            return _dependencyMap.TryAdd(name, dexp)
                ? dexp
                : throw new ArgumentException($"DependencyExpression already registered for property '{name}'.", name);
        }

        public DependencyExpression Register(string name, params DependencyExpression[] dependencies)
        {
            DependencyExpression dexp = new DependencyExpression(name, dependencies.Select(r => r.Name).ToArray());
            return _dependencyMap.TryAdd(name, dexp)
                ? dexp
                : throw new ArgumentException($"DependencyExpression already registered for property '{name}'.", name);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is INotifyExpressionChanged source)
            {
                UpdateDependencies(source, e.PropertyName);
            }
        }

        public void UpdateDependencies(INotifyExpressionChanged source, string propertyName)
        {
            foreach (var dexp in _dependencyMap.Values)
            {
                foreach (var d in dexp.Dependencies)
                {
                    if (d == propertyName)
                    {
                        source.NotifyPropertyChanged(dexp.Name);
                    }
                }
            }
        }
    }
}
