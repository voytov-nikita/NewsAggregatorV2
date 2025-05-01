using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace CrawlerService.DAL.Migrations;

public class M1_00_00_test: DatabaseMigration
{
    public M1_00_00_test() : base("1.00.00")
    {
        
    }

    public override void Up(IMongoDatabase db)
    {
        db.CreateCollectionAsync("test_news");
        Test test1 = new Test
        {
            Id = 0,
            Value = 9877856,
            StringValue = "TEST"
        };
        Test test2 = new Test
        {
            Id = 1,
            Value = 124214,
            StringValue = "not null"
        };
        Test test3 = new Test
        {
            Id = 2,
            Value = 5235213,
            StringValue = "ldasdasas"
        };

        var collection = db.GetCollection<Test>("test_news");
        
        collection.InsertMany([
            test1,
            test2,
            test3
        ]);
    }

    public override void Down(IMongoDatabase db)
    {
    }
}

public class Test
{
    public int Id { get; set; }
    public int Value { get; set; }
    public string StringValue { get; set; }
}