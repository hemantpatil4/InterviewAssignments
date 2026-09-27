public abstract class Account
{
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }

    public Account(string accNo, decimal balance)
    {
        AccountNumber = accNo;
        Balance = balance;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        System.Console.WriteLine($"Deposited: {amount}");
    }

    public abstract bool Withdraw(decimal amount);

}

public class SavingsAccount : Account
{
    private decimal MinimumBalance = 1000;
    public SavingsAccount(string accNo, decimal balance) : base(accNo, balance)
    {

    }

    public override bool Withdraw(decimal amount)
    {
        if (Balance - amount < MinimumBalance)
        {
            System.Console.WriteLine($"Can't Withdraw");
            return false;
        }

        Balance -= amount;
        System.Console.WriteLine("dONE");
        return true;
    }

}

public class CurrentAccount : Account
{
    public CurrentAccount(string accNo, decimal balance) : base(accNo, balance)
    {

    }

    public override bool Withdraw(decimal amount)
    {
        if (amount > Balance)
        {
            System.Console.WriteLine($"Can't Withdraw");
            return false;
        }

        Balance -= amount;
        return true;
    }

}

public class Customer()
{
    public string? Name { get; set; }
    public List<Account> CustomerAccounts = new List<Account>();

}



public class Program()
{

    public static bool ProcessWithdrawTransaction(Customer customer, string AccountNumber, decimal Amount)
    {
        Account account = customer.CustomerAccounts.Where(x => x.AccountNumber.Equals(AccountNumber)).First();
        bool isSuccess = account.Withdraw(Amount);
        return isSuccess;
    }
    public static void ProcessDepositTransaction(Customer customer, string AccountNumber, decimal Amount)
    {
        Account account = customer.CustomerAccounts.Where(x => x.AccountNumber.Equals(AccountNumber)).First();
        account.Deposit(Amount);
    }

    public static void TransferAmounts(Customer customer, string FromAccountNumber, string ToAccountNumber, decimal TransferredAmount)
    {
        Account FromAccount = customer.CustomerAccounts.Where(x => x.AccountNumber.Equals(FromAccountNumber)).First();
        Account ToAccount = customer.CustomerAccounts.Where(x => x.AccountNumber.Equals(ToAccountNumber)).First();

        if (FromAccount.Withdraw(TransferredAmount))
        {
            ToAccount.Deposit(TransferredAmount);
        }
    }
    public static void Main()
    {
        Customer hemant = new Customer();
        hemant.Name = "Hemant";
        List<Account> accountList = new List<Account>();
        Account savings = new SavingsAccount("S001", 5000);
        Account current = new CurrentAccount("C001", 2000);
        accountList.Add(savings);
        accountList.Add(current);
        hemant.CustomerAccounts = accountList;

        // ProcessWithdrawTransaction(hemant, "S001", 4100);
        // ProcessWithdrawTransaction(hemant, "C001", 1900);
        // ProcessWithdrawTransaction(hemant, "C001", 200);

        Account FromAccount = hemant.CustomerAccounts.Where(x => x.AccountNumber.Equals("S001")).First();
        Account ToAccount = hemant.CustomerAccounts.Where(x => x.AccountNumber.Equals("C001")).First();

        System.Console.WriteLine($"FromAccount Balance: {FromAccount.Balance}");
        System.Console.WriteLine($"ToAccount Balance: {ToAccount.Balance}");

        TransferAmounts(hemant, "S001", "C001", 1000);

        System.Console.WriteLine($"FromAccount Balance: {FromAccount.Balance}");
        System.Console.WriteLine($"ToAccount Balance: {ToAccount.Balance}");
    }
}




