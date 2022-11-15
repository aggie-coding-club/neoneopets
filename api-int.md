# Interal API
This is the documentation for the unity to blazorserver communication channel.

# Endpoints
## GET /api/account/login
Does not require authorization header.

If a request is made here with an authorization header 
that contains a valid token, the token will be returned
and its session will be refreshed. If the request 
contains a token, the parameters will be ignored.

If a request is made here for a user that already
has a valid and open session, but the new login request
does not contain an authorization header, the old
session will be expired and a new one will be generated.
### Parameters
- `username`: string; the username of the account
- `password`: string; the user's password
### Response
#### success
```json
{
    "result": "success",
    "token": "string"
}
```
#### Failure
```json
{
    "result": "failure",
    "issue": "string"
}
```
Possible values for `issue` are:
- `"username"` meaning that the specified username is not associated with an existing account.
- `"password"` meaning that the password did not match the username given.
- `"token"` meaning that the authorization token was not valid.
## PUT /api/account/create
Does not require authorization header.
### Request Body
```json
{
    "username": "string",
    "password": "string"
}
```
### Response
#### Success
```json
{
    "result": "success",
    "token": "string"
}
```
#### Failure
```json
{
    "result": "failure",
    "issue": "string"
}
```
Possible values for `issue` are:
- `"username"`: username already exists.
- `"password"`: password failed validation, usually menaing that the string was not valid utf-8 or was too short.
## GET /api/leaderboard
Gets the leaderboards.
### Parameters
- `game`: string; represents the name of the game to get the leaderboard for
### Response
```json
[
    {
        "time": "string",
        "score": "number"
    }
]
```
## GET /api/scores
Gets highscores for current user.
### Parameters
- `game`: string; represents the name of the game
### Response
```json
{
    "game": "string",
    "time": "string",
    "score": "number"
}
```
## POST /api/scores
### Request Body
```json
{
    "game": "string",
    "score": "number"
}
```
### Response
Response will use http status codes
- `201` indicates that the request succeeded annd the score has been added.
- `400` indicates that the specified game or score was not valid.
- `401` indicates that the authorization token was invalid or not provided.
- `500` indicates that something went wrong and it is not the client's fault.
