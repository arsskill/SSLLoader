using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SSLLoader

{
    public class EmailSender
    {
        public string MessageTO { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        X509Certificate cert = new X509Certificate();
        public EmailSender(string messageTO, string subject, string messagetext, X509Certificate certificate)
        {
            MessageTO = messageTO;
            Subject = subject;
            MessageText = messagetext;
            cert = certificate;
        }
        public static void sendEmail(string MessageTO, string Subject, string MessageText, X509Certificate cert)
        {
            if (cert is null)
            {
                throw new ArgumentNullException(nameof(cert));
            }

            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(MessageTO, "имя");
            // кому отправляем
            MailAddress to = new MailAddress("кому");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = Subject;
            // текст письма
            m.Body = MessageText;
            // добваляем вложения
            m.Attachments.Add(new Attachment($"D:\\Cert\\{cert.GetSerialNumberString()}.cer"));
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient(from.Address, 465);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(from.Address, "пароль");
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(m);
            }
            catch (SmtpException exception)
            {
                StreamWriter exceptionWriter = new StreamWriter("exc.txt");
                exceptionWriter.Write(exception.ToString());
                Console.WriteLine("A");
                exceptionWriter.Close();
            }

        }
}
