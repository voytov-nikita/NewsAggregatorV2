![Show highlevel relationships](Solution diagram.drawio.svg "Solution diagram")

# News Service

## General description

This main goal of this service is to give access to aggregated news and its comments.
Also, it writes aggregated news from Schedule Service and sends events to Notification Service

## Stack:
1. ASP.Net
2. PostgreSQL
3. Message Queue

## API description

### Default pager parameters*

1. OrderBy
2. OrderDirection
3. Offset
4. Take

---

### NewsController

### `GET /api/v1/news`

Parameters:

1. Default pager parameters*
2. Keyword

Response:

```json
{
  "Id": "0",
  "Title": "Some title for current news",
  "Description": "",
  "OriginalLink": "https://news.com/some-id",
  "DateTime": "Thu, 03 Apr 2025 23:05:55 +0300",
  "ImageLink": "https://media.com/some-id",
  "Publisher": "Some News Publisher",
  "PublisherLink": "https://news.com/"
}
```

### CommentsController

### `GET /api/v1/news/{newsId}/comments`

Parameters:

1. newsId - Id of news
2. Default pager parameters*

Response:

```json
{
  "Id": 1,
  "AuthorId": "9768cef3-bbf4-468c-8f18-d68c0b0c7256",
  "AuthorName": "phase6",
  "Content": "Some message",
  "CreateDate": "2026-08-30T23:02:28",
  "LastModifiedDate": null,
  "Likes": 12,
  "DisLikes": 4
}
```

### `POST /api/v1/news/{newsId}/comments`

Requires the `comment.write` permission. **The author is taken from the token, never
from the body** - an author id accepted from the client is impersonation.

Parameters:

1. newsId - Id of news

Request:
```json
{
  "Content": "Some message"
}
```

### `PUT /api/v1/news/{newsId}/comments/{commentId}`

Requires authentication. The caller must be the comment's author, or hold
`comment.moderate`; otherwise the response is 403.

Parameters:

1. newsId - Id of news
2. commentId - Id of comment

Request:
```json
{
  "Content": "Some message"
}
```

### `DELETE /api/v1/news/{newsId}/comments/{commentId}`

Same authorization rule as the update above. Responds 204.

### `PUT /api/v1/news/{newsId}/comments/{commentId}/like`

Requires authentication. No body.

Parameters:

1. newsId - Id of news
2. commentId - Id of comment

### `PUT /api/v1/news/{newsId}/comments/{commentId}/dislike`

Requires authentication. No body.

Parameters:

1. newsId - Id of news
2. commentId - Id of comment

### NewsVotesController

### `POST /api/v1/news/{newsId}/vote`

Requires the `news.vote` permission. One row per (news, user), enforced by a unique
index, so voting twice the same way changes nothing.

Request:
```json
{ "value": 1 }
```

`value` is `1` (like), `-1` (dislike) or `0` (retract). Response:

```json
{ "likes": 12, "dislikes": 4, "myVote": 1 }
```

### `GET /api/v1/news/{newsId}/vote`

Anonymous. Returns the same shape, with `myVote` 0 for a caller without a token.

---

# Crawler Service

## General description
Regularly sends requests to RSS sources, parses them and send to queue on write.

## Stack:
1. ASP.Net
2. MongoDb
3. Message Queue
4. Hangfire

---

# Notification Service

## General description
Notify users from events, that appears in queue. Can subscribe webhooks and, in the future, notify in telegram and by email.

## API description

### Default pager parameters*

1. OrderBy
2. OrderDirection
3. Offset
4. Take

### `GET /api/v1/webhooks`

Parameters:

1. Default pager parameters*

Response:

```json
{
  "GuidId": "ab12cd34",
  "WebhookUrl": "gl12dD5v",
  "EventType": "NewsUpdated",
  "DateStamp": "5/6/2005 09:34:42 PM"
}
```

### `POST /api/v1/webhooks`

Request:
```json
{
  "WebhookUrl": "http://webhook/some-webhook-id",
  "EventType": "NewsUpdated"
}
```

