using System;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Draco.DB.QuickDataBase.Utility
{
	/// <summary>
	/// 分析数据库连接字符串
    /// 注意：不同的数据库类型，键值是不同的，请根据不同的数据类型进行取值
	/// </summary>
    public abstract partial class ConnectionStringAnalyzer
	{
		/// <summary>
		/// 连接串最大长度，超过这个长度的串不进行处理
		/// </summary>
		public const int ConnStrMaxLength = 500;

		/// <summary>
		/// 分析连接字符串，返回 名称/值对的哈西表
		/// </summary>
		/// <param name="ConnString"></param>
		/// <returns></returns>
		public static Hashtable GetConnctionStringAtt(string ConnString)
		{
			Hashtable hsb = new Hashtable(10,StringComparer.OrdinalIgnoreCase);

            if (String.IsNullOrEmpty(ConnString) || ConnString.Length > ConnStrMaxLength)
            {
                return hsb;
            }

			string key;
			char[]valuebuf = new char[ConnString.Length];
			int vallen;
			bool isempty;
			int current = 0;
			int connstrLen = ConnString.Length;
			char[] connAry = ConnString.ToCharArray();
			while (current < connstrLen)
			{
				current = GetKeyValuePair(connAry,current,out key,valuebuf,out vallen,out isempty);
				string val = Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(valuebuf, 0, vallen));
				if(!hsb.ContainsKey(key))
					hsb.Add(key,val);
				else
					hsb[key] = val;
			}

			return hsb;
		}

        /// <summary>
		/// 从连接串中的currentPosition位置开始找到第一个名称/值对
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="currentPosition"></param>
		/// <param name="key"></param>
		/// <param name="valuebuf"></param>
		/// <param name="vallength"></param>
		/// <param name="isempty"></param>
		/// <returns></returns>
		private static int GetKeyValuePair(char[] connectionString, int currentPosition, out string key, char[] valuebuf, out int vallength, out bool isempty)
		{
			PARSERSTATE parserstate1 = PARSERSTATE.NothingYet;
			int num1 = 0;
			int num2 = currentPosition;
			key = null;
			vallength = -1;
			isempty = false;
			char ch1 = '\0';
			while (currentPosition < connectionString.Length)
			{
				ch1 = connectionString[currentPosition];
				switch (parserstate1)
				{
					case PARSERSTATE.NothingYet:
						if ((';' != ch1) && !char.IsWhiteSpace(ch1))
						{
							num2 = currentPosition;
							if (ch1 != '\0')
							{
								break;
							}
							parserstate1 = PARSERSTATE.NullTermination;
						}
						goto Label_01E4;

					case PARSERSTATE.Key:
						if ('=' != ch1)
						{
							goto Label_00A2;
						}
						parserstate1 = PARSERSTATE.KeyEqual;
						goto Label_01E4;

					case PARSERSTATE.KeyEqual:
						goto Label_00C0;

					case PARSERSTATE.KeyEnd:
						goto Label_00D9;

					case PARSERSTATE.UnquotedValue:
						if (char.IsWhiteSpace(ch1) || (!char.IsControl(ch1) && (';' != ch1)))
						{
							goto Label_01DC;
						}
						goto Label_0216;

					case PARSERSTATE.DoubleQuoteValue:
						if ('"' != ch1)
						{
							goto Label_0151;
						}
						parserstate1 = PARSERSTATE.DoubleQuoteValueQuote;
						goto Label_01E4;

					case PARSERSTATE.DoubleQuoteValueQuote:
						goto Label_015F;

					case PARSERSTATE.DoubleQuoteValueEnd:
						goto Label_016A;

					case PARSERSTATE.SingleQuoteValue:
						goto Label_018A;

					case PARSERSTATE.SingleQuoteValueQuote:
						goto Label_019F;

					case PARSERSTATE.SingleQuoteValueEnd:
						goto Label_01AC;

					case PARSERSTATE.NullTermination:
						goto Label_01C9;

					default:
						goto Label_01DC;
				}
				if (char.IsControl(ch1))
				{
					throw ConnectionStringSyntax(currentPosition, connectionString);
				}
				parserstate1 = PARSERSTATE.Key;
				num1 = 0;
				goto Label_01DC;
			Label_00A2:
				if (char.IsWhiteSpace(ch1) || !char.IsControl(ch1))
				{
					goto Label_01DC;
				}
				throw ConnectionStringSyntax(currentPosition, connectionString);
			Label_00C0:
				if ('=' == ch1)
				{
					parserstate1 = PARSERSTATE.Key;
					goto Label_01DC;
				}
				key = GetKey(valuebuf, num1);
				num1 = 0;
				parserstate1 = PARSERSTATE.KeyEnd;
			Label_00D9:
				if (char.IsWhiteSpace(ch1))
				{
					goto Label_01E4;
				}
				if ('\'' == ch1)
				{
					parserstate1 = PARSERSTATE.SingleQuoteValue;
					goto Label_01E4;
				}
				if ('"' == ch1)
				{
					parserstate1 = PARSERSTATE.DoubleQuoteValue;
					goto Label_01E4;
				}
				if ((';' == ch1) || (ch1 == '\0'))
				{
					goto Label_0216;
				}
				if (char.IsControl(ch1))
				{
					throw ConnectionStringSyntax(currentPosition, connectionString);
				}
				parserstate1 = PARSERSTATE.UnquotedValue;
				goto Label_01DC;
			Label_0151:
				if (ch1 != '\0')
				{
					goto Label_01DC;
				}
				throw ConnectionStringSyntax(currentPosition, connectionString);
			Label_015F:
				if ('"' == ch1)
				{
					parserstate1 = PARSERSTATE.DoubleQuoteValue;
					goto Label_01DC;
				}
				parserstate1 = PARSERSTATE.DoubleQuoteValueEnd;
			Label_016A:
				if (char.IsWhiteSpace(ch1))
				{
					goto Label_01E4;
				}
				if (';' == ch1)
				{
					goto Label_0216;
				}
				if (ch1 == '\0')
				{
					parserstate1 = PARSERSTATE.NullTermination;
					goto Label_01E4;
				}
				throw ConnectionStringSyntax(currentPosition, connectionString);
			Label_018A:
				if ('\'' == ch1)
				{
					parserstate1 = PARSERSTATE.SingleQuoteValueQuote;
					goto Label_01E4;
				}
				if (ch1 != '\0')
				{
					goto Label_01DC;
				}
				throw ConnectionStringSyntax(currentPosition, connectionString);
			Label_019F:
				if ('\'' == ch1)
				{
					parserstate1 = PARSERSTATE.SingleQuoteValue;
					goto Label_01DC;
				}
				parserstate1 = PARSERSTATE.SingleQuoteValueEnd;
			Label_01AC:
				if (char.IsWhiteSpace(ch1))
				{
					goto Label_01E4;
				}
				if (';' == ch1)
				{
					goto Label_0216;
				}
				if (ch1 == '\0')
				{
					parserstate1 = PARSERSTATE.NullTermination;
					goto Label_01E4;
				}
				throw ConnectionStringSyntax(currentPosition, connectionString);
			Label_01C9:
				if ((ch1 == '\0') || char.IsWhiteSpace(ch1))
				{
					goto Label_01E4;
				}
				throw ConnectionStringSyntax(num2, connectionString);
			Label_01DC:
				valuebuf[num1++] = ch1;
			Label_01E4:
				currentPosition++;
			}
			if (PARSERSTATE.KeyEqual == parserstate1)
			{
				key = GetKey(valuebuf, num1);
				num1 = 0;
			}
			if (((PARSERSTATE.Key == parserstate1) || (PARSERSTATE.DoubleQuoteValue == parserstate1)) || (PARSERSTATE.SingleQuoteValue == parserstate1))
			{
				throw ConnectionStringSyntax(num2, connectionString);
			}
			Label_0216:
				if (PARSERSTATE.UnquotedValue == parserstate1)
				{
					num1 = TrimWhiteSpace(valuebuf, num1);
					if (('\'' == valuebuf[num1 - 1]) || ('"' == valuebuf[num1 - 1]))
					{
						throw ConnectionStringSyntax(currentPosition - 1, connectionString);
					}
				}
				else if ((PARSERSTATE.KeyEqual != parserstate1) && (PARSERSTATE.KeyEnd != parserstate1))
				{
					isempty = 0 == num1;
				}
			if ((';' == ch1) && (currentPosition < connectionString.Length))
			{
				currentPosition++;
			}
			vallength = num1;
			return currentPosition;
		}
		private enum PARSERSTATE
		{
			// Fields
			DoubleQuoteValue = 6,
			DoubleQuoteValueEnd = 8,
			DoubleQuoteValueQuote = 7,
			Key = 2,
			KeyEnd = 4,
			KeyEqual = 3,
			NothingYet = 1,
			NullTermination = 12,
			SingleQuoteValue = 9,
			SingleQuoteValueEnd = 11,
			SingleQuoteValueQuote = 10,
			UnquotedValue = 5
		}

        //连接串语法错误
		private static System.FormatException ConnectionStringSyntax(int p,char[] chs)
		{
            return new FormatException("无法解析的节:[" + 
				Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(chs,p,chs.Length-p)) + 
				"]");	
		}
        //获取键
		private static string GetKey(char[] valuebuf, int bufPosition)
		{
			bufPosition = TrimWhiteSpace(valuebuf, bufPosition);
			return Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(valuebuf, 0, bufPosition));
		}
        //去除空格
		private static int TrimWhiteSpace(char[] valuebuf, int bufPosition)
		{
			while ((0 < bufPosition) && char.IsWhiteSpace(valuebuf[bufPosition - 1]))
			{
				bufPosition--;
			}
			return bufPosition;
		}
	}
}
