using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

int score = 0;
int bonus = 5;
int penalty = 2;
int multiplicator = 2;

score += bonus;
score -= penalty;
score *= multiplicator;

Console.WriteLine($"Финальный счет: {score}");