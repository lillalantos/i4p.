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

                    if (temp < 0) temp += 27;
                    messageCode += abc[temp];
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

            //foreach (string word in words)
            //{
            //    Console.WriteLine(word);
            //}

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


            int maximumIterations = 1000;
            int currentIteration = 0;


            //Addig keresse az új szavakat && kulcsrészleteket, amíg valamelyik üzenet szóközre végződik
            while ((Message1[Message1.Length - 1] == ' ' || Message2[Message2.Length - 1] == ' ') && currentIteration < maximumIterations)
            {
                currentIteration++;


                int LastIndex1 = Message1.LastIndexOf(' ');
                if (LastIndex1 == -1) LastIndex1 = 0;


                //Ha az utolsó szóköz nem a végén szerepel

                while (Message1.LastIndexOf(' ') != Message1.Length - 1)
                {
                    
                    //Üzenet utolsó befejezetlen szava
                    string MessageBlock1 = Message1.Substring(LastIndex1 + 1);

                    //Minden szó a listából, ami ezzel a szóval kezdődik
                    string[] PossibleWords1 = words.Where(s => s.StartsWith(MessageBlock1) && s.Length >= MessageBlock1.Length).ToArray();
                    //Console.WriteLine(PossibleWords1.Length);

                    while (!words.Contains(Message2.Substring(Message2.LastIndexOf(' '))))
                    {
                        foreach (string PossibleWord in PossibleWords1)
                        {
                            string PossibleKeyBlock = KeyBlock;

                            for (int i = MessageBlock1.Length; i < PossibleWord.Length; i++)
                            {

                                int temp = Array.IndexOf(abc, Code1[i]) - Array.IndexOf(abc, PossibleWord[i]);
                                if (temp < 0) temp += 27;
                                PossibleKeyBlock += abc[temp];

                            }


                            //string[] PossibleKeyBlock = new string[PossibleWords1.Length];
                            //foreach (string PossibleWord in PossibleWords1)
                            //{
                            //    string KeyPart = "";
                            //    for (int i = MessageBlock1.Length; i < PossibleWord.Length ; i++)
                            //    {
                            //        int temp = Array.IndexOf(abc, Code1[KeyBlock.Length + i - MessageBlock1.Length ]) - Array.IndexOf(abc, PossibleWord[i]);
                            //        if (temp < 0) temp += 27;
                            //        KeyPart += abc[temp];

                            //    }
                            //    PossibleKeyBlock[Array.IndexOf(PossibleWords1, PossibleWord)] = KeyPart;


                            int space = Array.IndexOf(abc, Code1[Message1.Length - 1 + PossibleWord.Length]) - Array.IndexOf(abc, ' ');
                            if (space < 0) space += 27;
                            PossibleKeyBlock += abc[space];


                            //int j = 0;
                            //while (j<PossibleKeyBlock.Length && MBlock2 != Message2)
                            //{
                            //    MBlock2 = Message(Code2.Substring(0, PossibleWord.Length + 1), PossibleKeyBlock[j]);
                            //    j++;
                            //}



                            string MBlock2 = Message(Code2.Substring(0, Message2.Length + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);
                            Console.WriteLine(MBlock2);
                            while (!string.IsNullOrEmpty(MBlock2))
                            {
                                Console.WriteLine(Message1);
                                Message1 = Message(Code1.Substring(0, Message1.Length + 1 + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);
                                Console.WriteLine(Message1.Length);
                                Message2 = MBlock2;

                                KeyBlock = PossibleKeyBlock;
                                break;
                            }
                        }
                    }

                }


                int LastIndex2 = Message2.LastIndexOf(' ');
                if (LastIndex2 == -1) LastIndex2 = 0;


                if (Message2.LastIndexOf(' ') != Message2.Length - 1)
                {
                    //Üzenet utolsó befejezetlen szava
                    string MessageBlock2 = Message2.Substring(LastIndex2);

                    //Console.WriteLine(MessageBlock2);

                    //Minden szó a listából, ami ezzel a szóval kezdődik
                    string[] PossibleWords2 = words.Where(s => s.StartsWith(MessageBlock2) && s.Length > MessageBlock2.Length).ToArray();


                    while (!words.Contains(Message1.Substring(Message1.LastIndexOf(' '))))
                    {
                        //Console.WriteLine(PossibleWords2.Length);
                        foreach (string PossibleWord in PossibleWords2)
                        {
                            string PossibleKeyBlock = KeyBlock;

                            for (int i = MessageBlock2.Length; i < PossibleWord.Length; i++)
                            {

                                int temp = Array.IndexOf(abc, Code2[i]) - Array.IndexOf(abc, PossibleWord[i]);
                                if (temp < 0) temp += 27;
                                PossibleKeyBlock += abc[temp];

                            }

                            int space = Array.IndexOf(abc, Code2[PossibleWord.Length]) - Array.IndexOf(abc, ' ');
                            if (space < 0) space += 27;
                            PossibleKeyBlock += abc[space];

                            //Console.WriteLine(PossibleKeyBlock);

             
                            string MBlock1 = Message(Code1.Substring(0, Message1.Length + PossibleWord.Length - MessageBlock2.Length), PossibleKeyBlock);
                            
                            Console.WriteLine(MBlock1);
                            while (!string.IsNullOrEmpty(MBlock1))
                            {
                                Message2 = Message(Code2.Substring(0, Message2.Length + 1 + PossibleWord.Length - MessageBlock2.Length), PossibleKeyBlock);
                                Console.WriteLine(Message2);
                                Message1 = MBlock1;
                                //Console.WriteLine(Message1);

                                KeyBlock = PossibleKeyBlock;
                                break;
                            }
                        }

                        //Console.WriteLine(Message1);
                        //Console.WriteLine(Message2);

                    }

                }


                
            }
            return KeyBlock;
        }
    }
}
