namespace ParkingLot.Dtos
{
    public class PriceResponse
    {
        private readonly int _price;

        public PriceResponse(int price)
        {
            _price = price;
        }

        public string Price => $"${_price}";
    }
}
