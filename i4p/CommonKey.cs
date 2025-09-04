//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

//namespace i4p
//{
//    public class CommonKey 
//    {
//        string Message1;
//        string Message2;
//        string Code1;
//        string Code2;
//        string[] words;
//        char[] abc = Enumerable.Range('a', 26).Select(x => (char)x).Append(' ').ToArray();
//        string LastPart;
//        string Key;




//        public CommonKey(string message1, string Code1, string Code2)
//        {
//            string KeyBlock = "";
//            for (int i = 0; i < message1.Length; i++)
//            {
//                int temp = Array.IndexOf(abc, Code1[i]) - Array.IndexOf(abc, message1[i]);
//                if (temp >= 0)
//                    KeyBlock += abc[temp];
//                else
//                    KeyBlock += abc[(27 + Array.IndexOf(abc, Code1[i])) - Array.IndexOf(abc, message1[i])];
//            }

            
//            this.Message1 = message1;
//            Rejtjelezes titkositas = new Rejtjelezes(Code2.Substring(0, message1.Length), KeyBlock);
//            this.Message2 = titkositas.Message(Code2.Substring(0, message1.Length), KeyBlock);

//            this.Key = KeyBlock;
//            this.Code1 = Code1;
//            this.Code2 = Code2;

//            // TXT beolvasás soronként
//            string filePath = "words.txt";
//            this.words = File.ReadAllLines(filePath);


//            if (Message1[Message1.Length-1] != ' ')
//                LastPart = Message1.Substring(Message1.LastIndexOf(' ') + 1);
//            else
//                LastPart = Message2.Substring(Message2.LastIndexOf(' ') + 1);
//        }

        
        
//       public string KeyBlock()
//        {
//            //Utolsó szórészlettel kezdődő szavak
//            string[] PossibleWords = words.Where(s => s.StartsWith(LastPart) && s.Length >= LastPart.Length).ToArray();

//            string Code;
//            if (Message1.Contains(LastPart))
//                Code = Code1;
//            else
//                Code = Code2;

//            foreach (string PossibleWord in PossibleWords)
//            {
//                string PossibleKeyBlock = Key;

//                for (int i = LastPart.Length; i < PossibleWord.Length; i++)
//                {
//                    int CodeIndex = Message1.Length - LastPart.Length + i;
//                    if (CodeIndex >= Code.Length) break;

//                    int temp = Array.IndexOf(abc, Code[CodeIndex]) - Array.IndexOf(abc, PossibleWord[i]);
//                    if (temp < 0) temp += 27;
//                    PossibleKeyBlock += abc[temp];
//                }

//                //listából megtalált szó kiegészítése szóközzel
//                int space;
//                if (PossibleKeyBlock.Length < Code1.Length && Message1[Message1.Length - 1] != ' ')
//                {
//                    space = Array.IndexOf(abc, Code1[PossibleKeyBlock.Length]) - Array.IndexOf(abc, ' ');
//                    if (space < 0) space += 27;
//                    PossibleKeyBlock += abc[space];
//                }

//                string MBlock2;
//                if (Message2.Length + 1 + PossibleWord.Length - LastPart.Length <= Code2.Length)
//                    MBlock2 = Rejtjelezes.Message(Code2.Substring(0, PossibleKeyBlock.Length - 1), PossibleKeyBlock);
//                else
//                    MBlock2 = Rejtjelezes.Message(Code2, PossibleKeyBlock.Substring(0, Code2.Length - 1));
//                //string MBlock2 = Message(Code2.Substring(0, Message2.Length + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);

//                while (!string.IsNullOrEmpty(MBlock2))
//                {
//                    if (Message1.Length + 1 + PossibleWord.Length - LastPart.Length <= Code1.Length)
//                        Message1 = Rejtjelezes.Message(Code1.Substring(0, PossibleKeyBlock.Length + 1), PossibleKeyBlock);
//                    else
//                        Message1 = Rejtjelezes.Message(Code1, PossibleKeyBlock.Substring(0, Code1.Length));
//                    //Message1 = Message(Code1.Substring(0, Message1.Length + 1 + PossibleWord.Length - MessageBlock1.Length), PossibleKeyBlock);
//                    Message2 = MBlock2;

//                    Key = PossibleKeyBlock;
//                    Console.WriteLine(Message1);
//                    break;
//                }
//            }
//            return Message1;
            

//        }
//    }
//}
