
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using FusePlatformAPI = FuseAPI;
#elif UNITY_IPHONE
using FusePlatformAPI = FuseAPI_iOS;
#elif UNITY_ANDROID
using FusePlatformAPI = FuseAPI_Android;
#else
using FusePlatformAPI = FuseAPI;
#endif

public class FuseAPI : MonoBehaviour
{
	enum Errors
	{
    	NONE = 0,            /// no error has occurred
    	NOT_CONNECTED,       /// the user is not connected to the internet
    	REQUEST_FAILED,      /// there was an error in establishing a connection with the server
    	XML_PARSE_ERROR,     /// data was received, but there was a problem parsing the xml
	};
	
#region Session Creation

	public static void StartSession(string gameId)
	{
		FusePlatformAPI.StartSession(gameId);
	}
	
	public static event Action SessionStartReceived;
	public static event Action<int> SessionLoginError; 
	
#endregion
	
#region Analytics Event
	
	public static void RegisterEvent(string message)
	{
		FusePlatformAPI.RegisterEvent(message);
	}
	
#endregion
	
#region In-App Purchase Logging
	
	public struct Product
	{
		public string productId;
		public string priceLocale;
		public float price;
	}
	
	#if !UNITY_ANDROID
	public static void RegisterInAppPurchaseList(Product[] products)
	{
		FusePlatformAPI.RegisterInAppPurchaseList(products);
	}
	#endif
	
	#if UNITY_ANDROID && !UNITY_EDITOR
	public enum PurchaseState { PURCHASED, CANCELED, REFUNDED }

	public static void RegisterInAppPurchase(PurchaseState purchaseState, string notifyId, string productId, string orderId, DateTime purchaseTime, string developerPayload)
	{
		FusePlatformAPI.RegisterInAppPurchase(purchaseState, notifyId, productId, orderId, purchaseTime, developerPayload);
	}

	public static void RegisterInAppPurchase(PurchaseState purchaseState, string notifyId, string productId, string orderId, DateTime purchaseTime, string developerPayload, double price, string currency)
	{
		FusePlatformAPI.RegisterInAppPurchase(purchaseState, notifyId, productId, orderId, purchaseTime, developerPayload, price, currency);
	}
	#else
	public enum TransactionState { PURCHASING, PURCHASED, FAILED, RESTORED }

	public static void RegisterInAppPurchase(string productId, byte[] transactionReceipt, TransactionState transactionState)
	{
		FusePlatformAPI.RegisterInAppPurchase(productId, transactionReceipt, transactionState);
	}
	#endif
	
	public static event Action<bool, string, string> PurchaseVerification;
#endregion
	
#region Fuse Interstitial Ads
	
	public static void ShowAd()
	{
		FusePlatformAPI.ShowAd();
	}
	
	public static event Action AdWillClose;
	public static event Action AdDisplayed;
	public static event Action AdClicked;
#endregion

#region Notifications

	public static void DisplayNotifications()
	{
		FusePlatformAPI.DisplayNotifications();
	}
	
	public static event Action<string> NotificationAction;
#endregion

#region More Games
	
	public static void DisplayMoreGames()
	{
		FusePlatformAPI.DisplayMoreGames();
	}
	
	public static event Action OverlayWillClose;
#endregion
	
#region Gender
	public enum Gender { UNKNOWN, MALE, FEMALE };
	
	public static void RegisterGender(Gender gender)
	{
		FusePlatformAPI.RegisterGender(gender);
	}
#endregion
	
#region Account Login
	public enum AccountType
	{
		NONE = 0,
		GAMECENTER = 1,
		FACEBOOK = 2,
		TWITTER = 3,
		OPENFEINT = 4,
		USER = 5,
	}
	
	public static void GameCenterLogin()
	{
		FusePlatformAPI.GameCenterLogin();
	}
	
	public static void FacebookLogin(string facebookId, string name, string accessToken)
	{
		FusePlatformAPI.FacebookLogin(facebookId, name, accessToken);
	}
	
	public static void FacebookLogin(string facebookId, string name, Gender gender, string accessToken)
	{
		FusePlatformAPI.FacebookLogin(facebookId, name, gender, accessToken);
	}
	
	public static void TwitterLogin(string twitterId)
	{
		FusePlatformAPI.TwitterLogin(twitterId);
	}
	
	public static void OpenFeintLogin(string openFeintId)
	{
		FusePlatformAPI.OpenFeintLogin(openFeintId);
	}
	
