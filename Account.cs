namespace SimpleATM.UI.Models;

public class Account : IEquatable<Account?>
{
    private readonly string _pin;
    private readonly string _cardNumber;
    private decimal _balance = 0;
    private readonly List<Transaction> _transactions = new();
    private (DateTime, int) _todaysTransactions = (DateTime.Now.Date, 0);

    public string UserName { get; }

    public Account(string pin, string cardNumber, string name)
    {
        _pin = pin;
        _cardNumber = cardNumber;
        UserName = name;
    }

    private void AddTransaction(Operation operation, decimal amount)
    {
        Transaction transaction;
        transaction.amount = amount;
        transaction.operation = operation;
        transaction.date = DateTime.Now;

        _transactions.Add(transaction);

        if (operation == Operation.Withdraw)
        {
            var (today, _) = _todaysTransactions;

            if (today != DateTime.Now.Date)
            {
                _todaysTransactions = (DateTime.Now.Date, 1);
            }
        }
    }

    public bool ValidateCredentials(string cardNumber, string pin)
    {
        if (cardNumber != _cardNumber)
        {
            return false;
        }

        return pin == _pin;
    }

    public bool MakeDeposit(decimal amount)
    {
        _balance += amount;
        AddTransaction(Operation.Deposit, amount);

        return true;
    }

    public bool MakeWithdraw(decimal amount)
    {
        // Maximum 10 transactions per day
        if (_todaysTransactions.Item2 >= 10)
        {
            return false;
        }

        // Verify there's enough balance for withdraw
        if ((_balance - amount) >= 0 && amount <= 1000)
        {
            _balance -= amount;
            AddTransaction(Operation.Withdraw, amount);
            return true;
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Account);
    }

    public bool Equals(Account? other)
    {
        return other is not null &&
               _cardNumber == other._cardNumber;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_cardNumber);
    }

    enum Operation
    {
        Deposit,
        Withdraw
    }

    struct Transaction
    {
        public Operation operation;
        public decimal amount;
        public DateTime date;
    }


}


