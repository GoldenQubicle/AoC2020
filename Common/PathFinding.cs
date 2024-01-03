﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;

namespace Common;

public static class PathFinding
{
	// Technically not finding any path but closely related and first implementation for visualizing AoC stuff. 
	public static async Task FloodFill((int x, int y) start, Grid2d grid, Func<Grid2d.Cell, bool> constraint, Func<IEnumerable<Grid2d.Cell>, Task> renderAction = null)
	{
		var queue = new Queue<Grid2d.Cell>( );
		var visited = new HashSet<Grid2d.Cell>( );
		queue.Enqueue(grid[start]);

		while (queue.TryDequeue(out var current))
		{
			visited.Add(current);

			if (renderAction is not null)
				await renderAction(visited);

			queue.EnqueueAll(grid.GetNeighbors(current, n => !queue.Contains(n) && !visited.Contains(n) && constraint(n)));
		}
	}

	//note we do not always know the target position to begin with!
	public static async Task BreadthFirstSearch((int x, int y) start, (int x, int y) target,
		Grid2d grid, Func<Grid2d.Cell, bool> constraint, Func<IEnumerable<Grid2d.Cell>, Task> renderAction = null)
	{
		var queue = new Queue<Grid2d.Cell>( );
		var visited = new Dictionary<Grid2d.Cell, Grid2d.Cell>( );
		var current = grid[start];
		queue.Enqueue(current);
		visited.Add(grid[start], new Grid2d.Cell((0, 0)));

		while (queue.TryDequeue(out var next))
		{
			current = next;

			if (current.Position == target)
				break;

			var neighbors = grid.GetNeighbors(current, n => !queue.Contains(n) && !visited.ContainsKey(n) && constraint(n));
			neighbors.ForEach(n => visited.TryAdd(n, current));
			queue.EnqueueAll(neighbors);
			await renderAction(visited.Keys);
		}

		var path = new List<Grid2d.Cell>( );
		while (current.Position != start)
		{
			path.Add(current);
			current = visited[current];
		}
		path.Add(grid[start]);

		if (renderAction is not null)
		{
			Console.WriteLine($"BFS found path with length: {path.Count} and visited {visited.Count} cells");
			for (var i = 1 ;i <= path.Count ;i++)
				await renderAction(path.TakeLast(i));
		}
	}

	public static async Task UniformCostSearch((int x, int y) start, (int x, int y) target,
		Grid2d grid, Func<Grid2d.Cell, bool> constraint, Func<Grid2d.Cell, Grid2d.Cell, long> heuristic, Func<IEnumerable<Grid2d.Cell>, Task> renderAction = null)
	{
		var queue = new PriorityQueue<Grid2d.Cell, long>( );
		var visited = new Dictionary<Grid2d.Cell, Grid2d.Cell>( );
		var costs = new Dictionary<Grid2d.Cell, long>( );
		var current = grid[start];
		queue.Enqueue(current, 0);
		visited.Add(grid[start], grid[start]);
		costs.Add(grid[start], 0);

		while (queue.TryDequeue(out var next, out var cost))
		{
			current = next;

			if (current.Position == target)
				break;

			foreach (var n in grid.GetNeighbors(current, n => !visited.ContainsKey(n) && constraint(n)))
			{
				if (costs.TryGetValue(n, out var value) && cost + n.Value < value)
					costs[n] = cost + n.Value;
				else
					costs.Add(n, n.Value + cost);

				if (queue.UnorderedItems.Contains((n, costs[n])))
					continue;

				queue.Enqueue(n, costs[n] + heuristic(grid[target], n));
				visited.TryAdd(n, current);
				await renderAction(visited.Keys);
			}
		}
		
		var path = new List<Grid2d.Cell>( );
		while (current.Position != start)
		{
			path.Add(current);
			current = visited[current];
		}
		path.Add(grid[start]);

		if (renderAction is not null)
		{
			Console.WriteLine($"UCS found path with length: {path.Count} and visited {visited.Count} cells");

			for (var i = 1 ;i <= path.Count ;i++)
				await renderAction(path.TakeLast(i));

		}
	}
}
