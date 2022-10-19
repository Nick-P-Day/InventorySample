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

using System;

namespace Inventory
{
    public interface IValidationConstraint<T>
    {
        string Message { get; }
        Func<T, bool> Validate { get; }
    }

    public class GreaterThanConstraint<T> : IValidationConstraint<T>
    {
        public GreaterThanConstraint(string propertyName, Func<T, object> propertyValue, double value)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            Value = value;
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' must be greater than {Value}.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }
        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;
        public double Value { get; set; }

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            if (value != null)
            {
                if (Double.TryParse(value.ToString(), out double d))
                {
                    return d > Value;
                }
            }
            return true;
        }
    }

    public class LessThanConstraint<T> : IValidationConstraint<T>
    {
        public LessThanConstraint(string propertyName, Func<T, object> propertyValue, double value)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            Value = value;
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' must be less than {Value}.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }
        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;
        public double Value { get; set; }

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            if (value != null)
            {
                if (Double.TryParse(value.ToString(), out double d))
                {
                    return d < Value;
                }
            }
            return true;
        }
    }

    public class NonGreaterThanConstraint<T> : IValidationConstraint<T>
    {
        public NonGreaterThanConstraint(string propertyName, Func<T, object> propertyValue, double value, string valueDesc = null)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            Value = value;
            ValueDesc = valueDesc ?? Value.ToString();
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be greater than {ValueDesc}.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }
        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;
        public double Value { get; set; }
        public string ValueDesc { get; set; }

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            if (value != null)
            {
                if (Double.TryParse(value.ToString(), out double d))
                {
                    return d <= Value;
                }
            }
            return true;
        }
    }

    public class NonZeroConstraint<T> : IValidationConstraint<T>
    {
        public NonZeroConstraint(string propertyName, Func<T, object> propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be zero.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }

        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            if (value != null)
            {
                if (Double.TryParse(value.ToString(), out double d))
                {
                    return d != 0;
                }
            }
            return true;
        }
    }

    public class PositiveConstraint<T> : IValidationConstraint<T>
    {
        public PositiveConstraint(string propertyName, Func<T, object> propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' must be positive.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }

        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            if (value != null)
            {
                if (Double.TryParse(value.ToString(), out double d))
                {
                    return d >= 0;
                }
            }
            return true;
        }
    }

    public class RequiredConstraint<T> : IValidationConstraint<T>
    {
        public RequiredConstraint(string propertyName, Func<T, object> propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be empty.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }

        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            return value != null && !String.IsNullOrEmpty(value.ToString());
        }
    }

    public class RequiredGreaterThanZeroConstraint<T> : IValidationConstraint<T>
    {
        public RequiredGreaterThanZeroConstraint(string propertyName, Func<T, object> propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        string IValidationConstraint<T>.Message => $"Property '{PropertyName}' cannot be empty.";
        public string PropertyName { get; set; }
        public Func<T, object> PropertyValue { get; set; }

        Func<T, bool> IValidationConstraint<T>.Validate => ValidateProperty;

        private bool ValidateProperty(T model)
        {
            object value = PropertyValue(model);
            if (value != null)
            {
                if (Double.TryParse(value.ToString(), out double d))
                {
                    return d > 0;
                }
            }
            return true;
        }
    }

    public class ValidationConstraint<T> : IValidationConstraint<T>
    {
        public ValidationConstraint(string message, Func<T, bool> validate)
        {
            Message = message;
            Validate = validate;
        }

        public string Message { get; set; }
        public Func<T, bool> Validate { get; set; }
    }
}