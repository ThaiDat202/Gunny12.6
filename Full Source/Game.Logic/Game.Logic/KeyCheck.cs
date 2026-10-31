using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Game.Logic;

public static class KeyCheck
{
	public static bool Init(string key, string version, string ip, int rand)
	{
		return VerificaKey(key, version, ip, rand);
	}

	private static bool VerificaKey(string key, string version, string ip, int rand)
	{
		try
		{
			WebClient webClient = new WebClient();
			webClient.Headers.Add("user-agent", "keycheck");
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("key", key);
			nameValueCollection.Add("version", version);
			nameValueCollection.Add("ip", ip);
			nameValueCollection.Add("rand", rand.ToString() ?? "");
			byte[] bytes = webClient.UploadValues("https://key.gamesnexus.com.br/key.php", "POST", nameValueCollection);
			string @string = Encoding.UTF8.GetString(bytes);
			string text = CalculateMD5Hash(key + version + rand);
			return CalculateMD5Hash(key + version + ip + rand) == @string;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static string CalculateMD5Hash(string input)
	{
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input + "Joao159753845689!@#$");
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}
}
