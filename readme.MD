1. Statement Test; Code Analysis & Optimization. 
    <br>Questions:
    <br>&nbsp;&nbsp;&nbsp;1. Explain the main performance issue with this code.
    <br>&nbsp;&nbsp;&nbsp;2. Write an optimized version with better time complexity 
        (without external libraries).
    <br>&nbsp;&nbsp;&nbsp;3. Describe how your version avoids duplicate pairs such as (2,3) and (3,2)
    <br>Answers:
        <br>&nbsp;&nbsp;&nbsp;1. The main performance issue with this code is there is a nested loop, where each loop is iterating over a full list that can increase the time complexity. If there is a large list it will be very slow. the reason why this code is inefficient is it checks every possible pair even after a match is found, it checks both (a,b) and (b,a) where it produces duplicates and it checks i != j that is unnecessary for a optimized solution.  
        <br>&nbsp;&nbsp;&nbsp;2.

        public static List<(int, int)> FindPairs(List<int> numbers, int target)
        {
            var result = new List<(int, int)>();

            numbers.Sort();  // O(n log n)

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
    

    <br>&nbsp;&nbsp;&nbsp;3. The way my optimized code can avoid duplicates is that first it order the list from smaller to larger this makes every valid pair appears in only one order. After sorting the list this code implement a two-pointer technique where it checks every pair from left and right inwardly so this will never match a (b,a) pattern because the (a,b) pattern is found first, this makes the result will not have duplicates.
2. Logical Test; Concurrency & Thread Safety.
    <br>Questions:
        <br>&nbsp;&nbsp;&nbsp;1. What potential problems could occur if this class is used by multiple threads
simultaneously?
        <br>&nbsp;&nbsp;&nbsp;2. Provide a corrected version that ensures thread-safety without sacrificing too
much performance. 
        <br>&nbsp;&nbsp;&nbsp;3. Explain how your approach would differ if this counter were implemented in a
distributed environment instead of a single process.
    <br>Answers:
        <br>&nbsp;&nbsp;&nbsp;1. The potential problem that could occur is there will be a race condition where it causes a lost update that maskes the result incorrect. this happens because if a multi-thread runs at a same time every thread will read the same condition and increment/decrement at the same this that that will make an unpredictable result. for example when there is 2 threads that hits increment, first thread and second there will read the counter 0 at the same time, that will result the increment just will count 1 time and the result is 1, not 2 as expected. 
        <br>&nbsp;&nbsp;&nbsp;2.

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
                return Interlocked.CompareExchange(ref _counter, 0, 0);
            }
        }

    <br>&nbsp;&nbsp;&nbsp;3.In the correct version that I approach to ensure thread-safety is using interlocked, in a single process all thread share the same memory so atomic CPU intruction guarantee thread safety. In a distributed environment this approach cannot synchronize counter from across node because every node have its own memory. In a distributed environment I would move the counter to a shared distributed system like redis using ```INCR``` or a database with atomic update because this remain atomic and consistent across servers. 
3. Data Integrity & UI Test; Transactional Consistency with Visualization. 
    <br>Clone this repo and run the command: <br>

    ```csharp
    dotnet run
    ``` 


