# Description
This is a repo for hometasks from university. Here are some solutions to make a telegram bot which answers your questions. AI models were used in this project are from [HuggingFace](https://huggingface.co).

---

# Contents

1. [Hometask 2](#hometask 2)
   1. [Part 0](#part-0)
   2. [Part 1](#part-1)
   3. [Part 2](#part-2)
   4. [Task for Devs](#task-for-devs)
   5. [Instructions](#instructions-for-reproducing)

---

# Hometask 2

---

## Part 0
**Задание:** Переработать вашу третью buildship схему из прошлого дз, чтобы на вход она принимала input, вызывала huggingface модель и отправляла результат в телеграм чат

**Ответ:**
![Homework2Part0 GIF](media/Homework%202/Homework2Part0.gif)

---

## Part 1
### Вопрос 1
Зачем нужен ssh? Ответ на пару предложений.

> SSH (Secure Shell) - это сетевой протокол, который предоставляет безопасный способ удаленного доступа к серверу. Он обеспечивает защиту соединения посредством шифрования данных, что позволяет безопасно управлять серверами, передавать файлы и выполнять команды на удаленных компьютерах.

### Вопрос 2
Предположим, у вас есть прямой доступ к серверу(терминалу) под управлением ubuntu. У вас есть коллега Вася, который хочет получить доступ к этому серверу. Он генерирует пару ssh ключей с помощью команды ssh-keygen и дает вам свой публичный ключ. В какой файл на сервере нужно записать ключ, чтобы Вася смог подключиться к терминалу сервера?

> Публичный ключ Васи необходимо добавить в файл `/home/вашего_пользователя/.ssh/authorized_keys` на сервере. Для этого можно открыть этот файл и добавить ключ в новую строку.

### Вопрос 3
Тут вопрос про АПИ. Разберитесь, что такое long polling и webhooks, опишите сами в нескольких предложениях, как они работают.

**Long polling:**
> Long polling - это метод взаимодействия между клиентом и сервером, при котором клиент делает HTTP-запрос к серверу и ждет ответа до тех пор, пока на сервере не появятся новые данные. Как только серверу будет что передать, ответ отправляется клиенту, который затем немедленно создает новый запрос для продолжения этого процесса.

**Webhooks:**
> Webhooks - это механизм, с помощью которого сервер посылает HTTP POST-запросы на заранее указанный клиентом URL (конечную точку) при возникновении событий. Это позволяет клиенту немедленно получать уведомления о событиях, без необходимости постоянно запрашивать сервер.

### Вопрос 4
Найдите информацию, что такое issues на гитхабе и для чего нужны. Также вставьте ссылки на пару примеров issues в популярных open source проектах.

Issues на GitHub:
> Issues на GitHub - это инструмент, используемый для отслеживания работы, задач, багов и предложений в проекте. Они позволяют разработчикам и пользователям проекта обсуждать и документировать проблемы или улучшения.

Вот хороший пример issues:
> [ApplovinMAX Unity Plugin Issue #318](https://github.com/AppLovin/AppLovin-MAX-SDK-iOS/issues/318) - вьетнамские флешбеки оказались сильнее

### Вопрос 5
Ваш проект используется пустую папку images, но гит не поддерживает отслеживание пустых директорий. Что делать?

> Чтобы Git отслеживал пустую директорию, можно создать в ней скрытый файл, например, `.gitkeep`. Этот файл не несет полезной нагрузки, но заставляет Git отслеживать директорию. Просто создайте файл следующим образом:
> ```git
> touch images/.gitkeep
> ```
> Затем добавьте и закоммитьте этот файл в репозиторий:
> ```git
> git add images/.gitkeep
> git commit -m "Added .gitkeep for empty folder images"
> ```

---

## Part 2
Весь проект на гитхабе уже и есть ответ на part2

---

## Task for devs

1. &#9989; Done | Create a [readme.md](README.md) and answer questions in it
2. &#9989; Done | Write a script for telegram bot which:
   1. &#9989; Done | can handle user messages/prompts
   2. &#9989; Done | sends the message/prompt to hugging face stable diffusion model
   3. &#9989; Done | gets the image and sends it back to user
3. &#9989; Done | Record a screencast and push the repo
![Homework2Part0 GIF](media/Homework%202/Homework2PartDev.gif)

---

## Instructions for reproducing

You can see in a short screencast giff above or here's a text instruction:
1. Before you start, please, make sure that you set all configurations in [appsettings.json](web_technologies_itmo_2024/appsettings.json) or [appsettings.Development.json](web_technologies_itmo_2024/appsettings.Development.json) according to your environment.
2. Start the WebServer
3. Make sure that your endpoint is accessible to telegram webhook. Ex: I used it for http port `lt --port *YOUR_PORT* --subdomain *YOUR_DOMAIN*`
4. Make sure to set your Telegram webhook. Ex: `curl -F "url=https://*YOUR_DOMAIN*.loca.lt/api/telegram-bot-update-receiver" https://api.telegram.org/bot*YOUR_TELEGRAM_BOT_API_KEY*/setWebhook`
5. Try your bot in private chat or group chats!
6. For debugging, please, refer to debug logs in console.