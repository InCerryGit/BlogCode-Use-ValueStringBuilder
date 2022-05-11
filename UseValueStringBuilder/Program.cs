using System.Diagnostics;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using UseValueStringBuilder;

#if DEBUG
var bench = new StringBuilderBenchmark();
var str1 = bench.StringBuilder();
var str2 = bench.ValueStringBuilderOnStack();
var str3 = bench.StringConcat();
Debug.Assert(str1 == str2, "StringBuilder != ValueStringBuilder");
Debug.Assert(str1 == str3, "StringBuilder != StringConcat");
#else
var run = BenchmarkRunner.Run<StringBuilderBenchmark>();
#endif

[MemoryDiagnoser]
[HtmlExporter]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class StringBuilderBenchmark
{
    private static readonly SomeClass Data;
    static StringBuilderBenchmark()
    {
        var baseTime = DateTime.Now;
        Data = new SomeClass
        {
            Value1 = 100, Value2 = 200, Value3 = 333,
            Value4 = 400, Value5 = string.Join('-', Enumerable.Range(0, 10000).Select(i => i.ToString())),
            Value6 = 655, Value7 = baseTime.AddHours(12),
            Value8 = TimeOnly.MinValue, Value9 = DateOnly.MaxValue,
            Value10 = Enumerable.Range(0, 5).ToArray()
        };
    }

    [Benchmark]
    public string StringConcat()
    {
        var str = "";
        var data = Data;
        str += ("Value1:"); str += (data.Value1);
        if (data.Value2 > 10)
        {
            str += " ,Value2:"; str += data.Value2;
        }
        str += " ,Value3:"; str += (data.Value3);
        str += " ,Value4:"; str += (data.Value4);
        str += " ,Value5:"; str += (data.Value5);
        if (data.Value6 > 20)
        {
            str += " ,Value6:"; str += data.Value6.ToString("F2");
        }
        str += " ,Value7:"; str += data.Value7.ToString("yyyy-MM-dd HH:mm:ss");
        str += " ,Value8:"; str += data.Value8.ToString("HH:mm:ss");
        str += " ,Value9:"; str += data.Value9.ToString("yyyy-MM-dd");
        str += " ,Value10:";
        if (data.Value10 is not null && data.Value10.Length > 0)
        {
            for (int i = 0; i < data.Value10.Length; i++)
            {
                str += (data.Value10[i]);
            }   
        }

        return str;
    }



    [Benchmark]
    public string ValueStringBuilderOnStack()
    {
        var data = Data;
        Span<char> buffer = stackalloc char[20480];
         var sb = new ValueStringBuilder(buffer);
        sb.Append("Value1:"); sb.AppendSpanFormattable(data.Value1);
        if (data.Value2 > 10)
        {
            sb.Append(" ,Value2:"); sb.AppendSpanFormattable(data.Value2);
        }
        sb.Append(" ,Value3:"); sb.AppendSpanFormattable(data.Value3);
        sb.Append(" ,Value4:"); sb.AppendSpanFormattable(data.Value4);
        sb.Append(" ,Value5:"); sb.Append(data.Value5);
        if (data.Value6 > 20)
        {
            sb.Append(" ,Value6:"); sb.AppendSpanFormattable(data.Value6, "F2");
        }
        sb.Append(" ,Value7:"); sb.AppendSpanFormattable(data.Value7, "yyyy-MM-dd HH:mm:ss");
        sb.Append(" ,Value8:"); sb.AppendSpanFormattable(data.Value8, "HH:mm:ss");
        sb.Append(" ,Value9:"); sb.AppendSpanFormattable(data.Value9, "yyyy-MM-dd");
        sb.Append(" ,Value10:");
        if (data.Value10 is not null && data.Value10.Length > 0)
        {
            for (int i = 0; i < data.Value10.Length; i++)
            {
                sb.AppendSpanFormattable(data.Value10[i]);
            }   
        }

        return sb.ToString();
    }
    
    [Benchmark]
    public string ValueStringBuilderOnHeap()
    {
        var data = Data;
        var sb = new ValueStringBuilder(20480);
        sb.Append("Value1:"); sb.AppendSpanFormattable(data.Value1);
        if (data.Value2 > 10)
        {
            sb.Append(" ,Value2:"); sb.AppendSpanFormattable(data.Value2);
        }
        sb.Append(" ,Value3:"); sb.AppendSpanFormattable(data.Value3);
        sb.Append(" ,Value4:"); sb.AppendSpanFormattable(data.Value4);
        sb.Append(" ,Value5:"); sb.Append(data.Value5);
        if (data.Value6 > 20)
        {
            sb.Append(" ,Value6:"); sb.AppendSpanFormattable(data.Value6, "F2");
        }
        sb.Append(" ,Value7:"); sb.AppendSpanFormattable(data.Value7, "yyyy-MM-dd HH:mm:ss");
        sb.Append(" ,Value8:"); sb.AppendSpanFormattable(data.Value8, "HH:mm:ss");
        sb.Append(" ,Value9:"); sb.AppendSpanFormattable(data.Value9, "yyyy-MM-dd");
        sb.Append(" ,Value10:");
        if (data.Value10 is not null && data.Value10.Length > 0)
        {
            for (int i = 0; i < data.Value10.Length; i++)
            {
                sb.AppendSpanFormattable(data.Value10[i]);
            }   
        }

        return sb.ToString();
    }
    
    [Benchmark(Baseline = true)]
    public string StringBuilder()
    {
        var data = Data;
        var sb = new StringBuilder();
        sb.Append("Value1:"); sb.Append(data.Value1);
        if (data.Value2 > 10)
        {
            sb.Append(" ,Value2:"); sb.Append(data.Value2);
        }
        sb.Append(" ,Value3:"); sb.Append(data.Value3);
        sb.Append(" ,Value4:"); sb.Append(data.Value4);
        sb.Append(" ,Value5:"); sb.Append(data.Value5);
        if (data.Value6 > 20)
        {
            sb.Append(" ,Value6:"); sb.AppendFormat("{0:F2}", data.Value6);
        }
        sb.Append(" ,Value7:"); sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss}", data.Value7);
        sb.Append(" ,Value8:"); sb.AppendFormat("{0:HH:mm:ss}", data.Value8);
        sb.Append(" ,Value9:"); sb.AppendFormat("{0:yyyy-MM-dd}", data.Value9);
        sb.Append(" ,Value10:");
        if (data.Value10 is null or {Length: 0}) return sb.ToString();
        for (int i = 0; i < data.Value10.Length; i++)
        {
            sb.Append(data.Value10[i]);
        }

        return sb.ToString();
    }    
    
    [Benchmark]
    public string StringBuilderCapacity()
    {
        var data = Data;
        var sb = new StringBuilder(20480);
        sb.Append("Value1:"); sb.Append(data.Value1);
        if (data.Value2 > 10)
        {
            sb.Append(" ,Value2:"); sb.Append(data.Value2);
        }
        sb.Append(" ,Value3:"); sb.Append(data.Value3);
        sb.Append(" ,Value4:"); sb.Append(data.Value4);
        sb.Append(" ,Value5:"); sb.Append(data.Value5);
        if (data.Value6 > 20)
        {
            sb.Append(" ,Value6:"); sb.AppendFormat("{0:F2}", data.Value6);
        }
        sb.Append(" ,Value7:"); sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss}", data.Value7);
        sb.Append(" ,Value8:"); sb.AppendFormat("{0:HH:mm:ss}", data.Value8);
        sb.Append(" ,Value9:"); sb.AppendFormat("{0:yyyy-MM-dd}", data.Value9);
        sb.Append(" ,Value10:");
        if (data.Value10 is null or {Length: 0}) return sb.ToString();
        for (int i = 0; i < data.Value10.Length; i++)
        {
            sb.Append(data.Value10[i]);
        }

        return sb.ToString();
    }    
    
    [Benchmark]
    public string StringBuilderCapacityStringInterpor()
    {
        var data = Data;
        var sb = new StringBuilder(20480);
        sb.Append("Value1:"); sb.Append(data.Value1);
        if (data.Value2 > 10)
        {
            sb.Append(" ,Value2:"); sb.Append(data.Value2);
        }
        sb.Append(" ,Value3:"); sb.Append(data.Value3);
        sb.Append(" ,Value4:"); sb.Append(data.Value4);
        sb.Append(" ,Value5:"); sb.Append(data.Value5);
        if (data.Value6 > 20)
        {
            sb.Append(" ,Value6:");
            sb.Append($"{data.Value6:F2}");
        }
        sb.Append(" ,Value7:");
        sb.Append($"{data.Value7:yyyy-MM-dd HH:mm:ss}");
        sb.Append(" ,Value8:");
        sb.Append($"{data.Value8:HH:mm:ss}");
        sb.Append(" ,Value9:");
        sb.Append($"{data.Value9:yyyy-MM-dd}");
        sb.Append(" ,Value10:");
        if (data.Value10 is null or {Length: 0}) return sb.ToString();
        for (int i = 0; i < data.Value10.Length; i++)
        {
            sb.Append(data.Value10[i]);
        }

        return sb.ToString();
    }
}

public class SomeClass
{
    public int Value1; public int Value2; public float Value3;
    public double Value4; public string? Value5; public decimal Value6;
    public DateTime Value7; public TimeOnly Value8; public DateOnly Value9;
    public int[]? Value10;
}