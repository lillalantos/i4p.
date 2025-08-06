namespace i4p
{
    class Program
    {
        static void Main(String[] args)
        {
            Rejtjelezés titkositas = new Rejtjelezés("helloworld", "abcdefgijk");
            Console.WriteLine(titkositas.Code());
        }
    }
}
