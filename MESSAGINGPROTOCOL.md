# Technical specifications for the League Sockets Messaging Protocol (LSMP)
Websocket messages should conform to the following pattern:

```
DO <verb>
FROM <subject>
TO <object>
IN <location>
WITH
<message>
```

- Verb: The requested action
- Subject: The entity that requested the action
- Object: The entity on which the action was requested
- Location: The channel the request was performed in, and/or for alerts to be sent in
- Message: The text content of request or alert

### Details
- Each request must contain and start with the `DO` parameter.
- Each parameter (`DO`, `FROM`...etc) should be in their own line with their respective arguments (verb, subject...etc).
- The `WITH` parameter must be in a line of its own, it must also be the last parameter in the request, everything after the line until the end of the request will be considered as the argument (the message body/content).
- Parameter keywords like `DO` and `FROM`, and verbs are case**IN**sensitive. However, it is advised to use uppercase in order to standardize the format.
- Depending on the verb, certain parameters may be omitted. For example, an ALERT is sent from the server and thus doesn't need a `FROM` parameter. The parameter should be ignored in such cases even if provided by the request.

## Verb list (WIP)
\* `DO` is required for all requests.
- ### SEND: Send a message in a channel
   > Required params: `FROM <user-id>`, `IN <channel-id>`, `WITH <message>`
- ### ALERT: Send a server alert in a channel
   > Required params: `IN <channel-id>`, `WITH <message>`
- ### MUTE: Mute a user
   > Required params: `FROM <admin-id>`, `TO <user-id>`, `IN <channel-id>`  
   > Optional params: `WITH <message>`
- ### KICK: Kick a user
   > Required params: `FROM <admin-id>`, `TO <user-id>`  
   > Optional params: `IN <channel-id>`, `WITH <message>`
- ### BAN: Ban a user
   > Required params: `FROM <admin-id>`, `TO <user-id>`  
   > Optional params: `IN <channel-id>`, `WITH <message>`
- ### IPBAN: Ban an IP
   > Required params: `FROM <admin-id>`, `TO <ip>`  
   > Optional params: `IN <channel-id>`, `WITH <message>`
- ### CREATE: Create a channel
   > Required params: `FROM <admin-id>`, `WITH <channel-name>`
- ### DELETE: Delete a channel
   > Required params: `FROM <admin-id>`, `TO <channel-id>`
- ### AUTH: Authenticate a user
   > Required params: `FROM <auth-token>`

\* The `IN` parameter for infractions (mutes, kicks, bans...etc) specifies which channel to send the alert in. Thus, the `IN` and `WITH` parameters must either be both present, or both omitted. EXCEPT for mutes; mutes require the `IN` parameter to specify which channel the user is to be muted in.

## Example messages
```
DO SEND
FROM 74316
IN 47532
WITH
yoo good morning
how you been?
```
> Request to send a message from user with id `74316` in channel with id `47532`, with the message content being:  
> `yoo good morning`  
> `how you been?`

---
```
DO ALERT
IN 133
WITH
Okkio connected!
```
> Sends an alert from the server in channel with id `133` with the alert content being `Okkio connected!`

---
```
DO BAN
FROM 79
TO 94321
IN 23
WITH
Advertising is prohibited...
```
> Requests a ban from user with id `79` on user with id `94321`, displaying the message `Advertising is prohibited...` in channel with id `23`

---
```
DO CREATE
FROM 4553
WITH
general-chat
```
> Requests to create a channel with name `general-chat` from user with id `4553`

---
```
DO AUTH
FROM dbfde5c9-e59a-48ed-ae87-ffcbfb526775
```
> Requests authentication from the server using the token `dbfde5c9-e59a-48ed-ae87-ffcbfb526775`