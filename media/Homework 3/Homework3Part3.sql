-- List of usernames

SELECT "username"
FROM "users"
;

-- Number of sent messages by user

SELECT
    "users"."username",
    coalesce("sub"."messages_count", 0) AS "Total messages sent"
FROM
    "users"
    LEFT JOIN (
        SELECT
            "from" AS "user_id",
            count("id") AS "messages_count"
        FROM
            "messages"
        GROUP BY
            "from"
    ) AS "sub" ON "users"."id" = "sub"."user_id"
ORDER BY
    "Total messages sent" DESC
;

-- The most messages received user and the total number of received messages

SELECT
    "users"."username",
    "max_counts"."message_count" AS "Total messages received"
FROM
    (
        SELECT
            "to" AS "user_id",
            count("id") AS "message_count"
        FROM
            "messages"
        GROUP BY
            "to"
        ORDER BY
            "message_count" DESC
        LIMIT 1
    ) AS "max_counts"
    JOIN
    "users" ON "users"."id" = "max_counts"."user_id"
;

-- Average messages sent per user

SELECT
    avg("messages_count") AS "Average messages sent per user"
FROM (
         SELECT
             "from",
             count("id") AS "messages_count"
         FROM
             "messages"
         GROUP BY
             "from"
     ) AS "sub"
;