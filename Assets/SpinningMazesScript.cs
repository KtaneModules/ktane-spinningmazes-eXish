using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeepCoding;
using UnityEngine;

public class SpinningMazesScript : ModuleScript
{
	private void Start()
	{
		Buttons.Assign(null, null, null, null, null, null, null, new Action<int>(ButtonPress), null, null, null, null, null, null);
		ColoredButton = UnityEngine.Random.Range(0, 4);
		ColoredButtonColor = (Color)UnityEngine.Random.Range(0, 4);
		Color32 color = default(Color32);
		Direction direction = Direction.None;
		switch (ColoredButtonColor)
		{
		case Color.Red:
			color = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
			direction = Direction.Up;
			break;
		case Color.Blue:
			color = new Color32(0, 0, byte.MaxValue, byte.MaxValue);
			direction = Direction.Down;
			break;
		case Color.Yellow:
			color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
			direction = Direction.Left;
			break;
		case Color.Green:
			color = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
			direction = Direction.Right;
			break;
		}
		Buttons[ColoredButton].GetComponent<MeshRenderer>().material.color = color;
		do
		{
			InitialButtonDirections = InitialButtonDirections.Shuffle<Direction[]>();
		}
		while (InitialButtonDirections[ColoredButton] == direction);
		ButtonDirections = InitialButtonDirections;
		string text = string.Empty;
		switch (ColoredButton)
		{
		case 0:
			text = "top-left";
			break;
		case 1:
			text = "top-right";
			break;
		case 2:
			text = "bottom-left";
			break;
		case 3:
			text = "bottom-right";
			break;
		}
		Log("The {0} button is colored {1}.", new object[]
		{
			text,
			ColoredButtonColor
		});
		Log("The buttons (in reading order) move you in the following directions in the maze: {0}, {1}, {2}, {3}", new object[]
		{
			ButtonDirections[0],
			ButtonDirections[1],
			ButtonDirections[2],
			ButtonDirections[3]
		});
	}

	private void ButtonPress(int ix)
	{
		ButtonEffect(Buttons[ix], 0.05f, new Sound[]
		{
			Sound.ButtonPress
		});
		StartCoroutine(ButtonMovement(Buttons[ix]));
		if (IsSolved)
		{
			return;
		}
		if ((ButtonDirections[ix] & Maze[Y][X]) == Direction.None)
		{
			Strike(new string[]
			{
				"You ran into a wall by trying to go {0} from ({1},{2})! Resetting to (5,5).".Form(new object[]
				{
					ButtonDirections[ix],
					X + 1,
					Y + 1
				})
			});
			X = 4;
			Y = 4;
			LEDs.ForEach(delegate(MeshRenderer r)
			{
				r.material.color = new Color32(0, 0, 0, byte.MaxValue);
			});
			ButtonDirections = InitialButtonDirections;
			return;
		}
		switch (ButtonDirections[ix])
		{
		case Direction.Up:
			Y--;
			break;
		case Direction.Down:
			Y++;
			break;
		case Direction.Left:
			X--;
			break;
		case Direction.Right:
			X++;
			break;
		}
		if (X == 0 && Y == 0)
		{
			if (ButtonDirections[ix] == Direction.Up)
			{
				X = 2;
				RotateClockwise();
			}
			else
			{
				Y = 2;
				RotateCounterClockwise();
			}
		}
		if (X == 0 && Y == 8)
		{
			if (ButtonDirections[ix] == Direction.Left)
			{
				Y = 6;
				RotateClockwise();
			}
			else
			{
				X = 2;
				RotateCounterClockwise();
			}
		}
		if (X == 8 && Y == 8)
		{
			if (ButtonDirections[ix] == Direction.Down)
			{
				X = 6;
				RotateClockwise();
			}
			else
			{
				Y = 6;
				RotateCounterClockwise();
			}
		}
		if (X == 8 && Y == 0)
		{
			if (ButtonDirections[ix] == Direction.Right)
			{
				Y = 2;
				RotateClockwise();
			}
			else
			{
				X = 6;
				RotateCounterClockwise();
			}
		}
		Log("Moved to ({0},{1}).", new object[]
		{
			X + 1,
			Y + 1
		});
		if (X < 0)
		{
			if (ColoredButtonColor == Color.Yellow && ix == ColoredButton)
			{
				Solve(new string[]
				{
					"Congratulations! You got it correct."
				});
				PlaySound(new Sound[]
				{
					Sound.CorrectChime
				});
			}
			else
			{
				Strike(new string[]
				{
					"You tried to exit the maze incorrectly! Resetting to (5,5)."
				});
				X = 4;
				Y = 4;
				ButtonDirections = InitialButtonDirections;
			}
		}
		if (Y < 0)
		{
			if (ColoredButtonColor == Color.Red && ix == ColoredButton)
			{
				Solve(new string[]
				{
					"Congratulations! You got it correct."
				});
				PlaySound(new Sound[]
				{
					Sound.CorrectChime
				});
			}
			else
			{
				Strike(new string[]
				{
					"You tried to exit the maze incorrectly! Resetting to (5,5)."
				});
				X = 4;
				Y = 4;
				ButtonDirections = InitialButtonDirections;
			}
		}
		if (X > 8)
		{
			if (ColoredButtonColor == Color.Green && ix == ColoredButton)
			{
				Solve(new string[]
				{
					"Congratulations! You got it correct."
				});
				PlaySound(new Sound[]
				{
					Sound.CorrectChime
				});
			}
			else
			{
				Strike(new string[]
				{
					"You tried to exit the maze incorrectly! Resetting to (5,5)."
				});
				X = 4;
				Y = 4;
				ButtonDirections = InitialButtonDirections;
			}
		}
		if (Y > 8)
		{
			if (ColoredButtonColor == Color.Blue && ix == ColoredButton)
			{
				Solve(new string[]
				{
					"Congratulations! You got it correct."
				});
				PlaySound(new Sound[]
				{
					Sound.CorrectChime
				});
			}
			else
			{
				Strike(new string[]
				{
					"You tried to exit the maze incorrectly! Resetting to (5,5)."
				});
				X = 4;
				Y = 4;
				ButtonDirections = InitialButtonDirections;
			}
		}
		LEDs.ForEach(delegate(MeshRenderer r)
		{
			r.material.color = new Color32(0, 0, 0, byte.MaxValue);
		});
		if (X == 3)
		{
			LEDs[1].material.color = new Color32(0, 0, byte.MaxValue, byte.MaxValue);
		}
		if (X == 5)
		{
			LEDs[0].material.color = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
		}
		if (Y == 3)
		{
			LEDs[2].material.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
		}
		if (Y == 5)
		{
			LEDs[3].material.color = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
		}
	}

