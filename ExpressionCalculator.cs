using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{ 

    /// <summary>
    /// 表达式计算类
    /// </summary>
    public class ExpressionCalculator
    {
        /// <summary>
        /// 计算数学表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static decimal Calculate(string expression)
        {                   
            Analyse simple = new Analyse(expression);
            return (decimal)simple.DoEvent();           
        }

      
        public class Tool
        {
            /// <summary>
            /// 字符是否数字
            /// </summary>
            /// <param name="newChar"></param>
            /// <returns></returns>
            public static bool IsNumberic(char newChar)
            {
                try
                {
                    int.Parse(newChar.ToString());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        class Analyse
        {
            string m_formula;
            int m_numA = 0;
            int m_numB = 0;
            char m_operation = '~';

            public Analyse(string formula)
            {
                this.m_formula = formula;
            }

            public int DoEvent()
            {
                //IList<char> charList=new List<char>();

                char[] charArray = new char[m_formula.Length];
                charArray = this.m_formula.ToCharArray();

                for (int ii = 0; ii <= charArray.Length - 1; ii++)
                {
                    if (Tool.IsNumberic(charArray[ii]))
                    {
                        int temp = int.Parse(charArray[ii].ToString());
                        if (this.m_operation == '~')
                            m_numA = (m_numA == 0) ? temp : (temp + 10 * m_numA);
                        else
                            m_numB = (m_numB == 0) ? temp : (temp + 10 * m_numB);
                    }
                    else
                    {
                        if (this.m_operation != '~')
                        {
                            Operation operation = new Operation(m_numA, m_numB, this.m_operation);
                            m_numA = operation.Display();
                            this.m_operation = '~';
                            this.m_numB = 0;
                        }
                        try
                        {
                            this.m_operation = char.Parse(charArray[ii].ToString());
                        }
                        catch
                        { }
                    }
                }

                if (this.m_operation != '~')
                {
                    Operation operation = new Operation(m_numA, m_numB, this.m_operation);
                    m_numA = operation.Display();
                    this.m_operation = '~';
                    this.m_numB = 0;
                }

                return m_numA;
            }
        }

        class Operation
        {
            int operNum1 = 0;
            int operNum2 = 0;
            char operation;

            const char add = '+';
            const char minus = '-';
            const char multiply = '*';
            const char divide = '/';

            public Operation(int numA, int numB, char oper)
            {
                this.operNum1 = numA;
                this.operNum2 = numB;
                this.operation = oper;
            }

            public int Add()
            {
                return operNum1 + operNum2;
            }

            public int Minus()
            {
                return operNum1 - operNum2;
            }

            public int Multiply()
            {
                return operNum1 * operNum2;
            }

            public object Divide()
            {
                if (operNum2 == 0)
                {
                    return null;
                }
                else
                {
                    return (object)(operNum1 / operNum2);
                }
            }

            public int Display()
            {
                int m_numA = 0;
                switch (this.operation)
                {
                    case add:
                        m_numA = this.Add();
                        break;
                    case minus:
                        m_numA = this.Minus();
                        break;
                    case multiply:
                        m_numA = this.Multiply();
                        break;
                    case divide:
                        if (operNum2 != 0)
                            m_numA = (int)this.Divide();
                        break;
                }
                return m_numA;
            }
        }
    }
}
