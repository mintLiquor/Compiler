using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexAnalysis
{
    class myanalysis
    {
        //单词符号及类别编码
        public enum Type
        {
            IDENTIFIER,//标识符
            CONST_INT, CONST_FLOAT,//整数和浮点数常量
            CONST_CHAR, CONST_STRING,//字符和字符串常量
            //4
            ADD, SUB, MULTIPLY, DEVIDE, MOD,// + - * / % 加减乘除取余
            INC, DEC,// ++ -- 自增自减
            ASSIGN,// = 赋值
            ADDEQ, SUBEQ, MULTIPLYEQ, DEVIDEEQ, MODEQ,// += -= *= /= %= 加减乘除与等于的连接
            GT, LT, EQ, GET, LET,// > < == >= <= 比较运算符
            AND_BIT, OR_BIT, NOT_BIT,// & | ^ 位运算符
            PARENTHESIS_R, PARENTHESIS_L, BRACKET_R, BRACKET_L, BRACES_R, BRACES_L,// ( ) [ ] { } 三种括号
            AND_LOGICAL, OR_LOGICAL, NOT_LOGICAL,// && || ! 逻辑运算符
            //34
            COMMA, DOT, COLON, SEMICOLON, QUESTION, SHARP,// , . : ; ? #其他符号
            NOTES,//注释符
            //41
            //关键字
            AUTO, BREAK, CASE, CHAR, CONST, CONTINUE, DEFAULT, DO,
            DOUBLE, ELSE, ENUM, EXTERN, FLOAT, FOR, GOTO, IF,
            INT, LONG, REGISTER, RETURN, SHORT, SIGNED, SIZEOF, STATIC,
            STRUCT, SWITCH, TYPEDEF, UNION, UNSINGED, VOID, VOLATILE, WHILE
                //73
        };

        //关键字
        public Dictionary<string, Type> key = new Dictionary<string, Type> {
            { "auto", Type.AUTO }, { "break", Type.BREAK }, { "case", Type.CASE }, { "char",Type.CHAR },
            { "const", Type.CONST }, { "continue", Type.CONTINUE }, { "default", Type.DEFAULT }, { "do", Type.DO },
            { "double", Type.DOUBLE }, { "else", Type.ELSE }, { "enum", Type.ENUM }, { "extern", Type.EXTERN },
            { "float", Type.FLOAT }, { "for", Type.FOR }, { "goto", Type.GOTO }, { "if", Type.IF },
            { "int", Type.INT }, { "long", Type.LONG }, { "register", Type.REGISTER }, { "return", Type.RETURN },
            { "short", Type.SHORT }, { "signed", Type.SIGNED }, { "sizeof", Type.SIZEOF }, { "static", Type.STATIC },
            { "struct", Type.STRUCT }, { "switch", Type.SWITCH }, { "typedef", Type.TYPEDEF }, { "union", Type.UNION },
            { "unsigned", Type.UNSINGED }, { "void", Type.VOID }, { "volatile", Type.VOLATILE }, { "while", Type.WHILE }
        };

        //存储数据的结构
        public struct Data
        {
            public Type code;
            public string value;
            public string name;
            public Data(Type code, string value, string name)
            {
                this.code = code;
                this.value = value;
                this.name = name;
            }
        }

        //扫描到的单词列表
        public Queue<Data> token = new Queue<Data>();

        //分析一语句的词法
        public Queue<Data> analysis(String line)
        {
            if (line == String.Empty)
            {
                return token;
            }
            //按行遍历，方便跳过注释

            string keyword = string.Empty;    //识别的符号串
            int status = 0;//状态
            int length = 0;
            char current;
            int i = 0;

            length = line.Length;
            for (i = 0; i < length; i++)
            {
                current = line[i];
                switch (status)
                {
                    case 0:
                        status = currentStatus0(ref keyword, current);
                        break;
                    case 1:
                        status = currentStatus1(ref keyword, current, ref i);
                        break;
                    case 2:
                        status = currentStatus2(ref keyword, current, ref i);
                        break;
                    case 3:
                        status = currentStatus3(ref keyword, current, ref i);
                        break;
                    case 4:
                        status = currentStatus4(ref keyword, current, ref i);
                        break;
                    case 5:
                        status = currentStatus5(ref keyword, current, ref i);
                        break;
                    case 6:
                        status = currentStatus6(ref keyword, current, ref i);
                        break;
                    case 7:
                        status = currentStatus7(ref keyword, current, ref i, length);
                        break;
                    case 8:
                        status = currentStatus8(ref keyword, current);
                        break;
                    case 9:
                        status = currentStatus9(ref keyword, current);
                        break;
                    case 10:
                        status = currentStatus10(ref keyword, current, ref i);
                        break;
                    case 11:
                        status = currentStatus11(ref keyword, current, ref i);
                        break;
                    case 12:
                        status = currentStatus12(ref keyword, current, ref i);
                        break;
                    default:
                        break;
                }
            }
            //一行匹配结束
            if (!keyword.Equals(string.Empty))
            {
                status = endOfLine(status, ref keyword);
            }

            return token;
        }

        private int currentStatus0(ref string keyword, char current)
        {
            int status = 0;
            string type_temp = "";
            //字母或下划线
            if ((current >= 'a' && current <= 'z') || (current >= 'A' && current <= 'Z') || current == '_')
            {
                status = 1;
            }
            //数字
            else if (current >= '0' && current <= '9')
            {
                status = 2;
            }
            else if (current == '+')
            {
                status = 4;
            }
            else if (current == '-')
            {
                status = 5;
            }
            else if (current == '*')
            {
                status = 6;
                keyword += current;
            }
            else if (current == '/')
            {
                status = 7;
            }
            else if (current == '>')
            {
                status = 10;
            }
            else if (current == '<')
            {
                status = 11;
            }
            else if (current == '=')
            {
                status = 12;
            }
            else if ("()[]{}.,;?:#".ToCharArray().Contains<char>(current))
            {
                //string type_temp;
                Type type = new Type();
                switch (current)
                {
                    case '(':
                        type = Type.PARENTHESIS_L;
                        type_temp = "  ,左括号  ";
                        break;
                    case ')':
                        type = Type.PARENTHESIS_R;
                        type_temp = "  ,右括号  ";
                        break;
                    case '[':
                        type = Type.BRACKET_L;
                        type_temp = "  ,左中括号  ";
                        break;
                    case ']':
                        type = Type.BRACKET_R;
                        type_temp = "  ,右中括号  ";
                        break;
                    case '{':
                        type = Type.BRACES_L;
                        type_temp = "  ,左大括号  ";
                        break;
                    case '}':
                        type = Type.BRACES_R;
                        type_temp = "  ,右大括号  ";
                        break;
                    case '.':
                        type = Type.DOT;
                        type_temp = "  ,点  ";
                        break;
                    case ',':
                        type = Type.COMMA;
                        type_temp = "  ,分隔号  ";
                        break;
                    case ';':
                        type = Type.SEMICOLON;
                        type_temp = "  ,界符  ";
                        break;
                    case '?':
                        type = Type.QUESTION;
                        type_temp = "  ,问号  ";
                        break;
                    case ':':
                        type = Type.COLON;
                        type_temp = "  ,冒号  ";
                        break;
                    case '#':
                        type = Type.SHARP;
                        type_temp = "  ,结束符  ";
                        break;
                }
                token.Enqueue(new Data(type, current.ToString(), type_temp));
                return 0;
            }
            //忽略其他字符
            else
            {
                return status;
            }
            keyword += current;
            return status;
        }

        private int currentStatus1(ref string keyword, char current, ref int i)
        {
            int status = 1;
            //字母数字下划线
            if ((current >= 'a' && current <= 'z') || (current >= 'A' && current <= 'Z') ||
                (current >= '0' && current <= '9') || current == '_')
            {
                keyword += current;
            }
            else
            {
                //关键字
                if (key.ContainsKey(keyword))
                {
                    token.Enqueue(new Data(key[keyword], keyword, "  ,关键字  "));
                }
                //标识符
                else
                {

                    token.Enqueue(new Data(Type.IDENTIFIER, keyword, "  ,标识符  "));
                }
                keyword = string.Empty;
                i--;
                status = 0;
            }
            return status;
        }

        private int currentStatus2(ref string keyword, char current, ref int i)
        {
            int status = 2;
            //数字
            if (current >= '0' && current <= '9')
            {
                keyword += current;
            }
            //点
            else if (current == '.')
            {
                status = 3;
                keyword += current;
            }
            //整数匹配结束
            else
            {
                token.Enqueue(new Data(Type.CONST_INT, keyword, "  ,整数  "));
                keyword = string.Empty;
                status = 0;
                i--;
            }
            return status;
        }

        private int currentStatus3(ref string keyword, char current, ref int i)
        {
            int status = 3;
            if (current >= '0' && current <= '9')
            {
                keyword += current;
            }
            //小数匹配结束
            else
            {
                token.Enqueue(new Data(Type.CONST_FLOAT, keyword, "  ,小数  "));
                keyword = string.Empty;
                status = 0;
                i--;
            }
            return status;
        }

        private int currentStatus4(ref string keyword, char current, ref int i)
        {
            //++
            if (current == '+')
            {
                token.Enqueue(new Data(Type.INC, keyword + current, "  ,自增号  "));
            }
            //+=
            else if (current == '=')
            {
                token.Enqueue(new Data(Type.ADDEQ, keyword + current, "  ,加赋值号  "));

            }
            //+
            else
            {
                token.Enqueue(new Data(Type.ADD, keyword, "  ,加号  "));
                i--;
            }
            keyword = string.Empty;
            return 0;
        }

        private int currentStatus5(ref string keyword, char current, ref int i)
        {
            //--
            if (current == '-')
            {
                token.Enqueue(new Data(Type.DEC, keyword + current, "  ,自减号  "));
            }
            //-=
            else if (current == '=')
            {
                token.Enqueue(new Data(Type.SUBEQ, keyword + current, "  ,减赋值号  "));

            }
            //-
            else
            {
                token.Enqueue(new Data(Type.SUB, keyword, "  ,减号  "));
                i--;
            }
            keyword = string.Empty;
            return 0;
        }

        private int currentStatus6(ref string keyword, char current, ref int i)
        {
            // *=
            if (current == '=')
            {
                token.Enqueue(new Data(Type.MULTIPLYEQ, "*" + current, "  ,乘赋值号  "));
            }
            // *
            else
            {
                token.Enqueue(new Data(Type.MULTIPLY, "*", "  ,乘号  "));
                i--;
            }
            keyword = string.Empty;
            return 0;
        }

        private int currentStatus7(ref string keyword, char current, ref int i, int length)
        {
            int status = 0;
            // /=
            if (current == '=')
            {
                token.Enqueue(new Data(Type.DEVIDEEQ, keyword + current, "  ,除赋值号  "));
                keyword = string.Empty;
            }
            //单行注释
            else if (current == '/')
            {
                //下标移到行尾
                keyword += current;
                token.Enqueue(new Data(Type.NOTES, keyword, "  ,注释符  "));
                keyword = string.Empty;
                i = length;
            }
            // 区块注释/*
            else if (current == '*')
            {
                keyword += current;
                token.Enqueue(new Data(Type.NOTES, keyword, "  ,注释符  "));
                status = 8;
            }
            // /
            else
            {
                token.Enqueue(new Data(Type.DEVIDE, keyword, "  ,除号  "));
                keyword = string.Empty;
                status = 0;
                i--;
            }
            return status;
        }

        private int currentStatus8(ref string keyword, char current)
        {
            int status = 8;
            if (current == '*')
            {
                keyword += current;
                status = 9;
            }
            return status;
        }

        private int currentStatus9(ref string keyword, char current)
        {
            int status = 9;
            //区块注释结束
            if (current == '/')
            {
                keyword += current;
                token.Enqueue(new Data(Type.NOTES, "*/", "  ,注释符  "));
                keyword = string.Empty;
                status = 0;
            }
            else if (current != '*')
            {
                status = 8;
            }
            return status;
        }

        private int currentStatus10(ref string keyword, char current, ref int i)
        {
            // >=
            if (current == '=')
            {
                token.Enqueue(new Data(Type.GET, keyword + current, "  ,大于等于号  "));
            }
            // >
            else
            {
                token.Enqueue(new Data(Type.GT, keyword, "  ,大于号  "));
                i--;
            }
            keyword = string.Empty;
            return 0;
        }

        private int currentStatus11(ref string keyword, char current, ref int i)
        {
            // <=
            if (current == '=')
            {
                token.Enqueue(new Data(Type.LET, keyword + current, "  ,小于等于号  "));
            }
            // <
            else
            {
                token.Enqueue(new Data(Type.LT, keyword, "  ,小于号  "));
                i--;
            }
            keyword = string.Empty;
            return 0;
        }

        private int currentStatus12(ref string keyword, char current, ref int i)
        {
            // ==
            if (current == '=')
            {
                token.Enqueue(new Data(Type.EQ, keyword + current, "  ,等于号  "));
            }
            // =
            else
            {
                token.Enqueue(new Data(Type.ASSIGN, keyword, "  ,赋值号  "));
                i--;
            }
            keyword = string.Empty;
            return 0;
        }

        private int endOfLine(int status, ref string keyword)
        {
            string stype_temp = "";
            if (status == 9 || status == 8)
            {
                return 8;
            }
            Type type = new Type();
            switch (status)
            {
                case 1:
                    type = Type.IDENTIFIER;
                    stype_temp += "  ,标识符  ";
                    break;
                case 2:
                    type = Type.CONST_INT;
                    stype_temp += "  ,整数  ";
                    break;
                case 3:
                    type = Type.CONST_FLOAT;
                    stype_temp += "  ,小数  ";
                    break;
                case 4:
                    type = Type.ADD;
                    stype_temp += "  ,加号  ";
                    break;
                case 5:
                    type = Type.SUB;
                    stype_temp += "  ,减号  ";
                    break;
                case 6:
                    type = Type.MULTIPLY;
                    keyword = "*";
                    stype_temp += "  ,乘号  ";
                    break;
                case 7:
                    type = Type.DEVIDE;
                    stype_temp += "  ,除号  ";
                    break;
                case 10:
                    type = Type.GT;
                    stype_temp += "  ,大于号  ";
                    break;
                case 11:
                    type = Type.LT;
                    stype_temp += "  ,小于号  ";
                    break;
                case 12:
                    type = Type.EQ;
                    stype_temp += "  ,等于号  ";
                    break;
                default:
                    break;
            }
            token.Enqueue(new Data(type, keyword, stype_temp));
            keyword = string.Empty;
            return 0;
        }

    }
}
