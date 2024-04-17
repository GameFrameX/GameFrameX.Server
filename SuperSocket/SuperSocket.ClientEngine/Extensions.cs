using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SuperSocket.ClientEngine
{
	// Token: 0x02000006 RID: 6
	public static class Extensions
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002BDC File Offset: 0x00000DDC
		public static int IndexOf<T>(this IList<T> source, T target, int pos, int length) where T : IEquatable<T>
		{
			for (int i = pos; i < pos + length; i++)
			{
				T t = source[i];
				if (t.Equals(target))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002C12 File Offset: 0x00000E12
		public static int? SearchMark<T>(this IList<T> source, T[] mark) where T : IEquatable<T>
		{
			return source.SearchMark(0, source.Count, mark, 0);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002C23 File Offset: 0x00000E23
		public static int? SearchMark<T>(this IList<T> source, int offset, int length, T[] mark) where T : IEquatable<T>
		{
			return source.SearchMark(offset, length, mark, 0);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002C30 File Offset: 0x00000E30
		public static int? SearchMark<T>(this IList<T> source, int offset, int length, T[] mark, int matched) where T : IEquatable<T>
		{
			int num = offset;
			int num2 = offset + length - 1;
			int num3 = matched;
			if (matched > 0)
			{
				int i = num3;
				while (i < mark.Length)
				{
					T t = source[num++];
					if (!t.Equals(mark[i]))
					{
						break;
					}
					num3++;
					if (num > num2)
					{
						if (num3 == mark.Length)
						{
							return new int?(offset);
						}
						return new int?(0 - num3);
					}
					else
					{
						i++;
					}
				}
				if (num3 == mark.Length)
				{
					return new int?(offset);
				}
				num = offset;
				num3 = 0;
			}
			for (;;)
			{
				num = source.IndexOf(mark[num3], num, length - num + offset);
				if (num < 0)
				{
					break;
				}
				num3++;
				for (int j = num3; j < mark.Length; j++)
				{
					int num4 = num + j;
					if (num4 > num2)
					{
						goto Block_7;
					}
					T t = source[num4];
					if (!t.Equals(mark[j]))
					{
						break;
					}
					num3++;
				}
				if (num3 == mark.Length)
				{
					goto Block_9;
				}
				num++;
				num3 = 0;
			}
			return null;
			Block_7:
			return new int?(0 - num3);
			Block_9:
			return new int?(num);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002D38 File Offset: 0x00000F38
		public static int SearchMark<T>(this IList<T> source, int offset, int length, SearchMarkState<T> searchState) where T : IEquatable<T>
		{
			int? num = source.SearchMark(offset, length, searchState.Mark, searchState.Matched);
			if (num == null)
			{
				searchState.Matched = 0;
				return -1;
			}
			if (num.Value < 0)
			{
				searchState.Matched = 0 - num.Value;
				return -1;
			}
			searchState.Matched = 0;
			return num.Value;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002D95 File Offset: 0x00000F95
		public static int StartsWith<T>(this IList<T> source, T[] mark) where T : IEquatable<T>
		{
			return source.StartsWith(0, source.Count, mark);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002DA8 File Offset: 0x00000FA8
		public static int StartsWith<T>(this IList<T> source, int offset, int length, T[] mark) where T : IEquatable<T>
		{
			int num = offset + length - 1;
			for (int i = 0; i < mark.Length; i++)
			{
				int num2 = offset + i;
				if (num2 > num)
				{
					return i;
				}
				T t = source[num2];
				if (!t.Equals(mark[i]))
				{
					return -1;
				}
			}
			return mark.Length;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002DF9 File Offset: 0x00000FF9
		public static bool EndsWith<T>(this IList<T> source, T[] mark) where T : IEquatable<T>
		{
			return source.EndsWith(0, source.Count, mark);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002E0C File Offset: 0x0000100C
		public static bool EndsWith<T>(this IList<T> source, int offset, int length, T[] mark) where T : IEquatable<T>
		{
			if (mark.Length > length)
			{
				return false;
			}
			for (int i = 0; i < Math.Min(length, mark.Length); i++)
			{
				if (!mark[i].Equals(source[offset + length - mark.Length + i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002E60 File Offset: 0x00001060
		public static T[] CloneRange<T>(this T[] source, int offset, int length)
		{
			T[] array = new T[length];
			Array.Copy(source, offset, array, 0, length);
			return array;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002E80 File Offset: 0x00001080
		public static T[] RandomOrder<T>(this T[] source)
		{
			int num = source.Length / 2;
			for (int i = 0; i < num; i++)
			{
				int num2 = Extensions.m_Random.Next(0, source.Length - 1);
				int num3 = Extensions.m_Random.Next(0, source.Length - 1);
				if (num2 != num3)
				{
					T t = source[num3];
					source[num3] = source[num2];
					source[num2] = t;
				}
			}
			return source;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002EE6 File Offset: 0x000010E6
		public static string GetValue(this NameValueCollection collection, string key)
		{
			return collection.GetValue(key, string.Empty);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002EF4 File Offset: 0x000010F4
		public static string GetValue(this NameValueCollection collection, string key, string defaultValue)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (collection == null)
			{
				return defaultValue;
			}
			string text = collection[key];
			if (text == null)
			{
				return defaultValue;
			}
			return text;
		}

		// Token: 0x0400000A RID: 10
		private static Random m_Random = new Random();
	}
}
