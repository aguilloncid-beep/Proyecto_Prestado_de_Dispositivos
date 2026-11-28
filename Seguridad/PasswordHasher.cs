using System;
using System.Security.Cryptography;

namespace SistemaPresatamos.Seguridad
{
    public static class PasswordHasher
    {
        private const string Prefijo = "PBKDF2-SHA256";
        private const int TamanoSal = 16;
        private const int TamanoHash = 32;
        private const int Iteraciones = 600000;
        private const int IteracionesMaximasAceptadas = 5000000;

        public const int LongitudMinima = 8;

        public static string Generar(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            byte[] sal = RandomNumberGenerator.GetBytes(TamanoSal);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
            return Prefijo + "$" + Iteraciones + "$" + Convert.ToBase64String(sal) + "$" + Convert.ToBase64String(hash);
        }

        public static bool Verificar(string password, string? hashAlmacenado)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashAlmacenado))
                return false;

            string[] partes = hashAlmacenado.Split('$');
            if (partes.Length != 4 || partes[0] != Prefijo)
                return false;

            if (!int.TryParse(partes[1], out int iteraciones) || iteraciones < 1 || iteraciones > IteracionesMaximasAceptadas)
                return false;

            byte[] sal;
            byte[] esperado;
            try
            {
                sal = Convert.FromBase64String(partes[2]);
                esperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            if (sal.Length == 0 || esperado.Length == 0)
                return false;

            byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(password, sal, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }

        public static bool CumpleRequisitos(string? password, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrEmpty(password) || password.Length < LongitudMinima)
            {
                mensaje = "La contraseña debe tener al menos " + LongitudMinima + " caracteres.";
                return false;
            }

            bool hayLetra = false;
            bool hayNumero = false;
            foreach (char c in password)
            {
                if (char.IsLetter(c)) hayLetra = true;
                if (char.IsDigit(c)) hayNumero = true;
            }

            if (!hayLetra || !hayNumero)
            {
                mensaje = "La contraseña debe combinar letras y números.";
                return false;
            }

            return true;
        }
    }
}
