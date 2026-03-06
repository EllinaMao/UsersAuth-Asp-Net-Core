using MailKit.Net.Smtp;
using MimeKit;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class EmailHelper
    {

        private readonly EmailConfiguration _emailConfig;

        public EmailHelper(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task<bool> SendEmailRegistrationConfirm(string userEmail, string link)
        {
  
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("WebApplication1 Project", _emailConfig.UserName));
            message.To.Add(new MailboxAddress(name: userEmail, address: userEmail));
            message.Subject = "Confirmation of registration on the WebApplication1 website";
            message.Body = new TextPart("html")
            {
                Text = link,
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, 587, false);
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                try
                {
                    await client.SendAsync(message);
                    return true;
                }
                catch (Exception)
                {
                    //Logging information
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
            return false;
        }


        public bool SendEmailPasswordReset(string userEmail, string link)
        {

            var message = new MimeMessage();
            //от кого отправляем и заголовок
            message.From.Add(new MailboxAddress("WebApplication1 Project", "enykoruna1@gmail.com"));
            //кому отправляем
            message.To.Add(new MailboxAddress("Имя Человека", userEmail));
            //тема письма
            message.Subject = "Сброс пароля на сайте WebApplication1";
            //тело письма
            message.Body = new TextPart("html")
            {
                Text = link,
            };
            using (var client = new SmtpClient())
            {
                //Указываем smtp сервер почты и порт
                client.Connect(_emailConfig.SmtpServer, 587, false);
                //Указываем свой Email адрес и пароль приложения
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception)
                {

                    //Logging information
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
            return false;
        }
    }
}