using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class Shop_Control : MonoBehaviour, IStoreListener
{
	private enum intialize_state_t
	{
		unknown = 0,
		succeed = 1,
		failed = 2
	}

	private static IStoreController m_StoreController;

	private static IExtensionProvider m_StoreExtensionProvider;

	public static string kProductIDConsumable = "30gems";

	public static string kProductIDConsumableB = "60gems";

	public static Shop_Control Instance;

	private intialize_state_t initialize_state;

	private bool first;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if (this == Instance)
		{
			if (m_StoreController == null)
			{
				InitializePurchasing();
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public void InitializePurchasing()
	{
		if (!IsInitialized())
		{
			ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			configurationBuilder.AddProduct(kProductIDConsumable, ProductType.Consumable);
			configurationBuilder.AddProduct(kProductIDConsumableB, ProductType.Consumable);
			UnityPurchasing.Initialize(this, configurationBuilder);
		}
	}

	private bool IsInitialized()
	{
		if (m_StoreController != null)
		{
			return m_StoreExtensionProvider != null;
		}
		return false;
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		initialize_state = intialize_state_t.succeed;
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
		if (curr_scene_name() == "Game")
		{
			TransferStateInfo();
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		initialize_state = intialize_state_t.failed;
		if (curr_scene_name() == "Game")
		{
			TransferStateInfo();
		}
	}

	public void TransferStateInfo()
	{
		if (initialize_state == intialize_state_t.succeed)
		{
			Shop_positioner.Instance.shop_finished_loading();
		}
		else if (initialize_state == intialize_state_t.failed)
		{
			Shop_positioner.Instance.shop_failed_loading();
		}
	}

	public void Buy30gems()
	{
		if (Application.systemLanguage != SystemLanguage.Chinese && Application.systemLanguage != SystemLanguage.ChineseSimplified && Application.systemLanguage != SystemLanguage.ChineseTraditional)
		{
			BuyProductID(kProductIDConsumable);
		}
	}

	public void Buy60gems()
	{
		if (Application.systemLanguage != SystemLanguage.Chinese && Application.systemLanguage != SystemLanguage.ChineseSimplified && Application.systemLanguage != SystemLanguage.ChineseTraditional)
		{
			BuyProductID(kProductIDConsumableB);
		}
	}

	private void BuyProductID(string productId)
	{
		if (IsInitialized())
		{
			Product product = m_StoreController.products.WithID(productId);
			if (product != null && product.availableToPurchase)
			{
				m_StoreController.InitiatePurchase(product);
			}
			else
			{
				Shop_positioner.Instance.failed_purchase();
			}
		}
		else
		{
			Shop_positioner.Instance.failed_purchase();
		}
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Shop_positioner.Instance.failed_purchase();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		if (string.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
		{
			int num = 30;
			PlayerData.Instance.SetGlobalInt("GEMS", PlayerData.Instance.GetGlobalInt("GEMS") + num);
			Shop_positioner.Instance.succeded_purchase(num);
		}
		else if (string.Equals(args.purchasedProduct.definition.id, kProductIDConsumableB, StringComparison.Ordinal))
		{
			int num2 = 60;
			PlayerData.Instance.SetGlobalInt("GEMS", PlayerData.Instance.GetGlobalInt("GEMS") + num2);
			Shop_positioner.Instance.succeded_purchase(num2);
		}
		else
		{
			Shop_positioner.Instance.failed_purchase();
		}
		return PurchaseProcessingResult.Complete;
	}

	public void RestorePurchases()
	{
	}

	private string curr_scene_name()
	{
		return SceneManager.GetActiveScene().name;
	}
}