Response:

```json
{
  "GuidId": "ab12cd34",
  "WebhookUrl": "gl12dD5v",
  "EventType": "NewsUpdated",
  "DateStamp": "5/6/2005 09:34:42 PM"
}
```


## Stack:
1. ASP.Net
2. MongoDb
3. Message Queue
4. Hangfire
---

# Auth Service

## General description

Owns users, roles, permissions and refresh tokens for the whole platform. Issues RS256 JWTs;
the other three services validate them through the shared `Common.Auth` library against this
service's JWKS, so key material never leaves AuthService.

Authorization is **permission-based**, not role-based: roles map to permissions, permissions
travel in the token as `permission` claims, and endpoints opt in with
`[HasPermission(Permissions.CommentWrite)]`. Reading the site stays anonymous by design.

## Stack:
1. ASP.Net
2. PostgreSQL (shared `local-news` database, schema `authService`)
3. ASP.NET Core Identity

## Permissions

| Permission | `User` | `Admin` |
|---|---|---|
| `news.vote` | yes | yes |
| `comment.write` | yes | yes |
| `comment.moderate` | | yes |
| `sources.manage` | | yes |
| `stats.view` | | yes |
| `webhooks.manage` | | yes |

## API description

### AuthController

### `POST /api/v1/auth/register`

Anonymous. Creates the account in the `User` role.

Request:
```json
{
  "Email": "someone@example.com",
  "UserName": "someone",
  "Password": "Passw0rd!"
}
```

Response: `200` with the body below, plus the `na_rt` refresh cookie.

```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6...",
  "tokenType": "Bearer",
  "expiresIn": 900,
  "user": {
    "id": "9768cef3-bbf4-468c-8f18-d68c0b0c7256",
    "email": "someone@example.com",
    "userName": "someone",
    "displayName": null,
    "roles": ["User"],
    "permissions": ["news.vote", "comment.write"]
  }
}
```

### `POST /api/v1/auth/login`

Anonymous. Same response shape as register. Failures are uniform: an unknown email and a wrong
password both return `401 Invalid email or password`, so the endpoint cannot be used to enumerate
accounts. A locked-out account returns `403`.

### `POST /api/v1/auth/refresh`

Anonymous, no body - the refresh token is read from the `na_rt` cookie. Rotates on every use: the
old row is marked `Rotated` and a new one issued in the same family. Presenting an already-revoked
token means it leaked, so the whole `FamilyId` chain is revoked as `ReuseDetected` and the response
is `401`.

### `POST /api/v1/auth/logout?allDevices=false`

Anonymous, idempotent, always `204`. Revokes the refresh token; **the access token stays valid until
it expires** - inherent to stateless JWT, mitigated by the 15-minute lifetime.

### `GET /api/v1/auth/me`

Requires a bearer token. Returns the user profile plus `createdAt` and `lastLoginAt`.

### Discovery

- `GET /.well-known/openid-configuration`
- `GET /.well-known/jwks.json` - public key parameters only (`kty`, `use`, `alg`, `kid`, `n`, `e`)

## Tokens

**Access token** - RS256, 15 minutes, audience `news-aggregator`, claims `sub`, `email`, `name`,
`jti`, one `role` per role and one `permission` per permission. Permission changes therefore take
effect on the next refresh, not instantly.

**Refresh token** - opaque 32 random bytes, 14 days, single use, stored as a plain SHA-256 hash.
Not a slow KDF: those exist to protect *low-entropy* secrets, a 256-bit random token cannot be
brute-forced, and a salted hash could not be looked up by index.

**Signing key** - `Jwt:KeysPath/signing-key.pem`, generated on first run and persisted. The `kid` is
derived from the public key, so it is stable across restarts; a random per-startup `kid` would
silently invalidate every outstanding token. Rotation: move the active key to
`keys/retired/<kid>.pem` and restart - retired keys stay published in JWKS while the new one signs.
