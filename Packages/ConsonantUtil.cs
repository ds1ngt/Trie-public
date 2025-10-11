using System.Linq;
using System.Text;

namespace NGT.Trie
{
	public abstract class ConsonantUtil
	{
		private static readonly (int, int)[] NumberRange = new[]
		{
			// 0 ~ 9
			(0x30, 0x39),
		};
		private static readonly (int, int)[] EngRange = new[]
		{
			// a z
			(0x61, 0x7A),
			// A Z
			(0x41, 0x5A),
		};
		private static readonly (int, char)[] KorMap = new[]
		{
			// 가 까 나 다 따
			(0xAC00, 'ㄱ'), (0xAE4C, 'ㄲ'), (0xB098, 'ㄴ'), (0xB2E4, 'ㄷ'), (0xB530, 'ㄸ'),
			// 라 마 바 빠 사
			(0xB77C, 'ㄹ'), (0xB9C8, 'ㅁ'), (0xBC14, 'ㅂ'), (0xBE60, 'ㅃ'), (0xC0AC, 'ㅅ'),
			// 싸 아 자 짜 차
			(0xC2F8, 'ㅆ'), (0xC544, 'ㅇ'), (0xC790, 'ㅈ'), (0xC9DC, 'ㅉ'), (0xCC28, 'ㅊ'),
			// 카 타 파 하 힣
			(0xCE74, 'ㅋ'), (0xD0C0, 'ㅌ'), (0xD30C, 'ㅍ'), (0xD558, 'ㅎ'), (0xD7A4, 'ㅎ'),
		};

		private static readonly int[] SpecialChars = new[]
		{
			0x2A, // *
		};

		private static readonly StringBuilder Sb = new();
		private static readonly char InvalidConsonant = '_';

		/// <summary>
		/// 초성 변환 (한글이 있으면 초성으로 변환, 그 외의 문자는 그대로 반환)
		/// </summary>
		/// <param name="text">원문 문자열</param>
		/// <returns>초성 변환된 문자열</returns>
		public static string ConvertToConsonant(string text)
		{
			var unicodes = ConvertToUnicode(text.ToLower());
			return ConvertToConsonant(unicodes);
		}
		private static int[] ConvertToUnicode(string text)
		{
			var result = new int[text.Length];
			for (int i = 0; i < text.Length; ++i)
				result[i] = char.ConvertToUtf32(text, i);
			return result;
		}

		private static string ConvertToConsonant(int[] unicodes)
		{
			Sb.Clear();
			foreach (var unicode in unicodes)
				Sb.Append(GetConsonant(unicode));
			return Sb.ToString();
		}

		/// <summary>
		/// 입력된 문자열이 유효한 경우 초성 변환 (숫자, 알파벳, 한글 및 특수문자 '*')
		/// </summary>
		/// <param name="text">원문 문자열</param>
		/// <param name="result">초성 변환 된 문자열</param>
		/// <returns>모든 문자가 유효한 문자면 true</returns>
		public static bool TryConvertToConsonant(string text, out string result)
		{
			var unicodes = ConvertToUnicode(text.ToLower());
			return TryConvertToConsonant(unicodes, out result);
		}
		private static bool TryConvertToConsonant(int[] unicodes, out string result)
		{
			result = string.Empty;

			Sb.Clear();
			foreach (var unicode in unicodes)
			{
				var consonant = GetConsonant(unicode);
				if (consonant == InvalidConsonant)
					return false;

				Sb.Append(consonant);
			}

			result = Sb.ToString();
			return true;
		}
		private static char GetConsonant(int unicode)
		{
			if (IsNumber(unicode))
				return char.ConvertFromUtf32(unicode)[0];
			if (IsEnglish(unicode))
				return char.ConvertFromUtf32(unicode)[0];
			if (IsKorean(unicode))
				return GetConsonantKor(unicode);
			if (IsSpecialChar(unicode))
				return char.ConvertFromUtf32(unicode)[0];
			return InvalidConsonant;
		}

		private static bool IsNumber(int unicode) => unicode >= NumberRange[0].Item1 && unicode <= NumberRange[0].Item2;
		private static bool IsEnglish(int unicode)
		{
			foreach (var range in EngRange)
				if (unicode >= range.Item1 && unicode <= range.Item2)
					return true;
			return false;
		}
		private static bool IsKorean(int unicode) => unicode >= KorMap[0].Item1 &&
													 unicode <= KorMap[^1].Item1;

		private static bool IsSpecialChar(int unicode) => SpecialChars.Contains(unicode);
		private static char GetConsonantKor(int unicode)
		{
			for (int i = 1; i < KorMap.Length; ++i)
				if (KorMap[i - 1].Item1 <= unicode && unicode < KorMap[i].Item1)
					return KorMap[i - 1].Item2;

			return '-';
		}
	}
}
