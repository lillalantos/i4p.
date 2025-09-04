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

            if (Key == null)
                throw new ArgumentNullException(nameof(Key), "A kulcs nem lehet null!");

            if (Code.Any(ch => !abc.Contains(ch)))
                throw new ArgumentException("A rejtjelezett üzenet csak az angol abc kisbetűit és szóközt tartalmazhat!");

            if (Key.Length < Code.Length)
                throw new ArgumentException("A kulcs hossza nem lehet rövidebb, mint a rejtjelezett üzeneté!");

            string messageCode = "";
            for (int i = 0; i < Code.Length; i++)
            {
                int temp = Array.IndexOf(abc, Code[i]) - Array.IndexOf(abc, Key[i]);

                if (temp < 0) temp += 27;
                messageCode += abc[temp];
            }
            return messageCode;

        }

        string Code1;
        string Code2;
        
        string message1;
        string message2;
        

        public Rejtjelezes(string word1, string Code1, string Code2)
        {
            string KeyBlock = "";
            for (int i = 0; i < word1.Length; i++)
            {
                int temp = Array.IndexOf(abc, Code1[i]) - Array.IndexOf(abc, word1[i]);
                if (temp >= 0)
                    KeyBlock += abc[temp];
                else
                    KeyBlock += abc[(27 + Array.IndexOf(abc, Code1[i])) - Array.IndexOf(abc, word1[i])];
            }


            this.message1 = word1;

            Rejtjelezes titkositas = new Rejtjelezes(Code2.Substring(0, message1.Length), KeyBlock);
            this.message2 = titkositas.Message(Code2.Substring(0, message1.Length), KeyBlock);

           
            this.Code1 = Code1;
            this.Code2 = Code2;

            this.key = KeyBlock;

        }


        public string KeyBlock(string LastBlock, string WhichCode)
        {
            string[] Messages = {message1, message2};

            int WhichMessage;
            int Othermessage;
            string OtherCode;

            if (WhichCode == Code1)
            {
                WhichMessage = 0;
                Othermessage = 1;
                OtherCode = Code2;
            }

            else
            {
                WhichMessage = 1;
                Othermessage = 0;
                OtherCode = Code1;
            }

            // TXT beolvasás soronként
            string filePath = "words.txt";
            string[] words = File.ReadAllLines(filePath);



            //Utolsó szórészlettel kezdődő szavak
            string[] PossibleWords = words.Where(s => s.StartsWith(LastBlock) && s.Length >= LastBlock.Length).ToArray();



            foreach (string PossibleWord in PossibleWords)
            {
                string PossibleKeyBlock = key;


                if (Messages[WhichMessage][Messages[WhichMessage].Length - 1] == ' ')
                    break;

                for (int i = LastBlock.Length; i < PossibleWord.Length; i++)
                {
                    int CodeIndex = Messages[WhichMessage].Length - LastBlock.Length + i;
                    if (CodeIndex >= WhichCode.Length) break;

                    int temp = Array.IndexOf(abc, WhichCode[CodeIndex]) - Array.IndexOf(abc, PossibleWord[i]);
                    if (temp < 0) temp += 27;
                    PossibleKeyBlock += abc[temp];
                }

                //listából megtalált szó kiegészítése szóközzel
                int space;
                if (PossibleKeyBlock.Length < WhichCode.Length && Messages[WhichMessage][Messages[WhichMessage].Length - 1] != ' ')
                {
                    space = Array.IndexOf(abc, WhichCode[PossibleKeyBlock.Length]) - Array.IndexOf(abc, ' ');
                    if (space < 0) space += 27;
                    PossibleKeyBlock += abc[space];
                }

                string MBlock;
                if (PossibleKeyBlock.Length <= OtherCode.Length)
                    MBlock = Message(OtherCode.Substring(0, PossibleKeyBlock.Length ), PossibleKeyBlock);
                else
                    MBlock = Message(OtherCode, PossibleKeyBlock.Substring(0, OtherCode.Length ));
                //string MBlock2 = Message(Code2.Substring(0, Message2.Length + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);

                string LastPartOfOtherMessage = MBlock.Substring(MBlock.LastIndexOf(' ') + 1);
                string[] Contains = words.Where(s => s.StartsWith(LastPartOfOtherMessage) && s.Length >= LastPartOfOtherMessage.Length).ToArray();

                while (!string.IsNullOrEmpty(MBlock) && Contains.Length != 0)
                {
                    if (Messages[WhichMessage].Length + 1 + PossibleWord.Length - LastBlock.Length <= WhichCode.Length)
                        Messages[WhichMessage] = Message(WhichCode.Substring(0, PossibleKeyBlock.Length ), PossibleKeyBlock);
                    else
                        Messages[WhichMessage] = Message(WhichCode, PossibleKeyBlock.Substring(0, WhichCode.Length));
                    //Message1 = Message(Code1.Substring(0, Message1.Length + 1 + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);
                    Messages[Othermessage] = MBlock;

                    key = PossibleKeyBlock;
                    message1 = Messages[0];
                    message2 = Messages[1];
                    //Console.WriteLine(WhichMessage);
                    break;
                }
            }
            return this.key;
        }
        
        public string CommonKey()
        {

            while (message1.Length < Code1.Length || message2.Length < Code2.Length)
            {
                if (message1[message1.Length - 1] != ' ')
                {
                    string LastBlock = message1.Substring(message1.LastIndexOf(' ') + 1);
                    key = KeyBlock(LastBlock, Code1);
                }
                else
                {
                    string LastBlock = message2.Substring(message2.LastIndexOf(' ') + 1);
                    key = KeyBlock(LastBlock, Code2);
                }
            }
            //Console.WriteLine(message1);
            //Console.WriteLine(message2);
            Console.WriteLine(key);
            return key;
        } 

    }
}