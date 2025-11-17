using Bank_SaDi.Models;

namespace Bank_SaDi.ViewModels
{
    public class ATMViewModel
    {
        public Account Account { get; set; }
        public User User { get; set; }
        public List<Movement> Movements { get; set; }

        public int AccountNumberL { get; set; }
    }
}
