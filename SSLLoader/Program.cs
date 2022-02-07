using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Generic;

namespace SSLLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            // список сайтов
            string[] url = { "https://www.bestprog.net/ru/2019/12/19/c-arrays-of-strings-examples-of-solving-the-most-common-tasks-ru/", "https://docs.microsoft.com/ru-ru/dotnet/api/system.security.cryptography.x509certificates.x509certificate2.getcertcontenttype?view=netcore-3.0", "https://yandex.ru/maps/213/moscow/?ll=37.660549%2C55.718616&z=14", "https://www.cyberforum.ru/visual-studio/thread1226306.html",  "https://decor.gradientpro.ru/#/login" , "https://www.youtube.com/watch?v=-R455cuPsV4", "https://vk.com/im?peers=c178&sel=c177" };
            SaveCertificate(url);
        }
        static void SaveCertificate(string[] url)
        {
            int i = 0;
            while (i<url.Length)
            {
                var request = (HttpWebRequest)WebRequest.Create(url[i]);
                request.AllowAutoRedirect = false;
                request.ServerCertificateValidationCallback = ServerCertificateValidationCallback;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                i++;
            }
        }
        static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            int a = 0;
            List<string> codes = new List<string>();
            var dir = new DirectoryInfo(@"D:\\Cert"); // папка с файлами
            if(dir.GetFiles().Length==0)
            {
                File.WriteAllText($"D:\\Cert\\{certificate.GetSerialNumberString()}.cer", Convert.ToBase64String(certificate.Export(X509ContentType.Cert)));
            }
            foreach (FileInfo file in dir.GetFiles()) //создание динамического массива с хеш кодами для сравнения
            {
                Console.WriteLine(Path.GetFileNameWithoutExtension(file.FullName));
                codes.Add(Path.GetFileNameWithoutExtension(file.FullName));
                a = dir.GetFiles().Length; // что бы знать, сколько файлов в директории, что бы потом сравнить 
            }
            for (int i = 0; i < a; i++)
            {
                X509Certificate2 certificate1 = new X509Certificate2(Convert.FromBase64String(File.ReadAllText($"D:\\Cert\\{codes[i]}.cer")));
                {
                    if (certificate1.GetName() == certificate.GetName())
                    {
                        if(certificate1.GetSerialNumberString() != certificate.GetSerialNumberString())
                        {
                            Console.WriteLine($"deleted: {certificate1.GetName()}, replaced by: {certificate.GetName()}");
                            File.Delete($"D:\\Cert\\{certificate1.GetSerialNumberString()}.cer");
                            EmailSender.sendEmail("Ars", "test", "Hello", certificate);
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            //int b = 0;
            //for (int i = 0; i<=a;i++) //проверка на то, сходятся ли хеш коды у нового и уже имеющегося сертификата
            //{
            //    if(b>a-1)
            //    {
            //        if (i == a)
            //        {
            //            //EmailSender.sendEmail("Ars", "test", "Hello",certificate);
            //            //string name = certificate.GetName();
            //            //Console.WriteLine(name);
            //            File.WriteAllText($"D:\\Cert\\{certificate.GetSerialNumberString()}.cer", Convert.ToBase64String(certificate.Export(X509ContentType.Cert)));
            //            try
            //            {
            //                using (X509Certificate2 certificate1 = new X509Certificate2(Convert.FromBase64String(File.ReadAllText($"D:\\Cert\\{codes[i]}.cer"))))
            //                {
            //                    if (certificate1.GetName() == certificate.GetName())
            //                    {
            //                        Console.WriteLine($"deleted: {certificate1.GetName()}, replaced by: {certificate.GetName()}");
            //                        File.Delete($"D:\\Cert\\{certificate1.GetSerialNumberString()}.cer");
            //                    }
            //                    //Console.WriteLine(sr.ReadToEnd());
            //                    //string input = sr.ReadToEnd();
            //                    //byte[] buffer = Convert.FromBase64String(input);
            //                    //buffer.GetType();
            //                    //string text = BitConverter.ToString(buffer);
            //                    //Console.WriteLine(text);
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                Console.WriteLine(e.Message);
            //            }
            //            return true;
            //        }
            //    }
            //    if (codes[b] == certificate.GetSerialNumberString())
            //    {
            //        return true;
            //    }
            //    if (codes[b] != certificate.GetSerialNumberString())
            //    {
            //        b++;
            //    }            
            //}
            Console.WriteLine("-----BEGIN EXPDATE-----");
            Console.WriteLine(certificate.GetExpirationDateString());
            Console.WriteLine("-----END EXPDATE-----");
            return true;
        }
    }
}
