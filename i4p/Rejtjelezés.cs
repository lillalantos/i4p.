using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace i4p
{
    internal class Rejtjelezés
    {
        //Az 'a' kódjától 26 elem képzése, karakterré alakítása, és tömbbé formázása szóközöstül
        char[] abc = Enumerable.Range('a', 26).Select(x => (char)x).Append(' ').ToArray();

        //Adattagok és tulajdonságaik
        string message;
        string key;
        
        public Rejtjelezés(string message, string key)
        {
            //csak akkor állítsa be, ha a megfelelő karakterekkel rendelkezik és nem null értékű

            int MessageIdx = 0;
            while (MessageIdx < message.Length && abc.Contains(message[MessageIdx]))
                MessageIdx++;

            if (MessageIdx < message.Length)
                Console.WriteLine("Nem csak az angol abc vagy ' ' karaktereket használtad az üzenetedben!");
            else
                this.message = message;



            //megadott kulcs karaktereinek és hosszának vizsgálata
            if (key.Length < message.Length)
                Console.WriteLine("A kulcsnak legalább olyan hosszúnak kell legyen, mint az üzenetnek!");

            int KeyIdx = 0;

            while (KeyIdx < key.Length && abc.Contains(key[KeyIdx]))
                KeyIdx++;
            if (KeyIdx < key.Length)
                Console.WriteLine("Nem csak az angol abc vagy ' ' karaktereket használtad a kulcsodban!");
            
            else this.key = key;


        }

        //Metódus a rejtjelezésre
        public string Code()
        {
            //Nullvizsgálat
            if (message != null && key!=null)
            {
                string code = "";
                for (int i = 0; i < message.Length; i++)
                {
                    int temp = Array.IndexOf(abc, message[i]) + Array.IndexOf(abc, key[i]);

                    if (temp < 27)
                        code += abc[temp];
                    else
                    {
                        int remainder = temp % 27;
                        code += abc[remainder];
                    }
                }
                return $"Rejtjelezett üzenet: code";
            }
            
            //Ha az üzenet vagy kulcs hibás formátumú
            else if (message==null)
                return "A rejtjelezett üzenet nem készíthető el egy megfelelő formátumú üzenet nélkül!";

            return "A rejtjelezett üzenet nem készíthető el egy megfelelő formátumú kulcs nélkül!";
        }

        public string Message(string Code, string Key)
        {
            //Rejtjelezett üzenet karaktereinek vizsgálata
            int CodeIndex = 0;

            while (CodeIndex < key.Length && abc.Contains(key[CodeIndex]))
                CodeIndex++;
            if (CodeIndex < key.Length)
                Console.WriteLine("Nem csak az angol abc vagy ' ' karaktereket használtad a rejtjelezett üzenetedben!");

            if (key!=null)
            {
                string messageCode = "";
                for (int i = 0; i < Code.Length; i++)
                {
                    int temp = Array.IndexOf(abc, Code[i]) - Array.IndexOf(abc, key[i]);

                    if (temp >=0)
                        messageCode += abc[temp];
                    else
                    {
                        int remainder = (27 + Array.IndexOf(abc, Code[i])) - Array.IndexOf(abc, key[i]);
                        messageCode += abc[remainder];
                    }
                }
                return $"Eredeti Üzenet: {messageCode}";
            }
            return "Az üzenet dekódolása nem készíthető el egy megfelelő formátumú kulcs nélkül!";
        }
        
    }
}
