﻿namespace Common
{
    public sealed class SolutionTemplate
    {
        private string Year { get; }
        private string Day { get; }

        public SolutionTemplate(int year, int day)
        {
            Year = year.ToString( ); ;
            Day = day < 10 ? day.ToString( ).PadLeft(2, '0') : day.ToString( );
        }

        public string CreateSolution( ) =>
          $@"using System;
             using System.Collections.Generic;
             using System.Linq;
             using System.Text;
             using System.Threading.Tasks;
             using Common;

             namespace AoC{Year}
             {{
                 public class Day{Day} : Solution
                 {{
                     public Day{Day}(string file) : base(file) {{ }}

                     public override string SolvePart1( ) => null;

                     public override string SolvePart2( ) => null;
                 }}
             }}".Replace("             ", "");

        public string CreateUnitTest(int? part1Expected) =>
            $@"using AoC{Year};
             using NUnit.Framework;
             using System.Collections.Generic;
             using System.Linq;
             
             namespace AoC{Year}Tests
             {{
                 public class Day{Day}Test
                 {{
                     Day{Day} day{Day};
             
                     [SetUp]
                     public void Setup( )
                     {{
                         day{Day} = new Day{Day}(""day{Day}test1"");
                     }}
             
                     [Test]
                     public void Part1( )
                     {{
                         var actual = day{Day}.SolvePart1( );
                         Assert.AreEqual(""{( part1Expected.HasValue ? part1Expected.Value : string.Empty )}"", actual);
                     }}
             
                     [Test]
                     public void Part2( )
                     {{
                         var actual = day{Day}.SolvePart2( );
                         Assert.AreEqual("""", actual);
                     }}
                 }}
             }}".Replace("             ", "");
    }
}
