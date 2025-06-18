using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace BarrichCSSystem.Converters;

public class ReferenceEqualConverter: IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.Count != 2 ? BindingOperations.DoNothing : ReferenceEquals(values[0], values[1]);
    }
}