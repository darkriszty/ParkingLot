namespace ParkingLot.Dtos
{
    public class FreeSpacesResponse
    {
        public FreeSpacesResponse(int freeSpaces)
        {
            NumberOfFreeSpaces = freeSpaces;
        }
        public int NumberOfFreeSpaces { get; set; }
    }
}
