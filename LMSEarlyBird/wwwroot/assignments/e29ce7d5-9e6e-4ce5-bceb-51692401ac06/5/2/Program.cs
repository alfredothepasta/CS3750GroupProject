using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace MongoTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mongodbUrl = "mongodb+srv://myAtlasDBUser:spider48@myatlasclusteredu.vzzawez.mongodb.net/?retryWrites=true&w=majority";
            var client = new MongoClient(mongodbUrl);
            //------------------------------------

            IMongoDatabase database = client.GetDatabase("sample_airbnb");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("listingsAndReviews");

            //How many rentals allow for a minimum of two nights? 
            var filter = Builders<BsonDocument>.Filter.Eq("minimum_nights", "2");
            long count = collection.CountDocuments(filter);
            Console.WriteLine($"Number of rentals with a minimum of two nights: {count}");

            Console.WriteLine("----------------------------------");

            //List 5 rentals with at least 5 beds and at least 2 bathrooms
            filter = Builders<BsonDocument>.Filter.Gte("bedrooms", 5) &
                Builders<BsonDocument>.Filter.Gte("bathrooms", 2);

            var list = collection.Find(filter).ToList().Take(5);
            Console.WriteLine("\nFive rentals with at least 5 beds and 2 bath:");
            foreach(var item in list)
            {
                string name = item["name"].ToString();

                Console.WriteLine(name);
            }

            Console.WriteLine("----------------------------------");

            //List the name of 3 rentals with a property type of 'house'
            filter = Builders<BsonDocument>.Filter.Eq("property_type", "House");
            list = collection.Find(filter).ToList().Take(3);
            Console.WriteLine("\nList the name of 3 rentals with a property type of 'House':");
            foreach (var item in list)
            {
                string name = item["name"].ToString();

                Console.WriteLine(name);
            }

            Console.WriteLine("----------------------------------");

            //Find a rental with a minimum of 1 night that accommodates 1. List name and url
            filter = Builders<BsonDocument>.Filter.Eq("minimum_nights", "1") &
                Builders<BsonDocument>.Filter.Eq("accommodates", 1);
            list = collection.Find(filter).ToList().Take(1);
            Console.WriteLine("\nFind a rental with a minimum of 1 night that accommodates 1");
            foreach (var item in list)
            {
                string name = item["name"].ToString();
                string url = item["listing_url"].ToString();

                Console.WriteLine("Name: " + name + "\nURL: " + url);
            }

            Console.WriteLine("----------------------------------");

            //List the URL and bed type of 4 rentals that have "Read Beds"
            filter = Builders<BsonDocument>.Filter.Eq("bed_type", "Real Bed");
            list = collection.Find(filter).ToList().Take(4);
            Console.WriteLine("\nList the URL and bed type of 4 rentals that have \"Read Beds\"");
            foreach (var item in list)
            {
                string bed = item["bed_type"].ToString();
                string url = item["listing_url"].ToString();

                Console.WriteLine("Bed Type: " + bed + "\nURL: " + url + '\n');
            }

            Console.ReadLine();

        }
    }
}
