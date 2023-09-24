using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LeaderboardManager : Node
{
	[Export]
	public LeaderboardData LeaderboardData;

	[Signal]
	public delegate void AuthenticationSuccess();

	[Signal]
	public delegate void LeaderboardChanged();

	public string PlayerName { get; private set; }
	public int CurrentScore { get; set; }
	public List<LeaderboardItem> LeaderboardItems { get; set; }
	
	public bool IsAuthenticated { get => sessionToken != null; }
	private string sessionToken = null;

	private class LeaderboardResponse {
		public Error Error { get; set; }
		public string ErrorString { get; set; }
		public string[] Headers { get; set; }
		public Dictionary Body { get; set; }
	}

	public struct LeaderboardItem {
		public string MemberId { get; set; }
		public int Rank { get; set; }
		public int Score { get; set; }
		public int PlayerId { get; set; }
		public string PlayerName { get; set; }
	}

	public LeaderboardManager() {
		LeaderboardData = ResourceLoader.Load<LeaderboardData>("res://leaderboard/LeaderboardData.tres");
	}

	public override void _Ready()
	{
		Task.Run(() => Authenticate());
	}

	private async Task<LeaderboardResponse> MakeRequest(
		string url,
		HTTPClient.Method method,
		Dictionary body = null,
		string[] headers = null
	) {
		HTTPRequest req = new HTTPRequest();
		AddChild(req);
		req.Request(
			url,
			headers ?? new[] { "Content-Type: application/json" },
			true,
			method,
			body == null ? "" : JSON.Print(body));

		var completed = await ToSignal(req, "request_completed");

		int responseResult = (int) completed[0];
		int responseCode = (int) completed[1];
		string[] responseHeaders = (string[]) completed[2];
		byte[] responseBody = (byte[]) completed[3];

		req.QueueFree();

		if (responseResult != (int)HTTPRequest.Result.Success) {
			return new LeaderboardResponse() {
				Error = Error.CantConnect,
				ErrorString = "Couldn't connect to server",
				Headers = responseHeaders,
				Body = default
			};
		}

		var decodedBody = JSON.Parse(responseBody.GetStringFromUTF8());
		Dictionary objectBody;
		if (decodedBody.Error == Error.Ok) {
			objectBody = decodedBody.Result as Dictionary;
		} else {
			return new LeaderboardResponse() {
				Error = decodedBody.Error,
				ErrorString = decodedBody.ErrorString,
				Headers = responseHeaders,
				Body = null,
			};
		}

		if (responseCode != 200) {
			return new LeaderboardResponse() {
				Error = Error.CantConnect,
				ErrorString = (string) objectBody["message"],
				Headers = responseHeaders,
				Body = objectBody
			};
		}
		
		return new LeaderboardResponse () {
			Error = Error.Ok,
			ErrorString = "OK",
			Headers = responseHeaders,
			Body = objectBody,
		};
	}

	public async Task<(Error error, string message)> Authenticate()
	{
		//GD.Print("Authenticating...");
		var dataFile = new File();
		dataFile.Open("user://LootLocker.data", File.ModeFlags.Read);
		string playerIdentifier = dataFile.GetAsText();
		dataFile.Close();

		var data = new Dictionary {
			{"game_key", LeaderboardData.APIKey},
			{"game_version", "0.0.0.1"},
			{"development_mode", LeaderboardData.DevelopmentMode }
		};

		if(playerIdentifier.Length > 1) {
			data.Add("player_identifier", playerIdentifier);
		}

		var res = await MakeRequest(
			"https://api.lootlocker.io/game/v2/session/guest",
			HTTPClient.Method.Post,
			data
		);

		if (res.Error != Error.Ok)
		{	
			return (res.Error, res.ErrorString);
		}

		if (playerIdentifier.Length <= 1) {
			var newDataFile = new File();
			newDataFile.Open("user://LootLocker.data", File.ModeFlags.Write);
			newDataFile.StoreString((string) res.Body["player_identifier"]);
			newDataFile.Close();
		}

		sessionToken = (string) res.Body["session_token"];
		EmitSignal(nameof(AuthenticationSuccess));
		//GD.Print("Authentication success!");
		Task.Run(() => GetPlayerName());
		Task.Run(() => GetLeaderboard());
		return (Error.Ok, "");
	}

	public async Task<(Error error, List<LeaderboardItem> items, string errorMessage)> GetLeaderboard()
	{
		var res = await MakeRequest(
			$"https://api.lootlocker.io/game/leaderboards/{LeaderboardData.LeaderboardKey}/list?count=10",
			HTTPClient.Method.Get,
			null,
			new[] { "Content-Type: application/json", "x-session-token: "+sessionToken }
		);

		if (res.Error != Error.Ok) {
			return (res.Error, null, res.ErrorString);
		}

		LeaderboardItems = new List<LeaderboardItem>();
		foreach (Dictionary item in res.Body["items"] as Array) {
			LeaderboardItems.Add(new LeaderboardItem() {
				MemberId = (string) item["member_id"],
				Rank = (int) (float) item["rank"],
				Score = (int) (float) item["score"],
				PlayerId = (int) (float) (item["player"] as Dictionary)["id"],
				PlayerName = (string) (item["player"] as Dictionary)["name"],
			});
		}

		EmitSignal(nameof(LeaderboardChanged));
		return (Error.Ok, LeaderboardItems, "");
	}

	public async Task<(Error error, string playerName, string errorMessage)> GetPlayerName()
	{
		var res = await MakeRequest(
			$"https://api.lootlocker.io/game/player/name",
			HTTPClient.Method.Get,
			headers: new[] { "Content-Type: application/json", "x-session-token: "+sessionToken }
		);

		if (res.Error != Error.Ok) {
			return (res.Error, null, res.ErrorString);
		}

		PlayerName = (string) res.Body["name"];
		return (Error.Ok, PlayerName, "");
	}

	public async Task<(Error error, string errorMessage)> SetPlayerName(string newName)
	{
		var res = await MakeRequest(
			$"https://api.lootlocker.io/game/player/name",
			HTTPClient.Method.Patch,
			new Dictionary {
				{ "name", newName }
			},
			new[] { "Content-Type: application/json", "x-session-token: "+sessionToken }
		);

		if (res.Error != Error.Ok) {
			return (res.Error, res.ErrorString);
		}

		PlayerName = newName;
		return (Error.Ok, "");
	}

	public async Task<(Error error, string errorMessage)> RecordScore() {
		//GD.Print("Recording score...");
		var res = await MakeRequest(
			$"https://api.lootlocker.io/game/leaderboards/{LeaderboardData.LeaderboardKey}/submit",
			HTTPClient.Method.Post,
			new Dictionary {
				{ "score", CurrentScore }
			},
			new[] { "Content-Type: application/json", "x-session-token: "+sessionToken }
		);

		if (res.Error != Error.Ok) {
			return (res.Error, res.ErrorString);
		}

		//GD.Print("Recorded score!");
		Task.Run(() => GetLeaderboard());
		return (Error.Ok, "");
	}
}
