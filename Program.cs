using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class TestCase
{
    public List<int> Numbers {get; set;}
    public int Target {get; set;}
}

public class CounterService
{
    private int _counter = 0;

    public void Increment()
    {
        Interlocked.Increment(ref _counter);
    }

    public void Decrement()
    {
        Interlocked.Decrement(ref _counter);
    }

    public int GetValue()
    {
        // Atomic read
        return Interlocked.CompareExchange(ref _counter, 0, 0);
    }
}

public class Invoice
{
    public string InvoiceID { get; set; }
    public decimal TotalAmount { get; set; }
}

public class InvoiceDetail
{
    public string InvoiceID { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("===============================================");
        Console.WriteLine("1.Statement Test; Code Analysis & Optimization.");
        Console.WriteLine("===============================================");
        var testCases = new List<TestCase>
        {
            new TestCase
            {
                Numbers = new List<int> { 1,2,3,4,5},
                Target = 5
            },
            new TestCase
            {
                Numbers = new List<int> {2,2,3,3},
                Target = 5
            }
        };

        foreach (var tc in testCases)
        {
            var pairs = FindPairs(tc.Numbers, tc.Target);
            string formattedPairs =
                "[" + string.Join(", ", pairs.ConvertAll(p => $"({p.Item1},{p.Item2})")) + "]";

            Console.WriteLine($"Input: [{string.Join(",", tc.Numbers)}] | Target: {tc.Target} | Output: {formattedPairs}");
        }
        Console.WriteLine();

        Console.WriteLine("===============================================");
        Console.WriteLine("2. Logical Test; Concurrency & Thread Safety");
        Console.WriteLine("===============================================");

        CounterService counter = new CounterService();

        int threadCount = 100;
        Thread[] threads = new Thread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                counter.Increment();
            });
        }

        // Start all threads
        foreach (var t in threads) t.Start();

        // Wait for all to finish
        foreach (var t in threads) t.Join();

        Console.WriteLine("Final Counter = " + counter.GetValue());
        Console.WriteLine();

        Console.WriteLine("===============================================");
        Console.WriteLine("3. Data Integrity & UI Test; Transactional Consistency with Visualization.");
        Console.WriteLine("===============================================");
        
        RunInvoiceCheckerUI();
        Console.WriteLine("Invoice checker UI generated → opening browser...");
        Console.WriteLine();

    }

    public static List<(int, int)> FindPairs(List<int> numbers, int target)
    {
        var result = new List<(int, int)>();

        numbers.Sort();

        int left = 0;
        int right = numbers.Count - 1;

        while (left < right)
        {
            int sum = numbers[left] + numbers[right];

            if (sum == target)
            {
                result.Add((numbers[left], numbers[right]));

                int leftValue = numbers[left];
                int rightValue = numbers[right]; 

                while (left < right && numbers[left] == leftValue)
                    left++;
                while (left < right && numbers[right] == rightValue)    
                    right--;
            }
            else if (sum < target)
            {
                left++;
            }
            else
            {
                right--;
            }
        }

        return result;
    }

    public static void RunInvoiceCheckerUI()
    {
        var invoices = new List<Invoice>
        {
            new Invoice { InvoiceID = "INV001", TotalAmount = 250000 },
            new Invoice { InvoiceID = "INV002", TotalAmount = 90000 },
            new Invoice { InvoiceID = "INV003", TotalAmount = 180000 },
            new Invoice { InvoiceID = "INV004", TotalAmount = 200000 }
        };

        var details = new List<InvoiceDetail>
        {
            new InvoiceDetail { InvoiceID = "INV001", Qty = 2, Price = 125000 },
            new InvoiceDetail { InvoiceID = "INV002", Qty = 1, Price = 90000 },
            new InvoiceDetail { InvoiceID = "INV003", Qty = 2, Price = 90000 },
            new InvoiceDetail { InvoiceID = "INV004", Qty = 2, Price = 110000 } // mismatch
        };

        var rows_invoice = new List<string>();
        var rows_detail = new List<string>();
        var rows_checker = new List<string>();

        foreach (var invoice in invoices)
        {
            decimal calculated = 0;
            foreach (var d in details)
                if (d.InvoiceID == invoice.InvoiceID)
                    calculated += d.Qty * d.Price;

            bool isValid = calculated == invoice.TotalAmount;
            string color = isValid ? "#d4ffd4" : "#ffd4d4";
            string status = isValid ? "✔ Valid" : "❌ Mismatch";

            rows_invoice.Add(
                $"<tr>" +
                $"<td>{invoice.InvoiceID}</td>" +
                $"<td>{invoice.TotalAmount}</td>" +
                "</tr>"
            );
            rows_checker.Add(
                $"<tr style='background:{color};'>" +
                $"<td>{invoice.InvoiceID}</td>" +
                $"<td>{invoice.TotalAmount}</td>" +
                $"<td>{calculated}</td>" +
                $"<td>{status}</td>" +
                "</tr>"
            );
        }

        foreach (var detail in details)
        {
            rows_detail.Add(
                    $"<tr'>" +
                    $"<td>{detail.InvoiceID}</td>" +
                    $"<td>{detail.Qty}</td>" +
                    $"<td>{detail.Price}</td>" +
                    "</tr>"
                );
        }

        string html =
        $@"
        <html>
        <head>
            <title>Invoice Integrity Checker</title>
            <style>
                body {{ 
                        font-family: Arial; 
                        padding: 20px; 
                    }}
                .table-wrapper {{ 
                        display: flex; 
                        gap: 30px;
                    }}
                table {{ 
                        border-collapse: collapse; 
                        width: 100%; 
                    }}
                th, td {{ 
                        border: 1px solid #999; 
                        padding: 8px; 
                        text-align: left; 
                    }}
                th {{ 
                        background: #333; 
                        color: white; 
                    }}
                .box {{
                    width: 50%;
                }}
            </style>
        </head>

        <body>
            <div class='table-wrapper'>
                <div class='box'>
                    <h2>Invoices</h2>
                    <table>
                        <tr>
                            <th>InvoiceID</th>
                            <th>TotalAmount</th>
                        </tr>
                        {string.Join("", rows_invoice)}
                    </table>
                </div>
            
                <div class=box>
                    <h2>Invoice Details</h2>
                    <table>
                        <tr>
                            <th>InvoiceID</th>
                            <th>Qty</th>
                            <th>Price</th>
                        </tr>
                        {string.Join("", rows_detail)}
                    </table>
                </div>
            </div>
            
            <h2>Invoice Integrity Checker</h2>
            <table>
                <tr>
                    <th>InvoiceID</th>
                    <th>System Total</th>
                    <th>Calculated Total</th>
                    <th>Status</th>
                </tr>
                {string.Join("", rows_checker)}
            </table>
        </body>
        </html>
        ";

        File.WriteAllText("invoice_report.html", html);
        Process.Start(new ProcessStartInfo("invoice_report.html") { UseShellExecute = true });
    }
}
