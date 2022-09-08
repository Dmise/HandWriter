namespace Engine.OrLog.Models.Hand
{
    public class SeatDataSuprema: SeatData
    {
        public decimal Pot { get; set; }

        public Round ActionRound = Round.Undefined;
        public ActionType Role { get; set; } = ActionType.Undefined;

        public string Pattern = "";
        public List<CardData> LightCards = new List<CardData>();
        public SeatDataSuprema() { }
        public SeatDataSuprema(Round roundInit)
        {

            ActionRound = roundInit;
        }
    }
}
