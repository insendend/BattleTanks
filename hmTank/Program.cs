using System;
using hmTank.Classes;

namespace hmTank
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Game g = new Game();
                g.Run();             
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}