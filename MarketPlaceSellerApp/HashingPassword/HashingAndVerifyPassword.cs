﻿using System.Security.Cryptography;

namespace MarketPlaceSellerApp.HashingPassword
{
	public class HashingAndVerifyPassword
	{
		public class HashingPassword
		{
			public static string HashPassword(string password)
			{
				byte[] salt = new byte[16];
				RandomNumberGenerator.Fill(salt);

				var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
				byte[] hash = pbkdf2.GetBytes(20);

				byte[] hashBytes = new byte[36];
				Array.Copy(salt, 0, hashBytes, 0, 16);
				Array.Copy(hash, 0, hashBytes, 16, 20);

				return Convert.ToBase64String(hashBytes);
			}

			public static bool VerifyPassword(string password, string hashedPassword)
			{
				byte[] hashBytes = Convert.FromBase64String(hashedPassword);

				byte[] salt = new byte[16];
				Array.Copy(hashBytes, 0, salt, 0, 16);
				byte[] storedHash = new byte[20];
				Array.Copy(hashBytes, 16, storedHash, 0, 20);

				var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
				byte[] hash = pbkdf2.GetBytes(20);

				for (int i = 0; i < 20; i++)
				{
					if (storedHash[i] != hash[i])
						return false;
				}

				return true;
			}
		}
	}
}
