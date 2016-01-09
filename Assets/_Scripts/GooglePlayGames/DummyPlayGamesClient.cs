using UnityEngine;
using System.Collections;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public class DummyPlayGamesClient : IPlayGamesClient
{

	#region IPlayGamesClient implementation

	bool authenticated;
	IRealTimeMultiplayerClient mRealTimeClient;

	public void Authenticate (System.Action<bool> callback, bool silent)
	{
		mRealTimeClient = new DummyRealTimeMultiplayerClient ();
		SceneMaster.instance.Async (() => {
			authenticated = !Input.GetKey (KeyCode.F);
			if (callback != null) {
				callback.Invoke (authenticated);
			}
		}, Utils.DUMMY_PLAY_GAMES_ASYNC_DELAY);
	}

	public bool IsAuthenticated ()
	{
		return authenticated;
	}

	public void SignOut ()
	{
		authenticated = false;
		mRealTimeClient = null;
	}

	public string GetToken ()
	{
		return "DummyToken";
	}

	public string GetUserId ()
	{
		return "ford1978";
	}

	public void LoadFriends (System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public string GetUserDisplayName ()
	{
		return "Tricia McMillan";
	}

	public string GetAccessToken ()
	{
		return "DummyAccessToken";
	}

	public string GetIdToken ()
	{
		return "DummyIdToken";
	}

	public string GetUserEmail ()
	{
		return "ford@example.com";
	}

	public string GetUserImageUrl ()
	{
		return null;
	}

	public void GetPlayerStats (System.Action<CommonStatusCodes, GooglePlayGames.PlayGamesLocalUser.PlayerStats> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadUsers (string[] userIds, System.Action<UnityEngine.SocialPlatforms.IUserProfile[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public Achievement GetAchievement (string achievementId)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadAchievements (System.Action<Achievement[]> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void UnlockAchievement (string achievementId, System.Action<bool> successOrFailureCalllback)
	{
		throw new System.NotImplementedException ();
	}

	public void RevealAchievement (string achievementId, System.Action<bool> successOrFailureCalllback)
	{
		throw new System.NotImplementedException ();
	}

	public void IncrementAchievement (string achievementId, int steps, System.Action<bool> successOrFailureCalllback)
	{
		throw new System.NotImplementedException ();
	}

	public void SetStepsAtLeast (string achId, int steps, System.Action<bool> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void ShowAchievementsUI (System.Action<UIStatus> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void ShowLeaderboardUI (string leaderboardId, LeaderboardTimeSpan span, System.Action<UIStatus> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadScores (string leaderboardId, LeaderboardStart start, int rowCount, LeaderboardCollection collection, LeaderboardTimeSpan timeSpan, System.Action<LeaderboardScoreData> callback)
	{
		throw new System.NotImplementedException ();
	}

	public void LoadMoreScores (ScorePageToken token, int rowCount, System.Action<LeaderboardScoreData> callback)
	{
		throw new System.NotImplementedException ();
	}

	public int LeaderboardMaxResults ()
	{
		throw new System.NotImplementedException ();
	}

	public void SubmitScore (string leaderboardId, long score, System.Action<bool> successOrFailureCalllback)
	{
		throw new System.NotImplementedException ();
	}

	public void SubmitScore (string leaderboardId, long score, string metadata, System.Action<bool> successOrFailureCalllback)
	{
		throw new System.NotImplementedException ();
	}

	public GooglePlayGames.BasicApi.Multiplayer.IRealTimeMultiplayerClient GetRtmpClient ()
	{
		return mRealTimeClient;
	}

	public GooglePlayGames.BasicApi.Multiplayer.ITurnBasedMultiplayerClient GetTbmpClient ()
	{
		throw new System.NotImplementedException ();
	}

	public GooglePlayGames.BasicApi.SavedGame.ISavedGameClient GetSavedGameClient ()
	{
		throw new System.NotImplementedException ();
	}

	public GooglePlayGames.BasicApi.Events.IEventsClient GetEventsClient ()
	{
		throw new System.NotImplementedException ();
	}

	public GooglePlayGames.BasicApi.Quests.IQuestsClient GetQuestsClient ()
	{
		throw new System.NotImplementedException ();
	}

	public void RegisterInvitationDelegate (InvitationReceivedDelegate invitationDelegate)
	{
		throw new System.NotImplementedException ();
	}

	public UnityEngine.SocialPlatforms.IUserProfile[] GetFriends ()
	{
		throw new System.NotImplementedException ();
	}

	public System.IntPtr GetApiClient ()
	{
		throw new System.NotImplementedException ();
	}

	#endregion

}