	public static void FuseLogin(string fuseId, string alias)
	{
		FusePlatformAPI.FuseLogin(fuseId, alias);
	}
	
	public static string GetOriginalAccountId()
	{
		return FusePlatformAPI.GetOriginalAccountId();
	}
	
	public static AccountType GetOriginalAccountType()
	{
		return FusePlatformAPI.GetOriginalAccountType();
	}
	
	public static event Action<AccountType, string> AccountLoginComplete;
#endregion
	
#region Miscellaneous
	
	public static int GamesPlayed()
	{
		return FusePlatformAPI.GamesPlayed();
	}
	
	public static string LibraryVersion()
	{
		return FusePlatformAPI.LibraryVersion();
	}

	public static bool Connected()
	{
		return FusePlatformAPI.Connected();
	}
	
	public static void TimeFromServer()
	{
		FusePlatformAPI.TimeFromServer();
	}
	
	public static event Action<DateTime> TimeUpdated;
	
	public static bool NotReadyToTerminate()
	{
		return FusePlatformAPI.NotReadyToTerminate();
	}
#endregion
	
#region Data Opt In/Out
	
	public static void EnableData(bool enable)
	{
		FusePlatformAPI.EnableData(enable);
	}
	
	public static bool DataEnabled()
	{
		return FusePlatformAPI.DataEnabled();
	}
#endregion
	
#region User Game Data
	
	public static int SetGameData(Hashtable data)
	{
		return FusePlatformAPI.SetGameData(data);
	}
	
	public static int SetGameData(string key, Hashtable data)
	{
		return FusePlatformAPI.SetGameData(key, data);
	}
	
	public static int SetGameData(string key, Hashtable data, bool isCollection)
	{
		return FusePlatformAPI.SetGameData(key, data, isCollection);
	}
	
	public static int SetGameData(string key, Hashtable data, bool isCollection, string fuseId)
	{
		return FusePlatformAPI.SetGameData(key, data, isCollection, fuseId);
	}
	
	public static int GetGameData(string[] keys)
	{
		return FusePlatformAPI.GetGameData(keys);
	}
	
	public static int GetGameData(string key, string[] keys)
	{
		return FusePlatformAPI.GetGameData(key, keys);
	}
	
	public static int GetFriendGameData(string fuseId, string[] keys)
	{
		return FusePlatformAPI.GetFriendGameData(fuseId, keys);
	}
	
	public static int GetFriendGameData(string fuseId, string key, string[] keys)
	{
		return FusePlatformAPI.GetFriendGameData(fuseId, key, keys);
	}
	
	public static event Action<int, int> GameDataError;
	public static event Action<int> GameDataSetAcknowledged;
	public static event Action<string, string, Hashtable, int> GameDataReceived;
	
	public static string GetFuseId()
	{
		return FusePlatformAPI.GetFuseId();
	}
	
	#endregion
	
	#region Friend List
	
	public static void UpdateFriendsListFromServer()
	{
		FusePlatformAPI.UpdateFriendsListFromServer();
	}
	
	public struct Friend
	{
		public string fuseId;
		public string accountId;
		public string alias;
		public bool pending;
	}
	
	public static event Action<List<Friend>> FriendsListUpdated;
	public static event Action<int> FriendsListError;
	
	public static List<Friend> GetFriendsList()
	{
		return FusePlatformAPI.GetFriendsList();
	}
	#endregion

	#region Chat List
	#endregion

#region User-to-User Push Notifications
	public static void UserPushNotification(string fuseId, string message)
	{
		FusePlatformAPI.UserPushNotification(fuseId, message);
	}
	
	public static void FriendsPushNotification(string message)
	{
		FusePlatformAPI.FriendsPushNotification(message);
	}
#endregion
	
#region Gifting
	
	public static void GetMailListFromServer()
	{
		FusePlatformAPI.GetMailListFromServer();
	}
	
	public static void GetMailListFriendFromServer(string fuseId)
	{
		FusePlatformAPI.GetMailListFriendFromServer(fuseId);
	}
	
	public struct Mail
	{
		public int messageId;
		public DateTime timestamp;
		public string alias;
		public string message;
		public int giftId;
		public string giftName;
		public int giftAmount;
	}
	
	public static event Action<List<Mail>, string> MailListReceived;
	public static event Action<int> MailListError;
	
	public static List<Mail> GetMailList(string fuseId)
	{
		return FusePlatformAPI.GetMailList(fuseId);
	}
	
