// ArrayPrefs2 v 1.4

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerPrefsX
{
	//**************************************************************//
	//	データの保存・取り出しをする	
	//**************************************************************//
	static private int endianDiff1;
	static private int endianDiff2;
	static private int idx;
	static private byte [] byteBlock;
	
	enum ArrayType {Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color}

	//**************************************************************//
	//	関数名　:	SetBool
	//	機能		:	Bool型の値を、キーに対応させて保存する
	//	引数		:	String name		設定するキー
	//				bool value		保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetBool ( String name, bool value)
	{
		try
		{
			PlayerPrefs.SetInt(name, value? 1 : 0);
		}
		catch
		{
			return false;
		}
		return true;
	}

	//**************************************************************//
	//	関数名　:	GetBool
	//	機能		:	キーに対応したBool型の値を取り出す
	//	引数		:	String name		設定するキー
	//	戻り値	:	取り出した値を返す（true:false）
	//**************************************************************//
	public static bool GetBool (String name)
	{
		return PlayerPrefs.GetInt(name) == 1;
	}

	//**************************************************************//
	//	関数名　:	GetBool
	//	機能		:	キーに対応したBool型の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String name			設定するキー
	//				bool defaultValue	初期化する値
	//	戻り値	:	取り出した値を返す（true:false）
	//**************************************************************//
	public static bool GetBool (String name, bool defaultValue)
	{
		return (1==PlayerPrefs.GetInt(name, defaultValue?1:0));
	}

	//**************************************************************//
	//	関数名　:	GetLong
	//	機能		:	キーに対応したlong型の値を取り出す
	//				キーが無い時は、初期化をする
	//				int型が32bit/long型が64bitなのでlowとhighに分けて格納
	//	引数		:	string key			設定するキー
	//				long defaultValue	初期化する値
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static long GetLong(string key, long defaultValue)
	{
		int lowBits, highBits;
		SplitLong(defaultValue, out lowBits, out highBits);
		lowBits = PlayerPrefs.GetInt(key+"_lowBits", lowBits);
		highBits = PlayerPrefs.GetInt(key+"_highBits", highBits);
		
		ulong ret = (uint)highBits;
		ret = (ret << 32);
		return (long)(ret | (ulong)(uint)lowBits);
	}

	//**************************************************************//
	//	関数名　:	GetLong
	//	機能		:	キーに対応したlong型の値を取り出す
	//	引数		:	string key	設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static long GetLong(string key)
	{
		int lowBits = PlayerPrefs.GetInt(key+"_lowBits");
		int highBits = PlayerPrefs.GetInt(key+"_highBits");
		
		ulong ret = (uint)highBits;
		ret = (ret << 32);
		return (long)(ret | (ulong)(uint)lowBits);
	}

	//**************************************************************//
	//	関数名　:	SplitLong
	//	機能		:	int型に変換して格納する
	//	引数		:	long input			変換する値を受け取る
	//				out int lowBits		低い値を格納する変数を参照する 0~15bitの領域
	//				out int highBits	高い値を格納する変数を参照する 16~32bitの領域
	//	戻り値	:	なし
	//**************************************************************//
	private static void SplitLong(long input, out int lowBits, out int highBits)
	{
		lowBits = (int)(uint)(ulong)input;
		highBits = (int)(uint)(input >> 32);
	}

	//**************************************************************//
	//	関数名　:	SetLong
	//	機能		:	long型の値を、キーに対応させて保存する
	//	引数		:	string key		設定するキー
	//				long value		保存する値
	//	戻り値	:	なし
	//**************************************************************//
	public static void SetLong(string key, long value)
	{
		int lowBits, highBits;
		SplitLong(value, out lowBits, out highBits);
		PlayerPrefs.SetInt(key+"_lowBits", lowBits);
		PlayerPrefs.SetInt(key+"_highBits", highBits);
	}

	//**************************************************************//
	//	関数名　:	SetVector2
	//	機能		:	Vector2型の値を、キーに対応させて保存する
	//	引数		:	String key		設定するキー
	//				Vector2 vector	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetVector2 (String key, Vector2 vector)
	{
		return SetFloatArray(key, new float[]{vector.x, vector.y});
	}

	//**************************************************************//
	//	関数名　:	GetVector2
	//	機能		:	キーに対応したVector2型の値を取り出す
	//	引数		:	String key	設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	static Vector2 GetVector2 (String key)
	{
		var floatArray = GetFloatArray(key);
		if (floatArray.Length < 2)
		{
			return Vector2.zero;
		}
		return new Vector2(floatArray[0], floatArray[1]);
	}

	//**************************************************************//
	//	関数名　:	GetVector2
	//	機能		:	キーに対応したVector2型の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Vector2 defaultValue	初期化する値
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector2 GetVector2 (String key, Vector2 defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetVector2(key);
		}
		return defaultValue;
	}

	//**************************************************************//
	//	関数名　:	SetVector3
	//	機能		:	Vector3型の値を、キーに対応させて保存する
	//	引数		:	String key		設定するキー
	//				Vector3 vector	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetVector3 (String key, Vector3 vector)
	{
		return SetFloatArray(key, new float []{vector.x, vector.y, vector.z});
	}

	//**************************************************************//
	//	関数名　:	GetVector3
	//	機能		:	キーに対応したVector3型の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector3 GetVector3 (String key)
	{
		var floatArray = GetFloatArray(key);
		if (floatArray.Length < 3)
		{
			return Vector3.zero;
		}
		return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}

	//**************************************************************//
	//	関数名　:	GetVector3
	//	機能		:	キーに対応したVector3型の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Vector3 defaultValue	初期化する値
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector3 GetVector3 (String key, Vector3 defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetVector3(key);
		}
		return defaultValue;
	}

	//**************************************************************//
	//	関数名　:	SetQuaternion
	//	機能		:	Quaternion型の値を、キーに対応させて保存する
	//	引数		:	String key		設定するキー
	//				Quaternion vector	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetQuaternion (String key, Quaternion vector)
	{
		return SetFloatArray(key, new float[]{vector.x, vector.y, vector.z, vector.w});
	}

	//**************************************************************//
	//	関数名　:	GetQuaternion
	//	機能		:	キーに対応したQuaternion型の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Quaternion GetQuaternion (String key)
	{
		var floatArray = GetFloatArray(key);
		if (floatArray.Length < 4)
		{
			return Quaternion.identity;
		}
		return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
	}

	//**************************************************************//
	//	関数名　:	GetQuaternion
	//	機能		:	キーに対応したQuaternion型の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Quaternion defaultValue	初期化する値
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Quaternion GetQuaternion (String key, Quaternion defaultValue )
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetQuaternion(key);
		}
		return defaultValue;
	}

	//**************************************************************//
	//	関数名　:	SetColor
	//	機能		:	Color型の値を、キーに対応させて保存する
	//	引数		:	String key		設定するキー
	//				Color color		保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetColor (String key, Color color)
	{
		return SetFloatArray(key, new float[]{color.r, color.g, color.b, color.a});
	}

	//**************************************************************//
	//	関数名　:	GetColor
	//	機能		:	キーに対応したColor型の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Color GetColor (String key)
	{
		var floatArray = GetFloatArray(key);
		if (floatArray.Length < 4)
		{
			return new Color(0.0f, 0.0f, 0.0f, 0.0f);
		}
		return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
	}

	//**************************************************************//
	//	関数名　:	GetColor
	//	機能		:	キーに対応したColor型の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Color defaultValue	初期化する値
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Color GetColor (String key , Color defaultValue )
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetColor(key);
		}
		return defaultValue;
	}

	//**************************************************************//
	//	関数名　:	SetBoolArray
	//	機能		:	bool型配列の値を、キーに対応させて保存する
	//	引数		:	String key			設定するキー
	//				bool[] boolArray	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetBoolArray (String key, bool[] boolArray)
	{
		var bytes = new byte[(boolArray.Length + 7)/8 + 5];
		bytes[0] = System.Convert.ToByte (ArrayType.Bool);
		var bits = new BitArray(boolArray);
		bits.CopyTo (bytes, 5);
		Initialize();
		ConvertInt32ToBytes (boolArray.Length, bytes);
		
		return SaveBytes (key, bytes);	
	}

	//**************************************************************//
	//	関数名　:	GetBoolArray
	//	機能		:	キーに対応したbool型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static bool[] GetBoolArray (String key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
			if (bytes.Length < 5)
			{
				Debug.LogError ("Corrupt preference file for " + key);
				return new bool[0];
			}
			if ((ArrayType)bytes[0] != ArrayType.Bool)
			{
				Debug.LogError (key + " is not a boolean array");
				return new bool[0];
			}
			Initialize();
			
			var bytes2 = new byte[bytes.Length-5];
			System.Array.Copy(bytes, 5, bytes2, 0, bytes2.Length);
			var bits = new BitArray(bytes2);
			bits.Length = ConvertBytesToInt32 (bytes);
			var boolArray = new bool[bits.Count];
			bits.CopyTo (boolArray, 0);
			
			return boolArray;
		}
		return new bool[0];
	}

	//**************************************************************//
	//	関数名　:	GetBoolArray
	//	機能		:	キーに対応したbool型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				bool[] defaultValue		初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static bool[] GetBoolArray (String key, bool defaultValue, int defaultSize) 
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetBoolArray(key);
		}
		var boolArray = new bool[defaultSize];
		for(int i=0;i<defaultSize;i++)
		{
			boolArray[i] = defaultValue;
		}
		return boolArray;
	}

	//**************************************************************//
	//	関数名　:	SetStringArray
	//	機能		:	String型配列の値を、キーに対応させて保存する
	//	引数		:	String 		key			設定するキー
	//				String[] 	stringArray	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetStringArray (String key, String[] stringArray)
	{
		var bytes = new byte[stringArray.Length + 1];
		bytes[0] = System.Convert.ToByte (ArrayType.String);
		Initialize();
		
		for (var i = 0; i < stringArray.Length; i++)
		{
			if (stringArray[i] == null)
			{
				Debug.LogError ("Can't save null entries in the string array when setting " + key);
				return false;
			}
			if (stringArray[i].Length > 255)
			{
				Debug.LogError ("Strings cannot be longer than 255 characters when setting " + key);
				return false;
			}
			bytes[idx++] = (byte)stringArray[i].Length;
		}
		
		try
		{
			PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes) + "|" + String.Join("", stringArray));
		}
		catch
		{
			return false;
		}
		return true;
	}

	//**************************************************************//
	//	関数名　:	GetStringArray
	//	機能		:	キーに対応したString型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static String[] GetStringArray (String key)
	{
		if (PlayerPrefs.HasKey(key)) {
			var completeString = PlayerPrefs.GetString(key);
			var separatorIndex = completeString.IndexOf("|"[0]);
			if (separatorIndex < 4) {
				Debug.LogError ("Corrupt preference file for " + key);
				return new String[0];
			}
			var bytes = System.Convert.FromBase64String (completeString.Substring(0, separatorIndex));
			if ((ArrayType)bytes[0] != ArrayType.String) {
				Debug.LogError (key + " is not a string array");
				return new String[0];
			}
			Initialize();
			
			var numberOfEntries = bytes.Length-1;
			var stringArray = new String[numberOfEntries];
			var stringIndex = separatorIndex + 1;
			for (var i = 0; i < numberOfEntries; i++)
			{
				int stringLength = bytes[idx++];
				if (stringIndex + stringLength > completeString.Length)
				{
					Debug.LogError ("Corrupt preference file for " + key);
					return new String[0];
				}
				stringArray[i] = completeString.Substring(stringIndex, stringLength);
				stringIndex += stringLength;
			}
			
			return stringArray;
		}
		return new String[0];
	}

	//**************************************************************//
	//	関数名　:	GetStringArray
	//	機能		:	キーに対応したString型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				String defaultValue		初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static String[] GetStringArray (String key, String defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetStringArray(key);
		}
		var stringArray = new String[defaultSize];
		for(int i=0;i<defaultSize;i++)
		{
			stringArray[i] = defaultValue;
		}
		return stringArray;
	}

	//**************************************************************//
	//	関数名　:	SetIntArray
	//	機能		:	int型配列の値を、キーに対応させて保存する
	//	引数		:	String key			設定するキー
	//				int[] intArray		保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetIntArray (String key, int[] intArray)
	{
		return SetValue (key, intArray, ArrayType.Int32, 1, ConvertFromInt);
	}

	//**************************************************************//
	//	関数名　:	SetFloatArray
	//	機能		:	float型配列の値を、キーに対応させて保存する
	//	引数		:	String key			設定するキー
	//				float[] floatArray	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetFloatArray (String key, float[] floatArray)
	{
		return SetValue (key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
	}

	//**************************************************************//
	//	関数名　:	SetVector2Array
	//	機能		:	Vector2型配列の値を、キーに対応させて保存する
	//	引数		:	String key					設定するキー
	//				Vector2[] vector2Array		保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetVector2Array (String key, Vector2[] vector2Array )
	{
		return SetValue (key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
	}

	//**************************************************************//
	//	関数名　:	SetVector3Array
	//	機能		:	Vector3型配列の値を、キーに対応させて保存する
	//	引数		:	String key				設定するキー
	//				Vector3[] vector3Array	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetVector3Array (String key, Vector3[] vector3Array)
	{
		return SetValue (key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
	}

	//**************************************************************//
	//	関数名　:	SetQuaternionArray
	//	機能		:	Quaternion型配列の値を、キーに対応させて保存する
	//	引数		:	String key						設定するキー
	//				Quaternion[] quaternionArray	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetQuaternionArray (String key, Quaternion[] quaternionArray )
	{
		return SetValue (key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
	}

	//**************************************************************//
	//	関数名　:	SetColorArray
	//	機能		:	Color型配列の値を、キーに対応させて保存する
	//	引数		:	String key			設定するキー
	//				Color[] colorArray	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	public static bool SetColorArray (String key, Color[] colorArray)
	{
		return SetValue (key, colorArray, ArrayType.Color, 4, ConvertFromColor);
	}

	//**************************************************************//
	//	関数名　:	SetValue<T>
	//	機能		:	Template型(T)の値を、キーに対応させて保存する
	//	引数		:	String 					key			設定するキー
	//				T						array		保存する値
	//				ArrayType				arrayType	保存する配列の種類
	//				int						vectorNumber変数の格納数
	//				Action<T, byte[],int> 	convert		型変換を関数を受け取る:デリゲート（ラムダ式）
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	private static bool SetValue<T> (String key, T array, ArrayType arrayType, int vectorNumber, Action<T, byte[],int> convert) where T : IList
	{
		var bytes = new byte[(4*array.Count)*vectorNumber + 1];
		bytes[0] = System.Convert.ToByte (arrayType);
		Initialize();
		
		for (var i = 0; i < array.Count; i++) {
			convert (array, bytes, i);
		}
		return SaveBytes (key, bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertFromInt
	//	機能		:	int型をbyte型に変換する
	//	引数		:	int[]	array	変換する値
	//				byte[]	bytes	格納する値
	//				int		i		配列番号
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFromInt (int[] array, byte[] bytes, int i)
	{
		ConvertInt32ToBytes (array[i], bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertFromFloat
	//	機能		:	float型をbyte型に変換する
	//	引数		:	float[]	array	変換する値
	//				byte[]	bytes	格納する値
	//				int		i		配列番号
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFromFloat (float[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes (array[i], bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertFromVector2
	//	機能		:	Vector2型をbyte型に変換する
	//	引数		:	Vector2[]	array	変換する値
	//				byte[]		bytes	格納する値
	//				int			i		配列番号
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFromVector2 (Vector2[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes (array[i].x, bytes);
		ConvertFloatToBytes (array[i].y, bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertFromVector3
	//	機能		:	Vector3型をbyte型に変換する
	//	引数		:	Vector3[]	array	変換する値
	//				byte[]		bytes	格納する値
	//				int			i		配列番号
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFromVector3 (Vector3[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes (array[i].x, bytes);
		ConvertFloatToBytes (array[i].y, bytes);
		ConvertFloatToBytes (array[i].z, bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertFromQuaternion
	//	機能		:	Quaternion型をbyte型に変換する
	//	引数		:	Quaternion[]	array	変換する値
	//				byte[]			bytes	格納する値
	//				int				i		配列番号
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFromQuaternion (Quaternion[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes (array[i].x, bytes);
		ConvertFloatToBytes (array[i].y, bytes);
		ConvertFloatToBytes (array[i].z, bytes);
		ConvertFloatToBytes (array[i].w, bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertFromColor
	//	機能		:	Color型をbyte型に変換する
	//	引数		:	Color[]	array	変換する値
	//				byte[]	bytes	格納する値
	//				int		i		配列番号
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFromColor (Color[] array, byte[] bytes, int i)
	{
		ConvertFloatToBytes (array[i].r, bytes);
		ConvertFloatToBytes (array[i].g, bytes);
		ConvertFloatToBytes (array[i].b, bytes);
		ConvertFloatToBytes (array[i].a, bytes);
	}

	//**************************************************************//
	//	関数名　:	GetIntArray
	//	機能		:	キーに対応したint型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static int[] GetIntArray (String key)
	{
		var intList = new List<int>();
		GetValue (key, intList, ArrayType.Int32, 1, ConvertToInt);
		return intList.ToArray();
	}

	//**************************************************************//
	//	関数名　:	GetIntArray
	//	機能		:	キーに対応したint型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				int defaultValue		初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static int[] GetIntArray (String key, int defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetIntArray(key);
		}
		var intArray = new int[defaultSize];
		for (int i=0; i<defaultSize; i++)
		{
			intArray[i] = defaultValue;
		}
		return intArray;
	}

	//**************************************************************//
	//	関数名　:	GetFloatArray
	//	機能		:	キーに対応したfloat型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static float[] GetFloatArray (String key)
	{
		var floatList = new List<float>();
		GetValue (key, floatList, ArrayType.Float, 1, ConvertToFloat);
		return floatList.ToArray();
	}

	//**************************************************************//
	//	関数名　:	GetFloatArray
	//	機能		:	キーに対応したfloat型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				float defaultValue		初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static float[] GetFloatArray (String key, float defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetFloatArray(key);
		}
		var floatArray = new float[defaultSize];
		for (int i=0; i<defaultSize; i++)
		{
			floatArray[i] = defaultValue;
		}
		return floatArray;
	}

	//**************************************************************//
	//	関数名　:	GetVector2Array
	//	機能		:	キーに対応したVector2型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector2[] GetVector2Array (String key)
	{
		var vector2List = new List<Vector2>();
		GetValue (key, vector2List, ArrayType.Vector2, 2, ConvertToVector2);
		return vector2List.ToArray();
	}

	//**************************************************************//
	//	関数名　:	GetVector2Array
	//	機能		:	キーに対応したVector2型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Vector2 defaultValue	初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector2[] GetVector2Array (String key, Vector2 defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetVector2Array(key);
		}
		var vector2Array = new Vector2[defaultSize];
		for(int i=0; i< defaultSize;i++)
		{
			vector2Array[i] = defaultValue;
		}
		return vector2Array;
	}

	//**************************************************************//
	//	関数名　:	GetVector3Array
	//	機能		:	キーに対応したVector3型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector3[] GetVector3Array (String key)
	{
		var vector3List = new List<Vector3>();
		GetValue (key, vector3List, ArrayType.Vector3, 3, ConvertToVector3);
		return vector3List.ToArray();
	}

	//**************************************************************//
	//	関数名　:	GetVector3Array
	//	機能		:	キーに対応したVector3型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Vector3 defaultValue	初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Vector3[] GetVector3Array (String key, Vector3 defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
			
		{
			return GetVector3Array(key);
		}
		var vector3Array = new Vector3[defaultSize];
		for (int i=0; i<defaultSize;i++)
		{
			vector3Array[i] = defaultValue;
		}
		return vector3Array;
	}

	//**************************************************************//
	//	関数名　:	GetQuaternionArray
	//	機能		:	キーに対応したQuaternion型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Quaternion[] GetQuaternionArray (String key)
	{
		var quaternionList = new List<Quaternion>();
		GetValue (key, quaternionList, ArrayType.Quaternion, 4, ConvertToQuaternion);
		return quaternionList.ToArray();
	}

	//**************************************************************//
	//	関数名　:	GetQuaternionArray
	//	機能		:	キーに対応したQuaternion型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Quaternion defaultValue	初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Quaternion[] GetQuaternionArray (String key, Quaternion defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return GetQuaternionArray(key);
		}
		var quaternionArray = new Quaternion[defaultSize];
		for(int i=0;i<defaultSize;i++)
		{
			quaternionArray[i] = defaultValue;
		}
		return quaternionArray;
	}

	//**************************************************************//
	//	関数名　:	GetColorArray
	//	機能		:	キーに対応したColor型配列の値を取り出す
	//	引数		:	String key				設定するキー
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Color[] GetColorArray (String key)
	{
		var colorList = new List<Color>();
		GetValue (key, colorList, ArrayType.Color, 4, ConvertToColor);
		return colorList.ToArray();
	}

	//**************************************************************//
	//	関数名　:	GetColorArray
	//	機能		:	キーに対応したColor型配列の値を取り出す
	//				キーが無い時は、初期化をする
	//	引数		:	String key				設定するキー
	//				Color defaultValue		初期化する値
	//				int defaultSize			初期化する要素数
	//	戻り値	:	取り出した値を返す
	//**************************************************************//
	public static Color[] GetColorArray (String key, Color defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key)) {
			return GetColorArray(key);
		}
		var colorArray = new Color[defaultSize];
		for(int i=0;i<defaultSize;i++)
		{
			colorArray[i] = defaultValue;
		}
		return colorArray;
	}

	//**************************************************************//
	//	関数名　:	GetValue<T>
	//	機能		:	キーに対応したTemplate型(T)の値を取り出す
	//	引数		:	String 				key 			設定するキー
	//				T 					list 			取得した値を格納する
	//				ArrayType		 	arrayType		取得する配列の種類
	//				int 				vectorNumber	変数の格納数
	//				Action<T, byte[]> 	convert			型変換を関数を受け取る:デリゲート（ラムダ式）
	//	戻り値	:	なし
	//**************************************************************//
	private static void GetValue<T> (String key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
	{
		if (PlayerPrefs.HasKey(key))
		{
			var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
			if ((bytes.Length-1) % (vectorNumber*4) != 0)
			{
				Debug.LogError ("Corrupt preference file for " + key);
				return;
			}
			if ((ArrayType)bytes[0] != arrayType)
			{
				Debug.LogError (key + " is not a " + arrayType.ToString() + " array");
				return;
			}
			Initialize();
			
			var end = (bytes.Length-1) / (vectorNumber*4);
			for (var i = 0; i < end; i++)
			{
				convert (list, bytes);
			}
		}
	}

	//**************************************************************//
	//	関数名　:	ConvertToInt
	//	機能		:	byte型をint型に変換して、リストに追加する
	//	引数		:	List<int> list	取得した値を格納する
	//				byte[] bytes	変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertToInt (List<int> list, byte[] bytes)
	{
		list.Add (ConvertBytesToInt32(bytes));
	}

	//**************************************************************//
	//	関数名　:	ConvertToFloat
	//	機能		:	byte型をfloat型に変換して、リストに追加する
	//	引数		:	List<float> list	取得した値を格納する
	//				byte[] bytes		変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertToFloat (List<float> list, byte[] bytes)
	{
		list.Add (ConvertBytesToFloat(bytes));
	}

	//**************************************************************//
	//	関数名　:	ConvertToVector2
	//	機能		:	byte型をVector2型に変換して、リストに追加する
	//	引数		:	List<Vector2> list	取得した値を格納する
	//				byte[] bytes		変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertToVector2 (List<Vector2> list, byte[] bytes)
	{
		list.Add (new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	//**************************************************************//
	//	関数名　:	ConvertToVector3
	//	機能		:	byte型をVector3型に変換して、リストに追加する
	//	引数		:	List<Vector3> list	取得した値を格納する
	//				byte[] bytes		変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertToVector3 (List<Vector3> list, byte[] bytes)
	{
		list.Add (new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	//**************************************************************//
	//	関数名　:	ConvertToQuaternion
	//	機能		:	byte型をQuaternion型に変換して、リストに追加する
	//	引数		:	List<Quaternion> list	取得した値を格納する
	//				byte[] bytes			変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertToQuaternion (List<Quaternion> list,byte[] bytes)
	{
		list.Add (new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	//**************************************************************//
	//	関数名　:	ConvertToColor
	//	機能		:	byte型をColor型に変換して、リストに追加する
	//	引数		:	List<Color> list	取得した値を格納する
	//				byte[] bytes		変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertToColor (List<Color> list, byte[] bytes)
	{
		list.Add (new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
	}

	//**************************************************************//
	//	関数名　:	ShowArrayType
	//	機能		:	キーに対応した配列の種類を確認する
	//	引数		:	String key		設定するキー
	//	戻り値	:	なし
	//**************************************************************//
	public static void ShowArrayType (String key)
	{
		var bytes = System.Convert.FromBase64String (PlayerPrefs.GetString(key));
		if (bytes.Length > 0)
		{
			ArrayType arrayType = (ArrayType)bytes[0];
			Debug.Log (key + " is a " + arrayType.ToString() + " array");
		}
	}

	//**************************************************************//
	//	関数名　:	Initialize
	//	機能		:	初期化
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	private static void Initialize ()
	{
		if (System.BitConverter.IsLittleEndian)
		{
			endianDiff1 = 0;
			endianDiff2 = 0;
		}
		else
		{
			endianDiff1 = 3;
			endianDiff2 = 1;
		}
		if (byteBlock == null)
		{
			byteBlock = new byte[4];
		}
		idx = 1;
	}

	//**************************************************************//
	//	関数名　:	SaveBytes
	//	機能		:	byte型配列の値を、キーに対応させて保存する
	//	引数		:	String key		設定するキー
	//				byte[] bytes	保存する値
	//	戻り値	:	true:成功	false:失敗
	//**************************************************************//
	private static bool SaveBytes (String key, byte[] bytes)
	{
		try
		{
			PlayerPrefs.SetString (key, System.Convert.ToBase64String (bytes));
		}
		catch
		{
			return false;
		}
		return true;
	}

	//**************************************************************//
	//	関数名　:	ConvertFloatToBytes
	//	機能		:	floar型をbyte型に変換する
	//	引数		:	float	f		変換する値
	//				byte[] bytes	保存する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFloatToBytes (float f, byte[] bytes)
	{
		byteBlock = System.BitConverter.GetBytes (f);
		ConvertTo4Bytes (bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertBytesToFloat
	//	機能		:	byte型をfloar型に変換する
	//	引数		:	byte[] bytes	変換する値
	//	戻り値	:	変換した値を返す
	//**************************************************************//
	private static float ConvertBytesToFloat (byte[] bytes)
	{
		ConvertFrom4Bytes (bytes);
		return System.BitConverter.ToSingle (byteBlock, 0);
	}

	//**************************************************************//
	//	関数名　:	ConvertInt32ToBytes
	//	機能		:	int型をbyte型に変換する
	//	引数		:	int	i			変換する値
	//				byte[] bytes	保存する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertInt32ToBytes (int i, byte[] bytes)
	{
		byteBlock = System.BitConverter.GetBytes (i);
		ConvertTo4Bytes (bytes);
	}

	//**************************************************************//
	//	関数名　:	ConvertBytesToInt32
	//	機能		:	byte型をint型に変換する
	//	引数		:	byte[] bytes	変換する値
	//	戻り値	:	変換した値を返す
	//**************************************************************//
	private static int ConvertBytesToInt32 (byte[] bytes)
	{
		ConvertFrom4Bytes (bytes);
		return System.BitConverter.ToInt32 (byteBlock, 0);
	}

	//**************************************************************//
	//	関数名　:	ConvertTo4Bytes
	//	機能		:	4byteの形式に変換する
	//	引数		:	byte[] bytes	変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertTo4Bytes (byte[] bytes)
	{
		bytes[idx  ] = byteBlock[    endianDiff1];
		bytes[idx+1] = byteBlock[1 + endianDiff2];
		bytes[idx+2] = byteBlock[2 - endianDiff2];
		bytes[idx+3] = byteBlock[3 - endianDiff1];
		idx += 4;
	}

	//**************************************************************//
	//	関数名　:	ConvertFrom4Bytes
	//	機能		:	4byte形式から変換する
	//	引数		:	byte[] bytes	変換する値
	//	戻り値	:	なし
	//**************************************************************//
	private static void ConvertFrom4Bytes (byte[] bytes)
	{
		byteBlock[    endianDiff1] = bytes[idx  ];
		byteBlock[1 + endianDiff2] = bytes[idx+1];
		byteBlock[2 - endianDiff2] = bytes[idx+2];
		byteBlock[3 - endianDiff1] = bytes[idx+3];
		idx += 4;
	}


	//**************************************************************//
	//								End of class					//
	//**************************************************************//
}