namespace BookShop;

using Data;
using Initializer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BookShop.Models;
using System.Text;
using BookShop.Models.Enums;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using System.Reflection.Metadata.Ecma335;

public class StartUp
{
	public static void Main()
	{
		using var dbContext = new BookShopContext();

		//DbInitializer.ResetDatabase(db);
		//int year = int.Parse(Console.ReadLine()!);
		// (year == null || year == 0)
		//row new Exception("Please write correct year");
		//int lengthCheck = int.Parse(Console.ReadLine()!);
		var result = CountCopiesByAuthor(dbContext);
		Console.WriteLine(result);
		//CountBooks(dbContext);
	}


	// prob 14 Most Recent Books

	public static string GetMostRecentBooks(BookShopContext dbContext)
	{
		var categoriesWithMostRecentBooks = dbContext.Categories
			.OrderBy(c => c.Name)
			.Select(c => new
			{
				CategoryName = c.Name,
				MostRecentBooks = c.CategoryBooks
			.ToArray()
			.OrderByDescending(cb => cb.Book.ReleaseDate)
			.Take(3)
			.Select(cb => new
			{
				BookTitle = cb.Book.Title,
				ReleaseYear = cb.Book.ReleaseDate.Value.Year
			})
			.ToArray()
			})
			.ToArray();

		StringBuilder sb = new StringBuilder();

		foreach (var c in categoriesWithMostRecentBooks)
		{
			sb.AppendLine
				($"--{c.CategoryName}");
			foreach (var b in c.MostRecentBooks)
			{
				sb.AppendLine($"{b.BookTitle} {b.ReleaseYear}");
			}
		}
		return sb.ToString().TrimEnd();
	}

	// prob 15
	public static string IncreasePrices(BookShopContext dbContext)
	{
		StringBuilder sb = new StringBuilder();

		var booksBefor2010 = dbContext.Books
			.Where(b => b.ReleaseDate.HasValue &&
			b.ReleaseDate.Value.Year < 2010);
		foreach (var b in booksBefor2010)
		{
			var oldPrice = b.Price;
			var newPrice = b.Price += 5;
			sb.AppendLine($"{oldPrice} - {newPrice}");
		}
		return sb.ToString().TrimEnd();

	}
	// prob 3
	public static string GetGoldenBooks(BookShopContext dbContext)
	{
		var goldenBooks = dbContext.Books
			.Where(b => b.EditionType == EditionType.Gold &&
			b.Copies < 5000)
			.OrderBy(b => b.BookId)
			.Select(b => b.Title)
			.ToArray();

		return string.Join(Environment.NewLine, goldenBooks);
	}

	// prob 4 

	public static string GetBooksByPrice(BookShopContext dbContext)
	{
		var booksWithHigherPrice = dbContext.Books
			.Where(b => b.Price < 40)
			.OrderByDescending(b => b.Price)
			.Select(b => new
			{
				b.Title,
				b.Price,
			})
			.ToArray();

		StringBuilder sb = new StringBuilder();

		foreach (var book in booksWithHigherPrice)
		{
			sb.AppendLine($"{book.Title} - {book.Price}");
		}

		return sb.ToString().TrimEnd();
	}

	//prob 5

	public static string GetBooksNotReleasedIn(BookShopContext dbContext, int year)
	{


		var bookNotReleasedIn = dbContext.Books
			.Where(b => b.ReleaseDate.Value.Year != year)
			.OrderBy(b => b.BookId)
			.Select(b => b.Title);


		return string.Join(Environment.NewLine, bookNotReleasedIn);
	}

	// prob 7 

	public static string GetBooksReleasedBefore(BookShopContext dbContext, string date)
	{
		var parsedDate = DateTime.Parse(date);
		var bookReleasedBefore = dbContext.Books
			.OrderByDescending(a => a.ReleaseDate)
			.Where(b => b.ReleaseDate!.Value < parsedDate)
			.Select(b => new
			{
				b.Title,
				b.EditionType,
				b.Price
			})
			.ToArray();

		StringBuilder sb = new StringBuilder();

		foreach (var book in bookReleasedBefore)
		{
			sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price}");
		}

		return sb.ToString().TrimEnd();
	}
	//prob 8
	public static string GetAuthorNamesEndingIn(BookShopContext dbContext, string input)
	{
		var authorNamesEndingIn = dbContext.Authors
			.Where(a => a.FirstName.EndsWith(input))
			.OrderBy(a => a.FirstName)
			.ThenBy(a => a.LastName)
			.Select(a => new
			{
				a.FirstName,
				a.LastName
			})
			.ToArray();

		StringBuilder sb = new StringBuilder();

		foreach (var a in authorNamesEndingIn)
		{
			sb.AppendLine(a.FirstName + " " + a.LastName);
		}

		return sb.ToString().TrimEnd();


	}

	// prob 10

	public static string GetBooksByAuthor(BookShopContext dbContext, string input)
	{
		var booksByAuthor = dbContext.Books
			.Where(b => b.Author.LastName.StartsWith(input))
			.OrderBy(b => b.BookId)
			.Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
			.ToArray();

		return string.Join(Environment.NewLine, booksByAuthor);


	}

	// 11 
	public static  void  CountBooks(BookShopContext dbContext)
	{
        Console.WriteLine("Enter number of symbols:");

        int lengthCheck = int.Parse(Console.ReadLine()!);

	
		if (lengthCheck == null || lengthCheck == 0)
		{
            Console.WriteLine("Enter corect data");
			return;
		}
		
			Console.WriteLine("Enter number of symbols:");
			var countBooks = dbContext.Books
			.Where(b => b.Title.Length > lengthCheck)
			.Count();

          Console.WriteLine($"There are {countBooks} books with longer title than {lengthCheck} symbols");
		

    }

	//12
	public static string CountCopiesByAuthor(BookShopContext dbContext)
	{
		var totalNumberOfBooksByAuthor = dbContext.Authors
			.Select(a => new
			{
				Name = a.FirstName + " " + a.LastName,
				Copies = a.Books
				.Select(b => b.Copies)
				.Sum()
			})
			.OrderByDescending(a => a.Copies)
			.Select(a => $"{a.Name} - {a.Copies}");

		return string.Join(Environment.NewLine, totalNumberOfBooksByAuthor);
		
			
	}


}


