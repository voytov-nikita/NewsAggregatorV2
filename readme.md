# Docker setup
## PostgreSql
```
docker run -d --name postgres-news -p 5432:5432 --restart unless-stopped -e POSTGRES_PASSWORD=Qwerty123$% -v d:\news-data\postgresql:/var/lib/postgresql/data postgres:17
```
## Seq
```
docker run -d --name seq-news -p 5341:80 --restart unless-stopped -e ACCEPT_EULA=Y -v d:\news-data\seq:/data datalust/seq:latest
```
## Mongo
```
docker run -d --name mongodb-news -p 27017:27017 --restart unless-stopped -v d:\news-data\mongodb:/data/db mongo mongod --replSet rs0
```
## MessageQueue
```
TODO: Add in future
```