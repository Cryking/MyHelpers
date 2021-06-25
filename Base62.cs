using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// create by gw 2020.11.18
    /// 字符串base62编码
    /// </summary>
    public sealed class Base62
    {
        private static byte xtag = (int)'x';//0x78; //'x';
        private static String encd = "-enc";
        private static String decd = "-dec";
        private static String debg = "-v";
        private static String cntv = "-n"; //- numeric conversion
        private static byte[] b62x = {(int)'0', (int)'1', (int)'2', (int)'3',(int) '4',(int) '5', (int)'6', (int)'7', (int)'8', (int)'9',
            (int)'A', (int)'B',(int)'C',(int) 'D',(int) 'E',(int) 'F',(int)'G',(int) 'H',(int) 'I',(int) 'J',(int) 'K',(int) 'L',(int) 'M',(int) 'N',
            (int)'O', (int)'P',(int)'Q',(int) 'R',(int) 'S',(int) 'T',(int)'U',(int) 'V',(int) 'W',(int) 'X',(int) 'Y',(int) 'Z',(int) 'a',(int) 'b',
            (int)'c', (int)'d',(int)'e',(int) 'f',(int) 'g',(int) 'h',(int)'i',(int) 'j',(int) 'k',(int) 'l',(int) 'm',(int) 'n',(int) 'o',(int) 'p',
            (int)'q', (int)'r',(int)'s',(int) 't',(int) 'u',(int) 'v',(int)'w',(int) 'y',(int) 'z',(int) '1',(int) '2',(int) '3',(int) 'x'};
        private static int bpos = 60; //- 0-60 chars
        private static int xpos = 64; //- b62x[64] = 'x'
        private static int ascmax = 127;
        private static byte[] asclist = {
           (int)'4',(int) '5',(int) '6',(int)'7',(int) '8',(int) '9',(int) '0',
           (int)'A',(int) 'B',(int) 'C',(int)'D',(int) 'E',(int) 'F',(int) 'G',
           (int)'H',(int) 'I',(int) 'J',(int)'K',(int) 'L',(int) 'M',(int) 'N',
           (int)'O',(int) 'P',(int) 'Q',(int)'R',(int) 'S',(int) 'T',(int) 'U',
           (int)'V',(int) 'W',(int) 'X',(int)'Y',(int) 'Z',(int) 'a',(int) 'b',
           (int)'c',(int) 'd',(int) 'e',(int)'f',(int) 'g',(int) 'h',(int) 'i',
           (int)'j',(int) 'k',(int) 'l',(int)'m',(int) 'n',(int) 'o',(int) 'p',
           (int)'q',(int) 'r',(int) 's',(int)'t',(int) 'u',(int) 'v',(int) 'w',
           (int)'y',(int) 'z'};
        private static int max_safe_base = 36; //- 17:56 14 February 2017
        //private static double ver = 0.80;
        private static int[] ascidx = new int[ascmax + 1];
        private static byte[] ascrlist = new byte[ascmax + 1];
        //- variables
        //private bool isdebug = false;
        //private int i = 0;
        //private int codetype = 0; //- 0:encode, 1:decode
        //private int[] rb62x = new int[] { };

        //- contructors
        //- @todo
        //- methods
        //- encode, ibase=2,8,10,16,32...63
        public static String Encode(String input, int ibase)
        {
            var osb = new StringBuilder();
            if (string.IsNullOrEmpty(input))
            {
                return osb.ToString();
            }
            int codetype = 0;
            byte xtag = Base62.xtag;
            byte[] b62x = Base62.b62x;
            byte[] asclist = Base62.asclist;
            int bpos = Base62.bpos;
            int xpos = Base62.xpos;
            int ascmax = Base62.ascmax;
            int[] rb62x = Base62.fillRb62x(b62x, bpos, xpos);
            bool isnum = false;
            if (ibase > 0)
            {
                isnum = true;
            }

            if (isnum)
            {
                //- numeric conversion
                long num_input = Base62.xx2dec(input, ibase, rb62x);
                int obase = xpos;
                osb.Append(Base62.dec2xx(num_input, obase, b62x));
            }
            else
            {
                //- string encoding
                var isasc = false;
                byte[] inputArr = Encoding.UTF8.GetBytes(input);//input.GetBytes(); // new byte[]{-112, 25, 66, -12}; //StandardCharsets.UTF_8
                int inputlen = inputArr.Length;
                var setResult = Base62.setAscii(codetype, inputArr, ascidx, ascmax, asclist, ascrlist);
                isasc = (Boolean)setResult["isasc"];
                ascidx = (int[])setResult["ascidx"];
                ascrlist = (byte[])setResult["ascrlist"];
                byte[] op = new byte[inputlen * 2]; //- extend to 3/2 or 2 theoritical maxium length of base62x
                int i = 0;
                int m = 0;
                if (isasc)
                {
                    //- ascii
                    byte b = 0;
                    do
                    {
                        b = inputArr[i];
                        if (ascidx[b] > -1)
                        {
                            op[m] = xtag;
                            op[++m] = (byte)ascidx[b];
                        }
                        else if (b == xtag)
                        {
                            op[m] = xtag;
                            op[++m] = xtag;
                        }
                        else
                        {
                            op[m] = b;
                        }
                        m++;
                    }
                    while (++i < inputlen);
                    op[m++] = xtag; //- asctype has a tag 'x' appended
                }
                else
                {
                    //- non-ascii
                    int c0 = 0;
                    int c1 = 0;
                    int c2 = 0;
                    int c3 = 0;
                    i = 0;
                    int remaini = 0;
                    int tmpi = 0;
                    int tmpj = 0;
                    int tmpk = 0;
                    do
                    {
                        remaini = inputlen - i;
                        tmpi = (int)inputArr[i];
                        tmpi = tmpi < 0 ? (tmpi & 0xff) : tmpi; //- for minus byte
                        switch (remaini)
                        {
                            case 1:
                                c0 = tmpi >> 2;
                                c1 = ((tmpi << 6) & 0xff) >> 6;
                                if (c0 > bpos)
                                {
                                    op[m] = xtag;
                                    op[++m] = b62x[c0];
                                }
                                else
                                {
                                    op[m] = b62x[c0];
                                }
                                if (c1 > bpos)
                                {
                                    op[++m] = xtag;
                                    op[++m] = b62x[c1];
                                }
                                else
                                {
                                    op[++m] = b62x[c1];
                                }
                                break;
                            case 2:
                                tmpj = (int)inputArr[i + 1];
                                tmpj = tmpj < 0 ? (tmpj & 0xff) : tmpj;
                                c0 = tmpi >> 2;
                                c1 = (((tmpi << 6) & 0xff) >> 2) | (tmpj >> 4);
                                c2 = ((tmpj << 4) & 0xff) >> 4;
                                if (c0 > bpos)
                                {
                                    op[m] = xtag;
                                    op[++m] = b62x[c0];
                                }
                                else
                                {
                                    op[m] = b62x[c0];
                                }
                                if (c1 > bpos)
                                {
                                    op[++m] = xtag;
                                    op[++m] = b62x[c1];
                                }
                                else
                                {
                                    op[++m] = b62x[c1];
                                }
                                if (c2 > bpos)
                                {
                                    op[++m] = xtag;
                                    op[++m] = b62x[c2];
                                }
                                else
                                {
                                    op[++m] = b62x[c2];
                                }
                                i += 1;
                                break;
                            default:
                                tmpj = (int)inputArr[i + 1];
                                tmpj = tmpj < 0 ? (tmpj & 0xff) : tmpj;
                                tmpk = (int)inputArr[i + 2];
                                tmpk = tmpk < 0 ? (tmpk & 0xff) : tmpk;
                                c0 = tmpi >> 2;
                                c1 = (((tmpi << 6) & 0xff) >> 2) | (tmpj >> 4);
                                c2 = (((tmpj << 4) & 0xff) >> 2) | (tmpk >> 6);
                                c3 = ((tmpk << 2) & 0xff) >> 2;
                                if (c0 > bpos)
                                {
                                    op[m] = xtag;
                                    op[++m] = b62x[c0];
                                }
                                else
                                {
                                    op[m] = b62x[c0];
                                }
                                if (c1 > bpos)
                                {
                                    op[++m] = xtag;
                                    op[++m] = b62x[c1];
                                }
                                else
                                {
                                    op[++m] = b62x[c1];
                                }
                                if (c2 > bpos)
                                {
                                    op[++m] = xtag;
                                    op[++m] = b62x[c2];
                                }
                                else
                                {
                                    op[++m] = b62x[c2];
                                }
                                if (c3 > bpos)
                                {
                                    op[++m] = xtag;
                                    op[++m] = b62x[c3];
                                }
                                else
                                {
                                    op[++m] = b62x[c3];
                                }
                                i += 2;
                                break;
                        }
                        m++;
                    }
                    while (++i < inputlen);
                }
                byte[] op2 = new byte[m];
                Array.Copy(op, 0, op2, 0, m);
                //System.arraycopy(op, 0, op2, 0, m);
                osb.Append(Encoding.UTF8.GetString(op2));
            }
            return osb.ToString();
        }

        //- decode, obase=2,8,10,16,32...63
        //public static String decode(String input, int obase)
        //{

        //    StringBuffer osb = new StringBuffer();
        //    if (input == null || input.equals(""))
        //    {
        //        return osb.toString();
        //    }
        //    int codetype = 1;
        //    byte xtag = Base62.xtag;
        //    byte[] b62x = Base62.b62x;
        //    byte[] asclist = Base62.asclist;
        //    int bpos = Base62.bpos;
        //    int xpos = Base62.xpos;
        //    int ascmax = Base62.ascmax;
        //    int[] rb62x = Base62.fillRb62x(b62x, bpos, xpos);
        //    boolean isnum = false;
        //    if (obase > 0)
        //    {
        //        isnum = true;
        //    }

        //    if (isnum)
        //    {
        //        //- numeric conversion
        //        int ibase = xpos;
        //        long num_input = Base62.xx2dec(input, ibase, rb62x);
        //        osb.append(Base62.dec2xx(num_input, obase, b62x));
        //        //-  why a mediate number format is needed?
        //    }
        //    else
        //    {
        //        //- string decoding
        //        boolean isasc = false;
        //        byte[] inputArr = input.getBytes();
        //        int inputlen = inputArr.length;
        //        HashMap setResult = Base62.setAscii(codetype, inputArr, ascidx, ascmax, asclist, ascrlist);
        //        isasc = (Boolean)setResult.get("isasc");
        //        ascidx = (int[])setResult.get("ascidx");
        //        ascrlist = (byte[])setResult.get("ascrlist");
        //        byte[] op = new byte[inputlen]; //- shrink to 2/3 or 1/2 in maxium
        //        int i = 0;
        //        int m = 0;
        //        if (isasc)
        //        {
        //            //- ascii
        //            byte b = 0;
        //            inputlen--;
        //            do
        //            {
        //                b = inputArr[i];
        //                if (b == xtag)
        //                {
        //                    if (inputArr[i + 1] == xtag)
        //                    {
        //                        op[m] = xtag;
        //                        i++;
        //                    }
        //                    else
        //                    {
        //                        op[m] = ascrlist[inputArr[++i]];
        //                    }
        //                }
        //                else
        //                {
        //                    op[m] = b;
        //                }
        //                m++;
        //            }
        //            while (++i < inputlen);
        //        }
        //        else
        //        {
        //            //- non-ascii
        //            int c0 = 0;
        //            int c1 = 0;
        //            int c2 = 0;
        //            int remaini = 0;
        //            int maxidx = inputlen - 1;
        //            int last8 = inputlen - 8; //- avoid outofArrayIndex
        //            int[] tmpArr = new int[4];
        //            int[] bint = new int[xpos];
        //            bint[49] = 1;
        //            bint[50] = 2;
        //            bint[51] = 3; //- array('1'=>1, '2'=>2, '3'=>3);
        //            do
        //            {
        //                remaini = inputlen - i;
        //                tmpArr = new int[4];
        //                switch (remaini)
        //                {
        //                    case 1:
        //                        System.out.println("Base62x.decode: illegal base62x input:[" + input + "]. 1608091042.");
        //                        break;
        //                    case 2:
        //                        if (inputArr[i] == xtag)
        //                        {
        //                            tmpArr[0] = bpos + bint[inputArr[++i]];
        //                        }
        //                        else
        //                        {
        //                            tmpArr[0] = rb62x[inputArr[i]];
        //                        }
        //                        if (i == maxidx)
        //                        {
        //                            c0 = (tmpArr[0] << 2);
        //                            op[m] = (byte)c0;
        //                        }
        //                        else
        //                        {
        //                            if (inputArr[++i] == xtag)
        //                            {
        //                                tmpArr[1] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[1] = rb62x[inputArr[i]];
        //                            }
        //                            c0 = (tmpArr[0] << 2) | tmpArr[1];
        //                            op[m] = (byte)c0;
        //                        }
        //                        break;
        //                    case 3:
        //                        if (inputArr[i] == xtag)
        //                        {
        //                            tmpArr[0] = bpos + bint[inputArr[++i]];
        //                        }
        //                        else
        //                        {
        //                            tmpArr[0] = rb62x[inputArr[i]];
        //                        }
        //                        if (inputArr[++i] == xtag)
        //                        {
        //                            tmpArr[1] = bpos + bint[inputArr[++i]];
        //                        }
        //                        else
        //                        {
        //                            tmpArr[1] = rb62x[inputArr[i]];
        //                        }
        //                        if (i == maxidx)
        //                        {
        //                            c0 = (tmpArr[0] << 2) | tmpArr[1];
        //                            op[m] = (byte)c0;
        //                        }
        //                        else
        //                        {
        //                            if (inputArr[++i] == xtag)
        //                            {
        //                                tmpArr[2] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[2] = rb62x[inputArr[i]];
        //                            }
        //                            c0 = (tmpArr[0] << 2) | (tmpArr[1] >> 4);
        //                            c1 = ((tmpArr[1] << 4) & 0xf0) | tmpArr[2];
        //                            op[m] = (byte)c0;
        //                            op[++m] = (byte)c1;
        //                        }
        //                        break;
        //                    default:
        //                        if (i < last8)
        //                        {
        //                            if (inputArr[i] == xtag)
        //                            {
        //                                tmpArr[0] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[0] = rb62x[inputArr[i]];
        //                            }
        //                            if (inputArr[++i] == xtag)
        //                            {
        //                                tmpArr[1] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[1] = rb62x[inputArr[i]];
        //                            }
        //                            if (inputArr[++i] == xtag)
        //                            {
        //                                tmpArr[2] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[2] = rb62x[inputArr[i]];
        //                            }
        //                            if (inputArr[++i] == xtag)
        //                            {
        //                                tmpArr[3] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[3] = rb62x[inputArr[i]];
        //                            }
        //                            c0 = (tmpArr[0] << 2) | (tmpArr[1] >> 4);
        //                            c1 = ((tmpArr[1] << 4) & 0xf0) | (tmpArr[2] >> 2);
        //                            c2 = ((tmpArr[2] << 6) & 0xff) | tmpArr[3];
        //                            op[m] = (byte)c0;
        //                            op[++m] = (byte)c1;
        //                            op[++m] = (byte)c2;
        //                        }
        //                        else
        //                        {
        //                            if (inputArr[i] == xtag)
        //                            {
        //                                tmpArr[0] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[0] = rb62x[inputArr[i]];
        //                            }
        //                            if (inputArr[++i] == xtag)
        //                            {
        //                                tmpArr[1] = bpos + bint[inputArr[++i]];
        //                            }
        //                            else
        //                            {
        //                                tmpArr[1] = rb62x[inputArr[i]];
        //                            }
        //                            if (i == maxidx)
        //                            {
        //                                c0 = (tmpArr[0] << 2) | tmpArr[1];
        //                                op[m] = (byte)c0;
        //                            }
        //                            else
        //                            {
        //                                if (inputArr[++i] == xtag)
        //                                {
        //                                    tmpArr[2] = bpos + bint[inputArr[++i]];
        //                                }
        //                                else
        //                                {
        //                                    tmpArr[2] = rb62x[inputArr[i]];
        //                                }
        //                                if (i == maxidx)
        //                                {
        //                                    c0 = (tmpArr[0] << 2) | (tmpArr[1] >> 4);
        //                                    c1 = ((tmpArr[1] << 4) & 0xf0) | tmpArr[2];
        //                                    op[m] = (byte)c0;
        //                                    op[++m] = (byte)c1;
        //                                }
        //                                else
        //                                {
        //                                    if (inputArr[++i] == xtag)
        //                                    {
        //                                        tmpArr[3] = bpos + bint[inputArr[++i]];
        //                                    }
        //                                    else
        //                                    {
        //                                        tmpArr[3] = rb62x[inputArr[i]];
        //                                    }
        //                                    c0 = (tmpArr[0] << 2) | (tmpArr[1] >> 4);
        //                                    c1 = ((tmpArr[1] << 4) & 0xf0) | (tmpArr[2] >> 2);
        //                                    c2 = ((tmpArr[2] << 6) & 0xff) | tmpArr[3];
        //                                    op[m] = (byte)c0;
        //                                    op[++m] = (byte)c1;
        //                                    op[++m] = (byte)c2;
        //                                }
        //                            }
        //                        }
        //                }
        //                m++;
        //            }
        //            while (++i < inputlen);
        //        }
        //        byte[] op2 = new byte[m];
        //        System.arraycopy(op, 0, op2, 0, m);
        //        osb.append(new String(op2)); //, StandardCharsets.UTF_8
        //    }
        //    return osb.toString();

        //}

        //- encode in default
        public static String Encode(String input)
        {
            return Encode(input, 0);
        }

        ////- decode in default
        //public static String decode(String input)
        //{
        //    return decode(input, 0);
        //}

        //- inner facilites
        //-
        private static int[] fillRb62x(byte[] b62x, int bpos, int xpos)
        {
            int[] rb62x = new int[xpos * 2]; //{};
            for (int i = 0; i <= xpos; i++)
            {
                if (i > bpos && i < xpos)
                {
                    //- omit x1, x2, x3
                }
                else
                {
                    rb62x[b62x[i]] = i;
                }
            }
            return rb62x;
        }

        //-
        private static Hashtable setAscii(int codetype, byte[] inputArr, int[] ascidx,
                                        int ascmax, byte[] asclist, byte[] ascrlist)
        {

            var rethm = new Hashtable();
            bool isasc = false;
            char xtag = (char)Base62.xtag;
            int inputlen = inputArr.Length;

            if (codetype == 0 && inputArr[0] <= ascmax)
            {
                isasc = true;
                int tmpi = 0;
                for (int i = 0; i < inputlen; i++)
                {
                    tmpi = (int)inputArr[i];
                    if (tmpi < 0 || tmpi > ascmax
                            || (tmpi > 16 && tmpi < 21) //- DC1-4
                            || (tmpi > 27 && tmpi < 32) //-  FC, GS, RS, US
                            )
                    {
                        isasc = false;
                        break;
                    }
                }
            }
            else if (codetype == 1 && inputArr[inputlen - 1] == xtag)
            {
                isasc = true;
            }
            rethm.Add("isasc", isasc);
            if (isasc)
            {
                int i = 0;
                for (i = 0; i <= ascmax; i++)
                {
                    ascidx[i] = -1;
                }
                int idxi = 0;
                int[] starti = new int[] { 0, 21, 32, 58, 91, 123 }; //- 0, NAK, ' ', ':', '[', '{'
                int[] endi = new int[] { 17, 28, 48, 65, 97, ascmax + 1 }; //- 17, FS, '/', '@', '`'
                int ilen = starti.Length;
                for (int n = 0; n < ilen; n++)
                {
                    for (i = starti[n]; i < endi[n]; i++)
                    {
                        ascidx[i] = asclist[idxi];
                        ascrlist[asclist[idxi]] = (byte)i;
                        idxi++;
                    }
                }
            }
            rethm.Add("ascidx", ascidx);
            rethm.Add("ascrlist", ascrlist);
            return rethm;
        }

        //-
        private static long xx2dec(String input, int ibase, int[] rb62x)
        {
            long rtn = 0L;
            //- @todo
            int obase = 10;
            char xtag = (char)Base62.xtag;
            int bpos = Base62.bpos;
            int xpos = Base62.xpos;
            int max_safe_base = Base62.max_safe_base;
            if (ibase < 2 || ibase > xpos)
            {
                Console.WriteLine("Base62x.xx2dec: illegal ibase:[" + ibase + "]");
            }
            else if (ibase <= max_safe_base && obase <= max_safe_base)
            {
                rtn = long.Parse(input);
                //long.Parse(long.ToString(Long.parseLong(input, ibase), obase));
            }
            else
            {
                var iarr = input.ToCharArray().Reverse().ToArray();
                //char[] iarr = (new StringBuilder(input).reverse().toString())
                //        .toCharArray();
                int arrlen = iarr.Length;
                int xnum = 0;
                int tmpi = 0;
                //java.util.Collections.reverse(iarr);
                for (int i = 0; i < arrlen; i++)
                {
                    if (i + 1 < arrlen && iarr[i + 1] == xtag)
                    {
                        tmpi = bpos + rb62x[iarr[i]];
                        xnum++;
                        i++;
                    }
                    else
                    {
                        tmpi = rb62x[iarr[i]];
                    }
                    if (tmpi >= ibase)
                    {
                        //Console.WriteLine("Base62x::xx2dec: found out of radix:" + tmpi + " for base:" + ibase);
                        tmpi = ibase - 1;
                    }
                    rtn += tmpi * (long)Math.Pow((double)ibase, (double)(i - xnum));
                }
                //- oversize check?
                //- @todo
            }
            //System.out.print("static xx2dec: in:["+input+"] ibase:["+ibase+"] rtn:["+rtn+"] in 10.");
            return rtn;
        }

        //-
        private static String dec2xx(long num_input, int obase, byte[] b62x)
        {
            String rtn = "";
            //- @todo
            int ibase = 10;
            char xtag = (char)Base62.xtag;
            int bpos = Base62.bpos;
            int xpos = Base62.xpos;
            int max_safe_base = Base62.max_safe_base;
            String inputs = num_input.ToString();//Long.toString(num_input);
            if (ibase < 2 || ibase > xpos)
            {
                Console.WriteLine("Base62x.xx2dec: illegal ibase:[" + ibase + "]");
            }
            else if (obase <= max_safe_base && ibase <= max_safe_base)
            {
                rtn = inputs;//Long.toString(Long.parseLong(inputs, ibase), obase);
            }
            else
            {
                int i = 0;
                int b = 0;
                int inputlen = inputs.Length;
                int outlen = (int)(inputlen * Math.Log(ibase) / Math.Log(obase)) + 1;
                char[] oarr = new char[outlen]; //- why threefold?
                while (num_input >= obase)
                {
                    b = (int)(num_input % obase);
                    num_input = (long)Math.Floor((double)num_input / obase);
                    if (b <= bpos)
                    {
                        oarr[i++] = (char)b62x[b];
                    }
                    else
                    {
                        oarr[i++] = (char)b62x[b - bpos];
                        oarr[i++] = xtag;
                    }
                }
                b = (int)num_input;
                if (b > 0)
                {
                    if (b <= bpos)
                    {
                        oarr[i++] = (char)b62x[b];
                    }
                    else
                    {
                        oarr[i++] = (char)b62x[b - bpos];
                        oarr[i++] = xtag;
                    }
                }
                //Collections.reverse(oarr);
                //rtn = oarr.join();
                rtn = oarr.ToArray().Reverse().ToArray().ToString();
                //new StringBuilder(new String(oarr)).reverse().toString();
            }
            return rtn;
        }

        //- fix variable length of encoded string
        //- Dec 01, 2016
        private static byte[] _decodeByLength(int[] tmpArr, byte[] op, int m)
        {
            byte[] rtn = op;
            //- @todo replace ArrayIndexOutOfBoundsException and variable tmpArr in decode
            rtn[m++] = (byte)m; //- ?
            return rtn;
        }
    }
}
