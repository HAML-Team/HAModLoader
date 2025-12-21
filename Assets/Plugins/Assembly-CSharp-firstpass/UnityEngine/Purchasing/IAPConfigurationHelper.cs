using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
	public static class IAPConfigurationHelper
	{
		public static void PopulateConfigurationBuilder(ref ConfigurationBuilder builder, ProductCatalog catalog)
		{
			foreach (ProductCatalogItem allValidProduct in catalog.allValidProducts)
			{
				IDs ds = null;
				if (allValidProduct.allStoreIDs.Count > 0)
				{
					ds = new IDs();
					foreach (StoreID allStoreID in allValidProduct.allStoreIDs)
					{
						ds.Add(allStoreID.id, allStoreID.store);
					}
				}
				List<PayoutDefinition> list = new List<PayoutDefinition>();
				foreach (ProductCatalogPayout payout in allValidProduct.Payouts)
				{
					list.Add(new PayoutDefinition(payout.typeString, payout.subtype, payout.quantity, payout.data));
				}
				builder.AddProduct(allValidProduct.id, allValidProduct.type, ds, list.ToArray());
			}
		}
	}
}
