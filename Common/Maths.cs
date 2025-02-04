﻿namespace Common;

public static class Maths // dumb name but prevents namespace conflict with System.Math
{
	/// <summary>
	/// Calculates the area of a polygon using shoelace method. 
	/// </summary>
	/// <remarks>NOTE: this first coordinate needs to added at the end of the list as well! </remarks>
	/// <param name="polygon"></param>
	/// <returns>The area</returns>
	public static T CalculateAreaShoeLace<T>(IEnumerable<(T x, T y)> polygon) where T : INumber<T>
	{
		var poly = polygon.ToList();
		if (poly.First( ) != poly.Last( ))
			throw new ArgumentException("The first and last vertex in the list needs to be the same!");

		var sum1 = poly.Select((c, i) => (c, i)).SkipLast(1).Select(t => t.c.x * poly[t.i + 1].y).Aggregate(T.Zero, (sum, n) => sum + n);
		var sum2 = poly.Select((c, i) => (c, i)).SkipLast(1).Select(t => t.c.y * poly[t.i + 1].x).Aggregate(T.Zero, (sum, n) => sum + n);

		return T.Abs(sum1 - sum2) / T.CreateChecked(2);
	}

	public record PatternDetectionResult<T>(bool IsCycle = false, List<T> Pattern = default, int Idx = -1) where T : INumber<T>;

	/// <summary>
	/// Tries to find a repeating pattern in the given input. 
	/// </summary>
	/// <param name="input"></param>
	/// <returns><see cref="PatternDetectionResult{T}"/></returns>
	public static PatternDetectionResult<T> DetectPattern<T>(IEnumerable<T> input) where T : INumber<T>
	{
		var numbers = input.ToList( );
		for (var i = 0 ;i < numbers.Count ;i++)
		{
			var potentialPattern = numbers.GetRange(i, numbers.Count - i);
			var patternLength = FindPatternLength(potentialPattern);

			if (patternLength > 0)
			{
				return new PatternDetectionResult<T>(true, numbers.GetRange(i, patternLength), i);
			}
		}

		return new PatternDetectionResult<T>( );
	}

	private static int FindPatternLength<T>(IReadOnlyCollection<T> potentialPattern) where T : INumber<T>
	{
		for (var i = 1 ;i <= potentialPattern.Count / 2 ;i++)
		{
			var subPattern = potentialPattern.Take(i).ToList( );
			var repetitions = potentialPattern.Count / i;

			var repeatedPattern = Enumerable.Repeat(subPattern, repetitions)
				.SelectMany(item => item)
				.ToList( );

			if (potentialPattern.SequenceEqual(repeatedPattern.Take(potentialPattern.Count)))
			{
				return i;
			}
		}

		return 0;
	}


	/// <summary>
	/// Calculate the Least Common Multiple for the given list of input values. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="inputs"></param>
	/// <returns></returns>
	public static T LeastCommonMultiple<T>(IEnumerable<T> inputs) where T : INumber<T> =>
		inputs.Aggregate(LeastCommonMultiple);



	/// <summary>
	/// Calculate the Least Common Multiple for the 2 given values. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T> =>
		T.Abs(a * b) / GreatestCommonDivisor(a, b);



	/// <summary>
	/// Calculate the Greatest Common Divisor (aka Greatest Common Factor) for the 2 given values. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static T GreatestCommonDivisor<T>(T a, T b) where T : INumber<T> =>
		b == T.Zero ? a : GreatestCommonDivisor(b, a % b);



	//shamelessly copied from https://stackoverflow.com/a/67403631
	/// <summary>
	/// Simple test to see a point is inside the given polygon. 
	/// </summary>
	/// <remarks>Note the polygon must be ordered correctly, and a new GraphicsPath object is created with every call!</remarks>
	/// <param name="polygon"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static bool IsPointInsidePolygon(IEnumerable<(int x, int y)> polygon, (int x, int y) p)
	{
		var path = new GraphicsPath( );
		path.AddPolygon(polygon.Select(p => new Point(p.x, p.y)).ToArray( ));

		var region = new Region(path);

		return region.IsVisible(p.x, p.y);
	}

	/// <summary>
	/// Calculate the Manhattan Distance between point a and b. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static T GetManhattanDistance<T>((T x, T y) a, (T x, T y) b) where T : INumber<T> =>
		T.Abs(a.x - b.x) + T.Abs(a.y - b.y);

	public static long GetManhattanDistance(Vector3 a, Vector3 b) =>
		(long)(Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z));

	public static int GetManhattanDistance(INode a, INode b) =>
		GetManhattanDistance(a.Position, b.Position);

	/// <summary>
	/// Calculate the Manhattan Distance between point a and b. 
	/// </summary>
	public static int GetManhattanDistance(Vector2 a, Vector2 b) =>
		GetManhattanDistance(a.ToTuple(), b.ToTuple());


	public static string HashToHexadecimal(string input)
	{
		using var md5 = MD5.Create( );
		var bytes = Encoding.UTF8.GetBytes(input);
		var hashBytes = md5.ComputeHash(bytes);
		var sb = new StringBuilder( );
		hashBytes.ForEach(b => sb.Append(b.ToString("X2")));
		return sb.ToString( );
	}

	}