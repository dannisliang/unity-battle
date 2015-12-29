using UnityEngine;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.Quests;

public class DummyPlayGamesPlatform : IPlayGamesPlatform,ISocialPlatform
{

	IPlayGamesClient mClient;
	PlayGamesLocalUser mLocalUser = null;

	// achievement/leaderboard ID mapping table
	Dictionary<string, string> mIdMap = new Dictionary<string, string> ();

	public DummyPlayGamesPlatform ()
	{
		this.mLocalUser = new PlayGamesLocalUser (this);
	}


	#region ISocialPlatform implementation

	public void LoadUsers (string[] userIDs, System.Action<IUserProfile[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void ReportProgress (string achievementID, double progress, System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadAchievementDescriptions (System.Action<IAchievementDescription[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadAchievements (System.Action<IAchievement[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public IAchievement CreateAchievement ()
	{
		throw new System.NotImplementedException ();
	}

	public void ReportScore (long score, string board, System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadScores (string leaderboardID, System.Action<IScore[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public ILeaderboard CreateLeaderboard ()
	{
		throw new System.NotImplementedException ();
	}

	public void ShowAchievementsUI ()
	{
		throw new System.NotImplementedException ();
	}

	public void ShowLeaderboardUI ()
	{
		throw new System.NotImplementedException ();
	}

	public void Authenticate (ILocalUser user, System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadFriends (ILocalUser user, System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadScores (ILeaderboard board, System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public bool GetLoading (ILeaderboard board)
	{
		throw new System.NotImplementedException ();
	}

	public ILocalUser localUser {
		get {
			return mLocalUser;
		}
	}

	#endregion




	#region IPlayGamesPlatform implementation

	public IRealTimeMultiplayerClient RealTime {
		get {
			return mClient.GetRtmpClient ();
		}
	}

	public ITurnBasedMultiplayerClient TurnBased {
		get {
			return mClient.GetTbmpClient ();
		}
	}

	public ISavedGameClient SavedGame {
		get {
			return mClient.GetSavedGameClient ();
		}
	}

	public IEventsClient Events {
		get {
			return mClient.GetEventsClient ();
		}
	}

	public IQuestsClient Quests {
		get {
			return mClient.GetQuestsClient ();
		}
	}

	public IntPtr GetApiClient ()
	{
		return mClient.GetApiClient ();
	}

	public void AddIdMapping (string fromId, string toId)
	{
		mIdMap [fromId] = toId;
	}

	public void Authenticate (Action<bool> callback)
	{
		Authenticate (callback, false);
	}

	public void Authenticate (Action<bool> callback, bool silent)
	{
		// make a platform-specific Play Games client
		if (mClient == null) {
			Debug.Log ("Creating platform-specific Play Games client.");
			mClient = new DummyPlayGamesClient ();
		}

		// authenticate!
		mClient.Authenticate (callback, silent);
	}

	public bool IsAuthenticated ()
	{
		return mClient != null && mClient.IsAuthenticated ();
	}

	public void SignOut ()
	{
		if (mClient != null) {
			mClient.SignOut ();
		}
		mLocalUser = new PlayGamesLocalUser (this);
	}

	public string GetUserId ()
	{
		if (!IsAuthenticated ()) {
			Debug.LogError ("GetUserId() can only be called after authentication.");
			return "0";
		}

		return mClient.GetUserId ();
	}

	public string GetIdToken ()
	{
		if (mClient != null) {
			return mClient.GetIdToken ();
		}

		return null;
	}

	public string GetAccessToken ()
	{
		if (mClient != null) {
			return mClient.GetAccessToken ();
		}

		return null;
	}

	public string GetUserEmail ()
	{
		if (mClient != null) {
			return mClient.GetUserEmail ();
		}

		return null;
	}

	public void GetPlayerStats (Action<CommonStatusCodes, PlayGamesLocalUser.PlayerStats> callback)
	{
		if (mClient != null && mClient.IsAuthenticated ()) {
			mClient.GetPlayerStats (callback);
		} else {
			Debug.LogError ("GetPlayerStats can only be called after authentication.");

			callback (CommonStatusCodes.SignInRequired, null);
		}
	}

	public Achievement GetAchievement (string achievementId)
	{
		if (!IsAuthenticated ()) {
			Debug.LogError ("GetAchievement can only be called after authentication.");
			return null;
		}

		return mClient.GetAchievement (achievementId);
	}

	public string GetUserDisplayName ()
	{
		if (!IsAuthenticated ()) {
			Debug.LogError ("GetUserDisplayName can only be called after authentication.");
			return string.Empty;
		}

		return mClient.GetUserDisplayName ();
	}

	public string GetUserImageUrl ()
	{
		if (!IsAuthenticated ()) {
			Debug.LogError ("GetUserImageUrl can only be called after authentication.");
			return null;
		}

		return mClient.GetUserImageUrl ();
	}

	public void IncrementAchievement (string achievementID, int steps, Action<bool> callback)
	{
		throw new System.NotImplementedException ();

	}

	public void SetStepsAtLeast (string achievementID, int steps, Action<bool> callback)
	{
		throw new System.NotImplementedException ();

	}

	public void ReportScore (long score, string board, string metadata, Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadScores (string leaderboardId, LeaderboardStart start,
	                        int rowCount, LeaderboardCollection collection,
	                        LeaderboardTimeSpan timeSpan,
	                        Action<LeaderboardScoreData> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadMoreScores (ScorePageToken token, int rowCount,
	                            Action<LeaderboardScoreData> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void ShowAchievementsUI (Action<UIStatus> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void ShowLeaderboardUI (string lbId)
	{
		if (lbId != null) {
			lbId = MapId (lbId);
		}

		mClient.ShowLeaderboardUI (lbId, LeaderboardTimeSpan.AllTime, null);
	}

	public void ShowLeaderboardUI (string lbId, Action<UIStatus> callback)
	{
		ShowLeaderboardUI (lbId, LeaderboardTimeSpan.AllTime, callback);
	}

	public void ShowLeaderboardUI (string lbId, LeaderboardTimeSpan span,
	                               Action<UIStatus> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void SetDefaultLeaderboardForUI (string lbid)
	{
		throw new System.NotImplementedException ();
	}

	public void RegisterInvitationDelegate (InvitationReceivedDelegate deleg)
	{
		mClient.RegisterInvitationDelegate (deleg);
	}

	private string MapId (string id)
	{
		throw new System.NotImplementedException ();
	}

	public IUserProfile[] GetFriends ()
	{
		if (!IsAuthenticated ()) {
			Debug.Log ("Cannot get friends when not authenticated!");
			return new IUserProfile[0];
		}

		return mClient.GetFriends ();
	}

	public string GetToken ()
	{
		return mClient.GetToken ();
	}

	#endregion
}
