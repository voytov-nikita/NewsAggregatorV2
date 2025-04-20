# Docker setup
## PostgreSql
```
docker run -d -p 5432:5432 --name postgres-news --restart unless-stopped -e POSTGRES_PASSWORD=Qwerty123$% -v d:\news-data\postgresql:/var/lib/postgresql/data postgres:17
```
## Seq
```
docker run --name seq-news -d -v d:\news-data\postgresql\seq:/data --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```
## Mongo
```
TODO: Add in future
```
## MessageQueue
```
TODO: Add in future
```