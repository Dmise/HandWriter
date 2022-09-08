namespace Engine.OrLog.Models.Hand
{
    public class SeatData
    {
        public int Index { get; set; } // положение за столом
        public int Uid { get; set; } //user ID
        public string Name { get; set; } = "No username";
        public decimal Stack { get; set; }
        public List<CardData> HandCards { get; set; }
        public decimal Collected { get; set; }//размер выигрыша
        public bool Show { get; set; }//флаг о том, показал ли игрок карты
          
        //dmise added fields

        public SeatData()
        {
            HandCards = new List<CardData>();
        }
    }
    }

   
