using System;

namespace SqlDataProvider.Data;

public class ShopItemInfo
{
	public int ID { get; set; }

	public int ShopID { get; set; }

	public int GroupID { get; set; }

	public int TemplateID { get; set; }

	public int BuyType { get; set; }

	public bool IsContinue { get; set; }

	public int IsBind { get; set; }

	public int LimitGrade { get; set; }

	public int Label { get; set; }

	public decimal Beat { get; set; }

	public int AUnit { get; set; }

	public int APrice1 { get; set; }

	public int AValue1 { get; set; }

	public int APrice2 { get; set; }

	public int AValue2 { get; set; }

	public int APrice3 { get; set; }

	public int AValue3 { get; set; }

	public int BUnit { get; set; }

	public int BPrice1 { get; set; }

	public int BValue1 { get; set; }

	public int BPrice2 { get; set; }

	public int BValue2 { get; set; }

	public int BPrice3 { get; set; }

	public int BValue3 { get; set; }

	public int CUnit { get; set; }

	public int CPrice1 { get; set; }

	public int CValue1 { get; set; }

	public int CPrice2 { get; set; }

	public int CValue2 { get; set; }

	public int CPrice3 { get; set; }

	public int CValue3 { get; set; }

	public bool IsCheap { get; set; }

	public int LimitCount { get; set; }

	public DateTime StartDate { get; set; }

	public DateTime EndDate { get; set; }

	public bool CanBuy { get; set; }

	public int LimitPersonalCount { get; set; }
}
