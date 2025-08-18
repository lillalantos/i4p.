namespace i4p
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("Kódolni (1), vagy dekódolni (2) szeretne? (válasszon egy számot!)");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                Console.WriteLine("Adja meg a kódólandó üzenetet, majd a kulcsot!");

                Console.Write("Üzenet: ");
                string message = Console.ReadLine();
                Console.Write("Kulcs: ");
                string key = Console.ReadLine();

                Rejtjelezes titkositas = new Rejtjelezes(message, key);
                Console.WriteLine(titkositas.Code());
            }

            if (choice == 2) 
            {
                Console.WriteLine("Adja meg a dekódólandó üzenetet, majd a kulcsot!");

                Console.Write("Üzenet: ");
                string code = Console.ReadLine();
                Console.Write("Kulcs: ");
                string key = Console.ReadLine();

                Rejtjelezes titkositas = new Rejtjelezes(" ", key);
                Console.WriteLine(titkositas.Message(code, key));
            }

            else
            {
                Rejtjelezes titkositas = new Rejtjelezes("early bird catches the worm", "abcdefghijklmnopqrstlkmjnuzh");
                titkositas.CommonKey("ebtobehpzmjnmfqwuirlsoleakk", "cvtlsxo fiutxysspjzxkmmb",  "early ");
            }

        }
    }
}
