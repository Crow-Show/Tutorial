using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.Write("Введите email: ");
bool emailInput = (Console.ReadLine() == "Email");

Console.Write("Введите пароль: ");
bool passwordInput = (Console.ReadLine() == "Пароль");

Console.Write("Заблокирован ли аккаунт: ");
bool bannedInput = (Console.ReadLine() == "Да");

Console.Write("Активна ли подписка: ");
bool subInput = (Console.ReadLine() == "Да");

Console.Write("Есть ли VIP доступ: ");
bool vipInput = (Console.ReadLine() == "Да");

Console.Write("Есть ли Админ доступ: ");
bool adminInput = (Console.ReadLine() == "Да"); 

Console.Write("Введите ключ: ");
bool keyInput = (Console.ReadLine() == "Ключ");

Console.Clear();

//Первичные свойства и доступ на сайт
bool emailConfirmed = emailInput;                                                     
bool knowsPassword = passwordInput;                                                    
bool isBanned = bannedInput;                                                            

bool accessDocumentVault = knowsPassword && emailConfirmed && !isBanned;

//Свойства, теребующие входа
bool isAdmin = adminInput && accessDocumentVault;
bool hasKey = keyInput && isAdmin;
bool subscriptionActive = subInput && accessDocumentVault;
bool isVIP = vipInput && subscriptionActive;

//Вторичные доступы
bool accessAdminPanel = isAdmin;
bool accessPremiumContent = subscriptionActive || isAdmin; 
bool accessBetaFeatures = isAdmin || isVIP; 
bool accessConfidentialReports = hasKey; 

Console.WriteLine($@"Доступ на сайт: {accessDocumentVault}
Премиум доступ: {accessPremiumContent}
Права администратора: {accessAdminPanel}
Доступ к тестированию: {accessBetaFeatures}
Доступ к данным пользователей: {accessConfidentialReports}");

Console.ReadKey();