using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P05Shop.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Barcode", "Description", "Price", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "398563489476", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 878.61m, new DateTime(2020, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Fresh Gloves" },
                    { 2, "516282387459", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 794.15m, new DateTime(2021, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ergonomic Wooden Cheese" },
                    { 3, "851955177378", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 658.49m, new DateTime(2025, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intelligent Plastic Chicken" },
                    { 4, "310517593095", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 714.10m, new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorgeous Soft Pants" },
                    { 5, "466100501806", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 971.42m, new DateTime(2024, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Granite Ball" },
                    { 6, "534335118185", "The Football Is Good For Training And Recreational Purposes", 715.38m, new DateTime(2025, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Steel Car" },
                    { 7, "510990668732", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 923.03m, new DateTime(2022, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Steel Pants" },
                    { 8, "802098232530", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 42.50m, new DateTime(2023, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intelligent Fresh Salad" },
                    { 9, "453376747653", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 523.22m, new DateTime(2023, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Wooden Table" },
                    { 10, "451175802767", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 1272.51m, new DateTime(2022, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ergonomic Metal Bike" },
                    { 11, "542625158827", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 1479.88m, new DateTime(2022, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Small Plastic Shirt" },
                    { 12, "628857397971", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 549.01m, new DateTime(2020, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Steel Chicken" },
                    { 13, "082601798573", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 636.06m, new DateTime(2024, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intelligent Soft Car" },
                    { 14, "478523818642", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 855.41m, new DateTime(2021, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handmade Wooden Tuna" },
                    { 15, "502705881459", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 581.56m, new DateTime(2021, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Granite Chair" },
                    { 16, "946989304695", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 1441.23m, new DateTime(2025, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Steel Pants" },
                    { 17, "976363135822", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 1236.26m, new DateTime(2022, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Fresh Soap" },
                    { 18, "261240029546", "The Football Is Good For Training And Recreational Purposes", 318.78m, new DateTime(2022, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refined Steel Soap" },
                    { 19, "568274357274", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 1123.70m, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Soft Keyboard" },
                    { 20, "462623468857", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 979.63m, new DateTime(2020, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorgeous Concrete Tuna" },
                    { 21, "798789284485", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 631.56m, new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Concrete Shoes" },
                    { 22, "394471559957", "The Football Is Good For Training And Recreational Purposes", 204.80m, new DateTime(2023, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refined Rubber Pants" },
                    { 23, "177343808346", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1436.47m, new DateTime(2020, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Plastic Hat" },
                    { 24, "522613923732", "The Football Is Good For Training And Recreational Purposes", 212.98m, new DateTime(2020, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handmade Soft Pizza" },
                    { 25, "794057321205", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 35.40m, new DateTime(2021, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Wooden Shoes" },
                    { 26, "455432017513", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 286.83m, new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handcrafted Fresh Fish" },
                    { 27, "147438272779", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 842.73m, new DateTime(2021, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Awesome Steel Computer" },
                    { 28, "025204071702", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 4.78m, new DateTime(2025, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Concrete Fish" },
                    { 29, "591726023709", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 1356.66m, new DateTime(2025, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorgeous Soft Cheese" },
                    { 30, "384345746977", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 1120.22m, new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ergonomic Rubber Chair" },
                    { 31, "730827027280", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 222.14m, new DateTime(2022, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sleek Wooden Chicken" },
                    { 32, "714607005692", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 626.43m, new DateTime(2024, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Cotton Hat" },
                    { 33, "746722855583", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1279.07m, new DateTime(2021, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Plastic Bacon" },
                    { 34, "213282142471", "The Nagasaki Lander is the trademarked name of several series of Nagasaki sport bikes, that started with the 1984 ABC800J", 1206.26m, new DateTime(2024, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorgeous Rubber Shirt" },
                    { 35, "209284251276", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 38.87m, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Concrete Chicken" },
                    { 36, "346458937370", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 1087.58m, new DateTime(2023, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Soft Chair" },
                    { 37, "503802096549", "The Apollotech B340 is an affordable wireless mouse with reliable connectivity, 12 months battery life and modern design", 1204.80m, new DateTime(2020, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Wooden Gloves" },
                    { 38, "485025788141", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 93.30m, new DateTime(2025, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Frozen Bacon" },
                    { 39, "354345238757", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 355.44m, new DateTime(2024, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Awesome Steel Bike" },
                    { 40, "858801484555", "The Nagasaki Lander is the trademarked name of several series of Nagasaki sport bikes, that started with the 1984 ABC800J", 635.57m, new DateTime(2024, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Cotton Chicken" },
                    { 41, "714988602197", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 521.72m, new DateTime(2023, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Soft Tuna" },
                    { 42, "155286078237", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1385.53m, new DateTime(2022, 8, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intelligent Steel Mouse" },
                    { 43, "087910838718", "The Football Is Good For Training And Recreational Purposes", 598.37m, new DateTime(2022, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rustic Rubber Soap" },
                    { 44, "652891946415", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 851.28m, new DateTime(2024, 10, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Wooden Soap" },
                    { 45, "923535725089", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 265.89m, new DateTime(2024, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Small Metal Chair" },
                    { 46, "229696341323", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 1216.58m, new DateTime(2024, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Steel Pizza" },
                    { 47, "711706596633", "The Apollotech B340 is an affordable wireless mouse with reliable connectivity, 12 months battery life and modern design", 1043.24m, new DateTime(2023, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Plastic Mouse" },
                    { 48, "332719430121", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 971.15m, new DateTime(2025, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Frozen Shirt" },
                    { 49, "951309061766", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 652.17m, new DateTime(2025, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Cotton Gloves" },
                    { 50, "009127547504", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 1136.26m, new DateTime(2021, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Rubber Sausages" },
                    { 51, "603364866373", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 1182.32m, new DateTime(2020, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intelligent Concrete Chips" },
                    { 52, "772475156694", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 675.92m, new DateTime(2021, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tasty Soft Pants" },
                    { 53, "573436027479", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 705.63m, new DateTime(2025, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Granite Sausages" },
                    { 54, "523710816295", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 651.63m, new DateTime(2021, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Frozen Shoes" },
                    { 55, "824635878720", "Carbonite web goalkeeper gloves are ergonomically designed to give easy fit", 739.51m, new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rustic Steel Ball" },
                    { 56, "576361972194", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1177.23m, new DateTime(2022, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handmade Soft Salad" },
                    { 57, "698783219674", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 1182.88m, new DateTime(2024, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handcrafted Plastic Car" },
                    { 58, "955366277498", "The Football Is Good For Training And Recreational Purposes", 542.36m, new DateTime(2021, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Cotton Shoes" },
                    { 59, "989205139832", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 1497.16m, new DateTime(2022, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Granite Car" },
                    { 60, "633141672126", "The Nagasaki Lander is the trademarked name of several series of Nagasaki sport bikes, that started with the 1984 ABC800J", 1457.98m, new DateTime(2021, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Wooden Gloves" },
                    { 61, "574022643445", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1354.59m, new DateTime(2021, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handcrafted Cotton Hat" },
                    { 62, "673763390380", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 957.71m, new DateTime(2023, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refined Frozen Pizza" },
                    { 63, "950050813620", "The Football Is Good For Training And Recreational Purposes", 1136.46m, new DateTime(2025, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refined Concrete Towels" },
                    { 64, "381172135289", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 1115.41m, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Frozen Bacon" },
                    { 65, "662294067058", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 965.42m, new DateTime(2024, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Plastic Chair" },
                    { 66, "488881563485", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 632.51m, new DateTime(2022, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handmade Steel Salad" },
                    { 67, "509610517651", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 249.56m, new DateTime(2024, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tasty Wooden Ball" },
                    { 68, "488072168161", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 1383.53m, new DateTime(2022, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Fresh Pizza" },
                    { 69, "876726480221", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 1214.23m, new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Rubber Shoes" },
                    { 70, "353655245670", "The Nagasaki Lander is the trademarked name of several series of Nagasaki sport bikes, that started with the 1984 ABC800J", 982.97m, new DateTime(2023, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Soft Fish" },
                    { 71, "565168193620", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 770.79m, new DateTime(2020, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intelligent Frozen Car" },
                    { 72, "422158181956", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 1444.25m, new DateTime(2024, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handmade Steel Mouse" },
                    { 73, "828545161405", "Ergonomic executive chair upholstered in bonded black leather and PVC padded seat and back for all-day comfort and support", 513.22m, new DateTime(2023, 10, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Granite Ball" },
                    { 74, "831797723764", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 952.25m, new DateTime(2020, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Concrete Towels" },
                    { 75, "097609677569", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 373.12m, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Soft Mouse" },
                    { 76, "802182149670", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 1051.51m, new DateTime(2023, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorgeous Rubber Tuna" },
                    { 77, "013212015449", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 367.86m, new DateTime(2020, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Practical Cotton Keyboard" },
                    { 78, "690545999680", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 717.01m, new DateTime(2024, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Frozen Salad" },
                    { 79, "559006679047", "The Nagasaki Lander is the trademarked name of several series of Nagasaki sport bikes, that started with the 1984 ABC800J", 1096.25m, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Awesome Rubber Hat" },
                    { 80, "557731332596", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 569.74m, new DateTime(2024, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handmade Plastic Cheese" },
                    { 81, "168487695191", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 481.40m, new DateTime(2020, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sleek Frozen Table" },
                    { 82, "772777417267", "Ergonomic executive chair upholstered in bonded black leather and PVC padded seat and back for all-day comfort and support", 432.83m, new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Small Cotton Chips" },
                    { 83, "316913614118", "New ABC 13 9370, 13.3, 5th Gen CoreA5-8250U, 8GB RAM, 256GB SSD, power UHD Graphics, OS 10 Home, OS Office A & J 2016", 996.78m, new DateTime(2020, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gorgeous Cotton Mouse" },
                    { 84, "048107933781", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1278.33m, new DateTime(2025, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handcrafted Granite Mouse" },
                    { 85, "476815229856", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 117.41m, new DateTime(2024, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tasty Fresh Chair" },
                    { 86, "311897082322", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 1353.45m, new DateTime(2024, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Rubber Gloves" },
                    { 87, "013287717760", "The Nagasaki Lander is the trademarked name of several series of Nagasaki sport bikes, that started with the 1984 ABC800J", 701.28m, new DateTime(2021, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tasty Fresh Car" },
                    { 88, "657882299697", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 932.48m, new DateTime(2021, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Generic Wooden Hat" },
                    { 89, "403408032280", "The automobile layout consists of a front-engine design, with transaxle-type transmissions mounted at the rear of the engine and four wheel drive", 1007.35m, new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Concrete Towels" },
                    { 90, "623099711904", "The Football Is Good For Training And Recreational Purposes", 399.63m, new DateTime(2021, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Incredible Wooden Mouse" },
                    { 91, "524884900902", "The Football Is Good For Training And Recreational Purposes", 718.87m, new DateTime(2022, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handcrafted Granite Chicken" },
                    { 92, "730369702796", "The Apollotech B340 is an affordable wireless mouse with reliable connectivity, 12 months battery life and modern design", 594.98m, new DateTime(2021, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Small Concrete Hat" },
                    { 93, "083819971856", "Andy shoes are designed to keeping in mind durability as well as trends, the most stylish range of shoes & sandals", 643.00m, new DateTime(2020, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Small Soft Soap" },
                    { 94, "425418220358", "The slim & simple Maple Gaming Keyboard from Dev Byte comes with a sleek body and 7- Color RGB LED Back-lighting for smart functionality", 340.95m, new DateTime(2024, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Licensed Plastic Fish" },
                    { 95, "082039894639", "Boston's most advanced compression wear technology increases muscle oxygenation, stabilizes active muscles", 1266.44m, new DateTime(2020, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refined Steel Keyboard" },
                    { 96, "467568852368", "The Apollotech B340 is an affordable wireless mouse with reliable connectivity, 12 months battery life and modern design", 1157.86m, new DateTime(2022, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sleek Metal Gloves" },
                    { 97, "048661369054", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 482.72m, new DateTime(2023, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unbranded Steel Ball" },
                    { 98, "942417653689", "The Football Is Good For Training And Recreational Purposes", 110.93m, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fantastic Cotton Chicken" },
                    { 99, "579510970701", "The beautiful range of Apple Naturalé that has an exciting mix of natural ingredients. With the Goodness of 100% Natural Ingredients", 641.63m, new DateTime(2021, 11, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Refined Metal Chicken" },
                    { 100, "562191867907", "New range of formal shirts are designed keeping you in mind. With fits and styling that will make you stand apart", 885.55m, new DateTime(2023, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Handcrafted Plastic Car" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100);
        }
    }
}
