namespace POS02_E1; 

using System;
using System.Collections.Generic;

class LambdaTest
{
    public static decimal[] Converter(decimal[] values, Func<decimal, decimal> converterFunc)
    {
        if (values == null) { return new decimal[0]; }

        decimal[] result = new decimal[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = converterFunc(values[i]);
        }
        return result;
    }

    public static void ForEach(decimal[] values, Action<decimal> action)
    {
        foreach (decimal value in values)
        {
            action(value);
        }
    }

    public static decimal ArithmeticOperation(decimal x, decimal y, Func<decimal, decimal, decimal> operation)
    {
        return operation(x, y);
    }

    public static decimal ArithmeticOperation(decimal x, decimal y, Func<decimal, decimal, decimal> operation, Action<string> logFunction)
    {
        try
        {
            return operation(x, y);
        }
        catch (Exception e)
        {
            logFunction(e.Message);
            return 0;
        }
    }

    public static void RunCommand(Action command)
    {
        command();
    }

    public static decimal[] Filter(decimal[] values, Func<decimal, bool> filterFunction)
    {
        List<decimal> result = new List<decimal>();
        foreach (decimal value in values)
        {
            if (filterFunction(value))
            {
                result.Add(value);
            }
        }
        return result.ToArray();
    }
}
