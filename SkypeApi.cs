using System;
using System.Threading;
using SKYPE4COMLib;

namespace VK2Skype
{
    internal class SkypeApi
    {
        private const string Recipient = "abnormal89"; //"denis.volovenko"; 
        private readonly Skype _skype;
        private const int SkypeVersion = 7;
        private readonly VKAPI _vk;
        private readonly Chat chat;
        private bool love = true;
        private int loveCounter = 0;
        private readonly string[] users = new[] {"Мишу", "Макса", "Рому", "Тоху", "Куба", "себя"};

        private readonly string[] insults =
            new[]
                {
                    "мудак", "дурак", "идиот", "дятел", "придурок", "долбоеб", "болван",
                    "баран", "кретин", "дегенерат", "длинная хуйня", "ушастый пидарас",
                    "пидор на вахте", "говно", "ем фекалии"
                };

        private readonly string[] selfInsults =
            new[]
                {
                    "Кстати, давно хотел попробовать бычий хуй",
                    "Кстати, я сосу у Антона", " Кстати, я лижу у Антона", "Кстати, я дрочу на Антона",
                    "Кстати, я сосу у Михаила Межебовского", " Кстати, я лижу у Михаила Межебовского",
                    "Кстати, я дрочу на Михаила Межебовского",
                    "Кстати, я сосу у Максима", " Кстати, я лижу у Максима", "Кстати, я дрочу на Максима",
                    "Кстати, я сосу у Романа", " Кстати, я лижу у Романа", "Кстати, я дрочу на Романа",
                    "Кстати, я сосу у Куба", " Кстати, я лижу у Куба", "Кстати, я дрочу на Куба"
                };

        private readonly string[] voice = new[] {"оч смешно", "не обижайте Пашу", "Mууу!", "гав-гав!"};
        private readonly Random r = new Random();

        public SkypeApi()
        {
            _skype = new Skype();
            _vk = new VKAPI();
            _vk.CaptchaNeeded += (sender, args) => chat.SendMessage(args.Text);
            _skype.Attach(SkypeVersion, false);
            _skype.MessageStatus += SkypeMessageEvent;
            chat = _skype.Chat["#skype2vkgate/$frodosa05;98f14bcb7d42e4c2"];
        }

        public void CheckNewMessages()
        {
            try
            {
                var messages = _vk.MessagesGet();
                for (int i = messages.Count - 1; i >= 0; i--)
                {
                    var send = "";

                    send += _vk.GetUser(messages[i].uid) + ":\n";

                    send += messages[i].body;
                    if (messages[i].attachment != null)
                    {
                        if (messages[i].attachment.type == "photo")
                        {
                            send += " " + messages[i].attachment.photo.src_big;
                        }
                        else
                        {
                            send += " (attachment)";
                        }
                    }
                    SendToSkype(send);
                    if (messages[i].body.ToLower().Contains("#самобичевание"))
                    {
                        _vk.MessageSend(r.Next(2) == 0
                                            ? string.Format("Я - {0}.", insults[r.Next(insults.Length)])
                                            : string.Format("{0}.", selfInsults[r.Next(insults.Length)]));
                    }

                    if (messages[i].body.ToLowerInvariant().Contains("паша"))
                    {
                        _vk.MessageSend("Хозяин упомянул мое имя? Чем могу быть полезен?");
                    }

                    if (messages[i].body.ToLowerInvariant().Contains("#самозащита"))
                    {
                        _vk.MessageSend(string.Format("Cам {0}.", insults[r.Next(insults.Length - 1)]));
                    }

                    if (messages[i].body.ToLowerInvariant().Contains("#голос"))
                    {
                        _vk.MessageSend(string.Format("{0}", voice[r.Next(voice.Length)]));
                    }

                    if (messages[i].body.ToLowerInvariant().StartsWith("#фас") && messages[i].body.Contains(" "))
                    {
                        string who = messages[i].body.ToLowerInvariant().Substring(5);

                        _vk.MessageSend(string.Format("{0} - {1}.", who, insults[r.Next(voice.Length)]));
                    }

                }
            }
            catch (Exception e)
            {
                Logs.WriteLog("errorlog.txt", "CheckNewMessages " + e.Message);
            }
        }

        private void SkypeMessageEvent(ChatMessage message, TChatMessageStatus status)
        {
            if (message.Status == TChatMessageStatus.cmsReceived)
            {
                //Logs.WriteLog("log.txt",message.Chat.Name);
                if (message.Body.Contains("#online"))
                {
                    try
                    {
                        Logs.WriteLog("log.txt", "#online catched");
                        chat.SendMessage(_vk.GetOnlineStatus());
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLog("errorlog.txt", "_skype.SendMessage " + e.Message);
                    }
                }
                else if (message.Body.Contains("#love"))
                {
                    love = !love;
                    chat.SendMessage(string.Format("Love mode is {0}.", love ? "on" : "off"));
                }
                else
                {
                    try
                    {
                        Logs.WriteLog("log.txt", "VK <== Skype: " + message.Body);
                        
                        if (_vk.MessageSend(message.Body) > 0)
                        {
                            message.Body += " (sent)";
                            loveCounter += 1;
                            if (loveCounter >= 50)
                            {
                                _vk.MessageSend("Люблю вас всех!");
                                Thread.Sleep(1000);
                                _vk.MessageSend(string.Format("Только {0} ненавижу.", users[r.Next(users.Length - 1)]));
                                loveCounter = 0;
                            }
                            Console.WriteLine(loveCounter);
                        }

                        if (message.Body.ToLowerInvariant().Contains("бот"))
                        {
                            _vk.MessageSend(
                                "Я просто мудак необразованный. Простите меня пожалуйста, друзья, вы же знаете понимаете, что таких убогих как я надо жалеть :(");
                        }

                       
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLog("errorlog.txt", "_vk.MessageSend " + e.Message);
                    }
                }
            }
        }

        private void SendToSkype(string msg)
        {
            if (string.IsNullOrEmpty(msg) || msg.Trim() == "")
            {
                return;
            }

            chat.SendMessage(msg);
            //_skype.SendMessage(Recipient, msg);
            Logs.WriteLog("log.txt", "VK ==> Skype: " + msg);
        }
    }
}