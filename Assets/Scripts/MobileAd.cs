
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class MobileAd : MonoBehaviour
{
	//private RewardBasedVideoAd adReward;
	private RewardedAd rewardedAd;
	private InterstitialAd interstitial;
	private bool undoAd = false;

	private bool watchAdSuccess = false;

	//private string idApp, idReward;


	[SerializeField] GameObject buttonWatchAdInGame;
	[SerializeField] Button buttonWatchAdGameGameOver;

	public Survival2048.Z2048 z2048;

	private bool adCooldown; // cooldown flag
	public float adCooldownTime = 60.0f; // Timer
	private float adCooldownRemaining; // countdown
	// Countdown UI stuff
	public GameObject adCooldownRedCircle;
	public TextMeshProUGUI adCooldownText;
	


	void Start()
	{
		// Ad setup part
		string adUnitId;
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-7242052781619146/8110379919";
#elif UNITY_IOS
		adUnitId = "ca-app-pub-7242052781619146/1902110508";
#endif

		this.rewardedAd = new RewardedAd(adUnitId);

		// Called when an ad request has successfully loaded.
		this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		// Called when an ad request failed to load.
	//	this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		// Called when an ad is shown.
		this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
		// Called when an ad request failed to show.
		this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		// Called when the user should be rewarded for interacting with the ad.
		this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		// Called when the ad is closed.
		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the rewarded ad with the request.
		this.rewardedAd.LoadAd(request);

		RequestInterstitial();


		/*
		idApp = "ca-app-pub-7242052781619146~5628494721";
		idReward = "ca-app-pub-3940256099942544/5224354917";

		adReward = RewardBasedVideoAd.Instance;
		MobileAds.Initialize(idApp);
		*/

		// Game Part
		adCooldown = false;
		adCooldownText.fontSize = 20;
		adCooldownRemaining = adCooldownTime;
	}

	public string AndroidAdUnitID;
	public string IOSUnitID;
	private void RequestInterstitial()
	{
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7242052781619146/6090721233";
         adUnitId =AndroidAdUnitID;
#elif UNITY_IOS
		string adUnitId = "ca-app-pub-7242052781619146/4302980935";
		 adUnitId = IOSUnitID;
#endif

		// Initialize an InterstitialAd.
		this.interstitial = new InterstitialAd(adUnitId);

		// Called when an ad request has successfully loaded.
		this.interstitial.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		// Called when an ad is shown.
		this.interstitial.OnAdOpening += HandleOnAdOpened;
		// Called when the ad is closed.
		this.interstitial.OnAdClosed += HandleOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		//this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		this.interstitial.LoadAd(request);
	}

	#region Interestitial Ad Handlers
	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLoaded event received");
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
							+ args);
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdOpened event received");
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdClosed event received");
		RequestInterstitial();
	}

	public void HandleOnAdLeavingApplication(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleAdLeavingApplication event received");
	}
    #endregion

    void Update()
	{
		if (adCooldown)
		{
			float seconds = Mathf.CeilToInt(adCooldownRemaining);
			adCooldownText.text = seconds.ToString();
			if (adCooldownRemaining > 0)
			{
				adCooldownRemaining -= Time.deltaTime;
			}
			else
			{
				adCooldownRemaining = adCooldownTime;
				adCooldown = false;
				adCooldownRedCircle.SetActive(false);
				buttonWatchAdInGame.GetComponent<Button>().interactable = true;
			}
		}
		else if (z2048.adWatched == 0 && !buttonWatchAdInGame.activeInHierarchy)
		{
			buttonWatchAdInGame.SetActive(true);
		}
		if(watchAdSuccess)
        {
			StartCoroutine(Reward());
        }
	}

	IEnumerator Reward()
    {
		watchAdSuccess = false;
		yield return new WaitForSeconds(0.1f);
		if (undoAd)
		{
			z2048.IncrementUndoCount();
			adCooldownRedCircle.SetActive(true);
			adCooldown = true;

			z2048.adWatched++;
			if (z2048.adWatched >= 3)
			{
				adCooldownRemaining = adCooldownTime;
				adCooldown = false;
				adCooldownRedCircle.SetActive(false);
				buttonWatchAdInGame.GetComponent<Button>().interactable = true;
				buttonWatchAdInGame.SetActive(false);
			}
		}
		else
		{
			Debug.LogError("GameOver");
			z2048.GameOverUndo();
		}
	}

	public void ShowInteristitialAd()
    {
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
		}
	}

	public void RunInGameAd()
	{
		undoAd = true;
		// Allow to store a maximum of 3 undos by watching ads
		if (PlayerPrefs.GetInt("UndoCount") < 3 && buttonWatchAdInGame.GetComponent<Button>().interactable)
		{
			buttonWatchAdInGame.GetComponent<Button>().interactable = false;
			/*RequestRewardAd();*/
			// Show the ad
			if (rewardedAd.IsLoaded())
			{
				rewardedAd.Show();
			}
		}
	}
	public void RunGameOverAd()
	{
		undoAd = false;
		buttonWatchAdGameGameOver.interactable = false;

		/*RequestRewardAd();*/
		if (rewardedAd.IsLoaded())
		{
			rewardedAd.Show();
		}
		buttonWatchAdGameGameOver.interactable = true;

	}

	public void CreateAndLoadRewardedAd()
	{
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#else
		string adUnitId = "unexpected_platform";
#endif

		this.rewardedAd = new RewardedAd(adUnitId);

		this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the rewarded ad with the request.
		this.rewardedAd.LoadAd(request);
	}

	#region Rewarded Ad Handlers
	public void HandleRewardedAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdLoaded event received");
	}

	public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardedAdFailedToLoad event received with message: "
							 + args.Message);
	}

	public void HandleRewardedAdOpening(object sender, EventArgs args)
	{
		MonoBehaviour.print("HandleRewardedAdOpening event received");
	}

	public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardedAdFailedToShow event received with message: "
							 + args.Message);
	}

	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		CreateAndLoadRewardedAd();
	}

	public void HandleUserEarnedReward(object sender, Reward args)
	{
		// Watched AD successfully
		watchAdSuccess = true;
		
	}

    #endregion
	
    #region Reward video methods ---------------------------------------------
	/*
    public void RequestRewardAd()
	{
		AdRequest request = AdRequestBuild();
		adReward.LoadAd(request, idReward);

		adReward.OnAdLoaded += this.HandleOnRewardedAdLoaded;
		adReward.OnAdRewarded += this.HandleOnAdRewarded;
		adReward.OnAdClosed += this.HandleOnRewardedAdClosed;
	}

	public void ShowRewardAd()
	{
		if (adReward.IsLoaded())
			adReward.Show();
	}
	//events
	public void HandleOnRewardedAdLoaded(object sender, EventArgs args)
	{//ad loaded
		ShowRewardAd();

	}

	public void HandleOnAdRewarded(object sender, EventArgs args)
	{//user finished watching ad
		
	}

	public void HandleOnRewardedAdClosed(object sender, EventArgs args)
	{//ad closed (even if not finished watching)
		buttonWatchAdInGame.GetComponent<Button>().interactable = true;
		buttonWatchAdGameGameOver.interactable = true;
		

		adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
		adReward.OnAdRewarded -= this.HandleOnAdRewarded;
		adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
	}
	*/
	#endregion
	
	//other functions
	//btn (more points) clicked
	

	//------------------------------------------------------------------------
	/*
	AdRequest AdRequestBuild()
	{
		return new AdRequest.Builder().Build();
	}
	
	void OnDestroy()
	{
		adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
		adReward.OnAdRewarded -= this.HandleOnAdRewarded;
		adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
	}
	*/
}
