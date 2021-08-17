using System;

namespace SimpleAssembler
{
    class Program
    {
        static void Main(string[] args)
        {            
            VirtualMachine virtualMachine = new VirtualMachine();

            try
            {
                string[] lines = System.IO.File.ReadAllLines(@"C:\Users\usman\source\SimpleAssembler\SimpleAssembler\Factorial.asm");
                virtualMachine.run(lines);

            } catch(System.IO.FileNotFoundException)
            {
                Console.WriteLine("File not found. Invalid path.");
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
