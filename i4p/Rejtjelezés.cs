using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace i4p
{
    public class Rejtjelezes
    {
        //Az 'a' kódjától 26 elem képzése, karakterré alakítása, és tömbbé formázása szóközöstül
        char[] abc = Enumerable.Range('a', 26).Select(x => (char)x).Append(' ').ToArray();

        //Adattagok és tulajdonságaik
        string message;
        string key;


        public Rejtjelezes(string message, string key)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Az üzenet nem lehet null!");

            if (key == null)
                throw new ArgumentNullException(nameof(key), "A kulcs nem lehet null!");

            //Üzenet validálásvizsgálat
            if (message.Any(ch => !abc.Contains(ch)))
                throw new ArgumentException("Az üzenet csak az angol abc kisbetűit és szóközt tartalmazhat!");

            this.message = message;

            //Kulcs hosszvizsgálat
            if (key.Length < message.Length)
                throw new ArgumentException("A kulcsnak legalább olyan hosszúnak kell lennie, mint az üzenetnek!");

            //Kulcs validálásvizsgálat
            if (key.Any(ch => !abc.Contains(ch)))
                throw new ArgumentException("A kulcs csak az angol abc kisbetűit és szóközt tartalmazhat!");

            this.key = key;
        }

        //Metódus a rejtjelezésre
        public string Code()
        {
            //Nullvizsgálat
            if (message == null)
                throw new InvalidOperationException("A rejtjelezett üzenet nem készíthető el egy megfelelő formátumú üzenet nélkül!");

            if (key == null)
                throw new InvalidOperationException("A rejtjelezett üzenet nem készíthető el egy megfelelő formátumú kulcs nélkül!");

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

        public string Message(string Code, string Key)
        {
            if (Code == null)
                throw new ArgumentNullException(nameof(Code), "A rejtjelezett üzenet nem lehet null!");

            if (key == null)
                throw new ArgumentNullException(nameof(key), "A kulcs nem lehet null!");

            if (Code.Any(ch => !abc.Contains(ch)))
                throw new ArgumentException("A rejtjelezett üzenet csak az angol abc kisbetűit és szóközt tartalmazhat!");

            if (key.Length < Code.Length)
                throw new ArgumentException("A kulcs hossza nem lehet rövidebb, mint a rejtjelezett üzeneté!");

            string messageCode = "";
            for (int i = 0; i < Code.Length; i++)
            {
                int temp = Array.IndexOf(abc, Code[i]) - Array.IndexOf(abc, key[i]);

                if (temp < 0) temp += 27;
                messageCode += abc[temp];
            }
            return messageCode;

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

            string Message1 = Word1;

            //Második rejtjelezett üzenet első felének dekódolása a kulcsrészlettel
            string Message2 = Message(Code2.Substring(0, Word1.Length), KeyBlock);


            int maximumIterations = 1000;
            int currentIteration = 0;

            //Addig keresse az új szavakat && kulcsrészleteket, amíg valamelyik üzenet szóközre végződik
            //Addig keresse az új szavakat && kulcsrészleteket, amíg valamelyik üzenet szóközre végződik
            while ((Message1[Message1.Length - 1] == ' ' || Message2[Message2.Length - 1] == ' ') && currentIteration < maximumIterations)
            {
                currentIteration++;

                int LastIndex1 = Message1.LastIndexOf(' ');
                if (LastIndex1 == -1) LastIndex1 = 0;

                string LastPart1;
                string LastPart2;

                if (Message1.Length > Word1.Length && Message1[Message1.Length - 1] == ' ')
                {
                    string temp = Message1.Substring(0, Message1.Length - 1);
                    LastPart1 = Message1.Substring(temp.LastIndexOf(' ') + 1, Message1.LastIndexOf(' ') - temp.LastIndexOf(' ') - 1);

                }

                else
                    LastPart1 = Message1.Substring(Message1.LastIndexOf(' ') + 1);

                if (Message2[Message2.Length - 1] == ' ')
                {
                    string temp = Message2.Substring(0, Message2.Length - 1);
                    LastPart2 = Message2.Substring(temp.LastIndexOf(' ') + 1, Message1.LastIndexOf(' ') - temp.LastIndexOf(' ') - 1);

                }

                else
                    LastPart2 = Message2.Substring(Message2.LastIndexOf(' ') + 1);


                //Ha az utolsó szóköz nem a végén szerepel
                if (Message1.LastIndexOf(' ') != Message1.Length - 1)
                {
                    //Üzenet utolsó befejezetlen szava
                    string MessageBlock1 = Message1.Substring(LastIndex1 + 1);

                    //Minden szó a listából, ami ezzel a szóval kezdődik
                    string[] PossibleWords1 = words.Where(s => s.StartsWith(MessageBlock1) && s.Length >= MessageBlock1.Length).ToArray();

                    while (!words.Contains(LastPart2) && Message1.Length < Code1.Length)
                    {
                        foreach (string PossibleWord in PossibleWords1)
                        {
                            string PossibleKeyBlock = KeyBlock;

                            for (int i = MessageBlock1.Length; i < PossibleWord.Length; i++)
                            {
                                int CodeIndex = Message1.Length - MessageBlock1.Length + i;
                                if (CodeIndex >= Code1.Length) break;

                                int temp = Array.IndexOf(abc, Code1[CodeIndex]) - Array.IndexOf(abc, PossibleWord[i]);
                                if (temp < 0) temp += 27;
                                PossibleKeyBlock += abc[temp];

                            }

                            //listából megtalált szó kiegészítése szóközzel
                            int space;
                            if (PossibleKeyBlock.Length < Code1.Length && Message1[Message1.Length - 1] != ' ')
                            {
                                space = Array.IndexOf(abc, Code1[PossibleKeyBlock.Length]) - Array.IndexOf(abc, ' ');
                                if (space < 0) space += 27;
                                PossibleKeyBlock += abc[space];
                            }

                            string MBlock2;
                            if (Message2.Length + 1 + PossibleWord.Length - MessageBlock1.Length <= Code2.Length)
                                MBlock2 = Message(Code2.Substring(0, PossibleKeyBlock.Length - 1), PossibleKeyBlock);
                            else
                                MBlock2 = Message(Code2, PossibleKeyBlock.Substring(0, Code2.Length - 1));
                            //string MBlock2 = Message(Code2.Substring(0, Message2.Length + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);

                            while (!string.IsNullOrEmpty(MBlock2))
                            {
                                if (Message1.Length + 1 + PossibleWord.Length - MessageBlock1.Length <= Code1.Length)
                                    Message1 = Message(Code1.Substring(0, PossibleKeyBlock.Length + 1), PossibleKeyBlock);
                                else
                                    Message1 = Message(Code1, PossibleKeyBlock.Substring(0, Code1.Length));
                                //Message1 = Message(Code1.Substring(0, Message1.Length + 1 + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);
                                Message2 = MBlock2;

                                KeyBlock = PossibleKeyBlock;
                                Console.WriteLine(Message1);
                                break;
                            }
                        }
                    }

                }


                int LastIndex2 = Message2.LastIndexOf(' ');
                if (LastIndex2 == -1) LastIndex2 = 0;

                
                // TÚL SOK MEGHÍVÁS, BE FOG AKADNI VÉGTELEN CIKLUSBA => FÜGGVÉNY CÉLRAVEZETŐBB
                while (Message2.LastIndexOf(' ') != Message2.Length - 1)
                {

                    

                    while (!words.Contains(LastPart1) && Message2.Length < Code2.Length)
                    {
                        //Üzenet utolsó befejezetlen szava
                        string MessageBlock2 = Message2.Substring(LastIndex2);

                        //Minden szó a listából, ami ezzel a szóval kezdődik
                        string[] PossibleWords2 = words.Where(s => s.StartsWith(MessageBlock2) && s.Length > MessageBlock2.Length).ToArray();



                        foreach (string PossibleWord in PossibleWords2)
                        {
                            MessageBlock2 = Message2.Substring(LastIndex2);
                            string PossibleKeyBlock = KeyBlock;

                            for (int i = MessageBlock2.Length; i < PossibleWord.Length; i++)
                            {
                                int CodeIndex = Message2.Length - MessageBlock2.Length + i;
                                if (CodeIndex >= Code2.Length) break;

                                int temp = Array.IndexOf(abc, Code2[CodeIndex]) - Array.IndexOf(abc, PossibleWord[i]);
                                if (temp < 0) temp += 27;
                                PossibleKeyBlock += abc[temp];

                            }

                            int space;
                            if (PossibleKeyBlock.Length < Code2.Length && Message2[Message2.Length - 1] != ' ')
                            {
                                space = Array.IndexOf(abc, Code2[PossibleKeyBlock.Length]) - Array.IndexOf(abc, ' ');
                                if (space < 0) space += 27;
                                PossibleKeyBlock += abc[space];
                            }

                            string MBlock1;
                            if (Message1.Length + 1 + PossibleWord.Length - MessageBlock2.Length <= Code1.Length)
                                MBlock1 = Message(Code1.Substring(0, PossibleKeyBlock.Length + 1), PossibleKeyBlock);
                            else
                                MBlock1 = Message(Code1, PossibleKeyBlock.Substring(0, Code1.Length));
                            //string MBlock1 = Message(Code1.Substring(0, Message1.Length + PossibleWord.Length - MessageBlock2.Length), PossibleKeyBlock);

                            while (!string.IsNullOrEmpty(MBlock1))
                            {
                                if (Message2.Length + 1 + PossibleWord.Length - MessageBlock2.Length <= Code2.Length)
                                {
                                    Message2 = Message(Code2.Substring(0, PossibleKeyBlock.Length + 1), PossibleKeyBlock);

                                }
                                else
                                    Message2 = Message(Code2, PossibleKeyBlock.Substring(0, Code2.Length - 1));
                                Message1 = MBlock1;

                                KeyBlock = PossibleKeyBlock;
                                

                                MessageBlock2 = Message2.Substring(LastIndex2);

                                if (Message1.Length > Word1.Length && Message1[Message1.Length - 1] == ' ')
                                {
                                    string temp = Message1.Substring(0, Message1.Length - 1);
                                    LastPart1 = Message1.Substring(temp.LastIndexOf(' ') + 1, Message1.LastIndexOf(' ') - temp.LastIndexOf(' ') - 1);

                                }


                                Console.WriteLine(Message2);
                                Console.WriteLine(KeyBlock);
                                Console.WriteLine(Message1);
                                break;
                            }
                            
                        }
                        
                    }
                    LastIndex2 = Message2.LastIndexOf(' ');
                    if (LastIndex2 == -1) LastIndex2 = 0;
                }
                
                return KeyBlock;
            }
            return KeyBlock;
        }
    }
}

