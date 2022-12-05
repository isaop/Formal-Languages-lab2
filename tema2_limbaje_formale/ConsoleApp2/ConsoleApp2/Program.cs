using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp2
{
    internal class Program
    {
        bool isIdentifier(string str)
        {

            // If first character is invalid
            if (!((str[0] >= 'a' && str[0] <= 'z')
                  || (str[0] >= 'A' && str[0] <= 'Z')
                  || str[0] == '_'))
                return false;

            // Traverse the string for the rest of the characters
            for (int i = 1; i < str.Length; i++)
            {
                if (!((str[i] >= 'a' && str[i] <= 'z')
                      || (str[i] >= 'A' && str[i] <= 'Z')
                      || (str[i] >= '0' && str[i] <= '9')
                      || str[i] == '_'))
                    return false;
            }

            // String is a valid identifier
            return true;
        }

        bool isReservedWord(string str)
        {
            var reservedWords = new List<string>()
                    {
                        "list",
                        "character",
                        "begin",
                        "else",
                        "if",
                        "int",
                        "of",
                        "program",
                        "read",
                        "then",
                        "var",
                        "while",
                        "write",
                        "for",
                        "green",
                        "red",
                        "forall",
                        "true",
                        "false",
                        "flag",
                        "print",
                        "between",
                        "is"

                    };
            foreach(string word in reservedWords)
                if (str == word)
                    return true;
            return false;
        }

        bool isSpecialCharacter(string str)
        {
            var reservedWords = new List<string>()
                    {
                        "(",")","{","}",".",";",",",":","+","*","-","_","?","!","&&","||","|",
                        "<",">","=","==","%"
                    };

            foreach (string word in reservedWords)
                if (str == word)
                    return true;
            return false;
        }

        bool isStringConstant(string word)
        {
            if (word[0] > '!' && word[0] < '#')
                return true;

            return false;
        }

        bool isInteger(string a)
        {
            try
            {
                Convert.ToInt32(a);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }



        static void Main(string[] args)
        {
            Program p = new Program();
            Dictionary<int, string> TT = new Dictionary<int, string>();
            Dictionary<int, string> ST = new Dictionary<int, string>();
            Dictionary<int, string> errorList = new Dictionary<int, string>();

            TT.Add(0, "identifiers");
            TT.Add(1, "constants");
            TT.Add(2, "int");
            TT.Add(3, "if");
            TT.Add(4, "else");
            TT.Add(5, "list");
            TT.Add(6, "character");
            TT.Add(7, "begin");
            TT.Add(8, "of");
            TT.Add(9, "program");
            TT.Add(10, "read");
            TT.Add(11, "then");
            TT.Add(12, "var");
            TT.Add(13, "while");
            TT.Add(14, "write");
            TT.Add(15, "for");
            TT.Add(16, "green");
            TT.Add(17, "red");
            TT.Add(18, "false");
            TT.Add(19, ";");
            TT.Add(20, ":");
            TT.Add(21, "=");
            TT.Add(22, "(");
            TT.Add(23, ")");
            TT.Add(24, "{");
            TT.Add(25, "}");
            TT.Add(26, ".");
            TT.Add(27, ";");
            TT.Add(28, ",");
            TT.Add(29, ":");
            TT.Add(30, "+");
            TT.Add(31, "*");
            TT.Add(32, "-");
            TT.Add(33, "_");
            TT.Add(34, "?");
            TT.Add(35, "!");
            TT.Add(36, "&&");
            TT.Add(37, "||");
            TT.Add(38, "<");
            TT.Add(39, ">");
            TT.Add(40, ">=");
            TT.Add(41, "<=");
            TT.Add(42, "==");
            TT.Add(43, "%");
            TT.Add(44, "forall");
            TT.Add(45, "true");
            TT.Add(46, "is");
            TT.Add(47, "print");
            TT.Add(48, "flag");
            TT.Add(49, "between");


            Console.WriteLine("TOKEN TABLE: \n");
            Console.WriteLine(" code | token ");
            foreach (var elem in TT)
                Console.WriteLine("   " + elem.Key + "  |   " + elem.Value + "   ");
            var index = 0;
            Console.WriteLine("\n SYMBOL TABLE: \n");

            ///////////////symbol table
            int lineNumber = 0;
            foreach (string line in System.IO.File.ReadLines(@"..\\..\\..\\..\\small_program.txt"))
            {
                lineNumber++;
                string[] words = line.Split(' ');
                foreach (var word in words)
                {
                    if (word == " " || word == "\n" || word == "\t" || word == "")
                        continue;
                    else
                    if (p.isReservedWord(word))
                        continue;
                    else
                        if (p.isSpecialCharacter(word))
                        continue;

                    else
                        if (p.isIdentifier(word))
                    {
                        bool alreadyExists = false;
                        foreach (var elem in ST)
                            if (elem.Value == word)
                                alreadyExists = true;
                        if (alreadyExists == false)
                        {
                            ST.Add(index, word);
                            index++;
                        }
                    }
                    else if (p.isInteger(word) || p.isStringConstant(word))
                    {
                        bool alreadyExists = false;
                        foreach (var elem in ST)
                            if (elem.Value == word)
                                alreadyExists = true;
                        if (alreadyExists == false)
                        {
                            ST.Add(index, word);
                            index++;
                        }
                    }
                    else
                    {
                        errorList.Add(lineNumber,word);
                    }
                }

            }





            Console.WriteLine("[ code | symbol ]");
            foreach (var elem in ST)
            {

                Console.WriteLine("[  " + elem.Key + "   |  " + elem.Value + "  ]");
            }


            ////////////////////////////////////PIF
            
            List<string> values = new List<string>();
            Console.WriteLine("\n PIF: \n");
            Console.WriteLine("-- Symbol/Token | TT code | ST code --\n");
            foreach (string line in System.IO.File.ReadLines(@"..\\..\\..\\..\\small_program.txt"))
            {
                string[] words = line.Split(' ');
                foreach (var word in words)
                {
                    bool alreadyExists = false;
                    foreach(string value in values)
                    {
                        if(value == word)
                            alreadyExists = true;
                    }

                  

                        int TTPosition = -1;
                        int STPosition = -1;
                        if (word == " " || word == "\n" || word == "\t" || word == "")
                            continue;
                        else
                        if (p.isReservedWord(word))
                        {
                            foreach (var elem in TT)
                            {
                                if (elem.Value == word)
                                    TTPosition = elem.Key;
                            }
                        }
                        else
                            if (p.isSpecialCharacter(word))
                        {
                            foreach (var elem in TT)
                            {
                                if (elem.Value == word)
                                    TTPosition = elem.Key;
                            }
                        }
                        else
                            if (p.isIdentifier(word))
                        {
                            TTPosition = 0;
                            foreach (var elem in ST)
                            {
                                if (elem.Value == word)
                                    STPosition = elem.Key;
                            }
                        }
                        else if (p.isInteger(word) || p.isStringConstant(word))
                        {
                            TTPosition = 1;
                            foreach (var elem in ST)
                            {
                                if (elem.Value == word)
                                    STPosition = elem.Key;
                            }
                        }
                        else
                        {
                            STPosition = -1;
                            TTPosition = -1;
                            continue;

                        }

                        values.Add(word); 
                        Console.WriteLine("         " + word + " | " + TTPosition + " | " + STPosition + " \n");

                    
                }

            }
            
        

            foreach(var elem in errorList)
            {
                Console.WriteLine("You have a lexical error on line " + elem.Key + ": " + elem.Value + "\n");
            }



            Console.ReadKey();
        }
    }
}