	private void RotateCounterClockwise()
	{
		Log("You went across a curved space, so your inputs are now rotated counterclockwise.", 3);
		ButtonDirections = ButtonDirections.Select(delegate(Direction d)
		{
			switch (d)
			{
			case Direction.Up:
				return Direction.Left;
			case Direction.Down:
				return Direction.Right;
			case Direction.Left:
				return Direction.Down;
			case Direction.Right:
				return Direction.Up;
			}
			return Direction.None;
		}).ToArray();
	}

	private void RotateClockwise()
	{
		Log("You went across a curved space, so your inputs are now rotated clockwise.", 3);
		ButtonDirections = ButtonDirections.Select(delegate(Direction d)
		{
			switch (d)
			{
			case Direction.Up:
				return Direction.Right;
			case Direction.Down:
				return Direction.Left;
			case Direction.Left:
				return Direction.Up;
			case Direction.Right:
				return Direction.Down;
			}
			return Direction.None;
		}).ToArray();
	}

	private IEnumerator ButtonMovement(KMSelectable button)
	{
		float time = 0f;
		while (time < 0.1f)
		{
			time += Time.deltaTime;
			button.transform.parent.localPosition = Vector3.Lerp(Vector3.zero, Vector3.down * 0.03f, time);
			yield return null;
		}
		while (time < 0.2f)
		{
			time += Time.deltaTime;
			button.transform.parent.localPosition = Vector3.Lerp(Vector3.down * 0.03f, Vector3.zero, time + 0.8f);
			yield return null;
		}
		yield break;
	}

	public IEnumerator ProcessTwitchCommand(string command)
	{
		Regex r = new Regex("^(?:press\\s+)?((?:[1-4]\\s*)+)$");
		Match i = r.Match(command.Trim().ToLowerInvariant());
		if (!i.Success)
		{
			yield break;
		}
		yield return null;
		Regex cr = new Regex("[1-4]");
		IEnumerable<KMSelectable> bs = from c in i.Groups[1].Value
		where cr.IsMatch(c.ToString())
		select Buttons[int.Parse(c.ToString()) - 1];
		foreach (KMSelectable b in bs)
		{
			b.OnInteract.Invoke();
			yield return new WaitForSeconds(0.2f);
		}
	}