	public static void SetMailAsReceived(int messageId)
	{
		FusePlatformAPI.SetMailAsReceived(messageId);
	}
	
	public static void SendMailWithGift(string fuseId, string message, int giftId, int giftAmount)
	{
		FusePlatformAPI.SendMailWithGift(fuseId, message, giftId, giftAmount);
	}
	
	public static void SendMail(string fuseId, string message)
	{
		FusePlatformAPI.SendMail(fuseId, message);
	}
	
	public static event Action<int, string> MailAcknowledged;
	public static event Action<int> MailError;
	
#endregion

#region Game Configuration Data

	public static string GetGameConfigurationValue(string key)
	{
		return FusePlatformAPI.GetGameConfigurationValue(key);
	}
	
	public static event Action GameConfigurationReceived;
#endregion
	
#region Internal Event Triggers
	static protected void OnSessionStartReceived()
	{
		if (SessionStartReceived != null)
		{
			SessionStartReceived();
		}
	}
	
	static protected void OnSessionLoginError(int error)
	{
		if (SessionLoginError != null)
		{
			SessionLoginError(error);
		}
	}
	
	static protected void OnPurchaseVerification(bool verified, string transactionId, string originalTransactionId)
	{
		if (PurchaseVerification != null)
		{
			PurchaseVerification(verified, transactionId, originalTransactionId);
		}
	}
	
	static protected void OnAdWillClose()
	{
		if (AdWillClose != null)
		{
			AdWillClose();
		}
	}
	
	static protected void OnAdDisplayed()
	{
		if (AdDisplayed != null)
		{
			AdDisplayed();
		}
	}

	static protected void OnAdClicked()
	{
		if (AdClicked != null)
		{
			AdClicked();
		}
	}

	static protected void OnNotificationAction(string action)
	{
		if (NotificationAction != null)
		{
			NotificationAction(action);
		}
	}
	
	static protected void OnOverlayWillClose()
	{
		if (OverlayWillClose != null)
		{
			OverlayWillClose();
		}
	}
	
	static protected void OnAccountLoginComplete(AccountType type, string accountId)
	{
		if (AccountLoginComplete != null)
		{
			AccountLoginComplete(type, accountId);
		}
	}
	
	static protected void OnTimeUpdated(DateTime time)
	{
		if (TimeUpdated != null)
		{
			TimeUpdated(time);
		}
	}
	
	static protected void OnGameDataError(int error, int requestId)
	{
		if (GameDataError != null)
		{
			GameDataError(error, requestId);
		}
	}
	
	static protected void OnGameDataSetAcknowledged(int requestId)
	{
		if (GameDataSetAcknowledged != null)
		{
			GameDataSetAcknowledged(requestId);
		}
	}
	
	static protected void OnGameDataReceived(string fuseId, string dataKey, Hashtable data, int requestId)
	{
		if (GameDataReceived != null)
		{
			GameDataReceived(fuseId, dataKey, data, requestId);
		}
	}
	
	static protected void OnFriendsListUpdated(List<Friend> friends)
	{
		if (FriendsListUpdated != null)
		{
			FriendsListUpdated(friends);
		}
	}
	
	static protected void OnFriendsListError(int error)
	{
		if (FriendsListError != null)
		{
			FriendsListError(error);
		}
	}
	
	static protected void OnMailListReceived(List<Mail> mailList, string mailFuseId)
	{
		if (MailListReceived != null)
		{
			MailListReceived(mailList, mailFuseId);
		}
	}
	
	static protected void OnMailListError(int error)
	{
		if (MailListError != null)
		{
			MailListError(error);
		}
	}
	
	static protected void OnMailAcknowledged(int messageId, string fuseId)
	{
		if (MailAcknowledged != null)
		{
			MailAcknowledged(messageId, fuseId);
		}
	}
	
	static protected void OnMailError(int error)
	{
		if (MailError != null)
		{
			MailError(error);
		}
	}

	static protected void OnGameConfigurationReceived()
	{
		if (GameConfigurationReceived != null)
		{
			GameConfigurationReceived();
		}
	}
	#endregion

	#region Conversions
	static public long DateTimeToTimestamp(DateTime dateTime)
	{
		return (dateTime - unixEpoch).Ticks / TimeSpan.TicksPerSecond;
	}

	static public DateTime TimestampToDateTime(long timestamp)
	{
		return unixEpoch + TimeSpan.FromTicks(timestamp * TimeSpan.TicksPerSecond);
	}

	private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	#endregion
}
