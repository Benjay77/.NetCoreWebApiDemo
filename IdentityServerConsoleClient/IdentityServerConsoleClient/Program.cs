using System;

namespace IdentityServerConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ISHelper.RequestApi().GetAwaiter().GetResult();
        }
    }
}