	public IEnumerator TwitchHandleForcedSolve()
	{
		IEnumerator enumerator = SpinToDirection().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object e = enumerator.Current;
				yield return e;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		Vector2Int exitp;
		switch (ColoredButtonColor)
		{
		case Color.Red:
			exitp = new Vector2Int(5, 0);
			break;
		case Color.Blue:
			exitp = new Vector2Int(3, 8);
			break;
		case Color.Yellow:
			exitp = new Vector2Int(0, 3);
			break;
		case Color.Green:
			exitp = new Vector2Int(8, 5);
			break;
		default:
			throw new Exception("The colored button was not RGBY. This shouldn't have happened, so please contact eXish.");
		}
		IEnumerator enumerator2 = SearchAndMoveTo(new Vector2Int[]
		{
			exitp
		}).GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object e2 = enumerator2.Current;
				yield return e2;
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = (enumerator2 as IDisposable)) != null)
			{
				disposable2.Dispose();
			}
		}
		if (X == 0)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Left)].OnInteract.Invoke();
		}
		if (X == 8)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Right)].OnInteract.Invoke();
		}
		if (Y == 0)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Up)].OnInteract.Invoke();
		}
		if (Y == 8)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Down)].OnInteract.Invoke();
		}
		yield return null;
	}

	private IEnumerable SpinToDirection()
	{
		Direction exitdir;
		switch (ColoredButtonColor)
		{
		case Color.Red:
			exitdir = Direction.Up;
			break;
		case Color.Blue:
			exitdir = Direction.Down;
			break;
		case Color.Yellow:
			exitdir = Direction.Left;
			break;
		case Color.Green:
			exitdir = Direction.Right;
			break;
		default:
			throw new Exception("The colored button was not RGBY. This shouldn't have happened, so please contact eXish.");
		}
		Direction cdir = ButtonDirections[ColoredButton];
		string spin = GetSpin(cdir, exitdir);
		if (spin == "0")
		{
			yield break;
		}
		List<Vector2Int> locs = new List<Vector2Int>();
		if (spin == "90" || spin == "180")
		{
			locs.Add(new Vector2Int(0, 1));
			locs.Add(new Vector2Int(7, 0));
			locs.Add(new Vector2Int(8, 7));
			locs.Add(new Vector2Int(1, 8));
		}
		if (spin == "270" || spin == "180")
		{
			locs.Add(new Vector2Int(1, 0));
			locs.Add(new Vector2Int(8, 1));
			locs.Add(new Vector2Int(7, 8));
			locs.Add(new Vector2Int(0, 7));
		}
		IEnumerator enumerator = SearchAndMoveTo(locs).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object e = enumerator.Current;
				yield return e;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		if (X == 1)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Left)].OnInteract.Invoke();
		}
		if (X == 7)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Right)].OnInteract.Invoke();
		}
		if (Y == 1)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Up)].OnInteract.Invoke();
		}
		if (Y == 7)
		{
			Buttons[ButtonDirections.IndexOf(Direction.Down)].OnInteract.Invoke();
		}
		yield return new WaitForSeconds(0.1f);
		if (spin == "180")
		{
			IEnumerator enumerator2 = SpinToDirection().GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object e2 = enumerator2.Current;
					yield return e2;
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}
		yield break;
	}

	private IEnumerable SearchAndMoveTo(IEnumerable<Vector2Int> locs)
	{
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();
		Queue<Vector2Int> q = new Queue<Vector2Int>();
		Vector2Int startLoc = new Vector2Int(X, Y);
		q.Enqueue(startLoc);
		while (q.Count > 0)
		{
			Vector2Int vector2Int = q.Dequeue();
			if (visited.Add(vector2Int))
			{
				if (locs.Contains(vector2Int))
				{
					Vector2Int exit = vector2Int;
					List<Vector2Int> path = new List<Vector2Int>();
					Vector2Int p = exit;
					while (p != startLoc)
					{
						path.Add(p);
						p = parents[p];
					}
					for (int i = path.Count - 1; i >= 0; i--)
					{
						Vector2Int dif = path[i] - new Vector2Int(X, Y);
						if (dif.x == 1)
						{
							Buttons[ButtonDirections.IndexOf(Direction.Right)].OnInteract.Invoke();
						}
						if (dif.x == -1)
						{
							Buttons[ButtonDirections.IndexOf(Direction.Left)].OnInteract.Invoke();
						}
						if (dif.y == 1)
						{
							Buttons[ButtonDirections.IndexOf(Direction.Down)].OnInteract.Invoke();
						}
						if (dif.y == -1)
						{
							Buttons[ButtonDirections.IndexOf(Direction.Up)].OnInteract.Invoke();
						}
						yield return new WaitForSeconds(0.1f);
					}
					yield break;
				}
				List<Vector2Int> list = new List<Vector2Int>();
				if ((Maze[vector2Int.y][vector2Int.x] & Direction.Left) == Direction.Left)
				{
					list.Add(new Vector2Int(vector2Int.x - 1, vector2Int.y));
				}
				if ((Maze[vector2Int.y][vector2Int.x] & Direction.Right) == Direction.Right)
				{
					list.Add(new Vector2Int(vector2Int.x + 1, vector2Int.y));
				}
				if ((Maze[vector2Int.y][vector2Int.x] & Direction.Down) == Direction.Down)
				{
					list.Add(new Vector2Int(vector2Int.x, vector2Int.y + 1));
				}
				if ((Maze[vector2Int.y][vector2Int.x] & Direction.Up) == Direction.Up)
				{
					list.Add(new Vector2Int(vector2Int.x, vector2Int.y - 1));
				}
				list.RemoveAll((Vector2Int v) => v.x < 0 || v.x > 8 || v.y < 0 || v.y > 8);
				foreach (Vector2Int vector2Int2 in list)
				{
					if (!visited.Contains(vector2Int2))
					{
						q.Enqueue(vector2Int2);
						parents[vector2Int2] = vector2Int;
					}
				}
			}
		}
		throw new Exception("There was no valid turn found. This shouldn't have happened, so please contact eXish.");
	}

	private string GetSpin(Direction cdir, Direction exitdir)
	{
		IEnumerable<int> sequence = from d in new Direction[]
		{
			cdir,
			exitdir
		}
		select (int)d;
		if (sequence.SequenceEqual(new int[]
		{
			1,
			2
		}) || sequence.SequenceEqual(new int[]
		{
			2,
			1
		}) || sequence.SequenceEqual(new int[]
		{
			4,
			8
		}) || sequence.SequenceEqual(new int[]
		{
			8,
			4
		}))
		{
			return "180";
		}
		if (sequence.SequenceEqual(new int[]
		{
			1,
			8
		}) || sequence.SequenceEqual(new int[]
		{
			8,
			2
		}) || sequence.SequenceEqual(new int[]
		{
			2,
			4
		}) || sequence.SequenceEqual(new int[]
		{
			4,
			1
		}))
		{
			return "90";
		}
		if (sequence.SequenceEqual(new int[]
		{
			1,
			4
		}) || sequence.SequenceEqual(new int[]
		{
			4,
			2
		}) || sequence.SequenceEqual(new int[]
		{
			2,
			8
		}) || sequence.SequenceEqual(new int[]
		{
			8,
			1
		}))
		{
			return "270";
		}
		return "0";
	}

	[SerializeField]
	private KMSelectable[] Buttons;

	[SerializeField]
	private MeshRenderer[] LEDs;

	private int ColoredButton;

	private Color ColoredButtonColor;

	private Direction[] ButtonDirections;

	private Direction[] InitialButtonDirections = new Direction[]
	{
		Direction.Up,
		Direction.Down,
		Direction.Left,
		Direction.Right
	};

	private int X = 4;

	private int Y = 4;

	private readonly Direction[][] Maze = new Direction[][]
	{
		new int[]
		{
			0,
			12,
			14,
			12,
			12,
			5,
			10,
			12,
			0
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			3,
			0,
			9,
			14,
			12,
			14,
			5,
			0,
			3
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			9,
			12,
			14,
			15,
			14,
			15,
			12,
			6,
			3
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			12,
			6,
			6,
			3,
			11,
			5,
			8,
			15,
			7
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			10,
			5,
			3,
			11,
			15,
			12,
			14,
			7,
			3
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			9,
			14,
			5,
			3,
			11,
			14,
			13,
			7,
			9
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			10,
			13,
			14,
			15,
			13,
			7,
			10,
			13,
			6
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			3,
			0,
			3,
			3,
			10,
			13,
			5,
			0,
			3
		}.Cast<Direction>().ToArray(),
		new int[]
		{
			0,
			12,
			5,
			3,
			9,
			12,
			12,
			12,
			0
		}.Cast<Direction>().ToArray()
	};

	private readonly string TwitchHelpMessage = "Use '!{0} 12 34' to press the top-left, top-right, bottom-left, and bottom-right buttons in that order.";

	private enum Color
	{
		Red,
		Blue,
		Yellow,
		Green
	}

	[Flags]
	private enum Direction
	{
		None = 0,
		Up = 1,
		Down = 2,
		Left = 4,
		Right = 8
	}
}