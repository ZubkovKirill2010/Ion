using System.Net;
using System.Net.Mail;

namespace BCMEditor.Network
{
    public static class Email
    {
        public const string _SmtpServer = "smtp.mail.ru";
        public const int _SmtpPort = 587;

        public static void Send(Settings Settings, string Header, string MessageText, string[] Recipients)
        {
            if (Recipients.Length == 0)
            {
                MainWindow.Log("Укажите получателей");
                return;
            }

            string Email = Settings._Email;
            string Password = Settings._EmailPassword;

            if (Email.Length == 0)
            {
                MainWindow.Log("Для отправки письма укажите свою почту в настройках");
                return;
            }
            else if (Password.Length == 0)
            {
                MainWindow.Log("Для отправки письма укажите пароль приложения почту в настройках");
                return;
            }

            using (SmtpClient Client = new SmtpClient(_SmtpServer, _SmtpPort))
            {
                Client.EnableSsl = true;
                Client.Credentials = new NetworkCredential
                (
                    Email,
                    SecurePasswordStorage.Decrypt(Password)
                );

                using (MailMessage Message = new MailMessage())
                {
                    Message.From = new MailAddress(Email);

                    foreach (string Recipient in Recipients)
                    {
                        Message.To.Add(Recipient);
                    }

                    if (Message.To.Count == 0)
                    {
                        return;
                    }

                    Message.Subject = Header;
                    Message.Body = MessageText;

                    try
                    {
                        Client.Send(Message);
                        MainWindow.Log("Сообщение отправлено");
                    }
                    catch (Exception Exception)
                    {
                        MainWindow.Log($"Ошибка при отправке сообщения");
                        MainWindow.LogError(Exception);
                    }
                }
            }
        }
    }
}