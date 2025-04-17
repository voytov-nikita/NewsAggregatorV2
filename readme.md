# Docker setup
## PostgreSql
```
docker run -d -p 5432:5432 --name postgres-news --restart unless-stopped -e POSTGRES_PASSWORD=Qwerty123$% -v d:\news-data\postgresql:/var/lib/postgresql/data postgres:17
```
## Seq
```
TODO: Add in future
```
## Mongo
```
TODO: Add in future
```
## MessageQueue
```
TODO: Add in future
```