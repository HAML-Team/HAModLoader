namespace UnityEngine.Purchasing
{
	[AddComponentMenu("")]
	public class DemoInventory : MonoBehaviour
	{
		public void Fulfill(string productId)
		{
			if (productId == "100.gold.coins")
			{
				Debug.Log("You Got Money!");
			}
			else
			{
				Debug.Log(string.Format("Unrecognized productId \"{0}\"", productId));
			}
		}
	}
}
