//»щем public long Money { get; set; } =
//под ним вставл€ем:
public long CreditMoney { get; set; } = 0;

//»щем public DateTime Unwarn { get; set; } = DateTime.Now;
//под ним вставл€ем:
public DateTime CreditTime { get; set; } = DateTime.Now;
public int AllowCredit = 0;