using System.Text;
using System.Numerics;

namespace RSAkryptering
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, string> codeOfLetters = new Dictionary<int, string>
            {
                [0] = " ",
                [2] = "A",
                [3] = "B",
                [4] = "C",
                [5] = "D",
                [6] = "E",
                [7] = "F",
                [8] = "G",
                [9] = "H",
                [10] = "I",
                [11] = "J",
                [12] = "K",
                [13] = "L",
                [14] = "M",
                [15] = "N",
                [16] = "O",
                [17] = "P",
                [18] = "Q",
                [19] = "R",
                [20] = "S",
                [21] = "T",
                [22] = "U",
                [23] = "V",
                [24] = "W",
                [25] = "X",
                [26] = "Y",
                [27] = "Z"



            };
            var (k, N) = ((BigInteger)0, (BigInteger)0);    
            bool runProgram = true;
            BigInteger prime1 = 0;
            BigInteger prime2 = 0;
            var encryptedMeassage = new List<BigInteger>();
            string messageToEncrypt = "";   
            List<BigInteger> encryptedMessage = new List<BigInteger>();
            bool gotKeys = false;
            bool dGenerated = false;
            BigInteger privatKey = 0;
            List<string> decryptedMessage = new List<string>();

            while (runProgram)
            {
                Console.WriteLine("[1] Genererate public keyes");
                Console.WriteLine("[2] Genererate private key");
                Console.WriteLine("[3] Encrypt message");
                Console.WriteLine("[4] Decrypt message");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":

                         prime1 = ChoosePrime();
                         prime2 = ChoosePrime();
                        var publicKeys = GeneratePublicKeys(prime1, prime2);
                        (k, N) = publicKeys;
                        gotKeys = true;
                        break;

                    case "2":
                        if (!gotKeys)
                        {
                            Console.WriteLine("You need to generate public keys first");
                            
                            break;
                        }
                        privatKey = GenerateSecretKeyD(k, prime1, prime2);
                        dGenerated = true;
                        break;

                    case "3":
                        if (!gotKeys)
                        {
                            Console.WriteLine("You need to generate public keys first");
                            break;
                        }
                        Console.Write("Enter message to encrypt (A-Z and space only): ");
                        messageToEncrypt = Console.ReadLine();
                        if (string.IsNullOrEmpty(messageToEncrypt))
                        {
                            Console.WriteLine("Message cannot be empty");
                            break;
                        }
                        if (messageToEncrypt.Any(c => !char.IsLetter(c) && c != ' '))
                        {
                            Console.WriteLine("Message can only contain letters A-Z and spaces");
                            break;
                        }
                        Console.WriteLine("Type in the recievers public keyes ([k,N])");
                        Console.Write("k: ");
                        k = BigInteger.Parse(Console.ReadLine());
                        Console.Write("N: ");
                        N = BigInteger.Parse(Console.ReadLine());
                        encryptedMessage = EncryptMessage(codeOfLetters, k, N, messageToEncrypt);
                        break;

                        case "4":
                        if (encryptedMessage.Count == 0)
                        {
                            Console.WriteLine("You need to encrypt a message first");
                            break;
                        }
                        if (!dGenerated)
                        {
                            Console.WriteLine("You need to generate private key first");
                            break;
                        }
                            decryptedMessage= DecryptMessage(codeOfLetters, privatKey, N, encryptedMessage);

                        break;
                        case "5":
                            runProgram = false;
                            break;
                    default:
                        break;
                }

            }

        }
        public static BigInteger ChoosePrime()
        {
            Console.Write("Choose a prime number: ");
            BigInteger prime = BigInteger.Parse(Console.ReadLine());
            while (!FermatsPrimtalstest(prime))
            {
                Console.WriteLine();
                Console.Write("Inget Primtal, försök igen: ");
                prime = BigInteger.Parse(Console.ReadLine());
            }
            return prime;
        }
        public static bool FermatsPrimtalstest(BigInteger p)
        {

            if (p%2 ==0 )
            {
                Console.WriteLine("Delbart med 2");
                return false; 
            }

            BigInteger n = p - 1;
            BigInteger a = 2;

            while (a < p / 2)
            {
                

                bool passesFermat = BigInteger.ModPow(a, n, p) == 1;

                if (!passesFermat)
                {
                    
                    if (p % a == 0)
                    {
                        Console.WriteLine($"{p} är delbart med {a}");
                    }
                    else
                    {
                        BigInteger faktor = BigInteger.GreatestCommonDivisor(a, p);

                        BigInteger delare = p / faktor;

                        //Console.WriteLine($"Fermat-testet misslyckades. {p} är sammansat men ingen äkkta faktor hittades");
                    }
                    return false;
                }

                //Console.WriteLine($"Klarade Fermat-testet för a = {a}");
                a++;
            }

           // Console.WriteLine($"{p} är sannolikt primtal (klarade Fermat för alla testade baser).");
            return true;
        }


        public static (BigInteger,BigInteger) GeneratePublicKeys(BigInteger p, BigInteger q)
        {
            List<BigInteger> validK = new List<BigInteger>();



            var N = p*q;

            var upperLimitforK = (p - 1) * (q - 1);

                BigInteger k = 2;
               
                while (k < upperLimitforK)
                {
                    var sgd1 = BigInteger.GreatestCommonDivisor(k, N);
                    var sgd2 = BigInteger.GreatestCommonDivisor(k, upperLimitforK);
                    if (sgd1 == 1 && sgd2 == 1)
                    {
                        validK.Add(k);

                        
                    }
                        
                if (validK.Count == 5)
                {
                    break;
                }else
                    k++;
                }


 
              
                Console.WriteLine("Choose public key: ");

                foreach (var c in validK)
                {
                    Console.Write($"[{c}] ");
                }

                BigInteger valtK = int.Parse(Console.ReadLine());
            
                Console.WriteLine($"Public keys ([k,N]): [{valtK},{N}] - Give these to whomever you want to send encrypted messages to you\nPress any key to continue");
                
            return (valtK,N);


           
              

        }

        
        


        public static BigInteger GenerateSecretKeyD(BigInteger k, BigInteger p, BigInteger q)
        {
            var phiN = (p-1) * (q-1);
            var N = p * q;
            List<BigInteger> validKeyes = new List<BigInteger>();

            BigInteger d = 1;
            while (d<=phiN)
            {
                if (k*d % phiN == 1 )
                {
                    validKeyes.Add(d);
                    break;
                }

                d++;

            }
            Console.WriteLine($"Private key: [d,n]: [{d},{N}]- Never give away your secret key.\npress any key to continue");
            Console.ReadKey();
            return d; 
        }

        public static List<BigInteger> EncryptMessage(Dictionary<int,string> codeOfLetters, BigInteger k, BigInteger N, string message)
        {
            List<int> lettersToInt = new List<int>();
            List<BigInteger> encryptedMessageC = new List<BigInteger>();

            message = message.ToUpper(); 

            foreach ( char c in message )
            {
                string letter = c.ToString();

                int codedLetter = codeOfLetters.Where(x => x.Value == letter).Select(x=>x.Key).FirstOrDefault();
                lettersToInt.Add(codedLetter);

            }

           

           
            Console.WriteLine();
            Console.Write("Encrypted message: ");
            foreach (int i in lettersToInt)
            {
                //C = M^k (mod N) 

                BigInteger C = BigInteger.ModPow(i, k, N);
                encryptedMessageC.Add(C);

                

                if (C<10)
                {
                    Console.Write(C.ToString("D2"));
                }else
                {
                    Console.Write(C);

                }
                
            }
            return encryptedMessageC;
        }


        public static List<string> DecryptMessage(Dictionary<int,string> codeOfLetters, BigInteger d, BigInteger N, List<BigInteger> encryptedMessage)
        {
            List<BigInteger> decrptedNumbers = new List<BigInteger>();
            List<string> decodedString = new List<string>();
            //decrypt M = C^d mod N

            Console.WriteLine();
            foreach (var C in encryptedMessage)
            {
                var M = BigInteger.ModPow(C, d, N);
                decrptedNumbers.Add(M);

                //if (M<10)
                //{
                //    Console.Write(M.ToString("D2") + ",");
                //}else
                //{
                //    Console.Write(M+",");
                //}
            }

            foreach (var num in decrptedNumbers)
            {
                string decryptedString = codeOfLetters.Where(x => x.Key == num).Select(x=>x.Value).FirstOrDefault();
                decodedString.Add(decryptedString);
                
            }
            Console.WriteLine();
            Console.Write("Decrypted Message: ");

            foreach (string s in decodedString)
            {
                Console.Write(s);
            }
            Console.WriteLine();
            return decodedString;
        }


    }
    
}
