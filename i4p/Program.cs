namespace i4p
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("Kódolni (1), dekódolni (2), vagy közös kulcsot keresni(3) szeretne? (válasszon egy számot!)");
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

                Console.Write("Rejtjelezett üzenet: ");
                string code = Console.ReadLine();
                Console.Write("Kulcs: ");
                string key = Console.ReadLine();

                Rejtjelezes titkositas = new Rejtjelezes("alma", key);
                Console.WriteLine(titkositas.Message(code, key));
            }

            if (choice == 3) 
            {
                Console.WriteLine("Adja meg az üzenet kezdetét, majd a két rejtjelezett üzenetet");

                Console.Write("Első részlet: ");
                string MessageBlock = Console.ReadLine();
                Console.Write("Rejtjelezett üzenet 1: ");
                string Code1 = Console.ReadLine();
                Console.Write("Rejtjelezett üzenet 2: ");
                string Code2 = Console.ReadLine();


                Rejtjelezes titkositas = new Rejtjelezes(MessageBlock, Code1, Code2);
                //titkositas.KeyBlock("curios", "dvsjptjuzaljmmfeauifadbu");
                titkositas.CommonKey();
            }

            

        }
    }
}
