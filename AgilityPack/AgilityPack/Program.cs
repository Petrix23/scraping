using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AgilityPack;
using HtmlAgilityPack;

namespace AgilityPack
{
    class OtodomRoomCreator
    {
        public Offer CreateOffer(HtmlNode node)
        {
            string image = string.Empty;
            var singleNode = (node.SelectSingleNode(".//figure").Attributes["data-quick-gallery"]);
            if (singleNode == null)
            {
                image = null;
            }
            else
            {
                string ImageTemp = singleNode.Value
                    .Replace("[{\"photo\":\"", "");
                image = ImageTemp.Remove(ImageTemp.IndexOf(";"));
            }

            Offer offer = new Offer
            {
                Name = node.SelectSingleNode(".//span[@class='offer-item-title']").InnerHtml,
                Price = node.SelectSingleNode(".//li[@class='offer-item-price']").InnerHtml,
                Image = image,
                Link = node.SelectSingleNode(".//a").Attributes["href"].Value,
                City = "Olsztyn",
                Portal = "otodom"
            };
            return offer;
        }
    }
    public class BaseModel
        {
            public string Id { get; set; }
            public bool IsActive { get; set; } = true;
        }
    
    public class Offer : BaseModel
        {
    
            public string Name { get; set; }

        
            public string Price { get; set; }

 
            public string Description { get; set; }

     
            public string Image { get; set; }


            public string Link { get; set; }

            public string City { get; set; }
            public string Portal { get; set; }
        }
        class ScrapingServiceOtodom
        {
            public string Name { get; set; } = "otodom";

            public List<Offer> Scraper()
            {
                List<HtmlNode> nodesOtodom = new List<HtmlNode>();

                HtmlWeb web = new HtmlWeb();

                var pageCount = GetPageCount();

                for (int i = 1; i <= pageCount; i++)
                {
                    var html =
                        $"https://www.otodom.pl/sprzedaz/mieszkanie/olsztyn/?search[description]=1&search[dist]=0&search[subregion_id]=454&search[city_id]=210&nrAdsPerPage=72&page={i}";

                    var htmlDoc = web.Load(html);

                    HtmlNode[] nodes = htmlDoc.DocumentNode
                        .SelectNodes("//article[@data-featured-name='listing_no_promo']").ToArray();

                    foreach (HtmlNode node in nodes)
                    {
                        nodesOtodom.Add(node);
                    }
                    //to prevent ip ban
                   // Thread.Sleep(4000);
                }

                return GetDetails(nodesOtodom);
            }

            public int GetPageCount()
            {
                var html =
                    "https://www.otodom.pl/sprzedaz/mieszkanie/olsztyn/?search[description]=1&search[dist]=0&search[subregion_id]=454&search[city_id]=210&nrAdsPerPage=72&page=1";

                HtmlWeb web = new HtmlWeb();

                var htmlDoc = web.Load(html);

                HtmlNode pageCount = htmlDoc.DocumentNode.SelectSingleNode("//strong[@class='current']");

                return Convert.ToInt32(pageCount.InnerHtml);
            }

            public List<Offer> GetDetails(List<HtmlNode> nodes)
            {
                List<Offer> offers = new List<Offer>();
                OtodomRoomCreator creator = new OtodomRoomCreator();
                foreach (HtmlNode node in nodes)
                {
                    var room = creator.CreateOffer(node);

                    offers.Add(room);
                }

                return offers;
            }
        }
    

    class Program
    {
        static void Main(string[] args)
        {
            ScrapingServiceOtodom aa = new ScrapingServiceOtodom();
            List<Offer> b= aa.Scraper();
            foreach (var offer in b)
            {
                Console.WriteLine(offer.Name);
                Console.WriteLine(offer.Price);
            }

            Console.ReadKey();
        }
    }
}
