using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    Main, Int, Float, sTring, read, write, repeat, until, If, elseif, Else, then, Return, endl, end,
    Semicolon, Comma, LParanthesis, RParanthesis, Rbraces, Lbraces, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, assignmentOp, orOp, andOp,
    Idenifier, Number, stringVal
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {

            /*  Int , Float , sTring , read , write , repeat ,until , If ,elseif , Else ,then ,Return , endl,
       Semicolon, Comma, LParanthesis, RParanthesis,Rbracket,Lbracket, EqualOp, LessThanOp,
      GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
      Idenifier, Constant*/
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("endl", Token_Class.endl);
            ReservedWords.Add("end", Token_Class.end);
            ReservedWords.Add("until", Token_Class.until);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.elseif);
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.sTring);
            ReservedWords.Add("read", Token_Class.read);
            ReservedWords.Add("write", Token_Class.write);
            ReservedWords.Add("repeat", Token_Class.repeat);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("then", Token_Class.then);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add(":=", Token_Class.assignmentOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("}", Token_Class.Rbraces);
            Operators.Add("{", Token_Class.Lbraces);
            Operators.Add("||", Token_Class.orOp);
            Operators.Add("&&", Token_Class.andOp);




        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                bool zz = false;
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'Z' || CurrentChar >= 'a' && CurrentChar <= 'z') //if you read a character
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar >= 'A' && CurrentChar <= 'Z' || CurrentChar >= 'a' && CurrentChar <= 'z' || CurrentChar >= '0' && CurrentChar <= '9')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else
                            break;
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    bool letterIncluded = false;
                    j++;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if ((CurrentChar >= '0' && CurrentChar <= '9') || (CurrentChar == '.'))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else if (CurrentChar >= 'A' && CurrentChar <= 'z')
                        {
                            letterIncluded = true;
                            CurrentLexeme += CurrentChar.ToString();

                        }
                        else
                            break;
                        j++;
                    }
                    if (letterIncluded)
                    {
                        Errors.Error_List.Add(CurrentLexeme);

                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '/')
                {
                    bool openComment = true;
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '*')
                        {
                            j++;
                            while (j < SourceCode.Length)
                            {
                                if (SourceCode[j] == '*')
                                {
                                    j++;
                                    if (j < SourceCode.Length)
                                    {
                                        if (SourceCode[j] == '/')
                                        {
                                            openComment = false;
                                            i = j;
                                            break;
                                        }
                                    }

                                }
                                i = j;
                                j++;
                            }
                            if (openComment)
                            {
                                Errors.Error_List.Add("A Comment is Opened without being Closed Correctly");

                            }
                        }
                        else
                        {
                            i = j - 1;
                            FindTokenClass(CurrentLexeme);

                        }
                    }

                }
                else if (CurrentChar == '"')
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (CurrentChar == '"')
                            break;
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == ':')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '=')
                        {
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar.ToString();
                            FindTokenClass(CurrentLexeme);
                            i = j;
                        }
                        else
                        {
                            Errors.Error_List.Add(CurrentChar.ToString());
                            i = j - 1;

                        }
                    }
                }
                else if (CurrentChar == '.')
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                            break;
                        j++;
                    }
                    Errors.Error_List.Add(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '<')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '>')
                        {
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar.ToString();
                            FindTokenClass(CurrentLexeme);
                            i = j;
                        }
                        else
                        {
                            FindTokenClass(CurrentLexeme);
                            i = j - 1;

                        }
                    }
                }
                else if (CurrentChar == '|')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '|')
                        {
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar.ToString();
                            FindTokenClass(CurrentLexeme);
                            i = j;
                        }
                        else
                        {
                            Errors.Error_List.Add(CurrentLexeme);
                            i = j - 1;

                        }
                    }
                }
                else if (CurrentChar == '&')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '&')
                        {
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar.ToString();
                            FindTokenClass(CurrentLexeme);
                            i = j;
                        }
                        else
                        {
                            Errors.Error_List.Add(CurrentLexeme);
                            i = j - 1;

                        }
                    }
                }
                else
                {
                    zz = true;
                    FindTokenClass(CurrentLexeme);
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Tok.lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }

            //Is it a Constant?
            else if (isNumber(Tok.lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);

            }
            //IS a string value
            else if (isStringVal(Tok.lex))
            {
                Tok.token_type = Token_Class.stringVal;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }



        bool isIdentifier(string lex)
        {
            bool isValid = false;
            // Check if the lex is an identifier or not.
            var idnt = new Regex("^[a-zA-Z]([a-zA-Z0-9])*$", RegexOptions.Compiled);
            if (idnt.IsMatch(lex))
                isValid = true;

            return isValid;
        }
        bool isNumber(string lex)
        {
            bool isValid = false;
            // Check if the lex is a constant (Number) or not.
            var idnt = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            if (idnt.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
        bool isStringVal(string lex)
        {
            bool isString = false;
            var ss = new Regex("^\"([^(\")])*(\")$", RegexOptions.Compiled);
            if (ss.IsMatch(lex))
                isString = true;
            return isString;
        }

    }
}
