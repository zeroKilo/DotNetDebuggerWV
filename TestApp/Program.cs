using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestException();
        }

        static void TestException()
        {
            throw new Exception();
        }
    }
}
