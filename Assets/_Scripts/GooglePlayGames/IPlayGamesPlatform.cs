using UnityEngine;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public interface IPlayGamesPlatform : ISocialPlatform
{

	IRealTimeMultiplayerClient RealTime { get; }

	IntPtr GetApiClient ();

	void AddIdMapping (string fromId, string toId);

	void Authenticate (Action<bool> callback);

	void Authenticate (Action<bool> callback, bool silent);

	bool IsAuthenticated ();

	void SignOut ();

	string GetUserId ();

	void GetIdToken (Action<string> idTokenCallback);

	string GetAccessToken ();

	void GetServerAuthCode (Action<CommonStatusCodes, string> callback);

	string GetUserEmail ();

	void GetPlayerStats (Action<CommonStatusCodes, PlayerStats> callback);

	Achievement GetAchievement (string achievementId);

	string GetUserDisplayName ();

	string GetUserImageUrl ();

	void IncrementAchievement (string achievementID, int steps, Action<bool> callback);

	void SetStepsAtLeast (string achievementID, int steps, Action<bool> callback);

	void ReportScore (long score, string board, string metadata, Action<bool> callback);

	void LoadScores (string leaderboardId, LeaderboardStart start,
	                 int rowCount, LeaderboardCollection collection,
	                 LeaderboardTimeSpan timeSpan,
	                 Action<LeaderboardScoreData> callback);

	void LoadMoreScores (ScorePageToken token, int rowCount,
	                     Action<LeaderboardScoreData> callback);

	void ShowAchievementsUI (Action<UIStatus> callback);

	void ShowLeaderboardUI (string lbId);

	void ShowLeaderboardUI (string lbId, Action<UIStatus> callback);

	void ShowLeaderboardUI (string lbId, LeaderboardTimeSpan span,
	                        Action<UIStatus> callback);

	void SetDefaultLeaderboardForUI (string lbid);

	void RegisterInvitationDelegate (InvitationReceivedDelegate deleg);

	IUserProfile[] GetFriends ();

	string GetToken ();

}
