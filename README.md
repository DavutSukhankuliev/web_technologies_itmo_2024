# Description
This is a repo for hometasks from university. Here are some solutions to make a telegram bot which answers your questions. AI models were used in this project are from [HuggingFace](https://huggingface.co).

---

# Contents

1. [Hometask 2](#hometask-2)
   1. [Part 0](#part-0)
   2. [Part 1](#part-1)
   3. [Part 2](#part-2)
   4. [Task for Devs](#task-for-devs)
   5. [Instructions](#instructions-for-reproducing)
2. [Hometask 3](#hometask-3)
   1. [Part 0](#part-0-1)
   2. [Part 1](#part-1-1)
   3. [Part 2](#part-2-1)

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
&#9989; Done | Весь проект на гитхабе уже и есть ответ на part2

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

---

# Hometask 3

---

## Part 0
**Задание:** Создаем базу данных

**Ответ:** &#9989; Done | Создана - [Ссылка на YouTube видео](https://youtu.be/foVtHpNyNsM)

---

## Part 1
**Задание:** Создаем базу данных.
> Необходимо создать эндпоинт который принимает запрос с телом:
> ```json
> {
>   "username": "alex",
>   "password": "password"
> }
> ```
> И возвращает:
> ```json
> {
>   "id": 1,
>   "username": "alex"
> }
> ```
**Ответ:** &#9989; Done | 
Создал эндпоинт `/api/register-user` для добавления пользователя в БД и эндпоинт `/api/check-user` для псевдоавторизации юзера. - [Ссылка на YouTube видео](https://youtu.be/AhdjxJg93i8)
> Эндпоинт `/api/register-user` требует соблюдение вышеупомянутой структуры json и шифрует пароль при помощи SHA256 и генерит соль. Затем отправляет все в бд при помощи API (вариант с реализацией через `conenctionString` тоже есть).
> 
> Ответом приходит `StatusCode 200` с информацией о том, что юзер "зарегистрирован". Запись в БД появляется.
> 
> Эндпоинт `/api/check-user` требует соблюдение вышеупомянутой структуры json и сверяет креды с существующей (если есть) записью. И ответом отправляет, что пользователь смог/не смог авторизоваться. При этом возвращает вышеупомянутый json.

---

## Part 2
**Задание:** Необходимо создать эндпоинт который принимает POST запрос с телом вида:
```json
{
  "username": "alex",
  "password": "password",
  "message": {
    "to": "admin", 
    "text": "Hello"
  }
}
```
Который в случае успеха возвращает (id числом):
```json
{
  "message_id": "id"
}
```
**Ответ:** &#9989; Done |
Создал эндпоинт `/api/send-message` по тз в part2 - [Ссылка на YouTube видео](https://youtu.be/AhdjxJg93i8)
> - Перед отправкой сообщения, проверяет креды пользователя;
> - Потом проверяет существование получателя;
> - И потом отправляет само сообщение;
> - И потом возвращает message_id в json

---

## Part 3
**Задание:** Нужно написать SQLQuery для:
1. получить список юзернеймов пользователей
2. получить кол-во отправленных сообщений каждым пользователем:
   `username - number of sent messages`
3. Получить пользователя с самым большим кол-вом полученных сообщений и само количество:
   `username - number of received messages`
4. Получить среднее кол-во сообщений, отправленное каждым пользователем

В ответе написать использованные sql queries, обернуть их соответствующим markdown стилем

**Ответ:** &#9989; Done |
Ответ в файле [Homework3Part3.sql](media/Homework%203/Homework3Part3.sql)

[Ссылка на YouTube видео](https://youtu.be/0K_jR22YErQ)
> Пример markdown:
> ```sql
> -- Here is the list of usernames
> SELECT username
> FROM users
> ```
