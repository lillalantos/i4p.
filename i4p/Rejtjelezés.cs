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
            if (message != null && key != null)
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
                return $"Rejtjelezett üzenet: {code}";
            }

            //Ha az üzenet vagy kulcs hibás formátumú
            else if (message == null)
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

            if (key != null)
            {
                string messageCode = "";
                for (int i = 0; i < Code.Length; i++)
                {
                    int temp = Array.IndexOf(abc, Code[i]) - Array.IndexOf(abc, key[i]);

                    if (temp >= 0)
                        messageCode += abc[temp];
                    else
                    {
                        int remainder = (27 + Array.IndexOf(abc, Code[i])) - Array.IndexOf(abc, key[i]);
                        messageCode += abc[remainder];
                    }
                }
                return messageCode;
            }
            return "Az üzenet dekódolása nem készíthető el egy megfelelő formátumú kulcs nélkül!";
        }


        public string CommonKey(string Code1, string Code2, string Word1)
        {

            // TXT beolvasás soronként
            string filePath = "words.txt";
            string[] words = File.ReadAllLines(filePath);

            //Early -vel meghatározott kulcsrészlet
            string KeyBlock = "";
            for (int i = 0; i < Word1.Length; i++)
            {
                int temp = Array.IndexOf(abc, Code1[i]) - Array.IndexOf(abc, Word1[i]);
                if (temp >= 0)
                    KeyBlock += abc[temp];
                else
                    KeyBlock += abc[(27 + Array.IndexOf(abc, Code1[i])) - Array.IndexOf(abc, Word1[i])];
            }
            //Console.WriteLine(KeyBlock);
            string Message1 = Word1;
            //Console.WriteLine(Message1.Length);

            //Második rejtjelezett üzenet első felének dekódolása a kulcsrészlettel
            string Message2 = Message(Code2.Substring(0, Word1.Length), KeyBlock);
            //Console.WriteLine(Message2);

            //Addig keresse az új szavakat && kulcsrészleteket, amíg valamelyik üzenet szóközre végződik
            while (Message1[Message1.Length - 1] == ' ' || Message2[Message2.Length - 1] == ' ')
            {
                
                int space1 = Message1.LastIndexOf(' ');
                int LastIndex1;
                if (space1 != -1)  //Substringnél értelmezhető legyen 
                    LastIndex1 = space1;
                LastIndex1 = 0;

                //Ha az utolsó szóköz nem a végén szerepel
                if (LastIndex1 != Message1.Length - 1)        
                {
                    //Üzenet utolsó befejezetlen szava
                    string MessageBlock1 = Message1.Substring(LastIndex1);

                    //Minden szó a listából, ami ezzel a szóval kezdődik
                    string[] PossibleWords = words.Where(s => s.StartsWith(MessageBlock1)).ToArray();



                    //Lehetséges szavakkal új kulcs generálása
                    string PossibleKeyBlock = KeyBlock;
                    int i = 0;
                    //Új kulcsrészlet által üzenet tovább dekódolása
                    while (!words.Contains(Message(Code2.Substring(Message2.Length), PossibleKeyBlock)) && i<PossibleWords.Length)    //Amíg a szavak listája nem tartalmazza a dekódolt részletet, addig vizsgálja a lehetséges szavakat
                    {
                        
                        for (int j = LastIndex1; j < PossibleWords[i].Length; j++)
                        {
                            int temp = Array.IndexOf(abc, Code1[j]) - Array.IndexOf(abc, PossibleWords[i][j]);
                            if (temp >= 0)
                                PossibleKeyBlock += abc[temp];
                            PossibleKeyBlock += abc[(27 + Array.IndexOf(abc, Code1[j])) - Array.IndexOf(abc, PossibleWords[i][j])];
                        }
                        i++;
                    }

                    Message1 += Message(Code2.Substring(Message2.Length), PossibleKeyBlock);


                }


                int space2 = Message2.LastIndexOf(' ');
                int LastIndex2;
                if (space2 != -1)
                    LastIndex2 = space2;
                LastIndex2 = 0;

                if (LastIndex2 != Message2.Length - 1)        
                {
                    //Üzenet utolsó befejezetlen szava
                    string MessageBlock2 = Message2.Substring(LastIndex2);


                    //Minden szó a listából, ami ezzel a szóval kezdődik
                    string[] PossibleWords = words.Where(s => s.StartsWith(MessageBlock2)).ToArray();



                    //Lehetséges szavakkal új kulcs generálása
                    string PossibleKeyBlock ="";
                    int i = 0;
                    
                    //Új kulcsrészlet által üzenet tovább dekódolása
                   // while (!words.Contains(Message(Code1.Substring(Message1.Length, PossibleKeyBlock.Length), PossibleKeyBlock)) && i<PossibleWords.Length)    //Amíg a szavak listája nem tartalmazza a dekódolt részletet, addig vizsgálja a lehetséges szavakat
                    //{
                        for (int j = MessageBlock2.Length; j < PossibleWords[i].Length; j++)
                        {
                            int temp = Array.IndexOf(abc, Code2[j]) - Array.IndexOf(abc, PossibleWords[i][j]);
                            if (temp >= 0)
                                PossibleKeyBlock += abc[temp];
                            else
                                PossibleKeyBlock += abc[(27 + Array.IndexOf(abc, Code2[j])) - Array.IndexOf(abc, PossibleWords[i][j])];
                            
                        
                            Console.WriteLine(PossibleKeyBlock);
                            
                            Console.WriteLine(PossibleWords[i]);
                        }

                    Console.WriteLine(Message(Code1, PossibleKeyBlock));
                    Console.WriteLine(Message(Code2, PossibleKeyBlock));
                    i++;
                        
                   // }

                    
                }

            }
            return KeyBlock;
        }
    }
}