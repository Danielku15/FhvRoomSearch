using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FhvRoomSearch.Converter
{
    public enum SimpleArithmeticsOperation
    {
        Add,
        Multiply,
        Divide,
        Subtract
    }
    public class SimpleArithmeticsValueConverter : IValueConverter
    {
        public SimpleArithmeticsOperation Operation { get; set; }
        public double Operand { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double oldValue = (double) value;
            switch (Operation)
            {
                case SimpleArithmeticsOperation.Add:
                    return oldValue + Operand;
                case SimpleArithmeticsOperation.Multiply:
                    return oldValue*Operand;
                case SimpleArithmeticsOperation.Divide:
                    return oldValue/Operand;
                case SimpleArithmeticsOperation.Subtract:
                    return oldValue - Operand;
            }
            return oldValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newValue = (double)value;
            switch (Operation)
            {
                case SimpleArithmeticsOperation.Add:
                    return newValue - Operand;
                case SimpleArithmeticsOperation.Multiply:
                    return newValue / Operand;
                case SimpleArithmeticsOperation.Divide:
                    return newValue * Operand;
                case SimpleArithmeticsOperation.Subtract:
                    return newValue + Operand;
            }
            return newValue;
        }
    }
}
