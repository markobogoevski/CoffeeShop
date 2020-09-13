namespace CoffeeShop.Migrations
{
    using CoffeeShop.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CoffeeShop.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CoffeeShop.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            // Adding Roles to DB
            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Administrator" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "User"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "User" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Owner"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Owner" };

                manager.Create(role);
            }

            // Adding Users to DB

            if (!context.Users.Any(u => u.UserName == "admin@test.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "admin@test.com" };

                manager.Create(user, "Password1!");
                manager.AddToRole(user.Id, "Administrator");
            }


            if (!context.Users.Any(u => u.UserName == "user@test.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "user@test.com" };

                manager.Create(user, "Password1!");
                manager.AddToRole(user.Id, "User");
            }

            if (!context.Users.Any(u => u.UserName == "owner@test.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "owner@test.com" };

                manager.Create(user, "Password1!");
                manager.AddToRole(user.Id, "Owner");
            }


            // Adding Ingredients to DB

            var Cinnamon = new IngredientModel
            {
                IngredientId = new Guid("75a9cb73-e75b-4ca9-9bab-c01409785a32"),
                Name = "Cinnamon",
                Description = "If you’re a typical milk and sugar coffee drinker, cinnamon will transform your morning joe. Besides for cutting the calories and adding a punch-y sweetness to your drink, cinnamon will also help boost your immune system and lower blood sugar levels.",
                ImgUrl = "https://upload.wikimedia.org/wikipedia/commons/d/de/Cinnamomum_verum_spices.jpg",
                Price = 15,
                QuantityInStock = 100
            };
            var Butter = new IngredientModel
            {
                IngredientId = new Guid("75a9cb74-e75b-4ca9-9bab-c01409785a32"),
                Name = "Butter",
                Description = "Dave Asprey, the founder of the Bulletproof Coffee brand, claims that drinking coffee with butter gives your body fast quick energy that it will in turn use and burn. And once your body’s in fat burning mode, it’ll take on all that stored fat as well.",
                ImgUrl = "https://cimg3.ibsrv.net/cimg/www.fitday.com/693x350_85-1/791/4butter-108791.jpg",
                Price = 10,
                QuantityInStock = 100
            };
            var IceCream = new IngredientModel
            {
                IngredientId = new Guid("75a9cb75-e75b-4ca9-9bab-c01409785a32"),
                Name = "Ice Cream",
                Description = "A scoop of ice cream in your coffee will of course add calories, but if you’re just looking to enhance the taste than this sounds incredible. ",
                ImgUrl = "https://upload.wikimedia.org/wikipedia/commons/b/bb/Grape-nut_ice_cream.jpg",
                Price = 20,
                QuantityInStock = 60
            };
            var Cardamom = new IngredientModel
            {
                IngredientId = new Guid("75a9cb76-e75b-4ca9-9bab-c01409785a32"),
                Name = "Cardamom",
                Description = "Just like cinnamon, cardamom is another spice that the average coffee drinker will appreciate. It adds a bit more spiciness than cinnamon but the sweet undertones will make you forget that you ever needed sugar to begin with. ",
                ImgUrl = "https://www.rigpawiki.org/images/c/ce/Cardamom.jpg",
                Price = 25,
                QuantityInStock = 80
            };
            var Milk = new IngredientModel
            {
                IngredientId = new Guid("75a9cb77-e75b-4ca9-9bab-c01409785a32"),
                Name = "Plain milk",
                Description = "Milk is a nutrient - rich liquid food produced in the mammary glands of mammals.It is the primary source of nutrition for infant mammals (including humans who are breastfed) before they are able to digest other types of food.",
                ImgUrl = "https://img.webmd.com/dtmcms/live/webmd/consumer_assets/site_images/article_thumbnails/reference_guide/living_with_a_milk_allergy_ref_guide/650x350_living_with_a_milk_allergy_ref_guide.jpg?resize=*:350px",
                QuantityInStock = 70
            };
            var SkimMilk = new IngredientModel
            {
                IngredientId = new Guid("75a9cb78-e75b-4ca9-9bab-c01409785a32"),
                Name = "Soy milk",
                Description = "Skimmed milk(British English), or skim milk(American English), is made when all the milkfat is removed from whole milk.It tends to contain around 0.1 % fat.",
                ImgUrl = "https://www.healthbenefitstimes.com/9/uploads/2018/04/Skimmed-milk.png",
                Price = 10,
                QuantityInStock = 70
            };
            var SoyMilk = new IngredientModel
            {
                IngredientId = new Guid("75a9cb79-e75b-4ca9-9bab-c01409785a32"),
                Name = "Soy milk",
                Description = "Soy milk, also known as soya milk or soymilk, is a plant-based drink produced by soaking and grinding soybeans, boiling the mixture, and filtering out remaining particulates. It is a stable emulsion of oil, water, and protein.",
                ImgUrl = "https://frigiv.palsgaard.com/media/1234/how-to-produce-delicious-soy-milk-with-palsgaard-recmilk-emulsifiers-and-stabilisers.jpg?width=2048.jpg",
                Price = 10,
                QuantityInStock = 70
            };
            var AlmondMilk = new IngredientModel
            {
                IngredientId = new Guid("75a9cb80-e75b-4ca9-9bab-c01409785a32"),
                Name = "Almond milk",
                Description = "Almond milk is a plant milk manufactured from almonds with a creamy texture and nutty flavor,[1] although some types or brands are flavored in imitation of dairy milk.[2] It does not contain cholesterol, saturated fat or lactose, and is often consumed by those who are lactose-intolerant and others, such as vegans, who avoid dairy product.",
                ImgUrl = "https://detoxinista.com/wp-content/uploads/2010/11/how-to-make-almond-milk-500x375.jpg",
                Price = 15,
                QuantityInStock = 70
            };
            var WhiteSugar = new IngredientModel
            {
                IngredientId = new Guid("75a9cb81-e75b-4ca9-9bab-c01409785a32"),
                Name = "White sugar",
                Description = "White sugar, also called table sugar, granulated sugar or regular sugar, is the sugar commonly used in North America and Europe, made either of beet sugar or cane sugar, which has undergone a refining process.",
                ImgUrl = "https://5.imimg.com/data5/GM/XV/MY-28241240/white-sugar-500x500.jpg",
                Price = 5,
                QuantityInStock = 100
            };
            var BrownSugar = new IngredientModel
            {
                IngredientId = new Guid("75a9cb82-e75b-4ca9-9bab-c01409785a32"),
                Name = "Brown sugar",
                Description = "Brown sugar is a sucrose sugar product with a distinctive brown color due to the presence of molasses. It is either an unrefined or partially refined soft sugar consisting of sugar crystals with some residual molasses content (natural brown sugar), or it is produced by the addition of molasses to refined white sugar (commercial brown sugar). ",
                ImgUrl = "https://www.thespruceeats.com/thmb/fXZNkr0P2kV9yTzXgIBgzAf_BqY=/2122x1194/smart/filters:no_upscale()/462076039-56a20eef5f9b58b7d0c61e41.jpg",
                Price = 7,
                QuantityInStock = 100
            };
            var Stevia = new IngredientModel
            {
                IngredientId = new Guid("75a9cb83-e75b-4ca9-9bab-c01409785a32"),
                Name = "Stevia",
                Description = "Stevia is a sweetener and sugar substitute derived from the leaves of the plant species Stevia rebaudiana, native to Brazil and Paraguay. The active compounds are steviol glycosides (mainly stevioside and rebaudioside), which have 30 to 150 times the sweetness of sugar, are heat-stable, pH-stable, and not fermentable.",
                ImgUrl = "https://post.healthline.com/wp-content/uploads/2020/09/Stevia_Powder_1200x628-facebook-1200x628.jpg",
                Price = 15,
                QuantityInStock = 60
            };
            var Chocolate = new IngredientModel
            {
                IngredientId = new Guid("75a9cb84-e75b-4ca9-9bab-c01409785a32"),
                Name = "Chocolate",
                Description = "Chocolate is a preparation of roasted and ground cacao seeds that is made in the form of a liquid, paste, or in a block, which may also be used as a flavoring ingredient in other foods.",
                ImgUrl = "https://i1.wp.com/www.eatthis.com/wp-content/uploads/2017/10/dark-chocolate-bar-squares.jpg?fit=1024%2C750&ssl=1.jpg",
                Price = 25,
                QuantityInStock = 80
            };
            var Caramel = new IngredientModel
            {
                IngredientId = new Guid("75a9cb85-e75b-4ca9-9bab-c01409785a32"),
                Name = "Caramel",
                Description = "Caramel is a medium to dark-orange confectionery product made by heating a variety of sugars. It can be used as a flavoring in puddings and desserts, as a filling in bonbons, or as a topping for ice cream and custard. ",
                ImgUrl = "https://storcpdkenticomedia.blob.core.windows.net/media/recipemanagementsystem/media/recipe-media-files/recipes/retail/x17/2018_aunt-emilys-soft-caramels_2300_760x580.jpg?ext=.jpg",
                Price = 20,
                QuantityInStock = 80
            };
            var CoconutOil = new IngredientModel
            {
                IngredientId = new Guid("75a9cb86-e75b-4ca9-9bab-c01409785a32"),
                Name = "Coconut oil",
                Description = "Coconut oil, or copra oil, is an edible oil extracted from the kernel or meat of mature coconuts harvested from the coconut palm (Cocos nucifera). It has various applications.",
                ImgUrl = "https://shayandcompany.com/pub/media/catalog/product/cache/d90cbd11ee0c14292e6bc8707faea3bb/o/r/organic_extra_virgin_coconut_oil_2.jpg",
                Price = 25,
                QuantityInStock = 50
            };
            var ArabicaBean = new IngredientModel
            {
                IngredientId = new Guid("75a9cb87-e75b-4ca9-9bab-c01409785a32"),
                Name = "Arabica bean",
                Description = "Coffea arabica, also known as the Arabian coffee, 'coffee shrub of Arabia', 'mountain coffee' or 'arabica coffee', is a species of Coffea. It is believed to be the first species of coffee to be cultivated, and is the dominant cultivar, representing about 60% of global production.",
                ImgUrl = "https://aaronwebstore.com/wp-content/uploads/2017/12/Arabica-coffee-bean.jpg",
                Price = 10,
                QuantityInStock = 80
            };
            var RobustaBean = new IngredientModel
            {
                IngredientId = new Guid("75a9cb88-e75b-4ca9-9bab-c01409785a32"),
                Name = "Robusta bean",
                Description = "Coffea canephora (syn. Coffea robusta), commonly known as robusta coffee, is a species of coffee that has its origins in central and western sub-Saharan Africa. It is a species of flowering plant in the family Rubiaceae.",
                ImgUrl = "https://5.imimg.com/data5/GX/YT/MY-45122889/robusta-coffee-beans-500x500.jpg",
                Price = 10,
                QuantityInStock = 80
            };
            var LibericaBean = new IngredientModel
            {
                IngredientId = new Guid("75a9cb89-e75b-4ca9-9bab-c01409785a32"),
                Name = "Liberica bean",
                Description = "Excelsa is a member of the Liberica family, but its species is incredibly distinct. Like the Liberica coffee described above, Excelsa is grown primarily in Southeast Asia and represents only a small fraction of the world’s coffee production. Excelsa does boast a tart, fruitier flavor and is known for showing attributes of both light and dark roast coffees to create a unique profile that is frequently sought out by coffee enthusiasts.",
                ImgUrl = "https://sc01.alicdn.com/kf/HTB15ePCJVXXXXaJXpXXq6xXFXXXq.jpg_350x350.jpg",
                Price = 10,
                QuantityInStock = 80
            };
            var ExcelsaBean = new IngredientModel
            {
                IngredientId = new Guid("75a9cb90-e75b-4ca9-9bab-c01409785a32"),
                Name = "Excelsa bean",
                Description = "Coffea liberica (or Liberian coffee) is a species of flowering plant in the family Rubiaceae from which coffee is produced. It is native to western and central Africa from Liberia to Uganda and Angola, and has become naturalized in the Philippines, Indonesia, Seychelles, the Andaman & Nicobar Islands, Malaysia.",
                ImgUrl = "https://images-na.ssl-images-amazon.com/images/I/51EGHd7Io-L._AC_SY400_.jpg",
                Price = 10,
                QuantityInStock = 80
            };

            context.Ingredients.AddOrUpdate(Cinnamon, Butter, IceCream,
                Cardamom,
                Milk,
                    SoyMilk,
                    AlmondMilk,
                    WhiteSugar,
                    BrownSugar,
                    Stevia,
                    Chocolate,
                    Caramel,
                    CoconutOil,
                    ArabicaBean,
                    ExcelsaBean,
                    LibericaBean,
                    RobustaBean
                );

            // Add Coffees to DB

            var Espresso = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb91-e75b-4ca9-9bab-c01409785a32"),
                Name = "Espresso",
                BasePrice = 15,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "Coffee made is a coffee of Italian origin, in which a small amount of nearly boiling water is forced under pressure (expressed) through finely-ground coffee beans. Espresso coffee can be made with a wide variety of coffee beans and roast levels.",
                Ingredients = new List<IngredientModel> {
                            ArabicaBean
                    },
                ImgUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Tazzina_di_caff%C3%A8_a_Ventimiglia.jpg/1024px-Tazzina_di_caff%C3%A8_a_Ventimiglia.jpg"
            };

            var DoubleEspresso = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb92-e75b-4ca9-9bab-c01409785a32"),
                Name = "Double Espresso",
                BasePrice = 30,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "Even more esspresso",
                Ingredients = new List<IngredientModel> {
                            ArabicaBean
                    },
                ImgUrl = "data:image/jpeg;base64,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"
            };

            var Machiatto = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb93-e75b-4ca9-9bab-c01409785a32"),
                Name = "Machiatto",
                BasePrice = 20,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "Caffè macchiato(Italian pronunciation[kafˈfɛ makˈkjaːto](listen)), sometimes called espresso macchiato, is an espresso coffee drink with a small amount of milk, usually foamed.In Italian, macchiato means stained or spotted so the literal translation of caffè macchiato is stained” or “marked coffee.”",
                Ingredients = new List<IngredientModel> {
                            ArabicaBean,Milk
                    },
                ImgUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c8/Cappuccino_at_Sightglass_Coffee.jpg/640px-Cappuccino_at_Sightglass_Coffee.jpg"
            };

            var Cappuccino = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb94-e75b-4ca9-9bab-c01409785a32"),
                Name = "Cappuccino",
                BasePrice = 25,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "A cappuccino is an espresso - based coffee drink that originated in Italy, and is traditionally prepared with steamed milk foam(microfoam).",
                Ingredients = new List<IngredientModel> {
                            ArabicaBean,Milk
                    },
                ImgUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c8/Cappuccino_at_Sightglass_Coffee.jpg/640px-Cappuccino_at_Sightglass_Coffee.jpg"
            };

            var RandomCoffee1 = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb95-e75b-4ca9-9bab-c01409785a32"),
                Name = "Random Coffee 1",
                BasePrice = 25,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "Random coffee 1.",
                Ingredients = new List<IngredientModel> {
                            BrownSugar,Chocolate,ExcelsaBean,SoyMilk
                    },
                ImgUrl = "https://images.immediate.co.uk/production/volatile/sites/30/2020/08/flat-white-3402c4f.jpg?quality=90&resize=960,872.jpg"
            };

            var RandomCoffee2 = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb96-e75b-4ca9-9bab-c01409785a32"),
                Name = "Random Coffee 2",
                BasePrice = 20,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "Random coffee 2.",
                Ingredients = new List<IngredientModel> {
                            WhiteSugar,Stevia,LibericaBean,AlmondMilk,CoconutOil
                    },
                ImgUrl = "https://images.immediate.co.uk/production/volatile/sites/30/2020/08/flat-white-3402c4f.jpg?quality=90&resize=960,872.jpg"
            };

            var RandomCoffee3 = new CoffeeModel
            {
                CoffeeId = new Guid("75a9cb97-e75b-4ca9-9bab-c01409785a32"),
                Name = "Random Coffee 3",
                BasePrice = 30,
                IncomeCoef = 1.2m,
                QuantityInStock = 30,
                Description = "Random coffee 3.",
                Ingredients = new List<IngredientModel> {
                            WhiteSugar,Chocolate,RobustaBean,SkimMilk
                    },
                ImgUrl = "https://images.immediate.co.uk/production/volatile/sites/30/2020/08/flat-white-3402c4f.jpg?quality=90&resize=960,872.jpg"
            };


            var coffee = new List<CoffeeModel> { Espresso, Machiatto, Cappuccino, DoubleEspresso, RandomCoffee1, RandomCoffee2, RandomCoffee3 };

            // Add IngredientInCoffee to DB
            var ArabicaBeanInEspresso = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba1-e75b-4ca9-9bab-c01409785a32"),
                Coffee = Espresso,
                CoffeeId = Espresso.CoffeeId,
                Ingredient = ArabicaBean,
                IngredientId = ArabicaBean.IngredientId,
                Quantity = 1
            };

            var ArabicaBeanInDoubleEspresso = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba2-e75b-4ca9-9bab-c01409785a32"),
                Coffee = DoubleEspresso,
                CoffeeId = DoubleEspresso.CoffeeId,
                Ingredient = ArabicaBean,
                IngredientId = ArabicaBean.IngredientId,
                Quantity = 2
            };

            var ArabicaBeanInMachiatto = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba3-e75b-4ca9-9bab-c01409785a32"),
                Coffee = Machiatto,
                CoffeeId = Machiatto.CoffeeId,
                Ingredient = ArabicaBean,
                IngredientId = ArabicaBean.IngredientId,
                Quantity = 1
            };

            var MilkInMachiatto = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba4-e75b-4ca9-9bab-c01409785a32"),
                Coffee = Machiatto,
                CoffeeId = Machiatto.CoffeeId,
                Ingredient = Milk,
                IngredientId = Milk.IngredientId,
                Quantity = 1
            };

            var ArabicaBeanInCappuccino = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba5-e75b-4ca9-9bab-c01409785a32"),
                Coffee = Cappuccino,
                CoffeeId = Cappuccino.CoffeeId,
                Ingredient = ArabicaBean,
                IngredientId = ArabicaBean.IngredientId,
                Quantity = 1
            };

            var MilkInCappuccino = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba6-e75b-4ca9-9bab-c01409785a32"),
                Coffee = Cappuccino,
                CoffeeId = Cappuccino.CoffeeId,
                Ingredient = Milk,
                IngredientId = Milk.IngredientId,
                Quantity = 3
            };

            var BrownSugarInRandomCoffee1 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba7-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee1,
                CoffeeId = RandomCoffee1.CoffeeId,
                Ingredient = BrownSugar,
                IngredientId = BrownSugar.IngredientId,
                Quantity = 2
            };

            var ChocolateInRandomCoffee1 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba8-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee1,
                CoffeeId = RandomCoffee1.CoffeeId,
                Ingredient = Chocolate,
                IngredientId = Chocolate.IngredientId,
                Quantity = 3
            };

            var ExcelsaBeanInRandomCoffee1 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cba9-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee1,
                CoffeeId = RandomCoffee1.CoffeeId,
                Ingredient = ExcelsaBean,
                IngredientId = ExcelsaBean.IngredientId,
                Quantity = 1
            };

            var SoyMilkInRandomCoffee1 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb0-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee1,
                CoffeeId = RandomCoffee1.CoffeeId,
                Ingredient = SoyMilk,
                IngredientId = SoyMilk.IngredientId,
                Quantity = 2
            };

            var WhiteSugarInRandomCoffee2 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb1-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee2,
                CoffeeId = RandomCoffee2.CoffeeId,
                Ingredient = WhiteSugar,
                IngredientId = WhiteSugar.IngredientId,
                Quantity = 2
            };

            var SteviaInRandomCoffee2 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb2-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee2,
                CoffeeId = RandomCoffee2.CoffeeId,
                Ingredient = Stevia,
                IngredientId = Stevia.IngredientId,
                Quantity = 3
            };

            var LibericaBeanInRandomCoffee2 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb3-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee2,
                CoffeeId = RandomCoffee2.CoffeeId,
                Ingredient = LibericaBean,
                IngredientId = LibericaBean.IngredientId,
                Quantity = 1
            };

            var AlmondMilkInRandomCoffee2 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb4-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee2,
                CoffeeId = RandomCoffee2.CoffeeId,
                Ingredient = AlmondMilk,
                IngredientId = AlmondMilk.IngredientId,
                Quantity = 2
            };

            var CoconutOilInRandomCoffee2 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb5-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee2,
                CoffeeId = RandomCoffee2.CoffeeId,
                Ingredient = CoconutOil,
                IngredientId = CoconutOil.IngredientId,
                Quantity = 4
            };

            var WhiteSugarInRandomCoffee3 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb6-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee3,
                CoffeeId = RandomCoffee3.CoffeeId,
                Ingredient = WhiteSugar,
                IngredientId = WhiteSugar.IngredientId,
                Quantity = 2
            };

            var ChocolateInRandomCoffee3 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb7-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee3,
                CoffeeId = RandomCoffee3.CoffeeId,
                Ingredient = Chocolate,
                IngredientId = Chocolate.IngredientId,
                Quantity = 2
            };

            var RobustaBeanInRandomCoffee3 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb8-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee3,
                CoffeeId = RandomCoffee3.CoffeeId,
                Ingredient = RobustaBean,
                IngredientId = RobustaBean.IngredientId,
                Quantity = 1
            };

            var SkimMilkInRandomCoffee3 = new IngredientInCoffeeModel
            {
                IngredientInCoffeeId = new Guid("75a9cbb9-e75b-4ca9-9bab-c01409785a32"),
                Coffee = RandomCoffee3,
                CoffeeId = RandomCoffee3.CoffeeId,
                Ingredient = SkimMilk,
                IngredientId = SkimMilk.IngredientId,
                Quantity = 2
            };

            var ingredientInModelList = new List<IngredientInCoffeeModel>
            {
                ArabicaBeanInEspresso,
                ArabicaBeanInDoubleEspresso,
                ArabicaBeanInMachiatto,
                ArabicaBeanInCappuccino,
                MilkInCappuccino,
                MilkInMachiatto,
                BrownSugarInRandomCoffee1,
                ChocolateInRandomCoffee1,
                ExcelsaBeanInRandomCoffee1,
                SoyMilkInRandomCoffee1,
                WhiteSugarInRandomCoffee2,
                SteviaInRandomCoffee2,
                LibericaBeanInRandomCoffee2,
                AlmondMilkInRandomCoffee2,
                CoconutOilInRandomCoffee2,
                WhiteSugarInRandomCoffee3,
                ChocolateInRandomCoffee3,
                RobustaBeanInRandomCoffee3,
                SkimMilkInRandomCoffee3
            };
            
            foreach(var cof in coffee)
            {
                cof.ProductionPrice = cof.BasePrice + cof.Ingredients.Sum(ing => ing.Price * 
                                      ingredientInModelList.Where(ingModel=>ingModel.CoffeeId == cof.CoffeeId && ingModel.IngredientId == ing.IngredientId)
                                                           .Select(ingModel=>ingModel.Quantity).First());

                cof.TotalPrice = cof.ProductionPrice;
                cof.TotalPrice *= cof.IncomeCoef;
                int remainder = (int)(Math.Floor(cof.TotalPrice)) % 10;
                if (remainder != 0)
                {
                    int newMultiplier = ((int)cof.TotalPrice / 10) + 1;
                    cof.TotalPrice = newMultiplier * 10;
                }
                else
                {
                    cof.TotalPrice = (int)Math.Floor(cof.TotalPrice);
                }
            }

            context.Coffee.AddOrUpdate(Espresso,
                Machiatto,
                Cappuccino,
                DoubleEspresso,
                RandomCoffee1,
                RandomCoffee2,
                RandomCoffee3);

            context.IngredientInCoffee.AddOrUpdate(ArabicaBeanInEspresso,
            ArabicaBeanInDoubleEspresso,
            ArabicaBeanInMachiatto,
            ArabicaBeanInCappuccino,
            MilkInCappuccino,
            MilkInMachiatto,
            BrownSugarInRandomCoffee1,
            ChocolateInRandomCoffee1,
            ExcelsaBeanInRandomCoffee1,
            SoyMilkInRandomCoffee1,
            WhiteSugarInRandomCoffee2,
            SteviaInRandomCoffee2,
            LibericaBeanInRandomCoffee2,
            AlmondMilkInRandomCoffee2,
            CoconutOilInRandomCoffee2,
            WhiteSugarInRandomCoffee3,
            ChocolateInRandomCoffee3,
            RobustaBeanInRandomCoffee3,
            SkimMilkInRandomCoffee3);
        }
    }
}
