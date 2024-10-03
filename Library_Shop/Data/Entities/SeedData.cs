using ClassLibrary_Shop.Models.Book_m;
using Elfie.Serialization;
using Library_Shop.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore; 

namespace Library_Shop.Data.Entities
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            using (var context = new LibraryDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<LibraryDbContext>>()))
            {
                // Перевірка наявності даних у всіх таблицях
                if (await context.Books.AnyAsync() || await context.Authors.AnyAsync() || await context.Publishers.AnyAsync())
                {
                    Console.WriteLine("Database has already been seeded.");
                    return;
                }

                Console.WriteLine("Seeding the database...");

                // Додавання авторів
                var authors = new List<Author>
                {
                new Author { Name = "Author1", Bio = "Bio1", Nationality = "Nationality1" },
                new Author { Name = "Author2", Bio = "Bio2", Nationality = "Nationality2" },
                new Author { Name = "Author3", Bio = "Bio3", Nationality = "Nationality3" },
                new Author { Name = "Author4", Bio = "Bio4", Nationality = "Nationality4" },
                new Author { Name = "Author5", Bio = "Bio4", Nationality = "Nationality5" },
                new Author { Name = "Author6", Bio = "Bio6", Nationality = "Nationality6" },
                new Author { Name = "Author7", Bio = "Bio7", Nationality = "Nationality7" },
                new Author { Name = "Author8", Bio = "Bio8", Nationality = "Nationality8" }
                };
                await context.Authors.AddRangeAsync(authors);
                Console.WriteLine("Added authors.");

                // Додавання видавців
                var publishers = new List<Publisher>
            {
                new Publisher { Name = "Publisher1", Address = "Address1", Website = "Website1" },
                new Publisher { Name = "Publisher2", Address = "Address2", Website = "Website2" },
                new Publisher { Name = "Publisher3", Address = "Address3", Website = "Website3" },
                new Publisher { Name = "Publisher4", Address = "Address4", Website = "Website4" },
                new Publisher { Name = "Publisher5", Address = "Address5", Website = "Website5" },
                new Publisher { Name = "Publisher6", Address = "Address6", Website = "Website6" },
                new Publisher { Name = "Publisher7", Address = "Address7", Website = "Website7" },
                new Publisher { Name = "Publisher8", Address = "Address8", Website = "Website8" }
            };
                await context.Publishers.AddRangeAsync(publishers);
                Console.WriteLine("Added publishers.");

                // Додавання книг (поки що без зображень)
                var books = new List<Book>
            {
                new Book
                {
                    Title = "Книга 1",
                    Author = authors[0],
                    Publisher = publishers[0],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "1234567890"
                },
                new Book
                {
                    Title = "Книга 2",
                    Author = authors[1],
                    Publisher = publishers[1],
                    Genre = "Детектив",
                    Price = 24.99m,
                    StockQuantity = 5,
                    PublishedDate = DateTime.Parse("2022-05-15"),
                    ISBN = "0987654321"
                },
                new Book
                {
                    Title = "Книга 3",
                    Author = authors[2],
                    Publisher = publishers[2],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "122230"
                },
                new Book
                {
                    Title = "Книга 4",
                    Author = authors[3],
                    Publisher = publishers[3],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "1234890"
                },
                new Book
                {
                    Title = "Книга 5",
                    Author = authors[4],
                    Publisher = publishers[4],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "12367890"
                },
                new Book
                {
                    Title = "Книга 6",
                    Author = authors[5],
                    Publisher = publishers[5],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "12312890"
                },
                new Book
                {
                    Title = "Книга 7",
                    Author = authors[6],
                    Publisher = publishers[6],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "1234220"
                },
                new Book
                {
                    Title = "Книга 8",
                    Author = authors[7],
                    Publisher = publishers[7],
                    Genre = "Фантастика",
                    Price = 19.99m,
                    StockQuantity = 10,
                    PublishedDate = DateTime.Parse("2023-01-01"),
                    ISBN = "1234890"
                }
            };
                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();
                Console.WriteLine("Added books.");

                // Використання ImageService для додавання зображень та прив'язки їх до книг
                var imageService = new ImageService(context, environment);

                var relativeImagePaths = new List<string>
            {
                "Images/100factsMusic.webp",
                "Images/1793.jpg",
                "Images/AfterOpenWay.webp",
                "Images/BodyMyHouse.webp",
                "Images/huntingOnadelinu.jpg",
                "Images/Vavilon.jpg",
                "Images/wallInMyHead.webp",
                "Images/winners.jpg"
            };

                for (int i = 0; i < relativeImagePaths.Count; i++)
                {
                    var relativeImagePath = relativeImagePaths[i];
                    var imageData = imageService.LoadImageBytes(relativeImagePath);

                    if (imageData != null && i < books.Count)
                    {
                        var image = new Image
                        {
                            ImageData = imageData,
                            FilePath = relativeImagePath,
                            BookID = books[i].Id // Прив'язуємо зображення до відповідної книги
                        };

                        context.Images.Add(image);
                        books[i].Image = image; // Прив'язуємо зображення до книги
                        await context.SaveChangesAsync(); // Зберігаємо зміни після кожного додавання
                        Console.WriteLine($"Image with path {relativeImagePath} added to book {books[i].Title}.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to load image from path: {relativeImagePath}");
                    }
                }

                Console.WriteLine("Database seeding completed.");
            }
        }
    }



}